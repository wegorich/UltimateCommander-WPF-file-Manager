using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using PixelFormat = System.Windows.Media.PixelFormat;

namespace JoyOs.Media
{
    /// <summary>
    /// Расширение функционала битмапов методами конвертаций.
    /// </summary>
    public static class BitmapConvertedKW
    {
        /// <summary>
        /// Конвертация BitmapSource в другой формат BitmapSource.
        /// </summary>
        /// <param name="source">Источник для конвертации.</param>
        /// <param name="destinationFormat">Новый формат.</param>
        /// <param name="destinationPalette">
        /// Палитра для нового формата, если конечно она нужна для нового формата, иначе передать null.
        /// </param>
        /// <returns>BitmapSource в новом формате.</returns>
        public static BitmapSource ConvertTo(
            this BitmapSource source,
            PixelFormat destinationFormat,
            BitmapPalette destinationPalette)
        {
            return new FormatConvertedBitmap(
                source, destinationFormat, destinationPalette, 0);
        }

        /// <summary>
        /// Конвертация Bitmap в другой формат Bitmap.
        /// </summary>
        /// <param name="source">Источник для конвертации.</param>
        /// <param name="destinationFormat">Новый формат.</param>
        /// <param name="destinationPalette">
        /// Палитра для нового формата, если конечно она нужна для нового формата, иначе передать null.
        /// </param>
        /// <returns>Bitmap в новом формате.</returns>
        public static Bitmap ConvertTo(
            this Bitmap source,
            System.Drawing.Imaging.PixelFormat destinationFormat,
            ColorPalette destinationPalette)
        {
            var result =
                new Bitmap(source.Width, source.Height, destinationFormat);
            if (destinationPalette != null)
                result.Palette = destinationPalette;
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(source, 0, 0);
            return result;
        }

        /// <summary>
        /// Конвертация Bitmap в BitmapSource.
        /// </summary>
        public static BitmapSource ConvertTo(this Bitmap source)
        {
            return (new BitmapWrapperKW(source)).ToBitmapSource();
        }

        /// <summary>
        /// Конвертация BitmapSource в Bitmap.
        /// </summary>
        public static Bitmap ConvertTo(this BitmapSource source)
        {
            return (new BitmapWrapperKW(source)).ToBitmap();
        }
    }

    /// <summary>
    /// Класс позволяет производить шустро попиксельные операции с битмапами.
    /// При этом результат операций возвращается в виде нового битмапа.
    /// </summary>
    [Serializable]
    public class BitmapWrapperKW
    {
        #region Внутренние поля и методы.

        private readonly double _dpiX;
        private readonly double _dpiY;
        private readonly int _height;
        private readonly byte[] _pixels;
        private readonly int _width;

        private BitmapSource ToGrayscaleTransparentBitmapSource()
        {
            var pixels = new byte[_width*_height*4];
            int offset = 0;
            int offsetGray = 0;
            int cnt = _height*_width;
            for (int i = 0; i < cnt; i++)
            {
                byte blue = _pixels[offset++];
                byte green = _pixels[offset++];
                byte red = _pixels[offset++];
                byte alfa = _pixels[offset++];

                var gray = (byte) (0.299*red + 0.587*green + 0.114*blue);

                pixels[offsetGray++] = gray;
                pixels[offsetGray++] = gray;
                pixels[offsetGray++] = gray;
                pixels[offsetGray++] = alfa;
            }

            return BitmapSource.Create(
                _width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, pixels, _width*4);
        }

        private Bitmap ToTransparentBitmap()
        {
            var bm = new Bitmap(
                _width, _height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            BitmapData bmData = bm.LockBits(
                new Rectangle(0, 0, _width, _height),
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            IntPtr scan = bmData.Scan0;

            //TODO: сделать без unsafe
            unsafe
            {
                var p = (byte*) (void*) scan;
                int offset = 0;
                int cnt = _height*_width;
                for (int i = 0; i < cnt; i++)
                {
                    p[offset++] = _pixels[offset]; //blue
                    p[offset++] = _pixels[offset]; //green
                    p[offset++] = _pixels[offset]; //red
                    p[offset++] = _pixels[offset]; //alfa
                }
            }

            bm.UnlockBits(bmData);

            return bm;
        }

        #endregion

        /// <summary>
        /// Конструктор копирует пиксели из заданного битмапа.
        /// </summary>
        /// <param name="source">Исходный битмап.</param>
        public BitmapWrapperKW(BitmapSource source)
        {
            _dpiX = source.DpiX;
            _dpiY = source.DpiY;
            _height = source.PixelHeight;
            _width = source.PixelWidth;

            if (source.Format != PixelFormats.Bgra32)
                source = source.ConvertTo(PixelFormats.Bgra32, null);

            int stride = _width*4;
            _pixels = new byte[stride*_height];
            source.CopyPixels(_pixels, stride, 0);
        }

        /// <summary>
        /// Конструктор копирует пиксели из заданного битмапа.
        /// </summary>
        /// <param name="source">Исходный битмап.</param>
        public BitmapWrapperKW(Bitmap source)
        {
            _dpiX = source.HorizontalResolution;
            _dpiY = source.VerticalResolution;
            _height = source.Height;
            _width = source.Width;

            System.Drawing.Imaging.PixelFormat pf = source.PixelFormat;

            if (pf != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                source = source.ConvertTo(System.Drawing.Imaging.PixelFormat.Format32bppArgb, null);

            _pixels = new byte[_width*_height*4];

            BitmapData bmData = source.LockBits(
                new Rectangle(0, 0, _width, _height),
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            int strideBm = bmData.Stride;
            IntPtr scan = bmData.Scan0;

            //TODO: сделать без unsafe
            unsafe
            {
                var p = (byte*) (void*) scan;
                int offset = 0;
                for (int j = 0; j < _height; j++)
                {
                    int offsetBm = strideBm*j;
                    for (int i = 0; i < _width; i++)
                    {
                        _pixels[offset++] = p[offsetBm++]; //blue
                        _pixels[offset++] = p[offsetBm++]; //green
                        _pixels[offset++] = p[offsetBm++]; //red
                        _pixels[offset++] = p[offsetBm++]; //alfa
                    }
                }
            }

            source.UnlockBits(bmData);
        }

        /// <summary>
        /// Конструктор загружает битмап из файла типа:
        /// BMP, GIF, EXIG, JPG, PNG and TIFF.
        /// </summary>
        /// <param name="fileName">Полное имя файла.</param>
        public BitmapWrapperKW(string fileName)
            : this(new Bitmap(fileName))
        {
        }

        /// <summary>
        /// Ширина в пикселях.
        /// </summary>
        public int PixelWidth
        {
            get { return _width; }
        }

        /// <summary>
        /// Высота в пикселях.
        /// </summary>
        public int PixelHeight
        {
            get { return _height; }
        }

        /// <summary>
        /// Прямой доступ к пикселям в формате sRGB 32 бита на пиксел.
        /// Каждый канал по 8 бит. Порядок следования: blue, green, red, alpha.
        /// </summary>
        /// <example>
        /// int offset = (y * PixelWidth + x) * 4;
        /// byte blue = Pixels[offset++]
        /// byte green = Pixels[offset++]
        /// byte red = Pixels[offset++]
        /// byte alfa = Pixels[offset]
        /// </example>
        public byte[] Pixels
        {
            get { return _pixels; }
        }

        /// <summary>
        /// Пиксели
        /// </summary>
        /// <param name="x">Номер колонки пикселей</param>
        /// <param name="y">Номер строки пикселей</param>
        /// <returns>Цвет пикселя.</returns>
        public Color this[int x, int y]
        {
            get
            {
                int offset = (y*_width + x)*4;
                byte blue = _pixels[offset++];
                byte green = _pixels[offset++];
                byte red = _pixels[offset++];
                byte alfa = _pixels[offset];
                return Color.FromArgb(alfa, red, green, blue);
            }

            set
            {
                int offset = (y*_width + x)*4;

                _pixels[offset++] = value.B;
                _pixels[offset++] = value.G;
                _pixels[offset++] = value.R;
                _pixels[offset] = value.A;
            }
        }

        public BitmapSource ToBitmapSource()
        {
            return BitmapSource.Create(
                _width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, _pixels, _width*4);
        }

        public BitmapSource FilpHorizontal()
        {
            var bitmap = ToBitmap();
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return bitmap.ConvertTo();
        }

        public BitmapSource FilpVertical()
        {
            var bitmap = ToBitmap();
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap.ConvertTo();
        }

        /// <summary>
        /// Осветляет или затемняет
        /// </summary>
        /// <param name="x">любое число</param>
        /// <returns>Новое изображение</returns>
        public BitmapSource ToBrightness(int x)
        {
            var pixels = new byte[_width * _height * 4];
            int offset = 0;
            int offsetGray = 0;
            int cnt = _height * _width;
            for (int i = 0; i < cnt; i++)
            {
                var blue = _pixels[offset++];
                var green = _pixels[offset++];
                 var   red = _pixels[offset++];

                int tp = blue + x;
                blue = (byte)((tp < 256) ? (tp < 0) ? 0 : tp : 255);

                tp = green + x;
                green = (byte)((tp < 256) ? (tp < 0) ? 0 : tp : 255);

                tp = red + x;
                red = (byte)((tp < 256) ? (tp < 0) ? 0 : tp : 255);

                pixels[offsetGray++] = blue;
                pixels[offsetGray++] = green;
                pixels[offsetGray++] = red;
                pixels[offsetGray++] = _pixels[offset++];

            }

            return BitmapSource.Create(
                _width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, pixels, _width * 4);
        }

        /// <summary>
        /// Изменяет контрастность
        /// </summary>
        /// <param name="x">любое число</param>
        /// <returns>Новое изображение</returns>
        public BitmapSource ToContrast(double x)
        {
            var pixelsGray = new byte[_width * _height * 4];
            int offset = 0;
            int offsetGray = 0;
            int cnt = _height * _width;
            for (int i = 0; i < cnt; i++)
            {
                var blue = _pixels[offset++];
                var tp = blue * x;
                blue = (byte)((tp < 256) ? (tp < 0) ? 0 : tp : 255);

                var green = _pixels[offset++];
                tp = green * x;
                green = (byte)((tp < 256) ? (tp < 0) ? 0 : tp : 255);

                var red = _pixels[offset++];
                tp = red * x;
                red = (byte)((tp < 256) ? (tp < 0) ? 0 : tp : 255);

                byte alfa = _pixels[offset++];

                pixelsGray[offsetGray++] = blue;
                pixelsGray[offsetGray++] = green;
                pixelsGray[offsetGray++] = red;
                pixelsGray[offsetGray++] = alfa;
            }

            return BitmapSource.Create(
                _width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, pixelsGray, _width * 4);
        }

        /// <summary>
        /// Просто меняет цвет к определенному цвету
        /// </summary>
        /// <param name="color">цвет к которому стремимся</param>
        /// <returns>новое изображение</returns>
        public BitmapSource ToChangeColor(Color color)
        {
            var pixelsGray = new byte[_width * _height * 4];
            int offset = 0;
            int offsetGray = 0;
            int cnt = _height * _width;
            for (int i = 0; i < cnt; i++)
            {
                var blue = (byte)(_pixels[offset] + color.B > 255 ?255 : _pixels[offset] + color.B);
                offset++;
                var green = (byte)(_pixels[offset] + color.G > 255 ? 255 : _pixels[offset] + color.G);
                offset++;
                var red = (byte)(_pixels[offset] + color.R > 255 ? 255 : _pixels[offset] + color.R);
                offset++;
                byte alfa = _pixels[offset++];

                pixelsGray[offsetGray++] = blue;
                pixelsGray[offsetGray++] = green;
                pixelsGray[offsetGray++] = red;
                pixelsGray[offsetGray++] = alfa;
            }

            return BitmapSource.Create(
                _width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, pixelsGray, _width * 4);

            
        }
        /// <summary>
        /// Отображает цвет
        /// </summary>
        /// <returns>новое изображение</returns>
        public BitmapSource ToNegative()
        {
            var pixelsGray = new byte[_width * _height * 4];
            int offset = 0;
            int offsetGray = 0;
            int cnt = _height * _width;
            for (int i = 0; i < cnt; i++)
            {
                var blue = (byte)(255-_pixels[offset++]);
                var green = (byte)(255 - _pixels[offset++]); 
                var red = (byte)(255 - _pixels[offset++]);
                byte alfa = _pixels[offset++];

                pixelsGray[offsetGray++] = blue;
                pixelsGray[offsetGray++] = green;
                pixelsGray[offsetGray++] = red;
                pixelsGray[offsetGray++] = alfa;
            }

            return BitmapSource.Create(
                _width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, pixelsGray, _width * 4);

        }
        /// <summary>
        /// Получить копию изображения в оттенках серого цвета.
        /// </summary>
        public BitmapSource ToGrayscaleBitmapSource()
        {
            // Проверка на прозрачность, если есть прозрачные пиксели то обработать особым образом.
            int offset = 3;
            int cnt = _width*_height;
            for (int i = 0; i < cnt; i++)
            {
                if (_pixels[offset] != 255)
                    return ToGrayscaleTransparentBitmapSource();
                offset += 4;
            }

            var pixelsGray = new ushort[_width*_height];
            offset = 0;
            int offsetGray = 0;
            for (int i = 0; i < cnt; i++)
            {
                byte blue = _pixels[offset++];
                byte green = _pixels[offset++];
                byte red = _pixels[offset];
                offset += 2;
                var gray = (ushort) ((65535*0.299/255)*red + (65535*0.587/255)*green + (65535*0.114/255)*blue);
                pixelsGray[offsetGray++] = gray;
            }

            return BitmapSource.Create(
                _width, _height, _dpiX, _dpiY, PixelFormats.Gray16, null, pixelsGray, 2*_width);
        }

        public Bitmap ToBitmap()
        {
            // Проверка на прозрачность, если есть прозрачные пиксели то обработать особым образом.
            int offset = 3;
            int cnt = _width*_height;
            for (int i = 0; i < cnt; i++)
            {
                if (_pixels[offset] != 255)
                    return ToTransparentBitmap();
                offset += 4;
            }

            var bm = new Bitmap(
                _width, _height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );

            BitmapData bmData = bm.LockBits(
                new Rectangle(0, 0, _width, _height),
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );

            int strideBm = bmData.Stride;
            IntPtr scan = bmData.Scan0;

            //TODO: сделать без unsafe
            unsafe
            {
                var p = (byte*) (void*) scan;
                offset = 0;
                for (int j = 0; j < _height; j++)
                {
                    int offsetBm = strideBm*j;
                    for (int i = 0; i < _width; i++)
                    {
                        p[offsetBm++] = _pixels[offset++]; //blue
                        p[offsetBm++] = _pixels[offset++]; //green
                        p[offsetBm++] = _pixels[offset]; //red
                        offset += 2;
                    }
                }
            }

            bm.UnlockBits(bmData);

            return bm;
        }

        public void SaveToFileBmp(string fileName)
        {
            ToBitmap().Save(fileName);
        }
    }
}