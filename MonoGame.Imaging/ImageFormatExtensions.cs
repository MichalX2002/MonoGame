
namespace MonoGame.Imaging
{
    public static class ImageFormatExtensions
    {
        public static ImageSaveFormat ToSaveFormat(this ImageFormat format)
        {
            switch(format)
            {
                case ImageFormat.Bmp: return ImageSaveFormat.Bmp;
                case ImageFormat.Png: return ImageSaveFormat.Png;
                case ImageFormat.Jpg: return ImageSaveFormat.Jpg;
                case ImageFormat.Tga: return ImageSaveFormat.Tga;

                default: return ImageSaveFormat.Unknown;
            }
        }
    }
}
