// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class UnsafeIndexBuffer : IndexBufferBase
    {
		internal int _ibo;
        internal override int IBO => _ibo;

        private long _lastBufferSize;

        private void PlatformConstruct()
        {
            Threading.BlockOnUIThread(GenerateIfRequired);
        }

        private void PlatformGraphicsDeviceResetting()
        {
            _ibo = 0;
        }

        /// <summary>
        /// If the IBO does not exist, create it.
        /// </summary>
        void GenerateIfRequired()
        {
            if (_ibo == 0)
            {
                GL.GenBuffers(1, out _ibo);
                GraphicsExtensions.CheckGLError();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformGetData(IntPtr buffer, int startIndex, int elementCount)
        {
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Index buffers are write-only on OpenGL ES platforms.");
#endif
#if !GLES
            Threading.BlockOnUIThread(() => GetBufferData(buffer, startIndex, elementCount));
#endif
        }

#if !GLES
        private void GetBufferData(IntPtr buffer, int startIndex, int elementCount)
        {
            if (IndexCount == 0)
                throw new InvalidOperationException("There is no data to retrieve.");

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GraphicsExtensions.CheckGLError();

            int elementSize = GetIndexElementSize();
            int bufferSize = elementCount * elementSize;
            int bytesToCopy = (IndexCount - startIndex) * elementSize;
            IntPtr data = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.ReadOnly);
            unsafe
            {
                void* srcPtr = (void*)(new IntPtr(data.ToInt64() + startIndex * elementSize));
                void* dstPtr = (void*)(buffer);
                Buffer.MemoryCopy(srcPtr, dstPtr, bufferSize, bytesToCopy);
            }
            GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
            GraphicsExtensions.CheckGLError();
        }
#endif

        private void PlatformSetData(IntPtr buffer, int startIndex, int elementCount, SetDataOptions options)
        {
            Threading.BlockOnUIThread(() => BufferData(buffer, startIndex, elementCount, options));
        }

        private void BufferData(IntPtr data, int startIndex, int elementCount, SetDataOptions options)
        {
            GenerateIfRequired();
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GraphicsExtensions.CheckGLError();

            int elementSize = GetIndexElementSize();
            IntPtr size = new IntPtr(elementCount * elementSize);
            IntPtr offset = new IntPtr(startIndex * elementSize);

            if (options == SetDataOptions.Discard || _lastBufferSize < size.ToInt64())
            {
                // Hint device to discard the previous content by passing IntPtr.Zero as data.
                var usage = _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw;
                GL.BufferData(BufferTarget.ElementArrayBuffer, size, IntPtr.Zero, usage);
                GraphicsExtensions.CheckGLError();

                _lastBufferSize = size.ToInt64();
            }
            
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, offset, size, data);
            GraphicsExtensions.CheckGLError();

            IndexCount = elementCount;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                GraphicsDevice.DisposeBuffer(_ibo);
            }
            base.Dispose(disposing);
        }
	}
}
