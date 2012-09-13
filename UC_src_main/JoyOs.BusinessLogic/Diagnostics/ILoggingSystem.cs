namespace JoyOs.BusinessLogic.Diagnostics
{
    public enum LoggingResponsePolicy
    {

    }

    public delegate void LoggingListener(params object[] args);

    public interface ILoggingSystem : IBaseInterface
    {
        // Init / Shutdown
        bool Init();
        bool Shutdown();

        //	Find channel, false if not exists or invalid pointer ;) 
        bool FindChannel(ILoggingChannel hilchannel);

        //	Gets channel on its ID, NULL if no this ID
        ILoggingChannel this[ulong ulChannelId] { get; set; }

        // Num of channels, 0 if no registered ch.
        ulong ChannelCount { get; }

        bool FastLog(ulong channelId, DebuggerType level,
                                            string format, params object[] args);

        long FirstChannelId { get; }
        long NextChannelId { get; }

        bool RegisterLoggingChannel(ILoggingChannel loggingChannel);
        bool RegisterLoggingListener(LoggingListener loggingListener);
        void ResetCurrentLoggingState();

        // bool		SetChannelSpewLevelByName( string Name );
        bool SetChannelSpewLevelByTag(object objTag);
        LoggingResponsePolicy LoggingResponsePolicy { get; set; }
    }
}
