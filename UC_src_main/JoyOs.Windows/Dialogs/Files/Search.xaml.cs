using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace JoyOs.Windows.Dialogs.Files
{
    /// <summary>
    /// Interaction logic for ChangeAttributes.xaml
    /// </summary>
    public partial class Search
    {
        private static Search _search;
        
        public SearchOption Option { get; private set; }

        public string Text
        {
            get { return searchText.Text; }
        }

        public event RoutedEventHandler Click;

        public void OnClick(RoutedEventArgs e)
        {
            var handler = Click;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Конструктор Диалога
        /// </summary>
        private Search()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            Option = SearchOption.TopDirectoryOnly;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            base.OnClosing(e);
        }
        public static Search Create()
        {
            if (_search == null)
                _search = new Search();
            _search.WindowState = WindowState.Normal;
            _search.Click = null;
            return _search;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            OnClick(e);
        }

        private void TypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Option = (SearchOption)((ComboBox)sender).SelectedIndex;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            OnClosing(new System.ComponentModel.CancelEventArgs());
        }
    }
}