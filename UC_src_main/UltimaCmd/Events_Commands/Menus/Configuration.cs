using System.Windows;
using JoyOs.Windows;

namespace UltimaCmd
{
    public partial class MainWindow
    {
        // Settings item processing
        private void MenuItemSettingsClick(object sender, RoutedEventArgs e)
        {
            var settings = new Settings
                                            {
                                                Owner = this,
                                                ResizeMode = ResizeMode.CanMinimize
                                            };

            settings.ShowDialog();
        }
    }
}