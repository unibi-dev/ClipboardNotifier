// -----------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Timers;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using ClipboardNotifier.Models;
    using ClipboardNotifier.Windows;

    using Prism.Mvvm;

    /// <summary>
    /// ViewModel class for MainWindow.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private Timer timer;
        private Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="clipboardMonitor"></param>
        public MainWindowViewModel(ClipboardMonitor clipboardMonitor)
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();

            this.timer = new Timer(50);
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Start();

            clipboardMonitor.ClipboardChanged += this.ClipboardMonitor_ClipboardChanged;
        }

        /// <summary>
        /// Gets or sets clipboard histories.
        /// </summary>
        public ObservableCollection<ClipboardHistoryItemViewModel> Histories { get; set; }
            = new ObservableCollection<ClipboardHistoryItemViewModel>();

        private void Timer_Elapsed(object sender, ElapsedEventArgs ev)
        {
            while (true)
            {
                if (this.Histories.Count == 0)
                {
                    break;
                }

                var item = this.Histories[0];
                var elapsed = this.stopwatch.Elapsed.TotalSeconds - item.OcceredTime;
                if (elapsed > 5.0)
                {
                    this.RemoveItem(item);
                }
                else if (elapsed > 4.75)
                {
                    item.IsDisappearing = true;
                }
                else
                {
                    break;
                }
            }
        }

        private void ClipboardMonitor_ClipboardChanged(object sender, EventArgs ev)
        {
            if (Clipboard.ContainsFileDropList())
            {
                var list = Clipboard.GetFileDropList();
                var text = string.Empty;
                if (list.Count >= 2)
                {
                    text = Path.GetFileName(list[0]) + $" and {list.Count - 1} others";
                }
                else if (list.Count == 1)
                {
                    text = Path.GetFileName(list[0]);
                }

                var item = new ClipboardHistoryItemViewModel()
                {
                    ClipboardDataType = ClipboardDataType.File,
                    Text = text,
                    OcceredTime = this.stopwatch.Elapsed.TotalSeconds,
                };
                this.AddItem(item);
            }
            else if (Clipboard.ContainsImage())
            {
                // https://stackoverflow.com/questions/25749843/wpf-image-source-clipboard-getimage-is-not-displayed
                var data = System.Windows.Forms.Clipboard.GetDataObject();
                if (data != null)
                {
                    if (data.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
                    {
                        var bitmap = (System.Drawing.Bitmap)data.GetData(System.Windows.Forms.DataFormats.Bitmap);
                        var hBitmap = bitmap.GetHbitmap();
                        try
                        {
                            var image = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            var item = new ClipboardHistoryItemViewModel()
                            {
                                ClipboardDataType = ClipboardDataType.Image,
                                Image = image,
                                OcceredTime = this.stopwatch.Elapsed.TotalSeconds,
                            };
                            this.AddItem(item);
                        }
                        finally
                        {
                            NativeMethods.DeleteObject(hBitmap);
                        }
                    }
                }
                else
                {
                    var item = new ClipboardHistoryItemViewModel()
                    {
                        ClipboardDataType = ClipboardDataType.Image,
                        Image = Clipboard.GetImage(),
                        OcceredTime = this.stopwatch.Elapsed.TotalSeconds,
                    };
                    this.AddItem(item);
                }
            }
            else if (Clipboard.ContainsAudio())
            {
                var item = new ClipboardHistoryItemViewModel()
                {
                    ClipboardDataType = ClipboardDataType.Audio,
                    OcceredTime = this.stopwatch.Elapsed.TotalSeconds,
                };
                this.AddItem(item);
            }
            else if (Clipboard.ContainsText())
            {
                var item = new ClipboardHistoryItemViewModel()
                {
                    ClipboardDataType = ClipboardDataType.Text,
                    Text = Clipboard.GetText(),
                    OcceredTime = this.stopwatch.Elapsed.TotalSeconds,
                };
                this.AddItem(item);
            }
        }

        private void AddItem(ClipboardHistoryItemViewModel item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.Histories.Count == 0)
                {
                    item.IsFirst = true;
                }

                this.Histories.Add(item);
            });
        }

        private void RemoveItem(ClipboardHistoryItemViewModel item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Histories.Remove(item);

                if (this.Histories.Count > 0)
                {
                    this.Histories[0].IsFirst = true;
                }
            });
        }
    }
}
