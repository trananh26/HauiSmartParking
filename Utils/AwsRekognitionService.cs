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
    /// Service ?? t??ng tác v?i AWS Rekognition ?? nh?n di?n text trên ?nh
    /// </summary>
    public class AwsRekognitionService : IDisposable
    {
        private readonly AmazonRekognitionClient _rekognitionClient;
        private readonly double _minConfidence;
        private bool _disposed = false;

        public AwsRekognitionService()
        {
            var awsConfig = ConfigurationManager.Instance.Config.Aws;

            if (string.IsNullOrEmpty(awsConfig.AccessKey) || string.IsNullOrEmpty(awsConfig.SecretKey))
            {
                throw new InvalidOperationException(
                    "AWS credentials chưa được cấu hình. Vui lòng kiểm tra appsettings.json");
            }

            // Parse region
            RegionEndpoint region = ParseRegion(awsConfig.Region);

            // Kh?i t?o client
            _rekognitionClient = new AmazonRekognitionClient(
                awsConfig.AccessKey,
                awsConfig.SecretKey,
                region);

            _minConfidence = awsConfig.MinConfidenceThreshold;
        }

        /// <summary>
        /// Nh?n di?n text t? byte array c?a ?nh
        /// </summary>
        public AwsRekognitionResult DetectTextFromImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                throw new ArgumentNullException(nameof(imageBytes));

            try
            {
                var detectTextRequest = new DetectTextRequest
                {
                    Image = new AwsImage
                    {
                        Bytes = new MemoryStream(imageBytes)
                    }
                };

                var detectTextResponse = _rekognitionClient.DetectText(detectTextRequest);

                return ProcessDetectionResponse(detectTextResponse);
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
        /// Nh?n di?n text t? Bitmap
        /// </summary>
        public AwsRekognitionResult DetectTextFromBitmap(Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            byte[] imageBytes = BitmapToByteArray(image);
            return DetectTextFromImage(imageBytes);
        }

        /// <summary>
        /// Nh?n di?n text t? file path
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
        /// X? lý response t? AWS Rekognition
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

            // L?c theo confidence threshold
            var validDetections = response.TextDetections
                .Where(t => t.Confidence >= _minConfidence)
                .ToList();

            // Tách LINE và WORD
            result.Lines = validDetections
                .Where(t => t.Type == TextTypes.LINE)
                .Select(t => new DetectedText
                {
                    Text = t.DetectedText,
                    Confidence = (float)(t.Confidence ?? 0f),
                    BoundingBox = ConvertBoundingBox(t.Geometry.BoundingBox)
                })
                .OrderBy(t => t.BoundingBox.Top) // S?p x?p t? trên xu?ng d??i
                .ToList();

            result.Words = validDetections
                .Where(t => t.Type == TextTypes.WORD)
                .Select(t => new DetectedText
                {
                    Text = t.DetectedText,
                    Confidence = (float)(t.Confidence ?? 0f),
                    BoundingBox = ConvertBoundingBox(t.Geometry.BoundingBox)
                })
                .OrderBy(t => t.BoundingBox.Left) // S?p x?p t? trái sang ph?i
                .ToList();

            // Ghép text thành chu?i bi?n s?
            result.PlateNumber = ExtractLicensePlateNumber(result.Lines, result.Words);

            return result;
        }

        /// <summary>
        /// Trích xu?t bi?n s? xe t? các text ?ã detect
        /// </summary>
        private string ExtractLicensePlateNumber(List<DetectedText> lines, List<DetectedText> words)
        {
            if (lines.Count > 0)
            {
                // ?u tiên l?y text t? LINE (?ã ???c AWS ghép s?n)
                return string.Join(" ", lines.Select(l => l.Text.Trim()));
            }
            else if (words.Count > 0)
            {
                // Fallback: ghép t? WORD
                return string.Join("", words.Select(w => w.Text.Trim()));
            }

            return string.Empty;
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
    /// K?t qu? t? AWS Rekognition
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
    /// Text ???c phát hi?n
    /// </summary>
    public class DetectedText
    {
        public string Text { get; set; }
        public float Confidence { get; set; }
        public RectangleF BoundingBox { get; set; }
    }
}
