using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace JoyOs.BusinessLogic.Pluginable
{
    // Plugins are Ultimate Commander extensions
    // written by ourselves or by third parties.
    //
    // Plugins enhance the functionality of Ultimate Commander.
    //
    // The plugins section on <TODO: Add name> also contains a
    // plugin writer's guide for each plugin type. There are also
    // sample plugins with full source code, most written in C#,
    // some in Visual C++ .NET.

	public interface IPluginable : IBaseInterface
	{
	    IEnumerable<MenuItem> Menu { get; set; }

        string SupportedExtensions { get; }

	    Version CurrentVersion { get; }
	    string FriendlyName { get; }
	}
} 