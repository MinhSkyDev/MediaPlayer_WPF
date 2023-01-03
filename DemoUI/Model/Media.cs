using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Matroska;

namespace DemoUI.Model
{
    internal class Media
    {
        public string uri { get; set; }
        public string name { get; set; }
        public string duration { get; set; }

        public Media()
        {

        }
        public Media(FileInfo info) {

            name = info.Name;
            string info_directory = info.Directory.FullName;
            uri = "";

            // Dấu "//", mục đích fix bug khi gọi từ ổ chính ( ổ C hoặc D)
            const char SLASH = (char)92;

            if (info_directory[info_directory.Length - 1].Equals(SLASH))
            {
                uri = info.Directory.FullName + name;
            }
            else
            {
                uri = info.Directory.FullName + @"\" + name;
            }

        }

        public virtual string getType()
        {
            return "";
        }


    }
}
