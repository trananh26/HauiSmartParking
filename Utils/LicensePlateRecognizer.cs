using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Tesseract;

namespace Auto_parking
{
    public class LicensePlateRecognizer : IDisposable
    {
        #region Private Fields

        private readonly string _tesseractDataPath;
        private readonly string _cascadePath;
        private const string LANG = "eng";

        private TesseractEngine _fullTesseract;
        private TesseractEngine _chTesseract;
        private TesseractEngine _numTesseract;
        private bool _disposed = false;

        #endregion

        #region Constructor & Disposal

        public LicensePlateRecognizer(string tesseractDataPath, string cascadePath)
        {
            _tesseractDataPath = tesseractDataPath ?? throw new ArgumentNullException(nameof(tesseractDataPath));
            _cascadePath = cascadePath ?? throw new ArgumentNullException(nameof(cascadePath));

            InitializeTesseract();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _fullTesseract?.Dispose();
                _chTesseract?.Dispose();
                _numTesseract?.Dispose();
            }

            _disposed = true;
        }

        #endregion

        #region Initialization

        private void InitializeTesseract()
        {
            try
            {
                _fullTesseract = new TesseractEngine(_tesseractDataPath, LANG, EngineMode.Default);
                _fullTesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTVXY1234567890");

                _chTesseract = new TesseractEngine(_tesseractDataPath, LANG, EngineMode.Default);
                _chTesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTUVXY");

                _numTesseract = new TesseractEngine(_tesseractDataPath, LANG, EngineMode.Default);
                _numTesseract.SetVariable("tessedit_char_whitelist", "1234567890");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi khởi tạo Tesseract OCR: " + ex.Message, ex);
            }
        }

        #endregion

        #region Public Methods

        public RecognitionResult RecognizeFromFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException(nameof(imagePath));

            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Không tìm thấy file ảnh", imagePath);

            try
            {
                using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                using (var img = Image.FromStream(fs))
                using (var bitmap = new Bitmap(img))
                {
                    return RecognizeFromBitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                return new RecognitionResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi khi xử lý ảnh: " + ex.Message
                };
            }
        }

        public RecognitionResult RecognizeFromBitmap(Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            try
            {
                using (var plateRegion = FindLicensePlateRegion(image))
                {
                    if (plateRegion == null)
                    {
                        return new RecognitionResult
                        {
                            Success = false,
                            ErrorMessage = "Không tìm thấy biển số trong ảnh"
                        };
                    }

                    using (var resized = plateRegion.Resize(400, 400, Inter.Linear))
                    using (var plateBitmap = resized.ToBitmap())
                    {
                        return ExtractAndRecognizeCharacters(plateBitmap);
                    }
                }
            }
            catch (Exception ex)
            {
                return new RecognitionResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi nhận diện: " + ex.Message
                };
            }
        }

        #endregion

        #region Private Methods - Plate Detection

        private Image<Bgr, byte> FindLicensePlateRegion(Bitmap image)
        {
            using (CascadeClassifier cascade = new CascadeClassifier(_cascadePath))
            {
                for (float angle = 0; angle <= 20; angle += 3)
                {
                    for (float sign = -1; sign <= 1 && sign + angle != 1; sign += 2)
                    {
                        var plateRegion = DetectPlateAtAngle(image, cascade, angle * sign);
                        if (plateRegion != null)
                            return plateRegion;
                    }
                }
            }

            return null;
        }

        private Image<Bgr, byte> DetectPlateAtAngle(Bitmap image, CascadeClassifier cascade, float angle)
        {
            using (var rotated = RotateImage(image, angle))
            using (var frame = rotated.ToBgrImage())
            using (var grayframe = rotated.ToGrayImage())
            {
                var faces = cascade.DetectMultiScale(
                                        grayframe,
                                        1.1,
                                        8,
                                        new Size(24, 24));

                if (faces.Length > 0)
                {
                    var bestFace = SelectBestPlateRegion(faces);
                    return frame.Copy(bestFace);
                }
            }

            return null;
        }

        private Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            PointF offset = new PointF((float)image.Width / 2, (float)image.Height / 2);
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                g.TranslateTransform(offset.X, offset.Y);
                g.RotateTransform(angle);
                g.TranslateTransform(-offset.X, -offset.Y);
                g.DrawImage(image, new PointF(0, 0));
            }

            return rotatedBmp;
        }

        private Rectangle SelectBestPlateRegion(Rectangle[] detectedRegions)
        {
            if (detectedRegions.Length == 1)
                return detectedRegions[0];

            double[] standardRatios = new double[]
                        {
                            330.0 / 165.0,
                            520.0 / 110.0,
                            190.0 / 140.0,
                            280.0 / 200.0,
                            470.0 / 110.0
                        };

            var filteredCandidates = FilterOverlappingRegions(detectedRegions);

            Rectangle bestRegion = filteredCandidates[0];
            double bestScore = CalculatePlateScore(bestRegion, standardRatios);

            foreach (Rectangle region in filteredCandidates.Skip(1))
            {
                double score = CalculatePlateScore(region, standardRatios);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestRegion = region;
                }
            }

            return bestRegion;
        }

        private List<Rectangle> FilterOverlappingRegions(Rectangle[] regions)
        {
            var filtered = new List<Rectangle>(regions);

            foreach (Rectangle candidate in regions)
            {
                foreach (Rectangle other in regions)
                {
                    if (candidate == other) continue;

                    if (IsRectangleContained(candidate, other))
                    {
                        filtered.Remove(other);
                        break;
                    }
                }
            }

            return filtered;
        }

        private bool IsRectangleContained(Rectangle inner, Rectangle outer)
        {
            return inner.X >= outer.X &&
                        inner.Y >= outer.Y &&
                        inner.Right <= outer.Right &&
                        inner.Bottom <= outer.Bottom &&
                        !(inner.X == outer.X && inner.Y == outer.Y &&
                        inner.Width == outer.Width && inner.Height == outer.Height);
        }

        private double CalculatePlateScore(Rectangle region, double[] standardRatios)
        {
            if (region.Width == 0 || region.Height == 0)
                return 0;

            double actualRatio = (double)region.Width / region.Height;
            double minDifference = double.MaxValue;
            int bestMatchIndex = 0;

            for (int i = 0; i < standardRatios.Length; i++)
            {
                double difference = Math.Abs(actualRatio - standardRatios[i]);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    bestMatchIndex = i;
                }
            }

            double ratioScore = 100.0 / (1.0 + minDifference * 5.0);
            double priorityBonus = (standardRatios.Length - bestMatchIndex) * 2.0;

            int area = region.Width * region.Height;
            double areaScore = 0;
            if (area >= 5000 && area <= 100000)
                areaScore = 10.0;
            else if (area >= 3000 && area <= 150000)
                areaScore = 5.0;

            return ratioScore + priorityBonus + areaScore;
        }

        #endregion

        #region Private Methods - Character Recognition

        private RecognitionResult ExtractAndRecognizeCharacters(Bitmap plateImage)
        {
            var con = new FindContours();

            int count = con.IdentifyContours(
                                plateImage,
                                50,
                                false,
                                out Bitmap grayframe,
                                out Bitmap colorframe,
                                out List<Rectangle> rectangles);

            try
            {
                if (rectangles == null || rectangles.Count == 0)
                {
                    return new RecognitionResult
                    {
                        Success = false,
                        ErrorMessage = "Không tìm thấy ký tự trên biển số"
                    };
                }

                using (Image<Gray, byte> grayImage = grayframe.ToGrayImage())
                using (Bitmap processedGray = grayImage.ToBitmap())
                {
                    FilterAndSortRectangles(
                        processedGray,
                        rectangles,
                        out List<Rectangle> upRow,
                        out List<Rectangle> downRow);

                    string upText = RecognizeRectangleList(processedGray, upRow, true);
                    string downText = RecognizeRectangleList(processedGray, downRow, false);

                    string fullText = upText;
                    if (!string.IsNullOrEmpty(downText))
                        fullText += "\r\n" + downText;

                    return new RecognitionResult
                    {
                        Success = true,
                        PlateNumber = fullText.Replace("\n", "").Replace("\r", ""),
                        FormattedText = fullText,
                        PlateImage = (Bitmap)plateImage.Clone(),
                        GrayImage = (Bitmap)grayframe.Clone(),
                        ColorImage = (Bitmap)colorframe.Clone(),
                        UpperCharacters = upRow,
                        LowerCharacters = downRow
                    };
                }
            }
            finally
            {
                grayframe?.Dispose();
                colorframe?.Dispose();
            }
        }

        private void FilterAndSortRectangles(
            Bitmap grayframe,
            List<Rectangle> listRect,
            out List<Rectangle> up,
            out List<Rectangle> down)
        {
            up = new List<Rectangle>();
            down = new List<Rectangle>();

            for (int i = listRect.Count - 1; i >= 0; i--)
            {
                if (IsInvalidRectangle(grayframe, listRect[i]))
                {
                    listRect.RemoveAt(i);
                }
            }

            if (listRect.Count == 0)
                return;

            int upY = 0, downY = 0;
            bool foundTwoRows = FindTwoRows(listRect, out upY, out downY);

            foreach (Rectangle rect in listRect)
            {
                if (foundTwoRows)
                {
                    if (Math.Abs(rect.Y - upY) < 50)
                        up.Add(rect);
                    else if (Math.Abs(rect.Y - downY) < 50)
                        down.Add(rect);
                }
                else
                {
                    down.Add(rect);
                }
            }

            up.Sort((a, b) => a.X.CompareTo(b.X));
            down.Sort((a, b) => a.X.CompareTo(b.X));
        }

        private bool IsInvalidRectangle(Bitmap grayframe, Rectangle rect)
        {
            using (Bitmap ch = grayframe.Clone(rect, grayframe.PixelFormat))
            {
                string temp = PerformOCR(ch, _fullTesseract);
                int erosionCount = 0;

                Bitmap workingBitmap = ch;
                while (temp.Length > 3 && erosionCount < 10)
                {
                    using (Image<Gray, byte> tempImg = workingBitmap.ToGrayImage())
                    using (Image<Gray, byte> eroded = tempImg.Erode(2))
                    {
                        workingBitmap = eroded.ToBitmap();
                        temp = PerformOCR(workingBitmap, _fullTesseract);
                    }
                    erosionCount++;
                }

                if (workingBitmap != ch)
                    workingBitmap.Dispose();

                return erosionCount > 10;
            }
        }

        private bool FindTwoRows(List<Rectangle> rects, out int upY, out int downY)
        {
            upY = 0;
            downY = 0;

            for (int i = 0; i < rects.Count; i++)
            {
                for (int j = i + 1; j < rects.Count; j++)
                {
                    int diff = Math.Abs(rects[i].Y - rects[j].Y);
                    if (diff > 100)
                    {
                        if (rects[i].Y < rects[j].Y)
                        {
                            upY = rects[i].Y;
                            downY = rects[j].Y;
                        }
                        else
                        {
                            upY = rects[j].Y;
                            downY = rects[i].Y;
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        private string RecognizeRectangleList(Bitmap grayframe, List<Rectangle> rects, bool isUpperRow)
        {
            string result = "";

            for (int i = 0; i < rects.Count; i++)
            {
                using (Bitmap charImage = grayframe.Clone(rects[i], grayframe.PixelFormat))
                {
                    string character = RecognizeCharacter(charImage, isUpperRow, i);
                    result += character;
                }
            }

            return result;
        }

        private string RecognizeCharacter(Bitmap charImage, bool isUpperRow, int position)
        {
            if (isUpperRow && position < 2)
            {
                return Ocr(charImage, false, true);
            }
            else if (isUpperRow)
            {
                return Ocr(charImage, false, false);
            }
            else
            {
                return Ocr(charImage, false, true);
            }
        }

        private string Ocr(Bitmap image, bool isFull, bool isNum = false)
        {
            using (Image<Gray, byte> src = image.ToGrayImage())
            {
                int nonZeroCount = CountNonZero(src);
                Image<Gray, byte> processed = src;

                while (true)
                {
                    var ratio = (double)nonZeroCount / (src.Width * src.Height);
                    if (ratio > 0.5) break;

                    var dilated = processed.Dilate(2);
                    if (processed != src) processed.Dispose();
                    processed = dilated;

                    nonZeroCount = CountNonZero(processed);
                }

                using (Bitmap processedBitmap = processed.ToBitmap())
                {
                    TesseractEngine ocr = isFull ? _fullTesseract : (isNum ? _numTesseract : _chTesseract);
                    string result = PerformOCR(processedBitmap, ocr);

                    if (processed != src) processed.Dispose();

                    return result;
                }
            }
        }

        private int CountNonZero(Image<Gray, byte> src)
        {
            using (Mat srcMat = src.Mat)
            using (Mat mask = new Mat())
            {
                Mat zeroMat = new Mat(srcMat.Size, srcMat.Depth, srcMat.NumberOfChannels);
                zeroMat.SetTo(new MCvScalar(0));
                CvInvoke.Compare(srcMat, zeroMat, mask, CmpType.NotEqual);
                int count = CvInvoke.CountNonZero(mask);
                zeroMat.Dispose();
                return count;
            }
        }

        private string PerformOCR(Bitmap image, TesseractEngine ocr)
        {
            string result = "";

            try
            {
                using (Pix pix = PixConverter.ToPix(image))
                using (Page page = ocr.Process(pix))
                {
                    result = page.GetText().Trim();
                }

                Bitmap workingImage = image;
                int count = 0;

                while (result.Length > 3 && count < 10)
                {
                    using (Image<Gray, byte> temp = workingImage.ToGrayImage())
                    using (Image<Gray, byte> eroded = temp.Erode(2))
                    {
                        if (workingImage != image)
                            workingImage.Dispose();

                        workingImage = eroded.ToBitmap();
                    }

                    using (Pix pix = PixConverter.ToPix(workingImage))
                    using (Page page = ocr.Process(pix))
                    {
                        result = page.GetText().Trim();
                    }

                    count++;
                }

                if (workingImage != image)
                    workingImage.Dispose();
            }
            catch (Exception)
            {
                result = "";
            }

            return result;
        }

        #endregion
    }

    #region Result Class

    public class RecognitionResult : IDisposable
    {
        public bool Success { get; set; }
        public string PlateNumber { get; set; }
        public string FormattedText { get; set; }
        public string ErrorMessage { get; set; }

        public Bitmap PlateImage { get; set; }
        public Bitmap GrayImage { get; set; }
        public Bitmap ColorImage { get; set; }

        public List<Rectangle> UpperCharacters { get; set; }
        public List<Rectangle> LowerCharacters { get; set; }

        public void Dispose()
        {
            PlateImage?.Dispose();
            GrayImage?.Dispose();
            ColorImage?.Dispose();
        }
    }

    #endregion
}
