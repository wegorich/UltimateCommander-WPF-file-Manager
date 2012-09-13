using System;
using System.IO;
using System.Security.AccessControl;

using JoyOs.BusinessLogic;

namespace JoyOs.FileSystem
{
    public interface IBaseFilleSystem : IBaseInterface
    {
        IFileSystemWatchEnum Watchers { get; }

        FileSecurity GetAccessControl(string path);
        FileSecurity GetAccessControl(string path, AccessControlSections includeSections);
        void SetAccessControl(string path, FileSecurity fileSecurity);

        FileInfo FileInformation(string path);
        DirectoryInfo DirectoryInformation(string path);
        DriveInfo DriveInformation(string driveName);

        FileAttributes GetAttributes(string path);
        void SetAttributes(string path, FileAttributes fileAttributes);

        string[] GetLogicalDrives();

        bool FileExists(string path);
        bool DirectoryExists(string path);

        void CreateFile(string fileName, string basePath);
        void CreateDirectory(string basePath, string dirName);

        void DeleteFile(string fileName);
        void DeleteDirectory(string dirName);

        void RefreshDirectory(FileSystemInfo e);

        void CopyDirectory(string oldName, string newName);
        void CopyFile(string oldName, string newName);

        void MoveFile(string oldName, string newName);
        void MoveDirectory(string oldName, string newName);

        void RenameFile(string oldName, string newName);
        void RenameDirectory(string oldName, string newName);
    }
}
