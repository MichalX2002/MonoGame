using System;
using System.IO;

namespace MonoGame.Utilities
{
    /// <summary>
    /// A stream that calculates a <see cref="Crc32"/> (a checksum)
    /// on all bytes read or on all bytes written.
    /// </summary>
    ///
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
        private Crc32 _crc32;

        /// <summary>
        /// Gets the total number of bytes run through the CRC32 calculator.
        /// </summary>
        /// <remarks>
        /// This is either the total number of bytes read, or the total number of
        /// bytes written, depending on the direction of this stream.
        /// </remarks>
        public Int64 TotalBytesSlurped
        {
            get { return _crc32.TotalBytesRead; }
        }

        /// <summary>
        /// Provides the current CRC for all blocks slurped in.
        /// </summary>
        /// <remarks>
        /// The running total of the CRC is kept as data is written or read
        /// through the stream.  read this property after all reads or writes
        /// to get an accurate CRC for the entire stream.
        /// </remarks>
        public Int32 Crc
        {
            get { return _crc32.Crc32Result; }
        }

        /// <summary>
        /// Indicates whether the underlying stream will be left open when
        /// the <see cref="CrcCalculatorStream"/> is closed.
        /// </summary>
        /// <remarks>
        /// Set this at any point before calling <see cref="Stream.Close"/>.
        /// </remarks>
        public bool LeaveOpen { get; set; }

        /// <summary>
        /// Indicates whether the stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return _innerStream.CanRead; }
        }

        /// <summary>
        /// Indicates whether the stream supports seeking.
        /// </summary>
        /// <remarks>
        /// Always returns false.
        /// </remarks>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Indicates whether the stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return _innerStream.CanWrite; }
        }

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return _lengthLimit == UnsetLengthLimit ?
                    _innerStream.Length : _lengthLimit;
            }
        }

        /// <summary>
        /// The getter for this property returns the total bytes read.
        /// The setter will throw <see cref="NotSupportedException"/>.
        /// </summary>
        public override long Position
        {
            get { return _crc32.TotalBytesRead; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <remarks>
        /// Instances returned from this constructor will leave the underlying
        /// stream open upon <see cref="Stream.Close"/>. The stream uses the
        /// default CRC32 algorithm, which implies a polynomial of 0xEDB88320.
        /// </remarks>
        /// <param name="stream">The underlying stream</param>
        public CrcCalculatorStream(Stream stream) :
            this(true, UnsetLengthLimit, stream, null)
        {
        }

        /// <summary>
        /// The constructor allows the caller to specify how to handle the
        /// underlying stream at close.
        /// </summary>
        /// <remarks>
        /// The stream uses the default CRC32 algorithm, which implies a
        /// polynomial of 0xEDB88320.
        /// </remarks>
        /// <param name="stream">The underlying stream</param>
        /// <param name="leaveOpen">true to leave the underlying stream
        /// open upon close of the <c>CrcCalculatorStream</c>; false otherwise.</param>
        public CrcCalculatorStream(Stream stream, bool leaveOpen) :
            this(leaveOpen, UnsetLengthLimit, stream, null)
        {
        }

        /// <summary>
        /// A constructor allowing the specification of the length of the
        /// stream to read.
        /// </summary>
        /// <remarks>
        /// The stream uses the default CRC32 algorithm, which implies a
        /// polynomial of 0xEDB88320.
        /// Instances returned from this constructor will leave the underlying
        /// stream open upon <see cref="Stream.Close"/>.
        /// </remarks>
        /// <param name="stream">The underlying stream</param>
        /// <param name="length">The length of the stream to slurp</param>
        public CrcCalculatorStream(Stream stream, Int64 length)
            : this(true, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException(nameof(length));
        }

        /// <summary>
        /// A constructor allowing the specification of the length of the stream
        /// to read, as well as whether to keep the underlying stream open upon
        /// <see cref="Stream.Close"/>.
        /// </summary>
        /// <remarks>
        /// The stream uses the default CRC32 algorithm, which implies a
        /// polynomial of 0xEDB88320.
        /// </remarks>
        /// <param name="stream">The underlying stream</param>
        /// <param name="length">The length of the stream to slurp</param>
        /// <param name="leaveOpen">true to leave the underlying stream
        /// open upon close of the <c>CrcCalculatorStream</c>; false otherwise.</param>
        public CrcCalculatorStream(Stream stream, Int64 length, bool leaveOpen)
            : this(leaveOpen, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException(nameof(length));
        }

        /// <summary>
        /// A constructor allowing the specification of the length of the stream
        /// to read, as well as whether to keep the underlying stream open upon
        /// <see cref="Stream.Close"/>, and the <see cref="Crc32"/> instance to use.
        /// </summary>
        /// <remarks>
        /// The stream uses the specified <see cref="Crc32"/> instance, which allows the
        /// application to specify how the CRC gets calculated.
        /// </remarks>
        /// <param name="stream">The underlying stream</param>
        /// <param name="length">The length of the stream to slurp</param>
        /// <param name="leaveOpen">true to leave the underlying stream
        /// open upon close of the <see cref="CrcCalculatorStream"/>; false otherwise.</param>
        /// <param name="crc32">the CRC32 instance to use to calculate the CRC32</param>
        public CrcCalculatorStream(Stream stream, Int64 length, bool leaveOpen, Crc32 crc32)
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
            (bool leaveOpen, Int64 length, Stream stream, Crc32 crc32) : base()
        {
            _innerStream = stream;
            _crc32 = crc32 ?? new Crc32();
            _lengthLimit = length;
            LeaveOpen = leaveOpen;
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
            // necessarily read only the UncompressedSize number of bytes. For example
            // wrapping the stream returned from OpenReader() into a StreadReader() and
            // calling ReadToEnd() on it, We can "over-read" the zip data and get a
            // corrupt string.  The length limits that, prevents that problem.

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
        public override void Flush()
        {
            _innerStream.Flush();
        }

        /// <summary>
        /// Seeking is not supported on this stream.
        /// This method always throws <see cref="NotSupportedException"/>
        /// </summary>
        /// <param name="offset">N/A</param>
        /// <param name="origin">N/A</param>
        /// <returns>N/A</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method always throws <see cref="NotSupportedException"/>
        /// </summary>
        /// <param name="value">N/A</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
