using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoUI.Utilities;
using System.Windows.Input;
using DemoUI.Model;
using DemoUI.View;

namespace DemoUI.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;

        private Dictionary<string, object> prototype_view;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }


        //For basic data binding in the playback bars
        public string currentMediaName { get; set; }
        public string currentUri { get; set; }
        public string mediaDuration { get; set; }
        public string currentDuration { get; set; }
        private Media currentMedia;

        // data for selected playlist
        private string path, title;

        //Biến này dùng để binding value cho slider
        private double slidervalue;
        public double sliderValue { get { return slidervalue; } 
            set
            {
                slidervalue = value;

                //Ở đây phải check xem liệu có đang có Media nào đang play hay không ?

                //Trường hợp đã check xong Media đã play xong
                //Bắn value này qua cho màn hình 
                UserControlVM userControl1 = (UserControlVM)prototype_view["UserControl"];
                userControl1.changeTimeSpan(slidervalue);
            } 
        }
        public double sliderValueMaximum { get; set; }


        public ICommand HomeCommand { get; set; }
        public ICommand MusicLibraryCommand { get; set; }
        public ICommand VideoLibraryCommand { get; set; }
        public ICommand PlaylistCommand { get; set; }
        public ICommand PlayingCommand { get; set; }


        //ICommand cho Các thao tác với video
        public ICommand playMediaButton { get; set; }
        public ICommand pauseMediaButton { get; set; }
        public ICommand nextMediaButton { get; set; }
        public ICommand previousMediaButton { get; set; }

        private void Home(object obj) => CurrentView = prototype_view["Home"];
        private void MusicLibrary(object obj) => CurrentView = prototype_view["MusicLibrary"];
        private void VideoLibrary(object obj) => CurrentView = prototype_view["VideoLibrary"];
        private void Playlist(object obj) => CurrentView = prototype_view["Playlist"];
        private void Playing(object obj) => CurrentView = prototype_view["UserControl"];

        // Playlist được chọn
        private void SelectedPlaylist(object obj) => CurrentView = prototype_view["SelectedPlaylist"];

        public NavigationVM()
        {
            sliderValueMaximum = 0f;
            prototype_view = new Dictionary<string, object>();

            prototype_view.Add("Home", new HomeVM());
            prototype_view.Add("MusicLibrary", new MusicLibraryVM(this));
            prototype_view.Add("Playlist", new PlaylistVM(this));
            prototype_view.Add("VideoLibrary", new VideoLibraryVM(this));
            prototype_view.Add("UserControl", new UserControlVM());

            prototype_view.Add("SelectedPlaylist", new MusicLibraryVM(this));
            


            //Inject event here
            VideoLibraryVM videoLibraryVM = (VideoLibraryVM)prototype_view["VideoLibrary"];
            videoLibraryVM.passToNavigation += setInfoFromMedia;
            videoLibraryVM.navigateToPlayer += navigateToMediaPlayer;

            MusicLibraryVM musicLibraryVM = (MusicLibraryVM)prototype_view["MusicLibrary"];
            musicLibraryVM.passToNavigationMusic += setInfoFromMedia;
            musicLibraryVM.navigateToPlayer += navigateToMediaPlayer;


            UserControlVM userControl1 = (UserControlVM)prototype_view["UserControl"];
            userControl1.passMediaDurationToNavigation += setCurrentDuration;


            PlaylistVM playListVM = (PlaylistVM)prototype_view["Playlist"];
            playListVM.passToNavigationPath += setPathPlaylist;
            playListVM.navigateToMusic += navigateToMusic;


            HomeCommand = new RelayCommand(Home);
            MusicLibraryCommand = new RelayCommand(MusicLibrary);
            VideoLibraryCommand = new RelayCommand(VideoLibrary);
            PlaylistCommand = new RelayCommand(Playlist);
            PlayingCommand = new RelayCommand(Playing);
            playMediaButton = new RelayCommand(playButton_command);
            pauseMediaButton = new RelayCommand(pauseButton_command);
            nextMediaButton = new RelayCommand(nextMedia_command);
            previousMediaButton = new RelayCommand(previousMedia_command);
            // Startup Page
            CurrentView = prototype_view["Home"];

            
        }

        public void setPathPlaylist(string path, string title)
        {
            this.path = path;
            this.title = title;
        }

        public void setInfoFromMedia(Media media)
        {
            this.currentMediaName = media.name;
            this.mediaDuration = media.duration;
            this.currentUri = media.uri;
            this.currentMedia = media;
        }

       
        /// <summary>
        /// Bắn dữ liệu Media đang chơi hiện tại và chuyển view sang trình chơi Media
        /// </summary>
        public void navigateToMediaPlayer()
        {
            UserControlVM userControl = (UserControlVM)prototype_view["UserControl"];
            userControl.setData(currentMedia.name, currentMedia.duration, currentMedia.uri);
            CurrentView = userControl;

        }

        public void navigateToMusic()
        {
            MusicLibraryVM SelectedPlaylist = (MusicLibraryVM)prototype_view["SelectedPlaylist"];
            SelectedPlaylist.newPlaylist(path, title);
            CurrentView = SelectedPlaylist;
        }

        void playButton_command(Object obj)
        {
            UserControlVM userControl = (UserControlVM)prototype_view["UserControl"];
            userControl.playVideo();
            sliderValueMaximum = userControl.MEDIAPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }


        void nextMedia_command(Object obj)
        {
            string type = currentMedia.getType();
            if (type.Equals("VideoLibrary"))
            {
                VideoLibraryVM videoLibraryVM = (VideoLibraryVM)prototype_view["VideoLibrary"];
                Media nextMedia = videoLibraryVM.getNextMedia();

                if(nextMedia != null)
                {
                    currentMedia = nextMedia;
                    videoLibraryVM.selectedIndex += 1;
                    navigateToMediaPlayer(); 
                }

            }
            else if(type.Equals("MusicLibrary")) 
            {
                MusicLibraryVM musicLibraryVM = (MusicLibraryVM)prototype_view[type];
                Media nextMedia = musicLibraryVM.getNextMedia();

                if (nextMedia != null)
                {
                    currentMedia = nextMedia;
                    musicLibraryVM.selectedIndex += 1;
                    navigateToMediaPlayer();
                }
            }
            else
            {
                //Do nothing
            }

            this.setInfoFromMedia(currentMedia);
        }

        void previousMedia_command(Object obj)
        {
            string type = currentMedia.getType();
            if (type.Equals("VideoLibrary"))
            {
                VideoLibraryVM videoLibraryVM = (VideoLibraryVM)prototype_view["VideoLibrary"];
                Media previousMedia = videoLibraryVM.getPreviousMedia();

                if (previousMedia != null)
                {
                    currentMedia = previousMedia;
                    
                    videoLibraryVM.selectedIndex -= 1;
                    navigateToMediaPlayer();
                }

            }
            else if (type.Equals("MusicLibrary"))
            {
                MusicLibraryVM musicLibraryVM = (MusicLibraryVM)prototype_view["MusicLibrary"];
                Media previousMedia = musicLibraryVM.getPreviousMedia();

                if (previousMedia != null)
                {
                    currentMedia = previousMedia;
                    musicLibraryVM.selectedIndex -= 1;
                    navigateToMediaPlayer();
                }
            }
            else
            {
                //Do nothing
            }

            this.setInfoFromMedia(currentMedia);
        }

        void pauseButton_command(Object obj)
        {
            UserControlVM userControl = (UserControlVM)prototype_view["UserControl"];
            userControl.pauseVideo();
        }

        //Dùng để set duration được pass từ màn hình Usercontrol1 về
        public void setCurrentDuration(string duration)
        {
            this.currentDuration = duration;
        }
        


    }

    public static class ShuffleExtension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }



}
