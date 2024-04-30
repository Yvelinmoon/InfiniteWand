#include <SoftwareSerial.h>   // Library reference
SoftwareSerial BT(10, 11);  // Connect HC05 transmit pin to D8 and receive pin to D9
char val; // Store received data

void setup() {
  Serial.begin(9600);   
  Serial.println("BT is ready!");
  // For HC-06, change to 38400;
  BT.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  // Send data received from serial monitor to Bluetooth module
  if (Serial.available()) {
    val = Serial.read();
    BT.print(val);
  }
  // Send data received from Bluetooth module to serial monitor
  if (BT.available()) {
    val = BT.read();
    Serial.print(val);
  }
}