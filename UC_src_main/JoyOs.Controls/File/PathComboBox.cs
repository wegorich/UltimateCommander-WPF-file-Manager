using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JoyOs.BusinessLogic.Event;
using JoyOs.FileSystem;

namespace JoyOs.Controls.File
{
    public class PathComboBox : ComboBox
    {
        private int _oldSelLength;
        private int _oldSelStart;
        private string _oldText;
        private TextBox _textBox;
        private string _currentPath;
        private bool _textChangedByCode;
        private Thread _asyncThread;

        public bool Asynchronous { get; set; }

        public event ValueEventHandler PathChanged;

        public void OnPathChanged(ValueEventArgs e)
        {
            var handler = PathChanged;
            if (handler != null) handler(this, e);
        }

        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                if (PathDataProvider == null) return;
                _textChangedByCode = true;

                if (PathDataProvider.Exist(value,true))
                {
                    _textBox.Text = value;
                    _currentPath = value;
                }
                else
                {
                    _textBox.Text = _currentPath;
                }
                _textBox.SelectionStart = _textBox.Text.Length;
                _textChangedByCode = false;
            }
        }
        public IPathDataProvider PathDataProvider { get; set; }

        public PathComboBox()
        {
            IsEditable = true;
            GotMouseCapture += PathComboBoxGotMouseCapture;
        }

        private void PathComboBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            _oldSelStart = _textBox.SelectionStart;
            _oldSelLength = _textBox.SelectionLength;
            _oldText = _textBox.Text;
        }

        private void UpdateText()
        {
            _textBox.SelectionLength = 0;
            _textBox.SelectionStart = _textBox.Text.Length;

            var str = _currentPath;
            CurrentPath = _textBox.Text;
            if (str != CurrentPath)
                OnPathChanged(new ValueEventArgs(CurrentPath));
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    SelectedValue = Text;
                    IsDropDownOpen = true;
                    break;

                case Key.Enter:
                    IsDropDownOpen = false;
                    if (PathDataProvider.LegalPath(_textBox.Text))
                        UpdateText();
                    e.Handled = true;
                    break;

                case Key.Escape:
                    IsDropDownOpen = false;
                    UpdateText();
                    e.Handled = true;
                    break;

            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            IsTextSearchEnabled = true;
            SelectedValue = Text;

            base.OnDropDownOpened(e);

            if (SelectedValue != null) return;

            Text = _oldText;
            _textBox.SelectionStart = _oldSelStart;
            _textBox.SelectionLength = _oldSelLength;
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            IsTextSearchEnabled = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            if (_textBox == null) return;

            _textBox.TextChanged += TextBoxTextChanged;
            _textBox.LostFocus += TextBoxLostFocus;
        }

        #region TextBox Event
        private void TextBoxLostFocus(object sender,RoutedEventArgs e)
        {
            UpdateText();
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textChangedByCode || PathDataProvider == null || _textBox.Text.Length != _textBox.SelectionStart)
            {
                return;
            }

            var text = _textBox.Text;

            if (string.IsNullOrEmpty(text))
            {
                IsDropDownOpen = false;
                return;
            }
            if (Asynchronous)
            {
                if (_asyncThread != null && _asyncThread.IsAlive)
                {
                    _asyncThread.Abort();
                }
                _asyncThread = new Thread(() =>
                {
                    var items = PathDataProvider.GetItems(text);
                    var dispatcher = Application.Current.Dispatcher;
                    var currentText = dispatcher.Invoke(new Func<string>(() => _textBox.Text)).ToString();
                    if (text != currentText)
                    {
                        return;
                    }
                    dispatcher.Invoke(new Action(() => UpdateItemSource(items)));
                    IsDropDownOpen = Items.Count > 0;
                });
                _asyncThread.Start();
            }
            else
            {
                var items = PathDataProvider.GetItems(text);
                UpdateItemSource(items);
            }
        }

        private void UpdateItemSource(IEnumerable<string> items)
        {
            var text = _textBox.Text;

            ItemsSource = items;
            _textChangedByCode = true;
            _textBox.Text = text;
            _textBox.SelectionStart = _textBox.Text.Length;
            if (Items.Count > 1)
            {
                IsDropDownOpen = true;
            }
            _textChangedByCode = false;

        }
        #endregion
    }
}
