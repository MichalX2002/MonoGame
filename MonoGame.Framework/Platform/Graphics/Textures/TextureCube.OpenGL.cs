// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.OpenGL;
using GLPixelFormat = MonoGame.OpenGL.PixelFormat;

namespace MonoGame.Framework.Graphics
{
    public partial class TextureCube
    {
        private void PlatformConstruct(
            GraphicsDevice graphicsDevice, int size, bool mipMap, SurfaceFormat format, bool renderTarget)
        {
            _glTarget = TextureTarget.TextureCubeMap;

            _glTexture = GL.GenTexture();
            GL.CheckError();

            GL.BindTexture(TextureTarget.TextureCubeMap, _glTexture);
            GL.CheckError();

            GL.TexParameter(TextureTarget.TextureCubeMap,
                TextureParameterName.TextureMinFilter,
                mipMap ? (int)TextureMinFilter.LinearMipmapLinear : (int)TextureMinFilter.Linear);
            GL.CheckError();

            GL.TexParameter(TextureTarget.TextureCubeMap,
                TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.CheckError();

            GL.TexParameter(TextureTarget.TextureCubeMap,
                TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.CheckError();

            GL.TexParameter(TextureTarget.TextureCubeMap,
                TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.CheckError();

            format.GetGLFormat(GraphicsDevice, out _glInternalFormat, out _glFormat, out _glType);

            for (var i = 0; i < 6; i++)
            {
                var target = GetGLCubeFace((CubeMapFace)i);
                if (_glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    var imageSize = 0;
                    switch (format)
                    {
                        case SurfaceFormat.RgbPvrtc2Bpp:
                        case SurfaceFormat.RgbaPvrtc2Bpp:
                            imageSize = (Math.Max(size, 16) * Math.Max(size, 8) * 2 + 7) / 8;
                            break;

                        case SurfaceFormat.RgbPvrtc4Bpp:
                        case SurfaceFormat.RgbaPvrtc4Bpp:
                            imageSize = (Math.Max(size, 8) * Math.Max(size, 8) * 4 + 7) / 8;
                            break;

                        case SurfaceFormat.Dxt1:
                        case SurfaceFormat.Dxt1a:
                        case SurfaceFormat.Dxt1SRgb:
                        case SurfaceFormat.Dxt3:
                        case SurfaceFormat.Dxt3SRgb:
                        case SurfaceFormat.Dxt5:
                        case SurfaceFormat.Dxt5SRgb:
                        case SurfaceFormat.RgbEtc1:
                        case SurfaceFormat.Rgb8Etc2:
                        case SurfaceFormat.Srgb8Etc2:
                        case SurfaceFormat.Rgb8A1Etc2:
                        case SurfaceFormat.Srgb8A1Etc2:
                        case SurfaceFormat.Rgba8Etc2:
                        case SurfaceFormat.SRgb8A8Etc2:
                        case SurfaceFormat.RgbaAtcExplicitAlpha:
                        case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                            imageSize = (size + 3) / 4 * ((size + 3) / 4) * format.GetSize();
                            break;

                        default:
                            throw new NotSupportedException();
                    }

                    GL.CompressedTexImage2D(
                        target, 0, _glInternalFormat, size, size, 0, imageSize, IntPtr.Zero);
                }
                else
                {
                    GL.TexImage2D(
                        target, 0, _glInternalFormat, size, size, 0, _glFormat, _glType, IntPtr.Zero);
                }
                GL.CheckError();
            }

            if (mipMap)
            {
#if IOS || ANDROID
                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
#else
                GraphicsDevice.FramebufferHelper.Instance.GenerateMipmap((int)_glTarget);

                // This updates the mipmaps after a change in the base texture
                GL.TexParameter(
                    TextureTarget.TextureCubeMap, TextureParameterName.GenerateMipmap, (int)Bool.True);
                GL.CheckError();
#endif
            }
        }

        private unsafe void PlatformGetData<T>(
            CubeMapFace cubeMapFace, int level, Rectangle rect, Span<T> destination)
            where T : unmanaged
        {
#if OPENGL

            TextureTarget target = GetGLCubeFace(cubeMapFace);
            GL.BindTexture(TextureTarget.TextureCubeMap, _glTexture);

            int dstSize = destination.Length * sizeof(T);
            int pixelToT = Format.GetSize() / sizeof(T);
            var byteDst = MemoryMarshal.AsBytes(destination);

            if (_glFormat == GLPixelFormat.CompressedTextureFormats)
            {
                // Note: for compressed format Format.GetSize() returns the size of a 4x4 block
                var tFullWidth = Math.Max(Size >> level, 1) / 4 * pixelToT;
                var tmpSize = Math.Max(Size >> level, 1) / 4 * tFullWidth * sizeof(T);
                IntPtr tmp = Marshal.AllocHGlobal(tmpSize);
                GL.GetCompressedTexImage(target, level, tmp);
                GL.CheckError();

                var tmpSpan = new Span<byte>((void*)tmp, tmpSize);
                int rowCount = rect.Height / 4;
                int tRectWidth = rect.Width / 4 * pixelToT;
                for (int y = 0; y < rowCount; y++)
                {
                    int tmpStart = rect.X / 4 * pixelToT + (rect.Top / 4 + y) * tFullWidth;
                    var src = tmpSpan.Slice(tmpStart, tFullWidth);
                    var dst = byteDst[(y * tRectWidth)..];
                    src.CopyTo(dst);
                }
            }
            else
            {
                // we need to convert from our format size to the size of T here
                int tFullWidth = Math.Max(Size >> level, 1) * pixelToT;
                int tmpSize = Math.Max(Size >> level, 1) * tFullWidth * sizeof(T);
                IntPtr tmp = Marshal.AllocHGlobal(tmpSize);
                GL.GetTexImage(target, level, _glFormat, _glType, tmp);
                GL.CheckError();

                var tmpSpan = new Span<byte>((void*)tmp, tmpSize);
                int rowCount = rect.Height;
                int tRectWidth = rect.Width * pixelToT;
                for (int y = 0; y < rowCount; y++)
                {
                    int tmpStart = rect.X * pixelToT + (y + rect.Top) * tFullWidth;
                    var src = tmpSpan.Slice(tmpStart, tFullWidth);
                    var dst = byteDst[(y * tRectWidth)..];
                    src.CopyTo(dst);
                }
            }
#else
            throw new NotImplementedException();
#endif
        }

        private unsafe void PlatformSetData<T>(
            CubeMapFace face, int level, Rectangle rect, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, _glTexture);
            GL.CheckError();

            var target = GetGLCubeFace(face);
            fixed (T* dataPtr = data)
            {
                if (_glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    GL.CompressedTexSubImage2D(
                        target, level, rect.X, rect.Y, rect.Width, rect.Height,
                         _glInternalFormat, data.Length * sizeof(T), (IntPtr)dataPtr);
                }
                else
                {
                    GL.TexSubImage2D(
                        target, level, rect.X, rect.Y, rect.Width, rect.Height,
                        _glFormat, _glType, (IntPtr)dataPtr);
                }
                GL.CheckError();
            }
        }

        private TextureTarget GetGLCubeFace(CubeMapFace cubeMapFace)
        {
            switch (cubeMapFace)
            {
                case CubeMapFace.PositiveX: return TextureTarget.TextureCubeMapPositiveX;
                case CubeMapFace.NegativeX: return TextureTarget.TextureCubeMapNegativeX;
                case CubeMapFace.PositiveY: return TextureTarget.TextureCubeMapPositiveY;
                case CubeMapFace.NegativeY: return TextureTarget.TextureCubeMapNegativeY;
                case CubeMapFace.PositiveZ: return TextureTarget.TextureCubeMapPositiveZ;
                case CubeMapFace.NegativeZ: return TextureTarget.TextureCubeMapNegativeZ;
                default: throw new ArgumentOutOfRangeException(nameof(cubeMapFace));
            }
        }
    }
}

