// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Defines the buffers for clearing when calling 
    /// <see cref="GraphicsDevice.Clear(ClearOptions, Color, float, int)"/>.
    /// </summary>
    [Flags]
    public enum ClearOptions
    {
        /// <summary>
        /// Color buffer.
        /// </summary>
        Target = 1,

        /// <summary>
        /// Depth buffer.
        /// </summary>
        DepthBuffer = 2,

        /// <summary>
        /// Stencil buffer.
        /// </summary>
        Stencil = 4,

        /// <summary>
        /// All buffers.
        /// </summary>
        Full = Target | DepthBuffer | Stencil
    }
}

