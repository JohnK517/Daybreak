using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace daybreak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // App Buttons
        private void Flow_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.Navigate(new FlowPage());
        }

        private void Fuel_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.Navigate(new FuelPage());
        }

        private void Form_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.Navigate(new FormPage());
        }

        private void Feel_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.Navigate(new FeelPage());
        }

        // Exit Buttons
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitOverlay.Visibility = Visibility.Visible;
        }

        private void ExitNo_Click(object sender, RoutedEventArgs e)
        {
            ExitOverlay.Visibility = Visibility.Collapsed;
        }

        private void ExitYes_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}