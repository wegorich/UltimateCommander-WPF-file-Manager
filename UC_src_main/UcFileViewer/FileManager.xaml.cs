using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JoyOs.BusinessLogic.Pluginable;
using JoyOs.Controls.File;
using JoyOs.FileSystem;
using JoyOs.FileSystem.FilesAPI;

namespace UcFileViewer
{
    /// <summary>
    /// Класс файлового мэнеджера
    /// </summary>
    public partial class FileManager : IFileSystemPlugin
    {
      /// <summary>
        /// Constructor - start the FileManager
        /// </summary>
        public FileManager()
        {
            InitializeComponent();
            
            // что делать если скрытые поменялись
            ScanSystem.OnHiddenChanged += ShowHidden;
            ScanSystem.OnSystemChanged += ShowHidden;

            // Меню
            Menu = mainMenu.Items.Cast<MenuItem>().ToArray();

            mainMenu.Items.Clear();
            mainDockPanel.Children.Remove(mainMenu);

            _localFileTools = new FileTools(_fileSystem);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            // TODO: read from history
            FocusedExplorer = expLeft;

            //димина коллекция вотчеров не использую т.к. пока работает некорректно
            //_fileSystem.Watchers.DefaultChanged += OnChanged;
            //_fileSystem.Watchers.DefaultCreated += OnChanged;
            //_fileSystem.Watchers.DefaultDeleted += OnChanged;
            //_fileSystem.Watchers.DefaultRenamed += OnRenamed;

            //expLeft.Tag = _fileSystem.Watchers.Add(UcCurrentDirectory);
            //expRight.Tag = _fileSystem.Watchers.Add(UcCurrentDirectory);

            var watch = new FileSystemWatcher();
            expLeft.Tag = watch;
            InitializeWatcher(watch, UcCurrentDirectory);
            
            watch = new FileSystemWatcher();
            expRight.Tag = watch;
            InitializeWatcher(watch, UcCurrentDirectory);
            
            expLeft.CurrentPath = UcCurrentDirectory;
            expRight.CurrentPath = UcCurrentDirectory;

           var cmdBindCollection = new CommandBindingCollection
                                        {
                                            new CommandBinding(
                                                NavigationCommands.Refresh,
                                                RefreshOnExecute,
                                                RefreshCanExecute
                                                ),
                                            new CommandBinding(
                                                ApplicationCommands.Copy,
                                                CopyOnExecute,
                                                CopyCanExecute
                                                ),
                                            new CommandBinding(
                                                ApplicationCommands.Delete,
                                                DeleteOnExecute,
                                                DeleteCanExecute
                                                )
                                        };

            //cmdBindCollection.Add(new CommandBinding(
            //                                                        ApplicationCommands.Cut,
            //                                                        CutCanExecute,
            //                                                        CutOnExecute
            //         


            expLeft.GridCommandBindings.AddRange(cmdBindCollection);
            expRight.GridCommandBindings.AddRange(cmdBindCollection);

            Application.Current.SessionEnding += ApplicationOnSessionEnding;
        }

        // UNDONE: Check exit from ImageViewer ?
        private void ApplicationOnSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            var result = MessageBoxResult.Yes;

            // Если мы что-то делаем
            if (_fileSystem.IsOperationActive)
            {
                result = MessageBox.Show(
                    "Внимание, активны несколько фоновых процессов.\n\nВы настаиваете на завершении?",
                    "Unreality Commander - Подсистема защиты",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    result
                    );

                //if (result == MessageBoxResult.OK)
                //    _fileSystem.TerminateAllJobs();
            }

            e.Cancel = (result == MessageBoxResult.Yes);
        }

        //UNDONE: Make Dep property
        private void ShowHidden(ScanFlagEventArgs e)
        {
            ScanPathAndRefresh(expLeft);
            ScanPathAndRefresh(expRight);
        }

        /// <summary>
        /// Грид выделен
        /// </summary>
        /// <param name="sender">ExplorerGrid</param>
        /// <param name="e">ничего</param>
        private void ExplorerGridSetFocusedExplorer(object sender, System.EventArgs e)
        {
            FocusedExplorer = (ExplorerGrid) sender;
        }

        /// <summary>
        /// Требование обновления содержимого
        /// </summary>
        /// <param name="sender">ExplorerGrid</param>
        /// <param name="e">ничего</param>
        private void ExplorerGridNeedsUpdateSource(object sender, System.EventArgs e)
        {
            ScanPathAndRefresh((ExplorerGrid)sender);
        }
    }
}
