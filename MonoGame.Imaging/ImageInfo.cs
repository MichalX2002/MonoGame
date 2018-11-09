using System.Text;

namespace MonoGame.Imaging
{
    public class ImageInfo
    {
        public int Width { get; }
        public int Height { get; }
        
        public ImagePixelFormat SourcePixelFormat { get; }
        public ImagePixelFormat DesiredPixelFormat { get; }
        public ImagePixelFormat PixelFormat => DesiredPixelFormat == ImagePixelFormat.Source ? SourcePixelFormat : DesiredPixelFormat;
        public ImageFormat SourceFormat { get; }

        internal ImageInfo(
            int width, int height,
            ImagePixelFormat sourcePixelFormat,
            ImagePixelFormat desiredPixelFormat,
            ImageFormat sourceFormat)
        {
            Width = width;
            Height = height;

            SourcePixelFormat = sourcePixelFormat;
            DesiredPixelFormat = desiredPixelFormat;
            SourceFormat = sourceFormat;
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
            return builder.ToString(); // width x height | source format: pixel format
        }
    }
}
