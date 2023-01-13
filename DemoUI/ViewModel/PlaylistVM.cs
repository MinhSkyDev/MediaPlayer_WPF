using DemoUI.Utilities;
using DemoUI.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TagLib.Ape;
using TagLib.Tiff;

namespace DemoUI.ViewModel
{
    class PlaylistVM: INotifyPropertyChanged
    {
        private NavigationVM navigation;

        public ICommand NewPlaylist { get; }

        public ICommand doubleClickMusic { get; set; }

        //Cặp event delegate dùng để pass dữ liệu qua màn hình chính, nơi mà data context là NavigationVM
        public delegate void passDataMusic(Model.Media data);
        public event passDataMusic passToNavigationMusic;

        public delegate void NavigateToPlayer();
        public event NavigateToPlayer navigateToPlayer;

        //Implement get set here to invoke "Selection Change Event"
        private object _selectedItemMusic;
        public object selectedItemMusic
        {
            get
            {
                return _selectedItemMusic;
            }
            set
            {
                // Cài đặt hàm set cho selectedItemMusic là vì selectedItemMusic được binding với item được chọn trong ListView, '
                // vì thế mỗi khi gọi hàm set là tương ứng với việc là item được chọn trong ListView vừa mới bị thay đổi
                if (_selectedItemMusic == value)
                    return;
                //Gán cho _selectedItemMusic hiện tại, ép kiểu về Model.Music để xử lý
                _selectedItemMusic = value;
                Model.Music currentMusic = (Model.Music)value;
                //title = currentMusic.Name;

                //Thay đổi xong thì truyền dữ liệu qua cho màn hình chính
                passToNavigationMusic?.Invoke(currentMusic);

            }
        }

        public string title { get; set; }

        private string path;

        public ObservableCollection<Model.Music> musics { get; set; }

        public ObservableCollection<Model.Playlist> _playlist { get; set; }
        public PlaylistVM(NavigationVM navigation)
        {

            title = "Music"; // ?
            //doubleClickMusic = new RelayCommand(doubleClickMusic_button);
            NewPlaylist = new RelayCommand(newPlaylist_button);

            _playlist = new ObservableCollection<Model.Playlist>();
            AllPlaylist();

            this.navigation = navigation;

        }

        void AllPlaylist()
        {
            path = System.AppDomain.CurrentDomain.BaseDirectory;

            int cutOff = path.LastIndexOf(@"\bin\Debug\");
            cutOff = cutOff < 0 ? path.Length : cutOff;
            path = path.Substring(0, cutOff) + @"\MyPlaylist";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] playlists = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

            foreach (string playlist in playlists)
            {
                DirectoryInfo folder = new DirectoryInfo(playlist);
                _playlist.Add(new Model.Playlist(folder));
                
                FileInfo[] items = folder.GetFiles("*");
                string numberOfitem = items.Length.ToString();
                string uri = folder.FullName;
                string name = folder.Name;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        //Hàm này dùng để bắt event doubleClick trên ListView Item, mục đích là để play music
        private void doubleClickMusic_button(object obj)
        {
            Model.Music currentMusic = (Model.Music)selectedItemMusic;
            //Chọn xong truyền dữ liệu qua màn hình chính trước
            passToNavigationMusic?.Invoke(currentMusic);
            //Rồi sau đó invoke để chuyển màn hình sang MediaPlayer
            navigateToPlayer?.Invoke();
        }

        private void newPlaylist_button(object obj)
        {
            var screen = new AddPlaylistWindow();     

            if (screen.ShowDialog() == true)
            {
                string playlistName = screen.Keyword;
                if (playlistName != null)
                {
                    string newPlaylist = path + @"\" + playlistName;
                    if (Directory.Exists(newPlaylist))
                    {
                        int count = 2;
                        while (Directory.Exists(newPlaylist + " " + count.ToString()))
                            count++;
                        newPlaylist += " " + count.ToString();
                        playlistName += " " + count.ToString();
                        
                    }
                    Directory.CreateDirectory(newPlaylist);
                    _playlist.Add(new Model.Playlist(playlistName));

                }
            }
            else
            {
                // Do nothing
            }
        }
    }
}
