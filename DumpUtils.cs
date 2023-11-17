using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Utils {
    public class DumpUtils {
        [Flags]
        public enum Option : uint {
            // From dbghelp.h:
            Normal = 0x00000000,
            WithDataSegs = 0x00000001,
            WithFullMemory = 0x00000002,
            WithHandleData = 0x00000004,
            FilterMemory = 0x00000008,
            ScanMemory = 0x00000010,
            WithUnloadedModules = 0x00000020,
            WithIndirectlyReferencedMemory = 0x00000040,
            FilterModulePaths = 0x00000080,
            WithProcessThreadData = 0x00000100,
            WithPrivateReadWriteMemory = 0x00000200,
            WithoutOptionalData = 0x00000400,
            WithFullMemoryInfo = 0x00000800,
            WithThreadInfo = 0x00001000,
            WithCodeSegs = 0x00002000,
            WithoutAuxiliaryState = 0x00004000,
            WithFullAuxiliaryState = 0x00008000,
            WithPrivateWriteCopyMemory = 0x00010000,
            IgnoreInaccessibleMemory = 0x00020000,
            ValidTypeFlags = 0x0003ffff,
        }

        enum ExceptionInfo {
            None,
            Present
        }

        static bool Write(SafeHandle fileHandle, Option options, ExceptionInfo exceptionInfo) {
            Process currentProcess = Process.GetCurrentProcess();
            IntPtr currentProcessHandle = currentProcess.Handle;
            uint currentProcessId = (uint)currentProcess.Id;
            DllUtils.MiniDumpExceptionInformation exp;
            exp.ThreadId = DllUtils.GetCurrentThreadId();
            exp.ClientPointers = false;
            exp.ExceptionPointers = IntPtr.Zero;
            if (exceptionInfo == ExceptionInfo.Present) {
                exp.ExceptionPointers = Marshal.GetExceptionPointers();
            }
            return exp.ExceptionPointers == IntPtr.Zero ? DllUtils.MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) : DllUtils.MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, ref exp, IntPtr.Zero, IntPtr.Zero);
        }

        static bool Write(SafeHandle fileHandle, Option dumpType) {
            return Write(fileHandle, dumpType, ExceptionInfo.None);
        }

        public static bool TryDump(string dmpPath, Option dmpType = Option.Normal) {
            var path = Path.Combine(Environment.CurrentDirectory, dmpPath);
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            using var fs = new FileStream(path, FileMode.Create); return Write(fs.SafeFileHandle, dmpType);
        }
    }
}
