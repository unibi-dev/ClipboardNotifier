// -----------------------------------------------------------------------
// <copyright file="MouseHook.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Windows
{
    using System;
    using System.Runtime.InteropServices;

    using ClipboardNotifier.Windows.NativeMethods;

    /// <summary>
    /// Wrapper class for Windows hook methods about mouse.
    /// </summary>
    internal class MouseHook
    {
        private WindowsHook.MouseMessages message;
        private Action<WindowsHook.MSLLHOOKSTRUCT> action;
        private IntPtr hookID = IntPtr.Zero;

        private WindowsHook.LowLevelMouseProc proc;

        /// <summary>
        /// Sets a hook action for specified message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="action"></param>
        public void SetHook(WindowsHook.MouseMessages message, Action<WindowsHook.MSLLHOOKSTRUCT> action)
        {
            this.UnsetHook();

            this.message = message;
            this.action = action;

            this.proc = this.MouseProc;

            this.hookID = WindowsHook.SetWindowsHookEx(WindowsHook.WH_MOUSE_LL, this.proc, IntPtr.Zero, 0);
        }

        /// <summary>
        /// Removes a hook action.
        /// </summary>
        public void UnsetHook()
        {
            if (this.hookID == IntPtr.Zero)
            {
                return;
            }

            WindowsHook.UnhookWindowsHookEx(this.hookID);
            this.hookID = IntPtr.Zero;
        }

        private IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (WindowsHook.MouseMessages)wParam == this.message)
            {
                var hookStruct = (WindowsHook.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WindowsHook.MSLLHOOKSTRUCT));
                this.action?.Invoke(hookStruct);
            }

            return WindowsHook.CallNextHookEx(this.hookID, nCode, wParam, lParam);
        }
    }
}
