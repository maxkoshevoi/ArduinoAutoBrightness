using System;
using System.Threading;

namespace ArduinoAutoBrightness.Shared
{
    public static class BrightnessAdjustment
    {
        /// <summary>
        /// Changes monitor brightness according to light sensor value
        /// </summary>
        /// <param name="currentBrightness">Value from 0 to 100</param>
        /// <param name="sensorValue">Value from 0 (very bright) to 1024 (very dark)</param>
        /// <returns>New monitor brightness (from 0 to 100) or NULL if it didn't change</returns>
        public static int? AdjustBrightness(int sensorValue)
        {
            int neededBrightness = GetRecommendedBrightness(sensorValue);
            int currentBrightness = MonitorBrightness.Get();
            int difference = Math.Abs(neededBrightness - currentBrightness);

            if (difference <= 1 ||
                (difference < 10 && MonitorBrightness.SinceLastChanged.TotalSeconds < 3))
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
    }
}
