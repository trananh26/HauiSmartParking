using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using Tesseract;

namespace Auto_parking
{
    public partial class MainForm : Form
    {
        #region Private Fields 

        delegate void SetTextCallback(string text);
        private clsCommon cls = new clsCommon();
        private frmImage IF;
        private string o_Sensor;
        private bool IsFire;
        delegate void MyDelegate();

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
        private const string m_lang = "eng";

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

            // Initialize paths once
            m_path = Path.Combine(Application.StartupPath, "data") + Path.DirectorySeparatorChar;
            m_tesseractDataPath = Path.Combine(Application.StartupPath, "App_Data", "data");

            // Ensure data directory exists
            if (!Directory.Exists(m_path))
            {
                Directory.CreateDirectory(m_path);
            }

            // Initialize PictureBox array once
            for (int i = 0; i < box.Length; i++)
            {
                box[i] = new PictureBox();
            }
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

        #region định nghĩa

        List<Rectangle> listRect = new List<Rectangle>();
        PictureBox[] box = new PictureBox[12];

        public TesseractEngine full_tesseract = null;
        public TesseractEngine ch_tesseract = null;
        public TesseractEngine num_tesseract = null;
        private string m_path = Application.StartupPath + @"\data\";

        #endregion

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

            if (IF != null)
            {
                DisposeImage(IF.pictureBox2);
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
                using (var clone = new Bitmap(picInputCam.Image))
                {
                    clone.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            else if (Type == 2 && picOutputCam.Image != null)
            {
                using (var clone = new Bitmap(picOutputCam.Image))
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
                // Clone image để tránh lock file
                Image clonedImage = new Bitmap(temp);

                if (Type == 1)
                {
                    picInputPicture1.Image = clonedImage;
                }
                else if (Type == 2)
                {
                    picOutputPicture1.Image = clonedImage;
                }

                if (IF != null)
                {
                    IF.pictureBox2.Image = new Bitmap(clonedImage);
                }

                string bienSoText = RecognizeFromFile(tempImagePath, Type);

                if (Type == 1)
                {
                    txt_BiensoVao.Text = bienSoText;
                }
                else if (Type == 2)
                {
                    txt_BiensoRa.Text = bienSoText;
                }

                return bienSoText.Replace("\n", "").Replace("\r", "");
            }
        }

        private string RecognizeFromFile(string imagePath, int Type)
        {
            Recognize(imagePath, Type, out Image hienBienSo, out string bienSo, out string bienSoText);

            if (hienBienSo != null)
            {
                if (Type == 1)
                {
                    DisposeImage(picInputPicture2);
                    picInputPicture2.Image = hienBienSo;
                }
                else if (Type == 2)
                {
                    DisposeImage(picOutputPicture2);
                    picOutputPicture2.Image = hienBienSo;
                }
            }

            return bienSoText ?? string.Empty;
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

            IF = new frmImage();

            InitializeTesseract();

            // Thêm timer để force GC định kỳ
            _gcTimer = new System.Windows.Forms.Timer();
            _gcTimer.Interval = 30000; // 30 giây
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
                // Ignore errors
            }
        }

        private void InitializeTesseract()
        {
            try
            {
                full_tesseract = new TesseractEngine(m_tesseractDataPath, m_lang, EngineMode.Default);
                full_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTVXY1234567890");

                ch_tesseract = new TesseractEngine(m_tesseractDataPath, m_lang, EngineMode.Default);
                ch_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTUVXY");

                num_tesseract = new TesseractEngine(m_tesseractDataPath, m_lang, EngineMode.Default);
                num_tesseract.SetVariable("tessedit_char_whitelist", "1234567890");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo Tesseract OCR: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method
        private void DisposeImage(PictureBox pictureBox)
        {
            if (pictureBox == null) return;

            var oldImage = pictureBox.Image;
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
                var filterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                captureDevice1 = new VideoCaptureDevice(filterInfo[0].MonikerString);
                captureDevice1.NewFrame += CaptureDevice1_NewFrame;
                captureDevice1.Start();

                var cam2Index = filterInfo.Count > 1 ? 1 : 0;
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
                    var now = DateTime.Now;
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
                    var now = DateTime.Now;
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
                var oldImage = pictureBox.Image;

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
                    // Ignore errors
                }
            }
        }

        // OPTIMIZED: ProcessImage with using statements
        public Image<Bgr, byte> ProcessImage(string urlImage)
        {
            try
            {
                using (var fs = new FileStream(urlImage, FileMode.Open, FileAccess.Read))
                using (var img = Image.FromStream(fs))
                using (var image = new Bitmap(img))
                {
                    return FindLicensePlate4(image);
                }
            }
            catch
            {
                MessageBox.Show("Không tìm được biển số. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

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

        // OPTIMIZED: OCR with proper disposal
        private string Ocr(Bitmap image_s, bool isFull, bool isNum = false)
        {
            string temp = "";

            using (Image<Gray, byte> src = image_s.ToGrayImage())
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

                using (Bitmap image = processed.ToBitmap())
                {
                    TesseractEngine ocr = isFull ? full_tesseract : (isNum ? num_tesseract : ch_tesseract);
                    temp = PerformOCR(image, ocr);
                }

                if (processed != src) processed.Dispose();
            }

            return temp;
        }

        // Helper method for counting non-zero pixels
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

        // Helper method for OCR
        private string PerformOCR(Bitmap image, TesseractEngine ocr)
        {
            string temp = "";
            int cou = 0;

            try
            {
                using (Pix pix = PixConverter.ToPix(image))
                using (Page page = ocr.Process(pix))
                {
                    temp = page.GetText().Trim();
                }

                Bitmap workingImage = image;
                while (temp.Length > 3 && cou < 10)
                {
                    using (Image<Gray, byte> temp2 = workingImage.ToGrayImage())
                    using (Image<Gray, byte> eroded = temp2.Erode(2))
                    {
                        if (workingImage != image)
                            workingImage.Dispose();

                        workingImage = eroded.ToBitmap();
                    }

                    using (Pix pix = PixConverter.ToPix(workingImage))
                    using (Page page = ocr.Process(pix))
                    {
                        temp = page.GetText().Trim();
                    }

                    cou++;
                }

                if (workingImage != image)
                    workingImage.Dispose();
            }
            catch (Exception)
            {
                temp = "";
            }

            return temp;
        }

        public Image<Bgr, byte> FindLicensePlate4(Bitmap image)
        {
            // 1. Chuẩn bị biến để xử lý ảnh với Emgu CV
            Image<Bgr, byte> plateDraw = null;
            Image dst = image;

            string cascadePath = Path.Combine(Application.StartupPath, "App_Data", "data", "output-hv-33-x25.xml");
            using (CascadeClassifier cascade = new CascadeClassifier(cascadePath))
            {
                // 3. Quét ảnh với nhiều góc xoay
                // Xoay từ -20° đến + 20° với bước nhảy 3°
                for (float i = 0; i <= 20; i += 3)
                {
                    for (float s = -1; s <= 1 && s + i != 1; s += 2)
                    {
                        using (var src = RotateImage(dst, i * s))
                        using (var frame = src.ToBgrImage())
                        using (Image<Gray, byte> grayframe = src.ToGrayImage())
                        {
                            // Use DetectMultiScale for Emgu.CV 3.x
                            var faces = cascade.DetectMultiScale(
                         grayframe,
                                 1.1,
                           8,
                         new Size(24, 24));

                            // Nếu phát hiện nhiều vùng, chọn vùng tốt nhất
                            if (faces.Length > 0)
                            {
                                var bestFace = SelectBestPlateRegion(faces);

                                // Chỉ xử lý vùng tốt nhất
                                plateDraw = frame.Copy(bestFace);
                                frame.Draw(bestFace, new Bgr(Color.Blue), 2);

                                if (IF != null)
                                {
                                    DisposeImage(IF.pictureBox2);
                                    IF.pictureBox2.Image = plateDraw.ToBitmap();
                                }

                                return plateDraw;
                            }
                        }
                    }
                }
            }

            return plateDraw;
        }

        private Rectangle SelectBestPlateRegion(Rectangle[] detectedRegions)
        {
            if (detectedRegions.Length == 1)
                return detectedRegions[0];

            // Tỷ lệ kích thước chuẩn của biển số xe Việt Nam (rộng/cao)
            // Ưu tiên từ trên xuống dưới
            double[] standardRatios = new double[]
        {
        330.0 / 165.0,  // 2.0
        520.0 / 110.0,  // 4.73
        190.0 / 140.0,  // 1.36
        280.0 / 200.0,  // 1.4
        470.0 / 110.0   // 4.27
        };

            List<Rectangle> filteredCandidates = new List<Rectangle>(detectedRegions);
            foreach (Rectangle candidate in detectedRegions)
            {
                foreach (Rectangle other in detectedRegions)
                {
                    if (candidate == other) continue;

                    if (IsRectangleContained(candidate, other))
                    {
                        filteredCandidates.Remove(other);
                        break;
                    }
                }
            }

            // 2. Tính điểm cho mỗi vùng dựa trên tỷ lệ kích thước chuẩn
            Rectangle bestRegion = filteredCandidates[0];
            double bestScore = CalculatePlateScore(bestRegion, standardRatios);

            foreach (Rectangle region in filteredCandidates)
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
            {
                areaScore = 10.0;
            }
            else if (area >= 3000 && area <= 150000)
            {
                areaScore = 5.0;
            }

            return ratioScore + priorityBonus + areaScore;
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

        // OPTIMIZED: Recognize with proper disposal
        private void Recognize(string link, int Type, out Image hinhbienso, out string bienso, out string bienso_text)
        {
            // ...existing code...
            DisposeImage(pic_BiensoVao1);
            DisposeImage(pic_BiensoVao2);
            DisposeImage(pic_BiensoRa1);
            DisposeImage(pic_BiensoRa2);

            for (int i = 0; i < box.Length; i++)
            {
                this.Controls.Remove(box[i]);
            }

            hinhbienso = null;
            bienso = "";
            bienso_text = "";

            using (var plateDraw = ProcessImage(link))
            {
                if (plateDraw == null) return;

                using (var resized = plateDraw.Resize(400, 400, Inter.Linear))
                {
                    ProcessPlateRecognition(resized, Type, out hinhbienso, out bienso, out bienso_text);
                }
            }
        }

        private void ProcessPlateRecognition(Image<Bgr, byte> plateDraw, int Type, out Image hinhbienso, out string bienso, out string bienso_text)
        {
            var con = new FindContours();

            using (Bitmap plateImage = plateDraw.ToBitmap())
            {
                int c = con.IdentifyContours(plateImage, 50, false, out Bitmap grayframe, out Bitmap color, out listRect);

                SetRecognitionImages(Type, color, grayframe, plateDraw.ToBitmap());
                hinhbienso = plateDraw.ToBitmap();

                string zz = ExtractTextFromContours(grayframe, listRect);

                bienso = zz.Replace("\n", "").Replace("\r", "");
                bienso_text = zz;

                if (IF != null)
                {
                    IF.textBox6.Text = zz;
                }

                // Dispose bitmaps
                grayframe?.Dispose();
                color?.Dispose();
            }
        }

        private void SetRecognitionImages(int Type, Bitmap color, Bitmap grayframe, Bitmap plate)
        {
            if (Type == 1)
            {
                DisposeImage(pic_BiensoVao2);
                DisposeImage(pic_BiensoVao1);
                pic_BiensoVao2.Image = new Bitmap(color);
                pic_BiensoVao1.Image = new Bitmap(grayframe);

                if (IF != null)
                {
                    DisposeImage(IF.pictureBox1);
                    DisposeImage(IF.pictureBox3);
                    IF.pictureBox1.Image = new Bitmap(color);
                    IF.pictureBox3.Image = new Bitmap(grayframe);
                }
            }
            else if (Type == 2)
            {
                DisposeImage(pic_BiensoRa2);
                DisposeImage(pic_BiensoRa1);
                pic_BiensoRa2.Image = new Bitmap(color);
                pic_BiensoRa1.Image = new Bitmap(grayframe);

                if (IF != null)
                {
                    DisposeImage(IF.pictureBox1);
                    DisposeImage(IF.pictureBox3);
                    IF.pictureBox1.Image = new Bitmap(color);
                    IF.pictureBox3.Image = new Bitmap(grayframe);
                }
            }
        }

        private string ExtractTextFromContours(Bitmap grayframe, List<Rectangle> rectangles)
        {
            if (rectangles == null || rectangles.Count == 0)
                return string.Empty;

            using (Image<Gray, byte> dst = grayframe.ToGrayImage())
            {
                using (Bitmap processedGray = dst.ToBitmap())
                {
                    FilterAndSortRectangles(processedGray, rectangles, out List<Rectangle> up, out List<Rectangle> dow);

                    string zz = "";
                    int c_x = 0;

                    zz += ProcessRectangleList(processedGray, up, 0, ref c_x, 290);
                    zz += "\r\n";
                    zz += ProcessRectangleList(processedGray, dow, c_x, ref c_x, 390);

                    return zz;
                }
            }
        }

        private void FilterAndSortRectangles(Bitmap grayframe, List<Rectangle> listRect, out List<Rectangle> up, out List<Rectangle> dow)
        {
            // ...existing code...
            up = new List<Rectangle>();
            dow = new List<Rectangle>();
            int up_y = 0, dow_y = 0;
            bool flag_up = false;

            // Remove invalid rectangles
            for (int i = 0; i < listRect.Count; i++)
            {
                using (Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat))
                {
                    string temp = "";
                    int cou = 0;

                    try
                    {
                        using (Pix pix = PixConverter.ToPix(ch))
                        using (Page page = full_tesseract.Process(pix))
                        {
                            temp = page.GetText().Trim();
                        }
                    }
                    catch
                    {
                        temp = "";
                    }

                    while (temp.Length > 3 && cou < 10)
                    {
                        using (Image<Gray, byte> temp2 = ch.ToGrayImage())
                        using (Image<Gray, byte> eroded = temp2.Erode(2))
                        using (Bitmap erodedBmp = eroded.ToBitmap())
                        {
                            try
                            {
                                using (Pix pix = PixConverter.ToPix(erodedBmp))
                                using (Page page = full_tesseract.Process(pix))
                                {
                                    temp = page.GetText().Trim();
                                }
                            }
                            catch
                            {
                                temp = "";
                            }
                        }

                        cou++;
                    }

                    if (cou > 10)
                    {
                        listRect.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Find up and down rows
            for (int i = 0; i < listRect.Count; i++)
            {
                for (int j = i; j < listRect.Count; j++)
                {
                    if (listRect[i].Y > listRect[j].Y + 100)
                    {
                        flag_up = true;
                        up_y = listRect[j].Y;
                        dow_y = listRect[i].Y;
                        break;
                    }
                    else if (listRect[j].Y > listRect[i].Y + 100)
                    {
                        flag_up = true;
                        up_y = listRect[i].Y;
                        dow_y = listRect[j].Y;
                        break;
                    }
                    if (flag_up) break;
                }
                if (flag_up) break;
            }

            // Separate into up and down lists
            for (int i = 0; i < listRect.Count; i++)
            {
                if (listRect[i].Y < up_y + 50 && listRect[i].Y > up_y - 50)
                {
                    up.Add(listRect[i]);
                }
                else if (listRect[i].Y < dow_y + 50 && listRect[i].Y > dow_y - 50)
                {
                    dow.Add(listRect[i]);
                }
            }

            if (!flag_up)
                dow = new List<Rectangle>(listRect);

            // Sort by X coordinate
            up.Sort((a, b) => a.X.CompareTo(b.X));
            dow.Sort((a, b) => a.X.CompareTo(b.X));
        }

        private string ProcessRectangleList(Bitmap grayframe, List<Rectangle> rects, int startIndex, ref int c_x, int yPosition)
        {
            string result = "";
            int x = 12;

            for (int i = 0; i < rects.Count; i++)
            {
                using (Bitmap ch = grayframe.Clone(rects[i], grayframe.PixelFormat))
                {
                    string temp;
                    if (yPosition == 290 && i < 2)
                    {
                        temp = Ocr(ch, false, true);
                    }
                    else if (yPosition == 290)
                    {
                        temp = Ocr(ch, false, false);
                    }
                    else
                    {
                        temp = Ocr(ch, false, true);
                    }

                    result += temp;

                    if (box[startIndex + i] != null)
                    {
                        box[startIndex + i].Location = new Point(x + i * 50, yPosition);
                        box[startIndex + i].Size = new Size(50, 100);
                        box[startIndex + i].SizeMode = PictureBoxSizeMode.StretchImage;
                        DisposeImage(box[startIndex + i]);
                        box[startIndex + i].Image = new Bitmap(ch);
                        box[startIndex + i].Update();

                        if (IF != null)
                        {
                            IF.Controls.Add(box[startIndex + i]);
                        }
                    }
                }
                c_x++;
            }

            return result;
        }

        private void picInputCam_Click(object sender, EventArgs e) { }
        private void pic_BiensoRa1_Click(object sender, EventArgs e) { }
        private void btnHistory_Click(object sender, EventArgs e) { }

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
                     ? (NewFrameEventHandler)CaptureDevice1_NewFrame 
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
            full_tesseract?.Dispose();
            ch_tesseract?.Dispose();
            num_tesseract?.Dispose();

            _openFileDialog?.Dispose();
            _openFileDialog = null;

            // Dispose PictureBox array
            for (int i = 0; i < box.Length; i++)
            {
                DisposeImage(box[i]);
                box[i]?.Dispose();
            }

            IF?.Dispose();
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
