using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JoyOs.FileSystem.FilesAPI;
using JoyOs.FileSystem.Model;

using ChangeAttributes = JoyOs.Windows.Dialogs.Files.ChangeAttributes;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileManager
    {
        readonly FileTools _localFileTools;

        #region Archive / Broke / Add Operations

        void FilesArchiveOptionsClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "Pack":
                    break;

                case "Unpack":
                    break;

                case "Test":
                    break;

                case "Break":
                    break;

                case "Make":
                    break;
            }
        }

        #endregion

        #region Code / Decode / CRC Operations

        void FilesCryptoOptionsClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "Code":
                    break;

                case "Decode":
                    break;

                case "CRC_get":
                    break;

                case "CRC_check":
                    break;

                case "Add":
                    break;
            }
        }

        #endregion

        #region Additional Operations

        void FilesAdditionalClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "Attributes":
                    var itemToAttribute = FocusedExplorer.SelectedItem as ILogicItem;

                    if (itemToAttribute != null) ChangeAttributes.ShowDialog(itemToAttribute.Info);
                    break;

                case "Compare":
                    break;

                case "OpenWith":
                    var itemToOpenWith = FocusedExplorer.SelectedItem as ViewDataItem;

                    // UNDONE: На папке не вызывать?
              FileTools.OpenWithDialog(itemToOpenWith);
                    break;

                case "Assoc":

                    //import IContextMenu declare using

                    //[ComImport]
                    //[Guid("000214E4-0000-0000-C000-000000000046")]
                    //public interface IContextMenu
                    //{
                    //    [PreserveSig]
                    //    int QueryContextMenu(uint hMenu,uint indexMenu,int idCmdFirst,int
                    //idCmdLast,uint uFlags);
                    //    [PreserveSig]
                    //    void InvokeCommand(IntPtr pici);
                    //    [PreserveSig()]
                    //    void GetCommandString(int idcmd,uint uflags,int
                    //reserved,StringBuilder commandstring,int cch);
                    //}

                    //and implents in my plugin.GD will CRASH even if i just return 1 in
                    //QueryContextMenu ()

                    break;

                case "Properties":
                    break;

                case "Place":
                    MessageBox.Show(
                            _localFileTools.CalculateDataLength(
                                    FocusedExplorer.SelectedItems.Cast<ILogicItem>()
                                    ),
                            "Ultimate Commander - Подсистема доступа",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    break;

                case "GroupRename":
                    break;

                case "Comments":
                    break;

                case "Print":
                    break;
            }
        }

        #endregion

        #region Quit Operation

        void FilesQuitClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}