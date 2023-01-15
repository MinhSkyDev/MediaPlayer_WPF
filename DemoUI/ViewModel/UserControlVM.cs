using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DemoUI.ViewModel
{
    class UserControlVM : INotifyPropertyChanged
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string uri { get; set; }

        public DispatcherTimer timer;

        private bool _playing = false;

        private MediaElement mediaPlayer = new MediaElement();

        public MediaElement MEDIAPlayer
        {
            get
            {
                return mediaPlayer;
            }
            set { 
            
               
                mediaPlayer.Play();
                mediaPlayer.Stop();
            }
        }

        //Event delegate để pass chỉ số thời gian đang được chơi hiện tại tới playbar
        public delegate void PassMediaDurationToNavigation(string duration);
        public event PassMediaDurationToNavigation passMediaDurationToNavigation;

        public delegate void PassMediaDurationToNavigation2(double duration);
        public event PassMediaDurationToNavigation2 passMediaDurationToNavigation2;
       

        public UserControlVM()
        {
            
        }

       

       

        public event PropertyChangedEventHandler PropertyChanged;

        public void setData(string name, string duration, string uri)
        {
            this.name = name;
            this.duration = duration;
            this.uri = uri;

            Uri currentUri = new Uri(uri, UriKind.RelativeOrAbsolute);

            mediaPlayer.Source = currentUri;
            mediaPlayer.Width = (double)1000;
            mediaPlayer.Height = (double)520;
            mediaPlayer.Stretch = Stretch.Fill;
            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            mediaPlayer.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            mediaPlayer.Play();
            mediaPlayer.Stop();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,0,1,0);
            timer.Tick += time_ticker;
        }

        private void time_ticker(object sender, EventArgs e)
        {
            int hours = mediaPlayer.Position.Hours;
            int minutes = mediaPlayer.Position.Minutes;
            int seconds = mediaPlayer.Position.Seconds;
            String totalPosition = "";
            if (seconds < 10)
            {
                totalPosition = $"0{hours}:0{minutes}:0{seconds}";
            }
            else
            {
                totalPosition = $"0{hours}:0{minutes}:{seconds}";
            }

            passMediaDurationToNavigation2?.Invoke(mediaPlayer.Position.TotalSeconds);
            passMediaDurationToNavigation?.Invoke(totalPosition);
        }

        public void changeTimeSpan(double value)
        {
            TimeSpan newPosition = TimeSpan.FromSeconds(value);
            mediaPlayer.Position = newPosition;
        }

        

        public string playVideoImprove()
        {
            string result = "";
            if (_playing)
            {
                mediaPlayer.Pause();
                timer.Stop();
                _playing = false;
                result = @"\Images\img_play.png";
            }
            else
            {
                mediaPlayer.LoadedBehavior = MediaState.Manual;
                mediaPlayer.Play();
                timer.Start();
                _playing = true;
                result = @"\Images\img_pause.png";
            }

            return result;
        }

        public void playVideo()
        {
            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.Play();
            timer.Start();
        }

        public void pauseVideo()
        {
            mediaPlayer.Pause();
            timer.Stop();
        }

    }
}
