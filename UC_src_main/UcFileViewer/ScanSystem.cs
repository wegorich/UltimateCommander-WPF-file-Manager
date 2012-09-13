using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using JoyOs.Controls.File;
using JoyOs.FileSystem;
using JoyOs.Windows.Dialogs.Files;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for FileManager.xaml
    /// </summary>
    public partial class FileManager
    {
        #region Scan & Update view

        /// <summary>
        /// Scan the folder and add scaned information to dataGrid
        /// </summary>
        /// <param name="scannedGrid">DataGrid</param>
        public void ScanPathAndRefresh(ExplorerGrid scannedGrid)
        {
            if (scannedGrid == null)
                throw new ArgumentNullException("scannedDataGrid", "UC try to refresh view and get invalid [null] data panel");

            // Засекает время работы функции
            var stopWatchTime = Stopwatch.StartNew();
            
            long lFilesCount, lDirectoriesCount, lGeneralFilesLength;

            scannedGrid.ItemsSource = ScanSystem.ScanFileSystemEntries(
                                                                        scannedGrid.CurrentPath,
                                                                        out lFilesCount,
                                                                        out lDirectoriesCount,
                                                                        out lGeneralFilesLength
                                                                );
            stopWatchTime.Stop();

            scannedGrid.Status.SetTotal(lFilesCount, lDirectoriesCount, lGeneralFilesLength,
                                          stopWatchTime.Elapsed.TotalSeconds);
            scannedGrid.Status.SelectedDirectories = 1;
            scannedGrid.Status.RefreshAll();

            var watcher = (FileSystemWatcher)scannedGrid.Tag;
            watcher.Path = scannedGrid.CurrentPath;

            UcCurrentDirectory = scannedGrid.CurrentPath;
        }

        #endregion

        #region Search and Update FocusedExplorer
        public void SearchDlgClick(object sender, RoutedEventArgs e)
        {
            var searchDlg = (Search)sender;

            // Засекает время работы функции
            var stopWatchTime = Stopwatch.StartNew();
            long lFilesCount, lDirectoriesCount, lGeneralFilesLength;

            FocusedExplorer.ItemsSource = ScanSystem.ScanFileSystemEntries(
                                                                        FocusedExplorer.CurrentPath,
                                                                        out lFilesCount,
                                                                        out lDirectoriesCount,
                                                                        out lGeneralFilesLength,
                                                                        searchDlg.Text,
                                                                        searchDlg.Option
                                                                );
            FocusedExplorer.Status.SetTotal(lFilesCount, lDirectoriesCount, lGeneralFilesLength,
                                              stopWatchTime.Elapsed.TotalSeconds);
            FocusedExplorer.Status.SelectedDirectories = 1;
            FocusedExplorer.Status.RefreshAll();
            stopWatchTime.Stop();
        }
        #endregion
    }
}