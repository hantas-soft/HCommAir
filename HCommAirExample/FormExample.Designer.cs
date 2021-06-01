
namespace HCommAirExample
{
    partial class FormExample
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label lbLog;
            System.Windows.Forms.Label lbGOption;
            System.Windows.Forms.Label lbSampling;
            System.Windows.Forms.Label lbCh2;
            System.Windows.Forms.Label lbCh1;
            this.btStopMonitor = new System.Windows.Forms.Button();
            this.btStartMonitor = new System.Windows.Forms.Button();
            this.btSetParam = new System.Windows.Forms.Button();
            this.btGetParam = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nmCount = new System.Windows.Forms.NumericUpDown();
            this.nmAddr = new System.Windows.Forms.NumericUpDown();
            this.btPath = new System.Windows.Forms.Button();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btUnRegister = new System.Windows.Forms.Button();
            this.btRegister = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbRegisteredTools = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbScannedTools = new System.Windows.Forms.ListBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lbSerial = new System.Windows.Forms.Label();
            this.lbMac = new System.Windows.Forms.Label();
            this.lbIp = new System.Windows.Forms.Label();
            this.lbState = new System.Windows.Forms.Label();
            this.btClear = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gbGraph = new System.Windows.Forms.GroupBox();
            this.btGraphSet = new System.Windows.Forms.Button();
            this.btGraphStop = new System.Windows.Forms.Button();
            this.btGraphStart = new System.Windows.Forms.Button();
            this.cbOption = new System.Windows.Forms.ComboBox();
            this.cbSampling = new System.Windows.Forms.ComboBox();
            this.cbCh2 = new System.Windows.Forms.ComboBox();
            this.cbCh1 = new System.Windows.Forms.ComboBox();
            this.ssInfo = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btOpen = new System.Windows.Forms.Button();
            this.cbPorts = new System.Windows.Forms.ComboBox();
            this.cbInterface = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbIpAddr = new System.Windows.Forms.TextBox();
            lbLog = new System.Windows.Forms.Label();
            lbGOption = new System.Windows.Forms.Label();
            lbSampling = new System.Windows.Forms.Label();
            lbCh2 = new System.Windows.Forms.Label();
            lbCh1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nmCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmAddr)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbGraph.SuspendLayout();
            this.ssInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbLog
            // 
            lbLog.AutoSize = true;
            lbLog.Location = new System.Drawing.Point(569, 11);
            lbLog.Name = "lbLog";
            lbLog.Size = new System.Drawing.Size(26, 12);
            lbLog.TabIndex = 42;
            lbLog.Text = "Log";
            // 
            // lbGOption
            // 
            lbGOption.AutoSize = true;
            lbGOption.Location = new System.Drawing.Point(6, 80);
            lbGOption.Name = "lbGOption";
            lbGOption.Size = new System.Drawing.Size(41, 12);
            lbGOption.TabIndex = 13;
            lbGOption.Text = "Option";
            // 
            // lbSampling
            // 
            lbSampling.AutoSize = true;
            lbSampling.Location = new System.Drawing.Point(6, 106);
            lbSampling.Name = "lbSampling";
            lbSampling.Size = new System.Drawing.Size(58, 12);
            lbSampling.TabIndex = 11;
            lbSampling.Text = "Sampling";
            // 
            // lbCh2
            // 
            lbCh2.AutoSize = true;
            lbCh2.Location = new System.Drawing.Point(6, 54);
            lbCh2.Name = "lbCh2";
            lbCh2.Size = new System.Drawing.Size(62, 12);
            lbCh2.TabIndex = 8;
            lbCh2.Text = "Channel 2";
            // 
            // lbCh1
            // 
            lbCh1.AutoSize = true;
            lbCh1.Location = new System.Drawing.Point(6, 26);
            lbCh1.Name = "lbCh1";
            lbCh1.Size = new System.Drawing.Size(62, 12);
            lbCh1.TabIndex = 7;
            lbCh1.Text = "Channel 1";
            // 
            // btStopMonitor
            // 
            this.btStopMonitor.Location = new System.Drawing.Point(37, 73);
            this.btStopMonitor.Name = "btStopMonitor";
            this.btStopMonitor.Size = new System.Drawing.Size(110, 23);
            this.btStopMonitor.TabIndex = 36;
            this.btStopMonitor.Text = "Stop Real-Time";
            this.btStopMonitor.UseVisualStyleBackColor = true;
            this.btStopMonitor.Click += new System.EventHandler(this.btMonitorAction_Click);
            // 
            // btStartMonitor
            // 
            this.btStartMonitor.Location = new System.Drawing.Point(37, 41);
            this.btStartMonitor.Name = "btStartMonitor";
            this.btStartMonitor.Size = new System.Drawing.Size(110, 23);
            this.btStartMonitor.TabIndex = 35;
            this.btStartMonitor.Text = "Start Real-Time";
            this.btStartMonitor.UseVisualStyleBackColor = true;
            this.btStartMonitor.Click += new System.EventHandler(this.btMonitorAction_Click);
            // 
            // btSetParam
            // 
            this.btSetParam.Location = new System.Drawing.Point(144, 69);
            this.btSetParam.Name = "btSetParam";
            this.btSetParam.Size = new System.Drawing.Size(120, 23);
            this.btSetParam.TabIndex = 33;
            this.btSetParam.Text = "Set Param";
            this.btSetParam.UseVisualStyleBackColor = true;
            this.btSetParam.Click += new System.EventHandler(this.btParamAction_Click);
            // 
            // btGetParam
            // 
            this.btGetParam.Location = new System.Drawing.Point(18, 69);
            this.btGetParam.Name = "btGetParam";
            this.btGetParam.Size = new System.Drawing.Size(120, 23);
            this.btGetParam.TabIndex = 32;
            this.btGetParam.Text = "Get Param";
            this.btGetParam.UseVisualStyleBackColor = true;
            this.btGetParam.Click += new System.EventHandler(this.btParamAction_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(142, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 12);
            this.label4.TabIndex = 31;
            this.label4.Text = "Count / Value";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 12);
            this.label3.TabIndex = 30;
            this.label3.Text = "Address";
            // 
            // nmCount
            // 
            this.nmCount.Location = new System.Drawing.Point(144, 42);
            this.nmCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nmCount.Name = "nmCount";
            this.nmCount.Size = new System.Drawing.Size(120, 21);
            this.nmCount.TabIndex = 29;
            this.nmCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nmAddr
            // 
            this.nmAddr.Location = new System.Drawing.Point(18, 42);
            this.nmAddr.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nmAddr.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmAddr.Name = "nmAddr";
            this.nmAddr.Size = new System.Drawing.Size(120, 21);
            this.nmAddr.TabIndex = 28;
            this.nmAddr.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btPath
            // 
            this.btPath.Location = new System.Drawing.Point(133, 11);
            this.btPath.Name = "btPath";
            this.btPath.Size = new System.Drawing.Size(65, 23);
            this.btPath.TabIndex = 27;
            this.btPath.Text = "...";
            this.btPath.UseVisualStyleBackColor = true;
            this.btPath.Click += new System.EventHandler(this.btPath_Click);
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(12, 12);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(115, 21);
            this.tbPath.TabIndex = 26;
            // 
            // btUnRegister
            // 
            this.btUnRegister.Location = new System.Drawing.Point(255, 151);
            this.btUnRegister.Name = "btUnRegister";
            this.btUnRegister.Size = new System.Drawing.Size(65, 23);
            this.btUnRegister.TabIndex = 25;
            this.btUnRegister.Text = "<<<";
            this.btUnRegister.UseVisualStyleBackColor = true;
            this.btUnRegister.Click += new System.EventHandler(this.btRegister_Click);
            // 
            // btRegister
            // 
            this.btRegister.Location = new System.Drawing.Point(255, 122);
            this.btRegister.Name = "btRegister";
            this.btRegister.Size = new System.Drawing.Size(65, 23);
            this.btRegister.TabIndex = 24;
            this.btRegister.Text = ">>>";
            this.btRegister.UseVisualStyleBackColor = true;
            this.btRegister.Click += new System.EventHandler(this.btRegister_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(324, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 12);
            this.label2.TabIndex = 23;
            this.label2.Text = "Registered tools";
            // 
            // lbRegisteredTools
            // 
            this.lbRegisteredTools.DisplayMember = "Ip";
            this.lbRegisteredTools.FormattingEnabled = true;
            this.lbRegisteredTools.ItemHeight = 12;
            this.lbRegisteredTools.Location = new System.Drawing.Point(326, 84);
            this.lbRegisteredTools.Name = "lbRegisteredTools";
            this.lbRegisteredTools.Size = new System.Drawing.Size(237, 124);
            this.lbRegisteredTools.TabIndex = 22;
            this.lbRegisteredTools.ValueMember = "Mac";
            this.lbRegisteredTools.SelectedIndexChanged += new System.EventHandler(this.lbRegisteredTools_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 12);
            this.label1.TabIndex = 21;
            this.label1.Text = "Scanned tools";
            // 
            // lbScannedTools
            // 
            this.lbScannedTools.DisplayMember = "Ip";
            this.lbScannedTools.FormattingEnabled = true;
            this.lbScannedTools.ItemHeight = 12;
            this.lbScannedTools.Location = new System.Drawing.Point(12, 84);
            this.lbScannedTools.Name = "lbScannedTools";
            this.lbScannedTools.Size = new System.Drawing.Size(237, 124);
            this.lbScannedTools.TabIndex = 20;
            this.lbScannedTools.ValueMember = "Mac";
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lbSerial
            // 
            this.lbSerial.AutoSize = true;
            this.lbSerial.Location = new System.Drawing.Point(15, 66);
            this.lbSerial.Name = "lbSerial";
            this.lbSerial.Size = new System.Drawing.Size(32, 12);
            this.lbSerial.TabIndex = 39;
            this.lbSerial.Text = "S/N:";
            // 
            // lbMac
            // 
            this.lbMac.AutoSize = true;
            this.lbMac.Location = new System.Drawing.Point(15, 45);
            this.lbMac.Name = "lbMac";
            this.lbMac.Size = new System.Drawing.Size(37, 12);
            this.lbMac.TabIndex = 38;
            this.lbMac.Text = "MAC:";
            // 
            // lbIp
            // 
            this.lbIp.AutoSize = true;
            this.lbIp.Location = new System.Drawing.Point(15, 22);
            this.lbIp.Name = "lbIp";
            this.lbIp.Size = new System.Drawing.Size(20, 12);
            this.lbIp.TabIndex = 37;
            this.lbIp.Text = "IP:";
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(15, 88);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(49, 12);
            this.lbState.TabIndex = 40;
            this.lbState.Text = "STATE:";
            // 
            // btClear
            // 
            this.btClear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btClear.Location = new System.Drawing.Point(569, 448);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(392, 23);
            this.btClear.TabIndex = 44;
            this.btClear.Text = "Clear";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.BackColor = System.Drawing.Color.White;
            this.tbLog.Location = new System.Drawing.Point(569, 26);
            this.tbLog.MaxLength = 65535;
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(392, 416);
            this.tbLog.TabIndex = 41;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btStartMonitor);
            this.groupBox1.Controls.Add(this.btStopMonitor);
            this.groupBox1.Location = new System.Drawing.Point(383, 338);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 133);
            this.groupBox1.TabIndex = 45;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Event monitoring";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.nmAddr);
            this.groupBox2.Controls.Add(this.nmCount);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btGetParam);
            this.groupBox2.Controls.Add(this.btSetParam);
            this.groupBox2.Location = new System.Drawing.Point(287, 219);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 111);
            this.groupBox2.TabIndex = 46;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Get / Set parameter";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbState);
            this.groupBox3.Controls.Add(this.lbIp);
            this.groupBox3.Controls.Add(this.lbMac);
            this.groupBox3.Controls.Add(this.lbSerial);
            this.groupBox3.Location = new System.Drawing.Point(12, 219);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(269, 111);
            this.groupBox3.TabIndex = 47;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Selected tool information";
            // 
            // gbGraph
            // 
            this.gbGraph.Controls.Add(this.btGraphSet);
            this.gbGraph.Controls.Add(this.btGraphStop);
            this.gbGraph.Controls.Add(this.btGraphStart);
            this.gbGraph.Controls.Add(this.cbOption);
            this.gbGraph.Controls.Add(lbGOption);
            this.gbGraph.Controls.Add(this.cbSampling);
            this.gbGraph.Controls.Add(lbSampling);
            this.gbGraph.Controls.Add(this.cbCh2);
            this.gbGraph.Controls.Add(this.cbCh1);
            this.gbGraph.Controls.Add(lbCh2);
            this.gbGraph.Controls.Add(lbCh1);
            this.gbGraph.Location = new System.Drawing.Point(12, 336);
            this.gbGraph.Name = "gbGraph";
            this.gbGraph.Size = new System.Drawing.Size(346, 135);
            this.gbGraph.TabIndex = 48;
            this.gbGraph.TabStop = false;
            this.gbGraph.Text = "Graph monitoring";
            // 
            // btGraphSet
            // 
            this.btGraphSet.Location = new System.Drawing.Point(213, 23);
            this.btGraphSet.Name = "btGraphSet";
            this.btGraphSet.Size = new System.Drawing.Size(115, 23);
            this.btGraphSet.TabIndex = 18;
            this.btGraphSet.Text = "Set graph setting";
            this.btGraphSet.UseVisualStyleBackColor = true;
            this.btGraphSet.Click += new System.EventHandler(this.btGraphAction_Click);
            // 
            // btGraphStop
            // 
            this.btGraphStop.Location = new System.Drawing.Point(213, 104);
            this.btGraphStop.Name = "btGraphStop";
            this.btGraphStop.Size = new System.Drawing.Size(115, 23);
            this.btGraphStop.TabIndex = 17;
            this.btGraphStop.Text = "Stop";
            this.btGraphStop.UseVisualStyleBackColor = true;
            this.btGraphStop.Click += new System.EventHandler(this.btGraphAction_Click);
            // 
            // btGraphStart
            // 
            this.btGraphStart.Location = new System.Drawing.Point(213, 75);
            this.btGraphStart.Name = "btGraphStart";
            this.btGraphStart.Size = new System.Drawing.Size(115, 23);
            this.btGraphStart.TabIndex = 15;
            this.btGraphStart.Text = "Start";
            this.btGraphStart.UseVisualStyleBackColor = true;
            this.btGraphStart.Click += new System.EventHandler(this.btGraphAction_Click);
            // 
            // cbOption
            // 
            this.cbOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOption.FormattingEnabled = true;
            this.cbOption.Items.AddRange(new object[] {
            "Fastening",
            "Lossening",
            "Both"});
            this.cbOption.Location = new System.Drawing.Point(95, 77);
            this.cbOption.Name = "cbOption";
            this.cbOption.Size = new System.Drawing.Size(93, 20);
            this.cbOption.TabIndex = 14;
            // 
            // cbSampling
            // 
            this.cbSampling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSampling.FormattingEnabled = true;
            this.cbSampling.Items.AddRange(new object[] {
            "5 ms",
            "10 ms",
            "15 ms",
            "30 ms"});
            this.cbSampling.Location = new System.Drawing.Point(95, 104);
            this.cbSampling.Name = "cbSampling";
            this.cbSampling.Size = new System.Drawing.Size(93, 20);
            this.cbSampling.TabIndex = 12;
            // 
            // cbCh2
            // 
            this.cbCh2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCh2.FormattingEnabled = true;
            this.cbCh2.Items.AddRange(new object[] {
            "None",
            "Torque",
            "Current",
            "Speed",
            "Angle",
            "Speed command",
            "Current command",
            "Snug angle",
            "Torque/Angle"});
            this.cbCh2.Location = new System.Drawing.Point(95, 51);
            this.cbCh2.Name = "cbCh2";
            this.cbCh2.Size = new System.Drawing.Size(93, 20);
            this.cbCh2.TabIndex = 10;
            // 
            // cbCh1
            // 
            this.cbCh1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCh1.FormattingEnabled = true;
            this.cbCh1.Items.AddRange(new object[] {
            "None",
            "Torque",
            "Current",
            "Speed",
            "Angle",
            "Speed command",
            "Current command",
            "Snug angle",
            "Torque/Angle"});
            this.cbCh1.Location = new System.Drawing.Point(95, 23);
            this.cbCh1.Name = "cbCh1";
            this.cbCh1.Size = new System.Drawing.Size(93, 20);
            this.cbCh1.TabIndex = 9;
            // 
            // ssInfo
            // 
            this.ssInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.ssInfo.Location = new System.Drawing.Point(0, 479);
            this.ssInfo.Name = "ssInfo";
            this.ssInfo.Size = new System.Drawing.Size(968, 22);
            this.ssInfo.TabIndex = 49;
            this.ssInfo.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusLabel1.Text = "v1.00.0";
            // 
            // btOpen
            // 
            this.btOpen.Location = new System.Drawing.Point(488, 12);
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(75, 23);
            this.btOpen.TabIndex = 50;
            this.btOpen.Text = "Open";
            this.btOpen.UseVisualStyleBackColor = true;
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // cbPorts
            // 
            this.cbPorts.FormattingEnabled = true;
            this.cbPorts.Location = new System.Drawing.Point(361, 12);
            this.cbPorts.Name = "cbPorts";
            this.cbPorts.Size = new System.Drawing.Size(121, 20);
            this.cbPorts.TabIndex = 51;
            // 
            // cbInterface
            // 
            this.cbInterface.FormattingEnabled = true;
            this.cbInterface.Location = new System.Drawing.Point(127, 41);
            this.cbInterface.Name = "cbInterface";
            this.cbInterface.Size = new System.Drawing.Size(436, 20);
            this.cbInterface.TabIndex = 52;
            this.cbInterface.SelectedIndexChanged += new System.EventHandler(this.cbInterface_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 53;
            this.label5.Text = "Interface";
            // 
            // tbIpAddr
            // 
            this.tbIpAddr.Location = new System.Drawing.Point(240, 11);
            this.tbIpAddr.Name = "tbIpAddr";
            this.tbIpAddr.Size = new System.Drawing.Size(115, 21);
            this.tbIpAddr.TabIndex = 54;
            this.tbIpAddr.Text = "192.168.1.100";
            // 
            // FormExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 501);
            this.Controls.Add(this.tbIpAddr);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbInterface);
            this.Controls.Add(this.cbPorts);
            this.Controls.Add(this.btOpen);
            this.Controls.Add(this.ssInfo);
            this.Controls.Add(this.gbGraph);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btClear);
            this.Controls.Add(lbLog);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btPath);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.btUnRegister);
            this.Controls.Add(this.btRegister);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbRegisteredTools);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbScannedTools);
            this.Name = "FormExample";
            this.Text = "Hantas air tools communication example";
            this.Load += new System.EventHandler(this.FormExample_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nmCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmAddr)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbGraph.ResumeLayout(false);
            this.gbGraph.PerformLayout();
            this.ssInfo.ResumeLayout(false);
            this.ssInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btStopMonitor;
        private System.Windows.Forms.Button btStartMonitor;
        private System.Windows.Forms.Button btSetParam;
        private System.Windows.Forms.Button btGetParam;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nmCount;
        private System.Windows.Forms.NumericUpDown nmAddr;
        private System.Windows.Forms.Button btPath;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button btUnRegister;
        private System.Windows.Forms.Button btRegister;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbRegisteredTools;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbScannedTools;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lbSerial;
        private System.Windows.Forms.Label lbMac;
        private System.Windows.Forms.Label lbIp;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox gbGraph;
        private System.Windows.Forms.Button btGraphSet;
        private System.Windows.Forms.Button btGraphStop;
        private System.Windows.Forms.Button btGraphStart;
        private System.Windows.Forms.ComboBox cbOption;
        private System.Windows.Forms.ComboBox cbSampling;
        private System.Windows.Forms.ComboBox cbCh2;
        private System.Windows.Forms.ComboBox cbCh1;
        private System.Windows.Forms.StatusStrip ssInfo;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btOpen;
        private System.Windows.Forms.ComboBox cbPorts;
        private System.Windows.Forms.ComboBox cbInterface;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbIpAddr;
    }
}

