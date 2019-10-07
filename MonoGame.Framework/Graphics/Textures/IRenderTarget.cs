// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Represents a render target.
    /// </summary>
    internal partial interface IRenderTarget
    {
        /// <summary>
        /// Gets the width of the render target in pixels
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the render target in pixels
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the usage mode of the render target.
        /// </summary>
        RenderTargetUsage RenderTargetUsage { get; }
    }
}
