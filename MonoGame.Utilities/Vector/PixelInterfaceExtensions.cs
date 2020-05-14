
namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Provides extension methods that expose the default implementation of
    /// some <see cref="IPixel"/> methods.
    /// </summary>
    public static class PixelInterfaceExtensions
    {
        #region FromGray

        public static void FromGray8<T>(this ref T destination, Gray8 source)
            where T : struct, IPixel
        {
            destination.FromScaledVector4(source.ToScaledVector4());
        }

        public static void FromGray16<T>(this ref T destination, Gray16 source)
            where T : struct, IPixel
        {
            destination.FromScaledVector4(source.ToScaledVector4());
        }

        public static void FromGrayAlpha16<T>(this ref T destination, GrayAlpha16 source)
            where T : struct, IPixel
        {
            destination.FromScaledVector4(source.ToScaledVector4());
        }

        #endregion

        #region FromColor

        public static void FromRgb24<T>(this ref T destination, Rgb24 source)
            where T : struct, IPixel
        {
            destination.FromScaledVector4(source.ToScaledVector4());
        }

        public static void FromColor<T>(this ref T destination, Color source)
            where T : struct, IPixel
        {
            destination.FromScaledVector4(source.ToScaledVector4());
        }

        public static void FromRgb48<T>(this ref T destination, Rgb48 source)
            where T : struct, IPixel
        {
            destination.FromScaledVector4(source.ToScaledVector4());
        }

        public static void FromRgba64<T>(this ref T destination, Rgba64 source)
            where T : struct, IPixel
        {
            destination.FromScaledVector4(source.ToScaledVector4());
        }

        #endregion

        public static Color ToColor<T>(this T pixel)
            where T : IPixel
        {
            return pixel.ToColor();
        }
    }
}
