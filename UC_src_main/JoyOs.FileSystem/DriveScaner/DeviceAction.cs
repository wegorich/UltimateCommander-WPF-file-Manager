
namespace JoyOs.FileSystem.DriveScaner
{
    /// <summary>
    /// Enum - provide actions fore Drive menu ( threat )
    /// </summary>
    public enum DeviceAction
    {
        /// <summary>
        /// say to add new Device
        /// </summary>
        AddDevice = 0,

        /// <summary>
        /// say that the Device gone
        /// </summary>
        RemoveDevice,

        /// <summary>
        /// say to sort Device ( not releazed )
        /// </summary>
        RefreshDevice
    }
}