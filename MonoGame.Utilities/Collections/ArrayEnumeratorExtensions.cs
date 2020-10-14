
namespace MonoGame.Framework.Collections
{
    public static class ArrayEnumeratorExtensions
    {
        public static ArrayEnumerator<T> GetArrayEnumerator<T>(this T[] array)
        {
            return new ArrayEnumerator<T>(array);
        }
    }
}
