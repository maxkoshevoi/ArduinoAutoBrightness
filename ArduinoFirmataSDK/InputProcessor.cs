using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace LattePanda.Firmata
{
    internal class InputProcessor
    {
        private SerialPort _serialPort;
        private Arduino _arduino;

        private bool _parsingSysex;
        private int _sysexBytesRead;
        private int _waitForData = 0;
        private ArduinoMultiByteCommand _executeMultiByteCommand = ArduinoMultiByteCommand.None;
        private int _multiByteChannel = 0;
        private int[] _storedInputData = new int[Arduino.MAX_DATA_BYTES];

        private int _majorVersion = 0;
        private int _minorVersion = 0;

        public InputProcessor(SerialPort serialPort, Arduino arduino)
        {
            _serialPort = serialPort;
            _arduino = arduino;
        }

        public void InputProcess(Object stateInfo)
        {
            if (!Monitor.TryEnter(_serialPort))
            {
                return;
            }

            try
            {
                AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

                if (!_serialPort.IsOpen)
                {
                    // Signal the waiting thread we are done
                    autoEvent.Set();
                    return;
                }

                if (_serialPort.BytesToRead == 0)
                {
                    return;
                }

                int inputData = _serialPort.ReadByte();
                int command;

                if (_parsingSysex)
                {
                    if (inputData == Arduino.END_SYSEX)
                    {
                        _parsingSysex = false;
                        if (_sysexBytesRead > 5 && _storedInputData[0] == Arduino.I2C_REPLY)
                        {
                            byte[] i2cReceivedData = new byte[(_sysexBytesRead - 1) / 2];
                            for (int i = 0; i < i2cReceivedData.Count(); i++)
                            {
                                i2cReceivedData[i] = (byte)(_storedInputData[(i * 2) + 1] | _storedInputData[(i * 2) + 2] << 7);
                            }
                            _arduino.OnDidI2CDataReveive(i2cReceivedData[0], i2cReceivedData[1], i2cReceivedData.Skip(2).ToArray());

                        }
                        _sysexBytesRead = 0;
                    }
                    else if (_sysexBytesRead < _storedInputData.Length)
                    {
                        _storedInputData[_sysexBytesRead] = inputData;
                        _sysexBytesRead++;
                    }
                    else
                    {
                        throw new InvalidOperationException("Make sure you are using Firmata protocol in your Arduino program.");
                    }
                }
                else if (_waitForData > 0 && inputData < 128)
                {
                    _waitForData--;
                    _storedInputData[_waitForData] = inputData;

                    if (_executeMultiByteCommand != 0 && _waitForData == 0)
                    {
                        // We got everything
                        switch (_executeMultiByteCommand)
                        {
                            case ArduinoMultiByteCommand.DIGITAL_MESSAGE:
                                int currentDigitalInput = (_storedInputData[0] << 7) + _storedInputData[1];
                                for (int i = 0; i < 8; i++)
                                {
                                    if (((1 << i) & (currentDigitalInput & 0xff)) != ((1 << i) & (_arduino._digitalInputData[_multiByteChannel] & 0xff)))
                                    {
                                        if (((1 << i) & (currentDigitalInput & 0xff)) != 0)
                                        {
                                            _arduino.OnDigitalPinChanged((byte)(i + _multiByteChannel * 8), ArduinoDigitalValue.HIGH);
                                        }
                                        else
                                        {
                                            _arduino.OnDigitalPinChanged((byte)(i + _multiByteChannel * 8), ArduinoDigitalValue.LOW);
                                        }
                                    }
                                }
                                _arduino._digitalInputData[_multiByteChannel] = (_storedInputData[0] << 7) + _storedInputData[1];
                                break;
                            case ArduinoMultiByteCommand.ANALOG_MESSAGE:
                                int newValue = (_storedInputData[0] << 7) + _storedInputData[1];
                                if (newValue > 1024 || _arduino._analogInputData[_multiByteChannel] == newValue)
                                {
                                    break;
                                }
                                _arduino._analogInputData[_multiByteChannel] = newValue;
                                _arduino.OnAnalogPinChanged(_multiByteChannel, newValue);
                                break;
                            case ArduinoMultiByteCommand.REPORT_VERSION:
                                _majorVersion = _storedInputData[1];
                                _minorVersion = _storedInputData[0];
                                break;
                        }
                    }
                }
                else
                {
                    if (inputData < 0xF0)
                    {
                        command = inputData & 0xF0;
                        _multiByteChannel = inputData & 0x0F;
                        if (Enum.IsDefined(typeof(ArduinoMultiByteCommand), command) && (ArduinoMultiByteCommand)command != ArduinoMultiByteCommand.None)
                        {
                            _waitForData = 2;
                            _executeMultiByteCommand = (ArduinoMultiByteCommand)command;
                        }
                    }
                    else if (inputData == 0xF0)
                    {
                        _parsingSysex = true;
                        // commands in the 0xF* range don't use channel data
                    }
                }
            }
            finally
            {
                Monitor.Exit(_serialPort);
            }
        }
    }
}
