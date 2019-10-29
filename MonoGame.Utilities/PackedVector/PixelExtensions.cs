
namespace MonoGame.Utilities.PackedVector
{
    public static class PixelExtensions
    {
        /// <summary>Sets the pixel value from <see cref="Bgra5551"/>.</summary>
        public static void FromBgra5551<T>(this ref T pixel, Bgra5551 source) where T : struct, IPixel
        {
            pixel.FromVector4(source.ToVector4());
        }


        /// <summary>Sets the pixel value from <see cref="Bgr24"/>.</summary>
        public static void FromBgr24<T>(this ref T pixel, Bgr24 source) where T : struct, IPixel
        {
            pixel.FromVector4(source.ToVector4());
        }


        /// <summary>Sets the pixel value from <see cref="Bgra32"/>.</summary>
        public static void FromBgra32<T>(this ref T pixel, Bgra32 source) where T : struct, IPixel
        {
            pixel.FromVector4(source.ToVector4());
        }

        /// <summary>Sets the pixel value from <see cref="Argb32"/>.</summary>
        public static void FromArgb32<T>(this ref T pixel, Argb32 source) where T : struct, IPixel
        {
            pixel.FromVector4(source.ToVector4());
        }
    }
}
