using System;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using JoyOs.FileSystem.Functionality.Icon;
using JoyOs.FileSystem.Properties;

namespace JoyOs.FileSystem.Model
{
    #region Viewing File Manager Item

    public class ViewDataItem : ILogicItem
    {
        public static readonly string BackEntry;
        private static readonly IconExtractor IconExtractor;
        private static readonly NumberFormatInfo Nfi;
        private FileSystemInfo _info;
        private string _name;
        private ImageSource _icon;

        static ViewDataItem()
        {
            IconExtractor = new IconExtractor();
            BackEntry = Resources.Back;
            Nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            Nfi.NumberGroupSeparator = " ";
        }

        public ViewDataItem()
        {
            Length = long.MinValue;
            Type = string.Empty;
        }

        public ViewDataItem(FileSystemInfo dir)
            : this(dir,
                   (dir.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint
                       ? Resources.TypeFolder
                       : Resources.TypeLink
                  )
        {
            IsFolder = true;
            Name = dir.Name;
        }

        public ViewDataItem(FileInfo file)
            : this(file, file.Extension, file.Length)
        {
            Name = Path.GetFileNameWithoutExtension(file.Name);
        }

        private ViewDataItem(FileSystemInfo inf, string type, long length = long.MinValue)
        {
            _info = inf;
           
            IsHidden = ((_info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden);
            IsSystem = ((_info.Attributes & FileAttributes.System) == FileAttributes.System);
            IsLink = ((_info.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint);
            
            Length = length;
            Type = type;
        }

        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    IconExtractor.File =this;
                    _icon=IconExtractor.Icon;
                }
                return _icon;
            }

        }
        

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(Resources.ViewDataItem_NameNullException);

                _name = value;
            }
        }

        public string Type { get; private set; }

        public long Length { get; private set; }

        public string LengthToString { get { return Length.ToString("#,#; ", Nfi); } }

        public FileSystemInfo Info
        {
            get { return _info; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(Resources.ViewDataItem_InfoNullException);
               
                _info = value;

                 IsHidden = ((_info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden);
                 IsSystem = ((_info.Attributes & FileAttributes.System) == FileAttributes.System);
                 IsLink = ((_info.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint);

                if (!IsFolder)
                {
                    if (!_info.Exists) return;
                    Name = Path.GetFileNameWithoutExtension(_info.Name);
                    var file = (FileInfo)_info;
                    Type = Info.Extension;
                    Length = file.Length;
                }
                else
                {
                    Name = _info.Name;
                }

            }
        }

        public bool IsFolder { get; private set; }

        public bool IsHidden { get;  private set; }
        public bool IsLink { get; private set; }
        public bool IsSystem { get; private set; }

        public int CompareTo(ILogicItem other)
        {
            return Name == BackEntry && other.Name == BackEntry ? 0 : Name == BackEntry 
                                                                                                     ? -1 : other.Name == BackEntry
                                                                                                     ? 1 : IsFolder
                                                                                                     ? Name.CompareTo(other.Name) : 0;
        }

        /// <summary>
        /// ToString override, get information for debug
        /// </summary>
        /// <returns>string </returns>
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", Name, Type, LengthToString, Info.LastAccessTime);
        }

        public string ToFullString()
        {
            return String.Format("{0} {1} {2} {3}", Info.FullName, (Length == long.MinValue ? Type : ""),
                                                                                LengthToString, Info.LastAccessTime);
        }

    }

    #endregion
}