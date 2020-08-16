using System;

namespace ArduinoAutoBrightness.DesktopApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chLaunchMin = new System.Windows.Forms.CheckBox();
            this.numMonitorBr = new System.Windows.Forms.NumericUpDown();
            this.bTuggleAutoAdjust = new System.Windows.Forms.Button();
            this.numSensor = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbComPorts = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chAutoReconnect = new System.Windows.Forms.CheckBox();
            this.bRefreshCom = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numMonitorBr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSensor)).BeginInit();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Auto Brightness";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Location = new System.Drawing.Point(12, 136);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(581, 234);
            this.tbLog.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(388, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sensor value:";
            // 
            // chLaunchMin
            // 
            this.chLaunchMin.AutoSize = true;
            this.chLaunchMin.Checked = true;
            this.chLaunchMin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chLaunchMin.Enabled = false;
            this.chLaunchMin.Location = new System.Drawing.Point(12, 34);
            this.chLaunchMin.Name = "chLaunchMin";
            this.chLaunchMin.Size = new System.Drawing.Size(124, 19);
            this.chLaunchMin.TabIndex = 3;
            this.chLaunchMin.Text = "Launch minimized";
            this.chLaunchMin.UseVisualStyleBackColor = true;
            // 
            // numMonitorBr
            // 
            this.numMonitorBr.Location = new System.Drawing.Point(505, 37);
            this.numMonitorBr.Name = "numMonitorBr";
            this.numMonitorBr.Size = new System.Drawing.Size(66, 23);
            this.numMonitorBr.TabIndex = 4;
            this.numMonitorBr.ValueChanged += new System.EventHandler(this.numMonitorBr_ValueChanged);
            // 
            // bTuggleAutoAdjust
            // 
            this.bTuggleAutoAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bTuggleAutoAdjust.Location = new System.Drawing.Point(12, 86);
            this.bTuggleAutoAdjust.Name = "bTuggleAutoAdjust";
            this.bTuggleAutoAdjust.Size = new System.Drawing.Size(581, 23);
            this.bTuggleAutoAdjust.TabIndex = 5;
            this.bTuggleAutoAdjust.Text = "Enable auto adjustment";
            this.bTuggleAutoAdjust.UseVisualStyleBackColor = true;
            this.bTuggleAutoAdjust.Click += new System.EventHandler(this.bTuggleAutoAdjust_Click);
            // 
            // numSensor
            // 
            this.numSensor.Enabled = false;
            this.numSensor.Location = new System.Drawing.Point(505, 8);
            this.numSensor.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numSensor.Name = "numSensor";
            this.numSensor.Size = new System.Drawing.Size(66, 23);
            this.numSensor.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(388, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Monitor Brigntness:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(571, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Settings:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Log:";
            // 
            // cbComPorts
            // 
            this.cbComPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComPorts.FormattingEnabled = true;
            this.cbComPorts.Location = new System.Drawing.Point(161, 34);
            this.cbComPorts.Name = "cbComPorts";
            this.cbComPorts.Size = new System.Drawing.Size(121, 23);
            this.cbComPorts.TabIndex = 6;
            this.cbComPorts.SelectedIndexChanged += new System.EventHandler(this.cbComPorts_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(158, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 15);
            this.label6.TabIndex = 1;
            this.label6.Text = "Arduino\'s COM port:";
            // 
            // chAutoReconnect
            // 
            this.chAutoReconnect.AutoSize = true;
            this.chAutoReconnect.Enabled = false;
            this.chAutoReconnect.Location = new System.Drawing.Point(12, 59);
            this.chAutoReconnect.Name = "chAutoReconnect";
            this.chAutoReconnect.Size = new System.Drawing.Size(108, 19);
            this.chAutoReconnect.TabIndex = 3;
            this.chAutoReconnect.Text = "Auto reconnect";
            this.chAutoReconnect.UseVisualStyleBackColor = true;
            // 
            // bRefreshCom
            // 
            this.bRefreshCom.Location = new System.Drawing.Point(288, 34);
            this.bRefreshCom.Name = "bRefreshCom";
            this.bRefreshCom.Size = new System.Drawing.Size(81, 23);
            this.bRefreshCom.TabIndex = 5;
            this.bRefreshCom.Text = "Refresh";
            this.bRefreshCom.UseVisualStyleBackColor = true;
            this.bRefreshCom.Click += new System.EventHandler(this.bRefreshCom_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(605, 381);
            this.Controls.Add(this.cbComPorts);
            this.Controls.Add(this.bTuggleAutoAdjust);
            this.Controls.Add(this.numMonitorBr);
            this.Controls.Add(this.chLaunchMin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.numSensor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.chAutoReconnect);
            this.Controls.Add(this.bRefreshCom);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Arduino Auto Brigntness";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.numMonitorBr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSensor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chLaunchMin;
        private System.Windows.Forms.NumericUpDown numMonitorBr;
        private System.Windows.Forms.Button bTuggleAutoAdjust;
        private System.Windows.Forms.NumericUpDown numSensor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbComPorts;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chAutoReconnect;
        private System.Windows.Forms.Button bRefreshCom;
        private System.Windows.Forms.NotifyIcon trayIcon;
    }
}

