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

    using ClipboardNotifier.Controls;
    using ClipboardNotifier.Windows;

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

        private bool transparent;

        /// <summary>
        /// Gets or sets a value indicating whether the window is mouse transparent.
        /// </summary>
        public bool IsEnabled
        {
            get => (bool)this.GetValue(IsEnabledProperty);
            set => this.SetValue(IsEnabledProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            var window = this.FindWindow();
            if (window != null)
            {
                this.transparent = WindowHelper.IsTransparent(window);
                if (this.transparent != this.IsEnabled)
                {
                    this.UpdateTransparency(this.IsEnabled);
                }
            }
            else
            {
                this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
            }
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        private static void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs ev)
        {
            var behavior = sender as WindowMouseTransparentBehavior;
            if (behavior != null)
            {
                var enable = (bool)ev.NewValue;
                behavior.UpdateTransparency(enable);
            }
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
