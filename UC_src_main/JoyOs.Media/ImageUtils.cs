using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JoyOs.Media
{
    public static class ImageUtils
    {
        #region Open Small Image Copies
        /// <summary>
        /// Открывает картинку и уменьшает до необходимого размера
        /// объект в памяти занимает меньше места
        /// </summary>
        /// <param name="fileName">string - полный путь к картинке</param>
        /// <param name="size">double необходимый размер</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage GetSmallImage(string fileName, int size = 0)
        {
            var bmpImg = new BitmapImage();

            bmpImg.BeginInit();

            bmpImg.CacheOption = BitmapCacheOption.OnLoad;
            bmpImg.UriSource = new Uri(fileName);
            
            if (size > 0)
                bmpImg.DecodePixelWidth = size;

            bmpImg.EndInit();

            return bmpImg;
        }
        #endregion

        #region  Шаблоны фабрики BitmapSource
        
        #region Save Image
        //TODO: убрать switch
        /// <summary>
        /// Сохраняет изображение в заданной кодировке
        /// </summary>
        /// <param name="source">Image</param>
        /// <param name="fullName">string</param>
        /// <param name="encType">string тип кодировки</param>
        public static void UcSaveImage(this BitmapSource source, string fullName, string encType = ".jpg")
        {
            switch (encType)
            {
                case ".png": WriteTransformedBitmapToFile<PngBitmapEncoder>(source, fullName); break;
                case ".gif": WriteTransformedBitmapToFile<GifBitmapEncoder>(source, fullName); break;
                case ".bmp": WriteTransformedBitmapToFile<BmpBitmapEncoder>(source, fullName); break;
                case ".wpd": WriteTransformedBitmapToFile<WmpBitmapEncoder>(source, fullName); break;
                case ".tif": WriteTransformedBitmapToFile<TiffBitmapEncoder>(source, fullName); break;

                default: WriteTransformedBitmapToFile<JpegBitmapEncoder>(source, fullName); break;
            }
        }

        #region Save to File

        /// <summary>
        /// Сохраняет изображение в файл с заданной кодировкой
        /// </summary>
        /// <typeparam name="T">задает кодировку</typeparam>
        /// <param name="bitmapSource">объект хранящий изображение</param>
        /// <param name="fileName">полный путь к новому объекту</param>
        /// <returns>void</returns>
        private static void WriteTransformedBitmapToFile<T>(BitmapSource bitmapSource, string fileName) where T : BitmapEncoder, new()
        {
            if (string.IsNullOrEmpty(fileName) || bitmapSource == null)
                return;

            var encoder = new T();
            //creating frame and putting it to Frames collection of selected encoder
            var frame = BitmapFrame.Create(bitmapSource);
            encoder.Frames.Add(frame);

                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    encoder.Save(fs);
                    fs.Close();
                }
        }
        #endregion

        #endregion
        
        #region Flip
        /// <summary>
        /// Отобразить по горизонтали
        /// </summary>
        /// <param name="img">исходное изображение</param>
        /// <returns>отраженное изображение</returns>
        public static BitmapSource UcFlipHorizontal(this BitmapSource img )
        {
            var bit = new BitmapWrapperKW(img);
            return bit.FilpHorizontal();
        }
        /// <summary>
        /// Отобразить по вертикали
        /// </summary>
        /// <param name="img">исходное изображение</param>
        /// <returns>отраженное изображение</returns>
        public static BitmapSource UcFlipVertical(this BitmapSource img)
        {
            var bit = new BitmapWrapperKW(img);
            return bit.FilpVertical();
        }
        #endregion

        #region Change Format in Image
        /// <summary>
        /// Изменяют формат изображения
        /// </summary>
        /// <param name="img">BitmapSource</param>
        /// <param name="file">string</param>
        /// <param name="pixel">PixelFormat</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource UcFormatChange(this BitmapSource img, string file, PixelFormat pixel)
        {
            /* //Create source using xaml defined resource.
             FormatConvertedBitmap fcb = new FormatConvertedBitmap(
               new BitmapImage(new Uri(file)), pixel, null, 0);
             //set image source*/
            var fcb = new FormatConvertedBitmap(
                                    img, pixel, null, 0);
            return fcb;
        }
        #endregion

        #region Rotate Image
        /// <summary>
        /// Округляет угл поворота
        /// </summary>
        /// <param name="x">int</param>
        /// <returns>uint</returns>
        public static uint MaxRotationAngle(int x)
        {
            x %= 360;
            return x >= 0 ? (uint)x : (uint)(360 + x);
        }

        /// <summary>
        /// Функция поворота на произвольный угол
        /// </summary>
        /// <param name="source">BitmapSource</param>
        /// <param name="centerPoint">Point</param>
        /// <param name="width">double</param>
        /// <param name="height">double</param>
        /// <param name="rotationAngle">double</param>
        /// <returns>BitmapSource</returns>
        private static BitmapSource GenerateCroppedImage(this BitmapSource source, Point centerPoint, double width, double height, double rotationAngle)
        {
            // TODO: get bounding box for the rotation, as the total image could be really huge

            double sourceDpiX = source.DpiX;
            double sourceDpiY = source.DpiY;

            const double wpfUnitsX = 96;
            const double wpfUnitsY = 96;

            double centerPointXInScreenUnits = centerPoint.X / sourceDpiX * wpfUnitsX;
            double centerPointYInScreenUnits = centerPoint.Y / sourceDpiY * wpfUnitsY;

            var targetWidth = (int)Math.Round(width, 0);
            var targetHeight = (int)Math.Round(height, 0);

            double targetWidthInScreenUnits = targetWidth / sourceDpiX * wpfUnitsX;
            double targetHeightInScreenUnits = targetHeight / sourceDpiY * wpfUnitsY;

            //double sourceWidthInScreenUnits = source.PixelWidth / sourceDpiX * wpfUnitsX;
            //double sourceHeightInScreenUnits = source.PixelHeight / sourceDpiY * wpfUnitsY;

            // rotate the master image around the point
            var rotateTransform = new RotateTransform
                                      {
                                          CenterX = centerPointXInScreenUnits,
                                          CenterY = centerPointYInScreenUnits,
                                          Angle = rotationAngle*-1
                                      };

            // move the point up to the top left
            var translateTransform = new TranslateTransform
                                         {
                                             X = -1*(centerPointXInScreenUnits - targetWidthInScreenUnits/2),
                                             Y = -1*(centerPointYInScreenUnits - targetHeightInScreenUnits/2)
                                         };

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(translateTransform);

            // create an image element to do all the manipulation. This is a cheap way of
            // getting around doing the math myself.
            var image = new Image
                              {
                                  Stretch = Stretch.None, Source = source, RenderTransform = transformGroup
                              };

            // more ui cruft. This is just for layout
            var container = new Canvas();
            container.Children.Add(image);
            container.Arrange(new Rect(0, 0, source.PixelWidth, source.PixelHeight));

            // render the result to a new bitmap. 
            var target = new RenderTargetBitmap(targetWidth, targetHeight, sourceDpiX, sourceDpiY, PixelFormats.Default);
            target.Render(container);

            return target;
        }
        /// <summary>
        /// Поворачивает изображение фиксированно!!!!
        /// </summary>
        /// <param name="img">BitmapSource</param>
        /// <param name="angle">angle</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource RotateImage(this BitmapSource img, double angle = 0)
        {
            if (Math.Abs(angle) > 0.01 && Math.Abs(angle%90) <0.01)
            {
                // Create the TransformedBitmap to use as the Image source.
                var tb = new TransformedBitmap(img, new RotateTransform(angle));
                // Set the Image source.
                return tb;
            }
            //корректируем размер будующего имэйджа
            double cos = Math.Abs(Math.Cos(angle * Math.PI / 180));
            double sin = Math.Abs(Math.Sin(angle * Math.PI / 180));

            double newWidth = img.PixelWidth * cos + img.PixelHeight * sin;
            double newHeight = img.PixelHeight * cos + img.PixelWidth * sin;
            //поворачиваем
            return GenerateCroppedImage(img, new Point(img.PixelWidth / 2, img.PixelHeight / 2),
                                        newWidth, newHeight, angle);
        }
        #endregion

        #region Chropped Bitmap обрезка изображения, не подключенно
        /// <summary>
        /// Обрезает изображение 
        /// </summary>
        /// <param name="source">изображение</param>
        /// <param name="rect">прямоугольник который будет вырезан</param>
        /// <returns>новое изображение</returns>
        public static BitmapSource UcChroppedBitmap(this BitmapSource source, Int32Rect rect)
        {
            return new CroppedBitmap(source, rect);
        }
        #endregion

        #region Resize Image не реализованно
        /// <summary>
        /// Изменяют размер изображения
        /// </summary>
        /// <param name="img">BitmapSource</param>
        /// <param name="size">на сколько изменить размер</param>
        /// <param name="center">относительно чего увеличиваем</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource ResizeImage(BitmapSource img, Size size,Point center)
        {
            var tb = new TransformedBitmap(img, new ScaleTransform(img.PixelWidth + size.Width, 
                                                                                                                                img.PixelHeight + size.Height, 
                                                                                                                                center.X,
                                                                                                                                center.Y));
            // Set the Image source.
            return tb;
        }
        #endregion

        #region Brightness
        /// <summary>
        /// Затемнение или осветление
        /// </summary>
        /// <param name="source">картинка</param>
        /// <param name="x">любое число</param>
        /// <returns>новое изображение</returns>
        public static BitmapSource UcBrightness(this BitmapSource source, int x)
        {
            var wrapper = new BitmapWrapperKW(source);
            return wrapper.ToBrightness(x);
        }
        #endregion

        #region Contrast
        /// <summary>
        /// Изменяет контрастность
        /// </summary>
        /// <param name="source">картинка</param>
        /// <param name="x">любое число</param>
        /// <returns>новое изображение</returns>
        public static BitmapSource UcContrast(this BitmapSource source, double x)
        {
            var wrapper = new BitmapWrapperKW(source);
            return wrapper.ToContrast(x);
        }
        #endregion

        #region Change Color
        /// <summary>
        /// Изменяет цвет пикселей на изображении в зависимости от цвета
        /// </summary>
        /// <param name="source">изображение</param>
        /// <param name="color">Цвет</param>
        /// <returns>новое изображение</returns>
        public static BitmapSource UcChangeColor(this BitmapSource source, Color color)
        {
            var wrapper = new BitmapWrapperKW(source);
            return wrapper.ToChangeColor(color);
        }
        #endregion

        #region GrayScale
        /// <summary>
        /// Делает изображение серого цвета
        /// </summary>
        /// <param name="source">изображение</param>
        /// <returns>новое изображение</returns>
        public static BitmapSource UcGrayScale(this BitmapSource source)
        {
            var wrapper = new BitmapWrapperKW(source);
            return wrapper.ToGrayscaleBitmapSource();
        }
        #endregion

        #region Negative
        /// <summary>
        /// Отображает цвета
        /// </summary>
        /// <param name="source">изображение</param>
        /// <returns>новое изображение</returns>
        public static BitmapSource UcNegative(this BitmapSource source)
        {
            var wrapper = new BitmapWrapperKW(source);
            return wrapper.ToNegative();
        }
        #endregion

        #region Glow не реализованно
        public static BitmapSource Glow(BitmapSource img, double hight)
        {

            // BitmapDecoder bit=new BitmapDecoder();

            return img;
        }

        #endregion

        #region Twisted Image
        /// <summary>
        /// Скручивает изображение вокруг центра
        /// </summary>
        /// <param name="bit">BitmapSource</param>
        /// <param name="slider">Slider</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource TwistedBitmap(this BitmapSource bit, Slider slider)
        {
            var bitmap = new WriteableBitmap(bit);

            // Get bitmap bits
            int stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            var pixelsSrc = new byte[bitmap.PixelHeight * stride];
            var pixelsNew = new byte[pixelsSrc.Length];
            bitmap.CopyPixels(pixelsSrc, stride, 0);

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int xCenter = width / 2;
            int yCenter = height / 2;
            int bytesPerPixel = bitmap.Format.BitsPerPixel / 8;

            for (int row = 0; row < bitmap.PixelHeight; row += 1)
            {
                for (int col = 0; col < bitmap.PixelWidth; col += 1)
                {
                    // Calculate length of point to center and angle from horizontal
                    int xDelta = col - xCenter;
                    int yDelta = row - yCenter;
                    double distanceToCenter = Math.Sqrt(xDelta * xDelta + yDelta * yDelta);
                    double angleClockwise = Math.Atan2(yDelta, xDelta);

                    // Calculate angle of rotation for twisting effect 
                    double xEllipse = xCenter * Math.Cos(angleClockwise);
                    double yEllipse = yCenter * Math.Sin(angleClockwise);
                    double radius = Math.Sqrt(xEllipse * xEllipse + yEllipse * yEllipse);
                    double fraction = Math.Max(0, 1 - distanceToCenter / radius);
                    double twist = fraction * Math.PI * slider.Value / 180;

                    // Calculate the source pixel for each destination pixel
                    var colSrc = (int)(xCenter + (col - xCenter) * Math.Cos(twist)
                                                - (row - yCenter) * Math.Sin(twist));
                    var rowSrc = (int)(yCenter + (col - xCenter) * Math.Sin(twist)
                                                + (row - yCenter) * Math.Cos(twist));
                    colSrc = Math.Max(0, Math.Min(width - 1, colSrc));
                    rowSrc = Math.Max(0, Math.Min(height - 1, rowSrc));

                    // Calculate the indices
                    int index = stride * row + bytesPerPixel * col;
                    int indexSrc = stride * rowSrc + bytesPerPixel * colSrc;

                    // Transfer the pixels
                    for (int i = 0; i < bytesPerPixel; i++)
                        pixelsNew[index + i] = pixelsSrc[indexSrc + i];
                }
            }
            // Write out the array
            bitmap.WritePixels(rect, pixelsNew, stride, 0);
            return bitmap;
        }
        #endregion

        #region ColorizeBitmap
/*
        private static Color Colorize(Color originalColor, Color color)
        {
            int strength = (originalColor.R + originalColor.G + originalColor.B);
            return
                Color.FromArgb(
                    originalColor.A,
                    Convert.ToByte(color.R * strength / 255 / 3),
                    Convert.ToByte(color.G * strength / 255 / 3),
                    Convert.ToByte(color.B * strength / 255 / 3));
        }
*/

        public static BitmapSource ColorizeBitmap(this BitmapSource bit, Color tintColor)
        {
            var bitmap = new WriteableBitmap(bit);

            //Get color
            int strength = (tintColor.R + tintColor.G + tintColor.B);

            // Get bitmap bits
            int stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            var pixelsSrc = new byte[bitmap.PixelHeight * stride];
            var pixelsNew = new byte[pixelsSrc.Length];
            bitmap.CopyPixels(pixelsSrc, stride, 0);

            for (int i = 0; i < stride * bitmap.PixelHeight; i++)
                pixelsNew[i] = Convert.ToByte(pixelsSrc[i] * strength/3/255);
            
            // Write out the array
            bitmap.WritePixels(rect, pixelsNew, stride, 0);
            return bitmap;
        }
        #endregion
        
        #endregion
    }
}
