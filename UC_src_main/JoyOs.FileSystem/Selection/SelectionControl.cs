using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using JoyOs.FileSystem.Model;
using JoyOs.FileSystem.Properties;

namespace JoyOs.FileSystem.Selection
{
    public class SelectionControl<T> where T : ILogicItem
    {
        // Select / uselect group  
        public IEnumerable<T> SelectGroup(string defaultMask, string defaultTemplate, bool isSelect)
        {
            throw new NotImplementedException();
        }

        // Select all / unselect all; Inverse selection / select files/folders by extension 
        public IEnumerable SelectEntries(IList selectedItems, IEnumerable<T> allItems, SelectionOptions selectionOptions)
        {
            switch (selectionOptions)
            {
                case SelectionOptions.SelectAll:
                    InsertSelectItems(allItems,selectedItems);
                    break;

                case SelectionOptions.UnSelectAll:
                    selectedItems.Clear();
                    break;

                case SelectionOptions.Inverse:
                    var temp = allItems.Except(selectedItems.Cast<T>()).ToArray();
                    selectedItems.Clear();
                    InsertSelectItems(temp,selectedItems);
                    break;

                case SelectionOptions.SelectByExtension:

                    break;

                case SelectionOptions.SaveInternal:

                    break;

                case SelectionOptions.RestoreInternal:

                    break;

                case SelectionOptions.SaveToFile:
                    // Ничего не выделено
                    if (selectedItems == null)
                    {
                        MessageBox.Show(
                            "Ничего не выделено",
                            "Ultimate Commander - Подсистема выделения",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                        break;
                    }

                    var saveFileDialog = new SaveFileDialog
                                             {
                                                 Filter = "Текстовые документы (*.txt)|*.txt|Все файлы (*,*)|*.*",
                                                 FilterIndex = 0,
                                                 AddExtension = true
                                             };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var itemsText = new StringBuilder();
                        foreach (var item in selectedItems.Cast<T>())
                        {
                            itemsText.AppendLine(item.ToString());
                        }
                        File.WriteAllText(saveFileDialog.FileName,itemsText.ToString());

                    }
                    break;

                case SelectionOptions.RestoreFromFile:
                    var openFileDialog = new OpenFileDialog
                                              {
                                                  InitialDirectory = Environment.CurrentDirectory,
                                                  CheckPathExists = true,
                                                  DereferenceLinks = true,
                                                  Filter = "Текстовые документы (*.txt)|*.txt|Все файлы (*,*)|*.*",
                                                  FilterIndex = 0,
                                                  FileName = "*.txt"
                                              };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        // Check for vaild info in file

                        // Restore selection 
                    }
                    break;

                default:
                    if (selectionOptions != SelectionOptions.DoNothing)
                        throw new ArgumentOutOfRangeException("selectionOptions", selectionOptions,
                                                              Resources.SelectionControl_CopyPathsToClipBoard_OptionException);
                    break;
            }

            return selectedItems;
        }

        private static void InsertSelectItems(IEnumerable<T> allItems,IList selectedItems)
        {
            foreach (var item in allItems)
            {
                selectedItems.Add(item);
            }
        }

        public void CopyPathsToClipBoard(
            IEnumerable<ViewDataItem> collection1,
            IEnumerable<ViewDataItem> collection2,
            CopyPathsToClipBoardOptions copyOptions
            )
        {
            if (collection1 == null)
                throw new ArgumentNullException("collection1", 
                    Resources.SelectionControl_CopyPathsToClipBoard_Exception);

            if (collection2 == null)
                throw new ArgumentNullException("collection1", 
                    Resources.SelectionControl_CopyPathsToClipBoard_Exception);

            var clipData = new StringBuilder();

            switch (copyOptions)
            {
                case CopyPathsToClipBoardOptions.FileSystemEntriesNamesInternal:

                    foreach (var item in collection1)
                        clipData.AppendLine(item.Info.Name);

                    foreach (var item in collection2)
                        clipData.AppendLine(item.Info.Name);

                    break;

                case CopyPathsToClipBoardOptions.FileSystemEntriesNamesFullPaths:

                    foreach (var item in collection1)
                        clipData.AppendLine(item.Info.FullName);

                    foreach (var item in collection2)
                        clipData.AppendLine(item.Info.FullName);

                    break;

                case CopyPathsToClipBoardOptions.AllColumnsInternal:

                    foreach (var item in collection1)
                        clipData.AppendLine(item.ToString());

                    foreach (var item in collection2)
                        clipData.AppendLine(item.ToString());

                    break;

                case CopyPathsToClipBoardOptions.AllColumnsFullPaths:

                    foreach (var item in collection1)
                        clipData.AppendLine(item.ToFullString());

                    foreach (var item in collection2)
                        clipData.AppendLine(item.ToFullString());

                    break;

                default:
                    if (copyOptions != CopyPathsToClipBoardOptions.DoNothing)
                        throw new ArgumentOutOfRangeException("copyOptions", copyOptions,
                            Resources.SelectionControl_CopyPathsToClipBoard_OptionException);
                    break;
            }

            Clipboard.SetData(DataFormats.UnicodeText, clipData.ToString());
        }

        public int CompareCatalogs(IEnumerable collection1, IEnumerable collection2,
                                   CompareCatalogsOptions compareOptions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Выделить все папки в DataGrid
        /// </summary>
        public void SelectAllFolder()
        {
            //FocusedDataGrid.SelectedItems.Clear();
            //var list = (List<UcGridItem>)FocusedDataGrid.ItemsSource;

            //Debug.Assert(list != null, "Hey, the list of items in selected DataGrid is null");

            //foreach (var item in list.Where(item => item.Type == "<Папка>"))
            //    FocusedDataGrid.SelectedItems.Add(item);
        }

        /// <summary>
        /// Выделить все файлы или папки по разрешинию 
        /// надо указать name=""
        /// или по имени, либо по тому и другому
        ///  </summary>
        /// <remarks>
        /// Можно использовать для поиска
        /// </remarks>
        /// <param name="name">Строка содержащая имя файла</param>
        /// <param name="extension">Строка с разрешение файла</param>
        public void SelectItem(string name, string extension = "")
        {
            //FocusedDataGrid.SelectedItems.Clear();
            //var list = (List<UcGridItem>)FocusedDataGrid.ItemsSource;

            //name = name.ToLower();
            //extension = extension.ToLower();

            //foreach (var item in list.Where(item =>
            //    item.Name.ToLower().Contains(name) && item.Type.ToLower().Contains(extension)))
            //    FocusedDataGrid.SelectedItems.Add(item);
        }

        #region Nested type: CompareCatalogsOptions

        public enum CompareCatalogsOptions
        {
            DoNothing = 0,

            CompareAll,
            CompareWithoutSimilarFiles
        }

        #endregion

        #region Nested type: CopyPathsToClipBoardOptions

        public enum CopyPathsToClipBoardOptions
        {
            DoNothing = 0,

            FileSystemEntriesNamesInternal,
            FileSystemEntriesNamesFullPaths,

            AllColumnsInternal,
            AllColumnsFullPaths
        }

        #endregion

        #region Nested type: SelectionOptions

        public enum SelectionOptions
        {
            DoNothing = 0,

            SelectAll,
            UnSelectAll,
            Inverse,
            SelectByExtension,

            SaveInternal,
            RestoreInternal,
            SaveToFile,
            RestoreFromFile
        }

        #endregion
    }
}