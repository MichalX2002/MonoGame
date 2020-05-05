using System.IO;

namespace MonoGame.Content.Builder.Tasks
{
    public static class StringExtensions
    {
        public static string NormalizePath(this string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
