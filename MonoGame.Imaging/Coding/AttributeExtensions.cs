
namespace MonoGame.Imaging.Coding
{
    public static class AttributeExtensions
    {
        public static bool HasAttribute<TAttribute>(
            this IImageCoderAttribute item, out TAttribute attribute)
            where TAttribute : IImageCoderAttribute
        {
            if (item is TAttribute attrib)
            {
                attribute = attrib;
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
