using System;

namespace JoyOs.FileSystem.DriveScaner
{
    public class DriveEventArgs : EventArgs
    {
        public string DriveName { get; private set; }
        public int ListPosition { get; private set; }
        public DeviceAction Action { get; private set; }
        public DriveEventArgs(string driveName,int listPosition, DeviceAction action)
        {
            DriveName = driveName;
            ListPosition = listPosition;
            Action = action;
        }
    }
    public delegate void DriveEventHandler(object sender, DriveEventArgs e);
}
