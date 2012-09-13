using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace JoyOs.FileSystem.FilesAPI
{
    public class FileContextMenu
    {
        private FileContextMenu()
        {
            //
            // All static, no instances
            //
        }

        // CreatePopupMenu
        [DllImport("user32.dll", EntryPoint = "CreatePopupMenu", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreatePopupMenu();

        // MessageBeep
        [DllImport("User32", EntryPoint = "MessageBeep", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool MessageBeep(int SoundType);

        // TrackPopupMenu
        [DllImport("user32.dll", EntryPoint = "TrackPopupMenu", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int TrackPopupMenu(IntPtr hMenu, int wFlags, int x, int y, int nReserved, IntPtr hwnd, out Rect lprc);

        // SHGetDesktopFolder
        [DllImport("shell32.dll", EntryPoint = "SHGetDesktopFolder", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern void SHGetDesktopFolder(ref IShellFolder sf);

        public const int MIN_SHELL_ID = 1;
        public const int MAX_SHELL_ID = 0x7FFF;
        //
        public const int TPM_RETURNCMD = 0x100;
        public const int TPM_LEFTALIGN = 0x0;
        //
        public const int SW_SHOWNORMAL = 1;

        static int m_pidlCount = 0;
        static IntPtr[] m_ipList;

        public static int MAKEINTRESOURCE(int res)
        {
            return 0x0000FFFF & res;
        }

        public static int ShowContextMenu(IntPtr hwnd, string[] strFiles, int intX, int intY, bool blnErrorBeep)
        {
            CMINVOKECOMMANDINFO CI = new CMINVOKECOMMANDINFO();

            DirectoryInfo dInfo;

            FileInfo fInfo;

            IContextMenu CM; // LPCONTEXTMENU

            int cmdID; // LongBool
            int pchEaten = 0; // DWORD pchEaten;
            int dwAttributes = new int(); // ULONG
            int intZero = 0;
            int intCount;

            IntPtr ipParent = new IntPtr(); // LPITEMIDLIST
            IntPtr ipChild = new IntPtr(); // LPITEMIDLIST
            IntPtr hMenu = new IntPtr(); // HMENU
            //IntPtr ipPath = new IntPtr();
            //IntPtr ipFileName = new IntPtr();

            IShellFolder DesktopFolder = null; // IShellFolder
            IShellFolder ParentFolder = null; // IShellfolder ParentFolder

            Rect mRect;

            REFIID IID_IShellFolder = new REFIID("000214E6-0000-0000-c000-000000000046");
            REFIID IID_IContextMenu = new REFIID("000214E4-0000-0000-c000-000000000046");

            string strFullName = "";
            string strFilePath = "";
            string strFileName = "";
            string strFQName = "";

            m_pidlCount = 0;

            for (intCount = 0; intCount <= strFiles.GetUpperBound(0); intCount++)
            {
                strFullName = strFiles[intCount];

                fInfo = new FileInfo(strFullName);

                if (fInfo.Exists)
                {
                    strFilePath = fInfo.DirectoryName;
                    strFileName = fInfo.Name;
                    strFQName = fInfo.FullName;
                }
                else
                {
                    dInfo = new DirectoryInfo(strFullName);
                    if (dInfo.Exists)
                    {
                        try
                        {
                            strFileName = dInfo.Name;
                            strFilePath = dInfo.Parent.FullName;

                        }
                        catch
                        {
                            strFilePath = "";
                        }
                    }
                }

                SHGetDesktopFolder(ref DesktopFolder);

                // ParseDisplayName - parent
                // Translates a file object's or folder's display name into an item identifier list
                pchEaten = 1;
                dwAttributes = 0;
                ipParent = new IntPtr();
                DesktopFolder.ParseDisplayName(hwnd, IntPtr.Zero, strFilePath, ref pchEaten, ref ipParent, ref dwAttributes);

                // BindToObject
                // Retrieves an IShellFolder object for a subfolder
                ParentFolder = null;
                DesktopFolder.BindToObject(ipParent, IntPtr.Zero, ref IID_IShellFolder, ref ParentFolder);

                // ParseDisplayName - child
                // Translates a file object's or folder's display name into an item identifier list
                pchEaten = 1;
                dwAttributes = 0;
                ipChild = new IntPtr();
                ParentFolder.ParseDisplayName(hwnd, IntPtr.Zero, strFileName, ref pchEaten, ref ipChild, ref dwAttributes);

                JAddItemToIDList(ipChild);
            }

            CM = null;

            int intReturn = ParentFolder.GetUIObjectOf(hwnd, m_pidlCount, ref m_ipList[0], ref IID_IContextMenu, out intZero, ref CM);

            if (CM != null)
            {
                hMenu = CreatePopupMenu();

                CM.QueryContextMenu(hMenu, 0, MIN_SHELL_ID, MAX_SHELL_ID, QueryContextMenuFlags.CMF_EXPLORE);

                cmdID = TrackPopupMenu(hMenu, (TPM_RETURNCMD | TPM_LEFTALIGN), intX, intY, 0, hwnd, out mRect);

                if (cmdID != 0)
                {
                    CI.cbSize = Marshal.SizeOf(CI);
                    CI.hwnd = hwnd;
                    CI.lpVerb = (IntPtr)MAKEINTRESOURCE(cmdID - 1);
                    CI.lpParameters = IntPtr.Zero;
                    CI.lpDirectory = IntPtr.Zero;
                    CI.nShow = SW_SHOWNORMAL;
                    CM.InvokeCommand(ref CI);
                }
            }
            else
            {
                if (blnErrorBeep)
                    MessageBeep(-1);
            }
            return 0;
        }

        private static void JAddItemToIDList(IntPtr ipNew)
        {
            if (m_pidlCount == 0)
            {
                m_ipList = new IntPtr[1];
                m_ipList[0] = ipNew;
                m_pidlCount++;
                return;
            }

            int intCount;
            IntPtr[] ipTemp = new IntPtr[m_pidlCount];

            for (intCount = 0; intCount < m_pidlCount; intCount++)
                ipTemp[intCount] = m_ipList[intCount];

            m_ipList = new IntPtr[m_pidlCount + 1];

            for (intCount = 0; intCount < m_pidlCount; intCount++)
                m_ipList[intCount] = ipTemp[intCount];

            m_ipList[intCount] = ipNew;
            m_pidlCount++;
        }
    }

    [ComImport, Guid("00000000-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IUnknown
    {
        [PreserveSig]
        IntPtr QueryInterface(REFIID riid, out IntPtr pVoid);

        [PreserveSig]
        IntPtr AddRef();

        [PreserveSig]
        IntPtr Release();
    }

    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int Width
        {
            get
            {
                return right - left;
            }
        }

        public int Height
        {
            get
            {
                return bottom - top;
            }
        }
    }

    [ComImport, Guid("000214E6-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellFolder
    {
        [PreserveSig]
        int ParseDisplayName(IntPtr hwnd, IntPtr pbc, string pszDisplayName, ref int pchEaten, ref IntPtr ppidl, ref int pdwAttributes);

        [PreserveSig]
        int EnumObjects(IntPtr hWnd, ShellEnumFlags flags, ref IEnumIDList enumList);

        [PreserveSig]
        int BindToObject(IntPtr idList, IntPtr bindingContext, ref REFIID refiid, ref IShellFolder folder);

        [PreserveSig]
        int BindToStorage(ref IntPtr idList, IntPtr bindingContext, ref REFIID riid, IntPtr pVoid);

        [PreserveSig]
        int CompareIDs(int lparam, IntPtr idList1, IntPtr idList2);

        [PreserveSig]
        int CreateViewObject(IntPtr hWnd, REFIID riid, IntPtr pVoid);

        [PreserveSig]
        int GetAttributesOf(int count, ref IntPtr idList, out GetAttributeOfFlags attributes);

        [PreserveSig]
        int GetUIObjectOf(IntPtr hwnd, int cidl, ref IntPtr apidl, ref REFIID riid, out int rgfReserved, ref IContextMenu ppv);

        [PreserveSig]
        int GetDisplayNameOf(IntPtr idList, ShellGetDisplayNameOfFlags flags, ref STRRET strRet);

        [PreserveSig]
        int SetNameOf(IntPtr hWnd, ref IntPtr idList, IntPtr pOLEString, int flags, ref IntPtr pItemIDList);

    }

    [ComImport, Guid("000214f2-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumIDList
    {
        [PreserveSig]
        int Next(int count, ref IntPtr idList, out int fetched);

        [PreserveSig]
        int Skip(int count);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone(ref IEnumIDList list);
    }

    [ComImport, Guid("000214e4-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IContextMenu
    {
        [PreserveSig]
        int QueryContextMenu(IntPtr hMenu, int indexMenu, int idFirstCommand, int idLastCommand, QueryContextMenuFlags flags);

        [PreserveSig]
        int InvokeCommand(ref CMINVOKECOMMANDINFO ici);

        [PreserveSig]
        int GetCommandString(int idCommand, int type, int reserved, string commandName, int cchMax);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct REFIID
    {
        public int x;
        public short s1;
        public short s2;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] chars;

        public REFIID(string guid)
        {
            // Needs to be a string of the form:
            // "000214E6-0000-0000-c000-000000000046"
            string[] data = guid.Split('-');
            Debug.Assert(data.Length == 5);
            x = Convert.ToInt32(data[0], 16);
            s1 = Convert.ToInt16(data[1], 16);
            s2 = Convert.ToInt16(data[2], 16);
            string bytesData = data[3] + data[4];
            chars = new byte[] { Convert.ToByte(bytesData.Substring(0,2), 16), Convert.ToByte(bytesData.Substring(2,2), 16),
Convert.ToByte(bytesData.Substring(4,2), 16), Convert.ToByte(bytesData.Substring(6,2), 16),
Convert.ToByte(bytesData.Substring(8,2), 16), Convert.ToByte(bytesData.Substring(10,2), 16),
Convert.ToByte(bytesData.Substring(12,2), 16), Convert.ToByte(bytesData.Substring(14,2), 16) };
        }

    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct STRRET
    {
        public STRRETFlags uType; // One of the STRRET values
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] cStr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CMINVOKECOMMANDINFO
    {
        public int cbSize; // sizeof(CMINVOKECOMMANDINFO)
        public int fMask; // any combination of CMIC_MASK_*
        public IntPtr hwnd; // might be NULL (indicating no owner window)
        public IntPtr lpVerb; // either a string or MAKEINTRESOURCE(idOffset)
        public IntPtr lpParameters; // might be NULL (indicating no parameter)
        public IntPtr lpDirectory; // might be NULL (indicating no specific directory)
        public int nShow; // one of SW_ values for ShowWindow() API
        public int dwHotKey;
        public IntPtr hIcon;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct SHITEMID
    {
        public short cb;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] abID;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ITEMIDLIST
    {
        public SHITEMID mkid;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct POINT
    {
        public int x;
        public int y;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // Enums
    [Flags]
    public enum ShellEnumFlags
    {
        SHCONTF_FOLDERS = 32, // for shell browser
        SHCONTF_NONFOLDERS = 64, // for default view
        SHCONTF_INCLUDEHIDDEN = 128, // for hidden/system objects
    }

    [Flags]
    public enum ShellGetDisplayNameOfFlags
    {
        SHGDN_NORMAL = 0, // default (display purpose)
        SHGDN_INFOLDER = 1, // displayed under a folder (relative)
        SHGDN_INCLUDE_NONFILESYS = 0x2000, // if not set, display names for shell name space items that are not in the file system will fail.
        SHGDN_FORADDRESSBAR = 0x4000, // for displaying in the address (drives dropdown) bar
        SHGDN_FORPARSING = 0x8000, // for ParseDisplayName or path
    }

    [Flags]
    public enum STRRETFlags
    {
        STRRET_WSTR = 0x0000, // Use STRRET.pOleStr
        STRRET_OFFSET = 0x0001, // Use STRRET.uOffset to Ansi
        STRRET_CSTR = 0x0002 // Use STRRET.cStr
    }

    [Flags]
    public enum GetAttributeOfFlags : long
    {
        DROPEFFECT_NONE = 0,
        DROPEFFECT_COPY = 1,
        DROPEFFECT_MOVE = 2,
        DROPEFFECT_LINK = 4,
        DROPEFFECT_SCROLL = 0x80000000,
        SFGAO_CANCOPY = DROPEFFECT_COPY, // Objects can be copied
        SFGAO_CANMOVE = DROPEFFECT_MOVE, // Objects can be moved
        SFGAO_CANLINK = DROPEFFECT_LINK, // Objects can be linked
        SFGAO_CANRENAME = 0x00000010, // Objects can be renamed
        SFGAO_CANDELETE = 0x00000020, // Objects can be deleted
        SFGAO_HASPROPSHEET = 0x00000040, // Objects have property sheets
        SFGAO_DROPTARGET = 0x00000100, // Objects are drop target
        SFGAO_CAPABILITYMASK = 0x00000177,
        SFGAO_LINK = 0x00010000, // Shortcut (link)
        SFGAO_SHARE = 0x00020000, // shared
        SFGAO_READONLY = 0x00040000, // read-only
        SFGAO_GHOSTED = 0x00080000, // ghosted icon
        SFGAO_HIDDEN = 0x00080000, // hidden object
        SFGAO_DISPLAYATTRMASK = 0x000F0000,
        SFGAO_FILESYSANCESTOR = 0x10000000, // It contains file system folder
        SFGAO_FOLDER = 0x20000000, // It's a folder.
        SFGAO_FILESYSTEM = 0x40000000, // is a file system thing (file/folder/root)
        SFGAO_HASSUBFOLDER = 0x80000000, // Expandable in the map pane
        SFGAO_CONTENTSMASK = 0x80000000,
        SFGAO_VALIDATE = 0x01000000, // invalidate cached information
        SFGAO_REMOVABLE = 0x02000000, // is this removeable media?
        SFGAO_COMPRESSED = 0x04000000, // Object is compressed (use alt color)
        SFGAO_BROWSABLE = 0x08000000, // is in-place browsable
        SFGAO_NONENUMERATED = 0x00100000, // is a non-enumerated object
        SFGAO_NEWCONTENT = 0x00200000 // should show bold in explorer tree
    }

    public enum QueryContextMenuFlags : long
    {
        CMF_NORMAL = 0x00000000,
        CMF_DEFAULTONLY = 0x00000001,
        CMF_VERBSONLY = 0x00000002,
        CMF_EXPLORE = 0x00000004,
        CMF_NOVERBS = 0x00000008,
        CMF_CANRENAME = 0x00000010,
        CMF_NODEFAULT = 0x00000020,
        CMF_INCLUDESTATIC = 0x00000040,
        CMF_RESERVED = 0xffff0000
    }
    //------------------------------------
}
