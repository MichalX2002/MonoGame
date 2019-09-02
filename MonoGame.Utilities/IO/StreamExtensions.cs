using System.IO;
using MonoGame.Utilities.Memory;

namespace MonoGame.Utilities
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream,
        /// using a pooled buffer.
        /// </summary>
        public static void PooledCopyTo(this Stream source, Stream destination)
        {
            byte[] buffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                int read;
                while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
                    destination.Write(buffer, 0, read);
            }
            finally
            {
                RecyclableMemoryManager.Default.ReturnBlock(buffer);
            }
        }
    }
}
