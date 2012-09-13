using System.IO;
using System.Windows.Media.Imaging;
using JoyOs.Media;

namespace UcImageViewer
{
    public partial class ImageList
    {
        enum RotateAngle
        {
            Zero=0,
            Ninety = 90,
            MinusNinty = -90,
            HundredEighty = 180,
            MinusHundredEighty = -180
        }

        private void Rotate(RotateAngle angle)
        {
            uint uInt;
            uint.TryParse(angleTextBox.Text, out uInt);
            angleTextBox.Text = (ImageUtils.MaxRotationAngle((int)angle + (int)uInt)).ToString();
        }

        private void SaveImage()
        {
            var file = bigImage.Tag.ToString();
            ((BitmapSource)bigImage.Source).UcSaveImage(file, Path.GetExtension(file));
        }

        private void ToGrayScale()
        {
            bigImage.Source = ImageUtils.UcGrayScale(_tempBitmap);
        }

        private void ToNegative()
        {
            bigImage.Source = ImageUtils.UcNegative(_tempBitmap);
        }

        private void ToHorizontalFlip()
        {
            bigImage.Source = ImageUtils.UcFlipHorizontal(_tempBitmap);
        }

        private void ToVerticalFlip()
        {
            bigImage.Source = ImageUtils.UcFlipVertical(_tempBitmap);
        }

        //TODO: убрать слайдеры хотя бы
        private void DefaultEditingValues()
        {
            greenColorText.Text = "0";
            redColorText.Text = "0";
            blueColorText.Text = "0";
            twistedText.Text = "0";
            angleTextBox.Text = "0";
            zoomText.Text = "10";
            brigtnessColorText.Text = "0";
            contrastColorText.Text = "0";
            blueColorSlider.Value = 0;
            redColorSlider.Value = 0;
            greenColorSlider.Value = 0;
            britnessColorSlider.Value = 0;
            contrastColorSlider.Value = 0;
        }
    }
}
