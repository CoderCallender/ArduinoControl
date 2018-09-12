using System;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Diagnostics;

namespace ArduinoControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //global declaration of the serial port so the whole program can see it
        SerialPort port;

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

            //send something to Arduino....
            port.Write("a");
        }
        #endregion
    }
}
