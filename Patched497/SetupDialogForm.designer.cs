namespace ASCOM.LX90
{
   partial class SetupDialogForm
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
         this.cmdOK = new System.Windows.Forms.Button();
         this.cmdCancel = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.picASCOM = new System.Windows.Forms.PictureBox();
         this.label2 = new System.Windows.Forms.Label();
         this.chkTrace = new System.Windows.Forms.CheckBox();
         this.comboBoxComPort = new System.Windows.Forms.ComboBox();
         this.applySlewCoefEast = new System.Windows.Forms.RadioButton();
         this.applySlewCoefWest = new System.Windows.Forms.RadioButton();
         this.slewCoefGroupBox = new System.Windows.Forms.GroupBox();
         this.label3 = new System.Windows.Forms.Label();
         this.guideSlewCoefUpDown = new System.Windows.Forms.NumericUpDown();
         this.guideRateLbl = new System.Windows.Forms.Label();
         this.guideRateUpDown = new System.Windows.Forms.NumericUpDown();
         this.siderealLabel = new System.Windows.Forms.Label();
         this.focalLenghtUpDown = new System.Windows.Forms.NumericUpDown();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.label6 = new System.Windows.Forms.Label();
         this.apertureUpDown = new System.Windows.Forms.NumericUpDown();
         this.label7 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.secondaryDiaUpDown = new System.Windows.Forms.NumericUpDown();
         this.label9 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.elevationUpDown = new System.Windows.Forms.NumericUpDown();
         this.label11 = new System.Windows.Forms.Label();
         this.customRates = new System.Windows.Forms.CheckBox();
         this.customRateDirectionGroupBox = new System.Windows.Forms.GroupBox();
         this.negREMovesNcheckBox = new System.Windows.Forms.CheckBox();
         this.negRAMovesEcheckBox = new System.Windows.Forms.CheckBox();
         this.pulseGuideAlgGroupBox = new System.Windows.Forms.GroupBox();
         this.moveGuideRadioButton = new System.Windows.Forms.RadioButton();
         this.raReGuideRadioButton = new System.Windows.Forms.RadioButton();
         this.pulseGuideRadioButton = new System.Windows.Forms.RadioButton();
         this.verboseCheckBox = new System.Windows.Forms.CheckBox();
         ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
         this.slewCoefGroupBox.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.guideSlewCoefUpDown)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.guideRateUpDown)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.focalLenghtUpDown)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.apertureUpDown)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.secondaryDiaUpDown)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.elevationUpDown)).BeginInit();
         this.customRateDirectionGroupBox.SuspendLayout();
         this.pulseGuideAlgGroupBox.SuspendLayout();
         this.SuspendLayout();
         // 
         // cmdOK
         // 
         this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.cmdOK.Location = new System.Drawing.Point(281, 458);
         this.cmdOK.Name = "cmdOK";
         this.cmdOK.Size = new System.Drawing.Size(59, 24);
         this.cmdOK.TabIndex = 0;
         this.cmdOK.Text = "OK";
         this.cmdOK.UseVisualStyleBackColor = true;
         this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
         // 
         // cmdCancel
         // 
         this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.cmdCancel.Location = new System.Drawing.Point(281, 488);
         this.cmdCancel.Name = "cmdCancel";
         this.cmdCancel.Size = new System.Drawing.Size(59, 25);
         this.cmdCancel.TabIndex = 1;
         this.cmdCancel.Text = "Cancel";
         this.cmdCancel.UseVisualStyleBackColor = true;
         this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(12, 9);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(274, 27);
         this.label1.TabIndex = 2;
         this.label1.Text = "Driver for Patched Meade #497 (Etx/LX90)";
         // 
         // picASCOM
         // 
         this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
         this.picASCOM.Image = global::ASCOM.LX90.Properties.Resources.ASCOM;
         this.picASCOM.Location = new System.Drawing.Point(292, 7);
         this.picASCOM.Name = "picASCOM";
         this.picASCOM.Size = new System.Drawing.Size(48, 56);
         this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
         this.picASCOM.TabIndex = 3;
         this.picASCOM.TabStop = false;
         this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
         this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(13, 36);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(58, 13);
         this.label2.TabIndex = 5;
         this.label2.Text = "Comm Port";
         // 
         // chkTrace
         // 
         this.chkTrace.AutoSize = true;
         this.chkTrace.Location = new System.Drawing.Point(77, 64);
         this.chkTrace.Name = "chkTrace";
         this.chkTrace.Size = new System.Drawing.Size(69, 17);
         this.chkTrace.TabIndex = 8;
         this.chkTrace.Text = "Trace on";
         this.chkTrace.UseVisualStyleBackColor = true;
         this.chkTrace.CheckedChanged += new System.EventHandler(this.chkTrace_CheckedChanged);
         // 
         // comboBoxComPort
         // 
         this.comboBoxComPort.FormattingEnabled = true;
         this.comboBoxComPort.Location = new System.Drawing.Point(77, 33);
         this.comboBoxComPort.Name = "comboBoxComPort";
         this.comboBoxComPort.Size = new System.Drawing.Size(90, 21);
         this.comboBoxComPort.TabIndex = 7;
         // 
         // applySlewCoefEast
         // 
         this.applySlewCoefEast.AutoSize = true;
         this.applySlewCoefEast.Checked = true;
         this.applySlewCoefEast.Location = new System.Drawing.Point(87, 143);
         this.applySlewCoefEast.Name = "applySlewCoefEast";
         this.applySlewCoefEast.Size = new System.Drawing.Size(46, 17);
         this.applySlewCoefEast.TabIndex = 10;
         this.applySlewCoefEast.TabStop = true;
         this.applySlewCoefEast.Text = "East";
         this.applySlewCoefEast.UseVisualStyleBackColor = true;
         // 
         // applySlewCoefWest
         // 
         this.applySlewCoefWest.AutoSize = true;
         this.applySlewCoefWest.Location = new System.Drawing.Point(87, 162);
         this.applySlewCoefWest.Name = "applySlewCoefWest";
         this.applySlewCoefWest.Size = new System.Drawing.Size(50, 17);
         this.applySlewCoefWest.TabIndex = 11;
         this.applySlewCoefWest.Text = "West";
         this.applySlewCoefWest.UseVisualStyleBackColor = true;
         // 
         // slewCoefGroupBox
         // 
         this.slewCoefGroupBox.Controls.Add(this.label3);
         this.slewCoefGroupBox.Controls.Add(this.guideSlewCoefUpDown);
         this.slewCoefGroupBox.Location = new System.Drawing.Point(77, 123);
         this.slewCoefGroupBox.Name = "slewCoefGroupBox";
         this.slewCoefGroupBox.Size = new System.Drawing.Size(261, 64);
         this.slewCoefGroupBox.TabIndex = 10;
         this.slewCoefGroupBox.TabStop = false;
         this.slewCoefGroupBox.Text = "Asymmetric guiding correction coeficient";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(128, 22);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(70, 13);
         this.label3.TabIndex = 13;
         this.label3.Text = "× Guide Rate";
         // 
         // guideSlewCoefUpDown
         // 
         this.guideSlewCoefUpDown.DecimalPlaces = 2;
         this.guideSlewCoefUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
         this.guideSlewCoefUpDown.Location = new System.Drawing.Point(69, 20);
         this.guideSlewCoefUpDown.Maximum = new decimal(new int[] {
            199,
            0,
            0,
            131072});
         this.guideSlewCoefUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
         this.guideSlewCoefUpDown.Name = "guideSlewCoefUpDown";
         this.guideSlewCoefUpDown.Size = new System.Drawing.Size(56, 20);
         this.guideSlewCoefUpDown.TabIndex = 12;
         this.guideSlewCoefUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
         // 
         // guideRateLbl
         // 
         this.guideRateLbl.AutoSize = true;
         this.guideRateLbl.Location = new System.Drawing.Point(10, 95);
         this.guideRateLbl.Name = "guideRateLbl";
         this.guideRateLbl.Size = new System.Drawing.Size(61, 13);
         this.guideRateLbl.TabIndex = 11;
         this.guideRateLbl.Text = "Guide Rate";
         // 
         // guideRateUpDown
         // 
         this.guideRateUpDown.DecimalPlaces = 2;
         this.guideRateUpDown.Increment = new decimal(new int[] {
            33,
            0,
            0,
            131072});
         this.guideRateUpDown.Location = new System.Drawing.Point(77, 93);
         this.guideRateUpDown.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            131072});
         this.guideRateUpDown.Minimum = new decimal(new int[] {
            25,
            0,
            0,
            131072});
         this.guideRateUpDown.Name = "guideRateUpDown";
         this.guideRateUpDown.Size = new System.Drawing.Size(56, 20);
         this.guideRateUpDown.TabIndex = 9;
         this.guideRateUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
         // 
         // siderealLabel
         // 
         this.siderealLabel.AutoSize = true;
         this.siderealLabel.Location = new System.Drawing.Point(136, 95);
         this.siderealLabel.Name = "siderealLabel";
         this.siderealLabel.Size = new System.Drawing.Size(80, 13);
         this.siderealLabel.TabIndex = 12;
         this.siderealLabel.Text = "× Sidereal Rate";
         // 
         // focalLenghtUpDown
         // 
         this.focalLenghtUpDown.DecimalPlaces = 1;
         this.focalLenghtUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
         this.focalLenghtUpDown.Location = new System.Drawing.Point(77, 396);
         this.focalLenghtUpDown.Maximum = new decimal(new int[] {
            12000,
            0,
            0,
            0});
         this.focalLenghtUpDown.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            0});
         this.focalLenghtUpDown.Name = "focalLenghtUpDown";
         this.focalLenghtUpDown.Size = new System.Drawing.Size(90, 20);
         this.focalLenghtUpDown.TabIndex = 13;
         this.focalLenghtUpDown.Value = new decimal(new int[] {
            3048,
            0,
            0,
            0});
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(2, 398);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(69, 13);
         this.label4.TabIndex = 14;
         this.label4.Text = "Focal Length";
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(173, 398);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(23, 13);
         this.label5.TabIndex = 15;
         this.label5.Text = "mm";
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.Location = new System.Drawing.Point(173, 433);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(23, 13);
         this.label6.TabIndex = 18;
         this.label6.Text = "mm";
         // 
         // apertureUpDown
         // 
         this.apertureUpDown.DecimalPlaces = 1;
         this.apertureUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
         this.apertureUpDown.Location = new System.Drawing.Point(77, 431);
         this.apertureUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
         this.apertureUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
         this.apertureUpDown.Name = "apertureUpDown";
         this.apertureUpDown.Size = new System.Drawing.Size(90, 20);
         this.apertureUpDown.TabIndex = 16;
         this.apertureUpDown.Value = new decimal(new int[] {
            3048,
            0,
            0,
            65536});
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Location = new System.Drawing.Point(24, 433);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(47, 13);
         this.label7.TabIndex = 17;
         this.label7.Text = "Aperture";
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Location = new System.Drawing.Point(173, 468);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(23, 13);
         this.label8.TabIndex = 21;
         this.label8.Text = "mm";
         // 
         // secondaryDiaUpDown
         // 
         this.secondaryDiaUpDown.DecimalPlaces = 1;
         this.secondaryDiaUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
         this.secondaryDiaUpDown.Location = new System.Drawing.Point(77, 466);
         this.secondaryDiaUpDown.Maximum = new decimal(new int[] {
            800,
            0,
            0,
            0});
         this.secondaryDiaUpDown.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
         this.secondaryDiaUpDown.Name = "secondaryDiaUpDown";
         this.secondaryDiaUpDown.Size = new System.Drawing.Size(90, 20);
         this.secondaryDiaUpDown.TabIndex = 19;
         this.secondaryDiaUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.Location = new System.Drawing.Point(2, 468);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(69, 13);
         this.label9.TabIndex = 20;
         this.label9.Text = "Secondary Ø";
         // 
         // label10
         // 
         this.label10.AutoSize = true;
         this.label10.Location = new System.Drawing.Point(173, 504);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(92, 13);
         this.label10.TabIndex = 24;
         this.label10.Text = "meters > sea level";
         // 
         // elevationUpDown
         // 
         this.elevationUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.elevationUpDown.Location = new System.Drawing.Point(77, 502);
         this.elevationUpDown.Maximum = new decimal(new int[] {
            12000,
            0,
            0,
            0});
         this.elevationUpDown.Name = "elevationUpDown";
         this.elevationUpDown.Size = new System.Drawing.Size(90, 20);
         this.elevationUpDown.TabIndex = 22;
         this.elevationUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
         // 
         // label11
         // 
         this.label11.AutoSize = true;
         this.label11.Location = new System.Drawing.Point(2, 504);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(70, 13);
         this.label11.TabIndex = 23;
         this.label11.Text = "Site Elevaton";
         // 
         // customRates
         // 
         this.customRates.AutoSize = true;
         this.customRates.Location = new System.Drawing.Point(77, 195);
         this.customRates.Name = "customRates";
         this.customRates.Size = new System.Drawing.Size(174, 17);
         this.customRates.TabIndex = 25;
         this.customRates.Text = "Patched ROM incl. :RA#/:RE#";
         this.customRates.UseVisualStyleBackColor = true;
         this.customRates.CheckedChanged += new System.EventHandler(this.customRates_CheckedChanged);
         // 
         // customRateDirectionGroupBox
         // 
         this.customRateDirectionGroupBox.Controls.Add(this.negREMovesNcheckBox);
         this.customRateDirectionGroupBox.Controls.Add(this.negRAMovesEcheckBox);
         this.customRateDirectionGroupBox.Location = new System.Drawing.Point(78, 220);
         this.customRateDirectionGroupBox.Name = "customRateDirectionGroupBox";
         this.customRateDirectionGroupBox.Size = new System.Drawing.Size(261, 64);
         this.customRateDirectionGroupBox.TabIndex = 26;
         this.customRateDirectionGroupBox.TabStop = false;
         this.customRateDirectionGroupBox.Text = ":RA#/:RE# direction control";
         // 
         // negREMovesNcheckBox
         // 
         this.negREMovesNcheckBox.AutoSize = true;
         this.negREMovesNcheckBox.Location = new System.Drawing.Point(10, 41);
         this.negREMovesNcheckBox.Name = "negREMovesNcheckBox";
         this.negREMovesNcheckBox.Size = new System.Drawing.Size(114, 17);
         this.negREMovesNcheckBox.TabIndex = 29;
         this.negREMovesNcheckBox.Text = "-ve :RE# moves N";
         this.negREMovesNcheckBox.UseVisualStyleBackColor = true;
         // 
         // negRAMovesEcheckBox
         // 
         this.negRAMovesEcheckBox.AutoSize = true;
         this.negRAMovesEcheckBox.Location = new System.Drawing.Point(10, 18);
         this.negRAMovesEcheckBox.Name = "negRAMovesEcheckBox";
         this.negRAMovesEcheckBox.Size = new System.Drawing.Size(113, 17);
         this.negRAMovesEcheckBox.TabIndex = 0;
         this.negRAMovesEcheckBox.Text = "-ve :RA# moves E";
         this.negRAMovesEcheckBox.UseVisualStyleBackColor = true;
         // 
         // pulseGuideAlgGroupBox
         // 
         this.pulseGuideAlgGroupBox.Controls.Add(this.moveGuideRadioButton);
         this.pulseGuideAlgGroupBox.Controls.Add(this.raReGuideRadioButton);
         this.pulseGuideAlgGroupBox.Controls.Add(this.pulseGuideRadioButton);
         this.pulseGuideAlgGroupBox.Location = new System.Drawing.Point(40, 295);
         this.pulseGuideAlgGroupBox.Name = "pulseGuideAlgGroupBox";
         this.pulseGuideAlgGroupBox.Size = new System.Drawing.Size(298, 92);
         this.pulseGuideAlgGroupBox.TabIndex = 28;
         this.pulseGuideAlgGroupBox.TabStop = false;
         this.pulseGuideAlgGroupBox.Text = "Pulse Guide Algorithm";
         // 
         // moveGuideRadioButton
         // 
         this.moveGuideRadioButton.AutoSize = true;
         this.moveGuideRadioButton.Location = new System.Drawing.Point(11, 63);
         this.moveGuideRadioButton.Name = "moveGuideRadioButton";
         this.moveGuideRadioButton.Size = new System.Drawing.Size(268, 17);
         this.moveGuideRadioButton.TabIndex = 2;
         this.moveGuideRadioButton.TabStop = true;
         this.moveGuideRadioButton.Text = "Move commands at sidereal (guide rate scales time)";
         this.moveGuideRadioButton.UseVisualStyleBackColor = true;
         // 
         // raReGuideRadioButton
         // 
         this.raReGuideRadioButton.AutoSize = true;
         this.raReGuideRadioButton.Location = new System.Drawing.Point(11, 40);
         this.raReGuideRadioButton.Name = "raReGuideRadioButton";
         this.raReGuideRadioButton.Size = new System.Drawing.Size(210, 17);
         this.raReGuideRadioButton.TabIndex = 1;
         this.raReGuideRadioButton.TabStop = true;
         this.raReGuideRadioButton.Text = ":RA#/:RE# at guide rate (experimental)";
         this.raReGuideRadioButton.UseVisualStyleBackColor = true;
         // 
         // pulseGuideRadioButton
         // 
         this.pulseGuideRadioButton.AutoSize = true;
         this.pulseGuideRadioButton.Location = new System.Drawing.Point(11, 17);
         this.pulseGuideRadioButton.Name = "pulseGuideRadioButton";
         this.pulseGuideRadioButton.Size = new System.Drawing.Size(170, 17);
         this.pulseGuideRadioButton.TabIndex = 0;
         this.pulseGuideRadioButton.TabStop = true;
         this.pulseGuideRadioButton.Text = "Pulse Guide commands (:Mg#)";
         this.pulseGuideRadioButton.UseVisualStyleBackColor = true;
         // 
         // verboseCheckBox
         // 
         this.verboseCheckBox.AutoSize = true;
         this.verboseCheckBox.Location = new System.Drawing.Point(152, 64);
         this.verboseCheckBox.Name = "verboseCheckBox";
         this.verboseCheckBox.Size = new System.Drawing.Size(96, 17);
         this.verboseCheckBox.TabIndex = 29;
         this.verboseCheckBox.Text = "Trace Verbose";
         this.verboseCheckBox.UseVisualStyleBackColor = true;
         // 
         // SetupDialogForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(350, 534);
         this.Controls.Add(this.verboseCheckBox);
         this.Controls.Add(this.pulseGuideAlgGroupBox);
         this.Controls.Add(this.customRateDirectionGroupBox);
         this.Controls.Add(this.customRates);
         this.Controls.Add(this.label10);
         this.Controls.Add(this.elevationUpDown);
         this.Controls.Add(this.label11);
         this.Controls.Add(this.label8);
         this.Controls.Add(this.secondaryDiaUpDown);
         this.Controls.Add(this.label9);
         this.Controls.Add(this.label6);
         this.Controls.Add(this.apertureUpDown);
         this.Controls.Add(this.label7);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.focalLenghtUpDown);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.siderealLabel);
         this.Controls.Add(this.guideRateUpDown);
         this.Controls.Add(this.guideRateLbl);
         this.Controls.Add(this.applySlewCoefWest);
         this.Controls.Add(this.applySlewCoefEast);
         this.Controls.Add(this.comboBoxComPort);
         this.Controls.Add(this.chkTrace);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.picASCOM);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.cmdCancel);
         this.Controls.Add(this.cmdOK);
         this.Controls.Add(this.slewCoefGroupBox);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "SetupDialogForm";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "LX90 Setup";
         ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
         this.slewCoefGroupBox.ResumeLayout(false);
         this.slewCoefGroupBox.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.guideSlewCoefUpDown)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.guideRateUpDown)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.focalLenghtUpDown)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.apertureUpDown)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.secondaryDiaUpDown)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.elevationUpDown)).EndInit();
         this.customRateDirectionGroupBox.ResumeLayout(false);
         this.customRateDirectionGroupBox.PerformLayout();
         this.pulseGuideAlgGroupBox.ResumeLayout(false);
         this.pulseGuideAlgGroupBox.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button cmdOK;
      private System.Windows.Forms.Button cmdCancel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.PictureBox picASCOM;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.CheckBox chkTrace;
      private System.Windows.Forms.ComboBox comboBoxComPort;
      private System.Windows.Forms.RadioButton applySlewCoefEast;
      private System.Windows.Forms.RadioButton applySlewCoefWest;
      private System.Windows.Forms.GroupBox slewCoefGroupBox;
      private System.Windows.Forms.NumericUpDown guideSlewCoefUpDown;
      private System.Windows.Forms.Label guideRateLbl;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label siderealLabel;
      private System.Windows.Forms.NumericUpDown focalLenghtUpDown;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.NumericUpDown apertureUpDown;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.NumericUpDown secondaryDiaUpDown;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.NumericUpDown elevationUpDown;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.CheckBox customRates;
      private System.Windows.Forms.GroupBox customRateDirectionGroupBox;
      private System.Windows.Forms.CheckBox negRAMovesEcheckBox;
      private System.Windows.Forms.CheckBox negREMovesNcheckBox;
      private System.Windows.Forms.GroupBox pulseGuideAlgGroupBox;
      private System.Windows.Forms.RadioButton moveGuideRadioButton;
      private System.Windows.Forms.RadioButton raReGuideRadioButton;
      private System.Windows.Forms.RadioButton pulseGuideRadioButton;
      private System.Windows.Forms.NumericUpDown guideRateUpDown;
      private System.Windows.Forms.CheckBox verboseCheckBox;
   }
}