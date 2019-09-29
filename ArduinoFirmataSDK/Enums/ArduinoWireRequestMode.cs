namespace LattePanda.Firmata
{
    public enum ArduinoWireRequestMode
    {
        I2C_MODE_WRITE = 0x00,
        I2C_MODE_READ_ONCE = 0x08,
        I2C_MODE_READ_CONTINUOUSLY = 0x10,
        I2C_MODE_STOP_READING = 0x18
    }
}
