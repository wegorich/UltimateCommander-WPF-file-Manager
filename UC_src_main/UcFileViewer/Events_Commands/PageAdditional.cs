using System.Windows;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileManager
    {
        #region PageUnloaded Delegate

        // UNDONE: Port Watchers to FileSystem
        public void PageUnloaded(object sender, RoutedEventArgs e)
        {
            FocusedExplorer = null;
            if ( _fileSystem.Watchers != null )
            {
                _fileSystem.Watchers.Clear();
            }
        }

        #endregion
    }
}