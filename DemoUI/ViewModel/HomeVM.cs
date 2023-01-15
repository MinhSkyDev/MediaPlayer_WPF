using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoUI.ViewModel
{
    class HomeVM 
    {

        public ObservableCollection<Model.Music> musics { get; set; }
        public ObservableCollection<Model.Video> videos { get; set; }

        public HomeVM()
        {
            
        }

    }
}
