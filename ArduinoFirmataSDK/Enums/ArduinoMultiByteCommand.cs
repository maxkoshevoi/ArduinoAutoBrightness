namespace LattePanda.Firmata
{
    internal enum ArduinoMultiByteCommand
    {
        None = 0,
        DIGITAL_MESSAGE = 0x90, // send data for a digital port
        ANALOG_MESSAGE = 0xE0,  // send data for an analog pin (or PWM)
        REPORT_VERSION = 0xF9   // report firmware version
    }
}
