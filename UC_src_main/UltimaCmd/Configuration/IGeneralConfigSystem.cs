using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JoyOs.BusinessLogic;
using System.Configuration;

namespace UltimaCmd.Configuration
{
    interface IGeneralConfigSystem : IConfigurationSystem, IApplicationSettingsProvider, IBaseInterface
    {
        string MainConfigFilePath { get; }
        string NetworkingConfigFilePath { get; }

        string BuildedAppTitle { get; }
    }
}
