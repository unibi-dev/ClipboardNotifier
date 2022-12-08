// -----------------------------------------------------------------------
// <copyright file="ClipboardDataType.cs" company="unibi.dev">
// Copyright (c) unibi.dev. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace ClipboardNotifier.Models
{
    using System;

    /// <summary>
    /// Enum indicating the data type in the clipboard.
    /// </summary>
    public enum ClipboardDataType
    {
        Text,
        Image,
        File,
        Audio,
        Others,
    }
}
