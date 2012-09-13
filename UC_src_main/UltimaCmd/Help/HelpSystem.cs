using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using JoyOs.BusinessLogic.Utils;
using UltimaCmd.Configuration;

namespace UltimaCmd.Help
{
    class HelpSystem : IHelpSystem
    {
        private IGeneralConfigSystem _generalConfig;

        private IGeneralConfigSystem GeneralConfig
        {
            get { return _generalConfig ?? (_generalConfig = new GeneralConfigSystem()); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException( "value", "Конфигурaционная система не может быть неопределенной" );
            }
        }

        private SessionFlags _sessionFlags;

        public HelpSystem()
            : this(SessionFlags.LocalHelpSession)
        {
        }

        public HelpSystem(SessionFlags session)
        {
            if (!Enum.IsDefined( typeof( SessionFlags ), session ))
            {
                throw new ArgumentException(
                    string.Format(
                        "Неправильный тип перечисления {0} - {1}",
                        session.GetType().ToString(), (int)session
                        )
                );
            }

            _sessionFlags = session;
        }

        public HelpSystem(SessionFlags session, IGeneralConfigSystem genConfig)
            : this( session )
        {
            GeneralConfig = genConfig;
        }

        #region Члены IHelpSystem

        public void ShowGeneralHelpContent()
        {
            throw new NotImplementedException();
        }

        public bool LookUpForHelpContent
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsIndexFound(long index)
        {
            throw new NotImplementedException();
        }

        public bool IsPageFound(long page)
        {
            throw new NotImplementedException();
        }

        public bool IsInternetNeeded
        {
            get { throw new NotImplementedException(); }
        }

        public void ShowAboutBox()
        {
            MessageBox.Show(
                     string.Format(
                         "{0}, версия {1} {2} (TODO: add Date)\n\n{3}\n\nРусский перевод: Димыч\n\nЗарегистрировано на:\nUltima Belarus c.\n08-017 Minsk (BY)\n\nNo User licence\n\n{4}\n\nINI-Files:\n{5}\n{6}",
                         AssemblyInfo.AssemblyProduct,
                         AssemblyInfo.AssemblyVersion,
                         AssemblyInfo.ProcessorArchitecture,
                         AssemblyInfo.AssemblyCopyright,
                         AssemblyInfo.AssemblyInformationVersion,
                         GeneralConfig.MainConfigFilePath,
                         GeneralConfig.NetworkingConfigFilePath
                     ),
                     "О программе " + AssemblyInfo.AssemblyProduct,
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                 );
        }

        #endregion

        #region Члены IHelpService

        public void AddContextAttribute(string name, string value, System.ComponentModel.Design.HelpKeywordType keywordType)
        {
            throw new NotImplementedException();
        }

        public void ClearContextAttributes()
        {
            throw new NotImplementedException();
        }

        public System.ComponentModel.Design.IHelpService CreateLocalContext(System.ComponentModel.Design.HelpContextType contextType)
        {
            throw new NotImplementedException();
        }

        public void RemoveContextAttribute(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void RemoveLocalContext(System.ComponentModel.Design.IHelpService localContext)
        {
            throw new NotImplementedException();
        }

        public void ShowHelpFromKeyword(string helpKeyword)
        {
            throw new NotImplementedException();
        }

        public void ShowHelpFromUrl(string helpUrl)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
