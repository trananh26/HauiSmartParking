using System;
using System.IO;
using Newtonsoft.Json;

namespace Auto_parking.Models
{
    public class AppConfiguration
    {
        public DatabaseSettings Database { get; set; }
        public SerialPortSettings SerialPort { get; set; }
        public CameraSettings Camera { get; set; }
        public RecognitionSettings Recognition { get; set; }
        public PathSettings Paths { get; set; }
        public SystemSettings System { get; set; }

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
                    GrayscaleThreshold = 44
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
                    Language = "vi-VN"
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
    }
}
