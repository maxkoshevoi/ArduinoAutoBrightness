using System;
using System.Windows.Forms;

namespace ArduinoAutoBrightness.DesktopApp.Extentions
{
    public static class ControlExtentions
    {
        public static void Invoke(this Control control, Action action)
        {
            control.Invoke(action);
        }

        public static void BeginInvoke(this Control control, Action action)
        {
            control.BeginInvoke(action);
        }
    }
}
