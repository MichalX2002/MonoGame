// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class UnsafeIndexBuffer : IndexBufferBase
    {
        private long _lastBufferSize;

        internal override void DiscardCheck(BufferTarget target, SetDataOptions options, IntPtr bufferSize)
        {
            long bufferSize64 = bufferSize.ToInt64();
            if (options == SetDataOptions.Discard || _lastBufferSize < bufferSize64)
            {
                // Hint device to discard the previous content by passing IntPtr.Zero as data.
                GL.BufferData(target, bufferSize, IntPtr.Zero, _usageHint);
                GraphicsExtensions.CheckGLError();

                _lastBufferSize = bufferSize64;
            }
        }
    }
}
