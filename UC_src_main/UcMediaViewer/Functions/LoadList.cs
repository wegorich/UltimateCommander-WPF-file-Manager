using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UcMediaViewer
{
    public partial class MediaViewer
    {
        public void LoadList()
        {
            list.Items.Clear();
            wrapList.Visibility = Visibility.Collapsed;
            list.Visibility = Visibility.Visible;
            foreach (var item in _sourceData)
            {
                var pan = new StackPanel {Orientation = Orientation.Horizontal,Tag=item.Path};
                pan.MouseLeftButtonUp += MediaItemClick;
                pan.Children.Add(new TextBlock {Foreground = Brushes.LightSlateGray, Text = item.Name, Margin=new Thickness(0,0,0,5)});
                pan.Children.Add(new TextBlock { Foreground = Brushes.DarkSalmon, Text = item.Type, Margin = new Thickness(0, 0, 0, 5) });
                pan.Children.Add(new TextBlock { Foreground = Brushes.DarkGray, Text = item.Length, Margin = new Thickness(5, 0, 0, 5) });
                pan.Children.Add(new TextBlock { Foreground = Brushes.LightGray, Text = item.Date, Margin = new Thickness(5, 0, 0, 5) });
                list.Items.Add(pan);
            }
        }
     }
}