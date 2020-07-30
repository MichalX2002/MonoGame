// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        internal int ArraySize { get; private set; }

        #region Properties

        /// <summary>
        /// Gets the multiplicative inverse (1/x) of the width and height as a <see cref="Vector2"/>.
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

        static Texture2D()
        {
            InitializeVectorFormats();
        }

        /// <summary>
        /// Creates a new texture of the given size
        /// </summary>
        public Texture2D(GraphicsDevice graphicsDevice, int width, int height)
            : this(graphicsDevice, width, height, false, SurfaceFormat.Rgba32, SurfaceType.Texture, false, 1)
        {
        }

        /// <summary>
        /// Creates a new texture of a given size with a surface format and optional mipmaps 
        /// </summary>
        public Texture2D(
            GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format)
            : this(graphicsDevice, width, height, mipmap, format, SurfaceType.Texture, false, 1)
        {
        }

        /// <summary>
        /// Creates a new texture array of a given size with a surface format and optional mipmaps.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Given <see cref="GraphicsDevice"/> can't work with texture arrays.
        /// </exception>
        public Texture2D(
            GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, int arraySize)
            : this(graphicsDevice, width, height, mipmap, format, SurfaceType.Texture, false, arraySize)
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
            Texel = new Vector2(1f / width, 1f / height);

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
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            ReadOnlySpan<T> data, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where T : unmanaged
        {
            if (rectangle.HasValue)
                if (rectangle.Value.Width == 0 || rectangle.Value.Height == 0)
                    return;

            ValidateParams<T>(level, arraySlice, rectangle, data.Length, out Rectangle checkedRect);

            AssertMainThread(true);

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
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            Span<T> data, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where T : unmanaged
        {
            SetData((ReadOnlySpan<T>)data, rectangle, level, arraySlice);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            ReadOnlyMemory<T> data, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
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
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            Memory<T> data, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where T : unmanaged
        {
            SetData((ReadOnlyMemory<T>)data, rectangle, level, arraySlice);
        }

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <typeparam name="T">The pixel type.</typeparam>
        /// <param name="data">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        public void SetData<T>(
            T[] data, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
            where T : unmanaged
        {
            SetData(data.AsMemory(), rectangle, level, arraySlice);
        }

        #endregion

        #region SetData(Image)

        /// <summary>
        /// Changes the pixels of the texture.
        /// </summary>
        /// <param name="image">New data for the texture.</param>
        /// <param name="rectangle">Area to modify; defaults to texture bounds.</param>
        /// <param name="level">Layer of the texture to modify.</param>
        /// <param name="arraySlice">Index inside the texture array.</param>
        /// <exception cref="ArgumentException">
        ///  <paramref name="arraySlice"/> is greater than 0 and the graphics device does not support texture arrays.
        /// </exception>
        [CLSCompliant(false)]
        public void SetData(
            Image image, Rectangle? rectangle = null, int level = 0, int arraySlice = 0)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            var pixelType = image.PixelType;

            ValidateParams(
                pixelType.ElementSize, pixelType.Type.Name, level, arraySlice, rectangle,
                image.Width * image.Height, out Rectangle checkedRect);

            // TODO: finish this magical function
            throw new NotImplementedException();
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
            ValidateSizes(Unsafe.SizeOf<T>(), elementCount, byteSize);

            var ptr = new UnmanagedMemory<T>(elementCount);
            PlatformGetData(level, arraySlice, checkedRect, ptr.ByteSpan);
            return ptr;
        }

        #endregion

        #region Loading/Construction

        #region FromImage

        [CLSCompliant(false)]
        public static Texture2D FromImage(
            Image image, GraphicsDevice graphicsDevice, bool mipmap, SurfaceFormat format)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            ValidateFromImageParams(graphicsDevice, nameof(graphicsDevice), format, nameof(format));

            var texture = new Texture2D(graphicsDevice, image.Width, image.Height, mipmap, format);
            texture.SetData(image);
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
        ///  but the images should be identical perceptually. 
        ///  This call does not premultiply the image alpha.
        /// </remarks>
        [CLSCompliant(false)]
        public static Texture2D FromStream(
            ImagingConfig imagingConfig,
            Stream stream,
            GraphicsDevice graphicsDevice,
            bool mipmap,
            SurfaceFormat format)
        {
            ValidateFromImageParams(graphicsDevice, nameof(graphicsDevice), format, nameof(format));

            using (var image = Image.Load(imagingConfig, stream))
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
            Stream stream, GraphicsDevice graphicsDevice, bool mipmap, SurfaceFormat format)
        {
            return FromStream(ImagingConfig.Default, stream, graphicsDevice, mipmap, format);
        }

        /// <summary>
        /// Creates a 2D texture from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <param name="config">The configuration used to load the image.</param>
        /// <returns>The <see cref="SurfaceFormat.Rgba32"/> texture created from the image stream.</returns>
        [CLSCompliant(false)]
        public static Texture2D FromStream(
            ImagingConfig config, Stream stream, GraphicsDevice graphicsDevice)
        {
            return FromStream(config, stream, graphicsDevice, mipmap: false, SurfaceFormat.Rgba32);
        }

        /// <summary>
        /// Creates a 2D texture from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the image data.</param>
        /// <param name="graphicsDevice">The graphics device where the texture will be created.</param>
        /// <returns>The <see cref="SurfaceFormat.Rgba32"/> texture created from the image stream.</returns>
        [CLSCompliant(false)]
        public static Texture2D FromStream(Stream stream, GraphicsDevice graphicsDevice)
        {
            return FromStream(
                ImagingConfig.Default, stream, graphicsDevice, mipmap: false, SurfaceFormat.Rgba32);
        }

        #endregion

        #region Reload

        /// <summary>
        /// Reloads the texture without changing the surface format or texture dimensions.
        /// </summary>
        /// <param name="config">The configuration used to load the image.</param>
        /// <param name="stream">The image data with the same dimensions as this texture.</param>
        [CLSCompliant(false)]
        public void Reload(ImagingConfig config, Stream stream)
        {
            static Exception GetSizeException(string fieldName)
            {
                throw new InvalidOperationException(
                    $"The decoded image has a different {fieldName}. " +
                    $"Texture dimensions may not be changed after construction.");
            }

            using (var image = Image.Load(config, stream))
            {
                if (image.Width != Width) throw GetSizeException("width");
                if (image.Height != Height) throw GetSizeException("height");
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
        [CLSCompliant(false)]
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
        /// 
        /// </remarks>
        [CLSCompliant(false)]
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
                    return Image.LoadPixelData<TPixel>(
                        types[0], data.ByteSpan, checkedRect.Size);
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
        [CLSCompliant(false)]
        public void Save(
            ImagingConfig imagingConfig,
            Stream stream,
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0)
        {
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (format == null) throw new ArgumentNullException(nameof(format));

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
        [CLSCompliant(false)]
        public void Save(
            Stream stream,
            ImageFormat format,
            EncoderOptions encoderOptions = null,
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
        [CLSCompliant(false)]
        public void Save(
            ImagingConfig imagingConfig,
            string filePath,
            ImageFormat format = null,
            EncoderOptions encoderOptions = null,
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
        [CLSCompliant(false)]
        public void Save(
            string filePath,
            ImageFormat format = null,
            EncoderOptions encoderOptions = null,
            Rectangle? rectangle = null,
            int level = 0,
            int arraySlice = 0)
        {
            Save(
                ImagingConfig.Default, filePath, format, encoderOptions, rectangle, level, arraySlice);
        }

        #endregion

        #region Parameter Validation

        private static void ValidateFromImageParams(
            GraphicsDevice graphicsDevice, string deviceParamName,
            SurfaceFormat format, string formatParamName)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(deviceParamName);

            if (!VectorFormatsBySurfaceRO.TryGetValue(format, out _))
                throw UnsupportedSurfaceFormatForImagingException(format, formatParamName);
        }

        private void ValidateParams(
            int typeSize, string typeName, int level, int arraySlice, Rectangle? rect,
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

            var fSize = Format.GetSize();
            if (typeSize > fSize || fSize % typeSize != 0)
                throw new ArgumentException(
                    $"Type {typeName} is of an invalid size for the format of this texture.", typeName);

            Rectangle texBounds = CheckRect(level, rect, out checkedRect);

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
                        byteSize = roundedWidth * roundedHeight * fSize / (blockWidth * blockHeight);
                        break;
                }
            }
            else
            {
                byteSize = checkedRect.Width * checkedRect.Height * fSize;
            }
        }

        private void ValidateSizes(int elementSize, int elementCount, int minimumByteSize)
        {
            if (elementCount * elementSize < minimumByteSize)
                throw new ArgumentException(
                    "The given memory is not enough for the current texture format.", nameof(elementCount));
        }

        private void ValidateParams<T>(
            int level, int arraySlice, Rectangle? rect,
            out int byteSize, out Rectangle checkedRect)
        {
            ValidateParams(Unsafe.SizeOf<T>(), typeof(T).Name, level, arraySlice, rect, out byteSize, out checkedRect);
        }

        private void ValidateParams(
            int typeSize, string typeName, int level, int arraySlice, Rectangle? rect, int elementCount,
            out Rectangle checkedRect)
        {
            ValidateParams(typeSize, typeName, level, arraySlice, rect, out int byteSize, out checkedRect);
            ValidateSizes(typeSize, elementCount, byteSize);
        }

        private void ValidateParams<T>(
            int level, int arraySlice, Rectangle? rect, int elementCount, out Rectangle checkedRect)
            where T : unmanaged
        {
            ValidateParams<T>(level, arraySlice, rect, out int minByteSize, out checkedRect);
            ValidateSizes(Unsafe.SizeOf<T>(), elementCount, minByteSize);
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
