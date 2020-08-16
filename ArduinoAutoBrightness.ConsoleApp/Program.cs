using ArduinoAutoBrightness.Shared;
using LattePanda.Firmata;
using System;
using System.Threading.Tasks;

namespace ArduinoAutoBrightness.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().ConnectToArduino();
            Console.ReadLine();
        }

        private void ConnectToArduino()
        {
            Arduino arduino = null;
            while (true)
            {
                string[] ports;
                while ((ports = Arduino.GetAvailablePorts()).Length == 0)
                {
                    Console.Write("Arduino not found. Press <Enter> to try again.");
                    Console.ReadLine();
                }

                Console.Write($"Connecting to {ports[0]}...");
                try
                {
                    arduino = new Arduino(ports[0]);
                    break;
                }
                catch (Exception ex)
                {
                    Console.Write($"FAIL: {ex.Message}{Environment.NewLine}Press <Enter> to try again.");
                    Console.ReadLine();
                }
            }
            Console.WriteLine("DONE");

            arduino.AnalogPinChanged += Arduino_AnalogPinUpdated;
            arduino.ConnectionLost += Arduino_ConnectionLost;
        }

        private void Arduino_ConnectionLost()
        {
            Console.WriteLine("Connection lost");
            ConnectToArduino();
        }

        private void Arduino_AnalogPinUpdated(int pin, int value)
        {
            Task.Run(() =>
            {
                int? newBrightness = BrightnessAdjustment.AdjustBrightness(value);
                if (newBrightness.HasValue)
                {
                    Console.WriteLine(newBrightness);
                }
            });
        }
    }
}
