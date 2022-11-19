// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    using ClipboardNotifier.Models;
    using ClipboardNotifier.Views;
    using ClipboardNotifier.Windows.Forms;

    using FontAwesome.WPF;

    using Prism.Ioc;

    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        private AppNotifyIcon notifyIcon;

        /// <inheritdoc/>
        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        /// <inheritdoc/>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ClipboardMonitor>();
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            var clipboardMonitor = this.Container.Resolve<ClipboardMonitor>();
            clipboardMonitor.StartMonitor(this.MainWindow);
        }

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs ev)
        {
            base.OnStartup(ev);

            /*
            var imageSource = ImageAwesome.CreateImageSource(FontAwesomeIcon.Clipboard, System.Windows.Media.Brushes.White, 30);
            var bitmapSource = ConvertImageSourceToBitmapSource(imageSource);
            var bitmap = ConvertBitmapSourceToBitmap(bitmapSource);
            var icon = (Icon)Icon.FromHandle(bitmap.GetHicon()).Clone();
            */

            var iconStream = GetResourceStream(new Uri("pack://application:,,,/ClipboardNotifier;component/Resources/icon16.ico")).Stream;
            var icon = new Icon(iconStream);

            this.notifyIcon = new AppNotifyIcon(icon);
            this.notifyIcon.Closed += this.NotifyIcon_Closed;
            this.notifyIcon.Text = "Clipboard Notifier";
            this.notifyIcon.IsVisible = true;
        }

        /// <inheritdoc/>
        protected override void OnExit(ExitEventArgs ev)
        {
            this.notifyIcon.Dispose();

            base.OnExit(ev);
        }

        private static BitmapSource ConvertImageSourceToBitmapSource(ImageSource imageSource)
        {
            var width = imageSource.Width;
            var height = imageSource.Height;
            var rect = new Rect(new System.Windows.Point(0, 0), new System.Windows.Size(width, height));
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(imageSource, rect);
            drawingContext.Close();

            var bmp = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }

        private static Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            var width = bitmapSource.PixelWidth;
            var height = bitmapSource.PixelHeight;
            var stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            var ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(height * stride);
                bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
                using (var bitmap = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr))
                {
                    // Clone the bitmap so that we can dispose it and
                    // release the unmanaged memory at ptr
                    return new Bitmap(bitmap);
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        private void NotifyIcon_Closed(object sender, EventArgs ev)
        {
            this.Shutdown();
        }
    }
}
