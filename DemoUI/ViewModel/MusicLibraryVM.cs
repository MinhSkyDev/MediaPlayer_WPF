using DemoUI.Model;
using DemoUI.Utilities;
using DemoUI.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DemoUI.ViewModel
{
    class MusicLibraryVM : INotifyPropertyChanged
    {
        private NavigationVM navigation;
        public ICommand addMusic { get; }
        public ICommand addPlaylist { get; set; }
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
        private string path = "";
        private List<string> playlistItems = new List<string>();
        public string addToPlaylist { get; set; }
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
            addPlaylist = new RelayCommand(add_playlist);
            doubleClickMusic = new RelayCommand(doubleClickMusic_button);
            shuffleList = new RelayCommand(shuffleList_button);
            searchButton = new RelayCommand(getsearch);
            cancleButton = new RelayCommand(clearsearch);
            musics = new ObservableCollection<Model.Music>();
            temp = new ObservableCollection<Model.Music>();
            this.navigation = navigation;


            // Default Music
            string myMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            DirectoryInfo myMusicDirectory = new DirectoryInfo(myMusicPath);
            string extension = ".mp3, .wma, .wav, .flac, .aac, .ogg, .aiff, .alac";
            FileInfo[] musicInfos = myMusicDirectory.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo musicInfo in musicInfos)
                if (extension.Contains(musicInfo.Extension))
                {
                    string music_name = musicInfo.Name;
                    Model.Music currentMusic = new Model.Music(musicInfo);
                    musics.Add(currentMusic);
                    temp.Add(currentMusic);
                    passToNavigationMusic?.Invoke(currentMusic);
                }
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

        private void add_playlist(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            string format = "Video file | *.*";
            openFileDialog.Filter = format;

            if (openFileDialog.ShowDialog() == true)
            {
                // Do nothing
            }
            else
            {
                //do nothing
            }
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

                // Nếu đang trong playlist
                if (this.path != "")
                {
                    if (!playlistItems.Contains(music_path_uri))
                    {
                        playlistItems.Add(music_path_uri);
                        FileInfo playlist = new FileInfo(this.path);
                        File.AppendAllText(this.path, music_path_uri + Environment.NewLine);
                    }
                }

                //Sau khi add song thì kiểm tra nó đã tồn tại chưa
                
                bool isExists = false;
                foreach (Model.Music music in musics)
                {
                    if (music.name == music_name)
                    {
                        isExists = true;
                        break;
                    }
                }

                //Sau đó chuyển qua navigation vì hiện tại giao diện đang binding với NavigationVM
                if (!isExists)
                {
                    musics.Add(currentMusic);
                    temp.Add(currentMusic);
                    passToNavigationMusic?.Invoke(currentMusic);
                }
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
            this.path = path + @"\" + title + ".txt";
            //DirectoryInfo playlist = new DirectoryInfo(path + @"\" + title);
            FileInfo playlist = new FileInfo(this.path);
            musics.Clear();
            if (title != "Recent Music")
            {
                using (StreamReader sr = playlist.OpenText())
                {
                    string item = "";
                    while ((item = sr.ReadLine()) != null)
                    {
                        this.playlistItems.Add(item);
                        FileInfo music = new FileInfo(item);
                        Model.Music currentMusic = new Model.Music(music);
                        //Sau khi add song thì chuyển qua navigation vì hiện tại giao diện đang binding với NavigationVM
                        musics.Add(currentMusic);
                        passToNavigationMusic?.Invoke(currentMusic);
                    }
                }
            }
            else
            {
                string fileName = "statusPlayingMedia.json";
                string jsonStringReading = File.ReadAllText(fileName);

                if (jsonStringReading != "")
                {
                    StatusMedia recentMedia = JsonSerializer.Deserialize<StatusMedia>(jsonStringReading);

                    string music_path_uri = recentMedia.uriMedia;
                    if (music_path_uri != "" && recentMedia.typeMedia == "MusicLibrary")
                    {
                        FileInfo musicInfo = new FileInfo(music_path_uri);
                        string music_name = musicInfo.Name;
                        Model.Music currentMusic = new Model.Music(musicInfo);

                        //Sau khi add song thì chuyển qua navigation vì hiện tại giao diện đang binding với NavigationVM
                        musics.Add(currentMusic);
                        passToNavigationMusic?.Invoke(currentMusic);
                    }
                }
            }
        }
    }


    
}
