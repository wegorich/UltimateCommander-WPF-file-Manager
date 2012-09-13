using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UcImageViewer
{
    public partial class ImageList
    {
        private void ImageMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = (Image)sender;
            img.ReleaseMouseCapture();
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            var img = (Image)sender;
            if (!img.IsMouseCaptured) return;
            Vector v = _startImgMove - e.GetPosition(viewImg);
            viewImg.ScrollToHorizontalOffset(_originImgMove.X - v.X);
            viewImg.ScrollToVerticalOffset(_originImgMove.Y - v.Y);
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = (Image)sender;
            img.CaptureMouse();
            _startImgMove = e.GetPosition(viewImg);
            _originImgMove = new Point(viewImg.HorizontalOffset, viewImg.VerticalOffset);
        }
    }
}