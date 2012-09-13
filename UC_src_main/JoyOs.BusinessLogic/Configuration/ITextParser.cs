using System;

namespace JoyOs.BusinessLogic.Configuration
{
	public interface ITextParser : IBaseInterface, IDisposable
	{
		bool Init();
		bool Close();

		IAsyncResult BeginParseRead();
		IAsyncResult EndParseRead();

		IAsyncResult BeginParseWrite(IAsyncResult asyncResult);
		IAsyncResult EndParseWrite(IAsyncResult asyncResult);

		bool 	ParseCommand( int iIndex );

		String 	CheckCommand( String command, out String value );
		void	    RemoveParm( String parm );
		void	    AppendParm( String parm, String values );

		// Returns the argument after the one specified, or the default if not found
		long	ParmValue( String sz, long nDefaultVal );
		float	ParmValue( String sz, float flDefaultVal );

		// Gets at particular parameters
		int	    ParmCount();
		int	    FindParm( String parm );
		string 	GetParm( int nIndex );

		void 	Dispose( bool disposing );

		float 	WriteTimeout{ get; set; }
		float 	ReadTimeout{ get; set; }
	}
} 