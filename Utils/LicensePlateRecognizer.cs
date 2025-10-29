using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
                _fullTesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTVXY0123456789");

                _chTesseract = new TesseractEngine(_tesseractDataPath, LANG, EngineMode.Default);
                _chTesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTUVXY");

                _numTesseract = new TesseractEngine(_tesseractDataPath, LANG, EngineMode.Default);
                _numTesseract.SetVariable("tessedit_char_whitelist", "0123456789");
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
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                using (Image img = Image.FromStream(fs))
                using (Bitmap bitmap = new Bitmap(img))
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
                using (Image<Bgr, byte> plateRegion = FindLicensePlateRegion(image))
                {
                    if (plateRegion == null)
                    {
                        return new RecognitionResult
                        {
                            Success = false,
                            ErrorMessage = "Không tìm thấy biển số trong ảnh"
                        };
                    }

                    using (Image<Bgr, byte> resized = plateRegion.Resize(400, 400, Inter.Linear))
                    using (Bitmap plateBitmap = resized.ToBitmap())
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
                        Image<Bgr, byte> plateRegion = DetectPlateAtAngle(image, cascade, angle * sign);
                        if (plateRegion != null)
                            return plateRegion;
                    }
                }
            }

            return null;
        }

        private Image<Bgr, byte> DetectPlateAtAngle(Bitmap image, CascadeClassifier cascade, float angle)
        {
            using (Bitmap rotated = RotateImage(image, angle))
            using (Image<Bgr, byte> frame = rotated.ToBgrImage())
            using (Image<Gray, byte> grayframe = rotated.ToGrayImage())
            {
                Rectangle[] faces = cascade.DetectMultiScale(
                                        grayframe,
                                        1.1,
                                        8,
                                        new Size(24, 24));

                if (faces.Length > 0)
                {
                    Rectangle bestFace = SelectBestPlateRegion(faces);
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

            List<Rectangle> filteredCandidates = FilterOverlappingRegions(detectedRegions);

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
            List<Rectangle> filtered = new List<Rectangle>(regions);

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
            FindContours con = new FindContours();

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

                    var upText = RecognizeRectangleList(processedGray, upRow, true);
                    var downText = RecognizeRectangleList(processedGray, downRow, false);

                    string fullText = upText.Item1;
                    if (!string.IsNullOrEmpty(downText.Item1))
                        fullText += "\r\n" + downText.Item1;

                    return new RecognitionResult
                    {
                        Success = true,
                        PlateNumber = fullText.Replace("\n", "").Replace("\r", ""),
                        FormattedText = fullText,
                        PlateImage = (Bitmap)plateImage.Clone(),
                        GrayImage = (Bitmap)grayframe.Clone(),
                        ColorImage = (Bitmap)colorframe.Clone(),
                        UpperCharacters = upRow,
                        LowerCharacters = downRow,
                        CharImages = upText.Item2.Concat(downText.Item2).ToList()
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

            // Remove invalid rectangles in reverse order to avoid index issues
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

                // Early exit if OCR already succeeded
                if (temp.Length <= 3)
                    return false;

                Image<Gray, byte> workingImage = ch.ToGrayImage();
                try
                {
                    while (temp.Length > 3 && erosionCount < 10)
                    {
                        using (Image<Gray, byte> eroded = workingImage.Erode(2))
                        {
                            if (workingImage != null && workingImage.Equals(ch.ToGrayImage()) == false)
                                workingImage.Dispose();

                            workingImage = eroded.Clone();

                            using (Bitmap erodedBitmap = eroded.ToBitmap())
                            {
                                temp = PerformOCR(erodedBitmap, _fullTesseract);
                            }
                        }
                        erosionCount++;
                    }

                    return erosionCount >= 10;
                }
                finally
                {
                    workingImage?.Dispose();
                }
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

        private (string, List<Bitmap>) RecognizeRectangleList(Bitmap grayframe, List<Rectangle> rects, bool isUpperRow)
        {
            string result = "";
            List<Bitmap> charImages = new List<Bitmap>();

            for (int i = 0; i < rects.Count; i++)
            {
                using (Bitmap charImage = grayframe.Clone(rects[i], grayframe.PixelFormat))
                {
                    charImages.Add((Bitmap)charImage.Clone());

                    // Determine OCR engine based on position and row type
                    bool useNumericEngine = ShouldUseNumericEngine(isUpperRow, i);
                    string character = Ocr(charImage, useNumericEngine);
                    result += character;
                }
            }

            return (result, charImages);
        }

        private bool ShouldUseNumericEngine(bool isUpperRow, int position)
        {
            // For upper row: use numeric engine for positions 0-1, letter engine for others
            // For lower row: use numeric engine
            if (!isUpperRow)
                return true;

            return position < 2;
        }

        private string Ocr(Bitmap image, bool useNumericEngine)
        {
            // Use pre-initialized Tesseract engines
            TesseractEngine targetEngine = useNumericEngine ? _numTesseract : _chTesseract;

            // First attempt: direct OCR on original image
            string result = PerformOCR(image, targetEngine);

            // If result is empty or too long, try with image enhancement
            if (string.IsNullOrEmpty(result) || result.Length > 3)
            {
                result = PerformOCRWithEnhancement(image, targetEngine);
            }

            return result;
        }

        private string PerformOCRWithEnhancement(Bitmap image, TesseractEngine ocr)
        {
            using (Image<Gray, byte> src = image.ToGrayImage())
            {
                int nonZeroCount = CountNonZero(src);
                Image<Gray, byte> processed = src;
                bool shouldDispose = false;

                try
                {
                    // Dilate image if pixel density is too low
                    while (true)
                    {
                        double ratio = (double)nonZeroCount / (src.Width * src.Height);
                        if (ratio > 0.5)
                            break;

                        Image<Gray, byte> dilated = processed.Dilate(2);
                        if (shouldDispose)
                            processed.Dispose();

                        processed = dilated;
                        shouldDispose = true;
                        nonZeroCount = CountNonZero(processed);
                    }

                    using (Bitmap processedBitmap = processed.ToBitmap())
                    {
                        return PerformOCR(processedBitmap, ocr);
                    }
                }
                finally
                {
                    if (shouldDispose)
                        processed.Dispose();
                }
            }
        }

        private int CountNonZero(Image<Gray, byte> src)
        {
            using (Mat srcMat = src.Mat)
            using (Mat mask = new Mat())
            using (Mat zeroMat = new Mat(srcMat.Size, srcMat.Depth, srcMat.NumberOfChannels))
            {
                zeroMat.SetTo(new MCvScalar(0));
                CvInvoke.Compare(srcMat, zeroMat, mask, CmpType.NotEqual);
                return CvInvoke.CountNonZero(mask);
            }
        }

        private string PerformOCR(Bitmap image, TesseractEngine ocr)
        {
            if (image == null || ocr == null)
                return "";

            try
            {
                using (Pix pix = PixConverter.ToPix(image))
                using (Page page = ocr.Process(pix, PageSegMode.SingleChar))
                {
                    string result = page.GetText().Trim();

                    // If OCR result is still too long, try erosion
                    if (result.Length > 3)
                    {
                        result = PerformOCRWithErosion(image, ocr);
                    }

                    return result;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string PerformOCRWithErosion(Bitmap image, TesseractEngine ocr)
        {
            Bitmap workingImage = image;
            string result = "";
            int erosionCount = 0;

            try
            {
                while (result.Length > 3 && erosionCount < 10)
                {
                    using (Image<Gray, byte> temp = workingImage.ToGrayImage())
                    using (Image<Gray, byte> eroded = temp.Erode(2))
                    {
                        Bitmap previousImage = workingImage;
                        workingImage = eroded.ToBitmap();

                        if (previousImage != image)
                            previousImage.Dispose();

                        using (Pix pix = PixConverter.ToPix(workingImage))
                        using (Page page = ocr.Process(pix))
                        {
                            result = page.GetText().Trim();
                        }
                    }

                    erosionCount++;
                }

                return result;
            }
            finally
            {
                if (workingImage != image)
                    workingImage.Dispose();
            }
        }

        #endregion
    }
}
