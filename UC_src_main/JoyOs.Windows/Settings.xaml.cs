using System.Windows;
using JoyOs.FileSystem;

namespace JoyOs.Windows
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// Конструктор Диалога
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            hidden.IsChecked = ScanSystem.ShowHidden;
            system.IsChecked = ScanSystem.ShowSystem;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
           DialogResult = true;
        }

        private void CheckBoxHidden(object sender, RoutedEventArgs e)
        {
            ScanSystem.ShowHidden = hidden.IsChecked ?? false;
        }

        private void CheckBoxSystem(object sender, RoutedEventArgs e)
        {
            ScanSystem.ShowSystem = system.IsChecked ?? false;
        }
    }
}
