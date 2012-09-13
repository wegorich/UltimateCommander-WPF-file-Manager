using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using JoyOs.BusinessLogic;
using JoyOs.BusinessLogic.Pluginable;

namespace UcImageViewer
{
    /// <summary>
    /// Interaction logic for ImageList.xaml
    /// </summary>
    public partial class ImageList : IListerPlugin
    {
        public static int IndexOfCurImg { get; set; }
        public double SizeImage { get; set; }
        public readonly int DefSize = 60;

        BigImageWindow _bigWindow;
        BitmapSource _tempBitmap;

        Point _startImgMove, _originImgMove;

        public ImageList()
        {
            InitializeComponent();
            //Кокие-то внутренние переменные
            IndexOfCurImg = -2;
            SizeImage = 60;
            DefaultEditingValues();

            // Меню
            Menu = new List<MenuItem>(mainMenu.Items.Cast<MenuItem>());

            mainMenu.Items.Clear();
            mainDockPanel.Children.Remove(mainMenu);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            var group = new TransformGroup();
            var sc = new ScaleTransform();
            var tt = new TranslateTransform();

            group.Children.Add(sc);
            group.Children.Add(tt);
            bigImage.LayoutTransform = group;

            BigImageWindow.ImageList = imgWrapList;

            _galleryWorker.DoWork += GalleryLoadWorkerDoWork;
            _galleryWorker.RunWorkerCompleted += ScanImagesCompleted;

            _galleryWorker.RunWorkerAsync(this);

            CommandBindings.Add(new CommandBinding(
                                                        ApplicationCommands.Save, SaveOnExecute, SaveCanExecute));
            CommandBindings.Add(new CommandBinding(
                                                        ApplicationCommands.SaveAs, SaveAsOnExecute, SaveAsCanExecute));
            CommandBindings.Add(new CommandBinding(
                                                        ApplicationCommands.Close, CloseOnExecute, CloseCanExecute));
        }       
    }
}