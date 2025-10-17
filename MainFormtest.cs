using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.IO.Ports;
using tesseract;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using WindowsFormsApplication1;

namespace Auto_parking
{
    public partial class MainForm : Form
    {
        // object xein;
        delegate void SetTextCallback(string text);
        private clsCommon cls = new clsCommon();
        private ImageForm IF;
        private string o_Sensor;
        private bool IsFire;
        delegate void MyDelegate();
        private bool mInputRequire = true;
        private bool mOutputRequire = true;
        private string _outPlate = "";
        private int _capCount = 0;
        private string IPData = "";
        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        /*
        private void RFID_Analys(string mathe)
        {
            try
            {
                mathe = mathe.Replace("\0", "");
                string bienso = "";
                IPData = mathe;
                // Xử lý dữ liệu được nhận về từ STM32||Arduino
                // Cú pháp prefix_ + Data + x
                // Nếu bắt đầu là i_ : Mã thẻ cửa vào
                // Nếu bắt đầu là o_ :Mã thẻ cửa ra
                // Nếu bắt đầu là s_ :Chuỗi tín hiệu cảm biến báo vị trí

                //Quẹt thẻ vào khi có yêu cầu
                if (mathe.Substring(0, 2) == "i_")
                {
                    MyDelegate dt = delegate ()
                    {
                        string m_rf = mathe.Substring(2, mathe.Length - 2);
                        if (mInputRequire)
                        {
                            ///Cho xe vào bãi
                            CarInput(_outPlate, m_rf);
                            mInputRequire = false;
                        }

                    };
                    this.Invoke(dt);
                }

                //Quẹt thẻ ra khi có yêu cầu
                else if (mathe.Substring(0, 2) == "o_")
                {
                    MyDelegate dt = delegate ()
                    {
                        string m_rf = mathe.Substring(2, mathe.Length - 2);
                        if (mOutputRequire)
                        {
                            if (cls.GetInfor(m_rf, _outPlate).Rows.Count > 0)
                            {
                                //cho xe ra
                                CarOutput(_outPlate, m_rf);
                                mOutputRequire = false;
                            }
                            else
                            {
                                timercheckout.Stop();
                                MessageBox.Show("Biển số xe và thẻ không trùng khớp. Vui lòng kiểm tra lại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }

                    };
                    this.Invoke(dt);
                }

                ///Báo trạng thái ô để xe
                else if (mathe.Substring(0, 1) == "s")
                {
                    MyDelegate dt = delegate ()
                    {
                        SensorAnalys(mathe.Substring(1, 5));// xử lý tín hiệu cảm biến
                    };
                    this.Invoke(dt);
                }

                ///Báo cháy
                else if (mathe.Substring(0, 2) == "f_")
                {
                    if (mathe.Substring(0, 3) == "f_1")
                    {
                        IsFire = true;
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

                //có xe vào
                else if (mathe.Substring(0, 4) == "ci_1")
                {
                    MyDelegate dt = delegate ()
                    {
                        
                        lblOutputTime.Visible = false;
                        lblInputTime.Visible = false;
                        lblMoney.Visible = false;
                        try
                        {
                            bienso = CaptureImageThenRecognize(1);// nhận diện biển số
                            _outPlate = bienso;
                            if (bienso.Length < 5)
                            {
                                if (_capCount < 5)
                                {
                                    _capCount++;
                                    timercheckin.Stop();
                                    timercheckin.Start();
                                }
                                else
                                {
                                    timercheckin.Stop();
                                    MessageBox.Show("KHÔNG THỂ NHẬN DẠNG ĐƯỢC BIỂN SỐ XE. VUI LÒNG DI CHUYỂN LẠI XE VÀO VÙNG ĐỌC CAMERA", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    _capCount = 0;
                                    
                                }

                            }
                            else
                            {
                                _capCount = 0;
                                timercheckin.Stop();
                            }

                            //check biển số có đựơc vào thẳng k
                            if (cls.Check_SystemPlate(bienso))
                            {
                                mInputRequire = false;
                                if (cls.Check_BienSo(bienso))//check xe có đang trong bãi không
                                {
                                    MessageBox.Show("BIỂN SỐ XE ĐÃ TỒN TẠI TRONG BÃI", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    CarInput(bienso, "System");
                                }
                                //Cho xe vào bãi

                            }
                            else
                            {
                                if(!string.IsNullOrEmpty(bienso))
                                {
                                    mInputRequire = true;
                                    lblNoti.Visible = true;
                                }    
                                
                            }
                        }
                        catch (Exception)
                        {

                            RFID_Analys(mathe);
                        }

                    };
                    this.Invoke(dt);
                }

                //có xe ra
                else if (mathe.Substring(0, 4) == "co_1")
                {
                    MyDelegate dt = delegate ()
                    {
                        try
                        {
                            bienso = CaptureImageThenRecognize(2);// nhận diện biển số
                            _outPlate = bienso;
                            if (bienso.Length < 5)
                            {
                                if (_capCount < 5)
                                {
                                    _capCount++;
                                    timercheckout.Stop();
                                    timercheckout.Start();
                                }
                                else
                                {
                                    timercheckout.Stop();
                                    MessageBox.Show("KHÔNG THỂ NHẬN DẠNG ĐƯỢC BIỂN SỐ XE. VUI LÒNG DI CHUYỂN LẠI XE VÀO VÙNG ĐỌC CAMERA", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    _capCount = 0;
                                    
                                }

                            }
                            else
                            {
                                _capCount = 0;
                                timercheckout.Stop();
                            }
                            if (!string.IsNullOrEmpty(bienso))
                            {
                                DataTable dtOut = new DataTable();
                                dtOut = cls.Check_BienSoRa(bienso);
                                //check có phải quẹt thẻ hay không
                                if (dtOut.Rows.Count > 0)
                                {
                                    if (dtOut.Rows[0]["RFCode"].ToString() == "System")
                                    {
                                        CarOutput(bienso, "System");
                                    }
                                    else
                                    {
                                        if(!string.IsNullOrEmpty(bienso))
                                        {
                                            mOutputRequire = true;
                                            lblNoti.Visible = true;
                                        }    
                                        
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("BIỂN SỐ XE KHÔNG TỒN TẠI TRONG BÃI", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (Exception)
                        {

                            RFID_Analys(mathe);
                        }

                    };
                    this.Invoke(dt);
                }


                lblTotalInput.Text = cls.InputCount().ToString("00");
                lblTotalOutput.Text = cls.OutputCount().ToString("00");

            }

            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        /// <summary>
        /// lấy xe ra
        /// </summary>
        /// <param name="bienso"></param>
        /// <param name="mathe"></param>
        private void CarOutput(string bienso, string mathe)
        {
            try
            {
                MessageBox.Show("LẤY XE THÀNH CÔNG", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string m_DataSend = "B111111111111111";
                SendData(m_DataSend);

                lblOutputTime.Visible = true;
                lblInputTime.Visible = true;
                lblMoney.Visible = true;
                DateTime _inputTime;
                DateTime _outputTime;
                DataTable dt = new DataTable();
                dt = cls.GetInfor(mathe, bienso);

                lblOutputTime.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                lblInputTime.Text = dt.Rows[0]["UpdateTime"].ToString();
                Guid ID = new Guid(dt.Rows[0]["ID"].ToString());

                DateTime.TryParse(lblInputTime.Text, out _inputTime);
                DateTime.TryParse(lblOutputTime.Text, out _outputTime);

                lblMoney.Text = "10.000 Đồng";

                cls.LayXe(mathe, ID, 10);
                lb_vaora.Text = "XE RA";
                //Lưu số tiền vào bảng TotalMoney
                cls.SaveMoney(10);
                //Lấy lên tổng doanh thu mới
                lblTotalMoney.Text = cls.GetTotalMoney() + " Đồng";

                lblNoti.Visible = false;
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// Gửi xe vào
        /// </summary>
        /// <param name="bienso"></param>
        /// <param name="mathe"></param>
        private void CarInput(string bienso, string mathe)
        {
            try
            {
                MessageBox.Show("GỬI XE THÀNH CÔNG", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string m_DataSend = "B111111111111111";
                SendDataIN(m_DataSend);
                cls.GuiXe(mathe, bienso);
                lb_vaora.Text = "XE VÀO";

                lblNoti.Visible = false;
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// thực hiện chụp lại ngõ vào sau mỗi 1s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timercheckin_Tick(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(IPData))
            {
                RFID_Analys(IPData);
                timercheckin.Stop();
            }    
            
        }
        /// <summary>
        /// thực hiện chụp lại ngõ ra sau mỗi 1s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timercheckout_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(IPData))
            {
                RFID_Analys(IPData);
            }
        }
        */


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
                                DateTime _inputTime;
                                DateTime _outputTime;
                                DataTable dt = new DataTable();
                                dt = cls.GetInfor(mathe.Substring(2, mathe.Length - 2), bienso);

                                lblOutputTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                lblInputTime.Text = dt.Rows[0]["UpdateTime"].ToString();
                                Guid ID = new Guid(dt.Rows[0]["ID"].ToString());

                               
                                lblMoney.Text = "10.000 Đồng";

                                cls.LayXe(mathe.Substring(2, mathe.Length - 2), ID, 10);
                                lb_vaora.Text = "XE RA";
                                //Lưu số tiền vào bảng TotalMoney
                                cls.SaveMoney(10);
                                //Lấy lên tổng doanh thu mới
                                lblTotalMoney.Text = cls.GetTotalMoney().ToString();
                                //lblTotalMoney.Text = cls.GetTotalMoney().ToString() + ".000 Đồng";
                            }
                            else
                            {
                                MessageBox.Show("XE KHÔNG Ở TRONG BÃI. VUI LÒNG KIỂM TRA LẠI", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        MessageBox.Show("BÃI ĐỖ XE HIỆN ĐÃ ĐẦY. VUI LÒNG GIẢI PHÓNG XE", "CẢNH BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
        ///Gửi tín hiệu xuống STM ngõ ra 
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
        List<string> PlateTextList = new List<string>();
        List<Rectangle> listRect = new List<Rectangle>();
        PictureBox[] box = new PictureBox[12];

        public TesseractProcessor full_tesseract = null;
        public TesseractProcessor ch_tesseract = null;
        public TesseractProcessor num_tesseract = null;
        private string m_path = Application.StartupPath + @"\data\";
        private List<string> lstimages = new List<string>();
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
        private string CaptureImageThenRecognize(int Type)
        {
            try
            {
                //if (mCaptureInput != null || mCameraOutput != null)
                if (captureDevice1 != null || captureDevice2 != null)
                {

                    picInputPicture1.Image = null;
                    picOutputPicture1.Image = null;
                    picInputPicture2.Image = null;
                    picOutputPicture2.Image = null;
                    pic_BiensoRa1.Image = null;
                    pic_BiensoRa2.Image = null;
                    pic_BiensoVao1.Image = null;
                    pic_BiensoVao2.Image = null;
                    txt_BiensoVao.Text = "";
                    txt_BiensoRa.Text = "";
                    lblNoti.Visible = false;

                    IF.pictureBox2.Image = null;
                    if (Type == 1)
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
                        picInputPicture1.Image = temp;
                        picInputPicture1.Update();
                    }
                    else if (Type == 2)
                    {
                        picOutputPicture1.Image = temp;
                        picOutputPicture1.Update();
                    }
                    IF.pictureBox2.Image = temp;
                    IF.pictureBox2.Update();
                    Image temp1;
                    string temp2, temp3;
                    Reconize(m_path + "aa.bmp", Type, out temp1, out temp2, out temp3);
                    if (Type == 1)
                    {
                        picInputPicture2.Image = temp1;

                        if (temp3 == "")
                        {
                            txt_BiensoVao.Text = "";
                        }
                        else
                        {
                            txt_BiensoVao.Text = temp3;
                            temp3 = temp3.Replace("\n", "");
                            temp3 = temp3.Replace("\r", "");
                            // MessageBox.Show("BIỂN SỐ XE LÀ " + temp3, "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        }
                    }
                    else if (Type == 2)
                    {
                        picOutputPicture2.Image = temp1;
                        if (temp3 == "")
                        {
                            txt_BiensoRa.Text = "";
                        }
                        else
                        {
                            txt_BiensoRa.Text = temp3;
                            temp3 = temp3.Replace("\n", "");
                            temp3 = temp3.Replace("\r", "");
                            // MessageBox.Show("BIỂN SỐ XE LÀ " + temp3, "THÔNG BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        }
                    }
                    return temp3;
                }
                else
                    return "";
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.ToString());
                return "";
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
            GetCameraInfor();

            lblMoney.Visible = false;
            lblTotalInput.Text = cls.InputCount().ToString("00");
            lblTotalOutput.Text = cls.OutputCount().ToString("00");
            //Lấy lên tổng doanh thu mới
            lblTotalMoney.Text = cls.GetTotalMoney() + " Đồng";

            //cboSTMPorts.DataSource = SerialPort.GetPortNames();
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

            IF = new ImageForm();

            full_tesseract = new TesseractProcessor();
            bool succeed = full_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            full_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTVXY1234567890").ToString();

            ch_tesseract = new TesseractProcessor();
            succeed = ch_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            ch_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPRSTUVXY").ToString();

            num_tesseract = new TesseractProcessor();
            succeed = num_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            num_tesseract.SetVariable("tessedit_char_whitelist", "1234567890").ToString();


            m_path = System.Environment.CurrentDirectory + "\\";
            //string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < box.Length; i++)
            {
                box[i] = new PictureBox();
            }
        }

        FilterInfoCollection filterInfo;
        VideoCaptureDevice captureDevice1;
        VideoCaptureDevice captureDevice2;


        private void GetCameraInfor()

        {
            try
            {
                ///lấy danh sách camera
                filterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                ///set chọn camera
                captureDevice1 = new VideoCaptureDevice(filterInfo[0].MonikerString);//2
                captureDevice1.NewFrame += CaptureDevice1_NewFrame;
                captureDevice1.Start();

                captureDevice2 = new VideoCaptureDevice(filterInfo[1].MonikerString);
                captureDevice2.NewFrame += CaptureDevice2_NewFrame;
                captureDevice2.Start();
            }
            catch (Exception)
            {

                MessageBox.Show("Không tìm thấy thông tin camera. Vuui lòng kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CaptureDevice2_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            picInputCam.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void CaptureDevice1_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            picOutputCam.Image = (Bitmap)eventArgs.Frame.Clone();
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
                PlateTextList.Clear();
                FileStream fs = new FileStream(urlImage, FileMode.Open, FileAccess.Read);
                Image img = Image.FromStream(fs);
                Bitmap image = new Bitmap(img);
                //pictureBox2.Image = image;
                IF.pictureBox2.Image = image;
                fs.Close();

                FindLicensePlate4(image, out Plate_Draw);
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

        private string Ocr(Bitmap image_s, bool isFull, bool isNum = false)
        {
            string temp = "";
            Image<Gray, byte> src = new Image<Gray, byte>(image_s);
            double ratio = 1;
            while (true)
            {
                ratio = (double)CvInvoke.cvCountNonZero(src) / (src.Width * src.Height);
                if (ratio > 0.5) break;
                src = src.Dilate(2);
            }
            Bitmap image = src.ToBitmap();

            TesseractProcessor ocr;
            if (isFull)
                ocr = full_tesseract;
            else if (isNum)
                ocr = num_tesseract;
            else
                ocr = ch_tesseract;

            int cou = 0;
            ocr.Clear();
            ocr.ClearAdaptiveClassifier();
            temp = ocr.Apply(image);
            while (temp.Length > 3)
            {
                Image<Gray, byte> temp2 = new Image<Gray, byte>(image);
                temp2 = temp2.Erode(2);
                image = temp2.ToBitmap();
                ocr.Clear();
                ocr.ClearAdaptiveClassifier();
                temp = ocr.Apply(image);
                cou++;
                if (cou > 10)
                {
                    temp = "";
                    break;
                }
            }
            return temp;

        }


        public void FindLicensePlate4(Bitmap image, out Image plateDraw)
        {

            plateDraw = null;
            Image<Bgr, byte> frame;
            bool isface = false;
            Bitmap src;
            //pictureBox2.Image = new Image<Gray, byte>(image).ToBitmap();
            Image dst = image;
            HaarCascade haar = new HaarCascade(Application.StartupPath + "\\output-hv-33-x25.xml");
            for (float i = 0; i <= 20; i = i + 3)
            {
                for (float s = -1; s <= 1 && s + i != 1; s += 2)
                {
                    src = RotateImage(dst, i * s);
                    PlateImagesList.Clear();
                    frame = new Image<Bgr, byte>(src);
                    using (Image<Gray, byte> grayframe = new Image<Gray, byte>(src))
                    {
                        var faces =
                       grayframe.DetectHaarCascade(haar, 1.1, 8, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(0, 0))[0];
                        foreach (var face in faces)
                        {
                            Image<Bgr, byte> tmp = frame.Copy();
                            tmp.ROI = face.rect;

                            frame.Draw(face.rect, new Bgr(Color.Blue), 2);

                            PlateImagesList.Add(tmp);

                            isface = true;
                        }
                        if (isface)
                        {
                            Image<Bgr, byte> showimg = frame.Clone();
                            plateDraw = (Image)showimg.ToBitmap();
                            //showimg = frame.Resize(imageBox1.Width, imageBox1.Height, 0);
                            //pictureBox1.Image = showimg.ToBitmap();
                            IF.pictureBox2.Image = showimg.ToBitmap();
                            if (PlateImagesList.Count > 1)
                            {
                                for (int k = 1; k < PlateImagesList.Count; k++)
                                {
                                    if (PlateImagesList[0].Width < PlateImagesList[k].Width)
                                    {
                                        PlateImagesList[0] = PlateImagesList[k];
                                    }
                                }
                            }
                            PlateImagesList[0] = PlateImagesList[0].Resize(400, 400, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                            return;
                        }
                    }
                }
            }

        }

        private void Reconize(string link, int Type, out Image hinhbienso, out string bienso, out string bienso_text)
        {
            //try
            //{
            pic_BiensoVao1.Image = null;
            pic_BiensoVao2.Image = null;
            pic_BiensoRa1.Image = null;
            pic_BiensoRa2.Image = null;


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
                Image<Bgr, byte> src = new Image<Bgr, byte>(PlateImagesList[0].ToBitmap());
                Bitmap grayframe;
                FindContours con = new FindContours();
                Bitmap color;
                int c = con.IdentifyContours(src.ToBitmap(), 50, false, out grayframe, out color, out listRect);
                //int z = con.count;
                if (Type == 1)
                {
                    pic_BiensoVao2.Image = color;
                    IF.pictureBox1.Image = color;
                    hinhbienso = Plate_Draw;
                    pic_BiensoVao1.Image = grayframe;
                    IF.pictureBox3.Image = grayframe;
                }
                else if (Type == 2)
                {
                    pic_BiensoRa2.Image = color;
                    IF.pictureBox1.Image = color;
                    hinhbienso = Plate_Draw;
                    pic_BiensoRa1.Image = grayframe;
                    IF.pictureBox3.Image = grayframe;
                }

                //textBox2.Text = c.ToString();
                Image<Gray, byte> dst = new Image<Gray, byte>(grayframe);
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

                for (int i = 0; i < listRect.Count; i++)
                {
                    Bitmap ch = grayframe.Clone(listRect[i], grayframe.PixelFormat);
                    int cou = 0;
                    full_tesseract.Clear();
                    full_tesseract.ClearAdaptiveClassifier();
                    string temp = full_tesseract.Apply(ch);
                    while (temp.Length > 3)
                    {
                        Image<Gray, byte> temp2 = new Image<Gray, byte>(ch);
                        temp2 = temp2.Erode(2);
                        ch = temp2.ToBitmap();
                        full_tesseract.Clear();
                        full_tesseract.ClearAdaptiveClassifier();
                        temp = full_tesseract.Apply(ch);
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
            //}
            //catch (Exception)
            //{


            //}
        }

        private void resizeInGr(GroupBox gr, ref TextBox tx, ref Label lb, int dis_d, int dis_r_t, int dis_r_l, bool t)
        {
            if (dis_r_t < 0)
            {
                tx.Location = new Point(tx.Location.X, gr.Size.Height - dis_d);
                lb.Location = new Point(lb.Location.X, gr.Size.Height - dis_d);
            }
            else
            {
                tx.Location = new Point(gr.Size.Width - dis_r_t, gr.Size.Height - dis_d);
                lb.Location = new Point(gr.Size.Width - dis_r_l, gr.Size.Height - dis_d);
            }
            if (t)
                tx.Size = new Size(gr.Size.Width - 3 - tx.Location.X, tx.Size.Height);
        }

        private void splitter1_MouseMove(object sender, MouseEventArgs e)
        {

            if (mouseDown)
            {
                int w = Size.Width + (e.X - lastLocation.X);
                if (w < 796)
                {
                    w = 796;
                }
                this.Size = new Size(w, Size.Height);
                this.Update();
            }
        }

        private void splitter2_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                int h = Size.Height + (e.Y - lastLocation.Y);
                if (h < 504)
                {
                    h = 504;
                }
                this.Size = new Size(Size.Width, h);
                this.Update();
            }
        }

        private void panel5_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                int w = Size.Width + (e.X - lastLocation.X);
                if (w < 796)
                {
                    w = 796;
                }
                int h = Size.Height + (e.Y - lastLocation.Y);
                if (h < 504)
                {
                    h = 504;
                }
                this.Size = new Size(w, h);
                this.Update();
            }
        }

        #region WEBCAM
        WEBCAM[] cam = new WEBCAM[3];
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox p = (PictureBox)sender;
                for (int i = 0; i < cam.Length; i++)
                {
                    if (cam[i] != null && cam[i].status == "run" && cam[i].pb == p.Name)
                    {
                        cam[i].Stop();
                        cam[i] = null;
                    }
                }
                ContextMenu m = new ContextMenu();
                List<string> ls = WEBCAM.get_all_cam();
                for (int i = 0; i <= 2 & i < ls.Count; i++)
                {
                    m.MenuItems.Add(ls[i], (s, e2) =>
                    {
                        MenuItem menuItem = s as MenuItem;
                        ContextMenu owner = menuItem.Parent as ContextMenu;
                        PictureBox pb = (PictureBox)owner.SourceControl;
                        if (cam[menuItem.Index] != null && cam[menuItem.Index].status == "run")
                        {
                            cam[menuItem.Index].Stop();
                            //cam[menuItem.Index] = null;
                        }
                        cam[menuItem.Index] = new WEBCAM();
                        cam[menuItem.Index].Start(menuItem.Index);
                        cam[menuItem.Index].put_picturebox(pb.Name);
                    });
                }
                m.Show(p, new Point(e.X, e.Y));
            }
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < cam.Length; i++)
                {
                    if (cam[i] != null && cam[i].status == "run" && cam[i].image != null)
                    {
                        MethodInvoker mi = delegate
                        {
                            PictureBox pb = this.Controls.Find(cam[i].pb, true).FirstOrDefault() as PictureBox;
                            pb.Image = cam[i].image;
                            pb.Update();
                            pb.Invalidate();
                        };
                        if (InvokeRequired)
                        {
                            Invoke(mi);
                            return;
                        }

                        PictureBox pb2 = this.Controls.Find(cam[i].pb, true).FirstOrDefault() as PictureBox;
                        pb2.Image = cam[i].image;
                        pb2.Update();
                        pb2.Invalidate();
                    }
                }
            }
            catch (Exception) { }
        }

        #endregion


        private void btn_chonanh_Click(object sender, EventArgs e)
        {
            //while (true) ;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image (*.bmp; *.jpg; *.jpeg; *.png) |*.bmp; *.jpg; *.jpeg; *.png|All files (*.*)|*.*||";
            dlg.InitialDirectory = Application.StartupPath + "\\ImageTest";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string startupPath = dlg.FileName;

            Image temp1;
            string temp2, temp3;
            Reconize(startupPath, 1, out temp1, out temp2, out temp3);
            picInputPicture2.Image = temp1;
            if (temp3 == "")
                txt_BiensoVao.Text = "Không nhận dạng dc biển số";
            else
                txt_BiensoVao.Text = temp3;
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            frmHistory frm = new frmHistory();
            frm.ShowDialog();
        }

        private void btnCarSys_Click(object sender, EventArgs e)
        {
            frmCarSystem frm = new frmCarSystem();
            frm.ShowDialog();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            string processName = "Auto_Parking";

            // Tìm tất cả các quy trình có tên được chỉ định
            Process[] processes = Process.GetProcessesByName(processName);

            // Kiểm tra xem có quy trình nào không
            if (processes.Length > 0)
            {
                // Lặp qua tất cả các quy trình và tắt chúng
                foreach (Process process in processes)
                {
                    try
                    {
                        process.Kill();

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string processName = "Auto_Parking";

            // Tìm tất cả các quy trình có tên được chỉ định
            Process[] processes = Process.GetProcessesByName(processName);

            // Kiểm tra xem có quy trình nào không
            if (processes.Length > 0)
            {
                // Lặp qua tất cả các quy trình và tắt chúng
                foreach (Process process in processes)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private void pic_BiensoRa1_Click(object sender, EventArgs e)
        {

        }

        private void picInputCam_Click(object sender, EventArgs e)
        {

        }
    }
}
