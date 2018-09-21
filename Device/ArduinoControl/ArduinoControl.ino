/*
  Serial Call and Response
  Language: Wiring/Arduino

  This program sends an ASCII A (byte of value 65) on startup and repeats that
  until it gets some data in. Then it waits for a byte in the serial port, and
  sends three sensor values whenever it gets a byte in.

  The circuit:
  - potentiometers attached to analog inputs 0 and 1
  - pushbutton attached to digital I/O 2

  created 26 Sep 2005
  by Tom Igoe
  modified 24 Apr 2012
  by Tom Igoe and Scott Fitzgerald
  Thanks to Greg Shakar and Scott Fitzgerald for the improvements

  This example code is in the public domain.

  http://www.arduino.cc/en/Tutorial/SerialCallResponse
*/

int firstSensor = 0;    // first analog sensor
int secondSensor = 0;   // second analog sensor
int thirdSensor = 0;    // digital sensor
int inByte = 0;         // incoming serial byte
int LED = 13;           //LED is on pin 13?
static int PinState[14];       //Array to hold the values of the pin settings

void setup() {
  // start serial port at 9600 bps:
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  pinMode(2, INPUT);   // digital sensor is on digital pin 2
  pinMode(LED, OUTPUT);  
  pinMode(7, INPUT);
  digitalWrite(LED, LOW);

}

void loop() {
  // if we get a valid byte, read analog ins:
  
  if (Serial.available() > 0) 
  {
    
    // get incoming byte:
    inByte = Serial.read();

    switch (inByte)
    {
      case 's':
      //PC is saying it want's to load settings
        LoadSettings();
        break; 

      case 'g':
        //PC has asked us to get input data
        SendInfo();
        break;

      case 'o':
        //PC has asked us to set the outputs
        SetOutputs();
        break;
    }
    
 
  }
}



void LoadSettings(void)
{
  unsigned char BytesReceived = 0;
  unsigned char DataReceived[13];

  //should get all 13 digital ports from PC (should have a timeout later on in dev)
  while(BytesReceived < 13)
  {
    if (Serial.available() > 0)
    {
      //put it in an array for now, so we don't miss the next one
      DataReceived[BytesReceived] = Serial.read();

      //we have received a byte so inc the counter
      BytesReceived++;
    }
  }
  
  digitalWrite(LED, !digitalRead(LED)); //debug 

  //set the ports as per the received settings
  for(int x = 0; x < BytesReceived; x++)
  {
    //Check and set
    if(DataReceived[x] == '0')
    {
      PinState[x + 1] = 0; //save the settings for later
      pinMode((x + 1), OUTPUT);
    }
    else
    {
      //default to input if the PC sends something weird...
      PinState[x + 1] = 1;  //save the settings for later
      pinMode((x + 1), INPUT);
    }
  }
}

void SendInfo(void)
{
  char temp; 
  for(int i = 0; i < 13; i++)
  {
    //Check if it is an output
    if(PinState[i + 1] == 0)
 /// if(1)
    {
      //it is, so write back saying so
      Serial.write('x'); // x to indicate that we won't read it
    }
    else
    {
      //It is an input, so read the value
    //  temp = '1';
      temp = (char)digitalRead(i + 1);
   
      Serial.write(temp);
    }
    
  }
}

void SetOutputs(void)
{
  int x;
  unsigned char BytesReceived = 0;
  unsigned char DataReceived[13];
    
  //get the instructions from the PC
  //should get all 13 digital ports from PC (should have a timeout later on in dev)
  while(BytesReceived < 13)
  {
    if (Serial.available() > 0)
    {
      //put it in an array for now, so we don't miss the next one
      DataReceived[BytesReceived] = Serial.read();

      //we have received a byte so inc the counter
      BytesReceived++;
    }
  } 

  //now cycle through the list and set the values apropriately
  for(x = 1; x < 13; x++)
  {
    if(DataReceived[x] == '0')
    {
      //set the port low
      digitalWrite(x,LOW);
    }
    else if(DataReceived[x] == '1')
    {
      //set the port high
      digitalWrite(x,HIGH);
    }
    else
    {
      //pin is set to an input, so do nothing
    }
  }
}

