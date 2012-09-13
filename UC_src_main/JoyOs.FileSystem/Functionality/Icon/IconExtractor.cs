using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using JoyOs.FileSystem.Model;
using JoyOs.FileSystem.Properties;
using JoyOs.Media;

namespace JoyOs.FileSystem.Functionality.Icon
{
    public class IconExtractor
    {
        const string PathString = "pack://application:,,,/JoyOs.Media;component/img/Base/";
        private ILogicItem _file;
        public static int ImagePixelSize = 20;

        enum UcImages
        {
            Archive = 0,
            UnknownFile,
            Folder,
            Encrypted,
            HiddenFile,
            HiddenFolder,
            UnknownExe,
            Link,
            Shared,
            Cmd,
            Back
        }

        static readonly Dictionary<string, ImageSource> Table;

        public static ImageSource GetDriveIcon(DriveType driveType)
        {
            return Table[driveType.ToString()];
        }

        public ILogicItem File
        {
            get { return _file; }
            set
            {
                if (value == null)
                    throw new ArgumentException("file", Resources.IconExtractor_FileNullException);

                _file = value;
            }
        }

        static IconExtractor()
        {
            Table = new Dictionary<string, ImageSource>();

            var imageItems = Enum.GetNames(typeof(UcImages));
            // Загружаем стандартные изображения (UcImages) 
            foreach (var item in imageItems)
            {
                Table.Add(item, ImageUtils.GetSmallImage(PathString + "FIles/" + item + ".ico"));
            }

            imageItems = Enum.GetNames(typeof(DriveType));
            foreach (var item in imageItems)
            {
                Table.Add(item, ImageUtils.GetSmallImage(PathString + "Drives/" + item + ".png"));
            }
        }

        public IconExtractor()
        {
        }

        public static ImageSource GetIcon(string filePath)
        {
            ImageSource resultImageSource;
            var fileInfo = new FileInfo(filePath);
            var isHidden = ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden);
            var isSystem = ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System);
            var isLink = ((fileInfo.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint);

            // Если папка
            if (!fileInfo.Exists)
            {
                resultImageSource = (isHidden || isSystem) ?
                        Table[UcImages.HiddenFolder.ToString()] : Table[UcImages.Folder.ToString()];
                if (isLink)
                {
                    resultImageSource = Table[UcImages.Link.ToString()];
                }
            }
            else // File
            {
                if (isHidden || isSystem)
                {
                    resultImageSource = Table[UcImages.HiddenFile.ToString()];
                    return resultImageSource;
                }

                if (isLink)
                {
                    resultImageSource = Table[UcImages.Link.ToString()];
                    return resultImageSource;
                }

                var extension = fileInfo.Extension.ToLower();
                if (extension != ".exe" && extension != ".lnk")
                {
                    // Если такой файл мы уже загружали
                    if (Table.ContainsKey(extension))
                    {
                        resultImageSource = Table[extension];
                        return resultImageSource;
                    }

                    using (var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath))
                    {
                        resultImageSource = Imaging.CreateBitmapSourceFromHIcon(
                            sysicon.Handle,
                            Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(ImagePixelSize,ImagePixelSize));
                    }

                    Table.Add(extension, resultImageSource);
                }
                else
                {
                    //resultImageSource = Table[UcImages.UnknownExe.ToString()];

                    using (var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath))
                    {
                        Debug.Assert(sysicon != null, "sysicon != null");

                        resultImageSource = Imaging.CreateBitmapSourceFromHIcon(
                                                                                    sysicon.Handle,
                                                                                    Int32Rect.Empty,
                                                                                    System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(ImagePixelSize,ImagePixelSize));
                    }
                }
            }
            return resultImageSource;
        }

        public IconExtractor(ILogicItem file)
        {
            File = file;
        }

        #region Override methods / operators

        /// <summary>
        /// To get debug / inside info
        /// </summary>
        /// <returns>
        /// String - Description
        /// </returns>
        public override string ToString()
        {
            return "Image path:" + File.Info.FullName;
        }

        /// <summary>
        /// Equality overriding, for comparing instances
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>
        /// Boolean value
        /// 1. True - Equals
        /// 2. False - Not Equals
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            return obj != null && (GetType() == obj.GetType());
        }

        /// <summary>
        /// To "Hash" operations
        /// </summary>
        /// <returns>
        /// Int32 - hash code
        /// </returns>
        public override Int32 GetHashCode()
        {
            return (File != null) ? File.GetHashCode() : base.GetHashCode();
        }

        /// <summary>
        /// To compare items
        /// </summary>
        /// <param name="a">ImageExtractor</param>
        /// <param name="b">ImageExtractor</param>
        /// <returns></returns>
        public static Boolean operator ==(IconExtractor a, IconExtractor b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// To compare items
        /// </summary>
        /// <param name="a">ImageExtractor</param>
        /// <param name="b">ImageExtractor</param>
        /// <returns></returns>
        public static Boolean operator !=(IconExtractor a, IconExtractor b)
        {
            return !(a == b);
        }

        #endregion

        public ImageSource Icon
        {
            get
            {
                ImageSource resultImageSource;

                if (File.Name != ViewDataItem.BackEntry)
                {
                    // Если папка
                    if (File.IsFolder)
                    {
                        resultImageSource = (File.IsHidden || File.IsSystem) ?
                                Table[UcImages.HiddenFolder.ToString()] : Table[UcImages.Folder.ToString()];
                        if (File.IsLink)
                        {
                            resultImageSource = Table[UcImages.Link.ToString()];
                        }
                    }
                    else // File
                    {
                        if (File.IsHidden || File.IsSystem)
                        {
                            resultImageSource = Table[UcImages.HiddenFile.ToString()];
                            return resultImageSource;
                        }

                        if (File.IsLink)
                        {
                            resultImageSource = Table[UcImages.Link.ToString()];
                            return resultImageSource;
                        }

                        var extension = File.Info.Extension.ToLower();
                        if (extension != ".exe" && extension != ".lnk")
                        {
                            // Если такой файл мы уже загружали
                            if (Table.ContainsKey(extension))
                            {
                                resultImageSource = Table[extension];
                                return resultImageSource;
                            }

                            using (var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(File.Info.FullName))
                            {
                                resultImageSource = Imaging.CreateBitmapSourceFromHIcon(
                                    sysicon.Handle,
                                    Int32Rect.Empty,
                                    System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(ImagePixelSize, 
                                    ImagePixelSize));
                            }

                            Table.Add(extension, resultImageSource);
                        }
                        else
                        {
                          //  resultImageSource = Table[UcImages.UnknownExe.ToString()];

                            using (var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(File.Info.FullName))
                            {
                                Debug.Assert(sysicon != null, "sysicon != null");

                                resultImageSource = Imaging.CreateBitmapSourceFromHIcon(
                                                                                            sysicon.Handle,
                                                                                            Int32Rect.Empty,
                                                                                            System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(20, 20));
                            }
                        }
                    }

                }
                else // ...
                {
                    resultImageSource = Table[UcImages.Back.ToString()];
                }

                return resultImageSource;
            }
        }
    }
}