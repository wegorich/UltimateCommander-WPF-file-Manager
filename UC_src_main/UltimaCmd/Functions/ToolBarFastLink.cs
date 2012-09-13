using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using UcFileViewer;

namespace UltimaCmd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private void FastButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.SpecialFolder sf;

            if (!Enum.TryParse(((Button)sender).Tag.ToString(), false, out sf)) return;
            var fullPath = Environment.GetFolderPath(sf);

            if (FileManager.FocusedExplorer == null) return;
            
            FileManager.FocusedExplorer.CurrentPath = fullPath + Path.DirectorySeparatorChar;
        }
    }
}
