using System.IO;
using MonoGame.Utilities.Memory;

namespace MonoGame.Utilities
{
    public static class StreamExtensions
    {
        public delegate void CopyProgressCallback(int read, long copied);

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

        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream,
        /// using a pooled buffer and reporting every write.
        /// </summary>
        public static void PooledCopyTo(
            this Stream source, Stream destination, CopyProgressCallback onWrite)
        {
            if (onWrite == null)
            {
                PooledCopyTo(source, destination);
                return;
            }

            byte[] buffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                long copied = 0;
                int read;
                while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
                {
                    destination.Write(buffer, 0, read);

                    copied += read;
                    onWrite.Invoke(read, copied);
                }
            }
            finally
            {
                RecyclableMemoryManager.Default.ReturnBlock(buffer);
            }
        }
    }
}
