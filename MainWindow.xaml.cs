using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace ArduinoControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //show list of valid com ports
            foreach (string s in SerialPort.GetPortNames())
            {
                PortCombo.Items.Add(s);
            }

            PortCombo.SelectedIndex = 0;

            //Set up the possible Baud options
            //Can't do it in XAML as we won't be able to bubble down the values
            BaudCombo.Items.Add("1200");
            BaudCombo.Items.Add("2400");
            BaudCombo.Items.Add("4800");
            BaudCombo.Items.Add("9600");
            BaudCombo.Items.Add("19200");

            //Default Baud to 9600
            BaudCombo.SelectedIndex = 3;
        }

        private void PortCombo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //If the user clicks on the "select port" combo box
            //This code will be run

            //Clear the list first
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
                SerialPort port = new SerialPort(
                  PortCombo.SelectedItem.ToString(), int.Parse(BaudCombo.SelectedItem.ToString()), Parity.None, 8, StopBits.One);

                // Open the port for communications
                port.Open();

                //send something to Arduino....
                port.Write("a");


                //indicate it has worked by blanking out the button
                ConnectButton.IsEnabled = false;
            }
            catch
            {
                MessageBox.Show("Port Error!");
            }



        }
    }
}
