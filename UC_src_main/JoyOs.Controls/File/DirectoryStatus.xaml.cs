using System.Globalization;

namespace JoyOs.Controls.File
{
    /// <summary>
    /// Interaction logic for DirectoryStatus.xaml
    /// </summary>
    public partial class DirectoryStatus
    {
        private static readonly NumberFormatInfo Nfi;
        static DirectoryStatus()
        {
            Nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            Nfi.NumberGroupSeparator = " ";
        }

        public DirectoryStatus()
        {
            InitializeComponent();
        }

        public long SelectedFiles { get; set; }
        public long TotalFiles { get; set; }

        public long SelectedDirectories { get; set; }
        public long TotalDirectories { get; set; }

        public long SelectedLength { get; set; }
        public long TotalLength { get; set; }

        public double Speed { get; set; }

        public void RefreshFiles()
        {
            files.Text = string.Format("файлов: {0} из {1}", SelectedFiles, TotalFiles);
        }
        public void RefreshDirectory()
        {
            folders.Text = string.Format("папок: {0} из {1}", SelectedDirectories, TotalDirectories);
        }
        public void RefreshLength()
        {
            size.Text = string.Format("{0} Кб из {1} Кб", SelectedLength.ToString("#,#;0;0"), TotalLength.ToString("#,#;0;0"));
        }
        public void RefreshSpeed()
        {
            speed.Text = string.Format("в/д: {0} сек", Speed.ToString("0.000"));
        }

        public void Clear()
        {
            SelectedFiles = TotalFiles = 
                                         SelectedDirectories =
                                         TotalDirectories =
                                         SelectedLength =
                                         TotalLength = 0;
            Speed = 0;
        }

        public void SetTotal(long totalFiles, long totalDirectories, long totalLength, double totalSpeed)
        {
            Clear();
            TotalLength = totalLength;
            TotalFiles = totalFiles;
            TotalDirectories = totalDirectories;
            Speed = totalSpeed;
        }

        public void RefreshAll()
        {
            RefreshFiles();
            RefreshDirectory();
            RefreshLength();
            RefreshSpeed();
        }
    }
}
