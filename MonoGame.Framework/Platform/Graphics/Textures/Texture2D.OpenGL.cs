// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.OpenGL;
using GLPixelFormat = MonoGame.OpenGL.PixelFormat;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        private void PlatformConstruct(
            int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared)
        {
            void Construct()
            {
                _glTarget = TextureTarget.Texture2D;
                format.GetGLFormat(GraphicsDevice, out glInternalFormat, out glFormat, out glType);

                GenerateGLTextureIfRequired();
                int level = 0;

                while (true)
                {
                    if (glFormat == GLPixelFormat.CompressedTextureFormats)
                    {
                        int imageSize;
                        // PVRTC has explicit calculations for imageSize
                        // https://www.khronos.org/registry/OpenGL/extensions/IMG/IMG_texture_compression_pvrtc.txt
                        switch (format)
                        {
                            case SurfaceFormat.RgbPvrtc2Bpp:
                            case SurfaceFormat.RgbaPvrtc2Bpp:
                                imageSize = (Math.Max(width, 16) * Math.Max(height, 8) * 2 + 7) / 8;
                                break;

                            case SurfaceFormat.RgbPvrtc4Bpp:
                            case SurfaceFormat.RgbaPvrtc4Bpp:
                                imageSize = (Math.Max(width, 8) * Math.Max(height, 8) * 4 + 7) / 8;
                                break;

                            default:
                            {
                                format.GetBlockSize(out int blockWidth, out int blockHeight);
                                int wBlocks = (width + (blockWidth - 1)) / blockWidth;
                                int hBlocks = (height + (blockHeight - 1)) / blockHeight;
                                imageSize = wBlocks * hBlocks * format.GetSize();
                                break;
                            }
                        }

                        GL.CompressedTexImage2D(
                            TextureTarget.Texture2D, level, glInternalFormat, width, height, 0, imageSize, IntPtr.Zero);
                        GraphicsExtensions.CheckGLError();
                    }
                    else
                    {
                        GL.TexImage2D(
                            TextureTarget.Texture2D, level, glInternalFormat, width, height, 0, glFormat, glType, IntPtr.Zero);
                        GraphicsExtensions.CheckGLError();
                    }

                    if ((width == 1 && height == 1) || !mipmap)
                        break;

                    if (width > 1)
                        width /= 2;
                    if (height > 1)
                        height /= 2;
                    level++;
                }
            }

            if (Threading.IsOnUIThread())
                Construct();
            else
                Threading.BlockOnUIThread(Construct);
        }

        private unsafe void PlatformSetData<T>(
            int level, int arraySlice, Rectangle? rect, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            AssertIsOnUIThreadForSpan();

            // Store the current bound texture.
            var prevTexture = GraphicsExtensions.GetBoundTexture2D();
            if (prevTexture != _glTexture)
            {
                GL.BindTexture(TextureTarget.Texture2D, _glTexture);
                GraphicsExtensions.CheckGLError();
            }

            GenerateGLTextureIfRequired();
            int unpackSize = _format.GetSize();
            if (unpackSize == 3)
                unpackSize = 1;
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(unpackSize, 8));

            fixed (T* ptr = &MemoryMarshal.GetReference(data))
            {
                int bytes = data.Length * sizeof(T);
                if (rect.HasValue)
                {
                    Rectangle r = rect.Value;
                    if (glFormat == GLPixelFormat.CompressedTextureFormats)
                    {
                        GL.CompressedTexSubImage2D(
                            TextureTarget.Texture2D, level, r.X, r.Y, r.Width, r.Height, glInternalFormat, bytes, (IntPtr)ptr);
                    }
                    else
                    {
                        GL.TexSubImage2D(
                            TextureTarget.Texture2D, level, r.X, r.Y, r.Width, r.Height, glFormat, glType, (IntPtr)ptr);
                    }
                }
                else
                {
                    GetSizeForLevel(Width, Height, level, out int w, out int h);
                    if (glFormat == GLPixelFormat.CompressedTextureFormats)
                        GL.CompressedTexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, bytes, (IntPtr)ptr);
                    else
                        GL.TexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, glFormat, glType, (IntPtr)ptr);
                }
            }
            GraphicsExtensions.CheckGLError();

#if !ANDROID
            // Required to make sure that any texture uploads on a thread are completed
            // before the main thread tries to use the texture.
            GL.Finish();
            GraphicsExtensions.CheckGLError();
#endif
            // Restore the bound texture.
            if (prevTexture != _glTexture)
            {
                GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                GraphicsExtensions.CheckGLError();
            }
        }

        private unsafe void PlatformGetData<T>(
            int level, int arraySlice, Rectangle rect, Span<T> destination)
            where T : unmanaged
        {
            AssertIsOnUIThreadForSpan();
#if GLES
            // TODO: check for for non renderable formats (formats that can't be attached to FBO)
            GL.GenFramebuffers(1, out int framebufferId);
            GraphicsExtensions.CheckGLError();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            GraphicsExtensions.CheckGLError();

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, glTexture, 0);
            GraphicsExtensions.CheckGLError();

            GL.ReadPixels(rect.X, rect.Y, rect.Width, rect.Height, glFormat, glType, data);
            GraphicsExtensions.CheckGLError();
            GraphicsDevice.DisposeFramebuffer(framebufferId);
#else
            GL.BindTexture(TextureTarget.Texture2D, _glTexture);
            GL.PixelStore(PixelStoreParameter.PackAlignment, Math.Min(sizeof(T), 8));

            int dstSize = destination.Length * sizeof(T);
            var dstBytes = MemoryMarshal.AsBytes(destination);
            var buffer = IntPtr.Zero;
            try
            {
                if (glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    // Note: for compressed format Format.GetSize() returns the size of a 4x4 block
                    int pixelToT = Format.GetSize() / sizeof(T);
                    int tFullWidth = Math.Max(Width >> level, 1) / 4 * pixelToT;
                    int bufferBytes = Math.Max(Height >> level, 1) / 4 * tFullWidth * sizeof(T);
                    buffer = Marshal.AllocHGlobal(bufferBytes);
                    var bufferSpan = new ReadOnlySpan<byte>((void*)buffer, bufferBytes);

                    GL.GetCompressedTexImage(TextureTarget.Texture2D, level, buffer);
                    GraphicsExtensions.CheckGLError();

                    int rows = rect.Height / 4;
                    int tRectWidth = rect.Width / 4 * Format.GetSize() / sizeof(T);

                    for (int row = 0; row < rows; row++)
                    {
                        int bufferStart = rect.X / 4 * pixelToT + (rect.Top / 4 + row) * tFullWidth;
                        int dataStart = row * tRectWidth;

                        var src = bufferSpan.Slice(bufferStart * sizeof(T), tRectWidth * sizeof(T));
                        var dst = dstBytes.Slice(dataStart * sizeof(T), src.Length);
                        src.CopyTo(dst);
                    }
                }
                else
                {
                    // we need to convert from our format size to the size of T here
                    int tFullWidth = Math.Max(Width >> level, 1) * Format.GetSize() / sizeof(T);
                    int bufferBytes = Math.Max(Height >> level, 1) * tFullWidth * sizeof(T);
                    buffer = Marshal.AllocHGlobal(bufferBytes);
                    var bufferSpan = new ReadOnlySpan<byte>((void*)buffer, bufferBytes);

                    GL.GetTexImage(TextureTarget.Texture2D, level, glFormat, glType, buffer);
                    GraphicsExtensions.CheckGLError();

                    int pixelToT = Format.GetSize() / sizeof(T);
                    int tRectWidth = rect.Width * pixelToT;

                    for (int row = 0; row < rect.Height; row++)
                    {
                        int bufferStart = rect.X * pixelToT + (row + rect.Top) * tFullWidth;
                        int dataStart = row * tRectWidth;

                        var src = bufferSpan.Slice(bufferStart * sizeof(T), tRectWidth * sizeof(T));
                        var dst = dstBytes.Slice(dataStart * sizeof(T), src.Length);
                        src.CopyTo(dst);
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
#endif
        }

        private void GenerateGLTextureIfRequired()
        {
            if (_glTexture < 0)
            {
                GL.GenTextures(1, out _glTexture);
                GraphicsExtensions.CheckGLError();

                // For best compatibility and to keep the default wrap mode of XNA, only set ClampToEdge if
                // either dimension is not a power of two.
                var wrap = TextureWrapMode.Repeat;

                if (((Width & (Width - 1)) != 0) || ((Height & (Height - 1)) != 0))
                    wrap = TextureWrapMode.ClampToEdge;

                GL.BindTexture(TextureTarget.Texture2D, _glTexture);
                GraphicsExtensions.CheckGLError();

                var minFilter = (_levelCount > 1) ? (int)TextureMinFilter.LinearMipmapLinear : (int)TextureMinFilter.Linear;
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, minFilter);
                GraphicsExtensions.CheckGLError();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GraphicsExtensions.CheckGLError();

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrap);
                GraphicsExtensions.CheckGLError();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrap);
                GraphicsExtensions.CheckGLError();

                // Set mipmap levels
#if !GLES
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
#endif
                GraphicsExtensions.CheckGLError();
                if (GraphicsDevice.GraphicsCapabilities.SupportsTextureMaxLevel)
                {
                    var paramName = SamplerState.TextureParameterNameTextureMaxLevel;
                    if (_levelCount > 0)
                        GL.TexParameter(TextureTarget.Texture2D, paramName, _levelCount - 1);
                    else
                        GL.TexParameter(TextureTarget.Texture2D, paramName, 1000);
                    GraphicsExtensions.CheckGLError();
                }
            }
        }
    }
}
