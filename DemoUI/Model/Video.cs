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
using System.Xml.Linq;

namespace DemoUI.Model
{

    // Đây là lớp để biểu thị thông tin của một video
    internal class Video : Media,INotifyPropertyChanged
    {
       
        public BitmapImage CoverPath { get; set; }
        public string singer { get; set; }

        


        public event PropertyChangedEventHandler PropertyChanged;

        public Video() { }

        public static BitmapImage ConvertBitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            // before encoding/decoding, check if bitmapSource is already a BitmapImage

            if (!(bitmapSource is BitmapImage bitmapImage))
            {
                bitmapImage = new BitmapImage();

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Save(memoryStream);
                    memoryStream.Position = 0;

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }
            }

            return bitmapImage;
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return ConvertBitmapSourceToBitmapImage(i);
        }

        public Video(FileInfo info) : base(info) {
            
            
            ShellFile shellFile = ShellFile.FromFilePath(uri);
            Bitmap bm = shellFile.Thumbnail.Bitmap;


            using (ShellObject shell = ShellObject.FromParsingName(uri))
            {
                // alternatively: shell.Properties.GetProperty("System.Media.Duration");
                IShellProperty prop = shell.Properties.System.Media.Duration;
                // Duration will be formatted as 00:44:08
                duration = prop.FormatForDisplay(PropertyDescriptionFormatOptions.None);
            }



            CoverPath = Bitmap2BitmapImage(bm);

        }



    }
}
