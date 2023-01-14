using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoUI.Model
{
    internal class Playlist
    {
        public string name { get; set; }
        public string numberOfitem { get; set; }

        public Playlist()
        {
            this.name = "Unknown";
            this.numberOfitem = "0 Items";
        }

        public Playlist(string name)
        {
            this.name = name;
            this.numberOfitem = "0 Items";
        }

        public Playlist(DirectoryInfo folder)
        {
            FileInfo[] items = folder.GetFiles("*");
            this.name = folder.Name;
            this.numberOfitem = items.Length.ToString() + " Items";
        }
    }
}
