//air_mouse_with_click
#include <Wire.h>
#include <I2Cdev.h>
#include <MPU6050.h>
#include <Mouse.h>

MPU6050 mpu;
int16_t ax, ay, az, gx, gy, gz;
int vx, vy, vx_prec, vy_prec;
int count=0;

void setup() {
  Serial.begin(9600);
  Wire.begin();
  mpu.initialize();
  if (!mpu.testConnection()) {
    while (1);
    }
}

void loop() {
  mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

  vx = (gx-500)/150;
  vy = -(gz-100)/150;


  Serial.print("gx = ");
  Serial.print(gx);
  Serial.print(" | gz = ");
  Serial.print(gz);
  
  Serial.print("        | X = ");
  Serial.print(vx);
  Serial.print(" | Y = ");
  Serial.println(vy);

  Mouse.move(vx, vy);

  // Check if the cursor is still within the 10 pixel threshold
  if ( (vx_prec-10)<=vx && vx<=vx_prec+10 && (vy_prec-10)<=vy && vy<=vy_prec+10) {
    count++;
    if (count == 150) { // After ~2 seconds single click and release
      Mouse.click(MOUSE_LEFT);
    } else if (count >= 300) { // After ~4 seconds start holding
      if (!Mouse.isPressed(MOUSE_LEFT)) {
        Mouse.press(MOUSE_LEFT);
      }
    }
  } else {
    if (Mouse.isPressed(MOUSE_LEFT)) {
      Mouse.release(MOUSE_LEFT); // Release if the cursor moves out of the threshold
    }
    vx_prec = vx; // Update the values to check the position of the pointer
    vy_prec = vy;
    count = 0; // Reset the count when the cursor has moved
  }

  delay(20); //millis（）use it
}