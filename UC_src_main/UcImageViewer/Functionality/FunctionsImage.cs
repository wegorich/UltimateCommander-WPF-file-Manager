using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UcImageViewer
{
    public partial class ImageList
    {
        /// <summary>
        /// Картинка по умолчанию
        /// </summary>
        public static BitmapImage DefImage = new BitmapImage(
            new Uri("pack://application:,,,/JoyOs.Media;component/img/Plugins/Photos/PHOTO.ico", UriKind.RelativeOrAbsolute));

        /// <summary>
        /// Начальная директория, нужно задавать до создания объекта
        /// </summary>
        public static string UiStartDirectory { get; set; }



        #region Thread AutoPlay

        private void StartThreat(object obj,DoWorkEventArgs eventArgs)
        {
            do
            {
                Dispatcher.Invoke(new Action(() => InterfaceEventStart("forwardBtn")), null);
                Thread.Sleep(1500);
            } while (true);
        }

        #endregion

        #region InterfaceEventStart

        protected void InterfaceEventStart(string strName)
        {
            if (imgWrapList.Children.Count > 0)
                switch (strName)
                {
                    case "backBtn"://берет следующую картинку слева
                        if (0 < UcImageViewer.ImageList.IndexOfCurImg)
                            --UcImageViewer.ImageList.IndexOfCurImg;
                        else
                            UcImageViewer.ImageList.IndexOfCurImg = imgWrapList.Children.Count - 1;
                        OnImageWrapPanelClick(imgWrapList.Children[UcImageViewer.ImageList.IndexOfCurImg], new RoutedEventArgs());
                        break;
                    case "forwardBtn"://берет следующую картинку справа
                        if (imgWrapList.Children.Count - 1 > UcImageViewer.ImageList.IndexOfCurImg)
                            ++UcImageViewer.ImageList.IndexOfCurImg;
                        else
                            UcImageViewer.ImageList.IndexOfCurImg = 0;
                        OnImageWrapPanelClick(imgWrapList.Children[UcImageViewer.ImageList.IndexOfCurImg], new RoutedEventArgs());
                        break;
                    case "playBtn"://автопроигрывание
                        if (!_slideShowWorker.IsBusy)
                        {
                            _slideShowWorker.DoWork += StartThreat;
                            _slideShowWorker.RunWorkerAsync(this);
                            playStopImg.Source = new BitmapImage(new Uri("pack://application:,,,/JoyOs.Media;component/img/Plugins/Photos/pause.png"));
                        }
                        else
                        {
                            _slideShowWorker.DoWork -= StartThreat;
                            playStopImg.Source = new BitmapImage(new Uri("pack://application:,,,/JoyOs.Media;component/img/Plugins/Photos/play.png"));
                        }
                        break;

                    case "maximazeBtn":
                        _bigWindow = new BigImageWindow();
                        _bigWindow.Show();
                        _bigWindow.OnStop += BigWindowStop;
                        break;
                }
        }

        #endregion
    }
}
