﻿using ArduinoAutoBrightness.Shared;
using LattePanda.Firmata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArduinoAutoBrightness.DesktopApp.Extentions;

namespace ArduinoAutoBrightness.DesktopApp
{
    public partial class FormMain : Form
    {
        private Arduino arduino = null;
        private bool _autoAdjustBrightness = false;
        private bool autoAdjustBrightness 
        { 
            get => _autoAdjustBrightness; 
            set 
            {
                _autoAdjustBrightness = value;
                if (autoAdjustBrightness)
                {
                    bTuggleAutoAdjust.Text = "Disable auto adjustment";
                }
                else
                {
                    bTuggleAutoAdjust.Text = "Enable auto adjustment";
                }
            }
        }
        private bool forceMinimizeToTray = false;

        public FormMain()
        {
            InitializeComponent();
            trayIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            autoAdjustBrightness = true;
            UpdatePorts();
            UpdateMonitorBrightness(MonitorBrightness.Get());

            if (chLaunchMin.Checked)
            {
                forceMinimizeToTray = true;
                WindowState = FormWindowState.Minimized;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            arduino?.Dispose();
        }

        private void UpdatePorts()
        {
            cbComPorts.Items.Clear();
            foreach (var portName in Arduino.GetAvailablePorts())
            {
                cbComPorts.Items.Add(portName);
            }
            if (cbComPorts.Items.Count > 0)
            {
                cbComPorts.SelectedIndex = 0;
            }
            else
            {
                Log("Arduino not found.");
            }
        }

        private void cbComPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbComPorts.SelectedIndex == -1)
            {
                arduino?.Dispose();
                arduino = null;
                return;
            }

            string port = (string)cbComPorts.SelectedItem;
            Log($"Connecting to {port}...", false);
            try
            {
                arduino = new Arduino(port);
            }
            catch (Exception ex)
            {
                Log($"FAIL: {ex.Message}", addTimestamp: false);
            }
            Log("DONE", addTimestamp: false);

            arduino.AnalogPinChanged += Arduino_AnalogPinUpdated;
            arduino.ConnectionLost += Arduino_ConnectionLost;
        }

        private void Log(string message, bool newLine = true, bool addTimestamp = true)
        {
            if (newLine)
            {
                message += Environment.NewLine;
            }
            if (addTimestamp)
            {
                message = $"{DateTime.Now.ToString()} - {message}";
            }
            tbLog.BeginInvoke(() =>
            {
                tbLog.AppendText(message);
                tbLog.SelectionStart = tbLog.Text.Length - 1;
            });
        }

        private void Arduino_ConnectionLost()
        {
            Log("Connection lost");
            cbComPorts.BeginInvoke(() =>
            {
                UpdatePorts();
            });
        }

        private void Arduino_AnalogPinUpdated(int pin, int value)
        {
            numSensor.BeginInvoke(() =>
            {
                numSensor.Value = value;
            });

            if (!autoAdjustBrightness)
            {
                return;
            }

            int? newBrightness = BrightnessAdjustment.AdjustBrightness(value);
            if (newBrightness.HasValue)
            {
                Log($"Brightness automatically changed to {newBrightness}%");
                numMonitorBr.BeginInvoke(() =>
                {
                    UpdateMonitorBrightness(newBrightness.Value);
                });
            }
        }

        private void UpdateMonitorBrightness(int newBrightness)
        {
            numMonitorBr.ValueChanged -= numMonitorBr_ValueChanged;
            numMonitorBr.Value = newBrightness;
            numMonitorBr.ValueChanged += numMonitorBr_ValueChanged;
        }

        private void numMonitorBr_ValueChanged(object sender, EventArgs e)
        {
            MonitorBrightness.Set((int)numMonitorBr.Value);
            Log($"Brightness manually changed to {(int)numMonitorBr.Value}%");
        }

        private void bTuggleAutoAdjust_Click(object sender, EventArgs e)
        {
            autoAdjustBrightness = !autoAdjustBrightness;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
            if (WindowState == FormWindowState.Minimized && (forceMinimizeToTray || cursorNotInBar))
            {
                forceMinimizeToTray = false;

                ShowInTaskbar = false;
                trayIcon.Visible = true;
                Hide();
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
            ShowInTaskbar = true;
        }
    }
}
