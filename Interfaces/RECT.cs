using System.Runtime.InteropServices;

namespace Utils.Interfaces {
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        private readonly int _Left;
        private readonly int _Top;
        private readonly int _Right;
        private readonly int _Bottom;
    }
}
