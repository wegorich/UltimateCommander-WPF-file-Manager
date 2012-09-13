//
//	Abstract:
//		Debug System public API Definions
//

namespace JoyOs.BusinessLogic.Diagnostics
{
    /// <summary>
    /// Debugger message types
    /// </summary>
    public enum DebuggerType
    {
	    Message = 0,
	    Warning,
	    Assert,
	    Error,
	    Log,

	    TypeCount
    }

    /// <summary>
    /// Debugger return values
    /// </summary>
    public enum DebuggerRetval
    {
	    DebugDebugger = 0,
	    DebuggerContinue,
	    DebuggerAbort
    }

    // Debugger Output Delegate
    public delegate DebuggerRetval DebuggerOutputFunc ( DebuggerType spewType, string message );
    // Recovery debug out Delegate
    public delegate ulong RecoveryOutputFunc ( object objContext );
    // Unhandled exception Filter Delegate
    public delegate void UnhandledOutputFunc ( object objContext );

    // Default Debugger Function
    // static DebuggerRetval DefaultDebuggerFunc( DebuggerType type, string Message );
    // Default Recovery Function
    // static ulong DefaultRecoveryFunc( object objContext );

    // NOTE: HDebuggingSystem() should be called AFTER HloggingSystem Initialization !!!

    /// <summary>
    /// Debugging system interface
    /// </summary>
    public interface IDebuggingSystem : IBaseInterface
    {
        bool Init( );
	    bool Shutdown( );

        event DebuggerOutputFunc DebuggerOutputFunc;
	    void DebuggerInfo( DebuggerType type, string file, ulong line );

	    /* Used to manage spew groups and subgroups */
	    void DebuggerActivate( string groupName, byte level );
	    bool IsDebuggerActive( string groupName, byte level );

        event UnhandledOutputFunc UnhandledExceptionsFilter;
        bool IsUeFilterActive { get; }

	    long  RegisterApplicationRestart( string restartCommandLine, ulong flags = 0 );
	    long  UnregisterApplicationRestart( );

	    long  RegisterApplicationRecovery( string recoveryCommandLine, ulong flags = 0 );
	    long  UnregisterApplicationRecovery( );

        event RecoveryOutputFunc RecoveryOutputFunc;

	    /* A couple of super-common dynamic spew messages, here for convenience */
	    /* These looked at the "developer" group */
	    void DevMsg( byte level, string message, params object[] arg );
	    void DevWarning( byte level, string warning, params object[] arg );
	    void DevLog( byte level, string warning, params object[] arg );

	    /* These are always compiled in */
	    void Msg( string message, params object[] arg );
	    void DMsg( string groupName, byte level, string dmessage, params object[] arg );

	    void Warning( string warning, params object[] arg );
	    void DWarning( string groupName, byte level, string dwarning, params object[] arg );

	    void Log( string log, params object[] arg );
	    void DLog( string groupName, byte level, string dlog, params object[] arg );

	    void Error( string error, params object[] arg );
	    DebuggerRetval DAssert( string assertMessage, params object[] arg );
    }

    // DEBUGGING_SYSTEM_INTERFACE IDebuggingSystem * HDebuggingSystem( ); 
}
