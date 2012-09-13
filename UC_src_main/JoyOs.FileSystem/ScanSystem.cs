using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using JoyOs.FileSystem.Model;

namespace JoyOs.FileSystem
{
    public class ScanFlagEventArgs : EventArgs
    {
        public bool OldValue { get; private set; }
        public bool NewValue { get; set; }

        public ScanFlagEventArgs(bool oldValue, bool newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public delegate void ScanFlagEventHandler(ScanFlagEventArgs e);

    public static class ScanSystem
    {
        private static readonly IFileSystem FileSystem = new UFileSystem();
        private static bool _showHidden;
        private static bool _showSystem;

        public static event ScanFlagEventHandler OnHiddenChanged;
        public static event ScanFlagEventHandler OnSystemChanged;

        public static bool ShowHidden
        {
            get { return _showHidden; }
            set
            {
                if (_showHidden == value || OnHiddenChanged == null) return;
                _showHidden = value;
                OnHiddenChanged(new ScanFlagEventArgs(_showHidden, value));
            }
        }

        public static bool ShowSystem
        {
            get { return _showSystem; }
            set
            {
                if (_showSystem == value || OnSystemChanged == null) return;
                _showSystem = value;
                OnSystemChanged(new ScanFlagEventArgs(_showSystem, value));
            }
        }
        
        #region Scan FileSystem Entries
        public static IEnumerable<ILogicItem> ScanFileSystemEntries(
            string pathToScan,
            out long lFilesCount,
            out long lDirectoriesCount,
            out long lGeneralFilesSize,
            string searchPattern="*",
            SearchOption searchOption=SearchOption.TopDirectoryOnly)
        {
            Debug.Assert(pathToScan != null, "Hat, pathToScan is null?");

            var currentDirectory = FileSystem.DirectoryInformation(pathToScan);
            //UNDONE: Search pattern
            var entriesInfo = currentDirectory.EnumerateFileSystemInfos(searchPattern,searchOption);

            lFilesCount = lDirectoriesCount = lGeneralFilesSize = 0L;
            ICollection<ILogicItem> folderCollection = new List<ILogicItem>();
            ICollection<ILogicItem> itemCollection = new List<ILogicItem>();

            if (currentDirectory.Parent != null&&searchOption==SearchOption.TopDirectoryOnly)
            {
                folderCollection.Add(new ViewDataItem(currentDirectory.Parent) { Name = ViewDataItem.BackEntry });
            }

            foreach (var entry in entriesInfo)
            {
                if ((entry.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden && !ShowHidden ||
                    ((entry.Attributes & FileAttributes.System) == FileAttributes.System && !ShowSystem))
                    continue;

                var fileInfo = entry as FileInfo;
                // Если это файл
                if (fileInfo != null)
                {
                    lGeneralFilesSize += fileInfo.Length;
                    ++lFilesCount;

                    itemCollection.Add(new ViewDataItem(fileInfo));
                }
                else // Директория
                {
                    ++lDirectoriesCount;

                    folderCollection.Add(new ViewDataItem(entry));
                }
            }

            return folderCollection.Union(itemCollection).ToList(); //Cant change list
        }
        #endregion
    }
}