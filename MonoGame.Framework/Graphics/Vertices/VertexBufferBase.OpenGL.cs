using MonoGame.OpenGL;
using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class VertexBufferBase : BufferBase
    {
        /// <summary>
        /// If the VBO does not exist, create it.
        /// </summary>
        protected override void GenerateIfRequired()
        {
            if (_vbo == 0)
            {
                //GLExt.Oes.GenVertexArrays(1, out this.vao);
                //GLExt.Oes.BindVertexArray(this.vao);
                GL.GenBuffers(1, out this._vbo);
                GraphicsExtensions.CheckGLError();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this._vbo);
                GraphicsExtensions.CheckGLError();

                IntPtr size = new IntPtr(VertexDeclaration.VertexStride * VertexCount);
                GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, _usageHint);
                GraphicsExtensions.CheckGLError();
            }
        }

        protected void PlatformGetData(int offsetInBytes, IntPtr ptr,
            int startIndex, int elementCount, int elementSize, int vertexStride)
        {
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Vertex buffers are write-only on OpenGL ES platforms");
#else
            Threading.BlockOnUIThread(() =>
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GraphicsExtensions.CheckGLError();

                // Pointer to the start of data in the vertex buffer
                IntPtr mapPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
                GraphicsExtensions.CheckGLError();

                unsafe
                {
                    int startByteOffset = startIndex * vertexStride;
                    byte* byteMapPtr = (byte*)(mapPtr + offsetInBytes);
                    byte* outputPtr = (byte*)(ptr + startByteOffset);

                    if (vertexStride == 1 && elementSize == 1)
                    {
                        // If data is already in bytes and stride is 1 we can skip the temporary buffer
                        int bytes = elementCount * vertexStride - startByteOffset;
                        Buffer.MemoryCopy(byteMapPtr, outputPtr, bytes, bytes);
                    }
                    else
                    {
                        // Copy from vertex buffer to data
                        for (int i = 0; i < elementCount; i++)
                        {
                            Buffer.MemoryCopy(byteMapPtr, outputPtr + i * elementSize, elementSize, elementSize);
                            byteMapPtr += vertexStride;
                        }
                    }
                }

                GL.UnmapBuffer(BufferTarget.ArrayBuffer);
                GraphicsExtensions.CheckGLError();
            });
#endif
        }

        protected void PlatformSetData(
            int offsetInBytes, IntPtr ptr, int startIndex,
            int elementCount, int elementSize, int vertexStride, SetDataOptions options)
        {
            Threading.BlockOnUIThread(() =>
            {
                GenerateIfRequired();

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GraphicsExtensions.CheckGLError();

                IntPtr bufferSize = (IntPtr)(elementSize * elementCount);
                DiscardCheck(BufferTarget.ArrayBuffer, options, bufferSize);

                IntPtr dataPtr = ptr + startIndex * elementSize;
                if (elementSize == vertexStride || elementSize % vertexStride == 0)
                {
                    // there are no gaps so we can copy in one go
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offsetInBytes, bufferSize, dataPtr);
                    GraphicsExtensions.CheckGLError();
                }
                else
                {
                    IntPtr eSize = (IntPtr)elementSize;
                    IntPtr dataPtrWithOffset = dataPtr;

                    // else we must copy each element separately
                    for (var i = 0; i < elementCount; i++)
                    {
                        IntPtr offset = new IntPtr(offsetInBytes + i * vertexStride);
                        GL.BufferSubData(BufferTarget.ArrayBuffer, offset, eSize, dataPtrWithOffset);
                        GraphicsExtensions.CheckGLError();

                        dataPtrWithOffset += elementSize;
                    }
                }
            });
        }
    }
}
