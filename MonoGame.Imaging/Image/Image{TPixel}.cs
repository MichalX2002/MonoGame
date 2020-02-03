using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public partial class Image<TPixel> : Image, IPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        protected PixelBuffer Buffer { get; private set; }

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
            Buffer = new PixelBuffer(memory, width, leaveOpen: false);
        }

        #endregion



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Buffer.Dispose();
            Buffer = default;
        }
    }
}