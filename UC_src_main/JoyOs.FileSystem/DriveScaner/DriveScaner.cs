using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace JoyOs.FileSystem.DriveScaner
{
    public static partial class DriveScaner
    {
        public static IFileSystem FileSystem;

        private static readonly BackgroundWorker BgDrivesFinder;
        private static readonly List<string> LogicalDrives;

        public static List<string> GetLogicalDrives()
        {
            return LogicalDrives;
        }

        static DriveScaner()
        {
            FileSystem = new UFileSystem();
            LogicalDrives = new List<string>(FileSystem.GetLogicalDrives());
            BgDrivesFinder = new BackgroundWorker();

            BgDrivesFinder.DoWork += DrivesScannerDoWork;
            BgDrivesFinder.RunWorkerAsync();
        }

        public static event DriveEventHandler DriveChanged;

        public static void OnDriveChanged(DriveEventArgs e)
        {
            var handler = DriveChanged;
            if (handler != null) handler(null, e);
        }

        public static void AddDrive(string drive)
        {
            LogicalDrives.Add(drive);
            OnDriveChanged(new DriveEventArgs(drive, LogicalDrives.Count-1, DeviceAction.AddDevice));
        }

        public static void RemoveDrive(string drive)
        {
            OnDriveChanged(new DriveEventArgs(drive, LogicalDrives.IndexOf(drive), DeviceAction.RemoveDevice));

            LogicalDrives.Remove(drive);
            
        }
    }
}
