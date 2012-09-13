using System.Windows;
using System.Windows.Controls;

namespace UcImageViewer
{
    public partial class ImageList
    {
        private void ImageItemClick(object sender, RoutedEventArgs e)
        {
            InterfaceEventStart(((MenuItem) sender).Tag.ToString());
        }
    }
}