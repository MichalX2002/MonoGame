using System.Text;

namespace MonoGame.Imaging
{
    public class ImageInfo
    {
        public int Width { get; }
        public int Height { get; }

        public ImagePixelFormat PixelFormat { get; }
        public ImageFormat SourceFormat { get; }

        internal ImageInfo(int width, int height,
            ImagePixelFormat pixelFormat, ImageFormat sourceFormat)
        {
            Width = width;
            Height = height;

            SourceFormat = sourceFormat;
            PixelFormat = pixelFormat;
        }

        public bool IsValid()
        {
            return !(
                Width == 0 ||
                Height == 0 ||                
                PixelFormat == ImagePixelFormat.Unknown ||
                SourceFormat == ImageFormat.Unknown);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Width).Append('x').Append(Height);
            builder.Append(" | ").Append(SourceFormat);
            builder.Append(": ").Append(PixelFormat);
            return builder.ToString();
        }
    }
}
