// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Imaging;
using MonoGame.Utilities.Png;

#if IOS
using UIKit;
using CoreGraphics;
using Foundation;
using System.Drawing;
#endif

#if OPENGL
using MonoGame.OpenGL;
using GLPixelFormat = MonoGame.OpenGL.PixelFormat;
using PixelFormat = MonoGame.OpenGL.PixelFormat;
using MonoGame.Utilities;

#if ANDROID
using Android.Graphics;
#endif
#endif // OPENGL

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        private void PlatformConstruct(int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared)
        {
            this.glTarget = TextureTarget.Texture2D;
            format.GetGLFormat(GraphicsDevice, out glInternalFormat, out glFormat, out glType);
            Threading.BlockOnUIThread(() =>
            {
                GenerateGLTextureIfRequired();

                int w = width;
                int h = height;
                int level = 0;

                while (true)
                {
                    if (glFormat == GLPixelFormat.CompressedTextureFormats)
                    {
                        int imageSize = 0;
                        // PVRTC has explicit calculations for imageSize
                        // https://www.khronos.org/registry/OpenGL/extensions/IMG/IMG_texture_compression_pvrtc.txt
                        if (format == SurfaceFormat.RgbPvrtc2Bpp || format == SurfaceFormat.RgbaPvrtc2Bpp)
                        {
                            imageSize = (Math.Max(w, 16) * Math.Max(h, 8) * 2 + 7) / 8;
                        }
                        else if (format == SurfaceFormat.RgbPvrtc4Bpp || format == SurfaceFormat.RgbaPvrtc4Bpp)
                        {
                            imageSize = (Math.Max(w, 8) * Math.Max(h, 8) * 4 + 7) / 8;
                        }
                        else
                        {
                            int blockSize = format.GetSize();
                            format.GetBlockSize(out int blockWidth, out int blockHeight);
                            int wBlocks = (w + (blockWidth - 1)) / blockWidth;
                            int hBlocks = (h + (blockHeight - 1)) / blockHeight;
                            imageSize = wBlocks * hBlocks * blockSize;
                        }
                        GL.CompressedTexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, imageSize, IntPtr.Zero);
                        GraphicsExtensions.CheckGLError();
                    }
                    else
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, glFormat, glType, IntPtr.Zero);
                        GraphicsExtensions.CheckGLError();
                    }

                    if ((w == 1 && h == 1) || !mipmap)
                        break;
                    if (w > 1)
                        w = w / 2;
                    if (h > 1)
                        h = h / 2;
                    ++level;
                }
            });
        }

        private void PlatformSetData(int level, IntPtr data, int startIndex, int elementSize, int elementCount)
        {
            int startBytes = startIndex * elementSize;
            IntPtr dataPtr = data + startBytes;
            GetSizeForLevel(Width, Height, level, out int w, out int h);

            Threading.BlockOnUIThread(() =>
            {
                // Store the current bound texture.
                var prevTexture = GraphicsExtensions.GetBoundTexture2D();
                if (prevTexture != glTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, glTexture);
                    GraphicsExtensions.CheckGLError();
                }

                GenerateGLTextureIfRequired();
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(_format.GetSize(), 8));

                if (glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    int size = elementCount * elementSize;
                    GL.CompressedTexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, size, dataPtr);
                }
                else
                {
                    GL.TexImage2D(TextureTarget.Texture2D, level, glInternalFormat, w, h, 0, glFormat, glType, dataPtr);
                }
                GraphicsExtensions.CheckGLError();

#if !ANDROID
                // Required to make sure that any texture uploads on a thread are completed
                // before the main thread tries to use the texture.
                GL.Finish();
                GraphicsExtensions.CheckGLError();
#endif
                // Restore the bound texture.
                if (prevTexture != glTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                    GraphicsExtensions.CheckGLError();
                }
            });
        }

        private void PlatformSetData<T>(int level, T[] data, int startIndex, int elementCount) where T : struct
        {
            int elementSizeInBytes = ReflectionHelpers.SizeOf<T>.Get();

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                IntPtr pin = dataHandle.AddrOfPinnedObject();
                PlatformSetData(level, pin, startIndex, elementSizeInBytes, elementCount);
            }
            finally
            {
                dataHandle.Free();
            }
        }

        private void PlatformSetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            Threading.BlockOnUIThread(() =>
            {
                var elementSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
                var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                // Use try..finally to make sure dataHandle is freed in case of an error
                try
                {
                    var startBytes = startIndex * elementSizeInByte;
                    var dataPtr = new IntPtr(dataHandle.AddrOfPinnedObject().ToInt64() + startBytes);
                    // Store the current bound texture.
                    var prevTexture = GraphicsExtensions.GetBoundTexture2D();

                    if (prevTexture != glTexture)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, glTexture);
                        GraphicsExtensions.CheckGLError();
                    }

                    GenerateGLTextureIfRequired();
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(_format.GetSize(), 8));

                    if (glFormat == GLPixelFormat.CompressedTextureFormats)
                    {
                        GL.CompressedTexSubImage2D(TextureTarget.Texture2D, level, rect.X, rect.Y, rect.Width, rect.Height,
                            glInternalFormat, elementCount * elementSizeInByte, dataPtr);
                    }
                    else
                    {
                        GL.TexSubImage2D(TextureTarget.Texture2D, level, rect.X, rect.Y,
                            rect.Width, rect.Height, glFormat, glType, dataPtr);
                    }
                    GraphicsExtensions.CheckGLError();

#if !ANDROID
                    // Required to make sure that any texture uploads on a thread are completed
                    // before the main thread tries to use the texture.
                    GL.Finish();
                    GraphicsExtensions.CheckGLError();
#endif
                    // Restore the bound texture.
                    if (prevTexture != glTexture)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                        GraphicsExtensions.CheckGLError();
                    }
                }
                finally
                {
                    dataHandle.Free();
                }
            });
        }

        private void PlatformGetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            Threading.EnsureUIThread();

#if GLES
            // TODO: check for for non renderable formats (formats that can't be attached to FBO)

            var framebufferId = 0;
            GL.GenFramebuffers(1, out framebufferId);
            GraphicsExtensions.CheckGLError();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            GraphicsExtensions.CheckGLError();
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, this.glTexture, 0);
            GraphicsExtensions.CheckGLError();

            GL.ReadPixels(rect.X, rect.Y, rect.Width, rect.Height, this.glFormat, this.glType, data);
            GraphicsExtensions.CheckGLError();
            GraphicsDevice.DisposeFramebuffer(framebufferId);
#else
            var tSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
            GL.BindTexture(TextureTarget.Texture2D, this.glTexture);
            GL.PixelStore(PixelStoreParameter.PackAlignment, Math.Min(tSizeInByte, 8));

            unsafe
            {
                GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr dataPointer = dataHandle.AddrOfPinnedObject();
                int dstSize = data.Length * tSizeInByte;

                try
                {
                    if (glFormat == GLPixelFormat.CompressedTextureFormats)
                    {
                        // Note: for compressed format Format.GetSize() returns the size of a 4x4 block
                        int pixelToT = Format.GetSize() / tSizeInByte;
                        int tFullWidth = Math.Max(this.Width >> level, 1) / 4 * pixelToT;
                        IntPtr temp = Marshal.AllocHGlobal(Math.Max(this.Height >> level, 1) / 4 * tFullWidth * tSizeInByte);
                        GL.GetCompressedTexImage(TextureTarget.Texture2D, level, temp);
                        GraphicsExtensions.CheckGLError();

                        int rowCount = rect.Height / 4;
                        int tRectWidth = rect.Width / 4 * Format.GetSize() / tSizeInByte;

                        for (var r = 0; r < rowCount; r++)
                        {
                            int tempStart = rect.X / 4 * pixelToT + (rect.Top / 4 + r) * tFullWidth;
                            int dataStart = startIndex + r * tRectWidth;

                            CopyMemory(temp, tempStart, dataPointer, dataStart, dstSize, tRectWidth, tSizeInByte);
                        }
                    }
                    else
                    {
                        // we need to convert from our format size to the size of T here
                        int tFullWidth = Math.Max(this.Width >> level, 1) * Format.GetSize() / tSizeInByte;
                        IntPtr temp = Marshal.AllocHGlobal(Math.Max(this.Height >> level, 1) * tFullWidth * tSizeInByte);
                        GL.GetTexImage(TextureTarget.Texture2D, level, glFormat, glType, temp);
                        GraphicsExtensions.CheckGLError();

                        int pixelToT = Format.GetSize() / tSizeInByte;
                        int rowCount = rect.Height;
                        int tRectWidth = rect.Width * pixelToT;

                        for (var r = 0; r < rowCount; r++)
                        {
                            int tempStart = rect.X * pixelToT + (r + rect.Top) * tFullWidth;
                            int dataStart = startIndex + r * tRectWidth;

                            CopyMemory(temp, tempStart, dataPointer, dataStart, dstSize, tRectWidth, tSizeInByte);
                        }
                    }
                }
                finally
                {
                    dataHandle.Free();
                }
            }
#endif
        }

        private void PlatformGetData(int level, int arraySlice, Rectangle rect, IntPtr output, int startIndex, int elementSize, int elementCount)
        {
            Threading.EnsureUIThread();

#if GLES
            // TODO: check for for non renderable formats (formats that can't be attached to FBO)

            var framebufferId = 0;
            GL.GenFramebuffers(1, out framebufferId);
            GraphicsExtensions.CheckGLError();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            GraphicsExtensions.CheckGLError();
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, this.glTexture, 0);
            GraphicsExtensions.CheckGLError();

            GL.ReadPixels(rect.X, rect.Y, rect.Width, rect.Height, this.glFormat, this.glType, output);
            GraphicsExtensions.CheckGLError();
            GraphicsDevice.DisposeFramebuffer(framebufferId);
#else

            GL.BindTexture(TextureTarget.Texture2D, this.glTexture);
            GL.PixelStore(PixelStoreParameter.PackAlignment, Math.Min(elementSize, 8));

            unsafe
            {
                int outputSize = rect.Width * rect.Height * 4;
                if (glFormat == GLPixelFormat.CompressedTextureFormats)
                {
                    // Note: for compressed format Format.GetSize() returns the size of a 4x4 block
                    int pixelToT = Format.GetSize() / elementSize;
                    int tFullWidth = Math.Max(this.Width >> level, 1) / 4 * pixelToT;
                    IntPtr temp = Marshal.AllocHGlobal(Math.Max(this.Height >> level, 1) / 4 * tFullWidth * elementSize);
                    GL.GetCompressedTexImage(TextureTarget.Texture2D, level, temp);
                    GraphicsExtensions.CheckGLError();

                    int rowCount = rect.Height / 4;
                    int tRectWidth = rect.Width / 4 * Format.GetSize() / elementSize;

                    for (var r = 0; r < rowCount; r++)
                    {
                        int tempStart = rect.X / 4 * pixelToT + (rect.Top / 4 + r) * tFullWidth;
                        int dataStart = startIndex + r * tRectWidth;

                        IntPtr tempOffset = new IntPtr(temp.ToInt64() + tempStart * elementSize);
                        IntPtr dataOffset = new IntPtr(output.ToInt64() + dataStart * elementSize);
                        Buffer.MemoryCopy((void*)tempOffset, (void*)dataOffset, outputSize, tRectWidth * elementSize);
                    }
                }
                else
                {
                    // we need to convert from our format size to the size of T here
                    int tFullWidth = Math.Max(this.Width >> level, 1) * Format.GetSize() / elementSize;
                    IntPtr temp = Marshal.AllocHGlobal(Math.Max(this.Height >> level, 1) * tFullWidth * elementSize);
                    GL.GetTexImage(TextureTarget.Texture2D, level, glFormat, glType, temp);
                    GraphicsExtensions.CheckGLError();

                    int pixelToT = Format.GetSize() / elementSize;
                    int rowCount = rect.Height;
                    int tRectWidth = rect.Width * pixelToT;

                    for (var r = 0; r < rowCount; r++)
                    {
                        int tempStart = rect.X * pixelToT + (r + rect.Top) * tFullWidth;
                        int dataStart = startIndex + r * tRectWidth;

                        IntPtr tempOffset = new IntPtr(temp.ToInt64() + tempStart * elementSize);
                        IntPtr dataOffset = new IntPtr(output.ToInt64() + dataStart * elementSize);
                        Buffer.MemoryCopy((void*)tempOffset, (void*)dataOffset, outputSize, tRectWidth * elementSize);
                    }
                }
            }
#endif
        }

        private static Texture2D PlatformFromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            using (var img = new Image(stream, ImagePixelFormat.RgbWithAlpha, true))
            {
                IntPtr data = img.GetPointer();
                int channels = (int)img.PixelFormat;

                if (data == IntPtr.Zero || channels != 4)
                    throw new InvalidDataException($"Could not decode stream {img.Info}: \n" + img.Errors);

                int length = img.PointerLength;
                unsafe
                {
                    // XNA blacks out any pixels with an alpha of zero.

                    byte* src = (byte*)data;
                    for (int i = 0; i < length; i += 4)
                    {
                        if (src[i + 3] == 0)
                        {
                            src[i + 0] = 0;
                            src[i + 1] = 0;
                            src[i + 2] = 0;
                        }
                    }
                }

                Texture2D texture = new Texture2D(graphicsDevice, img.Width, img.Height);
                texture.SetData(data, 0, channels, length / channels);
                return texture;
            }
        }

#if IOS
        [CLSCompliant(false)]
        public static Texture2D FromStream(GraphicsDevice graphicsDevice, UIImage uiImage)
        {
            return PlatformFromStream(graphicsDevice, uiImage.CGImage);
        }
#elif ANDROID
        [CLSCompliant(false)]
        public static Texture2D FromStream(GraphicsDevice graphicsDevice, Bitmap bitmap)
        {
            return PlatformFromStream(graphicsDevice, bitmap);
        }

        [CLSCompliant(false)]
        public void Reload(Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;

            int[] pixels = new int[width * height];
            if ((width != image.Width) || (height != image.Height))
            {
                using (Bitmap imagePadded = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888))
                {
                    Canvas canvas = new Canvas(imagePadded);
                    canvas.DrawARGB(0, 0, 0, 0);
                    canvas.DrawBitmap(image, 0, 0, null);
                    imagePadded.GetPixels(pixels, 0, width, 0, 0, width, height);
                    imagePadded.Recycle();
                }
            }
            else
            {
                image.GetPixels(pixels, 0, width, 0, 0, width, height);
            }

            image.Recycle();

            this.SetData<int>(pixels);
        }
#endif

#if IOS
        private static Texture2D PlatformFromStream(GraphicsDevice graphicsDevice, CGImage cgImage)
        {
			var width = cgImage.Width;
			var height = cgImage.Height;

            var data = new byte[width * height * 4];

            var colorSpace = CGColorSpace.CreateDeviceRGB();
            var bitmapContext = new CGBitmapContext(data, width, height, 8, width * 4, colorSpace, CGBitmapFlags.PremultipliedLast);
            bitmapContext.DrawImage(new RectangleF(0, 0, width, height), cgImage);
            bitmapContext.Dispose();
            colorSpace.Dispose();

            Texture2D texture = null;
            Threading.BlockOnUIThread(() =>
            {
                texture = new Texture2D(graphicsDevice, (int)width, (int)height, false, SurfaceFormat.Color);
                texture.SetData(data);
            });

            return texture;
        }
#elif ANDROID
        private static Texture2D PlatformFromStream(GraphicsDevice graphicsDevice, Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;

            int[] pixels = new int[width * height];
            if ((width != image.Width) || (height != image.Height))
            {
                using (Bitmap imagePadded = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888))
                {
                    Canvas canvas = new Canvas(imagePadded);
                    canvas.DrawARGB(0, 0, 0, 0);
                    canvas.DrawBitmap(image, 0, 0, null);
                    imagePadded.GetPixels(pixels, 0, width, 0, 0, width, height);
                    imagePadded.Recycle();
                }
            }
            else
            {
                image.GetPixels(pixels, 0, width, 0, 0, width, height);
            }
            image.Recycle();

            // Convert from ARGB to ABGR
            ConvertToABGR(height, width, pixels);

            Texture2D texture = null;
            Threading.BlockOnUIThread(() =>
            {
                texture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);
                texture.SetData<int>(pixels);
            });

            return texture;
        }
#endif

        private void FillTextureFromStream(Stream stream)
        {
#if ANDROID
            using (Bitmap image = BitmapFactory.DecodeStream(stream, null, new BitmapFactory.Options
            {
                InScaled = false,
                InDither = false,
                InJustDecodeBounds = false,
                InPurgeable = true,
                InInputShareable = true,
            }))
            {
                var width = image.Width;
                var height = image.Height;

                int[] pixels = new int[width * height];
                image.GetPixels(pixels, 0, width, 0, 0, width, height);

                // Convert from ARGB to ABGR
                ConvertToABGR(height, width, pixels);

                this.SetData<int>(pixels);
                image.Recycle();
            }
#endif
        }

        private void PlatformSaveAsJpeg(Stream stream, int width, int height)
        {
#if DESKTOPGL
            SaveAsImage(stream, width, height, ImageSaveFormat.Png);
#elif ANDROID
            SaveAsImage(stream, width, height, Bitmap.CompressFormat.Jpeg);
#else
            throw new NotImplementedException();
#endif
        }

        private void PlatformSaveAsPng(Stream stream, int width, int height)
        {
#if DESKTOPGL
            SaveAsImage(stream, width, height, ImageSaveFormat.Png);
#elif ANDROID
            SaveAsImage(stream, width, height, Bitmap.CompressFormat.Png);
#else
            var pngWriter = new PngWriter();
            pngWriter.Write(this, stream);
#endif
        }

#if DESKTOPGL
        internal void SaveAsImage(Stream stream, int width, int height, ImageSaveFormat format)
        {
	        if (stream == null)
		        throw new ArgumentNullException(nameof(stream));

	        if (width <= 0)
		        throw new ArgumentOutOfRangeException(nameof(width), "Texture width must be greater than zero.");

	        if (height <= 0)
		        throw new ArgumentOutOfRangeException(nameof(height), "Texture height must be greater than zero.");
            
            int elementCount = width * height;
            IntPtr buffer = Marshal.AllocHGlobal(elementCount * 4);
            try
            {
                PlatformGetData(0, 0, new Rectangle(0, 0, width, height), buffer, 0, 4, elementCount);

                using (var img = new Image(buffer, width, height, (ImagePixelFormat)4))
                    img.Save(stream, format);
            }
            finally
            {
                if (buffer != null)
                    Marshal.FreeHGlobal(buffer);
            }
        }
#elif ANDROID
        private void SaveAsImage(Stream stream, int width, int height, Bitmap.CompressFormat format)
        {
            int[] data = new int[width * height];
            GetData(data);
            // internal structure is BGR while bitmap expects RGB
            for (int i = 0; i < data.Length; ++i)
            {
                uint pixel = (uint)data[i];
                data[i] = (int)((pixel & 0xFF00FF00) | ((pixel & 0x00FF0000) >> 16) | ((pixel & 0x000000FF) << 16));
            }
            using (Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888))
            {
                bitmap.SetPixels(data, 0, width, 0, 0, width, height);
                bitmap.Compress(format, 100, stream);
                bitmap.Recycle();
            }
        }
#endif

        // This method allows games that use Texture2D.FromStream
        // to reload their textures after the GL context is lost.
        private void PlatformReload(Stream textureStream)
        {
            var prev = GraphicsExtensions.GetBoundTexture2D();

            GenerateGLTextureIfRequired();
            FillTextureFromStream(textureStream);

            GL.BindTexture(TextureTarget.Texture2D, prev);
        }

        private void GenerateGLTextureIfRequired()
        {
            if (this.glTexture < 0)
            {
                GL.GenTextures(1, out this.glTexture);
                GraphicsExtensions.CheckGLError();

                // For best compatibility and to keep the default wrap mode of XNA, only set ClampToEdge if either
                // dimension is not a power of two.
                var wrap = TextureWrapMode.Repeat;

                if (((Width & (Width - 1)) != 0) || ((Height & (Height - 1)) != 0))
                    wrap = TextureWrapMode.ClampToEdge;

                GL.BindTexture(TextureTarget.Texture2D, this.glTexture);
                GraphicsExtensions.CheckGLError();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                (_levelCount > 1) ? (int)TextureMinFilter.LinearMipmapLinear : (int)TextureMinFilter.Linear);
                GraphicsExtensions.CheckGLError();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                                (int)TextureMagFilter.Linear);
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
                    if (_levelCount > 0)
                        GL.TexParameter(TextureTarget.Texture2D, SamplerState.TextureParameterNameTextureMaxLevel, _levelCount - 1);
                    else
                        GL.TexParameter(TextureTarget.Texture2D, SamplerState.TextureParameterNameTextureMaxLevel, 1000);

                    GraphicsExtensions.CheckGLError();
                }
            }
        }
    }
}
