# ArduinoAutoBrightness
Automatically changes brightness of your monitor based on Arduino with analog light sensor

## How to use

1) Download and build repository (you will need .Net Core 3.0)
2) Connect analog light sensor to **A0** pin on your Arduino and upload [ArduinoAutoBrightnessSketch.ino](ArduinoAutoBrightnessSketch.ino "ArduinoAutoBrightnessSketch.ino") on it.
> Ensure that Arduino sends information to your PC using **"Port monitor"** on Arduino IDE or similar tool.
4) Launch **ArduinoAutoBrightness.ConsoleApp** or **ArduinoAutoBrightness.DesktopApp**
> **Note:** The **ArduinoAutoBrightness.DesktopApp** automatically minimizes to tray on launch.
