// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class VertexBuffer : VertexBufferBase
    {
		//internal uint vao;
		internal int _vbo;
        internal override int VBO => _vbo;

        private void PlatformConstruct()
        {
            Threading.BlockOnUIThread(GenerateIfRequired);
        }

        private void PlatformGraphicsDeviceResetting()
        {
            _vbo = 0;
        }

        /// <summary>
        /// If the VBO does not exist, create it.
        /// </summary>
        void GenerateIfRequired()
        {
            if (_vbo == 0)
            {
                //GLExt.Oes.GenVertexArrays(1, out this.vao);
                //GLExt.Oes.BindVertexArray(this.vao);
                GL.GenBuffers(1, out this._vbo);
                GraphicsExtensions.CheckGLError();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this._vbo);
                GraphicsExtensions.CheckGLError();
                GL.BufferData(BufferTarget.ArrayBuffer,
                              new IntPtr(VertexDeclaration.VertexStride * VertexCount), IntPtr.Zero,
                              _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformGetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct
        {
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Vertex buffers are write-only on OpenGL ES platforms");
#else
            Threading.BlockOnUIThread (() => GetBufferData(offsetInBytes, data, startIndex, elementCount, vertexStride));
#endif
        }

#if !GLES

        private void GetBufferData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();

            // Pointer to the start of data in the vertex buffer
            var ptr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            GraphicsExtensions.CheckGLError();

            ptr = (IntPtr) (ptr.ToInt64() + offsetInBytes);

            if (vertexStride == 1 && data is byte[] byteBuffer)
            {
                // If data is already a byte[] and stride is 1 we can skip the temporary buffer
                Marshal.Copy(ptr, byteBuffer, startIndex * vertexStride, elementCount * vertexStride);
            }
            else
            {
                // Copy from vertex buffer to data
                for (var i = 0; i < elementCount; i++)
                {
                    data[startIndex + i] = (T)Marshal.PtrToStructure(ptr, typeof(T));
                    ptr += vertexStride;
                }
            }

            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            GraphicsExtensions.CheckGLError();
        }
        
#endif

        private void PlatformSetDataInternal<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride, SetDataOptions options, int bufferSize, int elementSizeInBytes) where T : struct
        {
            Threading.BlockOnUIThread(() => SetBufferData(bufferSize, elementSizeInBytes, offsetInBytes, data, startIndex, elementCount, vertexStride, options));
        }

        private void SetBufferData<T>(int bufferSize, int elementSizeInBytes, int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride, SetDataOptions options) where T : struct
        {
            GenerateIfRequired();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();
            
            if (options == SetDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                GL.BufferData(BufferTarget.ArrayBuffer,
                              (IntPtr)bufferSize, 
                              IntPtr.Zero,
                              _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw);
                GraphicsExtensions.CheckGLError();
            }

            var elementSizeInByte = Marshal.SizeOf(typeof(T));
            if (elementSizeInByte == vertexStride || elementSizeInByte % vertexStride == 0)
            {
                // there are no gaps so we can copy in one go
                var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                var dataPtr = (IntPtr) (dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * elementSizeInBytes);
                try
                {
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr) offsetInBytes, (IntPtr) (elementSizeInBytes * elementCount), dataPtr);
                    GraphicsExtensions.CheckGLError();
                }
                finally
                {
                    dataHandle.Free();
                }
            }
            else
            {
                // else we must copy each element separately
                var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    var dstOffset = offsetInBytes;
                    var dataPtr = (IntPtr) (dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * elementSizeInByte);
                    for (var i = 0; i < elementCount; i++)
                    {
                        GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr) dstOffset, (IntPtr) elementSizeInByte, dataPtr);
                        GraphicsExtensions.CheckGLError();

                        dstOffset += vertexStride;
                        dataPtr = (IntPtr) (dataPtr.ToInt64() + elementSizeInByte);
                    }
                }
                finally
                {
                    dataHandle.Free();
                }
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                GraphicsDevice.DisposeBuffer(_vbo);
            }
            base.Dispose(disposing);
        }
    }
}
