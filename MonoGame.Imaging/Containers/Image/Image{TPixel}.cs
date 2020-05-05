using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public partial class Image<TPixel> : Image, IPixelMemory<TPixel>
        where TPixel : unmanaged, IPixel
    {
        private PixelBuffer _buffer;

        protected PixelBuffer Buffer
        {
            get
            {
                AssertNotDisposed();
                return _buffer;
            }
        }

        public override int ByteStride => Buffer.ByteStride;

        #region Constructors

        public Image(PixelBuffer buffer, Size size) : base(VectorTypeInfo.Get<TPixel>(), size)
        {
            if (buffer.IsEmpty)
                throw new ArgumentEmptyException(nameof(buffer));

            _buffer = buffer;
        }

        protected Image(Size size, bool zeroFill) : base(VectorTypeInfo.Get<TPixel>(), size)
        {
            var memory = new UnmanagedMemory<TPixel>(size.Width * size.Height, zeroFill);
            int byteStride = size.Width * PixelType.ElementSize;
            _buffer = new PixelBuffer(memory, byteStride, leaveOpen: false);
        }

        /// <summary>
        /// Constructs an empty image with the given size.
        /// </summary>
        /// <param name="size">The size of the image.</param>
        public Image(Size size) : this(size, zeroFill: true)
        {
        }

        #endregion

        #region Create

        internal static Image<TPixel> CreateCore(Size size, bool zeroFill)
        {
            return new Image<TPixel>(size, zeroFill);
        }

        /// <summary>
        /// Creates an empty image with the given size and pixel type.
        /// </summary>
        public static Image<TPixel> Create(Size size)
        {
            return CreateCore(size, zeroFill: true);
        }

        /// <summary>
        /// Creates an empty image with the given size and pixel type.
        /// </summary>
        public static Image<TPixel> Create(int width, int height)
        {
            return Create(new Size(width, height));
        }

        /// <summary>
        /// Creates an image with the given size and pixel type
        /// without clearing the allocated memory.
        /// </summary>
        public static Image<TPixel> CreateUninitialized(Size size)
        {
            return CreateCore(size, zeroFill: false);
        }

        /// <summary>
        /// Creates an image with the given size and pixel type
        /// without clearing the allocated memory.
        /// </summary>
        public static Image<TPixel> CreateUninitialized(int width, int height)
        {
            return CreateUninitialized(new Size(width, height));
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _buffer.Dispose();
        }
    }
}