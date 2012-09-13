using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileManager
    {
        #region View Items Processing

        /// <summary>
        /// All "Files" menu events
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">RoutedEventArgs</param>
        private void MenuItemViewClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                //case "0": //По имени
                //    DataGridSorting(FocusedExplorer,new DataGridSortingEventArgs(FocusedExplorer.Columns[1]));
                //    break;
                //case "1": //По типу
                //    DataGridSorting(FocusedExplorer, new DataGridSortingEventArgs(FocusedExplorer.Columns[2]));
                //    break;
                //    //subMenu
                //case "2": //По дате/времени
                //    DataGridSorting(FocusedExplorer, new DataGridSortingEventArgs(FocusedExplorer.Columns[4]));
                //    break;
                //case "3": //По размеру
                //    DataGridSorting(FocusedExplorer, new DataGridSortingEventArgs(FocusedExplorer.Columns[3]));
                //    break;
                //case "4": //Без сортировки
                //    //FocusedExplorer.Items.Refresh();
                //    break;
            }
        }

        static void RefreshCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        public void RefreshOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            expRight.RefreshData();
            expLeft.RefreshData();
        }

        #endregion
    }
}