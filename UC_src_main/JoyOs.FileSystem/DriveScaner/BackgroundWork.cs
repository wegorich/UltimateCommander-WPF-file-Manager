using System.ComponentModel;
using System.Threading;

namespace JoyOs.FileSystem.DriveScaner
{
    /// <summary>
    /// Interaction logic for FileManager.xaml
    /// </summary>
    public static partial class DriveScaner
    {
        private static void DrivesScannerDoWork(object sender, DoWorkEventArgs e)
        {
            // This event handler is where the actual work is done.
            // This method runs on the background thread.

            // Get the BackgroundWorker object that raised this event.
            var worker = (BackgroundWorker)sender;

            // Get the FileManager object and call the main method.
            //var fileManager = (FileManager)e.Argument;
            UpdateDrivesThread(worker);
        }

        /// <summary>
        /// Add or remove drive`s in toolBar
        /// </summary>
        private static void UpdateDrivesThread(
                                BackgroundWorker drivesScanner)
        {
            do
            {
                // Получаем новые данные об логических дисках
                var updatedDrives = FileSystem.GetLogicalDrives();
                // Если произошли изменения
                if (LogicalDrives.Count != updatedDrives.Length)
                {
                    // Добавлено(ы) ...                    
                    if (LogicalDrives.Count < updatedDrives.Length)
                        for (var i = updatedDrives.Length - 1; i >= 0; i--)
                        {
                            var bIsFound = false;

                            for (var j = LogicalDrives.Count - 1; j >= 0; j--)
                            {
                                bIsFound = LogicalDrives[j] == updatedDrives[i];

                                if (bIsFound)
                                    break;
                            }

                            if (!bIsFound)
                            {
                                AddDrive(updatedDrives[i]);
                            }
                        }
                    else  // Если диск(и) удалены
                        for (var i = LogicalDrives.Count - 1; i >= 0; i--)
                        {
                            var bIsFound = false;

                            for (var j = updatedDrives.Length - 1; j >= 0; j--)
                            {
                                bIsFound = updatedDrives[j] == LogicalDrives[i];

                                if (bIsFound)
                                    break;
                            }

                            if (!bIsFound)
                                RemoveDrive(LogicalDrives[i]);
                        }
                }

                // How much time to wait
                // until start new drives search                    
                Thread.Sleep(300);
            } while (true);
        }
    }
}
