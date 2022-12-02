// -----------------------------------------------------------------------
// <copyright file="WindowMouseTransparentBehavior.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    using ClipboardNotifier.Controls;
    using ClipboardNotifier.Windows;
    using ClipboardNotifier.Windows.NativeMethods;

    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// Changes <see cref="Window"/>'s mouse transparency of the attached window.
    /// </summary>
    public class WindowMouseTransparentBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Attached property of IsEnabled.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                nameof(IsEnabled),
                typeof(bool),
                typeof(WindowMouseTransparentBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        /// <summary>
        /// Attached property of IsMouseMover.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register(
                nameof(IsMouseOver),
                typeof(bool),
                typeof(WindowMouseTransparentBehavior),
                new PropertyMetadata(false));

        private MouseHook mouseHook;
        private bool transparent;

        /// <summary>
        /// Gets or sets a value indicating whether the window is mouse transparent.
        /// </summary>
        public bool IsEnabled
        {
            get => (bool)this.GetValue(IsEnabledProperty);
            set => this.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse is over associated object.
        /// </summary>
        public bool IsMouseOver
        {
            get => (bool)this.GetValue(IsMouseOverProperty);
            set => this.SetValue(IsMouseOverProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            var window = this.FindWindow();
            if (window != null)
            {
                this.transparent = WindowHelper.IsTransparent(window);
            }
            else
            {
                this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
            }

            this.mouseHook = new MouseHook();
            var targetMessage = WindowsHook.MouseMessages.WM_MOUSEMOVE;
            this.mouseHook.SetHook(targetMessage, this.OnMouseMove);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            this.mouseHook?.UnsetHook();

            this.IsMouseOver = false;

            base.OnDetaching();
        }

        private static void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs ev)
        {
            var behavior = sender as WindowMouseTransparentBehavior;
            if (behavior != null)
            {
                var enable = (bool)ev.NewValue;
                if (enable)
                {
                    if (behavior.AssociatedObject != null)
                    {
                        var cursorPos = behavior.AssociatedObject.PointToScreen(Mouse.GetPosition(behavior.AssociatedObject));
                        behavior.OnMouseMove(cursorPos);
                    }
                }
                else
                {
                    behavior.UpdateTransparency(false);
                }
            }
        }

        private void OnMouseMove(WindowsHook.MSLLHOOKSTRUCT hookStruct)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.OnMouseMove(new Point(hookStruct.pt.x, hookStruct.pt.y));
        }

        private void OnMouseMove(Point mouse)
        {
            var window = this.FindWindow();
            if (window == null || window.IsVisible == false)
            {
                this.IsMouseOver = false;
                return;
            }

            var screenPos = window.PointToScreen(new Point(0, 0));
            var pos = new Point(mouse.X - screenPos.X, mouse.Y - screenPos.Y);
            var windowRect = new Rect(0, 0, window.ActualWidth, window.ActualHeight);
            var element = window.InputHitTest(pos) as Visual;
            var inside = windowRect.Contains(pos) && element != null && this.AssociatedObject.IsAncestorOf(element);
            this.IsMouseOver = inside;

            this.UpdateTransparency(inside);
        }

        private void UpdateTransparency(bool transparent)
        {
            if (this.transparent == transparent)
            {
                return;
            }

            var window = this.FindWindow();
            if (window != null)
            {
                if (transparent)
                {
                    WindowHelper.SetTransparent(window);
                }
                else
                {
                    WindowHelper.UnsetTransprent(window);
                }
            }

            this.transparent = transparent;
        }

        private Window FindWindow()
        {
            return ControlHelper.GetParentOfType<Window>(this.AssociatedObject);
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs ev)
        {
            this.AssociatedObject.Loaded -= this.AssociatedObject_Loaded;

            var window = this.FindWindow();
            if (window != null)
            {
                this.transparent = WindowHelper.IsTransparent(window);
            }
        }
    }
}
