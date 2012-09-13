using System;

using JoyOs.BusinessLogic;
using JoyOs.FileSystem.Properties;


namespace JoyOs.FileSystem.FilesAPI
{
    public partial class FileTools
    {
        private IFileSystem _localFileSystem;

        public FileTools()
        {
        }

        public FileTools(IFileSystem fileSystem)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem", Resources.FileTools_FileSystem_NullException);

            _localFileSystem = fileSystem;
        }

        public IFileSystem FileSystem
        {
            get
            {
                return _localFileSystem;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("fileSystem", Resources.FileTools_FileSystem_NullException);

                _localFileSystem = value;
            }
        }

    }
}
