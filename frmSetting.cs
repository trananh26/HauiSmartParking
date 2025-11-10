using System;
using System.IO.Ports;
using System.Windows.Forms;
using Auto_parking.Models;
using Auto_parking.Utils;

namespace Auto_parking
{
    public partial class frmSetting : Form
    {
        private AppConfiguration _tempConfig;

        public frmSetting()
        {
            InitializeComponent();
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            // Load c?u hình hi?n t?i vào temporary config
            _tempConfig = CloneConfiguration(ConfigurationManager.Instance.Config);
            
            LoadSettingsToUI();
            PopulateComPorts();
        }

        private AppConfiguration CloneConfiguration(AppConfiguration source)
        {
            // Deep clone using JSON serialization
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(source);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AppConfiguration>(json);
        }

        private void LoadSettingsToUI()
        {
            // Database Tab
            txtConnectionString.Text = _tempConfig.Database.ConnectionString;
            numCommandTimeout.Value = _tempConfig.Database.CommandTimeout;

            // Serial Port Tab
            cboCOM_STM1.Text = _tempConfig.SerialPort.COM_STM1;
            cboCOM_STM2.Text = _tempConfig.SerialPort.COM_STM2;
            cboBaudRate.Text = _tempConfig.SerialPort.BaudRate.ToString();
            numReconnectInterval.Value = _tempConfig.SerialPort.AutoReconnectInterval;

            // Camera Tab
            numCamera1Index.Value = _tempConfig.Camera.Camera1Index;
            numCamera2Index.Value = _tempConfig.Camera.Camera2Index;
            numFrameInterval.Value = _tempConfig.Camera.FrameIntervalMs;
            numMaxImageDimension.Value = _tempConfig.Camera.MaxImageDimension;

            // Recognition Tab
            txtTesseractPath.Text = _tempConfig.Recognition.TesseractDataPath;
            txtCascadePath.Text = _tempConfig.Recognition.CascadePath;
            numGrayscaleThreshold.Value = _tempConfig.Recognition.GrayscaleThreshold;

            // System Tab
            numParkingSlots.Value = _tempConfig.System.ParkingSlots;
            numParkingFee.Value = _tempConfig.System.ParkingFeePerUnit;
            numGCInterval.Value = _tempConfig.System.GCIntervalSeconds;
            cboLanguage.Text = _tempConfig.System.Language;
        }

        private void PopulateComPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            
            cboCOM_STM1.Items.Clear();
            cboCOM_STM2.Items.Clear();
            
            foreach (string port in ports)
            {
                cboCOM_STM1.Items.Add(port);
                cboCOM_STM2.Items.Add(port);
            }

            // Add common baud rates
            int[] baudRates = { 9600, 19200, 38400, 57600, 115200 };
            cboBaudRate.Items.Clear();
            foreach (int rate in baudRates)
            {
                cboBaudRate.Items.Add(rate.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Update temp config from UI
                UpdateConfigFromUI();

                // Assign to singleton instance
                var config = ConfigurationManager.Instance.Config;
                config.Database = _tempConfig.Database;
                config.SerialPort = _tempConfig.SerialPort;
                config.Camera = _tempConfig.Camera;
                config.Recognition = _tempConfig.Recognition;
                config.Paths = _tempConfig.Paths;
                config.System = _tempConfig.System;

                // Validate
                if (!ConfigurationManager.Instance.ValidateConfiguration(out string error))
                {
                    MessageBox.Show(error, "L?i Validation", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Save to file
                ConfigurationManager.Instance.SaveConfiguration();

                MessageBox.Show("L?u c?u hình thành công!\n\nVui lòng kh?i ??ng l?i ?ng d?ng ?? áp d?ng thay ??i.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i khi l?u c?u hình: " + ex.Message,
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateConfigFromUI()
        {
            // Database
            _tempConfig.Database.ConnectionString = txtConnectionString.Text;
            _tempConfig.Database.CommandTimeout = (int)numCommandTimeout.Value;

            // Serial Port
            _tempConfig.SerialPort.COM_STM1 = cboCOM_STM1.Text;
            _tempConfig.SerialPort.COM_STM2 = cboCOM_STM2.Text;
            _tempConfig.SerialPort.BaudRate = int.Parse(cboBaudRate.Text);
            _tempConfig.SerialPort.AutoReconnectInterval = (int)numReconnectInterval.Value;

            // Camera
            _tempConfig.Camera.Camera1Index = (int)numCamera1Index.Value;
            _tempConfig.Camera.Camera2Index = (int)numCamera2Index.Value;
            _tempConfig.Camera.FrameIntervalMs = (int)numFrameInterval.Value;
            _tempConfig.Camera.MaxImageDimension = (int)numMaxImageDimension.Value;

            // Recognition
            _tempConfig.Recognition.TesseractDataPath = txtTesseractPath.Text;
            _tempConfig.Recognition.CascadePath = txtCascadePath.Text;
            _tempConfig.Recognition.GrayscaleThreshold = (int)numGrayscaleThreshold.Value;

            // System
            _tempConfig.System.ParkingSlots = (int)numParkingSlots.Value;
            _tempConfig.System.ParkingFeePerUnit = (int)numParkingFee.Value;
            _tempConfig.System.GCIntervalSeconds = (int)numGCInterval.Value;
            _tempConfig.System.Language = cboLanguage.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "B?n có ch?c mu?n khôi ph?c c?u hình m?c ??nh?",
                "Xác nh?n",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _tempConfig = AppConfiguration.LoadDefault();
                LoadSettingsToUI();
            }
        }

        private void btnBrowseTesseract_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Ch?n th? m?c ch?a Tesseract data";
                dialog.SelectedPath = _tempConfig.Recognition.TesseractDataPath;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtTesseractPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnBrowseCascade_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                dialog.Title = "Ch?n file Cascade XML";
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(_tempConfig.Recognition.CascadePath);
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtCascadePath.Text = dialog.FileName;
                }
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(txtConnectionString.Text))
                {
                    conn.Open();
                    MessageBox.Show("K?t n?i database thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i k?t n?i: " + ex.Message,
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
