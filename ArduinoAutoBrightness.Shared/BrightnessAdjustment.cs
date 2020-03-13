using System;
using System.Threading;

namespace ArduinoAutoBrightness.Shared
{
    public static class BrightnessAdjustment
    {
        public static DateTime? LastManualChange { get; private set; }

        public static DateTime InactiveUntil { get; private set; }

        /// <summary>
        /// Changes monitor brightness according to light sensor value
        /// </summary>
        /// <param name="currentBrightness">Value from 0 to 100</param>
        /// <param name="sensorValue">Value from 0 (very bright) to 1024 (very dark)</param>
        /// <returns>New monitor brightness (from 0 to 100) or NULL if it didn't change</returns>
        public static int? AdjustBrightness(int sensorValue)
        {
            if (DateTime.Now < InactiveUntil)
            {
                return null;
            }

            int neededBrightness = GetRecommendedBrightness(sensorValue);
            int currentBrightness = MonitorBrightness.Get();
            int difference = Math.Abs(neededBrightness - currentBrightness);

            if (MonitorBrightness.LastChangedTo != null && currentBrightness != MonitorBrightness.LastChangedTo)
            {
                if (LastManualChange.TimePassed()?.TotalSeconds < 3)
                {
                    InactiveUntil = DateTime.Now.AddSeconds(10);
                    return null;
                }
                LastManualChange = DateTime.Now;
            }

            if (difference <= 1 ||
                (difference < 10 && MonitorBrightness.LastChanged.TimePassed().TotalSeconds < 3))
            {
                return null;
            }

            ChangeBrightness(currentBrightness, neededBrightness);
            return neededBrightness;
        }

        private static int GetRecommendedBrightness(int sensorValue)
        {
            return Math.Max(0, 100 - (sensorValue / 10));
        }

        private static void ChangeBrightness(int currentBrightness, int neededBrightness)
        {
            const int increment = 4;

            int dirrection = neededBrightness > currentBrightness ? 1 : -1;
            for (int i = currentBrightness; Math.Abs(neededBrightness - i) > increment; i += increment * dirrection)
            {
                MonitorBrightness.Set(i);
                Thread.Sleep(1);
            }
            MonitorBrightness.Set(neededBrightness);
        }

        public static TimeSpan? TimePassed(this DateTime? date)
        {
            return DateTime.Now - date;
        }
        
        public static TimeSpan TimePassed(this DateTime date)
        {
            return DateTime.Now - date;
        }
    }
}
