using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using JoyOs.FileSystem.Model;

namespace JoyOs.FileSystem.FilesAPI
{
    public partial class FileTools
    {
        private const int SwShow = 5;
        private const int SeeMaskInvokeidlist = 0x0C;

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int ShellExecuteEx(SHELLEXECUTEINFO shinfo);

        public bool FilePropertiesDialog(ILogicItem itemToAttribute)
        {
            if (itemToAttribute == null)
                throw new ArgumentNullException("itemToAttribute", "Не выбрана сущность для получения атрибутов");

            var shInfo = new SHELLEXECUTEINFO
            {
                cbSize = Marshal.SizeOf(typeof(SHELLEXECUTEINFO)),
                lpFile = itemToAttribute.Info.FullName,
                nShow = SwShow,
                fMask = SeeMaskInvokeidlist,
                lpVerb = "properties"
            };

            return ShellExecuteEx(shInfo) == 1;
        }

        public static bool OpenWithDialog(ILogicItem itemToOpenWith)
        {
            if (itemToOpenWith == null)
                throw new ArgumentNullException("itemToOpenWith", "Не выбрана сущность для получения атрибутов");

            if (itemToOpenWith.Length == long.MinValue)
                throw new ArgumentException("itemToOpenWith", "Выбрана папка");

            var shInfo = new SHELLEXECUTEINFO
            {
                cbSize = Marshal.SizeOf(typeof(SHELLEXECUTEINFO)),
                lpFile = "rundll32.exe",
                lpParameters = "shell32.dll,OpenAs_RunDLL " + itemToOpenWith.Info.FullName,
                nShow = SwShow,
                fMask = SeeMaskInvokeidlist,
                lpVerb = "open"
            };

            return ShellExecuteEx(shInfo) == 1;
        }

        public string CalculateDataLength(IEnumerable<ILogicItem> itemsCollection)
        {
            if (itemsCollection == null)
                throw new ArgumentNullException("itemsCollection", "Не выбраны файлы");

            var filesCount = 0L;
            var directoriesCount = 0L;
            var filesLength = 0L;

            foreach (var item in itemsCollection)
            {
                // UNDONE: Если это не папка ( а вдруг ссылка ? )
                if (item.IsFolder)
                {
                    FolderLength(item.Info.FullName, ref filesLength, ref filesCount, ref directoriesCount);
                }
                else
                {
                    ++filesCount;

                    filesLength += item.Length;
                }
            }

            var viewString = string.Format(
                "Общий размер файлов:\n{0} байт.\nВсего файлов - {1},\nкаталогов - {2}.\n",
                filesLength,
                filesCount,
                directoriesCount);

            if (filesLength >= 1024)
            {
                viewString += "(= " + (filesLength >> 10) + " Кб)\n\n";
            }

            return viewString;
        }

        private void FolderLength(string folder, ref long space, ref long totalFileCount, ref long totalDirectoryCount)
        {
            Debug.Assert(folder != null, "Folder is null!");
            // Текущая папка тоже считается
            ++totalDirectoryCount;

            var currentDir = _localFileSystem.DirectoryInformation(folder);
            var collection = currentDir.EnumerateFiles();

            // Все файлы в текущем каталоге
            totalFileCount += collection.LongCount();

            // Занимаемое файлами место 
            space += collection.Select(file => file.Length).Sum();

            // Продолжаем искать
            foreach (var newFolder in currentDir.GetDirectories())
            {
                FolderLength(
                    newFolder.FullName, ref space,
                    ref totalFileCount, ref totalDirectoryCount
                    );
            }
        }

        #region Nested type: SHELLEXECUTEINFO

        [StructLayout(LayoutKind.Sequential)]
        private class SHELLEXECUTEINFO
        {
            public int cbSize;
            public int fMask;
            public int hwnd;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectory;
            public int nShow;
            public int hInstApp;
            public int lpIDList;
            public string lpClass;
            public int hkeyClass;
            public int dwHotKey;
            public int hIcon;
            public int hProcess;
        }

        #endregion
    }
}