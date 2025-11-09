using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace Auto_parking
{
    public partial class MainForm : Form
    {
        #region Private Fields 

        private string m_path = Application.StartupPath + @"\data\";
        private clsCommon cls = new clsCommon();
        private frmImage frmImage;
        private string o_Sensor;
        private bool IsFire;

        // Reuse buffer để tránh tạo mảng mới liên tục
        private readonly object _serialLock = new object();

        // Camera locks and throttling
        private readonly object _camera1Lock = new object();
        private readonly object _camera2Lock = new object();
        private DateTime _lastFrameTime1 = DateTime.MinValue;
        private DateTime _lastFrameTime2 = DateTime.MinValue;
        private const int FRAME_INTERVAL_MS = 120;

        // GC Timer
        private System.Windows.Forms.Timer _gcTimer;

        // Cache paths
        private readonly string m_tesseractDataPath;

        private LicensePlateRecognizer _plateRecognizer;

        // Singleton OpenFileDialog
        private OpenFileDialog _openFileDialog;
        private OpenFileDialog FileDialog
        {
            get
            {
                if (_openFileDialog == null)
                {
                    _openFileDialog = new OpenFileDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        Filter = "Bitmap files (*.bmp)|*.bmp|All Image files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png",
                        FilterIndex = 1,
                        Title = "Select a bitmap image for license plate recognition"
                    };
                }
                return _openFileDialog;
            }
        }

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            m_path = Path.Combine(Application.StartupPath, "data") + Path.DirectorySeparatorChar;
            m_tesseractDataPath = Path.Combine(Application.StartupPath, "App_Data", "data");

            // Ensure data directory exists
            if (!Directory.Exists(m_path))
            {
                Directory.CreateDirectory(m_path);
            }

            string cascadePath = Path.Combine(Application.StartupPath, "App_Data", "data", "output-hv-33-x25.xml");
            _plateRecognizer = new LicensePlateRecognizer(m_tesseractDataPath, cascadePath);
        }

        #endregion

        #region Methods

        private void RFID_Analys(string mathe)
        {
            // Chức năng phân tích dữ liệu RFID từ đầu đọc
            mathe = mathe.Trim();
            mathe = mathe.Replace("\0", "");

            // Validate input early
            if (mathe.Length < 2) return;

            string bienso = "";

            if (mathe.Substring(0, 2) == "i_")
            {
                bienso = CaptureImageThenRecognize(1);
            }
            else if (mathe.Substring(0, 2) == "o_")
            {
                bienso = CaptureImageThenRecognize(2);
            }
            else if (mathe.Substring(0, 1) == "s" && mathe.Length >= 6)
            {
                SensorAnalys(mathe.Substring(1, 5));
            }
            else if (mathe.Substring(0, 2) == "f_")
            {
                if (mathe.Length >= 3 && mathe.Substring(0, 3) == "f_1")
                {
                    IsFire = true;
                    MessageBox.Show("Bãi đỗ xe đang có cảnh báo NGUY HIỂM !!", "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                    SendData("1   FIRE EXIT   ");
                    Thread.Sleep(1000);
                    SendData("2 Please Go out ");
                    Thread.Sleep(1000);
                }
                else
                {
                    IsFire = false;
                    SendData("1    WELCOME    ");
                    Thread.Sleep(1000);
                    SendData("2       ");
                    Thread.Sleep(1000);
                }
            }

            if (!string.IsNullOrEmpty(bienso))
            {
                ProcessRecognizedPlate(mathe, bienso);
            }
        }

        // Extract method để tránh duplicate code
        private void ProcessRecognizedPlate(string mathe, string bienso)
        {
            DialogResult ketqua1 = MessageBox.Show("BIỂN SỐ XE LÀ " + bienso, "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (ketqua1 != DialogResult.OK) return;

            if (mathe.Substring(0, 2) == "i_")
            {
                ProcessInputPlate(mathe, bienso);
            }
            else if (mathe.Substring(0, 2) == "o_")
            {
                ProcessOutputPlate(mathe, bienso);
            }

            lblTotalInput.Text = cls.InputCount().ToString("00");
            lblTotalOutput.Text = cls.OutputCount().ToString("00");
        }

        private void ProcessInputPlate(string mathe, string bienso)
        {
            lblOutputTime.Visible = false;
            lblInputTime.Visible = false;
            lblMoney.Visible = false;

            string rfid = mathe.Substring(2, mathe.Length - 2);
            if (cls.Check_RF(rfid))
            {
                if (cls.Check_BienSo(bienso))
                {
                    MessageBox.Show("BIỂN SỐ XE ĐÃ TỒN TẠI TRONG BÃI", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    SendDataIN("B111111111111111");
                    cls.GuiXe(rfid, bienso);
                    lb_vaora.Text = "XE VÀO";
                }
            }
            else
            {
                MessageBox.Show("THẺ KHÔNG TỒN TẠI TRONG HỆ THỐNG", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ProcessOutputPlate(string mathe, string bienso)
        {
            string rfid = mathe.Substring(2, mathe.Length - 2);
            if (!cls.Check_RF(rfid))
            {
                MessageBox.Show("THẺ KHÔNG TỒN TẠI TRONG HỆ THỐNG", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!cls.Check_BienSo(bienso))
            {
                MessageBox.Show("XE KHÔNG Ở TRONG BÃI. VUI LÒNG KIỂM TRA LẠi", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SendData("B111111111111111");

            lblOutputTime.Visible = true;
            lblInputTime.Visible = true;
            lblMoney.Visible = true;

            DataTable dt = cls.GetInfor(rfid, bienso);
            lblOutputTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            lblInputTime.Text = dt.Rows[0]["UpdateTime"].ToString();
            Guid ID = new Guid(dt.Rows[0]["ID"].ToString());

            lblMoney.Text = "10.000 Đồng";
            cls.LayXe(rfid, ID, 10);
            lb_vaora.Text = "XE RA";
            cls.SaveMoney(10);
            lblTotalMoney.Text = cls.GetTotalMoney().ToString();

            // Dispose DataTable
            dt.Dispose();
        }

        //Xử lý tín hiệu cảm biến
        private void SensorAnalys(string SensorData)
        {
            try
            {
                if (SensorData == o_Sensor || IsFire) return;

                int Empty = 0;

                // Use array for panels
                Panel[] panels = { pnO1, pnO2, pnO3, pnO4, pnO5 };

                for (int i = 0; i < 5; i++)
                {
                    string sensor = SensorData.Substring(i, 1);
                    if (sensor == "0")
                    {
                        panels[i].BackColor = Color.Red;
                    }
                    else
                    {
                        panels[i].BackColor = Color.LightGreen;
                        Empty++;
                    }
                }

                lblEmpty.Text = Empty.ToString("00");

                if (Empty == 0)
                {
                    MessageBox.Show("BÃI ĐỖ XE HIỆN ĐÃ ĐẦY. VUI LÒNG GIẢI PHÓNG XE!", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SendData("1FULL - No Space");
                    Thread.Sleep(1000);
                    SendData("2 Sorry so much ");
                    Thread.Sleep(1000);
                }
                else
                {
                    SendData("1Vacancy Slot: " + Empty.ToString());
                    Thread.Sleep(1000);

                    string m_DataSend = "2No.:";
                    for (int i = 0; i < 5; i++)
                    {
                        if (SensorData.Substring(i, 1) == "1")
                        {
                            m_DataSend += " " + (i + 1).ToString();
                        }
                    }
                    m_DataSend = m_DataSend.PadRight(16).Substring(0, 16);
                    SendData(m_DataSend);
                    Thread.Sleep(1000);
                }

                o_Sensor = SensorData;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gửi tín hiệu xuống STM ngõ vào
        /// </summary>
        private void SendDataIN(string data)
        {
            lock (_serialLock)
            {
                if (STM1_Serial != null && STM1_Serial.IsOpen)
                {
                    STM1_Serial.Write(data);
                }
            }
        }

        /// <summary>
        /// Gửi tín hiệu xuống STM ngõ ra 
        /// </summary>
        private void SendData(string data)
        {
            lock (_serialLock)
            {
                if (STM2_Serial != null && STM2_Serial.IsOpen)
                {
                    STM2_Serial.Write(data);
                }
            }
        }

        #region di chuyển
        bool mouseDown = false;
        Point lastLocation;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseDown == false && e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.SetDesktopLocation(this.DesktopLocation.X - lastLocation.X + e.X, this.DesktopLocation.Y - lastLocation.Y + e.Y);
                this.Update();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        #endregion

        private void btn_chup_Click(object sender, EventArgs e)
        {
            CaptureImageThenRecognize(1);
        }

        private void btn_chupOP_Click(object sender, EventArgs e)
        {
            CaptureImageThenRecognize(2);
        }

        /// <summary>
        /// OPTIMIZED: Captures and recognizes license plate with proper resource management
        /// </summary>
        private string CaptureImageThenRecognize(int Type, string imageTestPath = null)
        {
            try
            {
                if (captureDevice1 == null && captureDevice2 == null)
                    return string.Empty;

                // Clear UI first
                ClearRecognitionUI(Type);

                // Save image to disk (reuse path)
                string tempImagePath = Path.Combine(m_path, "aa.bmp");
                SaveCapturedImage(Type, imageTestPath, tempImagePath);

                // Load and process image with proper disposal
                return ProcessCapturedImage(Type, tempImagePath);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
            finally
            {
                frmImage.ShowDialog();
                // Force cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void ClearRecognitionUI(int Type)
        {
            DisposeImage(picInputPicture1);
            DisposeImage(picOutputPicture1);
            DisposeImage(picInputPicture2);
            DisposeImage(picOutputPicture2);
            DisposeImage(pic_BiensoRa1);
            DisposeImage(pic_BiensoRa2);
            DisposeImage(pic_BiensoVao1);
            DisposeImage(pic_BiensoVao2);

            if (frmImage != null)
            {
                DisposeImage(frmImage.pictureBox2);
            }

            txt_BiensoVao.Text = string.Empty;
            txt_BiensoRa.Text = string.Empty;
            lblNoti.Visible = false;
        }

        private void SaveCapturedImage(int Type, string imageTestPath, string tempImagePath)
        {
            if (!string.IsNullOrEmpty(imageTestPath))
            {
                File.Copy(imageTestPath, tempImagePath, true);
            }
            else if (Type == 1 && picInputCam.Image != null)
            {
                using (Bitmap clone = new Bitmap(picInputCam.Image))
                {
                    clone.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            else if (Type == 2 && picOutputCam.Image != null)
            {
                using (Bitmap clone = new Bitmap(picOutputCam.Image))
                {
                    clone.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        private string ProcessCapturedImage(int Type, string tempImagePath)
        {
            using (FileStream fs = new FileStream(tempImagePath, FileMode.Open, FileAccess.Read))
            using (Image temp = Image.FromStream(fs))
            {
                Image clonedImage = new Bitmap(temp);

                if (Type == 1)
                {
                    picInputPicture1.Image = clonedImage;
                }
                else if (Type == 2)
                {
                    picOutputPicture1.Image = clonedImage;
                }

                if (frmImage != null)
                {
                    frmImage.pictureBox2.Image = new Bitmap(clonedImage);
                }

                using (RecognitionResult result = _plateRecognizer.RecognizeFromFile(tempImagePath))
                {
                    if (!result.Success)
                    {
                        MessageBox.Show(result.ErrorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return string.Empty;
                    }

                    DisplayRecognitionResult(Type, result);

                    return result.PlateNumber;
                }
            }
        }

        private void DisplayRecognitionResult(int Type, RecognitionResult result)
        {
            if (Type == 1)
            {
                DisposeImage(picInputPicture2);
                picInputPicture2.Image = result.PlateImage != null ? new Bitmap(result.PlateImage) : null;

                DisposeImage(pic_BiensoVao1);
                DisposeImage(pic_BiensoVao2);
                pic_BiensoVao1.Image = result.GrayImage != null ? new Bitmap(result.GrayImage) : null;
                pic_BiensoVao2.Image = result.ColorImage != null ? new Bitmap(result.ColorImage) : null;

                txt_BiensoVao.Text = result.PlateNumber;
            }
            else if (Type == 2)
            {
                DisposeImage(picOutputPicture2);
                picOutputPicture2.Image = result.PlateImage != null ? new Bitmap(result.PlateImage) : null;

                DisposeImage(pic_BiensoRa1);
                DisposeImage(pic_BiensoRa2);
                pic_BiensoRa1.Image = result.GrayImage != null ? new Bitmap(result.GrayImage) : null;
                pic_BiensoRa2.Image = result.ColorImage != null ? new Bitmap(result.ColorImage) : null;

                txt_BiensoRa.Text = result.PlateNumber;
            }

            if (frmImage != null)
            {
                DisposeImage(frmImage.pictureBox1);
                DisposeImage(frmImage.pictureBox3);

                frmImage.pictureBox1.Image = result.ColorImage != null ? new Bitmap(result.ColorImage) : null;
                frmImage.pictureBox3.Image = result.GrayImage != null ? new Bitmap(result.GrayImage) : null;
                frmImage.textBox6.Text = result.FormattedText;

                DisplayCharacterBoxes(result);
            }
        }

        private void DisplayCharacterBoxes(RecognitionResult result)
        {
            if (frmImage == null) return;

            int boxIndex = 0;

            var foobar = new List<PictureBox>();
            foobar.Add(frmImage.pictureBox5);
            foobar.Add(frmImage.pictureBox6);
            foobar.Add(frmImage.pictureBox7);
            foobar.Add(frmImage.pictureBox8);
            foobar.Add(frmImage.pictureBox9);
            foobar.Add(frmImage.pictureBox10);
            foobar.Add(frmImage.pictureBox11);
            foobar.Add(frmImage.pictureBox12);


            foreach (var rect in result.CharImages)
            {
                if (boxIndex >= foobar.Count) break;
                foobar[boxIndex].Image = rect;
                boxIndex++;
            }

            while (boxIndex < foobar.Count) {
                foobar[boxIndex].Image = null;
                boxIndex++;
            }
        }

        private void tm_AutoReconnect_Tick(object sender, EventArgs e)
        {
            TryReconnectSerial(STM1_Serial, "COM_STM1", STM1_Serial_DataReceived);
            TryReconnectSerial(STM2_Serial, "COM_STM2", STM2_Serial_DataReceived);
        }

        // Extract method for serial reconnection
        private void TryReconnectSerial(SerialPort serial, string comKey, SerialDataReceivedEventHandler handler)
        {
            if (serial == null || serial.IsOpen) return;

            try
            {
                serial.PortName = XINIFILE.ReadValue(comKey);
                serial.BaudRate = int.Parse(XINIFILE.ReadValue("BAURATE"));
                serial.Open();
                serial.DataReceived += handler;
            }
            catch
            {
                // Ignore connection errors
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            GetCameraInfor();

            lblMoney.Visible = false;
            lblTotalInput.Text = cls.InputCount().ToString("00");
            lblTotalOutput.Text = cls.OutputCount().ToString("00");
            lblTotalMoney.Text = cls.GetTotalMoney() + " Đồng";

            InitializeSerialPorts();

            frmImage = new frmImage();

            _gcTimer = new System.Windows.Forms.Timer();
            _gcTimer.Interval = 30000;
            _gcTimer.Tick += GcTimer_Tick;
            _gcTimer.Start();
        }

        private void GcTimer_Tick(object sender, EventArgs e)
        {
            // Force garbage collection để giải phóng memory
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
            GC.WaitForPendingFinalizers();
        }

        private void InitializeSerialPorts()
        {
            try
            {
                STM1_Serial.PortName = XINIFILE.ReadValue("COM_STM1");
                STM1_Serial.BaudRate = int.Parse(XINIFILE.ReadValue("BAURATE"));
                STM1_Serial.Open();
                STM1_Serial.DataReceived += STM1_Serial_DataReceived;

                STM2_Serial.PortName = XINIFILE.ReadValue("COM_STM2");
                STM2_Serial.BaudRate = int.Parse(XINIFILE.ReadValue("BAURATE"));
                STM2_Serial.Open();
                STM2_Serial.DataReceived += STM2_Serial_DataReceived;

                SendData("1 TRUONG DHCN HN");
                Thread.Sleep(500);
                SendData("2 KHOA DIEN TU  ");
                Thread.Sleep(500);
            }
            catch (Exception)
            {
            }
        }

        // Helper method
        private void DisposeImage(PictureBox pictureBox)
        {
            if (pictureBox == null) return;

            Image oldImage = pictureBox.Image;
            pictureBox.Image = null;
            oldImage?.Dispose();
        }

        VideoCaptureDevice captureDevice1;
        VideoCaptureDevice captureDevice2;

        /// <summary>
        /// Xử lý kết nối camera
        /// </summary>
        private void GetCameraInfor()
        {
            try
            {
                FilterInfoCollection filterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                captureDevice1 = new VideoCaptureDevice(filterInfo[0].MonikerString);
                captureDevice1.NewFrame += CaptureDevice1_NewFrame;
                captureDevice1.Start();

                int cam2Index = filterInfo.Count > 1 ? 1 : 0;
                captureDevice2 = new VideoCaptureDevice(filterInfo[cam2Index].MonikerString);
                captureDevice2.NewFrame += CaptureDevice2_NewFrame;
                captureDevice2.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tìm thấy thông tin camera. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"{ex}");
                Application.Exit();
            }
        }

        /// <summary>
        /// OPTIMIZED: Xử lý frame từ camera 1 với proper disposal và throttling
        /// </summary>
        private void CaptureDevice1_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            lock (_camera1Lock)
            {
                try
                {
                    // Throttle frame rate để giảm tải
                    DateTime now = DateTime.Now;
                    if ((now - _lastFrameTime1).TotalMilliseconds < FRAME_INTERVAL_MS)
                    {
                        return;
                    }
                    _lastFrameTime1 = now;

                    // Clone bitmap từ event args
                    Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();

                    // Sử dụng BeginInvoke để update UI thread an toàn
                    if (picOutputCam.InvokeRequired)
                    {
                        picOutputCam.BeginInvoke(new Action(() =>
                        {
                            UpdatePictureBox(picOutputCam, newFrame);
                        }));
                    }
                    else
                    {
                        UpdatePictureBox(picOutputCam, newFrame);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Camera 1 error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// OPTIMIZED: Xử lý frame từ camera 2 với proper disposal và throttling
        /// </summary>
        private void CaptureDevice2_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            lock (_camera2Lock)
            {
                try
                {
                    // Throttle frame rate để giảm tải
                    DateTime now = DateTime.Now;
                    if ((now - _lastFrameTime2).TotalMilliseconds < FRAME_INTERVAL_MS)
                    {
                        return;
                    }
                    _lastFrameTime2 = now;

                    // Clone bitmap từ event args
                    Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();

                    // Sử dụng BeginInvoke để update UI thread an toàn
                    if (picInputCam.InvokeRequired)
                    {
                        picInputCam.BeginInvoke(new Action(() =>
                        {
                            UpdatePictureBox(picInputCam, newFrame);
                        }));
                    }
                    else
                    {
                        UpdatePictureBox(picInputCam, newFrame);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Camera 2 error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Helper method để update PictureBox an toàn với proper disposal
        /// </summary>
        private void UpdatePictureBox(PictureBox pictureBox, Bitmap newImage)
        {
            try
            {
                // Lưu reference đến image cũ
                Image oldImage = pictureBox.Image;

                // Gán image mới
                pictureBox.Image = newImage;

                // Dispose image cũ sau khi đã gán image mới
                if (oldImage != null && oldImage != newImage)
                {
                    oldImage.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdatePictureBox error: {ex.Message}");
                newImage?.Dispose();
            }
        }

        // OPTIMIZED: Serial event với buffer management
        private void STM2_Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_serialLock)
            {
                try
                {
                    if (STM2_Serial.BytesToRead > 500)
                    {
                        STM2_Serial.DiscardInBuffer();
                        return;
                    }

                    string InputData = STM2_Serial.ReadTo("x");
                    if (!string.IsNullOrEmpty(InputData))
                    {
                        RFID_Analys(InputData);
                    }
                }
                catch
                {
                    // Ignore errors
                }
            }
        }

        private void STM1_Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_serialLock)
            {
                try
                {
                    if (STM1_Serial.BytesToRead > 500)
                    {
                        STM1_Serial.DiscardInBuffer();
                        return;
                    }

                    string InputData = STM1_Serial.ReadTo("x");
                    if (!string.IsNullOrEmpty(InputData))
                    {
                        RFID_Analys(InputData);
                    }
                }
                catch
                {
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Dừng timer
                if (_gcTimer != null)
                {
                    _gcTimer.Stop();
                    _gcTimer.Dispose();
                }

                // Dừng camera với timeout
                StopCameraWithTimeout(captureDevice1, "Camera 1");
                StopCameraWithTimeout(captureDevice2, "Camera 2");

                // Đóng serial ports
                CloseSerialPort(STM1_Serial);
                CloseSerialPort(STM2_Serial);

                // Clear PictureBox images
                DisposeImage(picInputCam);
                DisposeImage(picOutputCam);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormClosing error: {ex.Message}");
            }
        }

        private void StopCameraWithTimeout(VideoCaptureDevice device, string cameraName)
        {
            if (device != null && device.IsRunning)
            {
                try
                {
                    device.SignalToStop();

                    // AForge WaitForStop không có timeout parameter
                    device.WaitForStop();

                    device.NewFrame -= (device == captureDevice1)
                     ? CaptureDevice1_NewFrame
                  : (NewFrameEventHandler)CaptureDevice2_NewFrame;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error stopping {cameraName}: {ex.Message}");
                }
            }
        }

        private void CloseSerialPort(SerialPort port)
        {
            if (port != null && port.IsOpen)
            {
                try
                {
                    port.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error closing serial port: {ex.Message}");
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _plateRecognizer?.Dispose();
            _openFileDialog?.Dispose();
            _openFileDialog = null;

            frmImage?.Dispose();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                FileDialog.FileName = string.Empty;

                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = FileDialog.FileName;
                    CaptureImageThenRecognize(1, selectedFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
