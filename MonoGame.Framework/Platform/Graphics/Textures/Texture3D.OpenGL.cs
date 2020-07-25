// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using System;
using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture3D : Texture
    {
        private void PlatformConstruct(
            GraphicsDevice graphicsDevice, int width, int height, int depth, bool mipMap, SurfaceFormat format, bool renderTarget)
        {
#if GLES
            throw new NotSupportedException("OpenGL ES 2.0 doesn't support 3D textures.");
#else
            _glTarget = TextureTarget.Texture3D;

            if (mipMap)
                throw new NotImplementedException(nameof(Texture3D) + " does not yet support mipmaps.");

            _glTexture = GL.GenTexture();
            GL.CheckError();

            GL.BindTexture(_glTarget, _glTexture);
            GL.CheckError();

            format.GetGLFormat(GraphicsDevice, out _glInternalFormat, out _glFormat, out _glType);

            GL.TexImage3D(_glTarget, 0, _glInternalFormat, width, height, depth, 0, _glFormat, _glType, IntPtr.Zero);
            GL.CheckError();
#endif
        }

        private unsafe void PlatformSetData<T>(
            int level,
            int left, int top, int right, int bottom, int front, int back,
            int width, int height, int depth, ReadOnlySpan<T> data)
            where T : unmanaged
        {
#if GLES
            throw new NotSupportedException("OpenGL ES 2.0 doesn't support 3D textures.");
#else

            fixed (T* ptr = data)
            {
                GL.BindTexture(_glTarget, _glTexture);
                GL.CheckError();

                GL.TexSubImage3D(_glTarget, level, left, top, front, width, height, depth, _glFormat, _glType, (IntPtr)ptr);
                GL.CheckError();
            }
#endif
        }

        private void PlatformGetData<T>(
            int level, int left, int top, int right, int bottom, int front, int back, 
            Span<T> destination)
            where T : unmanaged
        {
            throw new NotImplementedException();
        }
    }
}

