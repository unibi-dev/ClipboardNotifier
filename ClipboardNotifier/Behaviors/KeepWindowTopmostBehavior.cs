// -----------------------------------------------------------------------
// <copyright file="KeepWindowTopmostBehavior.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Behaviors
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;

    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// Keeps setting window topmost in each interval time.
    /// Used to bring window over other topmost windows (e.g. taskbar).
    /// </summary>
    internal class KeepWindowTopmostBehavior : Behavior<Window>
    {
        /// <summary>
        /// DependencyProperty of Interval.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            nameof(Interval),
            typeof(double),
            typeof(KeepWindowTopmostBehavior),
            new PropertyMetadata(1.0));

        private Stopwatch stopwatch;

        /// <summary>
        /// Gets or sets the interval time in second to bring window topmost.
        /// </summary>
        public double Interval
        {
            get => (double)this.GetValue(IntervalProperty);
            set => this.SetValue(IntervalProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            CompositionTarget.Rendering += this.CompositionTarget_Rendering;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
            base.OnDetaching();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs ev)
        {
            if (this.stopwatch.Elapsed.TotalSeconds > this.Interval)
            {
                // set window topmost without activating
                var window = this.AssociatedObject;
                var hwnd = new System.Windows.Interop.WindowInteropHelper(window).EnsureHandle();
                var topmost = Windows.NativeMethods.WindowStyle.HWND_TOPMOST;
                var flags =
                    Windows.NativeMethods.WindowStyle.SWP_NOMOVE |
                    Windows.NativeMethods.WindowStyle.SWP_NOSIZE |
                    Windows.NativeMethods.WindowStyle.SWP_NOACTIVATE;
                Windows.NativeMethods.WindowStyle.SetWindowPos(hwnd, topmost, 0, 0, 0, 0, flags);

                this.stopwatch.Restart();
            }
        }
    }
}
