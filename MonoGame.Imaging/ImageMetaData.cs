
namespace MonoGame.Imaging
{
    public readonly struct ImageMetaData
    {
        public int Width { get; }
        public int Height { get; }

        public int BitsPerPixel { get; }
        public int SourceBitsPerPixel { get; }

        public ImageMetaData(int width, int height, int bitsPerPixel) :
            this(width, height, bitsPerPixel, bitsPerPixel)
        {
        }

        internal ImageMetaData(int width, int height, int bpp, int sourceBpp)
        {
            Width = width;
            Height = height;
            BitsPerPixel = bpp;
            SourceBitsPerPixel = sourceBpp;
        }
    }
}
