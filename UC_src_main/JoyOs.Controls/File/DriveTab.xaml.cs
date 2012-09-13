using System;
using System.Windows;
using System.Windows.Controls;
using JoyOs.Controls.Simple;
using JoyOs.FileSystem.DriveScaner;

namespace JoyOs.Controls.File
{
    /// <summary>
    /// Interaction logic for DriveTab.xaml
    /// </summary>
    public partial class DriveTab
    {
        public DriveTab()
        {
            InitializeComponent();
            foreach (var drive in DriveScaner.GetLogicalDrives())
            {
                AddDevice(drive);
            }
            DriveScaner.DriveChanged += DriveChanged;
        }

        private string _drive;

        /// <summary>
        /// Задает или получает выделенный диск (Без события)
        /// </summary>
        public string Drive
        {
            get { return _drive; }
            set
            {
                if (_drive == value || string.IsNullOrEmpty(value)) return;
                value = value.ToUpper();
                var drives = DriveScaner.GetLogicalDrives();


                for (var i = 0; i < drives.Count;i++ )
                {
                    if (drives[i] != value) continue;
                    driveTab.SelectedIndex =i;
                    _drive = value;
                    break;
                }
            }
        }

        /// <summary>
        /// Число табов
        /// </summary>
        public int Count { get { return driveTab.Items.Count; } }

        /// <summary>
        /// Обновляет коллекцию табов в соответстивии с Drive Scaner
        /// </summary>
        /// <param name="sender">DriveEventArgs</param>
        /// <param name="e">DriveScaner - не используется</param>
        private void DriveChanged(object sender, DriveEventArgs e)
        {
            switch (e.Action)
            {
                case DeviceAction.AddDevice:
                    Dispatcher.Invoke(new Action(() => AddDevice(e.DriveName)), null);
                    break;
                case DeviceAction.RemoveDevice:
                    Dispatcher.Invoke(new Action(() => RemoveDevice(e.ListPosition)), null);
                    break;
            }
        }

        #region Событие перехода по табам
        public event DriveEventHandler DriveSelected;

        public void OnDriveSelected(DriveEventArgs e)
        {
            var handler = DriveSelected;
            if (handler != null) handler(this, e);
        }

        private void HardDriveTabItemClick(object sender, RoutedEventArgs e)
        {
            var tab = (TabItem)sender;
            _drive = tab.Tag.ToString();
            OnDriveSelected(new DriveEventArgs(_drive, 0, DeviceAction.RefreshDevice));
        }
        #endregion

        #region Добавление / Удаление табов
        private void AddDevice(string drive)
        {
            var tab = DriveTabItem.Create(drive);
            tab.PreviewMouseLeftButtonUp += HardDriveTabItemClick;
            driveTab.Items.Add(tab);
        }

        private void RemoveDevice(int i)
        {
            driveTab.Items.RemoveAt(i);
        }
        #endregion
    }
}