using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UcMediaViewer
{
    public partial class MediaViewer
    {
        /// <summary>
        /// Картинка по умолчанию
        /// </summary>
        public static Brush DefImage = new ImageBrush(new BitmapImage(
                                                          new Uri(
                                                              "pack://application:,,,/JoyOs.Media;component/img/Plugins/Media/MEDIA.ico",
                                                              UriKind.RelativeOrAbsolute)));

        /// <summary>
        /// Начальная директория, нужно задавать до создания объекта
        /// </summary>
        public static string UvStartDirectory { get; set; }

        #region Load Video Image

        protected void LoadImages(string folder)
        {
           const string supportedExtensions = "*.mkv,*.mpg,*.avi,*.asf,*.mov,*.wav,*.mp3,*.wmv,*.3gp, *.3g2,*.mp4";

            foreach (var imageFile in Directory.GetFiles(folder, "*.*").Where(
                s => supportedExtensions.Contains(
                    Path.GetExtension(s).ToLower())).Select(file => new FileInfo(file)))
                
                    _sourceData.Add(new MediaItem(imageFile));
        }

        #endregion
    }
}