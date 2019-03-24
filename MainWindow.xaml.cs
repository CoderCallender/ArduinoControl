using System;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Diagnostics;
using System.Timers;
using System.Windows.Media;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ArduinoControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //global declaration of the serial port so the whole program can see it
        private static SerialPort port;

        //Array to hold the value of the outputs of the Arduino
        private static char[] OutputValues = new char[16];

        //variable for a timer
        private static DispatcherTimer CommsTimer;

        private static byte[] ArduinoBuffer = new byte[20]; //buffer to store data from arduino

        private readonly Ellipse[] pinIndicators;
        private readonly Button[] OutputButtons;
        private readonly ComboBox[] PinModeSelections;

        private readonly ManualResetEventSlim resetEvent = new ManualResetEventSlim();

        public MainWindow()
        {
            InitializeComponent();

            //Create array of type "Ellipse"
            pinIndicators = new Ellipse[]
            {
                Pin1Indicator,
                Pin2Indicator,
                Pin3Indicator,
                Pin4Indicator,
                Pin5Indicator,
                Pin6Indicator,
                Pin7Indicator,
                Pin8Indicator,
                Pin9Indicator,
                Pin10Indicator,
                Pin11Indicator,
                Pin12Indicator,
                Pin13Indicator
            };

            //Create array of type "Button"
            OutputButtons= new Button[]
            {
                Pin1OutButton,
                Pin2OutButton,
                Pin3OutButton,
                Pin4OutButton,
                Pin5OutButton,
                Pin6OutButton,
                Pin7OutButton,
                Pin8OutButton,
                Pin9OutButton,
                Pin10OutButton,
                Pin11OutButton,
                Pin12OutButton,
                Pin13OutButton
            };

            //Create array of type "Button"
            PinModeSelections = new ComboBox[]
            {
                Pin1SetupCombo,
                Pin2SetupCombo,
                Pin3SetupCombo,
                Pin4SetupCombo,
                Pin5SetupCombo,
                Pin6SetupCombo,
                Pin7SetupCombo,
                Pin8SetupCombo,
                Pin9SetupCombo,
                Pin10SetupCombo,
                Pin11SetupCombo,
                Pin12SetupCombo,
                Pin13SetupCombo
            };

            //show list of valid com ports
            foreach (string s in SerialPort.GetPortNames())
            {
                PortCombo.Items.Add(s);
            }

            PortCombo.SelectedIndex = 0;

            #region ComboBox defaults
            //Set up the possible Baud options
            //Can't do it in XAML as we won't be able to bubble down the values
            BaudCombo.Items.Add("1200");
            BaudCombo.Items.Add("2400");
            BaudCombo.Items.Add("4800");
            BaudCombo.Items.Add("9600");
            BaudCombo.Items.Add("19200");

            //Default Baud to 9600
            BaudCombo.SelectedIndex = 3;

            //Set the values in the drop down boxes for setting the port directions
            Pin1SetupCombo.Items.Add("Output");
            Pin1SetupCombo.Items.Add("Input");
            Pin1SetupCombo.SelectedIndex = 0;

            Pin2SetupCombo.Items.Add("Output");
            Pin2SetupCombo.Items.Add("Input");
            Pin2SetupCombo.SelectedIndex = 0;

            Pin3SetupCombo.Items.Add("Output");
            Pin3SetupCombo.Items.Add("Input");
            Pin3SetupCombo.SelectedIndex = 0;

            Pin4SetupCombo.Items.Add("Output");
            Pin4SetupCombo.Items.Add("Input");
            Pin4SetupCombo.SelectedIndex = 0;

            Pin5SetupCombo.Items.Add("Output");
            Pin5SetupCombo.Items.Add("Input");
            Pin5SetupCombo.SelectedIndex = 0;

            Pin6SetupCombo.Items.Add("Output");
            Pin6SetupCombo.Items.Add("Input");
            Pin6SetupCombo.SelectedIndex = 0;

            Pin7SetupCombo.Items.Add("Output");
            Pin7SetupCombo.Items.Add("Input");
            Pin7SetupCombo.SelectedIndex = 0;

            Pin8SetupCombo.Items.Add("Output");
            Pin8SetupCombo.Items.Add("Input");
            Pin8SetupCombo.SelectedIndex = 0;

            Pin9SetupCombo.Items.Add("Output");
            Pin9SetupCombo.Items.Add("Input");
            Pin9SetupCombo.SelectedIndex = 0;

            Pin10SetupCombo.Items.Add("Output");
            Pin10SetupCombo.Items.Add("Input");
            Pin10SetupCombo.SelectedIndex = 0;

            Pin11SetupCombo.Items.Add("Output");
            Pin11SetupCombo.Items.Add("Input");
            Pin11SetupCombo.SelectedIndex = 0;

            Pin12SetupCombo.Items.Add("Output");
            Pin12SetupCombo.Items.Add("Input");
            Pin12SetupCombo.SelectedIndex = 0;

            Pin13SetupCombo.Items.Add("Output");
            Pin13SetupCombo.Items.Add("Input");
            Pin13SetupCombo.SelectedIndex = 0;

            #endregion

            #region Output Button Defaults
            //set the output button defaults
            for(int i = 0; i < 13; i++)
            {
                OutputButtons[i].IsEnabled = false;
            }
            #endregion

            //Grey out the load button until we have connected
            LoadButton.IsEnabled = false;

            //Pin1Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#F4F4F5");
        }

        private void PortCombo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //If the user clicks on the "select port" combo box
            //This code will be run

            //Clear the list first so we don't repeat what we already have
            PortCombo.Items.Clear();
            
            //show list of valid com ports
            foreach (string s in SerialPort.GetPortNames())
            {
                //only add if it is not already in the list
                if(!PortCombo.Items.Contains(s))
                {
                    PortCombo.Items.Add(s);
                }
                
            }

        }

        #region Connect Button
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            //This code will run if the user clicks the connect button

            //check if there is a selected com port
            if(PortCombo.SelectedItem == null)
            {
                MessageBox.Show("Error! No Com port selected");
                return;
            }

            try
            {
                // Instantiate the communications
                // port with the selected settings
                port = new SerialPort(
                  PortCombo.SelectedItem.ToString(), int.Parse(BaudCombo.SelectedItem.ToString()), Parity.None, 8, StopBits.One);

                port.ReadTimeout = 1000; // timeout
                // Open the port for communications
                port.Open();

                //indicate it has worked by blanking out the button
                ConnectButton.IsEnabled = false;

                //let the user talk to arduino now
                LoadButton.IsEnabled = true;
            }
            catch (Exception c)
            {
                //Show the error in a message box
                MessageBox.Show(c.Message,"Port Error!");
            }



        }
        #endregion

        #region Close window
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                port.Close();
                Debug.Print("Closing");
            }
            catch (Exception x)
            {
                MessageBox.Show(String.Format("Error Closing Port!\n{0}", x.Message));
            }
            
        }
        #endregion

        #region Load Button
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //This code will be run when the user presses the Load button

            //Tell the device we want to load the port settings
            port.Write("s");

            //write the data
            //TODO: is there a better way of doing this?
            port.Write(convertToIO(Pin1SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin2SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin3SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin4SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin5SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin6SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin7SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin8SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin9SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin10SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin11SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin12SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin13SetupCombo.SelectedItem.ToString()));

            //Enable the buttons that are outputs...
            for(int i = 0; i < 13; i++)
            {
                //if the user has said that we want the pin as an output
                if((string)PinModeSelections[i].SelectedItem == "Output")
                {
                    //enable the button that controls that pin
                    OutputButtons[i].IsEnabled = true;
                    OutputValues[i] = '0'; //default the value to off
                }
                else
                {
                    //else disable it
                    OutputButtons[i].IsEnabled = false;
                    OutputValues[i] = 'x'; //tell the device we are not using this pin
                }
            }

        }
        #endregion

        #region Helpers
        /// <summary>
        /// Helper to convert what's in the combo box to something useful to the Arduino
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string convertToIO(string val)
        {

            //Take the string and convert it to a number
            if (val == "Output")
                return "0";
            else if (val == "Input")
                return "1";
            else
            {
                MessageBox.Show("Setup Error!");
                return "1";  //invalid input, default to input
            }
                
        }

        //convert the pins value to a colour for the GUI
        private string ValueToRGB(byte val)
        {
            if (val == 0)
            {
                return "#FF0000";   //Red
            }
                
            else if (val == 1)
            {
                return "#00FF00";   //Green
            }

            else
            {
                return "#D3D3D3";
            }
        
        }

        /// <summary>
        /// Changes a colour based on its current value
        /// </summary>
        /// <param name="currentVal"></param>
        /// <returns></returns>
        private string ToggleButtonColour(string currentVal)
        {
            //if it is green, go red
            if (currentVal == "#FF00FF00")
            {
                return "#D3D3D3";   //Grey
            }
            //else go green!
            else
            {
                return "#00FF00";   //Green
            }
        }

        /// <summary>
        /// Function to find out what value we should send to the device, based on the button state
        /// </summary>
        /// <param name="PressedButton"></param>
        private char GetPinValue(Button PressedButton)
        {
            //find out what the button colour has been set to
            if (PressedButton.Background.ToString() == "#FF00FF00")
            {
                return '1';  //tell the arduino to set this pin high
            }

            else
            {
                return '0'; //tell the arduino to set this pin low
            }
        }

        #endregion

        private void SetTimer()
        {
           
            // Create a timer with a two second interval.
            CommsTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, OnTimedEvent, Dispatcher);
            // Hook up the Elapsed event for the timer. 
            CommsTimer.IsEnabled = true;
        }

        
        private void OnTimedEvent(Object source, EventArgs e)
        {
            //This is where we can read the data every time the timer triggers

            //send a request to device (g for get)
            try
            {
                port.Write("g");
            }
            catch 
            {
                CommsTimer.IsEnabled = false; //stop the timer from being called again
                MessageBox.Show("Error writing to device!\nPlease check connection");
                return; //exit
            }
            
            //TODO: try and use the port data received event to get rid of wait
            Thread.Sleep(100);
            try
            {
                port.Read(ArduinoBuffer, 0, 13);
            }
            catch
            {
                CommsTimer.IsEnabled = false;
                MessageBox.Show("Error reading device!");
                return; //exit
            }

            for(int x = 0; x < 13; x++)
            {
                Debug.Print(ArduinoBuffer[x].ToString());
            }

            //update the GUI

         
        
                for(var i = 0; i < 13; i++)
                {
                    //update the pin indicator 
                    pinIndicators[i].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[i]));
                }
           
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            //this function will run when the user clicks the run button

            if((String)RunButton.Content == "Run")
            {
                //Start timer so we can read the data from the device
                SetTimer();
                RunButton.Content = "Stop";
            }

            else
            {
                CommsTimer.IsEnabled = false; //stop the timer
                RunButton.Content = "Run";

              //  Thread.Sleep(600); //wait for longer than the timer to make sure that it won't 

         
                    for (var i = 0; i < 13; i++)
                    {
                        //update the pin indicators to all grey
                        pinIndicators[i].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#D3D3D3");
                        Thread.Sleep(10);
                    }
               
            }



            
        }

        private void Pin1OutButton_Click(object sender, RoutedEventArgs e)
        {
            //All output button clicks come here...

            //set a variable that holds all of the buttons info
            Button PressedButton = sender as Button;

            
            //change the button colour
            Dispatcher.Invoke(() => PressedButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(ToggleButtonColour(PressedButton.Background.ToString())));

            //get the value we need to set the relevant pin to (high or low)
            var pinstate = GetPinValue(PressedButton);

            switch (PressedButton.Name)
            {
                case "Pin1OutButton":
                    OutputValues[1] = pinstate;
                    break;

                case "Pin2OutButton":
                    OutputValues[2] = pinstate;
                    break;

                case "Pin3OutButton":
                    OutputValues[3] = pinstate;
                    break;

                case "Pin4OutButton":
                    OutputValues[4] = pinstate;
                    break;

                case "Pin5OutButton":
                    OutputValues[5] = pinstate;
                    break;

                case "Pin6OutButton":
                    OutputValues[6] = pinstate;
                    break;

                case "Pin7OutButton":
                    OutputValues[7] = pinstate;
                    break;

                case "Pin8OutButton":
                    OutputValues[8] = pinstate;
                    break;

                case "Pin9OutButton":
                    OutputValues[9] = pinstate;
                    break;

                case "Pin10OutButton":
                    OutputValues[10] = pinstate;
                    break;

                case "Pin11OutButton":
                    OutputValues[11] = pinstate;
                    break;

                case "Pin12OutButton":
                    OutputValues[12] = pinstate;
                    break;

                case "Pin13OutButton":
                    OutputValues[13] = pinstate;
                    break;
            }


            //Stop timer here to avoid comms conflicts
           // CommsTimer.IsEnabled = false;

            port.Write("o");    //tell the device that we want it to prepare to change the outputs

            for(int i = 0; i < OutputValues.Length; i++)
            {
                port.Write(OutputValues[i].ToString());
                Debug.Write(OutputValues[i].ToString());
            }

            //start the timer again
           // CommsTimer.IsEnabled = true;
        }
    }


}
