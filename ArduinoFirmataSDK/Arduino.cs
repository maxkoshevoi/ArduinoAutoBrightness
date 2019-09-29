/************************************************************
* Copyright(C),2016-2017,LattePanda
* FileName: arduino.cs
* Author:     Kevlin Sun
* Version:    V0.8
* Date:         2016.7
* Description: LattePanda.Firmata is an open-source Firmata
    library provided by LattePanda, which is suitable for
    Windows apps developed in Visual Studio. this class allows
    you to control the Arduino board from Windows apps:
    reading and writing to the digital pins
    reading the analog inputs
    controlling servo
    send and receive data to the I2C Bus
* This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.
* Special thanks to Tim Farley, on whose Firmata.NET library
    this code is based.
*************************************************************/
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace LattePanda.Firmata
{
    public class Arduino : IDisposable
    {
        #region Variables
        #region Events
        public delegate void DidI2CDataReveiveEventHandler(byte address, byte register, byte[] data);
        public delegate void DigitalPinChangedEventHandler(byte pin, ArduinoDigitalValue state);
        public delegate void AnalogPinChangedEventHandler(int pin, int value);

        public event DidI2CDataReveiveEventHandler DidI2CDataReveive;
        public event DigitalPinChangedEventHandler DigitalPinChanged;
        public event AnalogPinChangedEventHandler AnalogPinChanged;
        public event Action ConnectionLost;
        #endregion

        #region Constants
        public const int NONE = -1;

        public const int MAX_DATA_BYTES = 64;
        public const int START_SYSEX = 0xF0; // start a MIDI SysEx message
        public const int END_SYSEX = 0xF7; // end a MIDI SysEx message
        public const int I2C_REPLY = 0x77; // I2C reply messages from an I/O board to a host

        private const int TOTAL_PORTS = 2;
        private const int SERVO_CONFIG = 0x70; // set max angle, minPulse, maxPulse, freq
        private const int REPORT_ANALOG = 0xC0; // enable analog input by pin #
        private const int REPORT_DIGITAL = 0xD0; // enable digital input by port
        private const int SET_PIN_MODE = 0xF4; // set a pin to INPUT/OUTPUT/PWM/etc
        private const int SYSTEM_RESET = 0xFF; // reset from MIDI
        private const int I2C_REQUEST = 0x76; // I2C request messages from a host to an I/O board
        private const int I2C_CONFIG = 0x78; // Configure special I2C settings such as power pins and delay times
        private readonly SerialPort _serialPort;
        private readonly int _delay;
        
        public volatile int[] _digitalOutputData = new int[MAX_DATA_BYTES];
        public volatile int[] _digitalInputData = new int[MAX_DATA_BYTES];
        public volatile int[] _analogInputData = new int[MAX_DATA_BYTES];
        #endregion

        private Thread _readThread = null;
        private bool isDisposed = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the Arduino object using default arguments.
        /// Assumes the arduino is connected as the HIGHEST serial port on the machine,
        /// default baud rate (57600), and a reboot delay (8 seconds).
        /// and automatically opens the specified serial connection.
        /// </summary>
        public Arduino() : this(Arduino.GetAvailablePorts().Last()) { }

        /// <summary>
        /// Creates an instance of the Arduino object, based on a user-specified serial port.
        /// Assumes default values for baud rate (57600) and reboot delay (8 seconds)
        /// and automatically opens the specified serial connection.
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        public Arduino(string serialPortName) : this(serialPortName, autoStart: true) { }

        /// <summary>
        /// Creates an instance of the Arduino object, based on user-specified serial port and baud rate.
        /// Assumes default value for reboot delay (8 seconds).
        /// and automatically opens the specified serial connection.
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        /// <param name="baudRate">Baud rate.</param>
        public Arduino(string serialPortName, int baudRate) : this(serialPortName, baudRate, autoStart: true) { }
        
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        /// <param name="baudRate">The baud rate of the communication. Default 57600</param>
        /// <param name="autoStart">Determines whether the serial port should be opened automatically.
        /// Use the Open() method to open the connection manually.</param>
        /// <param name="_delay">Time delay that may be required to allow some arduino models
        /// to reboot after opening a serial connection. The delay will only activate
        /// when autoStart is true.</param> 
        /// <param name="autoListen">This parameter activate listen mode. Without this mode you will be
        /// unable to use events. This param will only activate when autoStart is true.</param>
        public Arduino(string serialPortName, int baudRate = 57600, bool autoStart = true, int delay = 0, bool autoListen = true)
        {
            _serialPort = new SerialPort(serialPortName, baudRate)
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };

            if (autoStart)
            {
                this._delay = delay;
                this.Open(autoListen);
            }
        }
        #endregion

        #region Public Properies
        /// <summary>
        /// Gets a value indicating the open or closed status of the System.IO.Ports.SerialPort
        /// </summary>
        public bool Available => _serialPort.IsOpen;

        /// <summary>
        /// Gets the port for communications, including but not limited to all available COM ports.
        /// </summary>
        public string PortName => _serialPort.PortName;

        /// <summary>
        /// Gets the serial baud rate for communication with Arduino.
        /// </summary>
        public int BaudRate => _serialPort.BaudRate;
        #endregion

        #region Event Handlers
        internal void OnDidI2CDataReveive(byte address, byte register, byte[] data)
        {
            if (isDisposed)
            {
                return;
            }
            DidI2CDataReveive?.Invoke(address, register, data);
        }

        internal void OnDigitalPinChanged(byte pin, ArduinoDigitalValue state)
        {
            if (isDisposed)
            {
                return;
            }
            DigitalPinChanged?.Invoke(pin, state);
        }

        internal void OnAnalogPinChanged(int pin, int value)
        {
            if (isDisposed)
            {
                return;
            }
            AnalogPinChanged?.Invoke(pin, value);
        }

        internal void OnConnectionLost()
        {
            if (isDisposed)
            {
                return;
            }
            ConnectionLost?.Invoke();
        }
        #endregion

        #region Monitoring Serial Port
        /// <summary>
        /// Opens the serial port connection, should it be required. By default the port is
        /// opened when the object is first created.
        /// </summary>
        public void Open(bool isListen)
        {
            _serialPort.Open();

            Thread.Sleep(_delay);

            byte[] command = new byte[2];
            for (int i = 0; i < 6; i++)
            {
                command[0] = (byte)(REPORT_ANALOG | i);
                command[1] = (byte)1;
                _serialPort.Write(command, 0, command.Length);
            }

            for (int i = 0; i < 2; i++)
            {
                command[0] = (byte)(REPORT_DIGITAL | i);
                command[1] = (byte)1;
                _serialPort.Write(command, 0, command.Length);
            }

            if (isListen)
            {
                StartListen();
            }
        }
        /// <summary>
        /// Closes the serial port.
        /// </summary>
        public void Close()
        {
            _serialPort.Close();
        }

        /// <summary>
        /// Start separate thread to monitor Firmata data for events DidI2CDataReveive, DigitalPinUpdated and AnalogPinUpdated
        /// </summary>
        public void StartListen()
        {
            if (_readThread == null)
            {
                _readThread = new Thread(ProcessInput);
                _readThread.Start();
            }
        }

        /// <summary>
        /// Stop monitor thread
        /// </summary>
        public void StopListen()
        {
            if (_readThread != null)
            {
                // Hold child thread
                _readThread.Join(500);
                // GC thread
                _readThread = null;
            }
        }
        
        /// <summary>
        /// This function executes only in thread
        /// </summary>
        private void ProcessInput()
        {
            var autoEvent = new AutoResetEvent(false);
            var inputProcessor = new InputProcessor(_serialPort, this);
            // Execute method by a timer every 5ms
            var stateTimer = new Timer(inputProcessor.InputProcess, autoEvent, 0, 5);

            // Wait when method return anything
            autoEvent.WaitOne();
            // This line will executes only when inputProcessor are done. For example serialPort is closed.
            stateTimer.Dispose();
            OnConnectionLost();
        }
        #endregion

        #region Other Methods
        /// <summary>         
        /// Lists all available serial ports on current system.
        /// </summary>
        /// <returns>An array of strings containing all available serial ports.</returns>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// Sets the mode of the specified pin (INPUT or OUTPUT).
        /// </summary>
        /// <param name="pin">The arduino pin.</param>
        /// <param name="mode">Mode Arduino.INPUT Arduino.OUTPUT Arduino.ANALOG Arduino.PWM or Arduino.SERVO .</param>
        public void PinMode(int pin, ArduinoPinMode mode)
        {
            byte[] message = new byte[3];
            message[0] = (byte)(SET_PIN_MODE);
            message[1] = (byte)(pin);
            message[2] = (byte)(mode);
            _serialPort.Write(message, 0, message.Length);
            message = null;
        }
        #endregion

        #region Read & Write
        /// <summary>
        /// Returns the last known state of the digital pin.
        /// </summary>
        /// <param name="pin">The arduino digital input pin.</param>
        /// <returns>Arduino.HIGH or Arduino.LOW</returns>
        public ArduinoDigitalValue DigitalRead(int pin)
        {
            return (ArduinoDigitalValue)((_digitalInputData[pin >> 3] >> (pin & 0x07)) & 0x01);
        }

        /// <summary>
        /// Returns the last known state of the analog pin.
        /// </summary>
        /// <param name="pin">The arduino analog input pin.</param>
        /// <returns>A value representing the analog value between 0 (0V) and 1023 (5V).</returns>
        public int AnalogRead(int pin)
        {
            return _analogInputData[pin];
        }
        /// <summary>
        /// Write to a digital pin that has been toggled to output mode with pinMode() method.
        /// </summary>
        /// <param name="pin">The digital pin to write to.</param>
        /// <param name="value">Value either Arduino.LOW or Arduino.HIGH.</param>
        public void DigitalWrite(int pin, ArduinoDigitalValue value)
        {
            int portNumber = (pin >> 3) & 0x0F;
            byte[] message = new byte[3];

            if (value == ArduinoDigitalValue.LOW)
                _digitalOutputData[portNumber] &= ~(1 << (pin & 0x07));
            else
                _digitalOutputData[portNumber] |= (1 << (pin & 0x07));

            message[0] = (byte)((int)ArduinoMultiByteCommand.DIGITAL_MESSAGE | portNumber);
            message[1] = (byte)(_digitalOutputData[portNumber] & 0x7F);
            message[2] = (byte)(_digitalOutputData[portNumber] >> 7);
            _serialPort.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Write to an analog pin using Pulse-width modulation (PWM).
        /// </summary>
        /// <param name="pin">Analog output pin.</param>
        /// <param name="value">PWM frequency from 0 (always off) to 255 (always on).</param>
        public void AnalogWrite(int pin, int value)
        {
            byte[] message = new byte[3];
            message[0] = (byte)((int)ArduinoMultiByteCommand.ANALOG_MESSAGE | (pin & 0x0F));
            message[1] = (byte)(value & 0x7F);
            message[2] = (byte)(value >> 7);
            _serialPort.Write(message, 0, message.Length);
        }
        /// <summary>
        /// controlling servo
        /// </summary>
        /// <param name="pin">Servo output pin.</param>
        /// <param name="angle">Servo angle from 0 to 180.</param>
        public void ServoWrite(int pin, int angle)
        {
            byte[] message = new byte[3];
            message[0] = (byte)((int)ArduinoMultiByteCommand.ANALOG_MESSAGE | (pin & 0x0F));
            message[1] = (byte)(angle & 0x7F);
            message[2] = (byte)(angle >> 7);
            _serialPort.Write(message, 0, message.Length);
        }
        /// <summary>
        /// Init I2C Bus.
        /// </summary>
        /// <param name="angle">delay is necessary for some devices such as WiiNunchuck</param>
        public void WireBegin(Int16 delay)
        {
            byte[] message = new byte[5];
            message[0] = (byte)(0XF0);
            message[1] = (byte)(I2C_CONFIG);
            message[2] = (byte)(delay & 0x7F);
            message[3] = (byte)(delay >> 7);
            message[4] = (byte)(END_SYSEX);
            _serialPort.Write(message, 0, message.Length);
        }
        /// <summary>
        /// Write to a digital pin that has been toggled to output mode with pinMode() method.
        /// </summary>
        /// <param name="slaveAddress">I2C slave address,7 bit</param>
        /// <param name="slaveRegister">value either I2C slave Register or Arduino.NONE</param>
        /// <param name="data">Write data or length of read data.</param>
        /// <param name="mode">Value either Arduino.I2C_MODE_WRITE or Arduino.I2C_MODE_READ_ONCE or Arduino.I2C_MODE_READ_ONCE or Arduino.I2C_MODE_STOP_READING</param>
        public void WireRequest(byte slaveAddress, Int16 slaveRegister, Int16[] data, ArduinoWireRequestMode mode)
        {
            byte[] message = new byte[MAX_DATA_BYTES];
            message[0] = (byte)(0xF0);
            message[1] = (byte)(I2C_REQUEST);
            message[2] = (byte)(slaveAddress);
            message[3] = (byte)(mode);
            int index = 4;
            if (slaveRegister != Arduino.NONE)
            {
                message[index] = (byte)(slaveRegister & 0x7F);
                index += 1;
                message[index] = (byte)(slaveRegister >> 7);
                index += 1;
            }
            for (int i = 0; i < (data.Count()); i++)
            {
                message[index] = (byte)(data[i] & 0x7F);
                index += 1;
                message[index] = (byte)(data[i] >> 7);
                index += 1;
            }
            message[index] = (byte)(END_SYSEX);
            _serialPort.Write(message, 0, index + 1);
        }
        #endregion
        
        public void Dispose()
        {
            isDisposed = true;
            Close();
        }
    }
}
