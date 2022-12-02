// -----------------------------------------------------------------------
// <copyright file="GDIMethods.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Windows.NativeMethods
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native methods for GDI.
    /// </summary>
    internal static class GDIMethods
    {
#pragma warning disable SA1600 // Elements should be documented
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
#pragma warning restore SA1600 // Elements should be documented
    }
}
