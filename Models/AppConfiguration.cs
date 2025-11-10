using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Auto_parking.Models
{
    /// <summary>
    /// Ph??ng th?c nh?n di?n bi?n s?
    /// </summary>
    public enum RecognitionMethod
    {
        /// <summary>
        /// 1 - S? d?ng Tesseract OCR (lu?ng code hi?n t?i)
        /// </summary>
        Tesseract = 1,

        /// <summary>
        /// 2 - G?i lên AWS Rekognition
        /// </summary>
        AwsRekognition = 2
    }

    public class AppConfiguration
    {
        public DatabaseSettings Database { get; set; }
        public SerialPortSettings SerialPort { get; set; }
        public CameraSettings Camera { get; set; }
        public RecognitionSettings Recognition { get; set; }
        public PathSettings Paths { get; set; }
        public SystemSettings System { get; set; }
        public AwsSettings Aws { get; set; }

        public static AppConfiguration LoadDefault()
        {
            return new AppConfiguration
            {
                Database = new DatabaseSettings
                {
                    ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Haui_SmartParking;Integrated Security=True",
                    CommandTimeout = 30
                },
                SerialPort = new SerialPortSettings
                {
                    COM_STM1 = "COM3",
                    COM_STM2 = "COM4",
                    BaudRate = 9600,
                    AutoReconnectInterval = 3000
                },
                Camera = new CameraSettings
                {
                    Camera1Index = 0,
                    Camera2Index = 1,
                    FrameIntervalMs = 120,
                    MaxImageDimension = 1280
                },
                Recognition = new RecognitionSettings
                {
                    TesseractDataPath = "App_Data\\data",
                    CascadePath = "App_Data\\data\\output-hv-33-x25.xml",
                    GrayscaleThreshold = 44,
                    Method = RecognitionMethod.Tesseract
                },
                Paths = new PathSettings
                {
                    DataDirectory = "data",
                    TempImagePath = "data\\aa.bmp"
                },
                System = new SystemSettings
                {
                    ParkingSlots = 5,
                    ParkingFeePerUnit = 10000,
                    GCIntervalSeconds = 30,
                    Language = "vi-VN",
                    EnableTestMode = false
                },
                Aws = new AwsSettings
                {
                    AccessKey = "",
                    SecretKey = "",
                    Region = "ap-southeast-1",
                    MinConfidenceThreshold = 80.0,
                    MinBoundingBoxWidth = 0.01,
                    MinBoundingBoxHeight = 0.01,
                    FilterByConfidence = true,
                    ApplyPostProcessing = true,
                    RemoveExtraSpaces = true,
                    ConvertToUpperCase = true
                }
            };
        }
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
    }

    public class SerialPortSettings
    {
        public string COM_STM1 { get; set; }
        public string COM_STM2 { get; set; }
        public int BaudRate { get; set; }
        public int AutoReconnectInterval { get; set; }
    }

    public class CameraSettings
    {
        public int Camera1Index { get; set; }
        public int Camera2Index { get; set; }
        public int FrameIntervalMs { get; set; }
        public int MaxImageDimension { get; set; }
    }

    public class RecognitionSettings
    {
        public string TesseractDataPath { get; set; }
        public string CascadePath { get; set; }
        public int GrayscaleThreshold { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public RecognitionMethod Method { get; set; }
    }

    public class PathSettings
    {
        public string DataDirectory { get; set; }
        public string TempImagePath { get; set; }
    }

    public class SystemSettings
    {
        public int ParkingSlots { get; set; }
        public int ParkingFeePerUnit { get; set; }
        public int GCIntervalSeconds { get; set; }
        public string Language { get; set; }
        public bool EnableTestMode { get; set; }
    }

    /// <summary>
    /// C?u hình AWS Rekognition v?i các tham s? t?i ?u
    /// </summary>
    public class AwsSettings
    {
        /// <summary>
        /// AWS Access Key
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// AWS Secret Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// AWS Region (ap-southeast-1, us-east-1, ...)
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Ng??ng confidence t?i thi?u (0-100)
        /// </summary>
        public double MinConfidenceThreshold { get; set; }

        /// <summary>
        /// Chi?u r?ng t?i thi?u c?a bounding box (0-1)
        /// </summary>
        public double MinBoundingBoxWidth { get; set; }

        /// <summary>
        /// Chi?u cao t?i thi?u c?a bounding box (0-1)
        /// </summary>
        public double MinBoundingBoxHeight { get; set; }

        /// <summary>
        /// L?c k?t qu? theo confidence
        /// </summary>
        public bool FilterByConfidence { get; set; }

        /// <summary>
        /// Áp d?ng x? lý sau (post-processing)
        /// </summary>
        public bool ApplyPostProcessing { get; set; }

        /// <summary>
        /// Lo?i b? kho?ng tr?ng th?a
        /// </summary>
        public bool RemoveExtraSpaces { get; set; }

        /// <summary>
        /// Chuy?n thành ch? in hoa
        /// </summary>
        public bool ConvertToUpperCase { get; set; }
    }
}
