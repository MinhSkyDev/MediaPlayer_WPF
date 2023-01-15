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
using System.Windows.Controls;
using System.Windows.Input;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DemoUI.ViewModel
{
    class VideoLibraryVM : INotifyPropertyChanged
    {
        private NavigationVM navigation;
        public ICommand addVideo { get; }
        public ICommand doubleClickVideo { get; set; }
        public ICommand shuffleList { get; set; }
        public ICommand searchButton { get; set; }
        public ICommand cancleButton { get; set; }

        //Cặp event delegate này dùng để pass dữ liệu qua màn hình chính, nơi mà data context là NavigationVM
        public delegate void passData(Model.Media data);
        public event passData passToNavigation;

        //Cặp delegate event dùng để pass và navigate sang màn hình player
        public delegate void NavigateToPlayer();
        public event NavigateToPlayer navigateToPlayer;


        //Implement get set here to invoke "Selection Change Event"
        private object _selectedItem;
        public object selectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                // Cài đặt hàm set cho selectedItem là vì selectedItem được binding với item được chọn trong ListView, '
                // vì thế mỗi khi gọi hàm set là tương ứng với việc là item được chọn trong ListView vừa mới bị thay đổi
                if (_selectedItem == value)
                    return;
                //Gán cho _selectedItem hiện tại, ép kiểu về Model.Video để xử lý
                _selectedItem = value;
                Model.Video currentVideo = (Model.Video)value;
                //title = currentVideo.name;

                //Thay đổi xong thì truyền dữ liệu qua cho màn hình chính
                passToNavigation?.Invoke(currentVideo);

            }
        }

        private int _selectedIndex;
        public int selectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                title = _selectedIndex.ToString();
            }
        }


        public string title { get; set; }

        public ObservableCollection<Model.Video> videos { get; set; }
        public ObservableCollection<Model.Video> temp { get; set; }
        public ObservableCollection<Model.Video> _subItems { get; set; }
        public VideoLibraryVM(NavigationVM navigation)
        {

            title = "Video";
            addVideo = new RelayCommand(addVideo_button);
            doubleClickVideo = new RelayCommand(doubleClickVideo_button);
            shuffleList = new RelayCommand(shuffleList_button);
            searchButton = new RelayCommand(getsearch);
            cancleButton = new RelayCommand(clearsearch);
            //selectVideo = new RelayCommand(selectVideo_button);
            videos = new ObservableCollection<Model.Video>();
            temp = new ObservableCollection<Model.Video>();
            this.navigation = navigation;

            // Default Video
            string myVideoPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            DirectoryInfo myVideoDirectory = new DirectoryInfo(myVideoPath);
            string extension = "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                          " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; ";
            FileInfo[] videoInfos = myVideoDirectory.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo videoInfo in videoInfos)
                if (extension.Contains(videoInfo.Extension))
                {
                    string video_name = videoInfo.Name;
                    Model.Video currentVideo = new Model.Video(videoInfo);
                    videos.Add(currentVideo);
                    temp.Add(currentVideo);
                    passToNavigation?.Invoke(currentVideo);
                }

        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string _keyword;
        public string Keyword
        {
            get { return _keyword; }
            set
            {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
            }
        }

        //Hàm này dùng để bắt event doubleClick trên ListView Item, mục đích là để play video
        private void doubleClickVideo_button(object obj)
        {
            Model.Video currentVideo = (Model.Video)selectedItem;
            //Chọn xong truyền dữ liệu qua màn hình chính trước
            passToNavigation?.Invoke(currentVideo);
            //Rồi sau đó invoke để chuyển màn hình sang MediaPlayer
            navigateToPlayer?.Invoke();



        }

        private void addVideo_button(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string video_path_uri = "";


            string VideoFormat = "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                          " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; ";
            openFileDialog.Filter = VideoFormat;

            if (openFileDialog.ShowDialog() == true)
            {
                video_path_uri = openFileDialog.FileName;
            }
            else
            {
                //do nothing
            }

            if (video_path_uri != "")
            {
                FileInfo videoInfo = new FileInfo(video_path_uri);
                string video_name = videoInfo.Name;
                Model.Video currentVideo = new Model.Video(videoInfo);

                //Sau khi add file thì cần bắn qua bên navigation vì hiện tại giao diện đang binding với NavigationVM
                videos.Add(currentVideo);
                temp.Add(currentVideo);
                selectedItem = videos[videos.Count() - 1];
                selectedIndex = videos.Count() - 1;
                passToNavigation?.Invoke(currentVideo);

            }
        }

        public Model.Media getNextMedia()
        {
            Model.Media result = null;

            int currentListSize = videos.Count;
            int nextIndex = selectedIndex + 1;
            if (nextIndex < currentListSize)
            {
                result = videos[nextIndex];
            }


            return result;
        }

        public Model.Media getPreviousMedia()
        {
            Model.Media result = null;

            int currentListSize = videos.Count;
            int previousIndex = selectedIndex - 1;
            if (previousIndex >= 0)
            {
                result = videos[previousIndex];
            }


            return result;
        }


        private void shuffleList_button(object obj)
        {
            videos.Shuffle();
        }
        private void getsearch(object obj)
        {
            _subItems = new ObservableCollection<Model.Video>(temp.Where(
               sv => sv.name.Contains(Keyword)
           ).ToList());
            videos = _subItems;
        }

        private void clearsearch(object obj)
        {
            Keyword = "";
            _subItems = new ObservableCollection<Model.Video>(temp.Where(x => x.name.Contains("")).ToList());
            videos = _subItems;
        }
    }
}