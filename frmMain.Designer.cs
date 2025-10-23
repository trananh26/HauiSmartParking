using AForge;

namespace Auto_parking
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.STM1_Serial = new System.IO.Ports.SerialPort(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lb_vaora = new System.Windows.Forms.Label();
            this.txt_BiensoVao = new System.Windows.Forms.TextBox();
            this.picInputCam = new System.Windows.Forms.PictureBox();
            this.pic_BiensoVao1 = new System.Windows.Forms.PictureBox();
            this.picInputPicture1 = new System.Windows.Forms.PictureBox();
            this.pic_BiensoVao2 = new System.Windows.Forms.PictureBox();
            this.btn_chupIP = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picOutputPicture1 = new System.Windows.Forms.PictureBox();
            this.picOutputPicture2 = new System.Windows.Forms.PictureBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.picInputPicture2 = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.picOutputCam = new System.Windows.Forms.PictureBox();
            this.pic_BiensoRa1 = new System.Windows.Forms.PictureBox();
            this.txt_BiensoRa = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.pic_BiensoRa2 = new System.Windows.Forms.PictureBox();
            this.btn_chupOP = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblTotalOutput = new System.Windows.Forms.Label();
            this.lblTotalInput = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblTotalMoney = new System.Windows.Forms.Label();
            this.lblEmpty = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pnO5 = new System.Windows.Forms.Panel();
            this.label23 = new System.Windows.Forms.Label();
            this.pnO4 = new System.Windows.Forms.Panel();
            this.label22 = new System.Windows.Forms.Label();
            this.pnO2 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.pnO3 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.pnO1 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblNoti = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblMoney = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblInputTime = new System.Windows.Forms.Label();
            this.lblOutputTime = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.STM2_Serial = new System.IO.Ports.SerialPort(this.components);
            this.tm_AutoReconnect = new System.Windows.Forms.Timer(this.components);
            this.btnHistory = new System.Windows.Forms.Button();
            this.timercheckin = new System.Windows.Forms.Timer(this.components);
            this.timercheckout = new System.Windows.Forms.Timer(this.components);
            this.btnTest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picInputCam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoVao1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInputPicture1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoVao2)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOutputPicture1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOutputPicture2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInputPicture2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOutputCam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoRa1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoRa2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.pnO5.SuspendLayout();
            this.pnO4.SuspendLayout();
            this.pnO2.SuspendLayout();
            this.pnO3.SuspendLayout();
            this.pnO1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "excel 2007|*xlxs";
            // 
            // lb_vaora
            // 
            this.lb_vaora.AutoSize = true;
            this.lb_vaora.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_vaora.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lb_vaora.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lb_vaora.Location = new System.Drawing.Point(1052, 306);
            this.lb_vaora.Name = "lb_vaora";
            this.lb_vaora.Size = new System.Drawing.Size(109, 32);
            this.lb_vaora.TabIndex = 1;
            this.lb_vaora.Text = "READY";
            // 
            // txt_BiensoVao
            // 
            this.txt_BiensoVao.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txt_BiensoVao.Enabled = false;
            this.txt_BiensoVao.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_BiensoVao.Location = new System.Drawing.Point(886, 290);
            this.txt_BiensoVao.Multiline = true;
            this.txt_BiensoVao.Name = "txt_BiensoVao";
            this.txt_BiensoVao.ReadOnly = true;
            this.txt_BiensoVao.Size = new System.Drawing.Size(109, 57);
            this.txt_BiensoVao.TabIndex = 10;
            this.txt_BiensoVao.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // picInputCam
            // 
            this.picInputCam.BackColor = System.Drawing.Color.Snow;
            this.picInputCam.Location = new System.Drawing.Point(9, 47);
            this.picInputCam.Name = "picInputCam";
            this.picInputCam.Size = new System.Drawing.Size(382, 250);
            this.picInputCam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInputCam.TabIndex = 6;
            this.picInputCam.TabStop = false;
            this.picInputCam.Click += new System.EventHandler(this.picInputCam_Click);
            // 
            // pic_BiensoVao1
            // 
            this.pic_BiensoVao1.BackColor = System.Drawing.Color.Gainsboro;
            this.pic_BiensoVao1.Location = new System.Drawing.Point(788, 191);
            this.pic_BiensoVao1.Name = "pic_BiensoVao1";
            this.pic_BiensoVao1.Size = new System.Drawing.Size(150, 90);
            this.pic_BiensoVao1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_BiensoVao1.TabIndex = 4;
            this.pic_BiensoVao1.TabStop = false;
            // 
            // picInputPicture1
            // 
            this.picInputPicture1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.picInputPicture1.Location = new System.Drawing.Point(399, 47);
            this.picInputPicture1.Name = "picInputPicture1";
            this.picInputPicture1.Size = new System.Drawing.Size(361, 250);
            this.picInputPicture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInputPicture1.TabIndex = 2;
            this.picInputPicture1.TabStop = false;
            // 
            // pic_BiensoVao2
            // 
            this.pic_BiensoVao2.BackColor = System.Drawing.Color.Gainsboro;
            this.pic_BiensoVao2.Location = new System.Drawing.Point(944, 191);
            this.pic_BiensoVao2.Name = "pic_BiensoVao2";
            this.pic_BiensoVao2.Size = new System.Drawing.Size(150, 90);
            this.pic_BiensoVao2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_BiensoVao2.TabIndex = 4;
            this.pic_BiensoVao2.TabStop = false;
            // 
            // btn_chupIP
            // 
            this.btn_chupIP.Location = new System.Drawing.Point(392, 18);
            this.btn_chupIP.Name = "btn_chupIP";
            this.btn_chupIP.Size = new System.Drawing.Size(107, 40);
            this.btn_chupIP.TabIndex = 11;
            this.btn_chupIP.Text = "Chup xe vao";
            this.btn_chupIP.UseVisualStyleBackColor = true;
            this.btn_chupIP.Visible = false;
            this.btn_chupIP.Click += new System.EventHandler(this.btn_chup_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.groupBox1.Controls.Add(this.picOutputPicture1);
            this.groupBox1.Controls.Add(this.picInputPicture1);
            this.groupBox1.Controls.Add(this.picOutputPicture2);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.picInputPicture2);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.picOutputCam);
            this.groupBox1.Controls.Add(this.picInputCam);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(4, 143);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(778, 621);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hình ảnh từ hệ thống Camera";
            // 
            // picOutputPicture1
            // 
            this.picOutputPicture1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.picOutputPicture1.Location = new System.Drawing.Point(399, 357);
            this.picOutputPicture1.Name = "picOutputPicture1";
            this.picOutputPicture1.Size = new System.Drawing.Size(361, 250);
            this.picOutputPicture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picOutputPicture1.TabIndex = 14;
            this.picOutputPicture1.TabStop = false;
            // 
            // picOutputPicture2
            // 
            this.picOutputPicture2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.picOutputPicture2.Location = new System.Drawing.Point(432, 386);
            this.picOutputPicture2.Name = "picOutputPicture2";
            this.picOutputPicture2.Size = new System.Drawing.Size(275, 192);
            this.picOutputPicture2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picOutputPicture2.TabIndex = 30;
            this.picOutputPicture2.TabStop = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label18.Location = new System.Drawing.Point(514, 332);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(108, 16);
            this.label18.TabIndex = 29;
            this.label18.Text = "Hình ảnh xe ra";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label19.Location = new System.Drawing.Point(139, 332);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(109, 16);
            this.label19.TabIndex = 28;
            this.label19.Text = "Camera ngõ ra";
            // 
            // picInputPicture2
            // 
            this.picInputPicture2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.picInputPicture2.Location = new System.Drawing.Point(432, 75);
            this.picInputPicture2.Name = "picInputPicture2";
            this.picInputPicture2.Size = new System.Drawing.Size(275, 192);
            this.picInputPicture2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInputPicture2.TabIndex = 1;
            this.picInputPicture2.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(523, 23);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 16);
            this.label12.TabIndex = 27;
            this.label12.Text = "Hình ảnh xe vào";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(134, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 16);
            this.label1.TabIndex = 26;
            this.label1.Text = "Camera ngõ vào";
            // 
            // picOutputCam
            // 
            this.picOutputCam.BackColor = System.Drawing.Color.Snow;
            this.picOutputCam.Location = new System.Drawing.Point(9, 357);
            this.picOutputCam.Name = "picOutputCam";
            this.picOutputCam.Size = new System.Drawing.Size(382, 250);
            this.picOutputCam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picOutputCam.TabIndex = 13;
            this.picOutputCam.TabStop = false;
            // 
            // pic_BiensoRa1
            // 
            this.pic_BiensoRa1.BackColor = System.Drawing.Color.Gainsboro;
            this.pic_BiensoRa1.Location = new System.Drawing.Point(1109, 191);
            this.pic_BiensoRa1.Name = "pic_BiensoRa1";
            this.pic_BiensoRa1.Size = new System.Drawing.Size(150, 90);
            this.pic_BiensoRa1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_BiensoRa1.TabIndex = 56;
            this.pic_BiensoRa1.TabStop = false;
            this.pic_BiensoRa1.Click += new System.EventHandler(this.pic_BiensoRa1_Click);
            // 
            // txt_BiensoRa
            // 
            this.txt_BiensoRa.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txt_BiensoRa.Enabled = false;
            this.txt_BiensoRa.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_BiensoRa.Location = new System.Drawing.Point(1207, 290);
            this.txt_BiensoRa.Multiline = true;
            this.txt_BiensoRa.Name = "txt_BiensoRa";
            this.txt_BiensoRa.ReadOnly = true;
            this.txt_BiensoRa.Size = new System.Drawing.Size(109, 57);
            this.txt_BiensoRa.TabIndex = 57;
            this.txt_BiensoRa.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(889, 170);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(101, 16);
            this.label20.TabIndex = 30;
            this.label20.Text = "Biển số xe vào";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(1215, 170);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(91, 16);
            this.label21.TabIndex = 58;
            this.label21.Text = "Biển số xe ra";
            // 
            // pic_BiensoRa2
            // 
            this.pic_BiensoRa2.BackColor = System.Drawing.Color.Gainsboro;
            this.pic_BiensoRa2.Location = new System.Drawing.Point(1265, 191);
            this.pic_BiensoRa2.Name = "pic_BiensoRa2";
            this.pic_BiensoRa2.Size = new System.Drawing.Size(150, 90);
            this.pic_BiensoRa2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_BiensoRa2.TabIndex = 55;
            this.pic_BiensoRa2.TabStop = false;
            // 
            // btn_chupOP
            // 
            this.btn_chupOP.Location = new System.Drawing.Point(505, 18);
            this.btn_chupOP.Name = "btn_chupOP";
            this.btn_chupOP.Size = new System.Drawing.Size(107, 40);
            this.btn_chupOP.TabIndex = 60;
            this.btn_chupOP.Text = "Chup xe ra";
            this.btn_chupOP.UseVisualStyleBackColor = true;
            this.btn_chupOP.Visible = false;
            this.btn_chupOP.Click += new System.EventHandler(this.btn_chupOP_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblTotalOutput);
            this.groupBox2.Controls.Add(this.lblTotalInput);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.lblTotalMoney);
            this.groupBox2.Controls.Add(this.lblEmpty);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.pnO5);
            this.groupBox2.Controls.Add(this.pnO4);
            this.groupBox2.Controls.Add(this.pnO2);
            this.groupBox2.Controls.Add(this.pnO3);
            this.groupBox2.Controls.Add(this.pnO1);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(788, 545);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(627, 199);
            this.groupBox2.TabIndex = 67;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trạng thái bãi đỗ xe";
            // 
            // lblTotalOutput
            // 
            this.lblTotalOutput.AutoSize = true;
            this.lblTotalOutput.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalOutput.Location = new System.Drawing.Point(147, 56);
            this.lblTotalOutput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalOutput.Name = "lblTotalOutput";
            this.lblTotalOutput.Size = new System.Drawing.Size(23, 16);
            this.lblTotalOutput.TabIndex = 78;
            this.lblTotalOutput.Text = "10";
            // 
            // lblTotalInput
            // 
            this.lblTotalInput.AutoSize = true;
            this.lblTotalInput.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalInput.Location = new System.Drawing.Point(147, 31);
            this.lblTotalInput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalInput.Name = "lblTotalInput";
            this.lblTotalInput.Size = new System.Drawing.Size(23, 16);
            this.lblTotalInput.TabIndex = 77;
            this.lblTotalInput.Text = "10";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(31, 56);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 16);
            this.label10.TabIndex = 76;
            this.label10.Text = "Số lượt xe ra:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(31, 31);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(113, 16);
            this.label13.TabIndex = 75;
            this.label13.Text = "Số lượt xe vào:";
            // 
            // lblTotalMoney
            // 
            this.lblTotalMoney.AutoSize = true;
            this.lblTotalMoney.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalMoney.Location = new System.Drawing.Point(436, 55);
            this.lblTotalMoney.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalMoney.Name = "lblTotalMoney";
            this.lblTotalMoney.Size = new System.Drawing.Size(56, 16);
            this.lblTotalMoney.TabIndex = 74;
            this.lblTotalMoney.Text = "0 Đồng";
            // 
            // lblEmpty
            // 
            this.lblEmpty.AutoSize = true;
            this.lblEmpty.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblEmpty.Location = new System.Drawing.Point(436, 31);
            this.lblEmpty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(15, 16);
            this.lblEmpty.TabIndex = 1;
            this.lblEmpty.Text = "5";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(281, 55);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 16);
            this.label7.TabIndex = 72;
            this.label7.Text = "Doanh thu ngày:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(281, 30);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(153, 16);
            this.label6.TabIndex = 71;
            this.label6.Text = "Số ô chứa còn trống:";
            // 
            // pnO5
            // 
            this.pnO5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnO5.Controls.Add(this.label23);
            this.pnO5.Location = new System.Drawing.Point(500, 122);
            this.pnO5.Name = "pnO5";
            this.pnO5.Size = new System.Drawing.Size(110, 48);
            this.pnO5.TabIndex = 71;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Arial", 16.25F, System.Drawing.FontStyle.Bold);
            this.label23.Location = new System.Drawing.Point(43, 11);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(24, 26);
            this.label23.TabIndex = 81;
            this.label23.Text = "5";
            // 
            // pnO4
            // 
            this.pnO4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnO4.Controls.Add(this.label22);
            this.pnO4.Location = new System.Drawing.Point(381, 122);
            this.pnO4.Name = "pnO4";
            this.pnO4.Size = new System.Drawing.Size(110, 48);
            this.pnO4.TabIndex = 70;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Arial", 16.25F, System.Drawing.FontStyle.Bold);
            this.label22.Location = new System.Drawing.Point(45, 11);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(24, 26);
            this.label22.TabIndex = 81;
            this.label22.Text = "4";
            // 
            // pnO2
            // 
            this.pnO2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnO2.Controls.Add(this.label16);
            this.pnO2.Location = new System.Drawing.Point(142, 122);
            this.pnO2.Name = "pnO2";
            this.pnO2.Size = new System.Drawing.Size(110, 48);
            this.pnO2.TabIndex = 68;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 16.25F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(42, 11);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(24, 26);
            this.label16.TabIndex = 81;
            this.label16.Text = "2";
            // 
            // pnO3
            // 
            this.pnO3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnO3.Controls.Add(this.label17);
            this.pnO3.Location = new System.Drawing.Point(262, 122);
            this.pnO3.Name = "pnO3";
            this.pnO3.Size = new System.Drawing.Size(110, 48);
            this.pnO3.TabIndex = 69;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Arial", 16.25F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(43, 11);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(24, 26);
            this.label17.TabIndex = 81;
            this.label17.Text = "3";
            // 
            // pnO1
            // 
            this.pnO1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnO1.Controls.Add(this.label14);
            this.pnO1.Location = new System.Drawing.Point(22, 122);
            this.pnO1.Name = "pnO1";
            this.pnO1.Size = new System.Drawing.Size(110, 48);
            this.pnO1.TabIndex = 67;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 16.25F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(43, 11);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(24, 26);
            this.label14.TabIndex = 81;
            this.label14.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(359, 94);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(676, 35);
            this.label2.TabIndex = 68;
            this.label2.Text = "HỆ THỐNG QUẢN LÝ BÃI ĐỖ XE THÔNG MINH";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label3.Location = new System.Drawing.Point(501, 16);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(388, 22);
            this.label3.TabIndex = 69;
            this.label3.Text = "TRƯỜNG ĐẠI HỌC CÔNG NGHIỆP HÀ NỘI";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(611, 47);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 22);
            this.label4.TabIndex = 70;
            this.label4.Text = "KHOA ĐIỆN TỬ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnTest);
            this.groupBox3.Controls.Add(this.lblNoti);
            this.groupBox3.Controls.Add(this.btn_chupIP);
            this.groupBox3.Controls.Add(this.btn_chupOP);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(788, 464);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(627, 66);
            this.groupBox3.TabIndex = 71;
            this.groupBox3.TabStop = false;
            // 
            // lblNoti
            // 
            this.lblNoti.AutoSize = true;
            this.lblNoti.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoti.ForeColor = System.Drawing.Color.Red;
            this.lblNoti.Location = new System.Drawing.Point(151, 19);
            this.lblNoti.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNoti.Name = "lblNoti";
            this.lblNoti.Size = new System.Drawing.Size(336, 37);
            this.lblNoti.TabIndex = 79;
            this.lblNoti.Text = "VUI LÒNG QUẸT THẺ";
            this.lblNoti.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(970, 432);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 19);
            this.label9.TabIndex = 72;
            this.label9.Text = "Phí gửi xe:";
            // 
            // lblMoney
            // 
            this.lblMoney.AutoSize = true;
            this.lblMoney.Font = new System.Drawing.Font("Arial", 16.25F, System.Drawing.FontStyle.Bold);
            this.lblMoney.ForeColor = System.Drawing.Color.Maroon;
            this.lblMoney.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMoney.Location = new System.Drawing.Point(1065, 428);
            this.lblMoney.Name = "lblMoney";
            this.lblMoney.Size = new System.Drawing.Size(139, 26);
            this.lblMoney.TabIndex = 73;
            this.lblMoney.Text = "10.000 Đồng";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(833, 379);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(122, 19);
            this.label11.TabIndex = 74;
            this.label11.Text = "Thời gian vào:";
            // 
            // lblInputTime
            // 
            this.lblInputTime.AutoSize = true;
            this.lblInputTime.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.lblInputTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblInputTime.Location = new System.Drawing.Point(954, 380);
            this.lblInputTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInputTime.Name = "lblInputTime";
            this.lblInputTime.Size = new System.Drawing.Size(75, 19);
            this.lblInputTime.TabIndex = 75;
            this.lblInputTime.Text = "00:00:00";
            // 
            // lblOutputTime
            // 
            this.lblOutputTime.AutoSize = true;
            this.lblOutputTime.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.lblOutputTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblOutputTime.Location = new System.Drawing.Point(1245, 380);
            this.lblOutputTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOutputTime.Name = "lblOutputTime";
            this.lblOutputTime.Size = new System.Drawing.Size(75, 19);
            this.lblOutputTime.TabIndex = 77;
            this.lblOutputTime.Text = "00:00:00";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(1138, 379);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(110, 19);
            this.label15.TabIndex = 76;
            this.label15.Text = "Thời gian ra:";
            // 
            // tm_AutoReconnect
            // 
            this.tm_AutoReconnect.Enabled = true;
            this.tm_AutoReconnect.Interval = 2000;
            this.tm_AutoReconnect.Tick += new System.EventHandler(this.tm_AutoReconnect_Tick);
            // 
            // btnHistory
            // 
            this.btnHistory.Location = new System.Drawing.Point(1317, 9);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(98, 40);
            this.btnHistory.TabIndex = 78;
            this.btnHistory.Text = "Lịch sử";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // timercheckin
            // 
            this.timercheckin.Interval = 1000;
            // 
            // timercheckout
            // 
            this.timercheckout.Interval = 1000;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(252, 18);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(107, 40);
            this.btnTest.TabIndex = 80;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1421, 769);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.lblOutputTime);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.lblInputTime);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblMoney);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lb_vaora);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.txt_BiensoRa);
            this.Controls.Add(this.pic_BiensoRa2);
            this.Controls.Add(this.pic_BiensoRa1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pic_BiensoVao2);
            this.Controls.Add(this.pic_BiensoVao1);
            this.Controls.Add(this.txt_BiensoVao);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto_Parking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picInputCam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoVao1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInputPicture1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoVao2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOutputPicture1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOutputPicture2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInputPicture2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOutputCam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoRa1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_BiensoRa2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pnO5.ResumeLayout(false);
            this.pnO5.PerformLayout();
            this.pnO4.ResumeLayout(false);
            this.pnO4.PerformLayout();
            this.pnO2.ResumeLayout(false);
            this.pnO2.PerformLayout();
            this.pnO3.ResumeLayout(false);
            this.pnO3.PerformLayout();
            this.pnO1.ResumeLayout(false);
            this.pnO1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// Handles communication with the entrance gate controller
        /// </summary>
        private System.IO.Ports.SerialPort STM1_Serial;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lb_vaora;
        private System.Windows.Forms.TextBox txt_BiensoVao;
        private System.Windows.Forms.PictureBox picInputCam;
        private System.Windows.Forms.PictureBox pic_BiensoVao1;
        private System.Windows.Forms.PictureBox picInputPicture1;
        private System.Windows.Forms.PictureBox pic_BiensoVao2;
        private System.Windows.Forms.Button btn_chupIP;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox picOutputCam;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picOutputPicture1;
        private System.Windows.Forms.PictureBox picInputPicture2;
        private System.Windows.Forms.PictureBox pic_BiensoRa1;
        private System.Windows.Forms.TextBox txt_BiensoRa;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.PictureBox picOutputPicture2;
        private System.Windows.Forms.PictureBox pic_BiensoRa2;
        private System.Windows.Forms.Button btn_chupOP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel pnO5;
        private System.Windows.Forms.Panel pnO4;
        private System.Windows.Forms.Panel pnO2;
        private System.Windows.Forms.Panel pnO3;
        private System.Windows.Forms.Panel pnO1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblEmpty;
        private System.Windows.Forms.Label lblTotalMoney;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblMoney;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblInputTime;
        private System.Windows.Forms.Label lblOutputTime;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblTotalOutput;
        private System.Windows.Forms.Label lblTotalInput;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label14;

        /// <summary>
        /// Handles communication with the exit gate controller
        /// </summary>
        private System.IO.Ports.SerialPort STM2_Serial;
        private System.Windows.Forms.Timer tm_AutoReconnect;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Label lblNoti;
        private System.Windows.Forms.Timer timercheckin;
        private System.Windows.Forms.Timer timercheckout;
        private System.Windows.Forms.Button btnTest;
    }
}

