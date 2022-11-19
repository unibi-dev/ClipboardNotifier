// -----------------------------------------------------------------------
// <copyright file="WindowHelper.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Helper methods for window class.
    /// </summary>
    internal static class WindowHelper
    {
        /// <summary>
        /// Show window to foreground.
        /// </summary>
        /// <param name="window"></param>
        public static void ActivateWindow(Window window)
        {
            var hwnd = new WindowInteropHelper(window).EnsureHandle();

            // restore minimized window
            if (NativeMethods.IsIconic(hwnd))
            {
                NativeMethods.ShowWindowAsync(hwnd, NativeMethods.SW_RESTORE);
            }

            NativeMethods.SetForegroundWindow(hwnd);
        }

        /// <summary>
        /// Gets mouse transparency of a window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns>true if transparent.</returns>
        public static bool IsTransparent(Window window)
        {
            var hwnd = new WindowInteropHelper(window).EnsureHandle();
            var style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            return (style & NativeMethods.WS_EX_TRANSPARENT) != 0;
        }

        /// <summary>
        /// Sets mouse transparency to a window.
        /// </summary>
        /// <param name="window"></param>
        public static void SetTransparent(Window window)
        {
            var hwnd = new WindowInteropHelper(window).EnsureHandle();
            var style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            style |= NativeMethods.WS_EX_TRANSPARENT;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, style);
        }

        /// <summary>
        /// Unsets mouse transparency from a window.
        /// </summary>
        /// <param name="window"></param>
        public static void UnsetTransprent(Window window)
        {
            var hwnd = new WindowInteropHelper(window).EnsureHandle();
            var style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            style &= ~NativeMethods.WS_EX_TRANSPARENT;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, style);
        }
    }
}
