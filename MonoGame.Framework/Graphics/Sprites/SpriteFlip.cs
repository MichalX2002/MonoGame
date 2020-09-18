// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Defines options for sprite mirroring.
    /// </summary>
    [Flags]
    public enum SpriteFlip
    {
        /// <summary>
        /// No options specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Reverse the sprite along the X axis.
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// Reverse the sprite along the Y axis.
        /// </summary>
        Vertical = 2,

        /// <summary>
        /// Reverse along all axes.
        /// </summary>
        All = Horizontal | Vertical
    }
}