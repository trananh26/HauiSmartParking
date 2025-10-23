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
//using WindowsFormsApplication1;

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
        }

        #endregion

        #region Methods

        private void RFID_Analys(string mathe)
        {
            string bienso = "";
            // Xử lý dữ liệu được nhận về từ STM32||Arduino
            // Cú pháp prefix_ + Data + x
            // Nếu bắt đầu là i_ : Mã thẻ cửa vào
            // Nếu bắt đầu là o_ :Mã thẻ cửa ra
            // Nếu bắt đầu là s_ :Chuỗi tín hiệu cảm biến báo vị trí
            mathe = mathe.Trim();
            mathe = mathe.Replace("\0", "");
            if (mathe.Substring(0, 2) == "i_")
            {
                bienso = CaptureImageThenRecognize(1);// nhận diện biển số

            }
            else if (mathe.Substring(0, 2) == "o_")
            {
                bienso = CaptureImageThenRecognize(2);// nhận diện biển số

            }
            //else if (mathe.Substring(1, 2) == "o_")
            //{
            //    bienso = CaptureImageThenRecognize(2);// nhận diện biển số

            //}
            else if (mathe.Substring(0, 1) == "s")
            {
                SensorAnalys(mathe.Substring(1, 5));// xử lý tín hiệu cảm biến
            }

            else if (mathe.Substring(0, 2) == "f_")
            {
                if (mathe.Substring(0, 3) == "f_1")
                {
                    IsFire = true;
                    MessageBox.Show("Bãi đỗ xe đang có cảnh báo NGUY HIỂM !!", "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                    string m_DataSend = "1   FIRE EXIT   ";
                    SendData(m_DataSend);
                    Thread.Sleep(1000);

                    m_DataSend = "2 Please Go out ";
                    SendData(m_DataSend);
                    Thread.Sleep(1000);

                }
                else
                {
                    IsFire = false;

                    string m_DataSend = "1    WELCOME    ";
                    SendData(m_DataSend);
                    Thread.Sleep(1000);

                    m_DataSend = "2               ";
                    SendData(m_DataSend);
                    Thread.Sleep(1000);

                }
            }

            if (bienso != string.Empty)
            {
                DialogResult ketqua1 = MessageBox.Show("BIỂN SỐ XE LÀ " + bienso, "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (ketqua1 == DialogResult.OK)
                {

                    if (mathe.Substring(0, 2) == "i_")
                    {
                        lblOutputTime.Visible = false;
                        lblInputTime.Visible = false;
                        lblMoney.Visible = false;

                        if (cls.Check_RF(mathe.Substring(2, mathe.Length - 2)))
                        {
                            if (cls.Check_BienSo(bienso))
                            {
                                MessageBox.Show("BIỂN SỐ XE ĐÃ TỒN TẠI TRONG BÃI", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                string m_DataSend = "B111111111111111";
                                SendDataIN(m_DataSend);
                                cls.GuiXe(mathe.Substring(2, mathe.Length - 2), bienso);
                                lb_vaora.Text = "XE VÀO";
                            }
                        }
                        else
                        {
                            MessageBox.Show("THẺ KHÔNG TỒN TẠI TRONG HỆ THỐNG", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (mathe.Substring(0, 2) == "o_")
                    {
                        if (cls.Check_RF(mathe.Substring(2, mathe.Length - 2)))
                        {
                            if (cls.Check_BienSo(bienso))
                            {
                                string m_DataSend = "B111111111111111";
                                SendData(m_DataSend);

                                lblOutputTime.Visible = true;
                                lblInputTime.Visible = true;
                                lblMoney.Visible = true;
                                DataTable dt = new DataTable();
                                dt = cls.GetInfor(mathe.Substring(2, mathe.Length - 2), bienso);

                                lblOutputTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                lblInputTime.Text = dt.Rows[0]["UpdateTime"].ToString();
                                Guid ID = new Guid(dt.Rows[0]["ID"].ToString());


                                lblMoney.Text = "10.000 Đồng";

                                cls.LayXe(mathe.Substring(2, mathe.Length - 2), ID, 10);
                                lb_vaora.Text = "XE RA";
                                // Lưu số tiền vào bảng TotalMoney
                                cls.SaveMoney(10);
                                // Lấy lên tổng doanh thu mới
                                lblTotalMoney.Text = cls.GetTotalMoney().ToString();
                                // lblTotalMoney.Text = cls.GetTotalMoney().ToString() + ".000 Đồng";
                            }
                            else
                            {
                                MessageBox.Show("XE KHÔNG Ở TRONG BÃI. VUI LÒNG KIỂM TRA LẠi", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("THẺ KHÔNG TỒN TẠI TRONG HỆ THỐNG", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    lblTotalInput.Text = cls.InputCount().ToString("00");
                    lblTotalOutput.Text = cls.OutputCount().ToString("00");
                }
            }
        }

        //Xử lý tín hiệu cảm  biến
        private void SensorAnalys(string SensorData)
        {
            try
            {
                if (SensorData != o_Sensor && !IsFire)
                {
                    string F1 = string.Empty;
                    string F2 = string.Empty;
                    string F3 = string.Empty;
                    string F4 = string.Empty;
                    string F5 = string.Empty;

                    string m_DataSend = string.Empty;
                    int Empty = 0;
                    string p1 = SensorData.Substring(0, 1);
                    string p2 = SensorData.Substring(1, 1);
                    string p3 = SensorData.Substring(2, 1);
                    string p4 = SensorData.Substring(3, 1);
                    string p5 = SensorData.Substring(4, 1);


                    if (p1 == "0") { pnO1.BackColor = Color.Red; F1 = " "; } else { pnO1.BackColor = Color.LightGreen; Empty++; F1 = "1"; }
                    if (p2 == "0") { pnO2.BackColor = Color.Red; F2 = " "; } else { pnO2.BackColor = Color.LightGreen; Empty++; F2 = "2"; }
                    if (p3 == "0") { pnO3.BackColor = Color.Red; F3 = " "; } else { pnO3.BackColor = Color.LightGreen; Empty++; F3 = "3"; }
                    if (p4 == "0") { pnO4.BackColor = Color.Red; F4 = " "; } else { pnO4.BackColor = Color.LightGreen; Empty++; F4 = "4"; }
                    if (p5 == "0") { pnO5.BackColor = Color.Red; F5 = " "; } else { pnO5.BackColor = Color.LightGreen; Empty++; F5 = "5"; }

                    lblEmpty.Text = Empty.ToString("00");

                    if (Empty == 0)
                    {
                        MessageBox.Show("BÃI ĐỖ XE HIỆN ĐÃ ĐẦY. VUI LÒNG GIẢI PHÓNG XE!", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        m_DataSend = "1FULL - No Space";
                        SendData(m_DataSend);
                        Thread.Sleep(1000);

                        m_DataSend = "2 Sorry so much ";
                        SendData(m_DataSend);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        m_DataSend = "1Vacancy Slot: " + Empty.ToString();
                        SendData(m_DataSend);
                        Thread.Sleep(1000);

                        m_DataSend = "2No.:";  //No.: 1,2,3,4,5
                        for (int i = 1; i < 6; i++)
                        {
                            if (SensorData.Substring(i - 1, 1) == "1")
                            {
                                m_DataSend += " " + i.ToString();
                            }
                        }
                        m_DataSend += "           ";
                        m_DataSend = m_DataSend.Substring(0, 16);
                        SendData(m_DataSend);
                        Thread.Sleep(1000);
                    }
                    o_Sensor = SensorData;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gửi tín hiệu xuống STM ngõ vào
        /// </summary>
        /// <param name="data"></param>
        private void SendDataIN(string data)
        {
            char[] Bdata = data.ToCharArray();
            //Bdata[data.Length - 1] = (char)0x03;
            STM1_Serial.Write(Bdata, 0, Bdata.Length);
        }

        /// <summary>
        /// Gửi tín hiệu xuống STM ngõ ra 
        /// </summary>
        /// <param name="data"></param>
        private void SendData(string data)
        {
            char[] Bdata = data.ToCharArray();
            //Bdata[data.Length - 1] = (char)0x03;
            STM2_Serial.Write(Bdata, 0, Bdata.Length);
        }

        #region định nghĩa

        List<Image<Bgr, byte>> PlateImagesList = new List<Image<Bgr, byte>>();
        Image Plate_Draw;
        List<Rectangle> listRect = new List<Rectangle>();
        PictureBox[] box = new PictureBox[12];

        public TesseractEngine full_tesseract = null;
        public TesseractEngine ch_tesseract = null;
        public TesseractEngine num_tesseract = null;
        private string m_path = Application.StartupPath + @"\data\";
        private const string m_lang = "eng";

        //int current = 0;
        //Capture mCaptureInput = null;
        //Capture mCameraOutput = null;

        #endregion


        #region di chuyển
        bool mouseDown = false;
        Point lastLocation;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseDown == false && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //contextMenuStrip1.Show(this.DesktopLocation.X + e.X, this.DesktopLocation.Y + e.Y);	
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
        /// Captures an image from the specified camera,
        /// performs recognition on the captured image,
        /// and returns the recognized license plate number.
        /// </summary>
        /// <remarks>This method resets relevant UI elements before capturing and processing the image. If
        /// the required capture devices are not available, the method returns an empty string.</remarks>
        /// <param name="Type">Specifies which camera to use for image capture. Use 1 for the input camera and 2 for the output camera.</param>
        /// <param name="imageTestPath"">Optional file path to an image for testing purposes. If provided, the method will use this image instead of capturing from a camera.</param>
        /// <returns>A string containing the recognized license plate number. Returns an empty string if recognition fails or no
        /// plate is detected.</returns>
        private string CaptureImageThenRecognize(int Type, string imageTestPath = null)
        {
            try
            {
                if (captureDevice1 != null || captureDevice2 != null)
                {
                    // ✅ Dispose images cũ trước khi gán mới
                    DisposeImage(picInputPicture1);
                    DisposeImage(picOutputPicture1);
                    DisposeImage(picInputPicture2);
                    DisposeImage(picOutputPicture2);
                    DisposeImage(pic_BiensoRa1);
                    DisposeImage(pic_BiensoRa2);
                    DisposeImage(pic_BiensoVao1);
                    DisposeImage(pic_BiensoVao2);
                    DisposeImage(IF.pictureBox2);

                    txt_BiensoVao.Text = "";
                    txt_BiensoRa.Text = "";
                    lblNoti.Visible = false;

                    IF.pictureBox2.Image = null;
                    if (!string.IsNullOrEmpty(imageTestPath))
                    {
                        File.Copy(imageTestPath, m_path + "aa.bmp", true);
                    }
                    else if (Type == 1)
                    {
                        picInputCam.Image.Save("aa.bmp");
                    }
                    else if (Type == 2)
                    {
                        picOutputCam.Image.Save("aa.bmp");
                    }

                    FileStream fs = new FileStream(m_path + "aa.bmp", FileMode.Open, FileAccess.Read);
                    Image temp = Image.FromStream(fs);
                    fs.Close();
                    if (Type == 1)
                    {
                        DisposeImage(picInputPicture1);
                        picInputPicture1.Image = temp;
                        picInputPicture1.Update();
                    }
                    else if (Type == 2)
                    {
                        DisposeImage(picOutputPicture1);
                        picOutputPicture1.Image = temp;
                        picOutputPicture1.Update();
                    }
                    DisposeImage(IF.pictureBox2);
                    IF.pictureBox2.Image = temp;
                    IF.pictureBox2.Update();
                    Recognize(m_path + "aa.bmp", Type, out Image hienBienSo, out string bienSo, out string bienSoText);
                    if (Type == 1)
                    {
                        DisposeImage(picInputPicture2);
                        picInputPicture2.Image = hienBienSo;

                        if (bienSoText == "")
                        {
                            txt_BiensoVao.Text = "";
                        }
                        else
                        {
                            txt_BiensoVao.Text = bienSoText;
                            bienSoText = bienSoText.Replace("\n", "");
                            bienSoText = bienSoText.Replace("\r", "");
                            // MessageBox.Show("BIỂN SỐ XE LÀ " + temp3, "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        }
                    }
                    else if (Type == 2)
                    {
                        DisposeImage(picOutputPicture2);
                        picOutputPicture2.Image = hienBienSo;
                        if (bienSoText == "")
                        {
                            txt_BiensoRa.Text = "";
                        }
                        else
                        {
                            txt_BiensoRa.Text = bienSoText;
                            bienSoText = bienSoText.Replace("\n", "");
                            bienSoText = bienSoText.Replace("\r", "");
                            // MessageBox.Show("BIỂN SỐ XE LÀ " + temp3, "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        }
                    }
                    return bienSoText;
                }
                else
                    return "";
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.ToString());
                return "";
            }
            finally
            {
                // Force cleanup ngay sau khi xử lý
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void tm_AutoReconnect_Tick(object sender, EventArgs e)
        {
            if (STM1_Serial.IsOpen != true)
            {
                try
                {
                    STM1_Serial.PortName = XINIFILE.ReadValue("COM_STM1");
                    STM1_Serial.BaudRate = int.Parse(XINIFILE.ReadValue("BAURATE"));
                    STM1_Serial.Open();
                    STM1_Serial.DataReceived += STM1_Serial_DataReceived;
                }
                catch
                {
                }
            }

            if (STM2_Serial.IsOpen != true)
            {
                try
                {
                    STM2_Serial.PortName = XINIFILE.ReadValue("COM_STM2");
                    STM2_Serial.BaudRate = int.Parse(XINIFILE.ReadValue("BAURATE"));
                    STM2_Serial.Open();
                    STM2_Serial.DataReceived += STM2_Serial_DataReceived;
                }
                catch
                {
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 1. kết nối camera
            GetCameraInfor();

            // 2. Khởi tạo giao diện và dữ liệu
            lblMoney.Visible = false;
            lblTotalInput.Text = cls.InputCount().ToString("00");
            lblTotalOutput.Text = cls.OutputCount().ToString("00");
            lblTotalMoney.Text = cls.GetTotalMoney() + " Đồng"; // Lấy lên tổng doanh thu mới

            // 3. Kết nối cổng Serial, điều khiển cổng vào và cổng ra
            // cboSTMPorts.DataSource = SerialPort.GetPortNames();
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

                string m_DataSend = "1 TRUONG DHCN HN";
                SendData(m_DataSend);
                Thread.Sleep(500);

                m_DataSend = "2 KHOA DIEN TU  ";
                SendData(m_DataSend);
                Thread.Sleep(500);
            }
            catch (Exception)
            {
            }

            IF = new frmImage();

            // 4. Khởi tạo Tesseract với API 5.x
            string testDataPath = Path.Combine(Application.StartupPath, "App_Data", @"data");
            try
            {
                full_tesseract = new TesseractEngine(testDataPath, m_lang, EngineMode.Default);
                full_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTVXY1234567890");

                ch_tesseract = new TesseractEngine(testDataPath, m_lang, EngineMode.Default);
                ch_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTUVXY");

                num_tesseract = new TesseractEngine(testDataPath, m_lang, EngineMode.Default);
                num_tesseract.SetVariable("tessedit_char_whitelist", "1234567890");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo Tesseract OCR: "
                    + ex.Message
                    + Environment.NewLine
                    + "Vui lòng đảm bảo thư mục 'testData' và file 'eng.traineddata' tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            m_path = Environment.CurrentDirectory + "\\";
            //string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < box.Length; i++)
            {
                box[i] = new PictureBox();
            }
        }

        // Helper method
        private void DisposeImage(PictureBox pictureBox)
        {
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
                // lấy danh sách camera
                var filterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                // set chọn camera
                captureDevice1 = new VideoCaptureDevice(filterInfo[0].MonikerString);//2
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

        private void CaptureDevice2_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var oldImage = picInputCam.Image;
            picInputCam.Image = (Bitmap)eventArgs.Frame.Clone();
            oldImage?.Dispose();
        }

        private void CaptureDevice1_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var oldImage = picOutputCam.Image;
            picOutputCam.Image = (Bitmap)eventArgs.Frame.Clone();
            oldImage?.Dispose();
        }

        private void STM2_Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string InputData = string.Empty;
                if (STM2_Serial.BytesToRead > 500)
                {
                    STM2_Serial.DiscardInBuffer();
                    return;
                }

                //InputData = STM2_Serial.ReadExisting();
                //if (InputData.Substring(0, 2) != "f_")
                //{
                InputData = STM2_Serial.ReadTo("x");
                //}
                if (InputData != string.Empty)
                {
                    RFID_Analys(InputData);
                }
            }
            catch
            {
            }
        }

        private void STM1_Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string InputData = string.Empty;
                if (STM1_Serial.BytesToRead > 500)
                {
                    STM1_Serial.DiscardInBuffer();
                    return;
                }
                //InputData = STM1_Serial.ReadExisting();
                //if (InputData.Substring(0, 2) != "f_")
                //{
                InputData = STM1_Serial.ReadTo("x");
                //}

                if (InputData != string.Empty)
                {
                    RFID_Analys(InputData);
                }
            }
            catch
            {
            }
        }

        public void ProcessImage(string urlImage)
        {
            try
            {
                PlateImagesList.Clear();
                using (var fs = new FileStream(urlImage, FileMode.Open, FileAccess.Read))
                {
                    using (var img = Image.FromStream(fs))
                    {
                        using (var image = new Bitmap(img))
                        {
                            //pictureBox2.Image = image;
                            DisposeImage(IF.pictureBox2);
                            IF.pictureBox2.Image = image;
                            fs.Close();

                            FindLicensePlate4(image, out Plate_Draw);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Không tìm được biển số. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PointF offset = new PointF((float)image.Width / 2, (float)image.Height / 2);

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

        /// <summary>
        /// Nhận diện ký tự sử dụng Tesseract OCR
        /// </summary>
        /// <param name="image_s"></param>
        /// <param name="isFull"></param>
        /// <param name="isNum"></param>
        /// <returns></returns>
        private string Ocr(Bitmap image_s, bool isFull, bool isNum = false)
        {
            string temp = "";
            // Convert Bitmap to Image using extension method
            Image<Gray, byte> src = image_s.ToGrayImage();

            double ratio = 1;

            // Count non-zero pixels for Emgu.CV 3.x
            int nonZeroCount = 0;
            using (Mat srcMat = src.Mat)
            using (Mat mask = new Mat())
            {
                Mat zeroMat = new Mat(srcMat.Size, srcMat.Depth, srcMat.NumberOfChannels);
                zeroMat.SetTo(new MCvScalar(0));
                CvInvoke.Compare(srcMat, zeroMat, mask, CmpType.NotEqual);
                nonZeroCount = CvInvoke.CountNonZero(mask);
            }

            while (true)
            {
                ratio = (double)nonZeroCount / (src.Width * src.Height);
                if (ratio > 0.5) break;
                src = src.Dilate(2);

                // Recalculate non-zero count
                using (Mat srcMat = src.Mat)
                using (Mat mask = new Mat())
                {
                    Mat zeroMat = new Mat(srcMat.Size, srcMat.Depth, srcMat.NumberOfChannels);
                    zeroMat.SetTo(new MCvScalar(0));
                    CvInvoke.Compare(srcMat, zeroMat, mask, CmpType.NotEqual);
                    nonZeroCount = CvInvoke.CountNonZero(mask);
                }
            }
            Bitmap image = src.ToBitmap();

            TesseractEngine ocr;
            if (isFull)
                ocr = full_tesseract;
            else if (isNum)
                ocr = num_tesseract;
            else
                ocr = ch_tesseract;

            int cou = 0;

            // Sử dụng API của Tesseract 5.x với Process()
            try
            {
                using (Pix pix = PixConverter.ToPix(image))
                {
                    using (Page page = ocr.Process(pix))
                    {
                        temp = page.GetText().Trim();
                    }
                }

                while (temp.Length > 3)
                {
                    Image<Gray, byte> temp2 = image.ToGrayImage();
                    temp2 = temp2.Erode(2);
                    image = temp2.ToBitmap();

                    using (Pix pix = PixConverter.ToPix(image))
                    {
                        using (Page page = ocr.Process(pix))
                        {
                            temp = page.GetText().Trim();
                        }
                    }

                    cou++;
                    if (cou > 10)
                    {
                        temp = "";
                        break;
                    }
                }
            }
            catch (Exception)
            {
                temp = "";
            }

            return temp;

        }

        /// <summary>
        /// Attempts to detect and highlight a license plate within the specified image using a trained cascade classifier.
        /// </summary>
        /// <remarks>This method uses a cascade classifier to scan the image for license plate-like
        /// regions, applying multiple rotations to improve detection accuracy. The output image will display the
        /// detected region with a visual highlight. If no license plate is detected, the output parameter will be null.
        /// The method does not modify the input image.</remarks>
        /// <param name="image">The source image in which to search for a license plate. Must be a valid, non-null bitmap.</param>
        /// <param name="plateDraw">When the method returns, contains a bitmap with the detected license plate region highlighted, or null if no
        /// plate is found.</param>
        public void FindLicensePlate4(Bitmap image, out Image plateDraw)
        {
            // 1. Chuẩn bị biến để xử lý ảnh với Emgu CV
            plateDraw = null;
            bool isface = false;
            Image dst = image;

            // 2. Sử dụng file XML đã được huấn luyện để nhận diện biển số xe
            using (CascadeClassifier cascade = new CascadeClassifier(Path.Combine(Application.StartupPath, "App_Data", "data", "output-hv-33-x25.xml")))
            {

                // 3. Quét ảnh với nhiều góc xoay
                // Xoay từ -20° đến + 20° với bước nhảy 3°
                for (float i = 0; i <= 20; i = i + 3)
                {
                    for (float s = -1; s <= 1 && s + i != 1; s += 2)
                    {
                        var src = RotateImage(dst, i * s);
                        PlateImagesList.Clear();
                        var frame = src.ToBgrImage();

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
                                var tmp = frame.Copy();
                                tmp.ROI = bestFace;
                                frame.Draw(bestFace, new Bgr(Color.Blue), 2);

                                PlateImagesList.Add(tmp);

                                isface = true;
                            }

                            if (isface)
                            {
                                var showimg = frame.Clone();
                                plateDraw = showimg.ToBitmap();
                                //showimg = frame.Resize(imageBox1.Width, imageBox1.Height, 0);
                                //pictureBox1.Image = showimg.ToBitmap();
                                DisposeImage(IF.pictureBox2);
                                IF.pictureBox2.Image = showimg.ToBitmap();
                                PlateImagesList[0] = PlateImagesList[0].Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear);
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Chọn vùng biển số tốt nhất
        /// </summary>
        /// <param name="detectedRegions">Mảng các vùng được phát hiện</param>
        /// <returns>Vùng Rectangle tốt nhất</returns>
        private Rectangle SelectBestPlateRegion(Rectangle[] detectedRegions)
        {
            if (detectedRegions.Length == 1)
                return detectedRegions[0];

            // 1. Loại bỏ các vùng chứa vùng khác (chọn vùng nhỏ hơn nếu 1 vùng chứa vùng kia)
            List<Rectangle> filteredCandidates = new List<Rectangle>(detectedRegions);
            foreach (Rectangle candidate in detectedRegions)
            {
                foreach (Rectangle other in detectedRegions)
                {
                    if (candidate == other) continue;

                    // Kiểm tra xem candidate có bị chứa trong other không
                    if (IsRectangleContained(candidate, other))
                    {
                        filteredCandidates.Remove(other);
                        break;
                    }
                }
            }

            // 2. Nếu không tìm thấy vùng nào phù hợp chọn vùng lớn nhất
            Rectangle bestRegion = filteredCandidates[0];
            foreach (Rectangle region in filteredCandidates)
            {
                int currentArea = region.Width * region.Height;
                int bestArea = bestRegion.Width * bestRegion.Height;

                if (currentArea > bestArea)
                {
                    bestRegion = region;
                }
            }

            return bestRegion;
        }

        /// <summary>
        /// Kiểm tra xem rectangle inner có bị chứa hoàn toàn trong rectangle outer không
        /// </summary>
        /// <param name="inner">Vùng bên trong</param>
        /// <param name="outer">Vùng bên ngoài</param>
        /// <returns>True nếu inner nằm hoàn toàn trong outer</returns>
        private bool IsRectangleContained(Rectangle inner, Rectangle outer)
        {
            return inner.X >= outer.X &&
                   inner.Y >= outer.Y &&
                   inner.Right <= outer.Right &&
                   inner.Bottom <= outer.Bottom &&
                   !(inner.X == outer.X && inner.Y == outer.Y &&
                     inner.Width == outer.Width && inner.Height == outer.Height);
        }

        /// <summary>
        /// Performs license plate recognition on the specified image and outputs the detected license plate image and
        /// text.
        /// </summary>
        /// <remarks>If no license plate is detected in the image, the output parameters are set to their
        /// default values (null or empty string). The method updates certain UI elements as part of its
        /// operation.</remarks>
        /// <param name="link">The file path or URL of the image to process for license plate recognition. Cannot be null or empty.</param>
        /// <param name="Type">An integer indicating the recognition context. Use 1 for entry recognition and 2 for exit recognition.
        /// Determines which UI elements are updated with the results.</param>
        /// <param name="hinhbienso">When this method returns, contains the image of the detected license plate if recognition is successful;
        /// otherwise, null. This parameter is passed uninitialized.</param>
        /// <param name="bienso">When this method returns, contains the recognized license plate string with formatting removed. This
        /// parameter is passed uninitialized.</param>
        /// <param name="bienso_text">When this method returns, contains the raw recognized license plate text, including line breaks. This
        /// parameter is passed uninitialized.</param>
        private void Recognize(string link, int Type, out Image hinhbienso, out string bienso, out string bienso_text)
        {
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
            ProcessImage(link);
            if (PlateImagesList.Count != 0)
            {
                Bitmap plateBitmap = PlateImagesList[0].ToBitmap();
                Image<Bgr, byte> src = plateBitmap.ToBgrImage();

                Bitmap grayframe;
                FindContours con = new FindContours();
                Bitmap color;
                int c = con.IdentifyContours(src.ToBitmap(), 50, false, out grayframe, out color, out listRect);
                //int z = con.count;
                if (Type == 1)
                {
                    DisposeImage(pic_BiensoVao2);
                    DisposeImage(IF.pictureBox1);
                    DisposeImage(IF.pictureBox3);
                    DisposeImage(pic_BiensoVao1);
                    pic_BiensoVao2.Image = color;
                    IF.pictureBox1.Image = color;
                    hinhbienso = Plate_Draw;
                    pic_BiensoVao1.Image = grayframe;
                    IF.pictureBox3.Image = grayframe;
                }
                else if (Type == 2)
                {
                    DisposeImage(pic_BiensoRa2);
                    DisposeImage(IF.pictureBox1);
                    DisposeImage(IF.pictureBox3);
                    DisposeImage(pic_BiensoRa1);
                    pic_BiensoRa2.Image = color;
                    IF.pictureBox1.Image = color;
                    hinhbienso = Plate_Draw;
                    pic_BiensoRa1.Image = grayframe;
                    IF.pictureBox3.Image = grayframe;
                }

                //textBox2.Text = c.ToString();
                Image<Gray, byte> dst = grayframe.ToGrayImage();
                grayframe = dst.ToBitmap();
                //pictureBox2.Image = grayframe.Clone(listRect[2], grayframe.PixelFormat);
                string zz = "";

                // lọc và sắp xếp số
                List<Bitmap> bmp = new List<Bitmap>();
                List<int> erode = new List<int>();
                List<Rectangle> up = new List<Rectangle>();
                List<Rectangle> dow = new List<Rectangle>();
                int up_y = 0, dow_y = 0;
                bool flag_up = false;

                int di = 0;

                if (listRect == null) return;

                // Sử dụng API của Tesseract 5.x
                for (int i = 0; i < listRect.Count; i++)
                {
                    Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat);
                    int cou = 0;

                    string temp = "";
                    try
                    {
                        using (Pix pix = PixConverter.ToPix(ch))
                        {
                            using (Page page = full_tesseract.Process(pix))
                            {
                                temp = page.GetText().Trim();
                            }
                        }
                    }
                    catch
                    {
                        temp = "";
                    }

                    while (temp.Length > 3)
                    {
                        Image<Gray, byte> temp2 = ch.ToGrayImage();
                        temp2 = temp2.Erode(2);
                        ch = temp2.ToBitmap();

                        try
                        {
                            using (Pix pix = PixConverter.ToPix(ch))
                            {
                                using (Page page = full_tesseract.Process(pix))
                                {
                                    temp = page.GetText().Trim();
                                }
                            }
                        }
                        catch
                        {
                            temp = "";
                        }

                        cou++;
                        if (cou > 10)
                        {
                            listRect.RemoveAt(i);
                            i--;
                            di = 0;
                            break;
                        }
                        di = cou;
                    }
                }

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
                        if (flag_up == true) break;
                    }
                }

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

                if (flag_up == false) dow = listRect;

                for (int i = 0; i < up.Count; i++)
                {
                    for (int j = i; j < up.Count; j++)
                    {
                        if (up[i].X > up[j].X)
                        {
                            Rectangle w = up[i];
                            up[i] = up[j];
                            up[j] = w;
                        }
                    }
                }
                for (int i = 0; i < dow.Count; i++)
                {
                    for (int j = i; j < dow.Count; j++)
                    {
                        if (dow[i].X > dow[j].X)
                        {
                            Rectangle w = dow[i];
                            dow[i] = dow[j];
                            dow[j] = w;
                        }
                    }
                }

                int x = 12;
                int c_x = 0;

                for (int i = 0; i < up.Count; i++)
                {
                    Bitmap ch = grayframe.Clone(up[i], grayframe.PixelFormat);
                    Bitmap o = ch;
                    string temp;
                    if (i < 2)
                    {
                        temp = Ocr(ch, false, true); // nhan dien so
                    }
                    else
                    {
                        temp = Ocr(ch, false, false);// nhan dien chu
                    }

                    zz += temp;
                    box[i].Location = new Point(x + i * 50, 290);
                    box[i].Size = new Size(50, 100);
                    box[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    box[i].Image = ch;
                    box[i].Update();
                    //this.Controls.Add(box[i]);
                    IF.Controls.Add(box[i]);
                    c_x++;
                }
                zz += "\r\n";
                for (int i = 0; i < dow.Count; i++)
                {
                    Bitmap ch = grayframe.Clone(dow[i], grayframe.PixelFormat);
                    //ch = con.Erodetion(ch);
                    string temp = Ocr(ch, false, true); // nhan dien so
                    zz += temp;
                    box[i + c_x].Location = new Point(x + i * 50, 390);
                    box[i + c_x].Size = new Size(50, 100);
                    box[i + c_x].SizeMode = PictureBoxSizeMode.StretchImage;
                    box[i + c_x].Image = ch;
                    box[i + c_x].Update();
                    //this.Controls.Add(box[i + c_x]);
                    IF.Controls.Add(box[i + c_x]);
                }
                bienso = zz.Replace("\n", "");
                bienso = bienso.Replace("\r", "");
                IF.textBox6.Text = zz;
                bienso_text = zz;

            }
        }

        // Add missing event handlers
        private void picInputCam_Click(object sender, EventArgs e)
        {
            // Event handler for picInputCam click
        }

        private void pic_BiensoRa1_Click(object sender, EventArgs e)
        {
            // Event handler for pic_BiensoRa1 click
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            // Event handler for history button
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (captureDevice1 != null && captureDevice1.IsRunning)
                {
                    captureDevice1.Stop();
                }
                if (captureDevice2 != null && captureDevice2.IsRunning)
                {
                    captureDevice2.Stop();
                }
                if (STM1_Serial != null && STM1_Serial.IsOpen)
                {
                    STM1_Serial.Close();
                }
                if (STM2_Serial != null && STM2_Serial.IsOpen)
                {
                    STM2_Serial.Close();
                }
            }
            catch (Exception)
            {
                // Ignore errors during cleanup
            }
        }

        /// <summary>
        /// Handles the FormClosed event for the main form, performing cleanup of resources when the form is closed.
        /// </summary>
        /// <remarks>This method disposes of resources associated with OCR processing to ensure proper
        /// release of unmanaged resources when the main form is closed.</remarks>
        /// <param name="sender">The source of the event, typically the main form instance.</param>
        /// <param name="e">An object containing data related to the form closure event.</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            full_tesseract?.Dispose();
            ch_tesseract?.Dispose();
            num_tesseract?.Dispose();

            // Dispose OpenFileDialog singleton
            if (_openFileDialog != null)
            {
                _openFileDialog.Dispose();
                _openFileDialog = null;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                // Reset lại FileName trước khi sử dụng
                FileDialog.FileName = string.Empty;

                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = FileDialog.FileName;
                    string recognizedPlate = CaptureImageThenRecognize(1, selectedFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

    }

}
