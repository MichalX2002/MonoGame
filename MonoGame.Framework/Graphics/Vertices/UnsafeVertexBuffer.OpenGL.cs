using MonoGame.OpenGL;
using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class UnsafeVertexBuffer : VertexBufferBase
    {
        internal int _vbo;
        internal override int VBO => _vbo;

        private long _lastBufferSize;

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
                GL.GenBuffers(1, out this._vbo);
                GraphicsExtensions.CheckGLError();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this._vbo);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformGetData(IntPtr buffer, int startIndex, int elementCount, int vertexStride)
        {
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Vertex buffers are write-only on OpenGL ES platforms.");
#else
            Threading.BlockOnUIThread(() => GetBufferData(buffer, startIndex, elementCount, vertexStride));
#endif
        }

#if !GLES
        private void GetBufferData(IntPtr buffer, int startIndex, int elementCount, int vertexStride)
        {
            if (VertexCount == 0)
                throw new InvalidOperationException("There is no data to retrieve.");

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();

            // Pointer to the start of data in the vertex buffer
            IntPtr data = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            GraphicsExtensions.CheckGLError();
            
            unsafe
            {
                int bufferSize = elementCount * vertexStride;
                int bytesToCopy = (VertexCount - startIndex) * vertexStride;
                void* srcPtr = (void*)(new IntPtr(data.ToInt64() + startIndex * vertexStride));

                if (vertexStride == 1)
                {
                    void* dstPtr = (void*)(buffer);
                    Buffer.MemoryCopy(srcPtr, dstPtr, bufferSize, bytesToCopy);
                }
                else
                {
                    IntPtr bufferOffset = buffer;
                    for (int i = 0; i < elementCount; i++)
                    {
                        Buffer.MemoryCopy(srcPtr, (void*)(bufferOffset), bufferSize, vertexStride);
                        bufferOffset += vertexStride;
                        bufferSize -= vertexStride;
                    }
                }
            }

            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            GraphicsExtensions.CheckGLError();
        }
#endif

        private void PlatformSetData(
            IntPtr data, int startIndex, int elementCount, int elementSize, int vertexStride, SetDataOptions options)
        {
            Threading.BlockOnUIThread(() => SetBufferData(
                data, startIndex, elementCount, elementSize, vertexStride, options));
        }

        private void SetBufferData(
            IntPtr data, int startIndex, int elementCount, int elementSize, int vertexStride, SetDataOptions options)
        {
            GenerateIfRequired();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();

            IntPtr offset = new IntPtr(startIndex * elementSize);
            IntPtr size = (IntPtr)(elementCount * elementSize);

            if (options == SetDataOptions.Discard || _lastBufferSize < size.ToInt64())
            {
                // Hint device to discard the previous content by passing IntPtr.Zero as data.
                var usage = _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw;
                GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, usage);
                GraphicsExtensions.CheckGLError();

                _lastBufferSize = size.ToInt64();
            }
            
            if (elementSize == vertexStride || elementSize % vertexStride == 0)
            {
                // There are no gaps; copy in one go.
                GL.BufferSubData(BufferTarget.ArrayBuffer, offset, size, data);
                GraphicsExtensions.CheckGLError();
            }
            else
            {
                IntPtr eSize = (IntPtr)elementSize;
                IntPtr dataPtr = data;
                for (var i = 0; i < elementCount; i++)
                {
                    GL.BufferSubData(BufferTarget.ArrayBuffer, offset, eSize, dataPtr);
                    GraphicsExtensions.CheckGLError();

                    offset += vertexStride;
                    dataPtr += elementSize;
                }
            }

            VertexCount = elementCount;
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
