using System.Windows.Input;
using UcFileViewer;

namespace UltimaCmd
{
    /// <summary>
    /// Interaction logic for FileManagerFiles.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Back | Forward
        /// <summary>
        /// Переходим на предыдущую директорию
        /// </summary>
        public void GoBackCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            var focused = FileManager.FocusedExplorer;
            args.CanExecute = (focused != null && focused.History.CurrentItemIndex > 0);
        }

        private void GoBackOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            var focused = FileManager.FocusedExplorer;
            focused.CurrentPath = focused.History.GoBack();
        }

        /// <summary>
        /// Переходим на следующую директорию
        /// </summary>
        public void GoAheadCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            var focused = FileManager.FocusedExplorer;
            args.CanExecute = (focused != null && focused.History.CurrentItemIndex < focused.History.Size-1);
        }

        private void GoAheadOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            var focused = FileManager.FocusedExplorer;
            focused.CurrentPath = focused.History.GoAhead();
        }

        #endregion        
    }
}