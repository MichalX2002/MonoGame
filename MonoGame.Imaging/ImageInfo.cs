namespace MonoGame.Imaging
{
    public class ImageInfo
    {
        public ImageFormat SourceFormat { get; private set; }
        public ImagePixelFormat PixelFormat { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        internal ImageInfo(
            ImageFormat sourceFormat, ImagePixelFormat pixelFormat, int width, int height)
        {
            SourceFormat = sourceFormat;
            PixelFormat = pixelFormat;

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return !(
                SourceFormat == ImageFormat.Unknown ||
                PixelFormat == ImagePixelFormat.Unknown ||
                Width == 0 ||
                Height == 0);
        }
    }
}
