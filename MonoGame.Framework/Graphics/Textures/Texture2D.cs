// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Memory;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        internal int ArraySize;

        #region Properties

        /// <summary>
        /// Gets the reciprocal (1/x) of the width and height as a <see cref="Vector2"/>.
        /// </summary>
        public Vector2 Texel { get; }

        /// <summary>
        /// Gets the dimensions of the texture.
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// </summary>
        public int Width => Bounds.Width;

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// </summary>
        public int Height => Bounds.Height;

        #endregion

        #region Constructors

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

            GraphicsDevice = graphicsDevice;
            Bounds = new Rectangle(0, 0, width, height);
            Texel = new Vector2(1f / width, 1f / height);

            _format = format;
            _levelCount = mipmap ? CalculateMipLevels(width, height) : 1;
            ArraySize = arraySize;

            // Texture will be assigned by the swap chain.
            if (type == SurfaceType.SwapChainRenderTarget)
                return;

            PlatformConstruct(width, height, mipmap, format, type, shared);
        }

        #endregion

        #region SetData

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="data"/> is empty.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="arraySlice"/> is greater than 0 and
        ///  the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(ReadOnlySpan<T> data, int level, int arraySlice, Rectangle? rectangle = null)
            where T : unmanaged
        {
            ValidateParams(level, arraySlice, rectangle, data, out Rectangle checkedRect);
            if (rectangle.HasValue)
                PlatformSetData(level, arraySlice, checkedRect, data);
            else
                PlatformSetData(level, arraySlice, rect: null, data);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        public void SetData<T>(ReadOnlySpan<T> data, int level, Rectangle? rectangle = null)
            where T : unmanaged
        {
            SetData(data, level, 0, rectangle);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        public void SetData<T>(ReadOnlySpan<T> data, Rectangle? rectangle = null)
            where T : unmanaged
        {
            SetData(data, 0, rectangle);
        }

        #region Span<T> Overloads

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="data"/> is empty.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="arraySlice"/> is greater than 0 and
        ///  the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(Span<T> data, int level, int arraySlice, Rectangle? rectangle = null)
            where T : unmanaged
        {
            SetData((ReadOnlySpan<T>)data, level, arraySlice, rectangle);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        public void SetData<T>(Span<T> data, int level, Rectangle? rectangle = null)
        where T : unmanaged
        {
            SetData((ReadOnlySpan<T>)data, level, 0, rectangle);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        public void SetData<T>(Span<T> data, Rectangle? rectangle = null)
            where T : unmanaged
        {
            SetData((ReadOnlySpan<T>)data, 0, rectangle);
        }

        #endregion

        #endregion

        #region GetData

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <param name="destination">Destination span for the data.</param>
        /// <param name="level">Layer of the texture.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="rectangle">Area of the texture; defaults to texture bounds.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="destination"/> is empty.</exception>
        /// <exception cref="ArgumentException">
        ///  Throws if <paramref name="arraySlice"/> is greater than 0
        ///  and the graphics device does not support texture arrays.
        /// </exception>
        public void GetData<T>(
            Span<T> destination, int level, int arraySlice, Rectangle? rectangle = null)
            where T : unmanaged
        {
            if (destination.IsEmpty)
                throw new ArgumentNullException(nameof(destination));

            ValidateParams<T>(level, arraySlice, rectangle, destination, out Rectangle checkedRect);
            PlatformGetData(level, arraySlice, checkedRect, destination);
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <param name="destination">Destination span for the data.</param>
        /// <param name="level">Layer of the texture.</param>
        /// <param name="rectangle">Area of the texture; defaults to texture bounds.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="destination"/> is empty.</exception>
        public void GetData<T>(
            Span<T> destination, int level, Rectangle? rectangle = null)
            where T : unmanaged
        {
            GetData(destination, level, 0, rectangle);
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <param name="destination">Destination span for the data.</param>
        /// <param name="rectangle">Area of the texture; defaults to texture bounds.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="destination"/> is empty.</exception>
        public void GetData<T>(Span<T> destination, Rectangle? rectangle = null)
            where T : unmanaged
        {
            GetData(destination, 0, rectangle);
        }

        #endregion

        #region Loading

        #region FromStream

        /// <summary>
        /// Creates a 2D texture from a stream. Supported formats by default are Bmp, Gif, Jpeg and Png.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <param name="mipmap"><see langword="true"/> to generate mipmaps.</param>
        /// <param name="format">The desired surface format to use for the texture.</param>
        /// <param name="configuration">The configuration used to load the image.</param>
        /// <returns>The texture created from the image stream.</returns>
        /// <remarks>
        ///  Note that different image decoders may generate slight differences between platforms, but perceptually 
        ///  the images should be identical. This call does not premultiply the image alpha, but areas of zero alpha will
        ///  result in black color data.
        /// </remarks>
        [CLSCompliant(false)]
        public static Texture2D FromStream(
            GraphicsDevice graphicsDevice, Stream stream, bool mipmap, SurfaceFormat format, Configuration configuration = null)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            using (var image = LoadImage(stream, configuration))
                return FromImage(graphicsDevice, image, mipmap, format);
        }

        /// <summary>
        /// Creates a 2D texture from a stream. Supported formats by default are Bmp, Gif, Jpeg and Png.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <param name="mipmap"><see langword="true"/> to generate mipmaps.</param>
        /// <param name="format">The desired surface format to use for the texture.</param>
        /// <returns>The texture created from the image stream.</returns>
        public static Texture2D FromStream(
            GraphicsDevice graphicsDevice, Stream stream, bool mipmap, SurfaceFormat format)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            using (var image = LoadImage(stream))
                return FromImage(graphicsDevice, image, mipmap, format);
        }

        /// <summary>
        /// Creates a 2D texture from a stream. Supported formats by default are Bmp, Gif, Jpeg and Png.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <param name="configuration">The configuration used to load the image.</param>
        /// <returns>The <see cref="SurfaceFormat.Rgba32"/> texture created from the image stream.</returns>
        [CLSCompliant(false)]
        public static Texture2D FromStream(
            GraphicsDevice graphicsDevice, Stream stream, Configuration configuration = null)
        {
            return FromStream(graphicsDevice, stream, false, SurfaceFormat.Rgba32, configuration);
        }

        /// <summary>
        /// Creates a 2D texture from a stream. Supported formats by default are Bmp, Gif, Jpeg and Png.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <returns>The <see cref="SurfaceFormat.Rgba32"/> texture created from the image stream.</returns>
        public static Texture2D FromStream(
            GraphicsDevice graphicsDevice, Stream stream)
        {
            return FromStream(graphicsDevice, stream, null);
        }

        #endregion

        #region FromImage & LoadImage

        [CLSCompliant(false)]
        public static Texture2D FromImage<TPixel>(
            GraphicsDevice graphicsDevice, Image<TPixel> image, bool mipmap, SurfaceFormat format)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var texture = new Texture2D(graphicsDevice, image.Width, image.Height, mipmap, format);
            texture.SetData(image.GetPixelSpan());
            return texture;
        }

        [CLSCompliant(false)]
        public static Texture2D FromImage<TPixel>(GraphicsDevice graphicsDevice, Image<TPixel> image)
           where TPixel : unmanaged, IPixel<TPixel>
        {
            return FromImage(graphicsDevice, image, false, SurfaceFormat.Rgba32);
        }

        [CLSCompliant(false)]
        public static Image<Rgba32> LoadImage(Stream stream, Configuration configuration = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (configuration == null)
                configuration = Configuration.Default;

            var image = Image.Load<Rgba32>(configuration, stream);
            var pixelSpan = image.GetPixelSpan();

            int pixels = image.Width * image.Height;
            for (int i = 0; i < pixels; i++)
            {
                // XNA blacks out any pixel with an alpha of zero
                if (pixelSpan[i].A == 0)
                    pixelSpan[i] = default;
            }
            return image;
        }

        #endregion

        #region Reload

        /// <summary>
        /// This allows reloading textures when the graphics context is lost.
        /// This method cannot change texture dimensions.
        /// </summary>
        /// <param name="stream">The image data in the same dimensions as this texture.</param>
        /// <param name="configuration">The configuration used to load the image.</param>
        [CLSCompliant(false)]
        public void Reload(Stream stream, Configuration configuration = null)
        {
            using (var image = LoadImage(stream, configuration))
            {
                if (image.Width != Width)
                    throw new InvalidOperationException(
                        "The decoded image has a different width. Texture dimensions cannot be changed.");

                if (image.Height != Height)
                    throw new InvalidOperationException(
                        "The decoded image has a different height. Texture dimensions cannot be changed.");

                SetData(image.GetPixelSpan());
            }
        }

        /// <summary>
        /// This allows reloading textures when the graphics context is lost.
        /// This method cannot change texture dimensions.
        /// </summary>
        /// <param name="stream">The image data in the same dimensions as this texture.</param>
        public void Reload(Stream stream)
        {
            Reload(stream, null);
        }

        #endregion

        #endregion

        #region Save

        private unsafe Memory<Color> GetDataAsMemory(int width, int height, Configuration configuration)
        {
            var memory = configuration.MemoryAllocator.Allocate<Rgba32>(
                width * height, AllocationOptions.None);

            GetData(memory.Memory.Span, new Rectangle(0, 0, width, height));
            return memory;
        }

        /// <summary>
        /// Saves the texture with the specified format.
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format">The format used to encode the image.</param>
        /// <param name="configuration">The configuration used to save the image.</param>
        [CLSCompliant(false)]
        public void Save(int width, int height, Stream stream, IImageFormat format, Configuration configuration = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Texture width must be greater than zero.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Texture height must be greater than zero.");

            if (configuration == null)
                configuration = Configuration.Default;

            using (var mem = GetDataAsMemory(width, height, configuration))
            using (var tmp = Image.WrapMemory(configuration, mem, width, height))
                tmp.Save(stream, format);
        }

        /// <summary>
        /// Saves the texture with the specified format.
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="encoder">The encoder used to encode the image.</param>
        /// <param name="configuration">The configuration used to save the image.</param>
        [CLSCompliant(false)]
        public void Save(int width, int height, Stream stream, IImageEncoder encoder, Configuration configuration = null)
        {
            if (configuration == null)
                configuration = Configuration.Default;

            using (var mem = GetDataAsMemory(width, height, configuration))
            using (var tmp = Image.WrapMemory(configuration, Memory<Rgba32>.Empty, width, height))
                tmp.Save(stream, encoder);
        }

        /// <summary>
        /// Saves the texture as a JPEG image.
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="configuration">The configuration used to save the image.</param>
        [CLSCompliant(false)]
        public void SaveAsJpeg(int width, int height, Stream stream, Configuration configuration)
        {
            Save(width, height, stream, JpegFormat.Instance, configuration);
        }

        /// <summary>
        /// Saves the texture as a JPEG image.
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SaveAsJpeg(int width, int height, Stream stream)
        {
            SaveAsJpeg(width, height, stream, null);
        }

        /// <summary>
        /// Saves the texture as a PNG image.
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="configuration">The configuration used to save the image.</param>
        [CLSCompliant(false)]
        public void SaveAsPng(int width, int height, Stream stream, Configuration configuration)
        {
            Save(width, height, stream, PngFormat.Instance, configuration);
        }

        /// <summary>
        /// Saves the texture as a PNG image.
        /// </summary>
        /// <param name="stream">Destination for the image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SaveAsPng(int width, int height, Stream stream)
        {
            Save(width, height, stream, PngFormat.Instance, null);
        }
        #endregion

        #region ValidateParams

        private unsafe void ValidateParams<T>(
            int level, int arraySlice, Rectangle? rect, ReadOnlySpan<T> data, out Rectangle checkedRect)
            where T : unmanaged
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
                    $"{nameof(arraySlice)} must be smaller than the {nameof(ArraySize)} of this texture and larger than 0.",
                    nameof(arraySlice));

            var texBounds = new Rectangle(0, 0, Math.Max(Bounds.Width >> level, 1), Math.Max(Bounds.Height >> level, 1));
            checkedRect = rect ?? texBounds;
            if (rect.HasValue && !texBounds.Contains(checkedRect) || checkedRect.Width <= 0 || checkedRect.Height <= 0)
                throw new ArgumentException("Rectangle must be inside the texture bounds.", nameof(rect));

            var fSize = Format.GetSize();
            if (sizeof(T) > fSize || fSize % sizeof(T) != 0)
                throw new ArgumentException(
                    $"Type {nameof(T)} is of an invalid size for the format of this texture.", nameof(T));

            int dataByteSize;
            if (Format.IsCompressedFormat())
            {
                Format.GetBlockSize(out int blockWidth, out int blockHeight);
                int blockWidthMinusOne = blockWidth - 1;
                int blockHeightMinusOne = blockHeight - 1;

                // round x and y down to next multiple of block size; width and height up to next multiple of block size
                int roundedWidth = (checkedRect.Width + blockWidthMinusOne) & ~blockWidthMinusOne;
                int roundedHeight = (checkedRect.Height + blockHeightMinusOne) & ~blockHeightMinusOne;
                checkedRect = new Rectangle(checkedRect.X & ~blockWidthMinusOne, checkedRect.Y & ~blockHeightMinusOne,
#if OPENGL
                    // OpenGL only: The last two mip levels require the width and height to be
                    // passed as 2x2 and 1x1, but there needs to be enough data passed to occupy
                    // a full block.
                    checkedRect.Width < blockWidth && texBounds.Width < blockWidth ? texBounds.Width : roundedWidth,
                    checkedRect.Height < blockHeight && texBounds.Height < blockHeight ? texBounds.Height : roundedHeight);
#else
					roundedWidth, roundedHeight);
#endif
                switch (Format)
                {
                    case SurfaceFormat.RgbPvrtc2Bpp:
                    case SurfaceFormat.RgbaPvrtc2Bpp:
                        dataByteSize = (Math.Max(checkedRect.Width, 16) * Math.Max(checkedRect.Height, 8) * 2 + 7) / 8;
                        break;

                    case SurfaceFormat.RgbPvrtc4Bpp:
                    case SurfaceFormat.RgbaPvrtc4Bpp:
                        dataByteSize = (Math.Max(checkedRect.Width, 8) * Math.Max(checkedRect.Height, 8) * 4 + 7) / 8;
                        break;

                    default:
                        dataByteSize = roundedWidth * roundedHeight * fSize / (blockWidth * blockHeight);
                        break;
                }
            }
            else
            {
                dataByteSize = checkedRect.Width * checkedRect.Height * fSize;
            }

            if (data.Length * sizeof(T) != dataByteSize)
                throw new ArgumentException(
                    "The span doesn't hold right amount of bytes for the specified format.", nameof(data));
        }

        #endregion
    }

    public enum SurfaceType
    {
        Texture,
        RenderTarget,
        SwapChainRenderTarget,
    }
}