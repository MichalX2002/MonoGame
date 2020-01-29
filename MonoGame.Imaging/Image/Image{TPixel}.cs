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
        private Buffer _pixelBuffer;

        #region Public Properties

        public int Stride => _pixelBuffer.PixelStride * Unsafe.SizeOf<TPixel>();

        #endregion

        #region Constructors

        internal Image(Buffer buffer, int width, int height) : base(width, height)
        {
            if (buffer.IsEmpty)
                throw new ArgumentEmptyException(nameof(buffer));

            _pixelBuffer = buffer;
        }

        /// <summary>
        /// Constructs an empty image with the given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public Image(int width, int height) : base(width, height)
        {
            var memory = new UnmanagedMemory<TPixel>(width * height, zeroFill: true);
            _pixelBuffer = new Buffer(memory, width, leaveOpen: false);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _pixelBuffer.Dispose();
            _pixelBuffer = default;
        }
    }
}