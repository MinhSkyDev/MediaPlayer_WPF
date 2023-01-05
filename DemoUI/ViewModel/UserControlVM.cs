using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DemoUI.ViewModel
{
    class UserControlVM : INotifyPropertyChanged
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string uri { get; set; }


        private MediaElement mediaPlayer = new MediaElement();
        public MediaElement MEDIAPlayer
        {
            get
            {
                return mediaPlayer;
            }
            set { 
            
                mediaPlayer = value;
                mediaPlayer.Source = new Uri(uri);
                mediaPlayer.Width = 1000;
                mediaPlayer.Height = 550;
                mediaPlayer.Stretch= Stretch.UniformToFill;
                mediaPlayer.Play();
                //mediaPlayer.Stop();

            }
        }


        public UserControlVM()
        {
            //Do nothing
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void setData(string name, string duration, string uri)
        {
            this.name = name;
            this.duration = duration;
            this.uri = uri;

            Uri currentUri = new Uri(uri, UriKind.RelativeOrAbsolute);

            mediaPlayer.Source = currentUri;
            mediaPlayer.Width = (double)800;
            mediaPlayer.Height = (double)500;
            mediaPlayer.Stretch = Stretch.UniformToFill;
            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            mediaPlayer.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            mediaPlayer.Play();
            mediaPlayer.Stop();
        }

        public void playVideo()
        {           
            mediaPlayer.Play();
        }

        public void pauseVideo()
        {
            mediaPlayer.Pause();
        }

    }
}
