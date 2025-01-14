using System;
using System.Runtime.InteropServices;
using System.Text;
using Utils.Interfaces;

namespace Utils {
    public class DllUtils {
        public const string
            User32 = "user32.dll",
            Gdi32 = "gdi32.dll",
            GdiPlus = "gdiplus.dll",
            Kernel32 = "kernel32.dll",
            Shell32 = "shell32.dll",
            MsImg = "msimg32.dll",
            NTdll = "ntdll.dll",
            DwmApi = "dwmapi.dll",
            DbgHelp = "dbghelp.dll",
            PsApi = "psapi.dll",
            WinMm = "winmm.dll",
            Crypt = "crypt32.dll";

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DATA_BLOB {
            public int cbData;

            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CRYPTPROTECT_PROMPTSTRUCT {
            public int cbSize;

            public int dwPromptFlags;

            public IntPtr hwndApp;

            public string szPrompt;
        }

        public struct Point {
            public int X;
            public int Y;

            // ReSharper disable once UnusedMember.Local
            public Point(int x, int y) {
                this.X = x;
                this.Y = y;
            }

            public override readonly string ToString() {
                return $"X:{X},Y:{Y}";
            }
        }

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATA_STRUCT {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        //typedef struct _MINIDUMP_EXCEPTION_INFORMATION {
        //    DWORD ThreadId;
        //    PEXCEPTION_POINTERS ExceptionPointers;
        //    BOOL ClientPointers;
        //} MINIDUMP_EXCEPTION_INFORMATION, *PMINIDUMP_EXCEPTION_INFORMATION;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]  // Pack=4 is important! So it works also for x64!
        public struct MiniDumpExceptionInformation {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            [MarshalAs(UnmanagedType.Bool)]
            public bool ClientPointers;
        }

        /** 强制GC API函数**/
        [DllImport(Kernel32)]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        /// <summary>
        /// 窗口操作，关闭请不要随意使用
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport(User32, EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, ushort nCmdShow);

        [DllImport(User32, EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 窗口置最前，最小化也会置
        /// </summary>
        /// <param name="hWnd">窗口的句柄</param>
        /// <param name="fAltTab">此参数的 TRUE 表示正在使用 Alt/Ctl+Tab 键序列将窗口切换到 。 否则，此参数应为 FALSE</param>
        /// <returns></returns>
        [DllImport(User32, CharSet = CharSet.Auto)]
        public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        /// Gets the window long.
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(User32, CharSet = CharSet.Auto)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport(User32)]
        public static extern uint PrivateExtractIcons(string szFileName, int nIconIndex, int cxIcon, int cyIcon, IntPtr[] phicon, uint[] piconid, uint nIcons, uint flags);

        [DllImport(User32)]
        public static extern int DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// 向控件发送消息
        /// </summary>
        /// <param name="handle">控件句柄</param>
        /// <param name="wMsg">发送消息类型</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">消息</param>
        /// <returns></returns>
        [DllImport(User32)]
        public static extern IntPtr SendMessage(IntPtr handle, uint wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(Shell32, EntryPoint = "SHGetFileInfo", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
        [DllImport(Shell32, EntryPoint = "#727")]
        public extern static int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppv);

        // 执行exe文件
        [DllImport(Shell32, CharSet = CharSet.Auto)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport(PsApi)]
        public static extern bool GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, int cbName);

        [DllImport(Kernel32)]
        public static extern bool CloseHandle(IntPtr hObject);


        [DllImport(WinMm, EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]
        public static extern int MciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback);
        /// <summary>
        /// 清空回收站
        /// </summary>
        /// <param name="handle">父窗口句柄</param>
        /// <param name="root">将要清空的回收站的地址,如果为Null值时将清空所有驱动器上的回收站.</param>
        /// <param name="falgs">用于清空回收站的功能参数.WinApi.SHERB_1+WinApi.SHERB_2</param>
        /// <returns></returns>
        [DllImport(Shell32)]//声明API函数
        public static extern int SHEmptyRecycleBin(IntPtr handle, string root, int falgs);
        /// <summary>
        /// 操作历史文档
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="path"></param>
        [DllImport(Shell32, CharSet = CharSet.Unicode)]
        public static extern void SHAddToRecentDocs(WinApi.ShellAddToRecentDocsFlags flag, string path);
        /// <summary>
        /// 设置波形音量
        /// </summary>
        /// <param name="hwo"></param>
        /// <param name="pdwVolume"></param>
        /// <returns></returns>
        [DllImport(WinMm)]
        public static extern int waveOutSetVolume(int hwo, uint pdwVolume);
        [DllImport(WinMm)]
        public static extern uint waveOutGetVolume(int hwo, out uint pdwVolume);


        [DllImport(Crypt, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CryptProtectData(ref DATA_BLOB pDataIn, string szDataDescr, ref DATA_BLOB pOptionalEntropy, IntPtr pvReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);

        [DllImport(User32, CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(ref Point pt);

        // 导入 Windows API 中的设置任务栏应用程序 ID 的方法
        [DllImport(Shell32, SetLastError = true)]
        public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);
        [DllImport(Gdi32)]
        public static extern int GetDeviceCaps(IntPtr hDc, int nIndex);
        [DllImport(User32)]
        public static extern IntPtr GetDC(IntPtr hDc);
        [DllImport(User32)]
        public static extern IntPtr ReleaseDC(IntPtr handler, IntPtr hDc);
        [DllImport(DbgHelp, EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, ref MiniDumpExceptionInformation expParam, IntPtr userStreamParam, IntPtr callbackParam);

        // Overload supporting MiniDumpExceptionInformation == NULL
        [DllImport(DbgHelp, EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

        [DllImport(Kernel32, EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern uint GetCurrentThreadId();
    }
}
