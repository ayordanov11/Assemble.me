using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using Xceed.Wpf.AvalonDock.Converters;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace Assemble.me
{
    public class ListItems
    {
        private Dropbox.Api.Files.Metadata item;
        private string type;
        public ListItems(Metadata item, string type)
        {
            this.item = item;
            this.type = type;
        }


        public String Message
        {
            get { return this.item.Name; }
        }

        public BitmapSource Icon
        {
            get
            {
                BitmapSource a = LoadIcon(type);
                //IntPtr hbitmap = a.
                return a;
            }
        }

        /// <summary>
        /// Loads the icon for the provided item.
        /// </summary>
        /// <param name="item">The name of the icon.</param>
        /// <returns></returns>
        private BitmapImage LoadIcon(string item)
        {
            if (item == "folder")
                return new BitmapImage(new Uri("icos/folder.png", UriKind.Relative));
            else if (item == "file")
                return new BitmapImage(new Uri("icos/file.png", UriKind.Relative));
            //ShowCameraInfo="True" ShowCoordinateSystem="True" ShowFrameRate="True"
            else
                return null;
        }
    }
}
