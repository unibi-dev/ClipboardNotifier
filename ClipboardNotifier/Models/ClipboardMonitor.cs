// -----------------------------------------------------------------------
// <copyright file="ClipboardMonitor.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Models
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Interop;
    using ClipboardNotifier.Windows;

    /// <summary>
    /// Monitors clipboard changes.
    /// </summary>
    public class ClipboardMonitor
    {
        private static readonly IntPtr WndProcSuccess = IntPtr.Zero;

        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Invokes when clipboard changed.
        /// </summary>
        public event EventHandler ClipboardChanged;

        /// <summary>
        /// Gets or sets the interval time preventing calling event.
        /// </summary>
        public double IntervalMilliseconds { get; set; } = 100;

        /// <summary>
        /// Start monitoring clipboard.
        /// </summary>
        /// <param name="window"></param>
        public void StartMonitor(Window window)
        {
            var source = PresentationSource.FromVisual(window) as HwndSource;
            if (source == null)
            {
                return;
            }

            source.AddHook(this.WndProc);

            // get window handle for interop
            var windowHandle = new WindowInteropHelper(window).Handle;

            // register for clipboard events
            NativeMethods.AddClipboardFormatListener(windowHandle);
        }

        private void OnClipboardChanged()
        {
            this.ClipboardChanged?.Invoke(this, EventArgs.Empty);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                // NOTE: some applications open/close clipboard multiple times for one action
                if (!this.stopwatch.IsRunning || this.stopwatch.ElapsedMilliseconds >= this.IntervalMilliseconds)
                {
                    this.OnClipboardChanged();
                    this.stopwatch.Restart();
                }

                handled = true;
            }

            return WndProcSuccess;
        }
    }
}
