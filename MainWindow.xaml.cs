using System;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Diagnostics;
using System.Timers;
using System.Windows.Media;

namespace ArduinoControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //global declaration of the serial port so the whole program can see it
        private static SerialPort port;

        //variable for a timer
        private static Timer CommsTimer;

        private static byte[] ArduinoBuffer = new byte[20]; //buffer to store data from arduino

       
        public MainWindow()
        {
            InitializeComponent();

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
            port.Write(convertToIO(Pin8SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin10SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin11SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin12SetupCombo.SelectedItem.ToString()));
            port.Write(convertToIO(Pin13SetupCombo.SelectedItem.ToString()));
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
        #endregion

        private void SetTimer()
        {
           
            // Create a timer with a two second interval.
            CommsTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            CommsTimer.Elapsed += OnTimedEvent;
            CommsTimer.AutoReset = true;
            CommsTimer.Enabled = true;
        }

        
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //This is where we can read the data every time the timer triggers

            //send a request to device (g for get)
            port.Write("g");

            try
            {
                port.Read(ArduinoBuffer, 0, 13);
            }
            catch
            {
                //TODO: catch timeout here
            }

            for(int x = 0; x < 13; x++)
            {
                Debug.Print(ArduinoBuffer[x].ToString());
            }

            //update the GUI

            
            this.Dispatcher.Invoke(() => Pin1Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[0])));
            this.Dispatcher.Invoke(() => Pin2Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[1])));
            this.Dispatcher.Invoke(() => Pin3Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[2])));
            this.Dispatcher.Invoke(() => Pin4Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[3])));
            this.Dispatcher.Invoke(() => Pin5Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[4])));
            this.Dispatcher.Invoke(() => Pin6Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[5])));
            this.Dispatcher.Invoke(() => Pin7Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[6])));
            this.Dispatcher.Invoke(() => Pin8Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[7])));
            this.Dispatcher.Invoke(() => Pin9Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[8])));
            this.Dispatcher.Invoke(() => Pin10Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[9])));
            this.Dispatcher.Invoke(() => Pin11Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[10])));
            this.Dispatcher.Invoke(() => Pin12Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[11])));
            this.Dispatcher.Invoke(() => Pin13Indicator.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(ValueToRGB(ArduinoBuffer[12])));


        }

        private void UpdateIndicators()
        {

        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            //this function will run when the user clicks the run button

            //Start timer so we can read the data from the device
            SetTimer();

            //Grey out the button, to indicate that we are already doing it
            RunButton.IsEnabled = false;
        }
    }


}
