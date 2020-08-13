// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public partial class IndexBuffer : BufferBase
    {
        private void PlatformConstruct()
        {
            base.PlatformConstruct(BufferTarget.ElementArrayBuffer, (int)ElementType);
        }

        private unsafe void PlatformGetData<T>(int byteOffset, Span<T> destination) 
            where T : unmanaged
        {
#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0. See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Index buffers are write-only on OpenGL ES platforms");
#else
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _glBuffer);
            GL.CheckError();

            IntPtr mapPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.ReadOnly);

            int srcBytes = Capacity * (int)ElementType;
            var byteSrc = new ReadOnlySpan<byte>((byte*)mapPtr + byteOffset, srcBytes);
            var byteDst = MemoryMarshal.AsBytes(destination);
            byteSrc.Slice(0, byteDst.Length).CopyTo(byteDst);

            GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
            GL.CheckError();
#endif
        }

        private unsafe void PlatformSetData<T>(
            int byteOffset, ReadOnlySpan<T> data, SetDataOptions options)
            where T : unmanaged
        {
            GenerateIfRequired();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _glBuffer);
            GL.CheckError();

            DiscardBuffer(BufferTarget.ElementArrayBuffer, options, Capacity * (int)ElementType);

            fixed (T* ptr = data)
            {
                int size = data.Length * sizeof(T);
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)byteOffset, (IntPtr)size, (IntPtr)ptr);
                GL.CheckError();
            }
        }
    }
}
