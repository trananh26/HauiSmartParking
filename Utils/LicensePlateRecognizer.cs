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
        private const int GRAYSCALE_THRESHOLD_VALUE = 44;
        private const int MAX_IMAGE_DIMENSION = 1280;

        private TesseractEngine _fullTesseract;
        private TesseractEngine _chTesseract;
        private TesseractEngine _numTesseract;
        private Utils.AwsRekognitionService _awsService;
        private bool _disposed = false;

        #endregion

        #region Constructor & Disposal

        public LicensePlateRecognizer(string tesseractDataPath, string cascadePath)
        {
            _tesseractDataPath = tesseractDataPath ?? throw new ArgumentNullException(nameof(tesseractDataPath));
            _cascadePath = cascadePath ?? throw new ArgumentNullException(nameof(cascadePath));

            InitializeTesseract();
            InitializeAwsIfNeeded();
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
                _awsService?.Dispose();
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

        private void InitializeAwsIfNeeded()
        {
            try
            {
                var config = Utils.ConfigurationManager.Instance.Config;
                
                // Chỉ khởi tạo AWS nếu method = AwsRekognition
                if (config.Recognition.Method == Models.RecognitionMethod.AwsRekognition)
                {
                    if (string.IsNullOrEmpty(config.Aws.AccessKey) || 
                        string.IsNullOrEmpty(config.Aws.SecretKey))
                    {
                        throw new InvalidOperationException(
                            "AWS credentials chưa được cấu hình. Vui lòng cập nhật appsettings.json");
                    }

                    _awsService = new Utils.AwsRekognitionService();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khởi tạo AWS Rekognition: {ex.Message}");
                // Không throw exception, để fallback sang Tesseract
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
                // Bước 1: Tìm vùng chứa biển số (VÙNG A)
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

                    // Calculate optimal resize dimensions
                    (int width, int height) = CalculateOptimalResizeDimensions(plateRegion.Width, plateRegion.Height);
                    
                    using (Image<Bgr, byte> resized = plateRegion.Resize(width, height, Inter.Linear))
                    using (Bitmap plateBitmap = resized.ToBitmap())
                    {
                        // Bước 2: Nhận diện text trong VÙNG A dựa trên config
                        return RecognizePlateRegion(plateBitmap);
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

        #region Recognition Methods

        /// <summary>
        /// Nhận diện VÙNG A (vùng biển số đã được detect) - CHỈ GỬI VÙNG NÀY
        /// </summary>
        private RecognitionResult RecognizePlateRegion(Bitmap plateRegionImage)
        {
            var config = Utils.ConfigurationManager.Instance.Config;

            switch (config.Recognition.Method)
            {
                case Models.RecognitionMethod.Tesseract:
                    // Phương thức 1: Sử dụng Tesseract OCR (code hiện tại)
                    return RecognizeWithTesseract(plateRegionImage);

                case Models.RecognitionMethod.AwsRekognition:
                    // Phương thức 2: Gửi lên AWS Rekognition
                    return RecognizeWithAws(plateRegionImage);

                default:
                    return new RecognitionResult
                    {
                        Success = false,
                        ErrorMessage = "Phương thức nhận diện không hợp lệ"
                    };
            }
        }

        /// <summary>
        /// Phương thức 1: Nhận diện bằng Tesseract (luồng code hiện tại)
        /// </summary>
        private RecognitionResult RecognizeWithTesseract(Bitmap plateImage)
        {
            return ExtractAndRecognizeCharacters(plateImage);
        }

        /// <summary>
        /// Phương thức 2: Nhận diện bằng AWS Rekognition
        /// </summary>
        private RecognitionResult RecognizeWithAws(Bitmap plateImage)
        {
            if (_awsService == null)
            {
                // Fallback to Tesseract nếu AWS chưa khởi tạo
                System.Diagnostics.Debug.WriteLine("AWS service not available, falling back to Tesseract");
                return RecognizeWithTesseract(plateImage);
            }

            try
            {
                var awsResult = _awsService.DetectTextFromBitmap(plateImage);

                if (!awsResult.Success)
                {
                    // Fallback to Tesseract nếu AWS fail
                    System.Diagnostics.Debug.WriteLine($"AWS failed: {awsResult.ErrorMessage}, falling back to Tesseract");
                    return RecognizeWithTesseract(plateImage);
                }

                // Xử lý kết quả từ AWS
                string cleanedPlateNumber = CleanAwsPlateNumber(awsResult.PlateNumber);

                return new RecognitionResult
                {
                    Success = true,
                    PlateNumber = cleanedPlateNumber,
                    FormattedText = FormatPlateNumber(cleanedPlateNumber),
                    PlateImage = (Bitmap)plateImage.Clone(),
                    AwsConfidence = awsResult.Lines.Count > 0 
                        ? awsResult.Lines.Average(l => l.Confidence) 
                        : 0
                };
            }
            catch (Exception ex)
            {
                // Fallback to Tesseract on error
                System.Diagnostics.Debug.WriteLine($"AWS exception: {ex.Message}, falling back to Tesseract");
                return RecognizeWithTesseract(plateImage);
            }
        }

        #endregion

        #region Helper Methods for AWS

        /// <summary>
        /// Làm sạch text biển số từ AWS
        /// </summary>
        private string CleanAwsPlateNumber(string plateNumber)
        {
            if (string.IsNullOrEmpty(plateNumber))
                return string.Empty;

            // Loại bỏ khoảng trắng thừa
            plateNumber = plateNumber.Trim();
            
            // Loại bỏ ký tự đặc biệt không hợp lệ
            plateNumber = System.Text.RegularExpressions.Regex.Replace(
                plateNumber, @"[^A-Z0-9\s\-\.]", "");

            return plateNumber;
        }

        /// <summary>
        /// Format biển số theo chuẩn VN (nếu cần)
        /// </summary>
        private string FormatPlateNumber(string plateNumber)
        {
            if (string.IsNullOrEmpty(plateNumber))
                return string.Empty;

            // Nếu có dấu xuống dòng hoặc space, giữ nguyên
            if (plateNumber.Contains("\n") || plateNumber.Contains("\r"))
                return plateNumber;

            // Format: XX-Y ZZZZ (ví dụ: 30A-12345)
            plateNumber = plateNumber.Replace(" ", "").Replace("-", "");
            
            if (plateNumber.Length >= 6)
            {
                // Tách thành: [2 số tỉnh][1 chữ loại xe][4-5 số biển]
                if (plateNumber.Length > 1 && 
                    char.IsDigit(plateNumber[0]) && 
                    char.IsDigit(plateNumber[1]))
                {
                    string result = plateNumber.Substring(0, 2); // Mã tỉnh
                    
                    if (plateNumber.Length > 2 && char.IsLetter(plateNumber[2]))
                    {
                        result += plateNumber[2]; // Loại xe
                        
                        if (plateNumber.Length > 3)
                        {
                            result += "-" + plateNumber.Substring(3); // Số biển
                        }
                    }
                    
                    return result;
                }
            }

            return plateNumber;
        }

        #endregion

        #region Private Methods - Plate Detection

        private (int, int) CalculateOptimalResizeDimensions(int currentWidth, int currentHeight)
        {
            // Vietnamese license plate standard ratios
            double[] standardRatios = new double[]
            {
                330.0 / 165.0,    // 2.0
                520.0 / 110.0,    // 4.727
                190.0 / 140.0,    // 1.357
                280.0 / 200.0,    // 1.4
                470.0 / 110.0     // 4.273
            };

            // Standard dimensions for each ratio (width x height)
            (int, int)[] standardDimensions = new (int, int)[]
            {
                (660, 330),       // Ratio 2.0
                (520, 110),       // Ratio 4.727
                (190, 140),       // Ratio 1.357
                (280, 200),       // Ratio 1.4
                (470, 110)        // Ratio 4.273
            };

            double actualRatio = (double)currentWidth / currentHeight;
            double minDifference = double.MaxValue;
            int bestMatchIndex = 0;

            // Find the closest matching standard ratio
            for (int i = 0; i < standardRatios.Length; i++)
            {
                double difference = Math.Abs(actualRatio - standardRatios[i]);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    bestMatchIndex = i;
                }
            }

            // Get the matching standard dimension
            (int stdWidth, int stdHeight) = standardDimensions[bestMatchIndex];

            // Ensure minimum edge is 400 pixels for optimal OCR
            double minStdDimension = Math.Min(stdWidth, stdHeight);
            double scale = 400.0 / minStdDimension;
            
            int targetWidth = (int)(stdWidth * scale);
            int targetHeight = (int)(stdHeight * scale);

            return (targetWidth, targetHeight);
        }

        private Image<Bgr, byte> FindLicensePlateRegion(Bitmap image)
        {
            using (Bitmap processedImage = DownscaleImageIfNeeded(image))
            using (CascadeClassifier cascade = new CascadeClassifier(_cascadePath))
            {
                for (float angle = 0; angle <= 20; angle += 3)
                {
                    for (float sign = -1; sign <= 1 && sign + angle != 1; sign += 2)
                    {
                        Image<Bgr, byte> plateRegion = DetectPlateAtAngle(processedImage, cascade, angle * sign);
                        if (plateRegion != null)
                            return plateRegion;
                    }
                }
            }

            return null;
        }

        private Bitmap DownscaleImageIfNeeded(Bitmap image)
        {
            if (image.Width <= MAX_IMAGE_DIMENSION && image.Height <= MAX_IMAGE_DIMENSION)
                return new Bitmap(image);

            float scale = Math.Min(
                (float)MAX_IMAGE_DIMENSION / image.Width,
                (float)MAX_IMAGE_DIMENSION / image.Height);

            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);

            Bitmap downscaled = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(downscaled))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return downscaled;
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
                                GRAYSCALE_THRESHOLD_VALUE,
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

                FilterAndSortRectangles(
                    grayframe,
                    rectangles,
                    out List<Rectangle> upRow,
                    out List<Rectangle> downRow);

                (string, List<Bitmap>) upText = RecognizeRectangleList(grayframe, upRow, true);
                (string, List<Bitmap>) downText = RecognizeRectangleList(grayframe, downRow, false);

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
                    // Enhance character clarity before padding
                    using (Bitmap enhancedImage = EnhanceCharacterClarity(charImage))
                    {
                        // Add 5px white padding around character image
                        using (Bitmap paddedImage = AddPaddingToCharacterImage(enhancedImage, 5))
                        {
                            charImages.Add((Bitmap)paddedImage.Clone());

                            // Determine OCR engine based on position and row type
                            bool useNumericEngine = ShouldUseNumericEngine(isUpperRow, i);
                            string character = Ocr(paddedImage, useNumericEngine);
                            result += character;
                        }
                    }
                }
            }

            return (result, charImages);
        }

        private Bitmap EnhanceCharacterClarity(Bitmap sourceImage)
        {
            if (sourceImage == null)
                return null;

            using (Image<Gray, byte> grayImage = sourceImage.ToGrayImage())
            {
                // Apply histogram equalization for better contrast
                using (Mat srcMat = grayImage.Mat)
                using (Mat dstMat = new Mat())
                {
                    CvInvoke.EqualizeHist(srcMat, dstMat);
                    Image<Gray, byte> enhancedImage = dstMat.ToImage<Gray, byte>();

                    // Apply bilateral filter to reduce noise while preserving edges
                    Image<Gray, byte> filtered = enhancedImage.SmoothBilateral(9, 75, 75);
                    enhancedImage.Dispose();

                    // Apply unsharp masking for sharpening
                    Image<Gray, byte> sharpened = ApplyUnsharpMask(filtered, 1.5);
                    filtered.Dispose();

                    // Convert back to bitmap
                    Bitmap result = sharpened.ToBitmap();
                    sharpened.Dispose();
                    return result;
                }
            }
        }

        private Image<Gray, byte> ApplyUnsharpMask(Image<Gray, byte> source, double strength)
        {
            using (Image<Gray, byte> blurred = source.SmoothGaussian(5))
            {
                Image<Gray, byte> result = new Image<Gray, byte>(source.Size);

                using (Mat srcMat = source.Mat)
                using (Mat blurMat = blurred.Mat)
                using (Mat resultMat = result.Mat)
                {
                    // Unsharp mask: result = source + strength * (source - blurred)
                    Mat diff = new Mat();
                    CvInvoke.Subtract(srcMat, blurMat, diff);
                    CvInvoke.ConvertScaleAbs(diff, diff, strength, 0);
                    CvInvoke.Add(srcMat, diff, resultMat);
                    diff.Dispose();
                }

                return result;
            }
        }

        private Bitmap AddPaddingToCharacterImage(Bitmap sourceImage, int padding)
        {
            if (sourceImage == null)
                return null;

            int newWidth = sourceImage.Width + (padding * 2);
            int newHeight = sourceImage.Height + (padding * 2);

            // Create new bitmap with white background
            Bitmap paddedBitmap = new Bitmap(newWidth, newHeight);
            
            using (Graphics g = Graphics.FromImage(paddedBitmap))
            {
                // Fill with white background
                g.Clear(Color.White);
                
                // Draw original image in the center with padding
                g.DrawImageUnscaled(sourceImage, padding, padding);
            }

            return paddedBitmap;
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
