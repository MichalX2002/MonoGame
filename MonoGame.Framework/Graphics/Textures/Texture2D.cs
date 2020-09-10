// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        /// <summary>
        /// Gets the default surface format preferred for most cases.
        /// </summary>
        public const SurfaceFormat DefaultSurfaceFormat = SurfaceFormat.Rgba32;

        /// <summary>
        /// Gets the default vector format based on <see cref="DefaultSurfaceFormat"/>.
        /// </summary>
        public static VectorFormat DefaultVectorFormat { get; }

        internal int ArraySize { get; private set; }

        #region Properties

        /// <summary>
        /// Gets the multiplicative inverse (1/x) of the width.
        /// </summary>
        public float TexelWidth => 1f / Bounds.Width;

        /// <summary>
        /// Gets the multiplicative inverse (1/x) of the height.
        /// </summary>
        public float TexelHeight => 1f / Bounds.Height;

        /// <summary>
        /// Gets the multiplicative inverse (1/x) of the width and height as a <see cref="SizeF"/>.
        /// </summary>
        public SizeF TexelSize => new SizeF(TexelWidth, TexelHeight);

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

        static Texture2D()
        {
            InitializeVectorFormats();

            DefaultVectorFormat = GetVectorFormat(DefaultSurfaceFormat) ??
                throw new TypeLoadException("Failed to load default vector type.");
        }

        /// <summary>
        /// Creates a new texture of a given size with a surface format and optional mipmaps.
        /// </summary>
        public Texture2D(
            GraphicsDevice graphicsDevice,
            int width,
            int height,
            bool mipmap = false,
            SurfaceFormat format = DefaultSurfaceFormat,
            int arraySize = 1) :
            this(
                graphicsDevice, width, height, mipmap, format, SurfaceType.Texture, false, arraySize)
        {
        }

        /// <summary>
        /// Creates a new texture of a given size with a surface format and optional mipmaps.
        /// </summary>
        internal Texture2D(
            GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type)
            : this(graphicsDevice, width, height, mipmap, format, type, false, 1)
        {
        }

        protected Texture2D(
            GraphicsDevice graphicsDevice, int width, int height, bool mipmap,
            SurfaceFormat format, SurfaceType type, bool shared, int arraySize)
            : base(graphicsDevice)
        {
            if (arraySize > 1 && !graphicsDevice.Capabilities.SupportsTextureArrays)
                throw new ArgumentException(
                    "Texture arrays are not supported on this graphics device", nameof(arraySize));

            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));
            ArgumentGuard.AssertGreaterThanZero(height, nameof(height));

            GraphicsDevice = graphicsDevice;
            Bounds = new Rectangle(0, 0, width, height);

            Format = format;
            LevelCount = mipmap ? CalculateMipLevels(width, height) : 1;
            ArraySize = arraySize;

            // Texture will be assigned by the swap chain.
            if (type == SurfaceType.SwapChainRenderTarget)
                return;

            void Construct() => PlatformConstruct(width, height, mipmap, format, type, shared);
            if (IsValidThreadContext)
                Construct();
            else
                Threading.BlockOnMainThread(Construct);
        }

        #endregion

        #region SetData(Span)

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="flush">Whether to flush the device after setting data.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            ReadOnlySpan<T> data,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0,
            bool flush = false)
            where T : unmanaged
        {
            if (rectangle.HasValue)
            {
                var rect = rectangle.GetValueOrDefault();
                if (rect.Width == 0 || rect.Height == 0)
                    return;
            }

            ValidateParams<T>(level, arraySlice, rectangle, data.Length, out Rectangle checkedRect);

            AssertMainThread(true);

            if (rectangle.HasValue)
                PlatformSetData(level, arraySlice, checkedRect, data);
            else
                PlatformSetData(level, arraySlice, rect: null, data);

            if (flush)
                Flush();
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="flush">Whether to flush the device after setting data.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            Span<T> data,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0,
            bool flush = false)
            where T : unmanaged
        {
            SetData((ReadOnlySpan<T>)data, rectangle, level, arraySlice, flush);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="flush">Whether to flush the device after setting data.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            ReadOnlyMemory<T> data,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0,
            bool flush = false)
            where T : unmanaged
        {
            if (rectangle.HasValue)
                if (rectangle.Value.Width == 0 || rectangle.Value.Height == 0)
                    return;

            ValidateParams<T>(level, arraySlice, rectangle, data.Length, out Rectangle checkedRect);

            void SetData()
            {
                if (rectangle.HasValue)
                    PlatformSetData(level, arraySlice, checkedRect, data.Span);
                else
                    PlatformSetData(level, arraySlice, rect: null, data.Span);

                if (flush)
                    Flush();
            }

            if (IsValidThreadContext)
                SetData();
            else
                Threading.BlockOnMainThread(SetData);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="flush">Whether to flush the device after setting data.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            Memory<T> data,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0,
            bool flush = false)
            where T : unmanaged
        {
            SetData((ReadOnlyMemory<T>)data, rectangle, level, arraySlice, flush);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="flush">Whether to flush the device after setting data.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            T[] data,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0,
            bool flush = false)
            where T : unmanaged
        {
            SetData(data.AsMemory(), rectangle, level, arraySlice, flush);
        }

        #endregion

        #region SetData(Image)

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <remarks>
        /// Pixel data is converted to the texture format.
        /// </remarks>
        /// <param name="image">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <param name="flush">Whether to flush the device after setting data.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData(
            Image image,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0,
            bool flush = false)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            var srcVectorType = image.PixelType;

            ValidateParams(
                Format.GetSize(), srcVectorType.Type.Name, level, arraySlice, rectangle,
                image.Width * image.Height, out Rectangle checkedRect);

            var dstVectorFormat = GetVectorFormat(Format);
            var dstVectorTypes = dstVectorFormat.VectorTypes.Span;
            foreach (var vectorType in dstVectorTypes)
            {
                if (srcVectorType == vectorType)
                {
                    int rowStride = checkedRect.Width * vectorType.ElementSize;
                    if (image.ByteStride == rowStride)
                    {
                        SetData(image.GetPixelByteSpan(), checkedRect, level, arraySlice, flush);
                    }
                    else
                    {
                        for (int y = 0; y < checkedRect.Height; y++)
                        {
                            var row = image.GetPixelByteRowSpan(y).Slice(0, rowStride);

                            var textureRect = checkedRect;
                            textureRect.Y += y;
                            textureRect.Height = 1;

                            SetData(row, textureRect, level, arraySlice, flush: false);
                        }

                        if (flush)
                            Flush();
                    }
                    return;
                }
            }

            Span<byte> buffer = stackalloc byte[4096];
            var dstVectorType = dstVectorTypes[0];
            int bufferCapacity = buffer.Length / dstVectorType.ElementSize;
            int srcRowStride = checkedRect.Width * srcVectorType.ElementSize;
            var convertPixels = Image.GetConvertPixelsDelegate(srcVectorType, dstVectorType);

            for (int y = 0; y < checkedRect.Height; y++)
            {
                Span<byte> vectorRow = image.GetPixelByteRowSpan(y).Slice(0, srcRowStride);
                int offsetX = 0;
                do
                {
                    int sliceWidth = checkedRect.Width - offsetX;
                    int count = sliceWidth > bufferCapacity ? bufferCapacity : sliceWidth;
                    var srcSlice = vectorRow.Slice(0, count * srcVectorType.ElementSize);
                    var dstSlice = buffer.Slice(0, count * dstVectorType.ElementSize);

                    convertPixels.Invoke(srcSlice, dstSlice);

                    var textureRect = new Rectangle(
                        checkedRect.X + offsetX,
                        checkedRect.Y + y,
                        width: count,
                        height: 1);

                    SetData(dstSlice, textureRect, level, arraySlice, flush: false);

                    vectorRow = vectorRow.Slice(srcSlice.Length);
                    offsetX += count;
                }
                while (!vectorRow.IsEmpty);
            }

            if (flush)
                Flush();
        }

        #endregion

        #region GetData

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <param name="destination">Destination span for the data.</param>
        /// <param name="rectangle">Area of the texture; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <exception cref="ArgumentEmptyException"><paramref name="destination"/> is empty.</exception>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void GetData<T>(
            Span<T> destination, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where T : unmanaged
        {
            ValidateParams<T>(level, arraySlice, rectangle, destination.Length, out Rectangle checkedRect);

            AssertMainThread(true);

            PlatformGetData(level, arraySlice, checkedRect, MemoryMarshal.AsBytes(destination));
        }

        /// <summary>
        /// Retrieves the contents of the texture.
        /// </summary>
        /// <param name="destination">Destination span for the data.</param>
        /// <param name="rectangle">Area of the texture; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <exception cref="ArgumentEmptyException"><paramref name="destination"/> is empty.</exception>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void GetData<T>(
            Memory<T> destination, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where T : unmanaged
        {
            ValidateParams<T>(level, arraySlice, rectangle, destination.Length, out Rectangle checkedRect);

            void GetData()
            {
                PlatformGetData(level, arraySlice, checkedRect, MemoryMarshal.AsBytes(destination.Span));
            }

            if (IsValidThreadContext)
                GetData();
            else
                Threading.BlockOnMainThread(GetData);
        }

        /// <summary>
        /// Retrieves the contents of the texture and stores them in unmanaged memory.
        /// </summary>
        /// <param name="rectangle">Area of the texture; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public UnmanagedMemory<T> GetData<T>(Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where T : unmanaged
        {
            ValidateParams<T>(level, arraySlice, rectangle, out int byteSize, out Rectangle checkedRect);

            int elementCount = checkedRect.Width * checkedRect.Height;
            ValidateSizes(elementCount, Unsafe.SizeOf<T>(), byteSize);

            var ptr = new UnmanagedMemory<T>(elementCount);
            PlatformGetData(level, arraySlice, checkedRect, ptr.ByteSpan);
            return ptr;
        }

        #endregion

        #region Loading/Construction

        #region FromImage

        /// <summary>
        /// Creates a 2D texture from an image.
        /// </summary>
        /// <param name="image">The image to load pixels from.</param>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="mipmap"><see langword="true"/> to generate mipmaps.</param>
        /// <param name="format">The desired surface format to use for the texture.</param>
        /// <returns>The texture created from the image.</returns>
        public static Texture2D FromImage(
            Image image,
            GraphicsDevice graphicsDevice,
            bool mipmap = false,
            SurfaceFormat? format = DefaultSurfaceFormat)
        {
            // TODO: premultiply alpha option?

            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            var checkedFormat = format ?? GetVectorFormat(image.PixelType.Type)?.SurfaceFormat ?? DefaultSurfaceFormat;
            var texture = new Texture2D(graphicsDevice, image.Width, image.Height, mipmap, checkedFormat);
            texture.SetData(image, new Rectangle(0, 0, image.Width, image.Height));
            return texture;
        }

        #endregion

        #region FromStream

        /// <summary>
        /// Creates a 2D texture from a stream.
        /// </summary>
        /// <param name="imagingConfig">The configuration used to load the image.</param>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="mipmap"><see langword="true"/> to generate mipmaps.</param>
        /// <param name="format">The desired surface format to use for the texture.</param>
        /// <returns>The texture created from the image stream.</returns>
        /// <remarks>
        ///  Different image decoders may generate slight differences between platforms,
        ///  but the images should be perceptually identical. 
        ///  This call does not premultiply the image alpha.
        /// </remarks>
        public static Texture2D FromStream(
            ImagingConfig imagingConfig,
            Stream stream,
            GraphicsDevice graphicsDevice,
            bool mipmap = false,
            SurfaceFormat? format = DefaultSurfaceFormat)
        {
            // TODO: premultiply alpha option?

            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            var vectorFormat = GetVectorFormat(format ?? DefaultSurfaceFormat);
            var vectorType = vectorFormat.VectorTypes.Span[0];

            using (var image = Image.Load(imagingConfig, stream, vectorType))
                return FromImage(image, graphicsDevice, mipmap, format);
        }

        /// <summary>
        /// Creates a 2D texture from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="mipmap"><see langword="true"/> to generate mipmaps.</param>
        /// <param name="format">The desired surface format to use for the texture.</param>
        /// <returns>The texture created from the image stream.</returns>
        public static Texture2D FromStream(
            Stream stream,
            GraphicsDevice graphicsDevice,
            bool mipmap = false,
            SurfaceFormat? format = DefaultSurfaceFormat)
        {
            return FromStream(ImagingConfig.Default, stream, graphicsDevice, mipmap, format);
        }

        #endregion

        #region Reload

        /// <summary>
        /// Reloads the texture without changing the surface format or texture dimensions.
        /// </summary>
        /// <param name="config">The configuration used to load the image.</param>
        /// <param name="stream">The image data with the same dimensions as this texture.</param>
        public void Reload(ImagingConfig config, Stream stream)
        {
            static Exception GetSizeException(string subject)
            {
                throw new InvalidOperationException(
                    $"The decoded image has a different {subject}. " +
                    $"Texture dimensions may not be changed after construction.");
            }

            using (var image = Image.Load(config, stream))
            {
                if (image == null)
                    throw new InvalidDataException("No image in stream.");

                if (image.Width != Width && image.Height != Height)
                    throw GetSizeException("size");
                if (image.Width != Width)
                    throw GetSizeException("width");
                if (image.Height != Height)
                    throw GetSizeException("height");

                SetData(image);
            }
        }

        /// <summary>
        /// Reloads the texture without changing the surface format or texture dimensions.
        /// </summary>
        /// <param name="stream">The image data with the same dimensions as this texture.</param>
        public void Reload(Stream stream)
        {
            Reload(ImagingConfig.Default, stream);
        }

        #endregion

        #endregion

        #region ToImage

        /// <summary>
        /// Retrieves the contents of the texture and puts them into an <see cref="Image"/>.
        /// </summary>
        public Image ToImage(Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
        {
            CheckRect(level, rectangle, out Rectangle checkedRect);

            var saveFormat = GetVectorFormat(Format);
            var data = saveFormat.GetData(this, checkedRect, level, arraySlice);
            try
            {
                return Image.WrapMemory(saveFormat.VectorTypes.Span[0], data, checkedRect.Size, leaveOpen: false);
            }
            catch
            {
                data.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Retrieves the contents of the texture and puts them into an <see cref="Image{TPixel}"/>.
        /// </summary>
        /// <remarks>
        /// The surface format is converted into the specified <typeparamref name="TPixel"/> type.
        /// </remarks>
        public Image<TPixel> ToImage<TPixel>(
            Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            CheckRect(level, rectangle, out Rectangle checkedRect);

            var saveFormat = GetVectorFormat(Format);
            var data = saveFormat.GetData(this, checkedRect, level, arraySlice);
            try
            {
                var types = saveFormat.VectorTypes.Span;
                foreach (var vectorType in types)
                    if (vectorType.Type == typeof(TPixel))
                        return Image.WrapMemory<TPixel>(data, checkedRect.Size, leaveOpen: false);

                using (data)
                {
                    return Image.LoadPixelData<TPixel>(
                        types[0], data.ByteSpan, checkedRect.Size.ToRect(), byteStride: null);
                }
            }
            catch
            {
                data.Dispose();
                throw;
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves the texture to a stream with the specified format and configuration.
        /// </summary>
        /// <param name="imagingConfig">The imaging configuration.</param>
        /// <param name="stream">Destination for the texture.</param>
        /// <param name="format">The format used to encode the texture.</param>
        /// <param name="encoderOptions">The encoder options.</param>
        /// <param name="rectangle">The cutout of the texture to save.</param>
        /// <param name="level">The texture level to save.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        public void Save(
            ImagingConfig imagingConfig,
            Stream stream,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0)
        {
            if (imagingConfig == null)
                throw new ArgumentNullException(nameof(imagingConfig));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            using (var image = ToImage(rectangle, level, arraySlice))
                image.Save(imagingConfig, stream, format, encoderOptions);
        }

        /// <summary>
        /// Saves the texture to a stream with the specified format and configuration.
        /// </summary>
        /// <param name="stream">Destination for the texture.</param>
        /// <param name="format">The format used to encode the texture.</param>
        /// <param name="encoderOptions">The encoder options.</param>
        /// <param name="rectangle">The cutout of the texture to save.</param>
        /// <param name="level">The texture level to save.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        public void Save(
            Stream stream,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0)
        {
            Save(ImagingConfig.Default, stream, format, encoderOptions, rectangle, level, arraySlice);
        }

        /// <summary>
        /// Saves the texture to a file,
        /// picking a format based on the path extension unless a format is specified.
        /// </summary>
        /// <param name="imagingConfig">The imaging configuration.</param>
        /// <param name="filePath">Destination file for the texture.</param>
        /// <param name="format">
        /// The format used to encode the texture.
        /// When null, it will try to find a format
        /// based on the extension of <paramref name="filePath"/>.
        /// </param>
        /// <param name="encoderOptions">The encoder options.</param>
        /// <param name="rectangle">The cutout of the texture to save.</param>
        /// <param name="level">The texture level to save.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        public void Save(
            ImagingConfig imagingConfig,
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0)
        {
            SaveExtensions.AssertValidPath(filePath);

            if (format == null)
                format = ImageFormat.GetByPath(filePath).FirstOrDefault();

            using (var fs = SaveExtensions.OpenWriteStream(filePath))
                Save(imagingConfig, fs, format, encoderOptions, rectangle, level, arraySlice);
        }

        /// <summary>
        /// Saves the texture to a file,
        /// picking a format based on the path extension unless a format is specified.
        /// </summary>
        /// <param name="filePath">Destination file for the texture.</param>
        /// <param name="format">
        /// The format used to encode the texture.
        /// When null, it will try to find a format
        /// based on the extension of <paramref name="filePath"/>.
        /// </param>
        /// <param name="encoderOptions">The encoder options.</param>
        /// <param name="rectangle">The cutout of the texture to save.</param>
        /// <param name="level">The texture level to save.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        public void Save(
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0)
        {
            Save(
                ImagingConfig.Default, filePath, format, encoderOptions, rectangle, level, arraySlice);
        }

        #endregion

        #region Parameter Validation

        private void ValidateParams(
            string typeName, int level, int arraySlice, Rectangle? rect,
            out int byteSize, out Rectangle checkedRect)
        {
            if (level < 0 || level >= LevelCount)
                throw new ArgumentException(
                    $"{nameof(level)} must be smaller than the number of levels in this texture.", nameof(level));

            if (arraySlice > 0 && !GraphicsDevice.Capabilities.SupportsTextureArrays)
                throw new ArgumentException(
                    "Texture arrays are not supported on this graphics device", nameof(arraySlice));

            if (arraySlice < 0 || arraySlice >= ArraySize)
                throw new ArgumentException(
                    $"{nameof(arraySlice)} must be smaller than the " +
                    $"{nameof(ArraySize)} of this texture and larger than 0.", nameof(arraySlice));

            Rectangle texBounds = CheckRect(level, rect, out checkedRect);
            int formatSize = Format.GetSize();

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
                        byteSize = (Math.Max(checkedRect.Width, 16) * Math.Max(checkedRect.Height, 8) * 2 + 7) / 8;
                        break;

                    case SurfaceFormat.RgbPvrtc4Bpp:
                    case SurfaceFormat.RgbaPvrtc4Bpp:
                        byteSize = (Math.Max(checkedRect.Width, 8) * Math.Max(checkedRect.Height, 8) * 4 + 7) / 8;
                        break;

                    default:
                        byteSize = roundedWidth * roundedHeight * formatSize / (blockWidth * blockHeight);
                        break;
                }
            }
            else
            {
                byteSize = checkedRect.Width * checkedRect.Height * formatSize;
            }
        }

        private void ValidateParams<T>(
            int level, int arraySlice, Rectangle? rect,
            out int byteSize, out Rectangle checkedRect)
        {
            ValidateParams(typeof(T).Name, level, arraySlice, rect, out byteSize, out checkedRect);
        }

        private void ValidateParams(
            int typeSize, string typeName, int level, int arraySlice, Rectangle? rect, int elementCount,
            out Rectangle checkedRect)
        {
            ValidateParams(typeName, level, arraySlice, rect, out int byteSize, out checkedRect);
            ValidateSizes(elementCount, typeSize, byteSize);
        }

        private void ValidateParams<T>(
            int level, int arraySlice, Rectangle? rect, int elementCount, out Rectangle checkedRect)
            where T : unmanaged
        {
            ValidateParams<T>(level, arraySlice, rect, out int minByteSize, out checkedRect);
            ValidateSizes(elementCount, Unsafe.SizeOf<T>(), minByteSize);
        }

        private Rectangle CheckRect(int level, Rectangle? rect, out Rectangle checkedRect)
        {
            var texBounds = new Rectangle(0, 0, Math.Max(Bounds.Width >> level, 1), Math.Max(Bounds.Height >> level, 1));
            checkedRect = rect ?? texBounds;
            if (rect.HasValue && !texBounds.Contains(checkedRect) || checkedRect.Width < 0 || checkedRect.Height < 0)
                throw new ArgumentException("Rectangle must be inside the texture bounds.", nameof(rect));
            return texBounds;
        }

        #endregion
    }
}
