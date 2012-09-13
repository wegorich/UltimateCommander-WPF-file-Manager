using System;
using System.Windows;

namespace UcImageViewer
{
    public partial class ImageList
    {
        private void BigWindowStop(object sender, EventArgs e)
        {
            OnImageWrapPanelClick(imgWrapList.Children[IndexOfCurImg], 
                                                               new RoutedEventArgs());
        }
    }
}