using System;
using System.Diagnostics;
using System.IO;

namespace MonoGame.Utilities
{
    public class PrefixedStream : Stream
    {
        private byte[] _prefix;
        private Stream _stream;
        private bool _leaveOpen;
        private long _position;

        public override bool CanSeek => _stream.CanSeek;
        public override bool CanRead => _stream.CanRead;
        public override bool CanWrite => false;
        public override long Length => _prefix.Length + _stream.Length;

        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public PrefixedStream(byte[] prefix, Stream stream, bool leaveOpen)
        {
            if (!stream.CanRead)
                throw new ArgumentException("Stream is not readable.", nameof(stream));

            _prefix = prefix;
            _stream = stream;
            _leaveOpen = leaveOpen;
        }

        public PrefixedStream(byte[] prefix, Stream stream) : this(prefix, stream, leaveOpen: false)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readBytes = 0;

            TryRead:
            if (_position < _prefix.Length)
            {
                int prefixBytesAvailable = _prefix.Length - (int)_position;
                int prefixBytesToRead = Math.Min(prefixBytesAvailable, count);
                for (int i = 0; i < prefixBytesToRead; i++)
                    buffer[offset + i] = _prefix[i];

                if (count > prefixBytesAvailable)
                {
                    _position += prefixBytesToRead;
                    readBytes += prefixBytesToRead;
                    count -= prefixBytesToRead;
                    goto TryRead;
                }
                return readBytes;
            }
            else
            {
                int read = _stream.Read(buffer, offset + readBytes, count);
                _position += read;
                readBytes += read;
            }
            return readBytes;
        }

        [DebuggerHidden]
        private void CheckSeekPosition(long value)
        {
            if (value < 0)
                throw new IOException("Cannot seek before begin.");
            if (value > Length)
                throw new IOException("Cannot seek past end.");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new InvalidOperationException();

            if (offset == 0)
                return _position;

            switch (origin)
            {
                case SeekOrigin.Current:
                {
                    long tmp = _position + offset;
                    CheckSeekPosition(tmp);
                    _position = tmp;
                    break;
                }

                case SeekOrigin.Begin:
                    CheckSeekPosition(offset);
                    _position = offset;
                    break;

                case SeekOrigin.End:
                {
                    long tmp = Length + offset;
                    CheckSeekPosition(tmp);
                    _position = tmp;
                    break;
                }

                default:
                    throw new ArgumentException($"Invalid seek origin ({origin}).", nameof(origin));
            }

            if (_position < _prefix.Length)
                _stream.Seek(0, SeekOrigin.Begin);
            else
                _stream.Seek(_position - _prefix.Length, SeekOrigin.Begin);

            return _position;
        }

        public override void SetLength(long value) => new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (!_leaveOpen)
                    _stream?.Dispose();
            }

            _position = 0;
            _stream = null;

            base.Dispose(disposing);
        }
    }
}