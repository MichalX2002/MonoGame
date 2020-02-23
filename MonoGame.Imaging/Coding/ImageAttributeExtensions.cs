using MonoGame.Imaging.Attributes;

namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Helper for checking attributes on image codecs and formats,
    /// being an <see langword="is"/> check codewise.
    /// </summary>
    public static class ImageAttributeExtensions
    {
        public static bool HasAttribute<TAttribute>(
            this IImageCodecAttribute item, out ImageAttributeQuery attribute)
            where TAttribute : IImageCodecAttribute
        {
            if (item is TAttribute codecAttrib)
            {
                attribute = new ImageAttributeQuery(codecAttrib);
                return true;
            }
            attribute = default;
            return false;
        }

        public static bool HasAttribute<TAttribute>(this IImageCodecAttribute item)
            where TAttribute : IImageCodecAttribute
        {
            return item.HasAttribute<TAttribute>(out _);
        }
    }
}
