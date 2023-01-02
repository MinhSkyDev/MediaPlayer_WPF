using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace DemoUI.Model
{
    internal class Music : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string uri { get; set; }
        public string Singer { get; set; }
        public string Year { get; set; }
        public string Length { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public Music() { }

        public Music(FileInfo info)
        {
            Name = info.Name;
            uri = info.Directory.FullName + @"\" + Name;

            using (ShellObject shell = ShellObject.FromParsingName(uri))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                // Duration will be formatted as 00:44:08
                Length = prop.FormatForDisplay(PropertyDescriptionFormatOptions.None);
            }

            var fileTag = TagLib.File.Create(info.FullName);
            Singer = fileTag.Tag.FirstPerformer;

            if (Singer == null)
            {
                Singer = "Unknown";
            }

            var year = fileTag.Tag.Year;

            if (year == 0)
            {
                Year = "Unknown";
            }
            else
            {
                Year = year.ToString();
            }

        }
    }
}
