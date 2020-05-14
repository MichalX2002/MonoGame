using System;
using System.IO;

namespace MonoGame.Framework
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream,
        /// using a pooled buffer.
        /// </summary>
        public static void PooledCopyTo(this Stream source, Stream destination)
        {
            Span<byte> buffer = stackalloc byte[2048];
            int read;
            while ((read = source.Read(buffer)) != 0)
                destination.Write(buffer.Slice(0, read));
        }

        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream,
        /// using a pooled buffer and reporting every write.
        /// </summary>
        public static void PooledCopyTo(
            this Stream source, Stream destination, Action<int> onWrite)
        {
            if (onWrite == null)
            {
                PooledCopyTo(source, destination);
                return;
            }

            Span<byte> buffer = stackalloc byte[2048];
            int read;
            while ((read = source.Read(buffer)) != 0)
            {
                destination.Write(buffer.Slice(0, read));
                onWrite.Invoke(read);
            }
        }
    }
}
