using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace DemoUI.View
{
    /// <summary>
    /// Interaction logic for AddPlaylistWindow.xaml
    /// </summary>
    public partial class AddPlaylistWindow : Window
    {

        public AddPlaylistWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Keyword { get; set; } = "";

        private void textbox(object sender, TextChangedEventArgs e)
        {
            if (Keyword == "")
                searchBox.Opacity = 0.5;
            else
                searchBox.Opacity = 1;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
