using System;
using System.Diagnostics;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace RobotArm_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> DemoItems { get; set; }
        public SerialPort SerialPort { get; set; }
        private string port = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            
            DemoItems = new ObservableCollection<string>
            {
                "Send Degrees",
                "Ask positions"
            };

            DemoItemsListBox.ItemsSource = DemoItems;
            //SerialPort serialPort = new SerialPort();
            string[] ports = SerialPort.GetPortNames();
            if(ports.Length > 0)
                Puertos.ItemsSource = ports;
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;

            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
            => DemoItemsListBox.Focus();

        //CONECTAR CON ARDUINO
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SerialPort = new SerialPort();
            if(Puertos.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un puerto del combobox.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            port = Puertos.SelectedItem.ToString();
            if(!string.IsNullOrEmpty(port))
            {
                try
                {
                    SerialPort.PortName = port;
                    SerialPort.BaudRate = 9600;
                    SerialPort.ReadTimeout = 500;
                    SerialPort.WriteTimeout = 500;

                    SerialPort.Open();

                    StatusBar.Background = Brushes.DarkGreen;
                    StatusText.Text = "Conectado";
                }
                catch (Exception)
                {
                    MessageBox.Show("Hubó un error al tratar de conectar el microcontrolador", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusBar.Background = Brushes.DarkMagenta;
                StatusText.Text = "Desconectado";
                Q0.Text = string.Empty;
                Q1.Text = string.Empty;
                Q2.Text = string.Empty;

                SerialPort.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Hubó un error al tratar de desconectar el microcontrolador", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(Q0.Text) && !string.IsNullOrEmpty(Q1.Text) && !string.IsNullOrEmpty(Q2.Text))
            {
                try
                {
                    int q0 = int.Parse(Q0.Text);
                    int q1 = int.Parse(Q1.Text);
                    int q2 = int.Parse(Q2.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Solo se permiten números enteros.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                try
                {
                    string sendValue = Q0.Text.Trim() + "," + Q1.Text.Trim() + "," + Q2.Text.Trim();
                    SerialPort.WriteLine(sendValue);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No se tienen todos los campos llenos.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
