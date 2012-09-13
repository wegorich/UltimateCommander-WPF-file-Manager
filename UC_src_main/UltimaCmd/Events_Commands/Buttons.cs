using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

using JoyOs.FileSystem;
using JoyOs.FileSystem.Functionality.Icon;
using UcFileViewer;

using DataReader = JoyOs.BusinessLogic.Common.DataReader;
using MessageBox = JoyOs.Windows.Dialogs.MessageBox;

namespace UltimaCmd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region File Open Click

        /// <summary>
        /// Открывает файл при нажатии на кнопку на тулбаре
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">RoutedEventArgs</param>
        private void FileOpenToolBarClick(object sender, RoutedEventArgs e)
        {
            var clickedButton = (Button)sender;
            var fileToLaunch = clickedButton.Tag.ToString();

            try
            {
                if (File.Exists(fileToLaunch))
                {
                    Process.Start(fileToLaunch);
                }
            }
            catch (Win32Exception exp)
            {
                MessageBox.ShowDialog(
                    String.Format(
                        "Невозможно запустить процесс {0}\n\nHRESULT [{1:x}]\n\n{2}",
                        fileToLaunch,
                        exp.ErrorCode,
                        exp.Message
                        ),
                    "Ultimate Commander - Подсистема защиты",
                    MessageBoxButton.OK
                );
            }

        }

        #endregion

        #region On / Off Hidden Elements

        /// <summary>
        /// Show new or hide old files in current foulder
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void HiddenOnOffToolBar(object sender, RoutedEventArgs e)
        {
              ScanSystem.ShowHidden = ((ToggleButton)sender).IsChecked ?? false;
        }

        #endregion

        #region Drag & Drop Files to ToolBar

        /// <summary>
        /// Добавляем кнопки файлов перенесенных на тулбар
        /// </summary>
        /// <param name="fileName">string путь к файлу</param>
        private void AddToolBarButton(string fileName)
        {
            var btn = new Button();
            btn.Width = btn.Height = btn.MinWidth = 26;
            btn.Tag = fileName;

            var img = new Image
            {
                Source = IconExtractor.GetIcon(fileName),
                Margin = new Thickness(3)
            };

            var toolTip = new ToolTip
            {
                Content = Path.GetFileName(fileName)
            };

            var contMenu = new ContextMenu
                               {
                                   StaysOpen = true
                               };

            var menuItem = new MenuItem
                             {
                                 Icon = new Image { Source = new BitmapImage(
                                     new Uri("/UltimaCmd;component/Resources/delete.png",
                                                      UriKind.RelativeOrAbsolute))},
                                 Header = "Удалить",
                                 VerticalContentAlignment=VerticalAlignment.Center
                             };
            menuItem.Click += RemoveButtonClick;
            menuItem.Tag = btn;
            contMenu.Items.Add(menuItem);
            btn.Content = img;
            btn.ContextMenu = contMenu;
            btn.ToolTip = toolTip;

            btn.Click += FileOpenToolBarClick;

            toolBarFastLink.Items.Add(btn);
        }

        private void RemoveToolBarButton(object sender)
        {
            toolBarFastLink.Items.Remove(sender);
            _toolFilesList.Remove(((Button)sender).Tag.ToString());
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            RemoveToolBarButton(((MenuItem)sender).Tag);
        }

        /// <summary>
        /// Обрабатывает DragAndDrop на ToolBarPanel и добавляет кнопки файлам на ToolBar
        /// </summary>
        /// <param name="sender">ToolBarPanel</param>
        /// <param name="e">DragEventArgs</param>
        private void ToolBarPanelDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var droppedFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop, true);

            foreach (var droppedFilePath in droppedFilePaths.Where(
                droppedFilePath =>
                File.Exists(droppedFilePath) && _toolFilesList.IndexOf(droppedFilePath) == -1))
            {
                AddToolBarButton(droppedFilePath);
                _toolFilesList.Add(droppedFilePath);
            }
        }

        #endregion

        #region Data Grid Forward, Back and Refresh

        /// <summary>
        /// Buttons which helps user navigate in DataGrid( Folder )
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">RoutedEventArgs</param>
        private void DataGridButtonsClick(object sender, RoutedEventArgs e)
        {
            // UNDONE: Find all bad ( ArgumentNullException ) calls
            var page = (FileManager)mainFrame.Content;
            var btn = (Button)sender;

            switch (btn.Name)
            {
                case "dataGridRefresh":
                    page.RefreshOnExecute(sender, null);
                    break;
            }
        }

        #endregion

        #region Close Form

        /// <summary>
        /// Закрытие формы удаляем все лишние потоки
        /// </summary>
        /// <param name="sender">Window</param>
        /// <param name="e">CancelEventArgs</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            // сохраняем все кнопки в файл
            DataReader.SaveDataToFile("Default.BAR", _toolFilesList.ToArray());

            // удаляем лишние потоки
            var page = mainFrame.Content as FileManager;

            if (page != null) 
                page.PageUnloaded(page, new RoutedEventArgs());
        }

        #endregion
    }
}