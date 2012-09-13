using System.Windows;
using System.Windows.Controls;
using JoyOs.Windows.Dialogs.Files;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileManager
    {
        private void CommandItemClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "search":
                    var searchDlg=Search.Create();
                    searchDlg.Click += SearchDlgClick;
                    searchDlg.Show();
                    break;
            }
        }
    }
}