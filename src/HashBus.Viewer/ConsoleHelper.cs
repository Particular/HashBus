namespace HashBus.Viewer
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    static class ConsoleHelper
    {
        internal static void MakeTopMost()
        {
            var currentWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

            SetWindowPos(currentWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }
}
