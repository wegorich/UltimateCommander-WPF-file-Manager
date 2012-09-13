using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UcMediaViewer
{
    public partial class MediaViewer
    {
        public void WrapList()
        {
            imgWrapList.Children.Clear();
            wrapList.Visibility = Visibility.Visible;
            list.Visibility = Visibility.Collapsed;
            foreach (var item in _sourceData)
            {
                var pan = new StackPanel { Tag = item.Path,Background = Brushes.LightSkyBlue, MaxHeight = SizeImage, MaxWidth = SizeImage, Margin = new Thickness(3) };
                pan.MouseEnter += OnStartHover;
                pan.MouseLeave += OnEndHover;
                pan.MouseLeftButtonUp += MediaItemClick;
                pan.Children.Add(new Image { Width=32,Height=32, Source=item.Image,Margin = new Thickness(3) });
                pan.Children.Add(new TextBlock { Foreground = Brushes.White, Text = item.Name, Margin = new Thickness(0, 0, 0, 5) });
                pan.Children.Add(new TextBlock { Foreground = Brushes.White, Text = item.Length, Margin = new Thickness(0, 0, 0, 5) });
                imgWrapList.Children.Add(pan);
            }
        }
    }
}