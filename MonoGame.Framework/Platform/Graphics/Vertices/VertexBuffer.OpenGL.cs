using MonoGame.OpenGL;
using System;

namespace MonoGame.Framework.Graphics
{
    public partial class VertexBuffer : BufferBase
    {
        private void PlatformConstruct()
        {
            base.PlatformConstruct(BufferTarget.ElementArrayBuffer, VertexDeclaration.VertexStride);
        }

        private unsafe void PlatformGetData<T>(
            int offsetInBytes, Span<T> destination, int elementStride)
            where T : unmanaged
        {
            AssertOnMainThreadForSpan();
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Vertex buffers are write-only on OpenGL ES platforms");
#else
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();

            // Pointer to the start of data in the vertex buffer
            IntPtr mapPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            GraphicsExtensions.CheckGLError();

            int bufferBytes = Capacity * VertexDeclaration.VertexStride;
            var src = new ReadOnlySpan<T>((void*)(mapPtr + offsetInBytes), bufferBytes);
            
            if (sizeof(T) % elementStride == 0)
            {
                // the source and destination use tightly packed data,
                // we can skip the interleaved copy
                src.Slice(0, destination.Length).CopyTo(destination);
            }
            else
            {
                var byteSrc = System.Runtime.InteropServices.MemoryMarshal.AsBytes(src);
                var byteDst = System.Runtime.InteropServices.MemoryMarshal.AsBytes(destination);

                // interleaved copy from buffer to destination
                for (int i = 0; i < destination.Length; i++)
                {
                    var srcElement = byteSrc.Slice(i * VertexDeclaration.VertexStride, elementStride);
                    var dstElement = byteDst.Slice(i * elementStride, elementStride);
                    srcElement.CopyTo(dstElement);
                }
            }

            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            GraphicsExtensions.CheckGLError();
#endif
        }

        private unsafe void PlatformSetData<T>(
            int offsetInBytes, ReadOnlySpan<T> data, int dataStride, SetDataOptions options)
            where T : unmanaged
        {
            AssertOnMainThreadForSpan();
            GenerateIfRequired();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GraphicsExtensions.CheckGLError();

            DiscardBuffer(BufferTarget.ArrayBuffer, options, Capacity * VertexDeclaration.VertexStride);

            fixed (T* ptr = data)
            {
                if (sizeof(T) % dataStride == 0)
                {
                    // there are no gaps so we can copy in one go
                    var size = (IntPtr)(sizeof(T) * data.Length);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offsetInBytes, size, (IntPtr)ptr);
                    GraphicsExtensions.CheckGLError();
                }
                else
                {
                    // else we must copy each element separately
                    var elementSize = (IntPtr)sizeof(T);
                    var ptrWithOffset = (IntPtr)ptr;
                    for (int i = 0; i < data.Length; i++)
                    {
                        var offset = new IntPtr(offsetInBytes + i * dataStride);
                        GL.BufferSubData(BufferTarget.ArrayBuffer, offset, elementSize, ptrWithOffset);
                        GraphicsExtensions.CheckGLError();

                        ptrWithOffset += sizeof(T);
                    }
                }
            }
        }
    }
}
