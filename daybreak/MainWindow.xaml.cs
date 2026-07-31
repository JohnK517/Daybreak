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
using System.Windows.Threading;

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

            // Wake Up Button
            wakeTimer.Interval = TimeSpan.FromSeconds(1);
            wakeTimer.Tick += WakeTimer_Tick;

            // Date and Time
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            Timer_Tick(null, null);
        }

        // Wake Up Button
        private DateTime wakeTime;
        private DispatcherTimer wakeTimer = new DispatcherTimer();
        private void WakeButton_Click(object sender, RoutedEventArgs e)
        {
            wakeTime = DateTime.Now;

            WakeTimeTextBlock.Text = wakeTime.ToString("h:mm tt");

            TimeAwakeTextBlock.Text = "00:00:00";

            wakeTimer.Start();
        }
        private void WakeTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - wakeTime;

            TimeAwakeTextBlock.Text =
                elapsed.ToString(@"hh\:mm\:ss");
        }


        // Date and Time
        private DispatcherTimer timer = new DispatcherTimer();

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            DateTextBlock.Text = now.ToString("dddd, MMMM dd, yyyy");
            TimeTextBlock.Text = now.ToString("hh:mm tt");
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