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

        /// <summary>
        /// Constructs an empty image with the given size.
        /// </summary>
        /// <param name="size">The size of the image.</param>
        public Image(Size size) : base(VectorTypeInfo.Get<TPixel>(), size)
        {
            var memory = new UnmanagedMemory<TPixel>(size.Width * size.Height, zeroFill: true);
            int byteStride = size.Width * PixelType.ElementSize;
            _buffer = new PixelBuffer(memory, byteStride, leaveOpen: false);
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates an empty image with the given size and pixel type.
        /// </summary>
        public static Image<TPixel> Create(Size size)
        {
            return new Image<TPixel>(size);
        }

        /// <summary>
        /// Creates an empty image with the given size and pixel type.
        /// </summary>
        public static Image<TPixel> Create(int width, int height)
        {
            return new Image<TPixel>(new Size(width, height));
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _buffer.Dispose();
        }
    }
}