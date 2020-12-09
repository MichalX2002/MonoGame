using MonoGame.OpenGL;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    public partial class VertexBuffer : BufferBase
    {
        private void PlatformConstruct()
        {
            base.PlatformConstruct(BufferTarget.ElementArrayBuffer, VertexDeclaration.VertexStride);
        }

        private unsafe void PlatformGetData(
            int offsetInBytes, Span<byte> destination, int stride)
        {
            if (GL.IsES)
            {
                // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
                // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
                throw new PlatformNotSupportedException("Vertex buffers are write-only on OpenGL ES platforms");
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _glBuffer);
            GL.CheckError();

            // Pointer to the start of data in the vertex buffer
            IntPtr mapPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            GL.CheckError();

            int bufferBytes = Capacity * VertexDeclaration.VertexStride;
            var src = new ReadOnlySpan<byte>((void*)(mapPtr + offsetInBytes), bufferBytes);

            if (stride % VertexDeclaration.VertexStride == 0)
            {
                // the source and destination use tightly packed data,
                // we can skip the interleaved copy
                src.Slice(0, destination.Length).CopyTo(destination);
            }
            else
            {
                var byteSrc = MemoryMarshal.AsBytes(src);
                var byteDst = MemoryMarshal.AsBytes(destination);

                // interleaved copy from buffer to destination
                for (int i = 0; i < destination.Length; i++)
                {
                    var srcElements = byteSrc.Slice(i * VertexDeclaration.VertexStride, stride);
                    var dstElements = byteDst.Slice(i * stride, stride);
                    srcElements.CopyTo(dstElements);
                }
            }

            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            GL.CheckError();
        }

        [SkipLocalsInit]
        private unsafe void PlatformSetData(
            int offsetInBytes, ReadOnlySpan<byte> data, int dataStride, SetDataOptions options)
        {
            GenerateIfRequired();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _glBuffer);
            GL.CheckError();

            DiscardBuffer(BufferTarget.ArrayBuffer, options, Capacity * VertexDeclaration.VertexStride);

            if (dataStride % VertexDeclaration.VertexStride == 0)
            {
                fixed (byte* dataPtr = data)
                {
                    // there are no gaps so we can copy in one go
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offsetInBytes, (IntPtr)data.Length, (IntPtr)dataPtr);
                    GL.CheckError();
                }
            }
            else
            {
                fixed (byte* dataPtr = data)
                {
                    nint dstOffset = offsetInBytes;
                    byte* ptr = dataPtr;
                    for (int i = 0; i < data.Length; i += dataStride)
                    {
                        GL.BufferSubData(BufferTarget.ArrayBuffer, dstOffset, (IntPtr)dataStride, (IntPtr)ptr);
                        GL.CheckError();
                
                        dstOffset += dataStride;
                        ptr += dataStride;
                    }
                }
            }
        }
    }
}