using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace UcImageViewer
{
    /// <summary>
    /// Interaction logic for BigImageWindow.xaml
    /// </summary>
    public partial class BigImageWindow
    {
        bool    _keyDown;   // следит за состоянием мышки
        Point   _pt;                //последние координаты курсора
        Size    _size;                  //размер экрана
        int     _indexCurr;

        public BigImageWindow()
        {
            InitializeComponent();

            _size = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            var img2 = new Image
            {
                Source = NextImg(NextImage.Current),
                Width = _size.Width,
                Height = _size.Height
            };

            StackPanel1.Children.Add(img2);
        }

        public event EventHandler OnStop;

        public void OnOnStop(EventArgs e)
        {
            var handler = OnStop;
            if (_indexCurr != 0&&_indexCurr!=-1)
                UcImageViewer.ImageList.IndexOfCurImg = _indexCurr;
            if (handler != null) handler(this, e);
        }

        public static WrapPanel ImageList { get; set; }

        private static BitmapImage NextImg(NextImage e)
        {
            if (ImageList.Children.Count <= 0) return UcImageViewer.ImageList.DefImage;
            switch (e)
            {
                case NextImage.Right:
                    if (ImageList.Children.Count - 1 > UcImageViewer.ImageList.IndexOfCurImg)
                        UcImageViewer.ImageList.IndexOfCurImg++;
                    else
                        UcImageViewer.ImageList.IndexOfCurImg = 0;
                    break;
                case NextImage.Left:
                    if (0 < UcImageViewer.ImageList.IndexOfCurImg)
                        UcImageViewer.ImageList.IndexOfCurImg--;
                    else
                        UcImageViewer.ImageList.IndexOfCurImg = ImageList.Children.Count - 1;
                    break;
            }
            var str = ((Image)ImageList.Children[UcImageViewer.ImageList.IndexOfCurImg]).Tag.ToString();
            return new BitmapImage(new Uri(str, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Событие на нажатие клавиши над Stack Panel
        /// </summary>
        /// <param name="sender">StackPanle</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void StackPanelMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            _keyDown = true;
            _pt = e.GetPosition(sender as ScrollViewer);
        }

        /// <summary>
        /// Событие на отпускание мышки на Stack Panel
        /// </summary>
        /// <param name="sender">StackPanel</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void StackPanelMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (!_keyDown) return;

            _keyDown = false;
            if (ScrollViewer1.ScrollableWidth - _size.Width / 2 >= ScrollViewer1.HorizontalOffset &&
                _size.Width / 2 <= ScrollViewer1.HorizontalOffset)
            {
                ScrollViewer1.ScrollToHorizontalOffset(_size.Width);
                return;
            }
            if (_size.Width / 2 > ScrollViewer1.HorizontalOffset)
            {
                ScrollViewer1.ScrollToHorizontalOffset(0);
                AddAndRemoveImage(ScrollViewer1);
            }
            else
            {
                ScrollViewer1.ScrollToHorizontalOffset(ScrollViewer1.ScrollableWidth);
                AddAndRemoveImage(ScrollViewer1);
            }
        }

        /// <summary>
        /// Добавляет изображение в StackPanel с одной стороны и удоляет изображение с другой
        ///</summary>
        ///<remarks>Следит что бы было только 3 изображения в StackPanel</remarks>
        /// <param name="panel">ScrollViewer</param>
        public void AddAndRemoveImage(ScrollViewer panel)
        {
            var someWidth = panel.HorizontalOffset; //текущее состояние прокрутки
            Image img;
            if (Math.Abs(someWidth - panel.ScrollableWidth) < 0.001) //ScrollableWidth - максимально возможная прокрутка
            {
                img = new Image
                {
                    Width = _size.Width,
                    Height = _size.Height,
                    Source = NextImg(NextImage.Right)
                };
                StackPanel1.Children.Add(img);
                if (StackPanel1.Children.Count > 3)
                    StackPanel1.Children.RemoveAt(0);
                panel.ScrollToHorizontalOffset(panel.ScrollableWidth - img.Width);
            }
            if(StackPanel1.Children.Count > 1)
                _indexCurr = ImageList.Children.IndexOf(StackPanel1.Children[1]);
            if (Math.Abs(someWidth) > 0.001) return;
            img = new Image
            {
                Width = _size.Width,
                Height = _size.Height,
                Source = NextImg(NextImage.Left)
            };
            StackPanel1.Children.Insert(0, img);
            if (StackPanel1.Children.Count > 3)
                StackPanel1.Children.RemoveAt(StackPanel1.Children.Count - 1);

            panel.ScrollToHorizontalOffset(0 + img.Width);
            if (StackPanel1.Children.Count > 1)
                _indexCurr = ImageList.Children.IndexOf(StackPanel1.Children[1]);
        }

        /// <summary>
        /// Событие срабатывающее кадый раз как мышка двигается
        /// </summary>
        /// <param name="sender">ScrollViewer</param>
        /// <param name="e">MouseEventArgs</param>
        private void ScrollViewerMouseMove(object sender, MouseEventArgs e)
        {
            if (!_keyDown) return;
            var scroll = (ScrollViewer)sender;
            var curPos = e.GetPosition(scroll);
            var someWidth = scroll.HorizontalOffset;

            scroll.ScrollToHorizontalOffset(someWidth + _pt.X - curPos.X);
            _pt = curPos;
        }

        /// <summary>
        /// Событие на нажатие клавиш на клавиатуре
        /// </summary>
        /// <remarks>обрабатывает : вправо, влево, выход и автопроигрывание</remarks>
        /// <param name="sender">ScrollViewer</param>
        /// <param name="e">KeyEventArgs</param>
        private void ScrollViewerKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    ScrollViewer1.ScrollToRightEnd();
                    break;
                case Key.Left:
                    ScrollViewer1.ScrollToLeftEnd();
                    break;
                case Key.Enter:
                case Key.Escape:
                    OnOnStop(new EventArgs());
                    Close();
                    break;
                case Key.Space:
                    break;
            }
        }

        /// <summary>
        /// Событие на каждое изменение прокрутки в ScrollViewer
        /// </summary>
        /// <param name="sender">ScrollViewer</param>
        /// <param name="e">ScrollChangedEventArgs</param>
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            AddAndRemoveImage(sender as ScrollViewer);
        }

        #region Nested type: NextImage

        /// <summary>
        /// Функция возвращающая следующую по списку картинку
        /// </summary>
        /// <returns>BitmapImage</returns>
        private enum NextImage
        {
            Right,
            Left,
            Current
        }

        #endregion
    }
}