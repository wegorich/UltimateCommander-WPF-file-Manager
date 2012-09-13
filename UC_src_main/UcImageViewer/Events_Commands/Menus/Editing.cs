using System.Windows;
using System.Windows.Controls;

namespace UcImageViewer
{
    public partial class ImageList
    {
        private void EditingItemClick(object sender, RoutedEventArgs e)
        {
            switch(((MenuItem)sender).Tag.ToString())
            {
                case "Grayscale":
                    ToGrayScale();
                    break;
                case "Inversion":
                    ToNegative();
                    break;
                case "Rotate180":
                    Rotate(RotateAngle.HundredEighty);
                    break;
                case "Rotate90":
                    Rotate(RotateAngle.Ninety);
                    break;
                case "Rotate-90":
                    Rotate(RotateAngle.MinusNinty);
                    break;
                case "FlipHorizontal":
                    ToHorizontalFlip();
                    break;
                case "FlipVertical":
                    ToVerticalFlip();
                    break;

            }
        }
    }
}