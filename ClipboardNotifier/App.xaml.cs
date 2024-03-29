﻿// -----------------------------------------------------------------------
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

    using ClipboardNotifier.Models;
    using ClipboardNotifier.Views;
    using ClipboardNotifier.Windows.Forms;

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
            containerRegistry.RegisterSingleton<ClipboardWatcher>();
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            var clipboardWatcher = this.Container.Resolve<ClipboardWatcher>();
            clipboardWatcher.Start(this.MainWindow);
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
            this.notifyIcon.Text = "Clipboard Notifier";
            this.notifyIcon.IsVisible = true;

            var startupItem = this.notifyIcon.AddItem("Start at login");
            startupItem.CheckOnClick = true;
            this.notifyIcon.AddSeparator();
            this.notifyIcon.AddItem("Close", this.Shutdown);

            this.notifyIcon.Opened += (s_, e_) =>
            {
                startupItem.Checked = IsStartupEnabled();
            };

            startupItem.Click += (s_, e_) =>
            {
                var enabled = startupItem.Checked;
                if (enabled)
                {
                    if (!IsStartupEnabled())
                    {
                        CreateStartupShortcut();
                    }
                }
                else
                {
                    if (IsStartupEnabled())
                    {
                        DeleteStartupShortcut();
                    }
                }
            };
        }

        /// <inheritdoc/>
        protected override void OnExit(ExitEventArgs ev)
        {
            var clipboardWatcher = this.Container.Resolve<ClipboardWatcher>();
            clipboardWatcher.Stop();

            this.notifyIcon.Dispose();

            base.OnExit(ev);
        }

        private static string GetStartupLinkPath()
        {
            var startupDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return System.IO.Path.Combine(startupDir, "ClipboardNotifier.exe.lnk");
        }

        private static bool IsStartupEnabled()
        {
            var path = GetStartupLinkPath();
            return System.IO.File.Exists(path);
        }

        private static void CreateStartupShortcut()
        {
            var path = GetStartupLinkPath();
            var shell = new IWshRuntimeLibrary.WshShell();
            var shortcut = (IWshRuntimeLibrary.WshShortcut)shell.CreateShortcut(path);
            shortcut.TargetPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            shortcut.IconLocation = System.Reflection.Assembly.GetEntryAssembly().Location + ",0";
            shortcut.Save();

            Marshal.FinalReleaseComObject(shortcut);
            Marshal.FinalReleaseComObject(shell);
        }

        private static void DeleteStartupShortcut()
        {
            var path = GetStartupLinkPath();
            System.IO.File.Delete(path);
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
    }
}
