using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoUI.Utilities;
using System.Windows.Input;
using DemoUI.Model;

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
        private Media currentMedia;


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
        public NavigationVM()
        {

            prototype_view = new Dictionary<string, object>();

            prototype_view.Add("Home", new HomeVM());
            prototype_view.Add("MusicLibrary", new MusicLibraryVM(this));
            prototype_view.Add("Playlist", new PlaylistVM());
            prototype_view.Add("VideoLibrary", new VideoLibraryVM(this));
            prototype_view.Add("UserControl", new UserControlVM());
            


            //Inject event here
            VideoLibraryVM videoLibraryVM = (VideoLibraryVM)prototype_view["VideoLibrary"];
            videoLibraryVM.passToNavigation += setInfoFromMedia;
            videoLibraryVM.navigateToPlayer += navigateToMediaPlayer;

            MusicLibraryVM musicLibraryVM = (MusicLibraryVM)prototype_view["MusicLibrary"];
            musicLibraryVM.passToNavigationMusic += setInfoFromMedia;
            musicLibraryVM.navigateToPlayer += navigateToMediaPlayer;

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

        void playButton_command(Object obj)
        {
            UserControlVM userControl = (UserControlVM)prototype_view["UserControl"];
            userControl.playVideo();
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
                
            }
            else
            {
                //Do nothing
            }
        }

        void previousMedia_command(Object obj)
        {
            string type = currentMedia.getType();
            if (type.Equals("VideoLibrary"))
            {
                VideoLibraryVM videoLibraryVM = (VideoLibraryVM)prototype_view["VideoLibrary"];
                Media nextMedia = videoLibraryVM.getPreviousMedia();

                if (nextMedia != null)
                {
                    currentMedia = nextMedia;
                    videoLibraryVM.selectedIndex -= 1;
                    navigateToMediaPlayer();
                }

            }
            else if (type.Equals("MusicLibrary"))
            {

            }
            else
            {
                //Do nothing
            }
        }

        void pauseButton_command(Object obj)
        {
            UserControlVM userControl = (UserControlVM)prototype_view["UserControl"];
            userControl.pauseVideo();
        }

    }
}
