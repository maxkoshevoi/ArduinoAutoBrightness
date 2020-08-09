using System;
using System.Linq;

namespace ArduinoAutoBrightness.Shared
{
    public class GlobalBrightnessController : IDisposable
    {
        public GlobalBrightnessController()
        {
            PhisicalBrightnessController = new PhisicalMonitorBrightnessController();
        }

        public DateTime LastChanged { get; private set; }
        public int? LastChangedTo { get; private set; }

        private PhisicalMonitorBrightnessController PhisicalBrightnessController { get; }

        public void Set(int brightness)
        {
            int brightnessInBounds = Math.Max(0, Math.Min(brightness, 100));

            WindowsSettingsBrightnessController.Set(brightnessInBounds);
            PhisicalBrightnessController.Set((uint)brightnessInBounds);

            LastChanged = DateTime.Now;
            LastChangedTo = brightness;
        }

        public int Get()
        {
            int windowsBrightness = WindowsSettingsBrightnessController.Get();
            int phisicalBrightness = PhisicalBrightnessController.Get();

            if (phisicalBrightness == -1)
            {
                return windowsBrightness;
            }

            return (int)new[] { windowsBrightness, phisicalBrightness }.Average();
        }

        public void Dispose()
        {
            PhisicalBrightnessController.Dispose();
        }
    }
}
