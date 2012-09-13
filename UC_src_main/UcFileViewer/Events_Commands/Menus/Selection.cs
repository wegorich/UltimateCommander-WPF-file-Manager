using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JoyOs.FileSystem.Model;
using JoyOs.FileSystem.Selection;

namespace UcFileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileManager
    {
        #region Selection Item Processing

        private readonly SelectionControl<ILogicItem> _selectionControl = new SelectionControl<ILogicItem>();

        private void MenuItemSelectEntriesClick(object sender, RoutedEventArgs e)
        {
            SelectionControl<ILogicItem>.SelectionOptions so;

            if (!Enum.TryParse(
                ((MenuItem)sender).Tag.ToString(), false, out so)) return;

            if (so == SelectionControl<ILogicItem>.SelectionOptions.SaveToFile
                && FocusedExplorer.SelectedItems == null)
            {
                MessageBox.Show(
                    "Ничего не выделено",
                    "Ultimate Commander - Подсистема выделения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
                return;
            }

            _selectionControl.SelectEntries(
                    FocusedExplorer.SelectedItems,
                    FocusedExplorer.ItemsSource.Cast<ILogicItem>(),
                    so);
        }

        private void MenuItemSelectGroupClick(object sender, RoutedEventArgs e)
        {
            bool isSelect;

            if (bool.TryParse(((MenuItem)sender).Tag.ToString(), out isSelect))
            {
                _selectionControl.SelectGroup("*.*", null, isSelect);
            }
        }

        private void MenuItemSelectCopyClick(object sender, RoutedEventArgs e)
        {
            SelectionControl<ILogicItem>.CopyPathsToClipBoardOptions cpcbo;

            if (Enum.TryParse(
                ((MenuItem)sender).Tag.ToString(), false, out cpcbo))
            {
                _selectionControl.CopyPathsToClipBoard(
                    expLeft.SelectedItems.Cast<ViewDataItem>(),
                    expRight.SelectedItems.Cast<ViewDataItem>(),
                    cpcbo);
            }
        }

        private void MenuItemSelectCompareClick(object sender, RoutedEventArgs e)
        {
             // Если пытаемся сравнить один и тот же каталог
            if (expLeft.CurrentPath == expRight.CurrentPath)
                // Не хотим сравнивать -> выходим ...
                if (MessageBoxResult.Cancel == MessageBox.Show(
                    string.Format(
                        "Внимание: На сравниваемых панелях - один и тот же каталог:\n( {0} ).\n\nВсе равно сравнить?",
                        expLeft.CurrentPath
                        ),
                    "Ultimate Commander - Подсистема сравнения файлов",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question
                                                   ))
                    return;

            SelectionControl<ILogicItem>.CompareCatalogsOptions cco;

            if (!Enum.TryParse(
                ((MenuItem) sender).Tag.ToString(), false, out cco)) return;

            // Сравниваем каталоги
            var compareResult = _selectionControl.CompareCatalogs(
                expLeft.SelectedItems.Cast<ILogicItem>(),
                expRight.SelectedItems.Cast<ILogicItem>(),
                cco);

            switch (compareResult)
            {
                case 0:
                    MessageBox.Show(
                        "Файлы на источнике и получателе выглядят одинаково",
                        "Ultimate Commander - Подсистема сравнения файлов",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                        );
                    break;

                case -1:
                    break;

                case 1:
                    break;
            }
        }
        
        #endregion
    }
}