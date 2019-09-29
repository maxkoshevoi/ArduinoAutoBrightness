using System;
using System.Management;

namespace ArduinoAutoBrightness.Shared
{
    public static class MonitorBrightness
    {
        public static DateTime LastChanged { get; private set; }
        public static TimeSpan SinceLastChanged => DateTime.Now - LastChanged;

        public static int Get()
        {
            using (var mclass = new ManagementClass("WmiMonitorBrightness"))
            {
                mclass.Scope = new ManagementScope(@"\\.\root\wmi");
                using (var instances = mclass.GetInstances())
                {
                    foreach (ManagementObject instance in instances)
                    {
                        return (byte)instance.GetPropertyValue("CurrentBrightness");
                    }
                }
            }
            return 0;
        }

        public static void Set(int brightness)
        {
            using (var mclass = new ManagementClass("WmiMonitorBrightnessMethods"))
            {
                mclass.Scope = new ManagementScope(@"\\.\root\wmi");
                using (var instances = mclass.GetInstances())
                {
                    foreach (ManagementObject instance in instances)
                    {
                        object[] args = new object[] { 1, brightness };
                        instance.InvokeMethod("WmiSetBrightness", args);
                    }
                    LastChanged = DateTime.Now;
                }
            }
        }
    }
}
