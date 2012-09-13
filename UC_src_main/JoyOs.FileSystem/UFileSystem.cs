using System;
using System.IO;
using System.Linq;
using System.Security;

using System.Security.AccessControl;

namespace JoyOs.FileSystem
{
    /// <summary>
    /// Класс для работы с файловой системой
    /// </summary>
    public class UFileSystem : IFileSystem
    {
        /// <summary>
        /// Here we have flag, that indicates, file system operation is still active
        /// We should check it before exit app. / Windows
        /// </summary>
        private readonly bool _isOperationActive;

        private readonly IFileSystemWatchEnum _fsWatchers;

        private FileWarningLevel _fileWarningLevel;

        public UFileSystem()
        {
            _isOperationActive = false;

            _fsWatchers = new FileSystemWatchEnum();

            _fileWarningLevel = FileWarningLevel.FilesystemWarning;
        }

        ~UFileSystem()
        {
            if (_fsWatchers != null)
            {
                _fsWatchers.Dispose();
            }
        }

        #region Члены IBaseFilleSystem

        public IFileSystemWatchEnum Watchers
        {
            get { return _fsWatchers; }
        }

        public void RefreshDirectory(FileSystemInfo e)
        {
            e.Refresh();
        }

        private static void CreateSubDirByPath(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directoryName);//fake resharper 
        }

        #region Delete
        public void DeleteFile(string fileName)
        {
            // UNDONE: Warning, if we have no permissions
            //  to access the path, FileExists() return false !
            if (!FileExists(fileName)) return;
            var f = FileInformation(fileName);
            DeleteFile(f);
        }

        public void DeleteDirectory(string dirName)
        {
            // UNDONE: Warning, if we have no permissions
            //  to access the path, DirectoryExists() return false !
            var dir = new DirectoryInfo(dirName);
            if (!dir.Exists) return;
            
            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            
            foreach (var s in files)
            {
                DeleteFile(s);
            }

            dir.Delete(true);
         }

        private static void DeleteFile(FileInfo s)
        {
            s.IsReadOnly = false;
            s.Delete();
        }
        #endregion      

        #region Copy
        public void CopyDirectory(string oldName, string newName)
        {
            var newDir = new DirectoryInfo(newName);
            if (newDir.Exists)
                DeleteDirectory(newName);

            newDir.Create();
            
            var dir = new DirectoryInfo(oldName);
            var dirNamesLength = dir.FullName.Length+1;//+1 remove \\
            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            
            foreach (var s in files)
            {
                var str = Path.Combine(newName,s.FullName.Substring(dirNamesLength));
                CreateSubDirByPath(str);
                CopyFile(s.FullName, str);
            }
        }

        [SecuritySafeCritical]
        public void CopyFile(string oldName, string newName)
        {
            File.Copy(oldName, newName,true);
        }
        #endregion

        #region Rename
        public void RenameFile(string oldName,string newName)
        {
            MoveFile(oldName, newName);
        }


        public void RenameDirectory(string oldName, string newName)
        {
            if (oldName == newName) return;
            if (Directory.Exists(newName))
                DeleteDirectory(newName);
            Directory.Move(oldName, newName);
        }
        #endregion

        #region Move
        public void MoveFile(string oldName, string newName)
        {
            if (oldName == newName) return;
            var file = new FileInfo(newName);
            if (file.Exists)
                DeleteFile(file);
            //UNDONE: сделать эксепшен и перемещать с заменой или оба файла
            File.Move(oldName, newName);//can be access denied
        }

        public void MoveDirectory(string oldName, string newName)
        {
            var dir = new DirectoryInfo(oldName);
            var newDir = new DirectoryInfo(newName);
            newDir.Create();

            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            var dirNameLength = dir.FullName.Length + 1;
            
            foreach (var s in files)
            {
                var str = Path.Combine(newName, s.FullName.Substring(dirNameLength));
                CreateSubDirByPath(str);
                MoveFile(s.FullName, str);
            }

            dir.Delete(true);
        }
        #endregion

        #region Create new
        // UNDONE: Check for basePath has new directories
        public void CreateFile(string fileName, string basePath)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = "Новый файл";

            var files = Directory.GetFiles(basePath, fileName+"*");

            fileName = Path.Combine(basePath, fileName);

            File.Create(files.Length == 0 ? fileName 
                                   : FindNewName(fileName, files));
        }

        public void CreateDirectory(string basePath, string dirName)
        {
            if (string.IsNullOrEmpty(dirName))
                dirName = "Новая папка";

            var directories = Directory.GetDirectories(basePath, dirName+"*");

            dirName = Path.Combine(basePath,dirName);

            Directory.CreateDirectory(directories.Length == 0 ? dirName 
                                                                : FindNewName(dirName, directories));
        }

        private static string FindNewName(string name, string[] files, string formatPattern = "{0} ({1})")
        {
            return (from newPath in files.Select((t, i) => String.Format(formatPattern, name, i)) 
                          let flag = files.Where(s => s == newPath).Any() 
                          where !flag select newPath).FirstOrDefault();
        }

        #endregion

        public bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path", "Путь к файлу пуст или не инициализирован");

            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path", "Путь к каталогу пуст или не инициализирован");

            return Directory.Exists(path);
        }

        // TODO: Remove, when IFileSystemWatchers Collection is ready  
        public FileSystemWatcher this[int index]
        {
            get
            {
                //var currentIndex = 0;

                //foreach (var watcher in _fsWatchersCollection)
                //{
                //    if (currentIndex++ == index)
                //        return watcher.Key;
                //}

                return null;
            }
        }

        // TODO: Remove, when IFileSystemWatchers Collection is ready  
        public FileSystemWatcher this[string scannedPath]
        {
            get
            {
                //foreach (var watcher in _fsWatchersCollection)
                //{
                //    if (watcher.Key.Path == scannedPath)
                //        return watcher.Key;
                //}

                return null;
            }
        }

        public FileSecurity GetAccessControl(string path)
        {
            return File.GetAccessControl(path);
        }

        public FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            return File.GetAccessControl(path, includeSections);
        }

        public void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            File.SetAccessControl(path, fileSecurity);
        }

        public FileInfo FileInformation(string path)
        {
            return new FileInfo(path);
        }

        public DirectoryInfo DirectoryInformation(string path)
        {
            return new DirectoryInfo(path);
        }

        public DriveInfo DriveInformation(string driveName)
        {
            return new DriveInfo(driveName);
        }

        public FileAttributes GetAttributes(string path)
        {
            // UNDONE: Interesting, is it work with directories ? 
            return File.GetAttributes(path);
        }

        public void SetAttributes(string path, FileAttributes fileAttributes)
        {
            File.SetAttributes(path, fileAttributes);
        }       

        public string[] GetLogicalDrives()
        {
            return Environment.GetLogicalDrives();
        }

        #endregion

        #region Члены IFileSystem

        public FileSystemStatistics FilesystemStatistics
        {
            get { throw new NotImplementedException(); }
        }

        public void AddSearchPath(string path, SearchPathAdd addType = SearchPathAdd.PathAddToTail)
        {
            throw new NotImplementedException();
        }

        public bool RemoveSearchPath(string path)
        {
            throw new NotImplementedException();
        }

        public void LogLevelLoadStarted(string name)
        {
            throw new NotImplementedException();
        }

        public void LogLevelLoadFinished(string name)
        {
            throw new NotImplementedException();
        }

        public int ProgressCounter()
        {
            throw new NotImplementedException();
        }

        public string CurrentDirectory
        {
            get
            {
                return Environment.CurrentDirectory;
            }
            set
            {
                Environment.CurrentDirectory = value;
            }
        }

        public bool IsOperationActive
        {
            get
            {
                return _isOperationActive;
            }
        }


        public void PrintOpenedFiles()
        {
            throw new NotImplementedException();
        }

        public event WarningFunction Warning;

        public FileWarningLevel WarningLevel
        {
            get
            {
                return _fileWarningLevel;
            }
            set
            {
                if (value < FileWarningLevel.FilesystemWarning
               || value > FileWarningLevel.FilesystemWarningReportAllAccessesReadWrite)
                    throw new ArgumentOutOfRangeException("value", value, "The value is out of range [FilesystemWarning; FilesystemWarningReportAllAccessesReadWrite]");

                _fileWarningLevel = value;
            }
        }

        #endregion
    }
}
