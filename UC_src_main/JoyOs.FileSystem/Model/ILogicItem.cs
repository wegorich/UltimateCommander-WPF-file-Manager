using System;
using System.IO;

namespace JoyOs.FileSystem.Model
{
    public enum LogicView
    {
        Standart = 0,

        ShortInfo,
        FullInfo,
        Comments,
        LookArt,

        UserConfigured
    }

    public interface ILogicItem : IComparable<ILogicItem>
    {
        FileSystemInfo Info { get; set; }
        bool IsFolder { get; }

        bool IsHidden { get;}
        bool IsSystem { get; }
        bool IsLink { get; }

        string Type { get; }
        string Name { get; set; }
        long Length { get; }
    }
}
