// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;
using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class BufferBase : GraphicsResource
    {
        internal int _vbo;
        internal BufferUsageHint _usageHint;

        internal protected override void GraphicsDeviceResetting()
        {
            _vbo = 0;
        }

        internal virtual void DiscardCheck(BufferTarget target, SetDataOptions options, IntPtr bufferSize)
        {
            if (options == SetDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                GL.BufferData(target, bufferSize, IntPtr.Zero, _usageHint);
                GraphicsExtensions.CheckGLError();
            }
        }

        protected virtual void PlatformConstruct()
        {
            _usageHint = _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw;

            if (Threading.IsOnUIThread())
                GenerateIfRequired();
            else
                Threading.BlockOnUIThread(GenerateIfRequired);
        }

        protected abstract void GenerateIfRequired();
    }
}
