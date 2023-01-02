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
    /// Interaction logic for MusicLibrary.xaml
    /// </summary>
    public partial class MusicLibrary : UserControl
    {
        public MusicLibrary()
        {
            InitializeComponent();
        }

        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string music_path_uri = "";

            if (openFileDialog.ShowDialog() == true)
            {
                music_path_uri = openFileDialog.FileName;
            }
            else
            {
                //do nothing
            }

            FileInfo musicInfo = new FileInfo(music_path_uri);
            string music_name = musicInfo.Name;
        }
    }
}
