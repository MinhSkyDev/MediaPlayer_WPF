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
        public string currentSinger { get; set; }
        public string mediaDuration { get; set; }


        public ICommand HomeCommand { get; set; }
        public ICommand MusicLibraryCommand { get; set; }
        public ICommand VideoLibraryCommand { get; set; }
        public ICommand PlaylistCommand { get; set; }

        private void Home(object obj) => CurrentView = prototype_view["Home"];
        private void MusicLibrary(object obj) => CurrentView = prototype_view["MusicLibrary"];
        private void VideoLibrary(object obj) => CurrentView = prototype_view["VideoLibrary"];
        private void Playlist(object obj) => CurrentView = prototype_view["Playlist"];
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
            videoLibraryVM.passToNavigation += setInfoFromVideo;

            MusicLibraryVM musicLibraryVM = (MusicLibraryVM)prototype_view["MusicLibrary"];
            musicLibraryVM.passToNavigationMusic += setInfoFromMusic;


            HomeCommand = new RelayCommand(Home);
            MusicLibraryCommand = new RelayCommand(MusicLibrary);
            VideoLibraryCommand = new RelayCommand(VideoLibrary);
            PlaylistCommand = new RelayCommand(Playlist);
            // Startup Page
            CurrentView = prototype_view["Home"];
        }

        public void setInfoFromVideo(Video video)
        {
            this.currentMediaName = video.name;
            this.mediaDuration = video.duration;
        }

        public void setInfoFromMusic(Music music)
        {
            this.currentMediaName = music.Name;
            this.mediaDuration = music.Length;
        }


    }
}
