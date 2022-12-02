// -----------------------------------------------------------------------
// <copyright file="ClipboardMethods.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Windows.NativeMethods
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native methods about clipboard.
    /// </summary>
    internal static class ClipboardMethods
    {
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1310 // Field names should not contain underscore
        public const int WM_CLIPBOARDUPDATE = 0x031D;
#pragma warning restore SA1310 // Field names should not contain underscore

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
#pragma warning restore SA1600 // Elements should be documented
    }
}
