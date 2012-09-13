using System;

namespace JoyOs.BusinessLogic.Diagnostics
{
    [Flags]
    public enum LoggingChannelFlags : byte
    {
        No = 0x00
    }

    public enum LoggingState
    {
        Enabled = 0,
        Disabled,
        Unknown
    }

    public interface ILoggingChannel : IBaseInterface
    {
        bool				InitOk { get; }
		bool				Shutdown( );

        bool            IsEnabled { get; set; }

	    bool				HasTag { get; }
        object         Tag { get; set; }

        DebuggerType LoggingLevel { get; set; }

        LoggingState PopLoggingState();
        void PushLoggingState(LoggingState loggingState);

	    // Log functions
        void Log(DebuggerType level, string format, params object[] args);
        void LogAssert(DebuggerType level, string format, params object[] args);

	    // Direct to log device
	    void	LogDirect( string format, params object[] args );

	    // Color of channel ( app console output ? )
	    long	ChannelColor { get; set; }

	    // Channel flags control
        LoggingChannelFlags ChannelFlags { get; set; }
    }
}
