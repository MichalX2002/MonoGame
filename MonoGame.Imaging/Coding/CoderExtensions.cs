
namespace MonoGame.Imaging
{
    public static class CoderExtensions
    {
        public static bool HasAttribute<TAttribute>(this IImageCoder coder, out TAttribute attribute)
            where TAttribute : IImageFormatAttribute
        {
            if (coder is TAttribute attrib)
            {
                attribute = attrib;
                return true;
            }

            attribute = default;
            return false;
        }

        public static bool HasAttribute<TAttribute>(this IImageCoder coder)
            where TAttribute : IImageFormatAttribute
        {
            return coder.HasAttribute<TAttribute>(out _);
        }
    }
}
