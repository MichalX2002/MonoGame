// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;
using System;

namespace MonoGame.Framework.Graphics
{
    public abstract partial class BufferBase : GraphicsResource
    {
        internal GLHandle _handle;
        internal BufferUsageHint _usageHint;

        private BufferTarget _target;
        private int _elementSize;

        /// <inheritdoc/>
        protected override void GraphicsDeviceResetting()
        {
            _handle = default;
        }

        internal virtual void DiscardBuffer(BufferTarget target, SetDataOptions options, int byteSize)
        {
            if (options == SetDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                GL.BufferData(target, (IntPtr)byteSize, IntPtr.Zero, _usageHint);
                GL.CheckError();
            }
        }

        internal virtual void PlatformConstruct(BufferTarget target, int elementSize)
        {
            _target = target;
            _elementSize = elementSize;
            _usageHint = IsDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw;

            GenerateIfRequired();
        }

        internal void GenerateIfRequired()
        {
            if (!_handle.IsNull)
                return;

            GL.GenBuffers(1, out int buffer);
            GL.CheckError();

            GL.BindBuffer(_target, buffer);
            GL.CheckError();

            int sizeInBytes = Capacity * _elementSize;
            GL.BufferData(_target, (IntPtr)sizeInBytes, IntPtr.Zero, _usageHint);
            GL.CheckError();

            _handle = GLHandle.Buffer(buffer);
        }

        /// <summary>
        /// Releases resources associated with this buffer.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if managed objects should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
                GraphicsDevice.DisposeResource(_handle);

            base.Dispose(disposing);
        }
    }
}
