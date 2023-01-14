using DemoUI.Model;
using DemoUI.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DemoUI.ViewModel
{
    class MusicLibraryVM : INotifyPropertyChanged
    {
        private NavigationVM navigation;
        public ICommand addMusic { get; }

        public ICommand doubleClickMusic { get; set; }
        public ICommand selectMusic { get; }
        public ICommand shuffleList { get; set; }
        public ICommand searchButton { get; set; }
        public ICommand cancleButton { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //Cặp event delegate dùng để pass dữ liệu qua màn hình chính, nơi mà data context là NavigationVM
        public delegate void passDataMusic(Model.Media data);
        public event passDataMusic passToNavigationMusic;

        public delegate void NavigateToPlayer();
        public event NavigateToPlayer navigateToPlayer;


        public int selectedIndex { get; set; }



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
        public string _keyword;
        public string Keyword
        {
            get { return _keyword; }
            set { 
                _keyword= value;
                OnPropertyChanged(nameof(Keyword));
            }
        }

        public ObservableCollection<Model.Music> musics { get; set; }
        public ObservableCollection<Model.Music> temp { get; set; }
        public ObservableCollection<Model.Music> _subItems { get; set; }
        public MusicLibraryVM(NavigationVM navigation)
        {

            title = "Music";
            addMusic = new RelayCommand(addMusic_button);
            doubleClickMusic = new RelayCommand(doubleClickMusic_button);
            shuffleList = new RelayCommand(shuffleList_button);
            searchButton = new RelayCommand(getsearch);
            cancleButton = new RelayCommand(clearsearch);
            musics = new ObservableCollection<Model.Music>();
            temp = new ObservableCollection<Model.Music>();
            this.navigation = navigation;

        }



        //Hàm này dùng để bắt event doubleClick trên ListView Item, mục đích là để play music
        private void doubleClickMusic_button(object obj)
        {
            Model.Music currentMusic = (Model.Music)selectedItemMusic;
            //Chọn xong truyền dữ liệu qua màn hình chính trước
            passToNavigationMusic?.Invoke(currentMusic);
            //Rồi sau đó invoke để chuyển màn hình sang MediaPlayer
            navigateToPlayer?.Invoke();
        }

        private void addMusic_button(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string music_path_uri = "";


            string MusicFormat = "Music files |*.mp3; *.wma; *.wav; *.flac; *.aac; *.ogg; *.aiff;  *.alac;";
            openFileDialog.Filter = MusicFormat;

            if (openFileDialog.ShowDialog() == true)
            {
                music_path_uri = openFileDialog.FileName;
            }
            else
            {
                //do nothing
            }

            if (music_path_uri != "")
            {
                FileInfo musicInfo = new FileInfo(music_path_uri);
                string music_name = musicInfo.Name;
                Model.Music currentMusic = new Model.Music(musicInfo);

                //Sau khi add song thì chuyển qua navigation vì hiện tại giao diện đang binding với NavigationVM
                musics.Add(currentMusic);
                temp.Add(currentMusic);
                passToNavigationMusic?.Invoke(currentMusic);
            }
        }

        public Model.Media getNextMedia()
        {
            Model.Media result = null;

            int currentListSize = musics.Count;
            int nextIndex = selectedIndex + 1;
            if (nextIndex < currentListSize)
            {
                result = musics[nextIndex];
            }


            return result;
        }

        public Model.Media getPreviousMedia()
        {
            Model.Media result = null;

            int currentListSize = musics.Count;
            int previousIndex = selectedIndex - 1;
            if (previousIndex >= 0)
            {
                result = musics[previousIndex];
            }


            return result;
        }

        private void shuffleList_button(object obj)
        {
            musics.Shuffle();
        }

        private void getsearch(object obj)
        {
            _subItems = new ObservableCollection<Model.Music>(temp.Where(
               sv => sv.name.Contains(Keyword)
           ).ToList());

            musics = _subItems;
        }

        private void clearsearch(object obj)
        {
            Keyword = "";
            _subItems = new ObservableCollection<Model.Music>(temp.Where(
               sv => sv.name.Contains("")
           ).ToList());
            musics = _subItems;

        }

        // Xử lý khi được xem là playlist
        public void newPlaylist(string path, string title)
        {
            this.title = title;
            DirectoryInfo playlist = new DirectoryInfo(path + @"\" + title);
            FileInfo[] items = playlist.GetFiles("*");
            musics.Clear();
            foreach (FileInfo item in items)
            {
                string music_name = item.Name;
                Model.Music currentMusic = new Model.Music(item);

                //Sau khi add song thì chuyển qua navigation vì hiện tại giao diện đang binding với NavigationVM
                musics.Add(currentMusic);
                passToNavigationMusic?.Invoke(currentMusic);
            }

        }
    }


    
}
