using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework
{
    [SkipLocalsInit]
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream,
        /// using a stack-allocated buffer buffer.
        /// </summary>
        public static void StackCopyTo(this Stream source, Stream destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            Span<byte> buffer = stackalloc byte[4096];
            int read;
            while ((read = source.Read(buffer)) != 0)
                destination.Write(buffer.Slice(0, read));
        }

        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream,
        /// using a stack-allocated buffer and reporting every read.
        /// </summary>
        public static void StackCopyTo(
            this Stream source, Stream destination, Action<int>? onRead)
        {
            if (onRead == null)
            {
                StackCopyTo(source, destination);
                return;
            }

            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            Span<byte> buffer = stackalloc byte[4096];
            int read;
            while ((read = source.Read(buffer)) != 0)
            {
                onRead.Invoke(read);
                destination.Write(buffer.Slice(0, read));
            }
        }
    }
}
