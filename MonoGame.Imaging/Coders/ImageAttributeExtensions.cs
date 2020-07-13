using MonoGame.Imaging.Attributes;

namespace MonoGame.Imaging.Coders
{
    /// <summary>
    /// Helper for checking attributes on image coders and formats,
    /// being an <see langword="is"/> check codewise.
    /// </summary>
    public static class ImageAttributeExtensions
    {
        public static bool HasAttribute<TAttribute>(
            this IImageCoderAttribute item, out ImageAttributeQuery attribute)
            where TAttribute : IImageCoderAttribute
        {
            if (item is TAttribute attrib)
            {
                attribute = new ImageAttributeQuery(attrib);
                return true;
            }
            attribute = default;
            return false;
        }

        public static bool HasAttribute<TAttribute>(this IImageCoderAttribute item)
            where TAttribute : IImageCoderAttribute
        {
            return item.HasAttribute<TAttribute>(out _);
        }
    }
}
