using System;
using System.IO;

namespace MonoGame.Framework.IO
{
    /// <summary>
    /// Calculates a <see cref="Crc32"/> checksum on 
    /// bytes read/written to the underlying stream. 
    /// </summary>
    /// <remarks>
    /// This class can be used to verify the CRC of a ZipEntry when reading
    /// from a stream, or to calculate a CRC when writing to a stream.
    /// The stream should be used to either read, or write, but not both.
    /// If you intermix reads and writes, the results are not defined.
    /// </remarks>
    public class CrcCalculatorStream : Stream
    {
        private const long UnsetLengthLimit = -99;
        private readonly long _lengthLimit = UnsetLengthLimit;

        private Stream _innerStream;
        private bool _leaveOpen;
        private Crc32 _crc32;

        /// <summary>
        /// Gets the total number of bytes run through the CRC32 calculator.
        /// </summary>
        /// <remarks>
        /// This is either the total number of bytes read, or the total number of
        /// bytes written, depending on the direction of this stream.
        /// </remarks>
        public long TotalBytesSlurped => _crc32.TotalBytesRead;

        /// <summary>
        /// Provides the current CRC for all blocks slurped in.
        /// </summary>
        /// <remarks>
        /// The running total of the CRC is kept as data is written or read
        /// through the stream.  read this property after all reads or writes
        /// to get an accurate CRC for the entire stream.
        /// </remarks>
        public int Crc => _crc32.Crc32Result;

        /// <summary>
        /// Indicates whether the stream supports reading.
        /// </summary>
        public override bool CanRead => _innerStream.CanRead;

        /// <summary>
        /// Indicates whether the stream supports seeking.
        /// </summary>
        /// <remarks>
        /// Always returns false.
        /// </remarks>
        public override bool CanSeek => false;

        /// <summary>
        /// Indicates whether the stream supports writing.
        /// </summary>
        public override bool CanWrite => _innerStream.CanWrite;

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length
        {
            get => _lengthLimit == UnsetLengthLimit ?
                    _innerStream.Length : _lengthLimit;
        }

        /// <summary>
        /// The getter for this property returns the total bytes read.
        /// The setter will throw <see cref="NotSupportedException"/>.
        /// </summary>
        public override long Position
        {
            get => _crc32.TotalBytesRead;
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Constructs the calculator stream, disposing the underlying stream after disposal.
        /// </summary>
        /// <remarks>
        /// The stream uses the default CRC32 algorithm, which implies a polynomial of 0xEDB88320.
        /// </remarks>
        /// <param name="stream">The underlying stream</param>
        public CrcCalculatorStream(Stream stream) :
            this(false, UnsetLengthLimit, stream, null)
        {
        }

        /// <summary>
        /// Constructs the calculator stream, optionally leaving the underlying stream open after disposal.
        /// </summary>
        /// <remarks>
        /// The stream uses the default CRC32 algorithm, which implies a  of 0xEDB88320.
        /// </remarks>
        /// <param name="stream">The underlying stream.</param>
        /// <param name="leaveOpen">true to leave the underlying stream open upon disposal.</param>
        public CrcCalculatorStream(Stream stream, bool leaveOpen) :
            this(leaveOpen, UnsetLengthLimit, stream, null)
        {
        }

        /// <summary>
        /// Constructs the calculator stream with a limited read range,
        /// disposing the underlying stream after disposal.
        /// </summary>
        /// <remarks>
        /// The stream uses the default CRC32 algorithm, which implies a polynomial of 0xEDB88320.
        /// </remarks>
        /// <param name="stream">The underlying stream</param>
        /// <param name="length">The length of the stream to slurp</param>
        public CrcCalculatorStream(Stream stream, long length)
            : this(true, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException(nameof(length));
        }

        /// <summary>
        /// Constructs the calculator stream with a limited read range,
        /// optionally leaving the underlying stream open after disposal.
        /// <see cref="Stream.Close"/>.
        /// </summary>
        /// <remarks>
        /// The stream uses the default CRC32 algorithm, which implies a
        /// polynomial of 0xEDB88320.
        /// </remarks>
        /// <param name="stream">The underlying stream.</param>
        /// <param name="length">The length of the stream to slurp.</param>
        /// <param name="leaveOpen">true to leave the underlying stream open upon disposal.</param>
        public CrcCalculatorStream(Stream stream, long length, bool leaveOpen)
            : this(leaveOpen, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException(nameof(length));
        }

        /// <summary>
        /// Constructs the calculator stream with a limited read range,
        /// using an existing <see cref="Crc32"/> instance, and optionally
        /// leaving the underlying stream open after disposal.
        /// </summary>
        /// <remarks>
        /// The stream uses the specified <see cref="Crc32"/> instance, which allows the
        /// application to specify how the CRC gets calculated.
        /// </remarks>
        /// <param name="stream">The underlying stream.</param>
        /// <param name="length">The length of the stream to slurp.</param>
        /// <param name="leaveOpen">true to leave the underlying stream open upon disposal.</param>
        /// <param name="crc32">The <see cref="Crc32"/> instance used for calculation.</param>
        public CrcCalculatorStream(Stream stream, long length, bool leaveOpen, Crc32 crc32)
            : this(leaveOpen, length, stream, crc32)
        {
            if (length < 0)
                throw new ArgumentException(nameof(length));
        }


        // This ctor is private - no validation is done here.  This is to allow the use
        // of a (specific) negative value for the _lengthLimit, to indicate that there
        // is no length set.  So we validate the length limit in those ctors that use an
        // explicit param, otherwise we don't validate, because it could be our special
        // value.
        private CrcCalculatorStream
            (bool leaveOpen, long length, Stream stream, Crc32 crc32) : base()
        {
            _innerStream = stream;
            _crc32 = crc32 ?? new Crc32();
            _lengthLimit = length;
            _leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Read from the stream.
        /// </summary>
        /// <param name="buffer">the buffer to read</param>
        /// <param name="offset">the offset at which to start</param>
        /// <param name="count">the number of bytes to read</param>
        /// <returns>the number of bytes actually read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Need to limit the X of bytes returned, if the stream is intended to have
            // a definite length. This is especially useful when returning a stream for
            // the uncompressed data directly to the application. The app won't
            // necessarily read only the UncompressedSize number of bytes. 
            // For example wrapping the stream returned from OpenReader() into a StreamReader()
            // and calling ReadToEnd() on it, We can "over-read" the zip data and get a
            // corrupt string. The length limits it, preventing that problem.

            long bytesToRead = count;
            if (_lengthLimit != UnsetLengthLimit)
            {
                if (_crc32.TotalBytesRead >= _lengthLimit)
                    return 0; // EOF

                long bytesRemaining = _lengthLimit - _crc32.TotalBytesRead;
                if (bytesRemaining < count)
                    bytesToRead = bytesRemaining;
            }
            int n = _innerStream.Read(buffer, offset, (int)bytesToRead);
            if (n > 0)
                _crc32.SlurpBlock(buffer, offset, n);
            return n;
        }

        /// <summary>
        /// Write to the stream.
        /// </summary>
        /// <param name="buffer">the buffer from which to write</param>
        /// <param name="offset">the offset at which to start writing</param>
        /// <param name="count">the number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > 0)
                _crc32.SlurpBlock(buffer, offset, count);
            _innerStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Flush the stream.
        /// </summary>
        public override void Flush() => _innerStream.Flush();

        /// <summary>
        /// Seeking is not supported on this stream.
        /// This method always throws <see cref="NotSupportedException"/>
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <summary>
        /// This method always throws <see cref="NotSupportedException"/>
        /// </summary>
        public override void SetLength(long value) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_leaveOpen)
                    _innerStream?.Dispose();
                _innerStream = null;
            }
            base.Dispose(disposing);
        }
    }
}
