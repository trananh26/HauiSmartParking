using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using AwsImage = Amazon.Rekognition.Model.Image;

namespace Auto_parking.Utils
{
    /// <summary>
    /// Service tương tác với AWS Rekognition để nhận diện text trên ảnh
    /// </summary>
    public class AwsRekognitionService : IDisposable
    {
        private readonly AmazonRekognitionClient _rekognitionClient;
        private readonly Models.AwsSettings _awsConfig;
        private bool _disposed = false;

        public AwsRekognitionService()
        {
            _awsConfig = ConfigurationManager.Instance.Config.Aws;

            if (string.IsNullOrEmpty(_awsConfig.AccessKey) || string.IsNullOrEmpty(_awsConfig.SecretKey))
            {
                throw new InvalidOperationException(
                    "AWS credentials chưa được cấu hình. Vui lòng kiểm tra appsettings.json");
            }

            // Parse region
            RegionEndpoint region = ParseRegion(_awsConfig.Region);

            // Khởi tạo client
            _rekognitionClient = new AmazonRekognitionClient(
                _awsConfig.AccessKey,
                _awsConfig.SecretKey,
                region);
        }

        /// <summary>
        /// Nhận diện text từ byte array của ảnh
        /// </summary>
        public AwsRekognitionResult DetectTextFromImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                throw new ArgumentNullException(nameof(imageBytes));

            try
            {
                using (var memoryStream = new MemoryStream(imageBytes))
                {
                    var detectTextRequest = new DetectTextRequest
                    {
                        Image = new AwsImage
                        {
                            Bytes = memoryStream
                        }
                    };

                    var detectTextResponse = _rekognitionClient.DetectText(detectTextRequest);

                    return ProcessDetectionResponse(detectTextResponse);
                }
            }
            catch (AmazonRekognitionException ex)
            {
                return new AwsRekognitionResult
                {
                    Success = false,
                    ErrorMessage = $"AWS Rekognition error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new AwsRekognitionResult
                {
                    Success = false,
                    ErrorMessage = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Nhận diện text từ Bitmap
        /// </summary>
        public AwsRekognitionResult DetectTextFromBitmap(Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            byte[] imageBytes = BitmapToByteArray(image);
            return DetectTextFromImage(imageBytes);
        }

        /// <summary>
        /// Nhận diện text từ file path
        /// </summary>
        public AwsRekognitionResult DetectTextFromFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException(nameof(imagePath));

            if (!File.Exists(imagePath))
                throw new FileNotFoundException("File không tồn tại", imagePath);

            byte[] imageBytes = File.ReadAllBytes(imagePath);
            return DetectTextFromImage(imageBytes);
        }

        /// <summary>
        /// Xử lý response từ AWS Rekognition với các tham số tối ưu từ config
        /// </summary>
        private AwsRekognitionResult ProcessDetectionResponse(DetectTextResponse response)
        {
            var result = new AwsRekognitionResult { Success = true };

            if (response.TextDetections == null || response.TextDetections.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Không phát hiện text trong ảnh";
                return result;
            }

            // Lọc theo confidence threshold từ config
            var validDetections = response.TextDetections
                .Where(t => t.Confidence >= _awsConfig.MinConfidenceThreshold)
                .ToList();

            // Áp dụng lọc bounding box nếu được cấu hình
            if (_awsConfig.MinBoundingBoxWidth > 0 || _awsConfig.MinBoundingBoxHeight > 0)
            {
                validDetections = validDetections
                    .Where(t => (t.Geometry?.BoundingBox?.Width ?? 0) >= _awsConfig.MinBoundingBoxWidth &&
                                (t.Geometry?.BoundingBox?.Height ?? 0) >= _awsConfig.MinBoundingBoxHeight)
                    .ToList();
            }

            // Tách LINE và WORD
            result.Lines = validDetections
                .Where(t => t.Type == TextTypes.LINE)
                .Select(t => new DetectedText
                {
                    Text = t.DetectedText,
                    Confidence = (float)(t.Confidence ?? 0f),
                    BoundingBox = ConvertBoundingBox(t.Geometry.BoundingBox)
                })
                .OrderBy(t => t.BoundingBox.Top) // Sắp xếp từ trên xuống dưới
                .ToList();

            result.Words = validDetections
                .Where(t => t.Type == TextTypes.WORD)
                .Select(t => new DetectedText
                {
                    Text = t.DetectedText,
                    Confidence = (float)(t.Confidence ?? 0f),
                    BoundingBox = ConvertBoundingBox(t.Geometry.BoundingBox)
                })
                .OrderBy(t => t.BoundingBox.Left) // Sắp xếp từ trái sang phải
                .ToList();

            // Áp dụng lọc confidence nếu được cấu hình
            if (_awsConfig.FilterByConfidence)
            {
                result.Lines = result.Lines
                    .Where(l => l.Confidence >= _awsConfig.MinConfidenceThreshold)
                    .ToList();

                result.Words = result.Words
                    .Where(w => w.Confidence >= _awsConfig.MinConfidenceThreshold)
                    .ToList();
            }

            // Ghép text thành chuỗi biển số
            result.PlateNumber = ExtractLicensePlateNumber(result.Lines, result.Words);

            return result;
        }

        /// <summary>
        /// Trích xuất biển số xe từ các text đã detect
        /// </summary>
        private string ExtractLicensePlateNumber(List<DetectedText> lines, List<DetectedText> words)
        {
            string result = string.Empty;

            if (lines.Count > 0)
            {
                // Ưu tiên lấy text từ LINE (đã được AWS ghép sẵn)
                result = string.Join(" ", lines.Select(l => l.Text.Trim()));
            }
            else if (words.Count > 0)
            {
                // Fallback: ghép từ WORD
                result = string.Join("", words.Select(w => w.Text.Trim()));
            }

            // Áp dụng post-processing nếu được cấu hình
            if (_awsConfig.ApplyPostProcessing && !string.IsNullOrEmpty(result))
            {
                result = ApplyPostProcessing(result);
            }

            return result;
        }

        /// <summary>
        /// Xử lý sau (post-processing) cho chuỗi biển số theo config
        /// </summary>
        private string ApplyPostProcessing(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Loại bỏ khoảng trắng thừa nếu được cấu hình
            if (_awsConfig.RemoveExtraSpaces)
            {
                text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
            }

            // Chuyển thành chữ in hoa nếu được cấu hình
            if (_awsConfig.ConvertToUpperCase)
            {
                text = text.ToUpper();
            }

            return text;
        }

        /// <summary>
        /// Convert AWS BoundingBox sang RectangleF
        /// </summary>
        private RectangleF ConvertBoundingBox(Amazon.Rekognition.Model.BoundingBox box)
        {
            return new RectangleF(
                (float)(box.Left ?? 0f),
                (float)(box.Top ?? 0f),
                (float)(box.Width ?? 0f),
                (float)(box.Height ?? 0f));
        }

        /// <summary>
        /// Convert Bitmap sang byte array
        /// </summary>
        private byte[] BitmapToByteArray(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Parse region string sang RegionEndpoint
        /// </summary>
        private RegionEndpoint ParseRegion(string regionName)
        {
            switch (regionName.ToLower())
            {
                case "ap-southeast-1":
                    return RegionEndpoint.APSoutheast1;
                case "us-east-1":
                    return RegionEndpoint.USEast1;
                case "us-west-2":
                    return RegionEndpoint.USWest2;
                case "eu-west-1":
                    return RegionEndpoint.EUWest1;
                case "ap-northeast-1":
                    return RegionEndpoint.APNortheast1;
                default:
                    return RegionEndpoint.APSoutheast1; // Default Singapore
            }
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
                _rekognitionClient?.Dispose();
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Kết quả từ AWS Rekognition
    /// </summary>
    public class AwsRekognitionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string PlateNumber { get; set; }
        public List<DetectedText> Lines { get; set; }
        public List<DetectedText> Words { get; set; }

        public AwsRekognitionResult()
        {
            Lines = new List<DetectedText>();
            Words = new List<DetectedText>();
        }
    }

    /// <summary>
    /// Text được phát hiện
    /// </summary>
    public class DetectedText
    {
        public string Text { get; set; }
        public float Confidence { get; set; }
        public RectangleF BoundingBox { get; set; }
    }
}
