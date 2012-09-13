using JoyOs.Controls.File;
using JoyOs.FileSystem;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for FileManager.xaml
    /// </summary>
    public partial class FileManager
    {
        /// <summary>
        /// DataGrid с текущим фокусом
        /// </summary>
        public static ExplorerGrid FocusedExplorer { get; private set; }

        public static string UcCurrentDirectory { get; set; }

        /// <summary>
        /// Храним FileSystem для непосредственной работы
        /// </summary>
        readonly IFileSystem _fileSystem = new UFileSystem();
    }
}