using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using JoyOs.FileSystem.Model;
using MessageBox = JoyOs.Windows.Dialogs.MessageBox;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileManager
    {
        #region Create Directory Click

        /// <summary>
        /// Add new folder (with default name) in current folder
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void CreateDirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            // Создаем Новую  папку
            _fileSystem.CreateDirectory(FocusedExplorer.CurrentPath, null);
        }
        #endregion

        #region Copy FileSystemEntry Click
        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = (FocusedExplorer != null && FocusedExplorer.SelectedItems != null);
        }

        private void CopyOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            var copyToDat = ((FocusedExplorer == expLeft)
                                 ? expRight.CurrentPath
                                 : expLeft.CurrentPath);

            if (MessageBox.ShowDialog(
                MakeMessageStr("Копировать"),
                "Ultimate Commander - Подсистема копирования данных",
                MessageBoxButton.YesNo
                    ) != true) return;

            ILogicItem currentItem = null;

            try
            {
                foreach (ILogicItem t in FocusedExplorer.SelectedItems)
                {
                    currentItem = t;

                    if (currentItem.IsFolder)
                        _fileSystem.CopyDirectory(currentItem.Info.FullName, Path.Combine(copyToDat, currentItem.Info.Name));
                    else
                        _fileSystem.CopyFile(currentItem.Info.FullName,Path.Combine(copyToDat, currentItem.Info.Name));

                }
            }
            catch (System.SystemException ex)
            {
                if (currentItem != null)
                    MessageBox.ShowDialog(
                        string.Format("Невозможно копировать файл {0}, " +
                                      "возможно, файл используется или защищен:\n{1}",
                                      currentItem.Info.FullName,
                                      ex.Message),
                        "Ultimate Commander - Подсистема доступа",
                        MessageBoxButton.OK);
            }
        }
        #endregion

        #region Delete FileSystemEntry Click
        private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = (FocusedExplorer != null && FocusedExplorer.SelectedItems != null);
        }

        private void DeleteOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            if (MessageBox.ShowDialog(
                MakeMessageStr("Удалить"),
                "Ultimate Commander - Подсистема данных",
                MessageBoxButton.YesNo
                    ) != true) return;

            ILogicItem currentItem = null;

            try
            {
                foreach (ILogicItem t in FocusedExplorer.SelectedItems)
                {
                    currentItem = t;

                    if (currentItem.IsFolder)
                        _fileSystem.DeleteDirectory(currentItem.Info.FullName);
                    else
                        _fileSystem.DeleteFile(currentItem.Info.FullName);
                  }
            }
            catch (System.SystemException ex)
            {
                if (currentItem != null)
                    MessageBox.ShowDialog(
                        string.Format("Невозможно удалить файл {0}, " +
                                      "возможно, файл используется или защищен:\n{1}",
                                      currentItem.Info.FullName,
                                      ex.Message),
                        "Ultimate Commander - Подсистема доступа",
                        MessageBoxButton.OK);
            }
        }


        #endregion

        #region Cut FileSystemEntry Click
        void CutCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = (FocusedExplorer != null && FocusedExplorer.SelectedItems != null);
        }
        void CutOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            // UNDONE: Save items names to Cilpboard

            // Clipboard.SetData( );
        }
        #endregion

        #region Move FileSystemEntry Click
        /// <summary>
        /// Move file from selected DataGrid to unselected (dont make copies )
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void MoveButtonClick(object sender, RoutedEventArgs e)
        {
            var moveTo = ((FocusedExplorer == expLeft) ? expRight.CurrentPath : expLeft.CurrentPath);

            if (MessageBox.ShowDialog(
                MakeMessageStr("Копировать"),
                "Ultimate Commander - Подсистема копирования данных",
                MessageBoxButton.YesNo
                    ) != true) return;

            foreach (ILogicItem item in FocusedExplorer.SelectedItems)
            {
                if (item.IsFolder)
                    _fileSystem.MoveDirectory(item.Info.FullName, Path.Combine(moveTo, item.Info.Name));
                else
                    _fileSystem.MoveFile(item.Info.FullName, Path.Combine(moveTo,item.Info.Name));

            }
        }
        #endregion
        /// <summary>
        /// Makes the message from selected items in focused data grid to string.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        private string MakeMessageStr(string eventName)
        {
            var messageStr = new StringBuilder(eventName + "\n\n...\n");
            for (var i = 0; i < 5 && i < FocusedExplorer.SelectedItems.Count; i++)
            {
                messageStr.Append(FocusedExplorer.SelectedItems[i]);
                messageStr.Append("\n");
            }

            if (FocusedExplorer.SelectedItems.Count > 0)
                messageStr.Append("...");
            return messageStr.ToString();
        }
    }
}