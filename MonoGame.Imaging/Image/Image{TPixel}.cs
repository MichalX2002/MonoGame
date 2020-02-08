using System.Runtime.CompilerServices;
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
            private set => _buffer = value;
        }

        public override int ByteStride => Buffer.ByteStride;

        #region Constructors

        public Image(PixelBuffer buffer, int width, int height) :
            base(width, height, PixelTypeInfo.Get(typeof(TPixel)))
        {
            if (buffer.IsEmpty)
                throw new ArgumentEmptyException(nameof(buffer));

            Buffer = buffer;
        }

        /// <summary>
        /// Constructs an empty image with the given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public Image(int width, int height) : 
            base(width, height, PixelTypeInfo.Get(typeof(TPixel)))
        {
            var memory = new UnmanagedMemory<TPixel>(width * height, zeroFill: true);
            Buffer = new PixelBuffer(memory, width * Unsafe.SizeOf<TPixel>(), leaveOpen: false);
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates an empty image.
        /// </summary>
        public static Image<TPixel> Create(int width, int height)
        {
            return new Image<TPixel>(width, height);
        }

        /// <summary>
        /// Creates an empty image.
        /// </summary>
        public static Image<TPixel> Create(Size size)
        {
            return new Image<TPixel>(size.Width, size.Height);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Buffer.Dispose();
        }
    }
}