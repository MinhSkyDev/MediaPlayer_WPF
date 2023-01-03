using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoUI.ViewModel
{
    class UserControlVM : INotifyPropertyChanged
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string uri { get; set; }

        public UserControlVM()
        {
            //Do nothing
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void setData(string name, string duration, string uri)
        {
            this.name = name;
            this.duration = duration;
            this.uri = uri;
        }



    }
}
