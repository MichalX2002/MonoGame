
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public class ImageInfo
    {
        public int Width { get; }
        public int Height { get; }
        public VectorComponentInfo ChannelInfo { get; }
        public ImageFormat Format { get; }

        // TODO: add meta data (that can be read AND written)

        public ImageInfo(int width, int height, VectorComponentInfo channelInfo, ImageFormat format)
        {
            Width = width;
            Height = height;
            ChannelInfo = channelInfo;
            Format = format;
        }
    }
}
