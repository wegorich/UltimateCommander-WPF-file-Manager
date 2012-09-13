using System.IO;
using System.Windows;

namespace JoyOs.Windows.Dialogs.Files
{
    /// <summary>
    /// Interaction logic for ChangeAttributes.xaml
    /// </summary>
    public partial class ChangeAttributes
    {
        private FileSystemInfo _info;

        /// <summary>
        /// Конструктор Диалога
        /// </summary>
        private ChangeAttributes()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        public FileSystemInfo Info
        {
            get { return _info; }
            set
            {
                _info = value;
                archive.IsChecked = (Info.Attributes & FileAttributes.Archive) == FileAttributes.Archive;
                readOnly.IsChecked = (Info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                hidden.IsChecked = (Info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                system.IsChecked = (Info.Attributes & FileAttributes.System) == FileAttributes.System;
            }
        }

        private static void SetOrDeleteAttr(bool flag,FileSystemInfo inf, FileAttributes attr)
        {
            if (flag)
            {
                if ((inf.Attributes & attr) == attr) return;
                inf.Attributes ^= attr;
            }
            else
            {
                inf.Attributes = inf.Attributes & ~attr;
            }
        }
        private void SetAttributesToSubFile(FileSystemInfo inf)
        {
            SetOrDeleteAttr(archive.IsChecked ?? false, inf, FileAttributes.Archive);
            SetOrDeleteAttr(readOnly.IsChecked ?? false, inf, FileAttributes.ReadOnly);
            SetOrDeleteAttr(hidden.IsChecked ?? false, inf, FileAttributes.Hidden);
            SetOrDeleteAttr(system.IsChecked ?? false, inf, FileAttributes.System);
            if (folderFile.IsChecked != true || (inf.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
                return;

            foreach (var file in ((DirectoryInfo)inf).EnumerateFileSystemInfos())
            {
                SetAttributesToSubFile(file);
            }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            SetAttributesToSubFile(Info);
            DialogResult = true;
        }

        public static bool? ShowDialog(FileSystemInfo info)
        {
            return  new ChangeAttributes {Info = info}.ShowDialog();
        }
    }
}