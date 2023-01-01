using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoUI.View
{
    /// <summary>
    /// Interaction logic for VideoLibrary.xaml
    /// </summary>
    public partial class VideoLibrary : UserControl
    {
        public VideoLibrary()
        {
            InitializeComponent();
        }

        public void AddVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string video_path_uri = "";

            if (openFileDialog.ShowDialog() == true)
            {
                video_path_uri = openFileDialog.FileName;
            }
            else
            {
                //do nothing
            }

            FileInfo videoInfo = new FileInfo(video_path_uri);
            string video_name = videoInfo.Name;

        }

    }
}
