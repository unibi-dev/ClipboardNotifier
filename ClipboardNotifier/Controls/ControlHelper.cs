// -----------------------------------------------------------------------
// <copyright file="ControlHelper.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Controls
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Helper class for Controls.
    /// </summary>
    internal static class ControlHelper
    {
        /// <summary>
        /// Finds an visual child with the specified type.
        /// </summary>
        /// <typeparam name="T">the type of the child to find.</typeparam>
        /// <param name="obj">the parent object.</param>
        /// <returns>the dependency object that found. may be null when no matching object found.</returns>
        public static T GetVisualChildOfType<T>(DependencyObject obj)
            where T : DependencyObject
        {
            if (obj == null)
            {
                return null;
            }

            var count = VisualTreeHelper.GetChildrenCount(obj);
            for (var childIndex = 0; childIndex < count; childIndex++)
            {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                var result = (child as T) ?? GetVisualChildOfType<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Find an visual parent with the specified type.
        /// </summary>
        /// <typeparam name="T">the type of the element to find.</typeparam>
        /// <param name="element">child object.</param>
        /// <returns>the element that found. may be null when no matching object found.</returns>
        public static T GetParentOfType<T>(DependencyObject element)
            where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T)
                {
                    return (T)element;
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}
