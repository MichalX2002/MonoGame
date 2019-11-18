// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    /// <summary>
    /// Provides helpers for a <see cref="ContentReader"/>,
    /// like getting specific services.
    /// </summary>
    public static class ContentReaderExtensions
    {
        /// <summary>
        /// Gets the <see cref="GraphicsDevice"/> from the <see cref="ContentReader.ContentManager"/>.
        /// </summary>
        public static GraphicsDevice GetGraphicsDevice(this ContentReader contentReader)
        {
            var serviceProvider = contentReader.ContentManager.ServiceProvider;
            if (!(serviceProvider.GetService<IGraphicsDeviceService>() is IGraphicsDeviceService gds))
                throw new InvalidOperationException("No Graphics Device Service.");

            return gds.GraphicsDevice;
        }
    }
}
