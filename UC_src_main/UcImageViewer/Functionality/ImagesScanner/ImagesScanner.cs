using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JoyOs.Media;

namespace UcImageViewer
{
    /// <summary>
    /// Interaction logic for ImageList.xaml
    /// </summary>
    public partial class ImageList
    {
        private readonly BackgroundWorker _galleryWorker = new BackgroundWorker
                                                               {
                                                                   WorkerSupportsCancellation = true,
                                                                   WorkerReportsProgress = true
                                                               };
        readonly BackgroundWorker _slideShowWorker = new BackgroundWorker
        {
            WorkerSupportsCancellation = true,
            WorkerReportsProgress = true
        };

        private void ScanImagesCompleted(
                                object sender,
                                RunWorkerCompletedEventArgs e)
        {

        }

        private void ScanImagesThread(
                                BackgroundWorker imagesScanner,
                                DoWorkEventArgs e)
        {
            IndexOfCurImg = -2;
            int count = 0;
            if (UiStartDirectory != null)
            {
                var directoryInfo = new DirectoryInfo(UiStartDirectory);

                foreach (var imageFile in directoryInfo.GetFiles().Where(s => s.Extension.Length>=3&&SupportedExtensions.Contains(
                    s.Extension.ToLower())))
                {
                    Dispatcher.Invoke(new Action(() =>
                                                     {
                                                         var img = new Image
                                                                       {
                                                                           Source = DefImage,
                                                                           Tag = imageFile.FullName,
                                                                           MaxWidth = SizeImage,
                                                                           MaxHeight = SizeImage*0.8,
                                                                           Stretch = Stretch.UniformToFill,
                                                                           Margin = new Thickness(3)
                                                                       };

                                                         img.MouseLeftButtonUp += OnImageWrapPanelClick;
                                                         img.MouseEnter += OnStartHover;
                                                         img.MouseLeave += OnEndHover;

                                                         imgWrapList.Children.Add(img);

                                                         var timeToolTip = new ToolTip
                                                                               {
                                                                                   PlacementTarget = img,
                                                                                   Content = imageFile.Name
                                                                               };

                                                         img.ToolTip = timeToolTip;
                                                         count = imgWrapList.Children.Count;
                                                         if (count != 1) return;

                                                         UcImageViewer.ImageList.IndexOfCurImg = 0;
                                                         OnImageWrapPanelClick(imgWrapList.Children[0],
                                                                               new RoutedEventArgs());
                                                     }), null);
                }

                for (int i = 0; i < count; i++)
                {
                    var counter = i;
                    Dispatcher.Invoke(new Action(() =>
                                                     {
                                                         if (imgWrapList.Children.Count == 0) return;

                                                         var img = ((Image) imgWrapList.Children[counter]);
                                                         img.Source = ImageUtils.GetSmallImage(img.Tag.ToString(),
                                                                                               (int) SizeImage + 30);
                                                     }), null);

                    Thread.Sleep(40);
                }
            }
        }

        private static void GalleryLoadWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker object that raised this event.
            var worker = (BackgroundWorker)sender;

            // Get the ImageList object and call the main method.
            var imagePlugin = (ImageList)e.Argument;

            imagePlugin.ScanImagesThread(worker, e);
        }
    }
}