using System;
using System.Management;

namespace ArduinoAutoBrightness.Shared
{
    public static class MonitorBrightness
    {
        public static DateTime LastChanged { get; private set; }
        public static int? LastChangedTo { get; private set; }

        public static int Get()
        {
        using var mclass = new ManagementClass("WmiMonitorBrightness")
        {
            Scope = new ManagementScope(@"\\.\root\wmi")
        };
        using var instances = mclass.GetInstances();
        foreach (ManagementObject instance in instances)
        {
            return (byte)instance.GetPropertyValue("CurrentBrightness");
        }
        return 0;
        }

        public static void Set(int brightness)
        {
            using var mclass = new ManagementClass("WmiMonitorBrightnessMethods")
            {
                Scope = new ManagementScope(@"\\.\root\wmi")
            };
            using var instances = mclass.GetInstances();
            foreach (ManagementObject instance in instances)
            {
                object[] args = new object[] { 1, brightness };
                instance.InvokeMethod("WmiSetBrightness", args);
            }
            LastChanged = DateTime.Now;
            LastChangedTo = brightness;
        }
    }
}
