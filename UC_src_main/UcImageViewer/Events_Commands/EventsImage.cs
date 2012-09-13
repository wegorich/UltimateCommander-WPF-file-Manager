using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JoyOs.BusinessLogic.Utils;
using JoyOs.Media;

namespace UcImageViewer
{
    public partial class ImageList
    {
        #region Key Down

        public void OnImageListKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    InterfaceEventStart("buttonNew");
                    break;
                case Key.Right:
                    InterfaceEventStart("buttonMove");
                    break;
                case Key.Escape:
                    // OnButtonsClick(btnWImage[2], new RoutedEventArgs());
                    break;
                case Key.Enter:
                    //OnButtonsClick(btnWImage[2], new RoutedEventArgs());
                    break;
                case Key.Space:
                    InterfaceEventStart("buttonCopy");
                    break;
            }
        }

        #endregion

        #region Size Changed

        private void ImageListWrapPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (WrapPanel) sender;
            Size size = e.NewSize;
            var count = (int) (size.Width/(DefSize + 6));

            SizeImage = (size.Width - count*6)/count;

            for (int i = 0; i < panel.Children.Count; i++)
            {
                var img = (Image) panel.Children[i];
                img.MaxWidth = SizeImage;
                img.MaxHeight = SizeImage*0.8;
            }
        }

        #endregion

        #region On Image Click

        protected void OnImageWrapPanelClick(object sender, RoutedEventArgs e)
        {
            var img = (Image) sender;
            IndexOfCurImg = imgWrapList.Children.IndexOf(img);

            var inf = new FileInfo(img.Tag.ToString());
            DefaultEditingValues();

            bigImage.Source = ImageUtils.GetSmallImage(img.Tag.ToString());

            // TODO: Повторяется при клике по табам
            _tempBitmap = ((BitmapSource) bigImage.Source).Clone();

            imageDescription.Content = "Файл: " + inf.Name + ", "
                                       + _tempBitmap.PixelWidth + "x" + _tempBitmap.PixelHeight + ", размер файла: "
                                       + LengthFormatter.LengthFormat(inf.Length);
            zoomText.Text = "1";
        }

        #endregion

        #region Image Hover

        //анимация при наведении
        protected void OnStartHover(object sender, MouseEventArgs e)
        {
            var img = (Image) sender;

            img.Margin = new Thickness(0);
            img.MaxHeight += 6;
            img.MaxWidth += 6;
        }

        #endregion

        #region Image End Hover

        //анимация при окончании наведения
        protected void OnEndHover(object sender, MouseEventArgs e)
        {
            var img = (Image) sender;
            img.Margin = new Thickness(3);
            img.MaxHeight -= 6;
            img.MaxWidth -= 6;
        }

        #endregion;

        #region On Main StatusBar Buttons Click

        public void OnStatusButtonsClick(object sender, RoutedEventArgs e)
        {
            InterfaceEventStart(((Button) sender).Name);
        }

        #endregion

        #region UcVisual

        #region SelectionChanged

        /// <summary>
        /// Сохранение изображения во временное при смене TabItem
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void TabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bigImage != null)
                _tempBitmap = (BitmapSource) bigImage.Source.Clone();
        }

        #endregion

        #region Color change

        /// <summary>
        /// Изменение цвета изображения слайдерами
        /// </summary>
        /// <param name="sender">Слайдер красный или синий или зеленый</param>
        /// <param name="e">изменение</param>
        private void ColorSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider) sender;

            if (slider == redColorSlider)
                redColorText.Text = slider.Value.ToString();
            if (slider == greenColorSlider)
                greenColorText.Text = slider.Value.ToString();
            if (slider == blueColorSlider)
                blueColorText.Text = slider.Value.ToString();

            Color color = Color.FromArgb(
                255,
                (byte) redColorSlider.Value,
                (byte) greenColorSlider.Value,
                (byte) blueColorSlider.Value);

            colorRect.Fill = new SolidColorBrush(color);
            bigImage.Source = _tempBitmap.UcChangeColor(color);
        }

        #endregion

        #region Contrast

        private void CValValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            contrastColorText.Text = e.NewValue.ToString();
            bigImage.Source = _tempBitmap.UcContrast(e.NewValue);
        }

        #endregion

        #region Brightness

        private void BValValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            brigtnessColorText.Text = e.NewValue.ToString();
            bigImage.Source = _tempBitmap.UcBrightness((int) e.NewValue);
        }

        #endregion

        #region Rotation Tab

        /// <summary>
        /// Текст бокс который хранит угол поворота и поворачивает изображение при изменении угла
        /// </summary>
        /// <param name="sender">object TextBox</param>
        /// <param name="e">TextChangedEventArgs</param>
        private void AngleTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_tempBitmap != null)
            {
                var textBox = (TextBox) sender;
                uint d = 0;

                foreach (TextChange change in e.Changes)
                    if (!uint.TryParse(textBox.Text, out d))
                        textBox.Text = textBox.Text.Remove(change.Offset, change.AddedLength);

                if (d > 0)
                {
                    d = ImageUtils.MaxRotationAngle((int) d);
                    bigImage.Source = _tempBitmap.RotateImage(d);
                    angleSlider.Value = d;
                }
            }
        }

        /// <summary>
        /// Slider который регулирует угл поворота и передает изменения в текст бокс
        /// </summary>
        /// <param name="sender">angleSlider</param>
        /// <param name="e">RoutedPropertyChangedEventArgs</param>
        private void AngleSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            angleTextBox.Text = ((uint) angleSlider.Value).ToString();
        }

        #endregion

        #region Twisted Tab

        /// <summary>
        /// TextBox который хранит параметр скручивания изображения
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">TextChangedEventArgs</param>
        private void TwistedTextTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_tempBitmap != null)
            {
                var textBox = (TextBox) sender;
                int d = 0;

                if (_tempBitmap.Format.BitsPerPixel != 32 ||
                    _tempBitmap.Format.BitsPerPixel != 8)
                    _tempBitmap.UcFormatChange(null, PixelFormats.Pbgra32);

                foreach (TextChange change in e.Changes.Where(
                    change => int.TryParse(textBox.Text, out d) == false))
                {
                    textBox.Text = textBox.Text.Remove(change.Offset, change.AddedLength);
                }

                bigImage.Source = _tempBitmap.TwistedBitmap(twistedSlider);
                twistedSlider.Value = d;
            }
        }

        /// <summary>
        /// Слайдер искривляющий изображение
        /// </summary>
        /// <param name="sender">Slider</param>
        /// <param name="e">RoutedPropertyChangedEventArgs</param>
        private void TwistedSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            twistedText.Text = ((int) ((Slider) sender).Value).ToString();
        }

        #endregion

        #region Zoom Tab

        /// <summary>
        /// Функция увеличения изображения
        /// </summary>
        /// <remarks>ScaleTransform видимое но виртуальное увеличение 
        /// изображения (нельзя сохранить, только через RenderBitmap)</remarks>
        /// <param name="img">Image</param>
        /// <param name="delta">double</param>
        private void Zooming(Image img, double delta)
        {
            var gr = img.LayoutTransform as TransformGroup;
            if (gr != null)
            {
                var st = (ScaleTransform) gr.Children[0];
                st.ScaleX = st.ScaleY = delta/100;
            }
        }

        /// <summary>
        /// Slider который изменяет TextBox зумирования
        /// </summary>
        /// <param name="sender">Slider</param>
        /// <param name="e">RoutedPropertyChangedEventArgs</param>
        private void ZoomSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zoomText.Text = ((int) ((Slider) sender).Value).ToString();
        }

        /// <summary>
        /// Обрабатывает изменения текста и увеличивает/ уменьшает изображение
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">TextChangedEventArgs</param>
        private void ZoomTextTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox) sender;
            double d = 10;

            foreach (TextChange change in e.Changes.Where(
                change => double.TryParse(textBox.Text, out d) == false))
            {
                textBox.Text = textBox.Text.Remove(change.Offset, change.AddedLength);
            }

            Zooming(bigImage, d);
            zoomSlider.Value = d;
        }

        #endregion

        #endregion

        #region Image Status Bar Buttons Click

        /// <summary>
        /// Нажатие кнопок на статус баре
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">RoutedEventArgs</param>
        private void ImageStatusBarButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button) sender).Name)
            {
                case "rotatePlus":
                    Rotate(RotateAngle.Ninety);
                    break;
                case "rotateMinus":
                    Rotate(RotateAngle.MinusNinty);
                    break;
                case "makeBlackWhite":
                    ToNegative();
                    break;
                case "makeGray":
                    ToGrayScale();
                    break;
                case "flipHor":
                    ToHorizontalFlip();
                    break;
                case "flipVert":
                    ToVerticalFlip();
                    break;
                case "save":
                    SaveImage();
                    break;
            }
        }

        #endregion
    }
}