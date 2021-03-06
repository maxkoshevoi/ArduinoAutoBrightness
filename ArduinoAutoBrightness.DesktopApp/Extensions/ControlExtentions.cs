﻿using System;
using System.Windows.Forms;

namespace ArduinoAutoBrightness.DesktopApp.Extensions
{
    public static class ControlExtensions
    {
        public static void Invoke(this Control control, Action action)
        {
            control.Invoke(action);
        }

        public static IAsyncResult BeginInvoke(this Control control, Action action)
        {
            return control.BeginInvoke(action);
        }
    }
}
