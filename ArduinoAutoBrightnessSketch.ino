#include <Firmata.h>

byte analogPin = 0;

void analogWriteCallback(byte pin, int value)
{
  if (IS_PIN_PWM(pin)) {
    pinMode(PIN_TO_DIGITAL(pin), OUTPUT);
    analogWrite(PIN_TO_PWM(pin), value);
  }
}

void setup()
{
  Firmata.setFirmwareVersion(FIRMATA_FIRMWARE_MAJOR_VERSION, FIRMATA_FIRMWARE_MINOR_VERSION);
  Firmata.attach(ANALOG_MESSAGE, analogWriteCallback);
  Firmata.begin(57600);
}

void loop()
{
  while (Firmata.available()) {
    Firmata.processInput();
  }
  Firmata.sendAnalog(analogPin, analogRead(analogPin));
  delay(100);
}
