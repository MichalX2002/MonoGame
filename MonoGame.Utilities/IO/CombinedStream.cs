using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MonoGame.Framework.IO
{
    /// <summary>
    /// Combines multiple streams into a larger stream that can only be read once.
    /// </summary>
    public class CombinedStream : Stream
    {
        private Queue<Slice> _slices;
        private long _position;

        #region Properties

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        /// <summary>
        /// Gets the total length of the stream. Returns -1 if any inner stream is not seekable.
        /// </summary>
        public override long Length
        {
            get
            {
                long total = 0;
                foreach (var slice in _slices)
                {
                    if (!slice.Stream.CanSeek)
                        return -1;
                    total += slice.Stream.Length;
                }
                return total;
            }
        }

        /// <summary>
        /// Gets whether any inner stream can time out.
        /// </summary>
        public override bool CanTimeout
        {
            get
            {
                foreach (var slice in _slices)
                    if (slice.Stream.CanTimeout)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// Gets the read position within the stream.
        /// </summary>
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the <see cref="CombinedStream"/> with two streams, 
        /// optionally leaving them open after disposal.
        /// </summary>
        public CombinedStream(Stream first, bool leaveFirstOpen, Stream second, bool leaveSecondOpen)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            _slices = new Queue<Slice>(2);
            _slices.Enqueue(new Slice(first, leaveFirstOpen));
            _slices.Enqueue(new Slice(second, leaveSecondOpen));
        }

        /// <summary>
        /// Constructs the <see cref="CombinedStream"/> with two streams, not leaving them open after disposal.
        /// </summary>
        public CombinedStream(Stream first, Stream second) : this(first, true, second, true)
        {
        }

        /// <summary>
        /// Constructs the <see cref="CombinedStream"/> from an enumerable of streams,
        /// optionally leaving them open after disposal.
        /// </summary>
        public CombinedStream(IEnumerable<Stream> streams, bool leaveOpen)
        {
            _slices = new Queue<Slice>(streams is ICollection coll ? coll.Count : 4);
            foreach (var stream in streams)
                _slices.Enqueue(new Slice(stream, leaveOpen));
        }

        /// <summary>
        /// Constructs the <see cref="CombinedStream"/> from an enumerable of streams.
        /// Use this to individually control which streams are left open after disposal.
        /// </summary>
        public CombinedStream(IEnumerable<Slice> slices)
        {
            _slices = new Queue<Slice>(slices);
        }

        #endregion

        public override int Read(byte[] buffer, int offset, int count)
        {
            TryRead:
            if (_slices.Count > 0)
            {
                if (offset + count > buffer.Length)
                    count = buffer.Length - offset;

                int read = _slices.Peek().Stream.Read(buffer, offset, count);
                if (read == 0)
                {
                    DisposeOneSlice();
                    goto TryRead;
                }
                else
                {
                    _position += read;
                    return read;
                }
            }
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        private void DisposeOneSlice()
        {
            var slice = _slices.Dequeue();
            if (!slice.LeaveOpen)
                slice.Stream.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    while (_slices.Count > 0)
                        DisposeOneSlice();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Represents a <see cref="System.IO.Stream"/> that is optionally disposed after use.
        /// </summary>
        public readonly struct Slice
        {
            public readonly Stream Stream;
            public readonly bool LeaveOpen;

            public Slice(Stream stream, bool leaveOpen)
            {
                Stream = stream;
                LeaveOpen = leaveOpen;
            }
        }
    }
}