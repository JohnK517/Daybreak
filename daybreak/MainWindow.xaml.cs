using daybreak.Spotify.Authentication;
using daybreak.Spotify.Services;
using daybreak.Spotify.Models;
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

            // Spotify Update Timer
            spotifyTimer.Interval = TimeSpan.FromSeconds(1);
            spotifyTimer.Tick += SpotifyTimer_Tick;
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

        // Spotify
        private readonly SpotifyAuthentication spotify =
            new SpotifyAuthentication();

        private SpotifyService? spotifyService;

        // Play, Pause, Next, Previous Buttons
        private bool spotifyIsPlaying;

        private async void SpotifyPlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (spotifyService == null)
                return;

            spotifyIsPlaying = !spotifyIsPlaying;

            await spotifyService.PlayPauseAsync(spotifyIsPlaying);

            SpotifyPlayIcon.Visibility =
                spotifyIsPlaying ? Visibility.Collapsed : Visibility.Visible;

            SpotifyPauseIcon.Visibility =
                spotifyIsPlaying ? Visibility.Visible : Visibility.Collapsed;
        }
        private async void SpotifyNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (spotifyService == null)
                return;

            await spotifyService.NextAsync();
        }
        private async void SpotifyPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (spotifyService == null)
                return;

            await spotifyService.PreviousAsync();
        }
        // Track Data
        private async void ConnectSpotify_Click(object sender, RoutedEventArgs e)
        {
            await spotify.AuthenticateAsync();

            if (spotify.Client != null)
            {
                spotifyService = new SpotifyService(spotify.Client);

                var track = await spotifyService.GetCurrentTrackAsync();

                if (track != null)
                {
                    // Song Title
                    SpotifyTitleText.Text = track.Title;
                    // Artist Name
                    SpotifyArtistText.Text = track.Artist;
                    // Album Art
                    if (!string.IsNullOrWhiteSpace(track.AlbumArtUrl))
                    {
                        SpotifyAlbumArt.Source = new BitmapImage(
                            new Uri(track.AlbumArtUrl));
                    }
                    // Song Progress and Duration
                    spotifyProgressMs = track.ProgressMs;
                    spotifyDurationMs = track.DurationMs;

                    SpotifyProgressBar.Maximum = spotifyDurationMs;
                    SpotifyProgressBar.Value = spotifyProgressMs;

                    TimeSpan progress = TimeSpan.FromMilliseconds(spotifyProgressMs);
                    TimeSpan duration = TimeSpan.FromMilliseconds(spotifyDurationMs);

                    SpotifyProgressText.Text =
                        $"{(int)progress.TotalMinutes}:{progress.Seconds:D2}";

                    SpotifyDurationText.Text =
                        $"{(int)duration.TotalMinutes}:{duration.Seconds:D2}";

                    spotifyTimer.Start();
                    // Play/Pause Button
                    spotifyIsPlaying = track.IsPlaying;

                    SpotifyPlayIcon.Visibility =
                        spotifyIsPlaying ? Visibility.Collapsed : Visibility.Visible;

                    SpotifyPauseIcon.Visibility =
                        spotifyIsPlaying ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        // Spotify Update Timer
        private DispatcherTimer spotifyTimer = new DispatcherTimer();
        private int spotifyProgressMs;
        private int spotifyDurationMs;

        private void SpotifyTimer_Tick(object? sender, EventArgs e)
        {
            if (spotifyProgressMs < spotifyDurationMs)
            {
                spotifyProgressMs += 1000;

                SpotifyProgressBar.Value = spotifyProgressMs;

                TimeSpan progress = TimeSpan.FromMilliseconds(spotifyProgressMs);

                SpotifyProgressText.Text =
                    $"{(int)progress.TotalMinutes}:{progress.Seconds:D2}";
            }
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