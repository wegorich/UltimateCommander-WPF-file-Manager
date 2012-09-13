using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JoyOs.FileSystem.Model;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileManager
    {
        #region File System Watcher Delete | Changed | New | Renamed

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var dat = FocusedExplorer;
            Dispatcher.Invoke(new Action(() =>
                                             {
                                                 dat = (source == expLeft.Tag) ? expLeft : expRight;
                                                 dat.Items.SortDescriptions.Clear();
                                             }), null);

            var itemList = (List<ILogicItem>)dat.ItemsSource;

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    foreach (var item in dat.Items.Cast<ILogicItem>().Where(item => item.Info.FullName == e.FullPath))
                    {
                        item.Info = new FileInfo(e.FullPath);
                        break;
                    }
                    break;
                case WatcherChangeTypes.Created:
                    // TODO: Необходима синхронизация с переименованием элемента
                    Dispatcher.Invoke(new Action(() => itemList.Add(
                        File.Exists(e.FullPath)
                            ? new ViewDataItem(new FileInfo(e.FullPath))
                            : new ViewDataItem(new DirectoryInfo(e.FullPath))
                                                           )), null);
                    break;
                case WatcherChangeTypes.Deleted:
                    for (var i = 0; i < dat.Items.Count; i++)
                    {
                        var item = itemList[i];

                        if (item.Info.FullName != e.FullPath) continue;

                        Dispatcher.Invoke(new Action(() => itemList.RemoveAt(i)), null);
                        break;
                    }
                    // TODO: sometimes failing
                    //var dataView = CollectionViewSource.GetDefaultView(dat.ItemsSource);
                    break;
            }
            Dispatcher.Invoke(new Action(dat.RefreshData), null);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            var dat = FocusedExplorer;
            Dispatcher.Invoke(new Action(() =>
            {
                dat = (source == expLeft.Tag) ? expLeft : expRight;
                dat.Items.SortDescriptions.Clear();
            }), null);

            foreach (var item in dat.Items.Cast<ILogicItem>()
                .Where(item => item.Info.FullName == e.OldFullPath))
            {
                item.Info = new FileInfo(e.FullPath);

                Dispatcher.Invoke(new Action(dat.RefreshData), null);
                return;
            }
        }

        #endregion

        #region fs Watcher ( наблюдаем за файловой системой инициализация )

        /// <summary>
        /// FileSystemWatcher initialization
        /// </summary>
        /// <param name="fswatcher">FileSystemWatcher object to initialize</param>
        /// <param name="monitoringPath">Path to monitore</param>
        /// <exception cref="FileNotFoundException">Generates if <paramref name="monitoringPath"/>
        /// is null or not exists </exception>
        private void InitializeWatcher(FileSystemWatcher fswatcher, string monitoringPath)
        {
            Debug.Assert(fswatcher != null, "watcher is null!");

            // Наблюдаем за измнениями файловой системы
            fswatcher.Path = monitoringPath;

            // Определяем события, которые будем отслеживать
            fswatcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName | NotifyFilters.Attributes;
            //fswatcher.Filter = "*.htm"; // Устанавливаем фильтр для отслеживаемых файлов

            // Вызываемые события
            fswatcher.Changed += OnChanged; // Изменен
            fswatcher.Created += OnChanged; // Создан
            fswatcher.Deleted += OnChanged; // Удален
            fswatcher.Renamed += OnRenamed; // Переименован

            // Включаем наблюдение
            fswatcher.EnableRaisingEvents = true;
        }

        #endregion
    }
}