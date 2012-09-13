using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using UcFileViewer;
using JoyOs.BusinessLogic.Pluginable;
using UcImageViewer;

namespace UltimaCmd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Frame Changed

        private void FrameMainNavigated(object sender, NavigationEventArgs e)
        {
            // TODO: Удаляем старые элементы
            for (var i = topMenu.Items.Count - 4; i >= 0; i--)
                topMenu.Items.RemoveAt(i);

            var menuIntegration = (IPluginable)mainFrame.Content;
            var menuItems = menuIntegration.Menu.ToArray();

            for (var i = 0; i < menuItems.Length; i++)
            {
                topMenu.Items.Insert(i, menuItems[i]);
            }

            var isFileManager = (mainFrame.Content as FileManager != null);

            dataGridRefresh.IsEnabled = isFileManager;
            hiddenElem.IsEnabled = isFileManager;
        }

        #endregion

        #region Open Data Grid Xaml

        private void OpenPluginsClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;

            var page = mainFrame.Content as FileManager;
            if (page != null)
                UcCurrentDirectory = FileManager.UcCurrentDirectory;

            switch (btn.Tag.ToString())
            {
                case "1":
                    ImageList.UiStartDirectory = UcCurrentDirectory;
                    mainFrame.Navigate(new Uri("pack://application:,,,/UcImageViewer;component/ImageList.xaml", UriKind.Absolute));
                    break;

                case "2":
                    UcMediaViewer.MediaViewer.UvStartDirectory = UcCurrentDirectory;
                    mainFrame.Navigate(new Uri("pack://application:,,,/UcMediaViewer;component/MediaViewer.xaml", UriKind.Absolute));
                    break;

                default:
                    FileManager.UcCurrentDirectory =UcCurrentDirectory;
                    mainFrame.Navigate(new Uri("pack://application:,,,/UcFileViewer;component/FileManager.xaml", UriKind.Absolute));
                    break;
            }
        }

        #endregion
    }
}