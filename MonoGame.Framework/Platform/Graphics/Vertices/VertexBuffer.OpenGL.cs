using MonoGame.OpenGL;
using System;
using System.Runtime.InteropServices;

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
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Vertex buffers are write-only on OpenGL ES platforms");
#else
            GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
            GL.CheckError();

            // Pointer to the start of data in the vertex buffer
            IntPtr mapPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            GL.CheckError();

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
            GL.CheckError();
#endif
        }

        private unsafe void PlatformSetData<T>(
            int offsetInBytes, ReadOnlySpan<T> data, int dataStride, SetDataOptions options)
            where T : unmanaged
        {
            GenerateIfRequired();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
            GL.CheckError();

            DiscardBuffer(BufferTarget.ArrayBuffer, options, Capacity * VertexDeclaration.VertexStride);

            if (sizeof(T) % dataStride == 0)
            {
                fixed (T* dataPtr = data)
                {
                    // there are no gaps so we can copy in one go
                    var size = (IntPtr)(sizeof(T) * data.Length);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offsetInBytes, size, (IntPtr)dataPtr);
                    GL.CheckError();
                }
            }
            else
            {
                // else we must copy each element separately
                int bufferSize = Math.Max(1, 2048 / sizeof(T));
                int bufferLength = bufferSize * sizeof(T);
                byte* buffer = stackalloc byte[bufferLength];
                var bufferSpan = new Span<byte>(buffer, bufferLength);

                var dataBytes = MemoryMarshal.AsBytes(data);
                int elementOffset = 0;
                int left = data.Length;
                while (left > 0)
                {
                    int elementsToCopy = Math.Min(bufferSize, left);
                    int copyByteSize = elementsToCopy * sizeof(T);
                    dataBytes.Slice(elementOffset * sizeof(T), copyByteSize).CopyTo(bufferSpan);

                    var offset = new IntPtr(offsetInBytes + elementOffset * dataStride);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, offset, (IntPtr)copyByteSize, (IntPtr)buffer);
                    GL.CheckError();

                    elementOffset += elementsToCopy;
                    left -= elementsToCopy;
                }
            }
        }
    }
}