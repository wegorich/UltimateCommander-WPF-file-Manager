using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UcMediaViewer
{
    public partial class MediaViewer
    {
        #region Image Hover

        //анимация при наведении
        protected void OnStartHover(object sender, MouseEventArgs e)
        {
            var stackPanel = (StackPanel)sender;

            stackPanel.Margin = new Thickness(0);
            stackPanel.MaxHeight += 6;
            stackPanel.MaxWidth += 6;
        }

        #endregion

        #region Image End Hover

        //анимация при окончании наведения
        protected void OnEndHover(object sender, MouseEventArgs e)
        {
            var stackPanel = (StackPanel)sender;
            stackPanel.Margin = new Thickness(3);
            stackPanel.MaxHeight -= 6;
            stackPanel.MaxWidth -= 6;
        }

        #endregion      
    }
}