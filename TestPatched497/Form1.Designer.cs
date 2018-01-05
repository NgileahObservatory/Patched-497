namespace ASCOM.LX90
{
   partial class LX90TestForm
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
         this.buttonChoose = new System.Windows.Forms.Button();
         this.buttonConnect = new System.Windows.Forms.Button();
         this.labelDriverId = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.utcDateLabel = new System.Windows.Forms.Label();
         this.latLabel = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.lonLabel = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.elevatonLabel = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.raLabel = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.decLabel = new System.Windows.Forms.Label();
         this.label11 = new System.Windows.Forms.Label();
         this.azLabel = new System.Windows.Forms.Label();
         this.label13 = new System.Windows.Forms.Label();
         this.altLabel = new System.Windows.Forms.Label();
         this.label15 = new System.Windows.Forms.Label();
         this.lstLabel = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.primaryAxisGroupBox = new System.Windows.Forms.GroupBox();
         this.ewPulseGuideNumericUpDown = new System.Windows.Forms.NumericUpDown();
         this.eCheckBox = new System.Windows.Forms.CheckBox();
         this.wCheckBox = new System.Windows.Forms.CheckBox();
         this.slewingRadioButton = new System.Windows.Forms.RadioButton();
         this.ewPulseGuidingRadioButton = new System.Windows.Forms.RadioButton();
         this.secondaryAxisGroupBox = new System.Windows.Forms.GroupBox();
         this.nsPulseGuideNumericUpDown = new System.Windows.Forms.NumericUpDown();
         this.sCheckBox = new System.Windows.Forms.CheckBox();
         this.nCheckBox = new System.Windows.Forms.CheckBox();
         this.radioButton1 = new System.Windows.Forms.RadioButton();
         this.nsPulseGuidingRadioButton = new System.Windows.Forms.RadioButton();
         this.bothAxesGroupBox = new System.Windows.Forms.GroupBox();
         this.slewToCoordButton = new System.Windows.Forms.Button();
         this.slewToAltAzButton = new System.Windows.Forms.Button();
         this.stopButton = new System.Windows.Forms.Button();
         this.label6 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.decTextBox = new System.Windows.Forms.TextBox();
         this.raTextBox = new System.Windows.Forms.TextBox();
         this.altTextBox = new System.Windows.Forms.TextBox();
         this.azTextBox = new System.Windows.Forms.TextBox();
         this.trackingRateComboBox = new System.Windows.Forms.ComboBox();
         this.trackingRateLabel = new System.Windows.Forms.Label();
         this.slewRateLabel = new System.Windows.Forms.Label();
         this.slewRateComboBox = new System.Windows.Forms.ComboBox();
         this.parkButton = new System.Windows.Forms.Button();
         this.stressRAGuidingButton = new System.Windows.Forms.Button();
         this.stressREGuidingButton = new System.Windows.Forms.Button();
         this.primaryAxisGroupBox.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.ewPulseGuideNumericUpDown)).BeginInit();
         this.secondaryAxisGroupBox.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nsPulseGuideNumericUpDown)).BeginInit();
         this.bothAxesGroupBox.SuspendLayout();
         this.SuspendLayout();
         // 
         // buttonChoose
         // 
         this.buttonChoose.Location = new System.Drawing.Point(309, 10);
         this.buttonChoose.Name = "buttonChoose";
         this.buttonChoose.Size = new System.Drawing.Size(72, 23);
         this.buttonChoose.TabIndex = 0;
         this.buttonChoose.Text = "Choose";
         this.buttonChoose.UseVisualStyleBackColor = true;
         this.buttonChoose.Click += new System.EventHandler(this.buttonChoose_Click);
         // 
         // buttonConnect
         // 
         this.buttonConnect.Location = new System.Drawing.Point(309, 39);
         this.buttonConnect.Name = "buttonConnect";
         this.buttonConnect.Size = new System.Drawing.Size(72, 23);
         this.buttonConnect.TabIndex = 1;
         this.buttonConnect.Text = "Connect";
         this.buttonConnect.UseVisualStyleBackColor = true;
         this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
         // 
         // labelDriverId
         // 
         this.labelDriverId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.labelDriverId.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ASCOM.LX90.Properties.Settings.Default, "DriverId", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
         this.labelDriverId.Location = new System.Drawing.Point(12, 40);
         this.labelDriverId.Name = "labelDriverId";
         this.labelDriverId.Size = new System.Drawing.Size(291, 21);
         this.labelDriverId.TabIndex = 2;
         this.labelDriverId.Text = global::ASCOM.LX90.Properties.Settings.Default.DriverId;
         this.labelDriverId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(56, 104);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(55, 13);
         this.label1.TabIndex = 3;
         this.label1.Text = "UTC Date";
         this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // utcDateLabel
         // 
         this.utcDateLabel.AutoSize = true;
         this.utcDateLabel.Location = new System.Drawing.Point(117, 104);
         this.utcDateLabel.Name = "utcDateLabel";
         this.utcDateLabel.Size = new System.Drawing.Size(65, 13);
         this.utcDateLabel.TabIndex = 4;
         this.utcDateLabel.Text = "MM/DD/YY";
         // 
         // latLabel
         // 
         this.latLabel.AutoSize = true;
         this.latLabel.Location = new System.Drawing.Point(117, 126);
         this.latLabel.Name = "latLabel";
         this.latLabel.Size = new System.Drawing.Size(57, 13);
         this.latLabel.TabIndex = 6;
         this.latLabel.Text = "sDDD.MM";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(66, 126);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(45, 13);
         this.label3.TabIndex = 5;
         this.label3.Text = "Latitude";
         this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // lonLabel
         // 
         this.lonLabel.AutoSize = true;
         this.lonLabel.Location = new System.Drawing.Point(117, 147);
         this.lonLabel.Name = "lonLabel";
         this.lonLabel.Size = new System.Drawing.Size(52, 13);
         this.lonLabel.TabIndex = 8;
         this.lonLabel.Text = "DDD.MM";
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(57, 147);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(54, 13);
         this.label5.TabIndex = 7;
         this.label5.Text = "Longitude";
         this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // elevatonLabel
         // 
         this.elevatonLabel.AutoSize = true;
         this.elevatonLabel.Location = new System.Drawing.Point(117, 169);
         this.elevatonLabel.Name = "elevatonLabel";
         this.elevatonLabel.Size = new System.Drawing.Size(39, 13);
         this.elevatonLabel.TabIndex = 10;
         this.elevatonLabel.Text = "DDDm";
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Location = new System.Drawing.Point(9, 169);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(102, 13);
         this.label7.TabIndex = 9;
         this.label7.Text = "Altitude > Sea Level";
         this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // raLabel
         // 
         this.raLabel.AutoSize = true;
         this.raLabel.Location = new System.Drawing.Point(262, 104);
         this.raLabel.Name = "raLabel";
         this.raLabel.Size = new System.Drawing.Size(52, 13);
         this.raLabel.TabIndex = 12;
         this.raLabel.Text = "DDD.MM";
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.Location = new System.Drawing.Point(225, 104);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(22, 13);
         this.label9.TabIndex = 11;
         this.label9.Text = "RA";
         this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // decLabel
         // 
         this.decLabel.AutoSize = true;
         this.decLabel.Location = new System.Drawing.Point(257, 126);
         this.decLabel.Name = "decLabel";
         this.decLabel.Size = new System.Drawing.Size(57, 13);
         this.decLabel.TabIndex = 14;
         this.decLabel.Text = "sDDD.MM";
         // 
         // label11
         // 
         this.label11.AutoSize = true;
         this.label11.Location = new System.Drawing.Point(220, 126);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(27, 13);
         this.label11.TabIndex = 13;
         this.label11.Text = "Dec";
         this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // azLabel
         // 
         this.azLabel.AutoSize = true;
         this.azLabel.Location = new System.Drawing.Point(257, 147);
         this.azLabel.Name = "azLabel";
         this.azLabel.Size = new System.Drawing.Size(52, 13);
         this.azLabel.TabIndex = 16;
         this.azLabel.Text = "DDD.MM";
         // 
         // label13
         // 
         this.label13.AutoSize = true;
         this.label13.Location = new System.Drawing.Point(225, 147);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(19, 13);
         this.label13.TabIndex = 15;
         this.label13.Text = "Az";
         this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // altLabel
         // 
         this.altLabel.AutoSize = true;
         this.altLabel.Location = new System.Drawing.Point(257, 169);
         this.altLabel.Name = "altLabel";
         this.altLabel.Size = new System.Drawing.Size(57, 13);
         this.altLabel.TabIndex = 18;
         this.altLabel.Text = "sDDD.MM";
         // 
         // label15
         // 
         this.label15.AutoSize = true;
         this.label15.Location = new System.Drawing.Point(225, 169);
         this.label15.Name = "label15";
         this.label15.Size = new System.Drawing.Size(19, 13);
         this.label15.TabIndex = 17;
         this.label15.Text = "Alt";
         this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // lstLabel
         // 
         this.lstLabel.AutoSize = true;
         this.lstLabel.Location = new System.Drawing.Point(257, 193);
         this.lstLabel.Name = "lstLabel";
         this.lstLabel.Size = new System.Drawing.Size(69, 13);
         this.lstLabel.TabIndex = 20;
         this.lstLabel.Text = "HH:MM:SS.s";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(147, 193);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(100, 13);
         this.label4.TabIndex = 19;
         this.label4.Text = "Local Sidereal Time";
         this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // primaryAxisGroupBox
         // 
         this.primaryAxisGroupBox.Controls.Add(this.ewPulseGuideNumericUpDown);
         this.primaryAxisGroupBox.Controls.Add(this.eCheckBox);
         this.primaryAxisGroupBox.Controls.Add(this.wCheckBox);
         this.primaryAxisGroupBox.Controls.Add(this.slewingRadioButton);
         this.primaryAxisGroupBox.Controls.Add(this.ewPulseGuidingRadioButton);
         this.primaryAxisGroupBox.Location = new System.Drawing.Point(18, 250);
         this.primaryAxisGroupBox.Name = "primaryAxisGroupBox";
         this.primaryAxisGroupBox.Size = new System.Drawing.Size(164, 132);
         this.primaryAxisGroupBox.TabIndex = 21;
         this.primaryAxisGroupBox.TabStop = false;
         this.primaryAxisGroupBox.Text = "Primary Axis";
         // 
         // ewPulseGuideNumericUpDown
         // 
         this.ewPulseGuideNumericUpDown.Location = new System.Drawing.Point(102, 24);
         this.ewPulseGuideNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
         this.ewPulseGuideNumericUpDown.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
         this.ewPulseGuideNumericUpDown.Name = "ewPulseGuideNumericUpDown";
         this.ewPulseGuideNumericUpDown.Size = new System.Drawing.Size(52, 20);
         this.ewPulseGuideNumericUpDown.TabIndex = 26;
         this.ewPulseGuideNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
         // 
         // eCheckBox
         // 
         this.eCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
         this.eCheckBox.AutoSize = true;
         this.eCheckBox.Location = new System.Drawing.Point(86, 86);
         this.eCheckBox.Name = "eCheckBox";
         this.eCheckBox.Size = new System.Drawing.Size(24, 23);
         this.eCheckBox.TabIndex = 25;
         this.eCheckBox.Text = "E";
         this.eCheckBox.UseVisualStyleBackColor = true;
         this.eCheckBox.CheckedChanged += new System.EventHandler(this.eCheckBox_CheckedChanged);
         // 
         // wCheckBox
         // 
         this.wCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
         this.wCheckBox.AutoSize = true;
         this.wCheckBox.Location = new System.Drawing.Point(52, 86);
         this.wCheckBox.Name = "wCheckBox";
         this.wCheckBox.Size = new System.Drawing.Size(28, 23);
         this.wCheckBox.TabIndex = 24;
         this.wCheckBox.Text = "W";
         this.wCheckBox.UseVisualStyleBackColor = true;
         this.wCheckBox.CheckedChanged += new System.EventHandler(this.wCheckBox_CheckedChanged);
         // 
         // slewingRadioButton
         // 
         this.slewingRadioButton.AutoSize = true;
         this.slewingRadioButton.Checked = true;
         this.slewingRadioButton.Location = new System.Drawing.Point(11, 47);
         this.slewingRadioButton.Name = "slewingRadioButton";
         this.slewingRadioButton.Size = new System.Drawing.Size(62, 17);
         this.slewingRadioButton.TabIndex = 23;
         this.slewingRadioButton.TabStop = true;
         this.slewingRadioButton.Text = "Slewing";
         this.slewingRadioButton.UseVisualStyleBackColor = true;
         // 
         // ewPulseGuidingRadioButton
         // 
         this.ewPulseGuidingRadioButton.AutoSize = true;
         this.ewPulseGuidingRadioButton.Location = new System.Drawing.Point(11, 24);
         this.ewPulseGuidingRadioButton.Name = "ewPulseGuidingRadioButton";
         this.ewPulseGuidingRadioButton.Size = new System.Drawing.Size(90, 17);
         this.ewPulseGuidingRadioButton.TabIndex = 23;
         this.ewPulseGuidingRadioButton.Text = "Pulse Guiding";
         this.ewPulseGuidingRadioButton.UseVisualStyleBackColor = true;
         this.ewPulseGuidingRadioButton.CheckedChanged += new System.EventHandler(this.ewPulseGuidingRadioButton_CheckedChanged);
         // 
         // secondaryAxisGroupBox
         // 
         this.secondaryAxisGroupBox.Controls.Add(this.nsPulseGuideNumericUpDown);
         this.secondaryAxisGroupBox.Controls.Add(this.sCheckBox);
         this.secondaryAxisGroupBox.Controls.Add(this.nCheckBox);
         this.secondaryAxisGroupBox.Controls.Add(this.radioButton1);
         this.secondaryAxisGroupBox.Controls.Add(this.nsPulseGuidingRadioButton);
         this.secondaryAxisGroupBox.Location = new System.Drawing.Point(213, 250);
         this.secondaryAxisGroupBox.Name = "secondaryAxisGroupBox";
         this.secondaryAxisGroupBox.Size = new System.Drawing.Size(164, 132);
         this.secondaryAxisGroupBox.TabIndex = 22;
         this.secondaryAxisGroupBox.TabStop = false;
         this.secondaryAxisGroupBox.Text = "Secondary Axis";
         // 
         // nsPulseGuideNumericUpDown
         // 
         this.nsPulseGuideNumericUpDown.Location = new System.Drawing.Point(105, 24);
         this.nsPulseGuideNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
         this.nsPulseGuideNumericUpDown.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
         this.nsPulseGuideNumericUpDown.Name = "nsPulseGuideNumericUpDown";
         this.nsPulseGuideNumericUpDown.Size = new System.Drawing.Size(52, 20);
         this.nsPulseGuideNumericUpDown.TabIndex = 34;
         this.nsPulseGuideNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
         // 
         // sCheckBox
         // 
         this.sCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
         this.sCheckBox.AutoSize = true;
         this.sCheckBox.Location = new System.Drawing.Point(68, 101);
         this.sCheckBox.Name = "sCheckBox";
         this.sCheckBox.Size = new System.Drawing.Size(24, 23);
         this.sCheckBox.TabIndex = 33;
         this.sCheckBox.Text = "S";
         this.sCheckBox.UseVisualStyleBackColor = true;
         this.sCheckBox.CheckedChanged += new System.EventHandler(this.sCheckBox_CheckedChanged);
         // 
         // nCheckBox
         // 
         this.nCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
         this.nCheckBox.AutoSize = true;
         this.nCheckBox.Location = new System.Drawing.Point(67, 73);
         this.nCheckBox.Name = "nCheckBox";
         this.nCheckBox.Size = new System.Drawing.Size(25, 23);
         this.nCheckBox.TabIndex = 32;
         this.nCheckBox.Text = "N";
         this.nCheckBox.UseVisualStyleBackColor = true;
         this.nCheckBox.CheckedChanged += new System.EventHandler(this.nCheckBox_CheckedChanged);
         // 
         // radioButton1
         // 
         this.radioButton1.AutoSize = true;
         this.radioButton1.Checked = true;
         this.radioButton1.Location = new System.Drawing.Point(13, 47);
         this.radioButton1.Name = "radioButton1";
         this.radioButton1.Size = new System.Drawing.Size(62, 17);
         this.radioButton1.TabIndex = 26;
         this.radioButton1.TabStop = true;
         this.radioButton1.Text = "Slewing";
         this.radioButton1.UseVisualStyleBackColor = true;
         // 
         // nsPulseGuidingRadioButton
         // 
         this.nsPulseGuidingRadioButton.AutoSize = true;
         this.nsPulseGuidingRadioButton.Location = new System.Drawing.Point(13, 24);
         this.nsPulseGuidingRadioButton.Name = "nsPulseGuidingRadioButton";
         this.nsPulseGuidingRadioButton.Size = new System.Drawing.Size(90, 17);
         this.nsPulseGuidingRadioButton.TabIndex = 25;
         this.nsPulseGuidingRadioButton.Text = "Pulse Guiding";
         this.nsPulseGuidingRadioButton.UseVisualStyleBackColor = true;
         this.nsPulseGuidingRadioButton.CheckedChanged += new System.EventHandler(this.nsPulseGuidingRadioButton_CheckedChanged);
         // 
         // bothAxesGroupBox
         // 
         this.bothAxesGroupBox.Controls.Add(this.slewToCoordButton);
         this.bothAxesGroupBox.Controls.Add(this.slewToAltAzButton);
         this.bothAxesGroupBox.Controls.Add(this.stopButton);
         this.bothAxesGroupBox.Controls.Add(this.label6);
         this.bothAxesGroupBox.Controls.Add(this.label2);
         this.bothAxesGroupBox.Controls.Add(this.decTextBox);
         this.bothAxesGroupBox.Controls.Add(this.raTextBox);
         this.bothAxesGroupBox.Controls.Add(this.altTextBox);
         this.bothAxesGroupBox.Controls.Add(this.azTextBox);
         this.bothAxesGroupBox.Location = new System.Drawing.Point(18, 392);
         this.bothAxesGroupBox.Name = "bothAxesGroupBox";
         this.bothAxesGroupBox.Size = new System.Drawing.Size(359, 99);
         this.bothAxesGroupBox.TabIndex = 26;
         this.bothAxesGroupBox.TabStop = false;
         this.bothAxesGroupBox.Text = "Both Axes";
         // 
         // slewToCoordButton
         // 
         this.slewToCoordButton.Location = new System.Drawing.Point(19, 63);
         this.slewToCoordButton.Name = "slewToCoordButton";
         this.slewToCoordButton.Size = new System.Drawing.Size(75, 23);
         this.slewToCoordButton.TabIndex = 32;
         this.slewToCoordButton.Text = "SlewToCoord";
         this.slewToCoordButton.UseVisualStyleBackColor = true;
         this.slewToCoordButton.Click += new System.EventHandler(this.slewToCoordButton_Click);
         // 
         // slewToAltAzButton
         // 
         this.slewToAltAzButton.Location = new System.Drawing.Point(19, 34);
         this.slewToAltAzButton.Name = "slewToAltAzButton";
         this.slewToAltAzButton.Size = new System.Drawing.Size(75, 23);
         this.slewToAltAzButton.TabIndex = 32;
         this.slewToAltAzButton.Text = "SlewToAltAz";
         this.slewToAltAzButton.UseVisualStyleBackColor = true;
         this.slewToAltAzButton.Click += new System.EventHandler(this.slewToAltAzButton_Click);
         // 
         // stopButton
         // 
         this.stopButton.BackColor = System.Drawing.Color.Red;
         this.stopButton.Location = new System.Drawing.Point(270, 31);
         this.stopButton.Name = "stopButton";
         this.stopButton.Size = new System.Drawing.Size(75, 58);
         this.stopButton.TabIndex = 32;
         this.stopButton.Text = "Stop!!";
         this.stopButton.UseVisualStyleBackColor = false;
         this.stopButton.Click += new System.EventHandler(this.stopAllSlews);
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.Location = new System.Drawing.Point(185, 18);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(44, 13);
         this.label6.TabIndex = 31;
         this.label6.Text = "Alt/Dec";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(108, 18);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(39, 13);
         this.label2.TabIndex = 30;
         this.label2.Text = "Az/RA";
         // 
         // decTextBox
         // 
         this.decTextBox.Location = new System.Drawing.Point(182, 65);
         this.decTextBox.MaxLength = 10;
         this.decTextBox.Name = "decTextBox";
         this.decTextBox.Size = new System.Drawing.Size(71, 20);
         this.decTextBox.TabIndex = 29;
         this.decTextBox.Text = "sDDD°MM:SS";
         // 
         // raTextBox
         // 
         this.raTextBox.Location = new System.Drawing.Point(105, 65);
         this.raTextBox.MaxLength = 10;
         this.raTextBox.Name = "raTextBox";
         this.raTextBox.Size = new System.Drawing.Size(71, 20);
         this.raTextBox.TabIndex = 28;
         this.raTextBox.Text = "HH:MM:SS.s";
         // 
         // altTextBox
         // 
         this.altTextBox.Location = new System.Drawing.Point(182, 36);
         this.altTextBox.MaxLength = 8;
         this.altTextBox.Name = "altTextBox";
         this.altTextBox.Size = new System.Drawing.Size(71, 20);
         this.altTextBox.TabIndex = 27;
         this.altTextBox.Text = "DDD.dddd";
         // 
         // azTextBox
         // 
         this.azTextBox.Location = new System.Drawing.Point(105, 36);
         this.azTextBox.MaxLength = 8;
         this.azTextBox.Name = "azTextBox";
         this.azTextBox.Size = new System.Drawing.Size(71, 20);
         this.azTextBox.TabIndex = 0;
         this.azTextBox.Text = "DDD.dddd";
         // 
         // trackingRateComboBox
         // 
         this.trackingRateComboBox.DisplayMember = "selectedTrackingRate";
         this.trackingRateComboBox.FormattingEnabled = true;
         this.trackingRateComboBox.Items.AddRange(new object[] {
            "Sidereal",
            "Lunar"});
         this.trackingRateComboBox.Location = new System.Drawing.Point(107, 220);
         this.trackingRateComboBox.Name = "trackingRateComboBox";
         this.trackingRateComboBox.Size = new System.Drawing.Size(67, 21);
         this.trackingRateComboBox.TabIndex = 27;
         this.trackingRateComboBox.Text = "Sidereal";
         this.trackingRateComboBox.ValueMember = "selectedTrackingRate";
         // 
         // trackingRateLabel
         // 
         this.trackingRateLabel.AutoSize = true;
         this.trackingRateLabel.Location = new System.Drawing.Point(26, 223);
         this.trackingRateLabel.Name = "trackingRateLabel";
         this.trackingRateLabel.Size = new System.Drawing.Size(75, 13);
         this.trackingRateLabel.TabIndex = 28;
         this.trackingRateLabel.Text = "Tracking Rate";
         // 
         // slewRateLabel
         // 
         this.slewRateLabel.AutoSize = true;
         this.slewRateLabel.Location = new System.Drawing.Point(184, 223);
         this.slewRateLabel.Name = "slewRateLabel";
         this.slewRateLabel.Size = new System.Drawing.Size(70, 13);
         this.slewRateLabel.TabIndex = 30;
         this.slewRateLabel.Text = "Slewing Rate";
         // 
         // slewRateComboBox
         // 
         this.slewRateComboBox.FormattingEnabled = true;
         this.slewRateComboBox.Items.AddRange(new object[] {
            "Sidereal",
            "Sidereal × 2",
            "Sidereal × 8",
            "Sidereal × 16",
            "Sidereal × 64",
            "0.5°/Sec",
            "1.5°/Sec",
            "3°/Sec",
            "6.5°/Sec"});
         this.slewRateComboBox.Location = new System.Drawing.Point(263, 220);
         this.slewRateComboBox.Name = "slewRateComboBox";
         this.slewRateComboBox.Size = new System.Drawing.Size(67, 21);
         this.slewRateComboBox.TabIndex = 29;
         this.slewRateComboBox.Text = "0.5°/Sec";
         // 
         // parkButton
         // 
         this.parkButton.Location = new System.Drawing.Point(309, 69);
         this.parkButton.Name = "parkButton";
         this.parkButton.Size = new System.Drawing.Size(72, 23);
         this.parkButton.TabIndex = 31;
         this.parkButton.Text = "Park";
         this.parkButton.UseVisualStyleBackColor = true;
         this.parkButton.Click += new System.EventHandler(this.onParkClick);
         // 
         // stressRAGuidingButton
         // 
         this.stressRAGuidingButton.Location = new System.Drawing.Point(39, 501);
         this.stressRAGuidingButton.Name = "stressRAGuidingButton";
         this.stressRAGuidingButton.Size = new System.Drawing.Size(155, 23);
         this.stressRAGuidingButton.TabIndex = 33;
         this.stressRAGuidingButton.Text = "Stress Pulse Guide :RA#";
         this.stressRAGuidingButton.UseVisualStyleBackColor = true;
         this.stressRAGuidingButton.Click += new System.EventHandler(this.stressRAGuidingButton_Click);
         // 
         // stressREGuidingButton
         // 
         this.stressREGuidingButton.Location = new System.Drawing.Point(38, 532);
         this.stressREGuidingButton.Name = "stressREGuidingButton";
         this.stressREGuidingButton.Size = new System.Drawing.Size(155, 23);
         this.stressREGuidingButton.TabIndex = 34;
         this.stressREGuidingButton.Text = "Stress Pulse Guide :RE#";
         this.stressREGuidingButton.UseVisualStyleBackColor = true;
         this.stressREGuidingButton.Click += new System.EventHandler(this.stressREGuidingButton_Click);
         // 
         // LX90TestForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(396, 569);
         this.Controls.Add(this.stressREGuidingButton);
         this.Controls.Add(this.stressRAGuidingButton);
         this.Controls.Add(this.parkButton);
         this.Controls.Add(this.slewRateLabel);
         this.Controls.Add(this.slewRateComboBox);
         this.Controls.Add(this.trackingRateLabel);
         this.Controls.Add(this.trackingRateComboBox);
         this.Controls.Add(this.secondaryAxisGroupBox);
         this.Controls.Add(this.primaryAxisGroupBox);
         this.Controls.Add(this.lstLabel);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.altLabel);
         this.Controls.Add(this.label15);
         this.Controls.Add(this.azLabel);
         this.Controls.Add(this.label13);
         this.Controls.Add(this.decLabel);
         this.Controls.Add(this.label11);
         this.Controls.Add(this.raLabel);
         this.Controls.Add(this.label9);
         this.Controls.Add(this.elevatonLabel);
         this.Controls.Add(this.label7);
         this.Controls.Add(this.lonLabel);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.latLabel);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.utcDateLabel);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.labelDriverId);
         this.Controls.Add(this.buttonConnect);
         this.Controls.Add(this.buttonChoose);
         this.Controls.Add(this.bothAxesGroupBox);
         this.Name = "LX90TestForm";
         this.Text = "Patched #497 Test";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
         this.primaryAxisGroupBox.ResumeLayout(false);
         this.primaryAxisGroupBox.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.ewPulseGuideNumericUpDown)).EndInit();
         this.secondaryAxisGroupBox.ResumeLayout(false);
         this.secondaryAxisGroupBox.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nsPulseGuideNumericUpDown)).EndInit();
         this.bothAxesGroupBox.ResumeLayout(false);
         this.bothAxesGroupBox.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button buttonChoose;
      private System.Windows.Forms.Button buttonConnect;
      private System.Windows.Forms.Label labelDriverId;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label utcDateLabel;
      private System.Windows.Forms.Label latLabel;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label lonLabel;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label elevatonLabel;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label raLabel;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label decLabel;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Label azLabel;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.Label altLabel;
      private System.Windows.Forms.Label label15;
      private System.Windows.Forms.Label lstLabel;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.GroupBox primaryAxisGroupBox;
      private System.Windows.Forms.RadioButton slewingRadioButton;
      private System.Windows.Forms.RadioButton ewPulseGuidingRadioButton;
      private System.Windows.Forms.GroupBox secondaryAxisGroupBox;
      private System.Windows.Forms.RadioButton radioButton1;
      private System.Windows.Forms.RadioButton nsPulseGuidingRadioButton;
      private System.Windows.Forms.GroupBox bothAxesGroupBox;
      private System.Windows.Forms.TextBox azTextBox;
      private System.Windows.Forms.TextBox altTextBox;
      private System.Windows.Forms.TextBox decTextBox;
      private System.Windows.Forms.TextBox raTextBox;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.ComboBox trackingRateComboBox;
      private System.Windows.Forms.Label trackingRateLabel;
      private System.Windows.Forms.Label slewRateLabel;
      private System.Windows.Forms.ComboBox slewRateComboBox;
      private System.Windows.Forms.Button stopButton;
      private System.Windows.Forms.Button parkButton;
      private System.Windows.Forms.Button slewToCoordButton;
      private System.Windows.Forms.Button slewToAltAzButton;
      private System.Windows.Forms.CheckBox wCheckBox;
      private System.Windows.Forms.CheckBox eCheckBox;
      private System.Windows.Forms.CheckBox sCheckBox;
      private System.Windows.Forms.CheckBox nCheckBox;
      private System.Windows.Forms.NumericUpDown ewPulseGuideNumericUpDown;
      private System.Windows.Forms.NumericUpDown nsPulseGuideNumericUpDown;
      private System.Windows.Forms.Button stressRAGuidingButton;
      private System.Windows.Forms.Button stressREGuidingButton;
   }
}

