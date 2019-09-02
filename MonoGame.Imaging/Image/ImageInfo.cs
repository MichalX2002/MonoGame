
namespace MonoGame.Imaging
{
    public class ImageInfo
    {
        public int Width { get; }
        public int Height { get; }
        public int BitDepth { get; }
        public ImageFormat Format { get; }

        public ImageInfo(int width, int height, int bitDepth, ImageFormat format)
        {
            Width = width;
            Height = height;
            BitDepth = bitDepth;
            Format = format;
        }
    }
}
