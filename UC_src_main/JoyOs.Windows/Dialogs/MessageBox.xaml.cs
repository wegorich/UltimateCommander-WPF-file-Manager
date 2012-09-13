using System.Windows;

namespace JoyOs.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox
    {
        /// <summary>
        /// Конструктор Диалога
        /// </summary>
        private MessageBox()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

        }
        /// <summary>
        /// Позволяет пользователю задать кол-во кнопок из перечисления
        /// MessageBoxButton
        /// </summary>
        public MessageBoxButton BoxButton
        {
            set
            {
                switch (value)
                {
                    case MessageBoxButton.OK: okButton.IsDefault = true;
                        break;
                    case MessageBoxButton.OKCancel:
                    case MessageBoxButton.YesNo:
                        noButton.Visibility = Visibility.Visible;
                        noButton.IsDefault = true;
                        break;
                    case MessageBoxButton.YesNoCancel:
                        noButton.Visibility = cancelButton.Visibility = Visibility.Visible;
                        cancelButton.IsDefault = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Позволяет задать статический текст
        /// </summary>
        public string StaticText
        {
            get { return staticText.Text; }
            set { staticText.Text = value; }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        /// <param name="btn">The button.</param>
        public static bool? ShowDialog(string text, string title, MessageBoxButton btn)
        {
            return new MessageBox {StaticText = text, Title = title, BoxButton = btn}.ShowDialog();
        }
    }
}
