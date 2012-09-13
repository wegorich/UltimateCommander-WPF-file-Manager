using System;
using System.Collections.Generic;
using System.IO;

using JoyOs.BusinessLogic;

namespace JoyOs.FileSystem
{
    public interface IFileSystemWatchEnum : IEnumerable<FileSystemWatcher>, IDisposable, IBaseInterface
    {
        int     Count { get; }
        long  LongCount { get; }

        long Capacity { get; set; }

        FileSystemWatcher this[string path] { get; }
        FileSystemWatcher this[long index] { get; }

        IEnumerable<string> WatchingPaths { get; }

        bool Contains(string path);
        void Clear();

        FileSystemWatcher Add(string path);   
        bool Remove(string path);

        bool GlobalEnable { get; set; }

        event FileSystemEventHandler DefaultChanged;
        event FileSystemEventHandler DefaultCreated;
        event FileSystemEventHandler DefaultDeleted;
        event ErrorEventHandler          DefaultError;
        event RenamedEventHandler DefaultRenamed;        

        event EventHandler Changed;
    }
}
