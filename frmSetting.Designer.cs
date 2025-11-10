namespace Auto_parking
{
    partial class frmSetting
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDatabase = new System.Windows.Forms.TabPage();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.numCommandTimeout = new System.Windows.Forms.NumericUpDown();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSerialPort = new System.Windows.Forms.TabPage();
            this.numReconnectInterval = new System.Windows.Forms.NumericUpDown();
            this.cboBaudRate = new System.Windows.Forms.ComboBox();
            this.cboCOM_STM2 = new System.Windows.Forms.ComboBox();
            this.cboCOM_STM1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabCamera = new System.Windows.Forms.TabPage();
            this.numMaxImageDimension = new System.Windows.Forms.NumericUpDown();
            this.numFrameInterval = new System.Windows.Forms.NumericUpDown();
            this.numCamera2Index = new System.Windows.Forms.NumericUpDown();
            this.numCamera1Index = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabRecognition = new System.Windows.Forms.TabPage();
            this.btnBrowseCascade = new System.Windows.Forms.Button();
            this.btnBrowseTesseract = new System.Windows.Forms.Button();
            this.numGrayscaleThreshold = new System.Windows.Forms.NumericUpDown();
            this.txtCascadePath = new System.Windows.Forms.TextBox();
            this.txtTesseractPath = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabSystem = new System.Windows.Forms.TabPage();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.numGCInterval = new System.Windows.Forms.NumericUpDown();
            this.numParkingFee = new System.Windows.Forms.NumericUpDown();
            this.numParkingSlots = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tabAwsSettings = new System.Windows.Forms.TabPage();
            this.chkConvertToUpperCase = new System.Windows.Forms.CheckBox();
            this.chkRemoveExtraSpaces = new System.Windows.Forms.CheckBox();
            this.chkApplyPostProcessing = new System.Windows.Forms.CheckBox();
            this.chkFilterByConfidence = new System.Windows.Forms.CheckBox();
            this.numMinBoundingBoxHeight = new System.Windows.Forms.NumericUpDown();
            this.numMinBoundingBoxWidth = new System.Windows.Forms.NumericUpDown();
            this.numMinConfidenceThreshold = new System.Windows.Forms.NumericUpDown();
            this.cboAwsRegion = new System.Windows.Forms.ComboBox();
            this.txtAwsSecretKey = new System.Windows.Forms.TextBox();
            this.txtAwsAccessKey = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnResetDefault = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCommandTimeout)).BeginInit();
            this.tabSerialPort.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReconnectInterval)).BeginInit();
            this.tabCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxImageDimension)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrameInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamera2Index)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamera1Index)).BeginInit();
            this.tabRecognition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGrayscaleThreshold)).BeginInit();
            this.tabSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGCInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numParkingFee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numParkingSlots)).BeginInit();
            this.tabAwsSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBoundingBoxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBoundingBoxWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinConfidenceThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDatabase);
            this.tabControl1.Controls.Add(this.tabSerialPort);
            this.tabControl1.Controls.Add(this.tabCamera);
            this.tabControl1.Controls.Add(this.tabRecognition);
            this.tabControl1.Controls.Add(this.tabSystem);
            this.tabControl1.Controls.Add(this.tabAwsSettings);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(560, 320);
            this.tabControl1.TabIndex = 0;
            // 
            // tabDatabase
            // 
            this.tabDatabase.Controls.Add(this.btnTestConnection);
            this.tabDatabase.Controls.Add(this.numCommandTimeout);
            this.tabDatabase.Controls.Add(this.txtConnectionString);
            this.tabDatabase.Controls.Add(this.label2);
            this.tabDatabase.Controls.Add(this.label1);
            this.tabDatabase.Location = new System.Drawing.Point(4, 22);
            this.tabDatabase.Name = "tabDatabase";
            this.tabDatabase.Padding = new System.Windows.Forms.Padding(3);
            this.tabDatabase.Size = new System.Drawing.Size(552, 294);
            this.tabDatabase.TabIndex = 0;
            this.tabDatabase.Text = "Database";
            this.tabDatabase.UseVisualStyleBackColor = true;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(20, 120);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(150, 30);
            this.btnTestConnection.TabIndex = 4;
            this.btnTestConnection.Text = "Ki?m tra k?t n?i";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // numCommandTimeout
            // 
            this.numCommandTimeout.Location = new System.Drawing.Point(180, 80);
            this.numCommandTimeout.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numCommandTimeout.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCommandTimeout.Name = "numCommandTimeout";
            this.numCommandTimeout.Size = new System.Drawing.Size(350, 20);
            this.numCommandTimeout.TabIndex = 3;
            this.numCommandTimeout.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(180, 20);
            this.txtConnectionString.Multiline = true;
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(350, 50);
            this.txtConnectionString.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Command Timeout (s):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection String:";
            // 
            // tabSerialPort
            // 
            this.tabSerialPort.Controls.Add(this.numReconnectInterval);
            this.tabSerialPort.Controls.Add(this.cboBaudRate);
            this.tabSerialPort.Controls.Add(this.cboCOM_STM2);
            this.tabSerialPort.Controls.Add(this.cboCOM_STM1);
            this.tabSerialPort.Controls.Add(this.label6);
            this.tabSerialPort.Controls.Add(this.label5);
            this.tabSerialPort.Controls.Add(this.label4);
            this.tabSerialPort.Controls.Add(this.label3);
            this.tabSerialPort.Location = new System.Drawing.Point(4, 22);
            this.tabSerialPort.Name = "tabSerialPort";
            this.tabSerialPort.Padding = new System.Windows.Forms.Padding(3);
            this.tabSerialPort.Size = new System.Drawing.Size(552, 294);
            this.tabSerialPort.TabIndex = 1;
            this.tabSerialPort.Text = "Serial Port";
            this.tabSerialPort.UseVisualStyleBackColor = true;
            // 
            // numReconnectInterval
            // 
            this.numReconnectInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numReconnectInterval.Location = new System.Drawing.Point(180, 110);
            this.numReconnectInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numReconnectInterval.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numReconnectInterval.Name = "numReconnectInterval";
            this.numReconnectInterval.Size = new System.Drawing.Size(350, 20);
            this.numReconnectInterval.TabIndex = 7;
            this.numReconnectInterval.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // cboBaudRate
            // 
            this.cboBaudRate.FormattingEnabled = true;
            this.cboBaudRate.Location = new System.Drawing.Point(180, 80);
            this.cboBaudRate.Name = "cboBaudRate";
            this.cboBaudRate.Size = new System.Drawing.Size(350, 21);
            this.cboBaudRate.TabIndex = 6;
            // 
            // cboCOM_STM2
            // 
            this.cboCOM_STM2.FormattingEnabled = true;
            this.cboCOM_STM2.Location = new System.Drawing.Point(180, 50);
            this.cboCOM_STM2.Name = "cboCOM_STM2";
            this.cboCOM_STM2.Size = new System.Drawing.Size(350, 21);
            this.cboCOM_STM2.TabIndex = 5;
            // 
            // cboCOM_STM1
            // 
            this.cboCOM_STM1.FormattingEnabled = true;
            this.cboCOM_STM1.Location = new System.Drawing.Point(180, 20);
            this.cboCOM_STM1.Name = "cboCOM_STM1";
            this.cboCOM_STM1.Size = new System.Drawing.Size(350, 21);
            this.cboCOM_STM1.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Auto Reconnect Interval (ms):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Baud Rate:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "COM STM2:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "COM STM1:";
            // 
            // tabCamera
            // 
            this.tabCamera.Controls.Add(this.numMaxImageDimension);
            this.tabCamera.Controls.Add(this.numFrameInterval);
            this.tabCamera.Controls.Add(this.numCamera2Index);
            this.tabCamera.Controls.Add(this.numCamera1Index);
            this.tabCamera.Controls.Add(this.label10);
            this.tabCamera.Controls.Add(this.label9);
            this.tabCamera.Controls.Add(this.label8);
            this.tabCamera.Controls.Add(this.label7);
            this.tabCamera.Location = new System.Drawing.Point(4, 22);
            this.tabCamera.Name = "tabCamera";
            this.tabCamera.Size = new System.Drawing.Size(552, 294);
            this.tabCamera.TabIndex = 2;
            this.tabCamera.Text = "Camera";
            this.tabCamera.UseVisualStyleBackColor = true;
            // 
            // numMaxImageDimension
            // 
            this.numMaxImageDimension.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMaxImageDimension.Location = new System.Drawing.Point(180, 110);
            this.numMaxImageDimension.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.numMaxImageDimension.Minimum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.numMaxImageDimension.Name = "numMaxImageDimension";
            this.numMaxImageDimension.Size = new System.Drawing.Size(350, 20);
            this.numMaxImageDimension.TabIndex = 7;
            this.numMaxImageDimension.Value = new decimal(new int[] {
            1280,
            0,
            0,
            0});
            // 
            // numFrameInterval
            // 
            this.numFrameInterval.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numFrameInterval.Location = new System.Drawing.Point(180, 80);
            this.numFrameInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numFrameInterval.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numFrameInterval.Name = "numFrameInterval";
            this.numFrameInterval.Size = new System.Drawing.Size(350, 20);
            this.numFrameInterval.TabIndex = 6;
            this.numFrameInterval.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // numCamera2Index
            // 
            this.numCamera2Index.Location = new System.Drawing.Point(180, 50);
            this.numCamera2Index.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCamera2Index.Name = "numCamera2Index";
            this.numCamera2Index.Size = new System.Drawing.Size(350, 20);
            this.numCamera2Index.TabIndex = 5;
            this.numCamera2Index.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numCamera1Index
            // 
            this.numCamera1Index.Location = new System.Drawing.Point(180, 20);
            this.numCamera1Index.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCamera1Index.Name = "numCamera1Index";
            this.numCamera1Index.Size = new System.Drawing.Size(350, 20);
            this.numCamera1Index.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 112);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(118, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Max Image Dimension:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 82);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Frame Interval (ms):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Camera 2 Index:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Camera 1 Index:";
            // 
            // tabRecognition
            // 
            this.tabRecognition.Controls.Add(this.btnBrowseCascade);
            this.tabRecognition.Controls.Add(this.btnBrowseTesseract);
            this.tabRecognition.Controls.Add(this.numGrayscaleThreshold);
            this.tabRecognition.Controls.Add(this.txtCascadePath);
            this.tabRecognition.Controls.Add(this.txtTesseractPath);
            this.tabRecognition.Controls.Add(this.label13);
            this.tabRecognition.Controls.Add(this.label12);
            this.tabRecognition.Controls.Add(this.label11);
            this.tabRecognition.Location = new System.Drawing.Point(4, 22);
            this.tabRecognition.Name = "tabRecognition";
            this.tabRecognition.Size = new System.Drawing.Size(552, 294);
            this.tabRecognition.TabIndex = 3;
            this.tabRecognition.Text = "Recognition";
            this.tabRecognition.UseVisualStyleBackColor = true;
            // 
            // btnBrowseCascade
            // 
            this.btnBrowseCascade.Location = new System.Drawing.Point(505, 78);
            this.btnBrowseCascade.Name = "btnBrowseCascade";
            this.btnBrowseCascade.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseCascade.TabIndex = 7;
            this.btnBrowseCascade.Text = "...";
            this.btnBrowseCascade.UseVisualStyleBackColor = true;
            this.btnBrowseCascade.Click += new System.EventHandler(this.btnBrowseCascade_Click);
            // 
            // btnBrowseTesseract
            // 
            this.btnBrowseTesseract.Location = new System.Drawing.Point(505, 18);
            this.btnBrowseTesseract.Name = "btnBrowseTesseract";
            this.btnBrowseTesseract.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseTesseract.TabIndex = 6;
            this.btnBrowseTesseract.Text = "...";
            this.btnBrowseTesseract.UseVisualStyleBackColor = true;
            this.btnBrowseTesseract.Click += new System.EventHandler(this.btnBrowseTesseract_Click);
            // 
            // numGrayscaleThreshold
            // 
            this.numGrayscaleThreshold.Location = new System.Drawing.Point(180, 110);
            this.numGrayscaleThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numGrayscaleThreshold.Name = "numGrayscaleThreshold";
            this.numGrayscaleThreshold.Size = new System.Drawing.Size(319, 20);
            this.numGrayscaleThreshold.TabIndex = 5;
            this.numGrayscaleThreshold.Value = new decimal(new int[] {
            44,
            0,
            0,
            0});
            // 
            // txtCascadePath
            // 
            this.txtCascadePath.Location = new System.Drawing.Point(180, 50);
            this.txtCascadePath.Multiline = true;
            this.txtCascadePath.Name = "txtCascadePath";
            this.txtCascadePath.Size = new System.Drawing.Size(319, 50);
            this.txtCascadePath.TabIndex = 4;
            // 
            // txtTesseractPath
            // 
            this.txtTesseractPath.Location = new System.Drawing.Point(180, 20);
            this.txtTesseractPath.Name = "txtTesseractPath";
            this.txtTesseractPath.Size = new System.Drawing.Size(319, 20);
            this.txtTesseractPath.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(20, 112);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(105, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Grayscale Threshold:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 53);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(76, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Cascade Path:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(120, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Tesseract Data Folder:";
            // 
            // tabSystem
            // 
            this.tabSystem.Controls.Add(this.cboLanguage);
            this.tabSystem.Controls.Add(this.numGCInterval);
            this.tabSystem.Controls.Add(this.numParkingFee);
            this.tabSystem.Controls.Add(this.numParkingSlots);
            this.tabSystem.Controls.Add(this.label17);
            this.tabSystem.Controls.Add(this.label16);
            this.tabSystem.Controls.Add(this.label15);
            this.tabSystem.Controls.Add(this.label14);
            this.tabSystem.Location = new System.Drawing.Point(4, 22);
            this.tabSystem.Name = "tabSystem";
            this.tabSystem.Size = new System.Drawing.Size(552, 294);
            this.tabSystem.TabIndex = 4;
            this.tabSystem.Text = "System";
            this.tabSystem.UseVisualStyleBackColor = true;
            // 
            // cboLanguage
            // 
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Items.AddRange(new object[] {
            "vi-VN",
            "en-US"});
            this.cboLanguage.Location = new System.Drawing.Point(180, 110);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(350, 21);
            this.cboLanguage.TabIndex = 7;
            // 
            // numGCInterval
            // 
            this.numGCInterval.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numGCInterval.Location = new System.Drawing.Point(180, 80);
            this.numGCInterval.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numGCInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numGCInterval.Name = "numGCInterval";
            this.numGCInterval.Size = new System.Drawing.Size(350, 20);
            this.numGCInterval.TabIndex = 6;
            this.numGCInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // numParkingFee
            // 
            this.numParkingFee.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numParkingFee.Location = new System.Drawing.Point(180, 50);
            this.numParkingFee.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numParkingFee.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numParkingFee.Name = "numParkingFee";
            this.numParkingFee.Size = new System.Drawing.Size(350, 20);
            this.numParkingFee.TabIndex = 5;
            this.numParkingFee.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // numParkingSlots
            // 
            this.numParkingSlots.Location = new System.Drawing.Point(180, 20);
            this.numParkingSlots.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numParkingSlots.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numParkingSlots.Name = "numParkingSlots";
            this.numParkingSlots.Size = new System.Drawing.Size(350, 20);
            this.numParkingSlots.TabIndex = 4;
            this.numParkingSlots.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(20, 113);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(58, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Language:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(20, 82);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(99, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "GC Interval (sec):";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(20, 52);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(130, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "Parking Fee Per Unit (VND):";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(20, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Parking Slots:";
            // 
            // tabAwsSettings
            // 
            this.tabAwsSettings.Controls.Add(this.chkConvertToUpperCase);
            this.tabAwsSettings.Controls.Add(this.chkRemoveExtraSpaces);
            this.tabAwsSettings.Controls.Add(this.chkApplyPostProcessing);
            this.tabAwsSettings.Controls.Add(this.chkFilterByConfidence);
            this.tabAwsSettings.Controls.Add(this.numMinBoundingBoxHeight);
            this.tabAwsSettings.Controls.Add(this.numMinBoundingBoxWidth);
            this.tabAwsSettings.Controls.Add(this.numMinConfidenceThreshold);
            this.tabAwsSettings.Controls.Add(this.cboAwsRegion);
            this.tabAwsSettings.Controls.Add(this.txtAwsSecretKey);
            this.tabAwsSettings.Controls.Add(this.txtAwsAccessKey);
            this.tabAwsSettings.Controls.Add(this.label25);
            this.tabAwsSettings.Controls.Add(this.label24);
            this.tabAwsSettings.Controls.Add(this.label23);
            this.tabAwsSettings.Controls.Add(this.label22);
            this.tabAwsSettings.Controls.Add(this.label21);
            this.tabAwsSettings.Controls.Add(this.label20);
            this.tabAwsSettings.Controls.Add(this.label19);
            this.tabAwsSettings.Controls.Add(this.label18);
            this.tabAwsSettings.Location = new System.Drawing.Point(4, 22);
            this.tabAwsSettings.Name = "tabAwsSettings";
            this.tabAwsSettings.Size = new System.Drawing.Size(552, 294);
            this.tabAwsSettings.TabIndex = 5;
            this.tabAwsSettings.Text = "AWS Settings";
            this.tabAwsSettings.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(20, 23);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(93, 13);
            this.label18.TabIndex = 0;
            this.label18.Text = "AWS Access Key:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(20, 53);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(91, 13);
            this.label19.TabIndex = 1;
            this.label19.Text = "AWS Secret Key:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(20, 83);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(72, 13);
            this.label20.TabIndex = 2;
            this.label20.Text = "AWS Region:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(20, 113);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(142, 13);
            this.label21.TabIndex = 3;
            this.label21.Text = "Min Confidence Threshold (%):";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(20, 143);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(137, 13);
            this.label22.TabIndex = 4;
            this.label22.Text = "Min Bounding Box Width:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(20, 173);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(139, 13);
            this.label23.TabIndex = 5;
            this.label23.Text = "Min Bounding Box Height:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(20, 200);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(114, 13);
            this.label24.TabIndex = 6;
            this.label24.Text = "Post-Processing:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label25.Location = new System.Drawing.Point(177, 143);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(34, 13);
            this.label25.TabIndex = 7;
            this.label25.Text = "(0-1)";
            // 
            // txtAwsAccessKey
            // 
            this.txtAwsAccessKey.Location = new System.Drawing.Point(180, 20);
            this.txtAwsAccessKey.Name = "txtAwsAccessKey";
            this.txtAwsAccessKey.Size = new System.Drawing.Size(350, 20);
            this.txtAwsAccessKey.TabIndex = 8;
            // 
            // txtAwsSecretKey
            // 
            this.txtAwsSecretKey.Location = new System.Drawing.Point(180, 50);
            this.txtAwsSecretKey.Name = "txtAwsSecretKey";
            this.txtAwsSecretKey.PasswordChar = '*';
            this.txtAwsSecretKey.Size = new System.Drawing.Size(350, 20);
            this.txtAwsSecretKey.TabIndex = 9;
            // 
            // cboAwsRegion
            // 
            this.cboAwsRegion.FormattingEnabled = true;
            this.cboAwsRegion.Items.AddRange(new object[] {
            "ap-southeast-1",
            "us-east-1",
            "us-west-2",
            "eu-west-1",
            "ap-northeast-1"});
            this.cboAwsRegion.Location = new System.Drawing.Point(180, 80);
            this.cboAwsRegion.Name = "cboAwsRegion";
            this.cboAwsRegion.Size = new System.Drawing.Size(350, 21);
            this.cboAwsRegion.TabIndex = 10;
            // 
            // numMinConfidenceThreshold
            // 
            this.numMinConfidenceThreshold.DecimalPlaces = 1;
            this.numMinConfidenceThreshold.Location = new System.Drawing.Point(180, 110);
            this.numMinConfidenceThreshold.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMinConfidenceThreshold.Name = "numMinConfidenceThreshold";
            this.numMinConfidenceThreshold.Size = new System.Drawing.Size(350, 20);
            this.numMinConfidenceThreshold.TabIndex = 11;
            this.numMinConfidenceThreshold.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // numMinBoundingBoxWidth
            // 
            this.numMinBoundingBoxWidth.DecimalPlaces = 3;
            this.numMinBoundingBoxWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numMinBoundingBoxWidth.Location = new System.Drawing.Point(180, 140);
            this.numMinBoundingBoxWidth.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinBoundingBoxWidth.Name = "numMinBoundingBoxWidth";
            this.numMinBoundingBoxWidth.Size = new System.Drawing.Size(350, 20);
            this.numMinBoundingBoxWidth.TabIndex = 12;
            this.numMinBoundingBoxWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            196608});
            // 
            // numMinBoundingBoxHeight
            // 
            this.numMinBoundingBoxHeight.DecimalPlaces = 3;
            this.numMinBoundingBoxHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numMinBoundingBoxHeight.Location = new System.Drawing.Point(180, 170);
            this.numMinBoundingBoxHeight.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinBoundingBoxHeight.Name = "numMinBoundingBoxHeight";
            this.numMinBoundingBoxHeight.Size = new System.Drawing.Size(350, 20);
            this.numMinBoundingBoxHeight.TabIndex = 13;
            this.numMinBoundingBoxHeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            196608});
            // 
            // chkFilterByConfidence
            // 
            this.chkFilterByConfidence.AutoSize = true;
            this.chkFilterByConfidence.Checked = true;
            this.chkFilterByConfidence.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFilterByConfidence.Location = new System.Drawing.Point(40, 220);
            this.chkFilterByConfidence.Name = "chkFilterByConfidence";
            this.chkFilterByConfidence.Size = new System.Drawing.Size(136, 17);
            this.chkFilterByConfidence.TabIndex = 14;
            this.chkFilterByConfidence.Text = "Filter By Confidence";
            this.chkFilterByConfidence.UseVisualStyleBackColor = true;
            // 
            // chkApplyPostProcessing
            // 
            this.chkApplyPostProcessing.AutoSize = true;
            this.chkApplyPostProcessing.Checked = true;
            this.chkApplyPostProcessing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkApplyPostProcessing.Location = new System.Drawing.Point(40, 243);
            this.chkApplyPostProcessing.Name = "chkApplyPostProcessing";
            this.chkApplyPostProcessing.Size = new System.Drawing.Size(149, 17);
            this.chkApplyPostProcessing.TabIndex = 15;
            this.chkApplyPostProcessing.Text = "Apply Post-Processing";
            this.chkApplyPostProcessing.UseVisualStyleBackColor = true;
            // 
            // chkRemoveExtraSpaces
            // 
            this.chkRemoveExtraSpaces.AutoSize = true;
            this.chkRemoveExtraSpaces.Checked = true;
            this.chkRemoveExtraSpaces.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveExtraSpaces.Location = new System.Drawing.Point(60, 266);
            this.chkRemoveExtraSpaces.Name = "chkRemoveExtraSpaces";
            this.chkRemoveExtraSpaces.Size = new System.Drawing.Size(146, 17);
            this.chkRemoveExtraSpaces.TabIndex = 16;
            this.chkRemoveExtraSpaces.Text = "Remove Extra Spaces";
            this.chkRemoveExtraSpaces.UseVisualStyleBackColor = true;
            // 
            // chkConvertToUpperCase
            // 
            this.chkConvertToUpperCase.AutoSize = true;
            this.chkConvertToUpperCase.Checked = true;
            this.chkConvertToUpperCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkConvertToUpperCase.Location = new System.Drawing.Point(230, 266);
            this.chkConvertToUpperCase.Name = "chkConvertToUpperCase";
            this.chkConvertToUpperCase.Size = new System.Drawing.Size(149, 17);
            this.chkConvertToUpperCase.TabIndex = 17;
            this.chkConvertToUpperCase.Text = "Convert To Upper Case";
            this.chkConvertToUpperCase.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(320, 345);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 35);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "L?u";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(450, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "H?y";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnResetDefault
            // 
            this.btnResetDefault.Location = new System.Drawing.Point(12, 345);
            this.btnResetDefault.Name = "btnResetDefault";
            this.btnResetDefault.Size = new System.Drawing.Size(150, 35);
            this.btnResetDefault.TabIndex = 3;
            this.btnResetDefault.Text = "Khôi ph?c m?c ??nh";
            this.btnResetDefault.UseVisualStyleBackColor = true;
            this.btnResetDefault.Click += new System.EventHandler(this.btnResetDefault_Click);
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 391);
            this.Controls.Add(this.btnResetDefault);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "C?u hình h? th?ng";
            this.Load += new System.EventHandler(this.frmSetting_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabDatabase.ResumeLayout(false);
            this.tabDatabase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCommandTimeout)).EndInit();
            this.tabSerialPort.ResumeLayout(false);
            this.tabSerialPort.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReconnectInterval)).EndInit();
            this.tabCamera.ResumeLayout(false);
            this.tabCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxImageDimension)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrameInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamera2Index)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamera1Index)).EndInit();
            this.tabRecognition.ResumeLayout(false);
            this.tabRecognition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGrayscaleThreshold)).EndInit();
            this.tabSystem.ResumeLayout(false);
            this.tabSystem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGCInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numParkingFee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numParkingSlots)).EndInit();
            this.tabAwsSettings.ResumeLayout(false);
            this.tabAwsSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBoundingBoxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBoundingBoxWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinConfidenceThreshold)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDatabase;
        private System.Windows.Forms.TabPage tabSerialPort;
        private System.Windows.Forms.TabPage tabCamera;
        private System.Windows.Forms.TabPage tabRecognition;
        private System.Windows.Forms.TabPage tabSystem;
        private System.Windows.Forms.TabPage tabAwsSettings;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numCommandTimeout;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numReconnectInterval;
        private System.Windows.Forms.ComboBox cboBaudRate;
        private System.Windows.Forms.ComboBox cboCOM_STM2;
        private System.Windows.Forms.ComboBox cboCOM_STM1;
        private System.Windows.Forms.NumericUpDown numMaxImageDimension;
        private System.Windows.Forms.NumericUpDown numFrameInterval;
        private System.Windows.Forms.NumericUpDown numCamera2Index;
        private System.Windows.Forms.NumericUpDown numCamera1Index;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnBrowseCascade;
        private System.Windows.Forms.Button btnBrowseTesseract;
        private System.Windows.Forms.NumericUpDown numGrayscaleThreshold;
        private System.Windows.Forms.TextBox txtCascadePath;
        private System.Windows.Forms.TextBox txtTesseractPath;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.NumericUpDown numGCInterval;
        private System.Windows.Forms.NumericUpDown numParkingFee;
        private System.Windows.Forms.NumericUpDown numParkingSlots;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtAwsAccessKey;
        private System.Windows.Forms.TextBox txtAwsSecretKey;
        private System.Windows.Forms.ComboBox cboAwsRegion;
        private System.Windows.Forms.NumericUpDown numMinConfidenceThreshold;
        private System.Windows.Forms.NumericUpDown numMinBoundingBoxWidth;
        private System.Windows.Forms.NumericUpDown numMinBoundingBoxHeight;
        private System.Windows.Forms.CheckBox chkFilterByConfidence;
        private System.Windows.Forms.CheckBox chkApplyPostProcessing;
        private System.Windows.Forms.CheckBox chkRemoveExtraSpaces;
        private System.Windows.Forms.CheckBox chkConvertToUpperCase;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnResetDefault;
    }
}
