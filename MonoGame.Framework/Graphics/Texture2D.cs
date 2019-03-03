// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Imaging;
using MonoGame.Utilities;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        public enum SurfaceType
        {
            Texture,
            RenderTarget,
            SwapChainRenderTarget,
        }

        internal Rectangle _bounds;
        internal int ArraySize;
        internal Vector2 _texel;

        public Vector2 Texel => _texel;
        public float TexelWidth => _texel.X;
        public float TexelHeight => _texel.Y;

        /// <summary>
        /// Gets the dimensions of the texture
        /// </summary>
        public Rectangle Bounds => _bounds;

        /// <summary>
        /// Creates a new texture of the given size
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Texture2D(GraphicsDevice graphicsDevice, int width, int height)
            : this(graphicsDevice, width, height, false, SurfaceFormat.Rgba32, SurfaceType.Texture, false, 1)
        {
        }

        /// <summary>
        /// Creates a new texture of a given size with a surface format and optional mipmaps 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mipmap"></param>
        /// <param name="format"></param>
        public Texture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format)
            : this(graphicsDevice, width, height, mipmap, format, SurfaceType.Texture, false, 1)
        {
        }

        /// <summary>
        /// Creates a new texture array of a given size with a surface format and optional mipmaps.
        /// Throws ArgumentException if the current GraphicsDevice can't work with texture arrays
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mipmap"></param>
        /// <param name="format"></param>
        /// <param name="arraySize"></param>
        public Texture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, int arraySize)
            : this(graphicsDevice, width, height, mipmap, format, SurfaceType.Texture, false, arraySize)
        {

        }

        /// <summary>
        ///  Creates a new texture of a given size with a surface format and optional mipmaps.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mipmap"></param>
        /// <param name="format"></param>
        /// <param name="type"></param>
        internal Texture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type)
            : this(graphicsDevice, width, height, mipmap, format, type, false, 1)
        {
        }

        protected Texture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared, int arraySize)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice), FrameworkResources.ResourceCreationWhenDeviceIsNull);

            if (arraySize > 1 && !graphicsDevice.GraphicsCapabilities.SupportsTextureArrays)
                throw new ArgumentException("Texture arrays are not supported on this graphics device", nameof(arraySize));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Texture width must be greater than zero");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Texture height must be greater than zero");

            this.GraphicsDevice = graphicsDevice;
            this._bounds = new Rectangle(0, 0, width, height);
            this._texel = new Vector2(1f / width, 1f / height);

            this._format = format;
            this._levelCount = mipmap ? CalculateMipLevels(width, height) : 1;
            this.ArraySize = arraySize;

            // Texture will be assigned by the swap chain.
            if (type == SurfaceType.SwapChainRenderTarget)
                return;

            PlatformConstruct(width, height, mipmap, format, type, shared);
        }

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// </summary>
        public int Width => _bounds.Width;

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// </summary>
        public int Height => _bounds.Height;

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level">Layer of the texture to modify</param>
        /// <param name="arraySlice">Index inside the texture array</param>
        /// <param name="rect">Area to modify</param>
        /// <param name="data">New data for the texture</param>
        /// <param name="startIndex">Start position of data</param>
        /// <param name="elementCount"></param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="arraySlice"/> is greater than 0, 
        ///  <paramref name="elementCount"/> is more than <paramref name="data"/>.Length,
        ///  and the GraphicsDevice does not support texture arrays.
        /// </exception>
        public void SetData<T>(int level, int arraySlice, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            ValidateParams(level, arraySlice, rect, data, startIndex, elementCount, out Rectangle checkedRect);
            PlatformSetData(level, arraySlice, checkedRect, data, startIndex, elementCount);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level">Layer of the texture to modify</param>
        /// <param name="rect">Area to modify</param>
        /// <param name="data">New data for the texture</param>
        /// <param name="startIndex">Start position of data</param>
        /// <param name="elementCount"></param>
        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            ValidateParams(level, 0, rect, data, startIndex, elementCount, out Rectangle checkedRect);
            if (rect.HasValue)
                PlatformSetData(level, 0, checkedRect, data, startIndex, elementCount);
            else
                PlatformSetData(level, data, startIndex, elementCount);
        }

        /// <summary>
        /// Changes the texture's pixels.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">New data for the texture</param>
        /// <param name="startIndex">Start position of data</param>
        /// <param name="elementCount"></param>
		public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            ValidateParams(0, 0, null, data, startIndex, elementCount, out Rectangle checkedRect);
            PlatformSetData(0, data, startIndex, elementCount);
        }

        /// <summary>
        /// Changes the texture's pixels.
        /// </summary>
        /// <typeparam name="T">New data for the texture</typeparam>
        /// <param name="data"></param>
        public void SetData<T>(T[] data) where T : struct
        {
            ValidateParams(0, 0, null, data, 0, data.Length, out Rectangle checkedRect);
            PlatformSetData(0, data, 0, data.Length);
        }

        /// <summary>
        /// Changes the texture's pixels.
        /// </summary>
        /// <param name="data">Image data</param>
        /// <param name="elementStartIndex">start index (not in bytes)</param>
        /// <param name="elementSize">per-element size in bytes</param>
        /// <param name="elementCount">amount of elements to write</param>
        public void SetData(IntPtr data, int elementStartIndex, int elementSize, int elementCount)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (elementSize <= 0) throw new ArgumentOutOfRangeException(nameof(elementSize));
            if (elementCount <= 0) throw new ArgumentOutOfRangeException(nameof(elementCount));

            PlatformSetData(0, data, elementStartIndex, elementSize, elementCount);
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level">Layer of the texture</param>
        /// <param name="arraySlice">Index inside the texture array</param>
        /// <param name="rect">Area of the texture to retrieve</param>
        /// <param name="data">Destination array for the data</param>
        /// <param name="startIndex">Starting index of data where to write the pixel data</param>
        /// <param name="elementCount">Number of pixels to read</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="arraySlice"/> is greater than 0, 
        ///  <paramref name="elementCount"/> is more than <paramref name="data"/>.Length,
        ///  and the GraphicsDevice does not support texture arrays.
        /// </exception>
        public void GetData<T>(int level, int arraySlice, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            ValidateParams(level, arraySlice, rect, data, startIndex, elementCount, out Rectangle checkedRect);
            PlatformGetData(level, arraySlice, checkedRect, data, startIndex, elementCount);
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <param name="level">Layer of the texture</param>
        /// <param name="arraySlice">Index inside the texture array</param>
        /// <param name="rect">Area of the texture to retrieve</param>
        /// <param name="buffer">Destination pointer for the data</param>
        /// <param name="startIndex">Starting index of data where to write the pixel data</param>
        /// <param name="elementSize">per-element size in bytes</param>
        /// <param name="elementCount">Number of pixels to read</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="buffer"/> is zero.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="arraySlice"/> is greater than 0,
        ///  and the GraphicsDevice does not support texture arrays.
        /// </exception>
        public void GetData(int level, int arraySlice, Rectangle rect, IntPtr buffer, int startIndex, int elementSize, int elementCount)
        {
            if (buffer == IntPtr.Zero) throw new ArgumentNullException(nameof(buffer)); 
            if (elementSize <= 0) throw new ArgumentOutOfRangeException(nameof(elementSize));
            if (elementCount <= 0) throw new ArgumentOutOfRangeException(nameof(elementCount));

            PlatformGetData(level, arraySlice, rect, buffer, startIndex, elementSize, elementCount);
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level">Layer of the texture</param>
        /// <param name="rect">Area of the texture</param>
        /// <param name="data">Destination array for the texture data</param>
        /// <param name="startIndex">First position in data where to write the pixel data</param>
        /// <param name="elementCount">Number of pixels to read</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="data"/> is zero.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="elementCount"/> is more than <paramref name="data"/>.Length,
        ///  and the GraphicsDevice does not support texture arrays.
        /// </exception>
        public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            this.GetData(level, 0, rect, data, startIndex, elementCount);
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Destination array for the texture data</param>
        /// <param name="startIndex">First position in data where to write the pixel data</param>
        /// <param name="elementCount">Number of pixels to read</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="elementCount"/> is more than <paramref name="data"/>.Length,
        ///  and the GraphicsDevice does not support texture arrays.
        /// </exception>
		public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            this.GetData(0, null, data, startIndex, elementCount);
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Destination array for the texture data</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if the GraphicsDevice does not support texture arrays.</exception>
        public void GetData<T>(T[] data) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            this.GetData(0, null, data, 0, data.Length);
        }

        /// <summary>
        /// Creates a Texture2D from a stream, supported formats bmp, gif, jpg, png, tga (only for simple textures).
        /// </summary>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <returns>The <see cref="SurfaceFormat.Rgba32"/> texture created from the image stream.</returns>
        /// <remarks>
        ///  Note that different image decoders may generate slight differences between platforms, but perceptually 
        ///  the images should be identical. This call does not premultiply the image alpha, but areas of zero alpha will
        ///  result in black color data.
        /// </remarks>
        public static Texture2D FromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            try
            {
                using (var img = new Image(stream, ImagePixelFormat.RgbWithAlpha, leaveOpen: true))
                {
                    IntPtr data = img.GetPointer();
                    int channels = (int)img.PixelFormat;

                    if (data == IntPtr.Zero || channels != 4)
                    {
                        string msg = "Could not decode stream.";
                        if (img.Errors.Count > 0)
                            msg += "\n " + img.Errors;

                        var exc = new InvalidDataException(msg);
                        exc.Data.Add("ImageInfo", img.Info);
                        throw exc;
                    }

                    int pixels = img.Width * img.Height;
                    int length = channels * pixels;
                    unsafe
                    {
                        byte* src = (byte*)data;
                        for (int i = 0; i < length; i += 4)
                        {
                            // XNA blacks out any pixels with an alpha of zero.
                            if (src[i + 3] == 0)
                            {
                                src[i + 0] = 0;
                                src[i + 1] = 0;
                                src[i + 2] = 0;
                            }
                        }
                    }

                    var texture = new Texture2D(graphicsDevice, img.Width, img.Height);
                    texture.SetData(data, 0, channels, pixels);
                    return texture;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("This image format is not supported.", e);
            }
        }

        /// <summary>
        /// Converts the texture to a JPG image
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SaveAsJpeg(Stream stream, int width, int height)
        {
            PlatformSaveAsJpeg(stream, width, height);
        }

        /// <summary>
        /// Converts the texture to a PNG image
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SaveAsPng(Stream stream, int width, int height)
        {
            PlatformSaveAsPng(stream, width, height);
        }

        // This method allows games that use Texture2D.FromStream 
        // to reload their textures after the GL context is lost.
        public void Reload(Stream textureStream)
        {
            PlatformReload(textureStream);
        }

        //Converts Pixel Data from ARGB to ABGR
        private static void ConvertToABGR(int pixelHeight, int pixelWidth, int[] pixels)
        {
            int pixelCount = pixelWidth * pixelHeight;
            for (int i = 0; i < pixelCount; ++i)
            {
                uint pixel = (uint)pixels[i];
                pixels[i] = (int)((pixel & 0xFF00FF00) | ((pixel & 0x00FF0000) >> 16) | ((pixel & 0x000000FF) << 16));
            }
        }

        private void ValidateParams<T>(int level, int arraySlice, Rectangle? rect, T[] data,
            int startIndex, int elementCount, out Rectangle checkedRect) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (level < 0 || level >= LevelCount)
                throw new ArgumentException(
                    $"{nameof(level)} must be smaller than the number of levels in this texture.", nameof(level));

            if (arraySlice > 0 && !GraphicsDevice.GraphicsCapabilities.SupportsTextureArrays)
                throw new ArgumentException("Texture arrays are not supported on this graphics device", nameof(arraySlice));

            if (arraySlice < 0 || arraySlice >= ArraySize)
                throw new ArgumentException(
                    $"{nameof(arraySlice)} must be smaller than the {nameof(ArraySize)} of this texture and larger than 0.", nameof(arraySlice));

            var textureBounds = new Rectangle(0, 0, Math.Max(_bounds.Width >> level, 1), Math.Max(_bounds.Height >> level, 1));
            checkedRect = rect ?? textureBounds;
            if (!textureBounds.Contains(checkedRect) || checkedRect.Width <= 0 || checkedRect.Height <= 0)
                throw new ArgumentException("Rectangle must be inside the texture bounds.", nameof(rect));

            var tSize = ReflectionHelpers.SizeOf<T>.Get();
            var fSize = Format.GetSize();
            if (tSize > fSize || fSize % tSize != 0)
                throw new ArgumentException(
                    $"Type {nameof(T)} is of an invalid size for the format of this texture.", nameof(T));

            if (startIndex < 0 || startIndex >= data.Length)
                throw new ArgumentException(
                    $"{nameof(startIndex)} must be at least zero and smaller than {nameof(data)}.{nameof(data.Length)}.",
                    nameof(startIndex));

            if (data.Length < startIndex + elementCount)
                throw new ArgumentException("The data array is too small.", nameof(data));

            int dataByteSize;
            if (Format.IsCompressedFormat())
            {
                Format.GetBlockSize(out int blockWidth, out int blockHeight);
                int blockWidthMinusOne = blockWidth - 1;
                int blockHeightMinusOne = blockHeight - 1;
                // round x and y down to next multiple of block size; width and height up to next multiple of block size
                var roundedWidth = (checkedRect.Width + blockWidthMinusOne) & ~blockWidthMinusOne;
                var roundedHeight = (checkedRect.Height + blockHeightMinusOne) & ~blockHeightMinusOne;
                checkedRect = new Rectangle(checkedRect.X & ~blockWidthMinusOne, checkedRect.Y & ~blockHeightMinusOne,
#if OPENGL
                    // OpenGL only: The last two mip levels require the width and height to be
                    // passed as 2x2 and 1x1, but there needs to be enough data passed to occupy
                    // a full block.
                    checkedRect.Width < blockWidth && textureBounds.Width < blockWidth ? textureBounds.Width : roundedWidth,
                    checkedRect.Height < blockHeight && textureBounds.Height < blockHeight ? textureBounds.Height : roundedHeight);
#else
                    roundedWidth, roundedHeight);
#endif
                if (Format == SurfaceFormat.RgbPvrtc2Bpp || Format == SurfaceFormat.RgbaPvrtc2Bpp)
                {
                    dataByteSize = (Math.Max(checkedRect.Width, 16) * Math.Max(checkedRect.Height, 8) * 2 + 7) / 8;
                }
                else if (Format == SurfaceFormat.RgbPvrtc4Bpp || Format == SurfaceFormat.RgbaPvrtc4Bpp)
                {
                    dataByteSize = (Math.Max(checkedRect.Width, 8) * Math.Max(checkedRect.Height, 8) * 4 + 7) / 8;
                }
                else
                {
                    dataByteSize = roundedWidth * roundedHeight * fSize / (blockWidth * blockHeight);
                }
            }
            else
            {
                dataByteSize = checkedRect.Width * checkedRect.Height * fSize;
            }

            if (elementCount * tSize != dataByteSize)
                throw new ArgumentException($"{nameof(elementCount)} is not the right size, " +
                                            $"{nameof(elementCount)} * sizeof({nameof(T)}) is {elementCount * tSize}, " +
                                            $"but data size is {dataByteSize}.", nameof(elementCount));
        }
    }
}
