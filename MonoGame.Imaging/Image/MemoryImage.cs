using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public class MemoryImage<TPixel> : Image<TPixel>, IPixelMemory<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the data stride (row width including padding) of the image in bytes.
        /// </summary>
        public int ByteStride => Buffer.ByteStride;

        public MemoryImage(int width, int height) : base(width, height)
        {
        }
    }
}
