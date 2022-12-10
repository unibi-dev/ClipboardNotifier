// -----------------------------------------------------------------------
// <copyright file="AppNotifyIcon.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Manages application's notify icon.
    /// </summary>
    public class AppNotifyIcon : IDisposable
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private List<ToolStripItem> items;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppNotifyIcon"/> class.
        /// </summary>
        /// <param name="icon">Icon shown in tray.</param>
        public AppNotifyIcon(Icon icon)
        {
            this.notifyIcon = new NotifyIcon()
            {
                Icon = icon,
            };

            this.contextMenu = new ContextMenuStrip();
            this.notifyIcon.ContextMenuStrip = this.contextMenu;

            this.items = new List<ToolStripItem>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AppNotifyIcon"/> class.
        /// </summary>
        ~AppNotifyIcon()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occurs when the user clicks a <see cref="AppNotifyIcon"/>.
        /// </summary>
        public event EventHandler Clicked
        {
            add => this.notifyIcon.Click += value;
            remove => this.notifyIcon.Click -= value;
        }

        /// <summary>
        /// Occurs when the user opens the context menu.
        /// </summary>
        public event EventHandler Opened
        {
            add => this.contextMenu.Opened += value;
            remove => this.contextMenu.Opened -= value;
        }

        /// <summary>
        /// Gets or sets the ToolTip text displayed when the mouse pointer rests on a notification area icon.
        /// </summary>
        public string Text
        {
            get => this.notifyIcon.Text;
            set => this.notifyIcon.Text = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the icon is visible in the notification area of the taskbar.
        /// The default value is false.
        /// </summary>
        public bool IsVisible
        {
            get => this.notifyIcon.Visible;
            set => this.notifyIcon.Visible = value;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds a item to context menu.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="action"></param>
        /// <returns>added item.</returns>
        public ToolStripMenuItem AddItem(string text, Action action = null)
        {
            var item = new ToolStripMenuItem(text);
            if (action != null)
            {
                item.Click += (s_, e_) => action.Invoke();
            }

            this.items.Add(item);
            this.contextMenu.Items.Add(item);

            return item;
        }

        /// <summary>
        /// Adds a separator to context menu.
        /// </summary>
        /// <returns>added item.</returns>
        public ToolStripSeparator AddSeparator()
        {
            var item = new ToolStripSeparator();
            this.contextMenu.Items.Add(item);

            return item;
        }

        /// <summary>
        /// Implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Dispose managed objects.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
                this.notifyIcon.Dispose();

                foreach (var item in this.items)
                {
                    item.Dispose();
                }
            }

            // Free any unmanaged objects here.
            this.disposed = true;
        }
    }
}
