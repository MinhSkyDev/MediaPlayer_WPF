using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace DemoUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentPlaying = string.Empty;

        private bool _playing = false;

        private string _shortName
        {
            get
            {
                var info = new FileInfo(_currentPlaying);
                var name = info.Name;
                return name;
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            ViewModel.NavigationVM navigationVM = new ViewModel.NavigationVM();
            this.DataContext = navigationVM;

        }

        DispatcherTimer _timer;
        
        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            int hours = player.NaturalDuration.TimeSpan.Hours;
            int minutes = player.NaturalDuration.TimeSpan.Minutes;
            int seconds = player.NaturalDuration.TimeSpan.Seconds;
            //totalPosition.Text = $"{hours}:{minutes}:{seconds}";

            // cập nhật max value của slider
            progressSlider.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
        }
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Tự động chơi tập tin kế tiếp ở đây

        }

        private void Media_click(object sender, RoutedEventArgs e)
        {
            var screen = new FullScreen();
        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Lay gia tri hien tai cua slide
            // Cap nhat vao player

            double value = progressSlider.Value;
            TimeSpan newPosition = TimeSpan.FromSeconds(value);
            player.Position = newPosition;

        }
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playing)
            {
                player.Pause();
                _playing = false;
                playButton.Content = "Play";
                Title = $"Stopped playing: {_shortName}";
                _timer.Stop();
            }
            else
            {
                _playing = true;
                player.Play();
                playButton.Content = "Pause";
                Title = $"Playing: {_shortName}";

                _timer.Start();
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"Images/plus.png", UriKind.Relative);
            bitmap.EndInit();

        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Title = $"Stopped playing: {_shortName}";
            _playing = false;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
