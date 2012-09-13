using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using JoyOs.BusinessLogic.History;
using JoyOs.FileSystem;
using JoyOs.FileSystem.DriveScaner;
using JoyOs.FileSystem.FilesAPI;
using JoyOs.FileSystem.Model;
using JoyOs.FileSystem.Provider;
using JoyOs.FileSystem.Sorting;

namespace JoyOs.Controls.File
{
    /// <summary>
    /// Interaction logic for ExplorerGrid.xaml
    /// </summary>
    public partial class ExplorerGrid
    {
        /// <summary>
        /// Храним FileSystem для непосредственной работы
        /// </summary>
        private readonly IFileSystem _fileSystem = new UFileSystem();
        /// <summary>
        /// Храним историю
        /// </summary>
        private readonly IHistory<string> _history = new History<string>();
        /// <summary>
        /// Start rename our DataGridItem
        /// </summary>
        ILogicItem _selectedItem;
        /// <summary>
        /// Провайдер показывает / определяте тип данных с коротым работаем
        /// должен быть вне контрола но так проще
        /// </summary>
        private static readonly FileSysPathDataProvider PathProvider = new FileSysPathDataProvider();

        public ExplorerGrid()
        {
            InitializeComponent();
            pathBox.PathDataProvider = PathProvider;
            _history.MaxSize = 25;
        }

        /// <summary>
        /// История
        /// </summary>
        public IHistory<string> History
        {
            get { return _history; }
        }

        /// <summary>
        /// Текущий путь
        /// </summary>
        public string CurrentPath
        {
            get { return pathBox.CurrentPath; }
            set
            {
                if (pathBox.CurrentPath == value || !pathBox.PathDataProvider.LegalPath(value)) return;

                pathBox.CurrentPath = value;
                driveTab.Drive = Path.GetPathRoot(value);
                
                if (_history.Size == 0 || _history.Current != value)
                    _history.Add(value);
                
                var e = new EventArgs();
                OnSetFocusedDataGrid(e);
                OnNeedsUpdateSource(e);
            }
        }

        /// <summary>
        /// Текущий диск
        /// </summary>
        public string CurrentDrive
        {
            get { return driveTab.Drive; }
            set { driveTab.Drive = value; }
        }

        /// <summary>
        /// Обновляет все содержимое, медленная операция
        /// </summary>
        public void RefreshData()
        {
            dataGrid.CommitEdit();
            dataGrid.CancelEdit();
            dataGrid.Items.Refresh();
        }

        /// <summary>
        /// Получить состояние директории
        /// </summary>
        public DirectoryStatus Status
        {
            get { return directoryStatus; }
        }

        /// <summary>
        /// Коллекция команд для грида
        /// </summary>
        public CommandBindingCollection GridCommandBindings
        {
            get { return dataGrid.CommandBindings; }
        }

        /// <summary>
        /// Содержимое представления, для удобства
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return dataGrid.ItemsSource; }
            set { dataGrid.ItemsSource = value; }
        }

        /// <summary>
        /// Коллекция всех элементов
        /// </summary>
        public ItemCollection Items
        {
            get { return dataGrid.Items; }
        }

        /// <summary>
        /// Коллекция выделенных элементов
        /// </summary>
        public IList SelectedItems
        {
            get { return dataGrid.SelectedItems; }
        }

        /// <summary>
        /// Текущий выбранный объект
        /// </summary>
        public object SelectedItem
        {
            get { return dataGrid.SelectedItem; }
            set { dataGrid.SelectedItem = value; }
        }
        /// <summary>
        /// Меняет поставщика / тип данных для работы (не работает)
        /// </summary>
        public IPathDataProvider Provider
        {
            get { return pathBox.PathDataProvider; }
            set { pathBox.PathDataProvider = value; }
        }

        #region Control Event
        public event EventHandler SetFocusedExplorer;
        public event EventHandler NeedsUpdateSource;

        public void OnNeedsUpdateSource(EventArgs e)
        {
            var handler = NeedsUpdateSource;
            if (handler != null) handler(this, e);
        }

        public void OnSetFocusedDataGrid(EventArgs e)
        {
            var handler = SetFocusedExplorer;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region DataGrid events

        #region Drag And Drop Test
        private void DataGridDrop(object sender, DragEventArgs e)
        {
            var dat = dataGrid;
            var box = pathBox;
            var items = (List<ILogicItem>)dat.ItemsSource;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var droppedFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop, true);

            foreach (var droppedFilePath in droppedFilePaths.Where(
                droppedFilePath => _fileSystem.FileExists(droppedFilePath)
                    && items.Find(i => i.Info.FullName == droppedFilePath) == null))
                _fileSystem.CopyFile(droppedFilePath, box.CurrentPath + Path.GetFileName(droppedFilePath));
        }

        #endregion

        #region Selection
        private void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var status = directoryStatus;
            foreach (ILogicItem item in e.AddedItems)
            {
                if (item.IsFolder)
                {
                    status.SelectedDirectories++;
                }
                else
                {
                    status.SelectedFiles++;
                    status.SelectedLength += item.Length;
                }
            }
            foreach (ILogicItem item in e.RemovedItems)
            {
                if (item.IsFolder)
                {
                    status.SelectedDirectories--;
                }
                else
                {
                    status.SelectedFiles--;
                    status.SelectedLength -= item.Length;
                }
            }
            status.RefreshAll();
        }
        #endregion

        #region File Renaming
        /// <summary>
        /// Запрещаем ввод недопустимых символов при переименовании
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">text changes</param>
        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;

            foreach (var change in from change in e.Changes
                                   from character in textBox.Text.Substring(change.Offset, change.AddedLength)
                                   .Where(character => Path.GetInvalidFileNameChars().Contains(character))
                                   select change)
                textBox.Text = textBox.Text.Remove(change.Offset, change.AddedLength);
        }

        /// <summary>
        /// Переименновываем файл в момент удаления текст бокса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxUnloaded(object sender, RoutedEventArgs e)
        {
            var item = _selectedItem;
            var textBox = ((TextBox)sender);
            var newFileName = textBox.Text;

            try
            {
                if (item == null) return;
                if (item.Info.Name != newFileName)
                {
                    if (!item.IsFolder)
                    {
                        var file = (FileInfo)item.Info;
                        _fileSystem.RenameFile(item.Info.FullName, Path.Combine(file.DirectoryName, newFileName));
                    }
                    else
                    {
                        var file = Path.GetDirectoryName(item.Info.FullName);
                        _fileSystem.RenameDirectory(item.Info.FullName, Path.Combine(file, newFileName));
                    }
                }
                else
                {
                    item.Name = Path.GetFileNameWithoutExtension(item.Info.Name);
                    dataGrid.Items.Refresh();
                }
            }
            finally
            {
                _selectedItem = null;
                dataGrid.CurrentCell.Column.IsReadOnly = true;
            }
        }

        /// <summary>
        /// При медленном нажатии по ячейке колонки "Имя" открывается редактирование имени
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void DataGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnSetFocusedDataGrid(e);

            // Get selected DataGrid Item to rename
            var item = (ILogicItem)dataGrid.CurrentItem;
            if (item == null) return;

            if (item.Name == ViewDataItem.BackEntry)
            {
                dataGrid.CurrentCell.Column.IsReadOnly = true;
                return;
            }

            if (_selectedItem == item)
            {
                item.Name = item.Info.Name;

                dataGrid.CurrentCell.Column.IsReadOnly =
                    (dataGrid.CurrentCell.Column.Header.ToString() != "Имя");
                return;
            }

            _selectedItem = item;
        }
        #endregion

        #region ContextMenu
        /// <summary>
        /// При нажатии вылетает системное контекстное меню
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void DataGridPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnSetFocusedDataGrid(e);

            // Get selected DataGrid Item to rename
            var item = dataGrid.CurrentItem;
            if (item == null) return;

            var pt = e.GetPosition(dataGrid);
            pt = dataGrid.PointToScreen(pt);
            var strFiles = new string[dataGrid.SelectedItems.Count];
            for (var i = 0; i < strFiles.Length; i++)
            {
                strFiles[i] = ((ILogicItem)dataGrid.SelectedItems[i]).Info.FullName;
            }
            var windowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            FileContextMenu.ShowContextMenu(windowHandle, strFiles, Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), true);
        }

        #endregion

        #region Double Click
        /// <summary>
        /// Open new foalder or start file in new process
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">RoutedEventArgs</param>
        private void DataGridMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var fileName = ((ILogicItem)dataGrid.SelectedItem).Info.FullName;

            if (pathBox.PathDataProvider.Exist(fileName, false))
                CurrentPath = pathBox.PathDataProvider.LegalPath(fileName) ? fileName : fileName + Path.DirectorySeparatorChar;
        }

        #endregion

        #region Sorting Logic use
        /// <summary>
        /// Datas the grid sorting.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        /// The <see cref="System.Windows.Controls.DataGridSortingEventArgs"/> 
        /// instance containing the event data.</param>
        private void DataGridSorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            var direction = (e.Column.SortDirection != ListSortDirection.Ascending)
                                        ? ListSortDirection.Ascending
                                        : ListSortDirection.Descending;

            e.Column.SortDirection = direction;

            var lcv = (ListCollectionView)
                CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            var sortLogic = new SortLogic(direction, e.Column);
            lcv.CustomSort = sortLogic;
        }

        #endregion

        #endregion

        #region PathBox events
        private void PathBoxPathChanged(object sender, BusinessLogic.Event.ValueEventArgs e)
        {
            driveTab.Drive = Path.GetPathRoot(e.Value);
            OnNeedsUpdateSource(e);
        }
        #endregion

        #region Drive click event
        /// <summary>
        /// Event on TabItem clicked - it open root;
        /// </summary>
        /// <param name="sender">TabItem</param>
        /// <param name="e">RoutedEventArgs</param>
        private void DriveTabDriveSelected(object sender, DriveEventArgs e)
        {
            if (_fileSystem.DirectoryExists(e.DriveName))
            {
                CurrentPath = e.DriveName;
            }
            else
                if (Windows.Dialogs.MessageBox.ShowDialog(
                       String.Format(
                           "Диск {0,3} не найден.\n\nПерейти на системный диск {1} ?",
                           CurrentDrive,
                           Environment.SystemDirectory.Substring(0, 3)
                           ),
                       "Ultimate Commander - Подсистема доступа",
                       MessageBoxButton.YesNo) == true)
                {
                    CurrentPath = Environment.SystemDirectory.Substring(0, 3);
                    CurrentDrive = CurrentPath;
                }

        }
        #endregion
    }
}
