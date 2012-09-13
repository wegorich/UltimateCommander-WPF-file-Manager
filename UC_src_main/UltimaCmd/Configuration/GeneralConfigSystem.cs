using System;
using System.IO;
using JoyOs.BusinessLogic.Utils;

namespace UltimaCmd.Configuration
{
    class GeneralConfigSystem : IGeneralConfigSystem
    {
        private static readonly string _mainConfigFile = "ULTIMATE" + Path.DirectorySeparatorChar + "utmcmd.ini";
        private static readonly string _networkingConfigFile = "ULTIMATE" + Path.DirectorySeparatorChar + "ucx_ftp.ini";

        private string BaseConfigPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );

        #region Члены IGeneralConfigSystem

        public string MainConfigFilePath
        {
            get { return Path.Combine( BaseConfigPath, _mainConfigFile ); }
        }

        public string NetworkingConfigFilePath
        {
            get { return Path.Combine( BaseConfigPath, _networkingConfigFile ); }
        }

        public string BuildedAppTitle
        {
            get 
            {
                return string.Format(
                            "Ultimate Commander ({0}) {1} - {2}",
                            AssemblyInfo.ProcessorArchitecture,
                            AssemblyInfo.AssemblyVersion,
                            AssemblyInfo.AssemblyCompany
                        );
            }
        }

        #endregion

        #region Члены IConfigurationSystem

        public object GetConfig(string configKey)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Члены IApplicationSettingsProvider

        public System.Configuration.SettingsPropertyValue GetPreviousVersion(System.Configuration.SettingsContext context, System.Configuration.SettingsProperty property)
        {
            throw new NotImplementedException();
        }

        public void Reset(System.Configuration.SettingsContext context)
        {
            throw new NotImplementedException();
        }

        public void Upgrade(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyCollection properties)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
