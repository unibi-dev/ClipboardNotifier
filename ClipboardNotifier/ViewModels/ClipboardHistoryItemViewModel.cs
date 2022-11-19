// -----------------------------------------------------------------------
// <copyright file="ClipboardHistoryItemViewModel.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.ViewModels
{
    using System;
    using System.Windows.Media.Imaging;

    using ClipboardNotifier.Models;

    using Prism.Mvvm;

    /// <summary>
    /// ViewModel class of a history item.
    /// </summary>
    public class ClipboardHistoryItemViewModel : BindableBase
    {
        private bool isFirst;
        private bool isDisappearing;

        /// <summary>
        /// Gets or sets the data type of this item.
        /// </summary>
        public ClipboardDataType ClipboardDataType { get; set; }

        /// <summary>
        /// Gets or sets text data from the clipboard.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets image data from the clipboard.
        /// </summary>
        public BitmapSource Image { get; set; }

        /// <summary>
        /// Gets or sets the event occered time in seconds.
        /// </summary>
        public double OcceredTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is the first item.
        /// </summary>
        public bool IsFirst
        {
            get => this.isFirst;
            set => this.SetProperty(ref this.isFirst, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is about to disappear.
        /// </summary>
        public bool IsDisappearing
        {
            get => this.isDisappearing;
            set => this.SetProperty(ref this.isDisappearing, value);
        }
    }
}
