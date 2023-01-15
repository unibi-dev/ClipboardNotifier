// -----------------------------------------------------------------------
// <copyright file="WindowStyle.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Windows.NativeMethods
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// GetWindowLong/SetWindowLong.
    /// </summary>
    internal static class WindowStyle
    {
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1310 // Field names should not contain underscore
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = -20;
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
#pragma warning restore SA1310 // Field names should not contain underscore

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);
#pragma warning restore SA1600 // Elements should be documented
    }
}
