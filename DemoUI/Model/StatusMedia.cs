using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DemoUI.Model
{
    internal class StatusMedia
    {
        public DateTimeOffset dateMedia { get; set; }
        public string typeMedia { get; set; }
        public string uriMedia { get; set; }
        public string nameMedia { get; set; }
        public string durationMedia { get; set; }
        public string singerMedia { get; set; }
        public string yearMedia { get; set; }
        public BitmapImage coverPathMedia { get; set; }

    }
}
