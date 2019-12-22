// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;
using System;
using System.Diagnostics;

namespace MonoGame.Framework.Graphics
{
    public abstract partial class BufferBase : GraphicsResource
    {
        internal int _vbo;
        internal BufferUsageHint _usageHint;
        private BufferTarget _target;
        private int _elementSize;

        internal protected override void GraphicsDeviceResetting()
        {
            _vbo = 0;
        }

        internal virtual void DiscardBuffer(BufferTarget target, SetDataOptions options, int bufferBytes)
        {
            if (options == SetDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                GL.BufferData(target, (IntPtr)bufferBytes, IntPtr.Zero, _usageHint);
                GraphicsExtensions.CheckGLError();
            }
        }

        internal virtual void PlatformConstruct(BufferTarget target, int elementSize)
        {
            _target = target;
            _elementSize = elementSize;

            _usageHint = _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw;

            if (Threading.IsOnMainThread)
                GenerateIfRequired();
            else
                Threading.BlockOnMainThread(GenerateIfRequired);
        }

        internal void GenerateIfRequired()
        {
            if (_vbo == 0)
            {
                GL.GenBuffers(1, out _vbo);
                GraphicsExtensions.CheckGLError();

                GL.BindBuffer(_target, _vbo);
                GraphicsExtensions.CheckGLError();

                int sizeInBytes = Capacity * _elementSize;
                GL.BufferData(_target, (IntPtr)sizeInBytes, IntPtr.Zero, _usageHint);
                GraphicsExtensions.CheckGLError();
            }
        }
    }
}
