using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using UltimaCmd.Help;
using UltimaCmd.Configuration;

using DataReader = JoyOs.BusinessLogic.Common.DataReader;

namespace UltimaCmd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Default start Directory
        /// </summary>
        private static string UcCurrentDirectory {get;set;}

        /// <summary>
        /// File`s tool box buttons
        /// </summary>
        private readonly List<string> _toolFilesList = new List<string>();

        private IHelpSystem HelpSystem
        {
            get { return new HelpSystem(); }
        }

        private IGeneralConfigSystem      _generalConfig;
        private IGeneralConfigSystem      GeneralConfig
        {
            get { return _generalConfig ?? (_generalConfig = new GeneralConfigSystem()); }
        }

        /// <summary>
        /// MainWindow`s constructor - start the Commander
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Title = GeneralConfig.BuildedAppTitle;

            UcCurrentDirectory = Environment.GetLogicalDrives()[0];//Environment.CurrentDirectory;

            OpenPluginsClick( dataToolBar, new RoutedEventArgs() );

            // TODO: Load and process from file
            CommandBindings.Add( new CommandBinding(
                                                            ApplicationCommands.Help,
                                                            HelpOnExecute ) );
        }

        private void ButtonLoaded(object sender, RoutedEventArgs e)
        {
            var data = DataReader.ReadDataFromFile("Default.BAR");

            if (data == null) return;
            _toolFilesList.AddRange(data);

            foreach (var t in data.Where(File.Exists))
            {
                AddToolBarButton(t);
            }
        }
    }
}