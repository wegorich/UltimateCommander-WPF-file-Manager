using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using JoyOs.Media;

namespace UcImageViewer
{
    public partial class ImageList
    {
        #region Open / Save / Close / Recent Operations

        readonly SaveFileDialog _saveFileDialog = new SaveFileDialog
                                                                {
                                                                    Title = "Сохранить как",
                                                                    Filter = "Точечные рисунки (*.bmp;*.dib)|*.bmp;*.dib" +
                                                                                "|JPEG (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif" +
                                                                                "|GIF (*.gif)|*.gif|TIFF (*.tif;*.tiff)|*.tif;*.tiff|PNG (*.png)|*.png" +
                                                                                "|WPD (*.wpd)|*.wpd",
                                                                    FilterIndex = 4,
                                                                    AddExtension = true
                                                                };

        readonly OpenFileDialog _openFileDialog = new OpenFileDialog
        {
                                                                    CheckPathExists = true,
                                                                    Title = "Открыть файл(ы)",
                                                                    Filter = "Точечные рисунки (*.bmp;*.dib)|*.bmp;*.dib|JPEG (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif" +
                                                                                "|GIF (*.gif)|*.gif|TIFF (*.tif;*.tiff)|*.tif;*.tiff|PNG (*.png)|*.png|ICO (*.ico)|*.ico" +
                                                                                "|Все файлы изображений|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico" +
                                                                                "|Все файлы|*.*",
                                                                    FilterIndex = 7
                                                                };
        void OpenOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            if (UiStartDirectory != null)
                _openFileDialog.InitialDirectory = UiStartDirectory;
            if (_openFileDialog.ShowDialog() != true) return;

            var someFile = new FileInfo(_openFileDialog.FileName);
            UiStartDirectory = someFile.DirectoryName;
            
            // TODO: To BackgroundWorker  
            _galleryWorker.RunWorkerAsync(this);
        }

        void SaveCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
        }

        void SaveOnExecute(object sender, ExecutedRoutedEventArgs args)
        {            
        }

        void SaveAsCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
        }

        void SaveAsOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            if (UiStartDirectory != null) 
                _saveFileDialog.InitialDirectory = UiStartDirectory;

            if (_saveFileDialog.ShowDialog() != true) return;

            var someFile = new FileInfo(_saveFileDialog.FileName);
            _tempBitmap.UcSaveImage(someFile.FullName, someFile.Extension);
        }

        void CloseCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
        }

        void CloseOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
        }

        #endregion

        private void FileItemClick(object sender, RoutedEventArgs e)
        {
            switch(((MenuItem)sender).Tag.ToString())
            {
                case "Save":
                    SaveImage();
                    break;
                case "Clear":
                    imgWrapList.Children.Clear();
                    bigImage.Source=_tempBitmap= DefImage;
                    imageDescription.Content = "Нет изображения";
                    break;
            }
        }
    }
}