using System;
using JoyOs.BusinessLogic;

namespace JoyOs.FileSystem
{
    public enum SearchPathAdd
    {
        PathAddToHead,	// First path searched
        PathAddToTail,	// Last path searched
    };

    public enum FileWarningLevel
    {
        // A problem!
        FilesystemWarning = -1,

        // Don't print anything
        FilesystemWarningQuiet,

        // On shutdown, report names of files left unclosed
        FilesystemWarningReportunclosed,

        // Report number of times a file was opened, closed
        FilesystemWarningReportusage,

        // Report all open/close events to console ( !slow! )
        FilesystemWarningReportAllAccesses,

        // Report all open/close/read events to the console ( !slower! )
        FilesystemWarningReportAllAccessesRead,

        // Report all open/close/read/write events to the console ( !slower! )
        FilesystemWarningReportAllAccessesReadWrite
    };

    public struct FileSystemStatistics
    {
        readonly uint _uiReads;
        readonly uint _uiWrites;
        readonly uint _uiBytesRead;
        readonly uint _uiBytesWritten;
        readonly uint _uiSeeks;

        public FileSystemStatistics(uint uiReads, uint uiWrites, uint uiBytesRead, 
            uint uiBytesWritten, uint uiSeeks) : this()
        {
            _uiReads = uiReads;
            _uiWrites = uiWrites;
            _uiBytesRead = uiBytesRead;
            _uiBytesWritten = uiBytesWritten;
            _uiSeeks = uiSeeks;
        }

        #region Standart overrides

        public override string ToString()
        {
            return String.Format(
                                    "read {0:D8}, written {1:D8}, bytes read {2:D8}, bytes written {3:D8}, seeks {4:D8}",
                                    _uiReads, _uiWrites, _uiBytesRead, _uiBytesWritten, _uiSeeks
                                    );
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileSystemStatistics)) return false;

            return Equals((FileSystemStatistics)obj);
        }

        public bool Equals(FileSystemStatistics fss)
        {
            if (!_uiReads.Equals(fss._uiReads)
                || !_uiWrites.Equals(fss._uiWrites)
                || !_uiBytesRead.Equals(fss._uiBytesRead)
                || !_uiBytesWritten.Equals(fss._uiBytesWritten)
                || !_uiSeeks.Equals(fss._uiSeeks))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int)(_uiBytesRead ^ _uiBytesWritten ^ _uiSeeks);
        }

        public static Boolean operator ==(FileSystemStatistics a, FileSystemStatistics b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(FileSystemStatistics a, FileSystemStatistics b)
        {
            return (!(a == b));
        }

        #endregion
    };

    public delegate bool WarningFunction(string format, params object[ ] args); 

    public interface IFileSystem : IBaseFilleSystem
    {
        FileSystemStatistics FilesystemStatistics{ get; }

        void    AddSearchPath(string path, SearchPathAdd addType = SearchPathAdd.PathAddToTail);
        bool	    RemoveSearchPath    ( string path );

	    void			LogLevelLoadStarted( string name );
	    void			LogLevelLoadFinished( string name );
	    int			ProgressCounter();
        
        string       CurrentDirectory { get; set; }

        bool        IsOperationActive { get; }

	    // Dump to printf/OutputDebugString the list of files that have not been closed
	    void			PrintOpenedFiles();

        event WarningFunction Warning; 
	    FileWarningLevel WarningLevel { get; set; }
    }
}
