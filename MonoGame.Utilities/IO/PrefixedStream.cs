using System;
using System.Diagnostics;
using System.IO;

namespace MonoGame.Utilities.IO
{
    public class PrefixedStream : Stream
    {
        private Memory<byte> _prefix;
        private Stream _stream;
        private int _aheadLength;
        private bool _leaveOpen;

        private long _position;
        private int _aheadPosition;
        private byte[] _aheadBuffer;

        public override bool CanSeek => _stream.CanSeek;
        public override bool CanRead => _stream.CanRead;
        public override bool CanWrite => false;
        public override bool CanTimeout => _stream.CanTimeout;
        public override int ReadTimeout { get => _stream.ReadTimeout; set => _stream.ReadTimeout = value; }
        public override int WriteTimeout { get => _stream.WriteTimeout; set => _stream.WriteTimeout = value; }

        public override long Length => _stream.Length + _aheadLength;
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public PrefixedStream(Memory<byte> prefix, Stream stream, bool leaveOpen)
        {
            if (!stream.CanRead)
                throw new ArgumentException("The stream is not readable.", nameof(stream));

            _prefix = prefix;
            _stream = stream;
            _aheadLength = -1;
            _leaveOpen = leaveOpen;
        }

        public PrefixedStream(Stream stream, int readAhead, bool leaveOpen)
        {
            if (!stream.CanRead)
                throw new ArgumentException("The stream is not readable.", nameof(stream));

            if (readAhead < 0)
                throw new ArgumentOutOfRangeException("Value may not be negative.", nameof(readAhead));

            _stream = stream;
            _aheadLength = readAhead;
            _leaveOpen = leaveOpen;

            if (_aheadLength > 0)
                _aheadBuffer = new byte[_aheadLength];
        }

        public PrefixedStream(Memory<byte> prefix, Stream stream) : this(prefix, stream, leaveOpen: false)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_aheadPosition < _aheadLength && _prefix.IsEmpty)
            {
                int read;
                while ((read = _stream.Read(_aheadBuffer, _aheadPosition, _aheadLength - _aheadPosition)) > 0)
                    _aheadPosition += read;

                _prefix = new Memory<byte>(_aheadBuffer, 0, _aheadPosition);
            }

            if (offset + count > buffer.Length)
                count = buffer.Length - offset;

            int readBytes = 0;

        TryRead:
            if (_position < _prefix.Length)
            {
                int prefixBytesAvailable = _prefix.Length - (int)_position;
                int prefixBytesToRead = Math.Min(prefixBytesAvailable, count);
                _prefix.Slice(0, prefixBytesToRead).CopyTo(buffer.AsMemory(offset));

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
                int read = _stream.Read(buffer, offset + readBytes, count - readBytes);
                _position += read;
                readBytes += read;
            }
            return readBytes;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new InvalidOperationException("The underlying stream is not seekable.");

            if (offset == 0)
                return _position;

            switch (origin)
            {
                case SeekOrigin.Current:
                {
                    long tmp = _position + offset;
                    ValidateSeekPosition(tmp);
                    _position = tmp;
                    break;
                }

                case SeekOrigin.Begin:
                    ValidateSeekPosition(offset);
                    _position = offset;
                    break;

                case SeekOrigin.End:
                {
                    long tmp = Length + offset;
                    ValidateSeekPosition(tmp);
                    _position = tmp;
                    break;
                }

                default:
                    throw new ArgumentException(nameof(origin));
            }

            if (_position < _prefix.Length)
                _stream.Seek(0, SeekOrigin.Begin);
            else
                _stream.Seek(_position - _prefix.Length, SeekOrigin.Begin);

            return _position;
        }

        [DebuggerHidden]
        private void ValidateSeekPosition(long value)
        {
            if (value < 0)
                throw new IOException("Can not seek before stream beginning.");
            if (value > Length)
                throw new IOException("Can not seek past stream end.");
        }

        public override void SetLength(long value) => new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_leaveOpen)
                    _stream?.Dispose();
                _stream = null;
            }
            base.Dispose(disposing);
        }
    }
}