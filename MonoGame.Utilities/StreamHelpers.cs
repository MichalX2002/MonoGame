using System;
using System.IO;

namespace MonoGame.Framework
{
    /// <summary>Provides methods to help in the implementation of Stream-derived types.</summary>
    internal static partial class StreamHelpers
    {
        /// <summary>Validate the arguments to CopyTo, as would Stream.CopyTo.</summary>
        public static void ValidateCopyToArgs(Stream source, Stream destination, int bufferSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(bufferSize), bufferSize, SR.ArgumentOutOfRange_NeedPosNum);
            
            bool sourceCanRead = source.CanRead;
            if (!sourceCanRead && !source.CanWrite)
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            
            bool destinationCanWrite = destination.CanWrite;
            if (!destinationCanWrite && !destination.CanRead)
                throw new ObjectDisposedException(nameof(destination), SR.ObjectDisposed_StreamClosed);
        
            if (!sourceCanRead)
                throw new NotSupportedException(SR.NotSupported_UnreadableStream);
        
            if (!destinationCanWrite)
                throw new NotSupportedException(SR.NotSupported_UnwritableStream);
        }

        internal class SR
        {
            public static string @ArgumentOutOfRange_NeedNonNegNum => "Non-negative number required.";

            public static string @ArgumentOutOfRange_NeedPosNum => "Positive number required.";

            public static string @ObjectDisposed_StreamClosed => "Cannot access a closed Stream.";

            public static string @NotSupported_UnreadableStream => "Stream does not support reading.";

            public static string @NotSupported_UnseekableStream => "Stream does not support seeking.";

            public static string @NotSupported_UnwritableStream => "Stream does not support writing.";

            public static string @NotSupported_CannotWriteToBufferedStreamIfReadBufferCannotBeFlushed =>
                "Cannot write to a BufferedStream while the read buffer is not empty if " +
                "the underlying stream is not seekable. " +
                "Ensure that the stream underlying this BufferedStream can seek or " +
                "avoid interleaving read and write operations on this BufferedStream.";

            public static string @Argument_InvalidOffLen =>
                "Offset and length were out of bounds for the array or count is greater than " +
                "the number of elements from index to the end of the source collection.";
        }
    }
}
