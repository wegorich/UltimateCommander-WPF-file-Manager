using System.Windows;
using System.Windows.Controls;

namespace UcMediaViewer
{
    public partial class MediaViewer
    {
        #region Size Changed

        private void PanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (WrapPanel)sender;
            var size = e.NewSize;
            var count = (int)(size.Width / (DefSize + 6));

            SizeImage = (size.Width / count) - 6;

            for (int i = 0; i < panel.Children.Count; i++)
            {
                var img = (StackPanel)panel.Children[i];
                img.MaxWidth = SizeImage;
                img.MaxHeight = SizeImage * 0.8;
            }
        }

        #endregion
    }
}