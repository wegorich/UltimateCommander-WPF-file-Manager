using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JoyOs.BusinessLogic.Utils;
using JoyOs.FileSystem;
using JoyOs.FileSystem.Functionality.Icon;

namespace JoyOs.Controls.Simple
{
    /// <summary>
    /// Interaction logic for DriveTabItem.xaml
    /// </summary>
    public class DriveTabItem : TabItem
    {
        private readonly string _text;
        private readonly ImageSource _img;
        public static IFileSystem FileSystem = new UFileSystem();

        private DriveTabItem(object toolTip,object tag)
        {
            ToolTip = toolTip;
            Tag = tag;
        }

        public static DriveTabItem Create(string drive)
        {
            var driveInfo = FileSystem.DriveInformation(drive);
            var source = IconExtractor.GetDriveIcon(driveInfo.DriveType);

            var toolTipText = (driveInfo.IsReady
                                    ? string.Format("{0}{1}\nСвободно: {2} КБ" +
                                                                              "\nВсего:         {3} КБ",
                                                                        driveInfo.VolumeLabel,
                                                                        driveInfo.DriveFormat,
                                                                        LengthFormatter.LengthFormat(driveInfo.AvailableFreeSpace),
                                                                        LengthFormatter.LengthFormat(driveInfo.TotalSize))
                                    : string.Format("{0} {1}",
                                                driveInfo.DriveType,
                                                driveInfo.Name));
            return new DriveTabItem(source, drive.ToLower(), toolTipText, driveInfo.RootDirectory);
        }

        public DriveTabItem(ImageSource img, string text, object toolTip, object tag)
            : this(toolTip,tag)
        {
            _img = img;
            _text = text;

            var stack = new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                VerticalAlignment=VerticalAlignment.Top,
                            };
            var image = new Image
                            {
                                Source = img,
                                Height=14,
                                Margin = new Thickness(3, 0, 3, 0)
                            };
            var label = new Label
                            {
                                VerticalAlignment =  VerticalAlignment.Top,
                                Content = text
                            };
            stack.Children.Add(image);
            stack.Children.Add(label);
            Header = stack;
        }

        public DriveTabItem Clone()
        {
            return new DriveTabItem(_img,_text, ToolTip,Tag);
        }
    }
}
