using System.Windows;
using System.Windows.Controls;

namespace UcImageViewer
{
    public partial class ImageList
    {
        private void ToolsItemCheck(object sender, RoutedEventArgs e)
        {
            // может быть нулем
            var item = sender as MenuItem;
            if (item == null||item.Tag==null) return;

            var visibility = !item.IsChecked ?
                Visibility.Collapsed : Visibility.Visible;
            switch (item.Tag.ToString())
            {
                case "List":
                    if (visibility == Visibility.Collapsed)
                    {
                        splitDef.Width =new GridLength(0);
                        listDef.MinWidth = 0;
                        imgDef.Width = GridLength.Auto;
                    }
                    else
                    {
                        listDef.MinWidth = 100;
                        if (imgGrid.Visibility == Visibility.Visible)
                        {
                            splitDef.Width = GridLength.Auto;
                            imgDef.Width = new GridLength(80, GridUnitType.Star);
                        }
                    }
                    imgList.Visibility = visibility;
                    break;
                case "Image":
                    if (visibility == Visibility.Collapsed)
                    {
                        splitDef.Width = new GridLength(0);
                        imgDef.MinWidth = 0;
                        imgDef.Width = GridLength.Auto;
                    }
                    else
                    {
                        imgDef.MinWidth = 100;
                        if (imgList.Visibility == Visibility.Visible)
                        {
                            splitDef.Width = GridLength.Auto;
                            imgDef.Width = new GridLength(80, GridUnitType.Star);
                        }
                    }
                    imgGrid.Visibility = visibility;
                    break;
                case "StatusBar":
                    statusBar.Visibility = visibility;
                    break;
                case "TabPanel":
                    tabControl.Visibility = visibility;
                    break;
            }
        }

        //private void ToolsItemClick(object sender, RoutedEventArgs e)
        //{
        //    var item = (MenuItem)sender;
        //    item.IsChecked = !item.IsChecked;
        //}
    }
}