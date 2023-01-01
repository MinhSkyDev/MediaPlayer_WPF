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
    /// Interaction logic for FullScreen.xaml
    /// </summary>
    public partial class FullScreen : Window
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
        public FullScreen()
        {
            InitializeComponent();
        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Lay gia tri hien tai cua slide
            // Cap nhat vao player

            double value = progressSlider.Value;
            TimeSpan newPosition = TimeSpan.FromSeconds(value);
            //player.Position = newPosition;

        }
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playing)
            {
                //player.Pause();
                _playing = false;
                playButton.Content = "Play";
                Title = $"Stopped playing: {_shortName}";
                //_timer.Stop();
            }
            else
            {
                _playing = true;
                //player.Play();
                playButton.Content = "Pause";
                Title = $"Playing: {_shortName}";

                //_timer.Start();
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"Images/plus.png", UriKind.Relative);
            bitmap.EndInit();

        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            //player.Stop();
            Title = $"Stopped playing: {_shortName}";
            _playing = false;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
