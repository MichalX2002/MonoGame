// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;
using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class IndexBufferBase : BufferBase
    {
        protected override void PlatformConstruct()
        {
            _indexElementSize = this.IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4;

            base.PlatformConstruct();
        }

        /// <summary>
        /// If the IBO does not exist, create it.
        /// </summary>
        protected override void GenerateIfRequired()
        {
            if (_vbo == 0)
            {
                GL.GenBuffers(1, out _vbo);
                GraphicsExtensions.CheckGLError();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vbo);
                GraphicsExtensions.CheckGLError();

                int sizeInBytes = IndexCount * _indexElementSize;
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)sizeInBytes, IntPtr.Zero, _usageHint);
                GraphicsExtensions.CheckGLError();
            }
        }

        protected void PlatformGetData(int offsetInBytes, IntPtr ptr, int startIndex, int elementCount)
        {
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0. See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Index buffers are write-only on OpenGL ES platforms");
#else
            if (Threading.IsOnUIThread()) // to not create unnecessary Action garbage with BlockOnUIThread
                GetDataInternal(offsetInBytes, ptr, startIndex, elementCount);
            else
                Threading.BlockOnUIThread(() => { GetDataInternal(offsetInBytes, ptr, startIndex, elementCount); });
#endif
        }

        private void GetDataInternal(int offsetInBytes, IntPtr ptr, int startIndex, int elementCount)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();

            IntPtr mapPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.ReadOnly);
            // Pointer to the start of data to read in the index buffer
            mapPtr = new IntPtr(mapPtr.ToInt64() + offsetInBytes);

            int startOffset = startIndex * _indexElementSize;
            int bytes = elementCount * _indexElementSize - startOffset;
            unsafe
            {
                Buffer.MemoryCopy((byte*)mapPtr, (byte*)ptr, bytes, bytes);
            }

            GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
            GraphicsExtensions.CheckGLError();
        }

        protected void PlatformSetData(
            int offsetInBytes, IntPtr data, int startIndex, int elementCount, SetDataOptions options)
        {
            if (Threading.IsOnUIThread()) // to not create unnecessary Action garbage with BlockOnUIThread
                SetDataInternal(offsetInBytes, data, startIndex, elementCount, options);
            else
                Threading.BlockOnUIThread(() => { SetDataInternal(offsetInBytes, data, startIndex, elementCount, options); });
        }

        private void SetDataInternal(
            int offsetInBytes, IntPtr data, int startIndex, int elementCount, SetDataOptions options)
        {
            if (options != SetDataOptions.Discard && IndexCount < elementCount)
                throw new ArgumentException(
                    $"Options suggested not overwriting and the buffer (of size " +
                    $"{IndexCount}) was too small for {nameof(elementCount)} ({elementCount})." +
                    $"Use {nameof(SetDataOptions.Discard)} to allow resizing of the buffer.", nameof(options));

            GenerateIfRequired();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();

            int bufferSize = elementCount * _indexElementSize;
            DiscardCheck(BufferTarget.ElementArrayBuffer, options, (IntPtr)bufferSize);
            
            IntPtr size = new IntPtr(_indexElementSize * elementCount);
            IntPtr dataPtr = new IntPtr(data.ToInt64() + startIndex * _indexElementSize);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)offsetInBytes, size, dataPtr);
            GraphicsExtensions.CheckGLError();
        }
    
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
                GraphicsDevice.DisposeBuffer(_vbo);
            
            base.Dispose(disposing);
        }
    }
}
