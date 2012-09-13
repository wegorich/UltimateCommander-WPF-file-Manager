using System.Windows;

namespace JoyOs.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for OneTextBox.xaml
    /// </summary>
    public partial class OneTextBox
    {
        /// <summary>
        /// Конструктор окна
        /// </summary>
        public OneTextBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Позволяет пользователю задать кол-во кнопок из перечисления
        /// MessageBoxButton
        /// </summary>
        public MessageBoxButton OneBoxButton
        {
            set
            {
                switch (value)
                {
                    case MessageBoxButton.OKCancel:
                    case MessageBoxButton.YesNo:
                        cancelButton.Visibility = Visibility.Visible;
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

        /// <summary>
        /// Позволяем задать и получить текст введенный пользователем
        /// </summary>
        public string Text
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }
    }
}