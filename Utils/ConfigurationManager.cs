using System;
using System.IO;
using Newtonsoft.Json;
using Auto_parking.Models;

namespace Auto_parking.Utils
{
    public sealed class ConfigurationManager
    {
        private static readonly object _lock = new object();
        private static ConfigurationManager _instance;
        private AppConfiguration _config;
        private readonly string _configPath;
        private readonly string _backupPath;

        private ConfigurationManager()
        {
            string appDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            _configPath = Path.Combine(appDataFolder, "appsettings.json");
            _backupPath = Path.Combine(appDataFolder, "appsettings.backup.json");
            
            LoadConfiguration();
        }

        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigurationManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public AppConfiguration Config
        {
            get { return _config; }
        }

        public void LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    _config = JsonConvert.DeserializeObject<AppConfiguration>(json);
                }
                else
                {
                    // T?o config m?c ??nh n?u file không t?n t?i
                    _config = AppConfiguration.LoadDefault();
                    SaveConfiguration();
                }

                // Validate và normalize paths
                NormalizePaths();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    "L?i khi ??c file c?u hình. S? d?ng c?u hình m?c ??nh.\n" + ex.Message,
                    "C?nh báo",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);

                _config = AppConfiguration.LoadDefault();
            }
        }

        public void SaveConfiguration()
        {
            lock (_lock)
            {
                try
                {
                    // Backup file c? tr??c khi save
                    if (File.Exists(_configPath))
                    {
                        File.Copy(_configPath, _backupPath, true);
                    }

                    // Save file m?i
                    string json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                    File.WriteAllText(_configPath, json);
                }
                catch (Exception ex)
                {
                    throw new Exception("Không th? l?u c?u hình: " + ex.Message, ex);
                }
            }
        }

        public void ResetToDefault()
        {
            _config = AppConfiguration.LoadDefault();
        }

        public void RestoreFromBackup()
        {
            if (File.Exists(_backupPath))
            {
                File.Copy(_backupPath, _configPath, true);
                LoadConfiguration();
            }
        }

        private void NormalizePaths()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Normalize Recognition paths
            if (!Path.IsPathRooted(_config.Recognition.TesseractDataPath))
            {
                _config.Recognition.TesseractDataPath = 
                    Path.Combine(baseDirectory, _config.Recognition.TesseractDataPath);
            }

            if (!Path.IsPathRooted(_config.Recognition.CascadePath))
            {
                _config.Recognition.CascadePath = 
                    Path.Combine(baseDirectory, _config.Recognition.CascadePath);
            }

            // Normalize data paths
            if (!Path.IsPathRooted(_config.Paths.DataDirectory))
            {
                _config.Paths.DataDirectory = 
                    Path.Combine(baseDirectory, _config.Paths.DataDirectory);
            }

            if (!Path.IsPathRooted(_config.Paths.TempImagePath))
            {
                _config.Paths.TempImagePath = 
                    Path.Combine(baseDirectory, _config.Paths.TempImagePath);
            }
        }

        public bool ValidateConfiguration(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate Tesseract data path
            if (!Directory.Exists(_config.Recognition.TesseractDataPath))
            {
                errorMessage = "Không tìm th?y th? m?c Tesseract Data: " + 
                    _config.Recognition.TesseractDataPath;
                return false;
            }

            // Validate Cascade file
            if (!File.Exists(_config.Recognition.CascadePath))
            {
                errorMessage = "Không tìm th?y file Cascade: " + 
                    _config.Recognition.CascadePath;
                return false;
            }

            // Validate BaudRate
            int[] validBaudRates = { 9600, 19200, 38400, 57600, 115200 };
            if (Array.IndexOf(validBaudRates, _config.SerialPort.BaudRate) == -1)
            {
                errorMessage = "BaudRate không h?p l?. Ch? ch?p nh?n: 9600, 19200, 38400, 57600, 115200";
                return false;
            }

            return true;
        }
    }
}
