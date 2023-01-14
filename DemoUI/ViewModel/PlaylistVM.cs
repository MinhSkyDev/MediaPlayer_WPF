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

        public ICommand doubleClickPlaylist { get; set; }

        //Cặp event delegate dùng để pass dữ liệu qua màn hình chính, nơi mà data context là NavigationVM
        public delegate void passPathPlaylist(string path, string title);
        public event passPathPlaylist passToNavigationPath;

        public delegate void NavigateToMusic();
        public event NavigateToMusic navigateToMusic;

        //Implement get set here to invoke "Selection Change Event"
        private object _selectedItemPlaylist;
        public object selectedItemPlaylist
        {
            get
            {
                return _selectedItemPlaylist;
            }
            set
            {
                // Cài đặt hàm set cho selectedItemMusic là vì selectedItemMusic được binding với item được chọn trong ListView, '
                // vì thế mỗi khi gọi hàm set là tương ứng với việc là item được chọn trong ListView vừa mới bị thay đổi
                if (_selectedItemPlaylist == value)
                    return;
                
                _selectedItemPlaylist = value;
            }
        }

        public string title { get; set; }

        private string path;

        public ObservableCollection<Model.Music> musics { get; set; }

        public ObservableCollection<Model.Playlist> _playlist { get; set; }
        public PlaylistVM(NavigationVM navigation)
        {
            doubleClickPlaylist = new RelayCommand(doubleClickPlaylist_button);
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
                
                //FileInfo[] items = folder.GetFiles("*");
                //string numberOfitem = items.Length.ToString();
                //string uri = folder.FullName;
                //string name = folder.Name;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        //Hàm này dùng để bắt event doubleClick trên ListView Item, mục đích là để play music
        private void doubleClickPlaylist_button(object obj)
        {
            Model.Playlist currentPlaylist = (Model.Playlist)selectedItemPlaylist;
            title = currentPlaylist.name;
            //truyền dữ liệu qua màn hình chính
            passToNavigationPath?.Invoke(path, title);
            //Rồi sau đó invoke để chuyển màn hình sang Music
            navigateToMusic?.Invoke();
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
