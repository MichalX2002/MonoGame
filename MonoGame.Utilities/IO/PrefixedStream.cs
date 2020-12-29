using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGame.Framework.IO
{
    public class PrefixedStream : Stream
    {
        private Memory<byte> _prefix;
        private Stream _stream;
        private bool _leaveOpen;

        private long _position;
        private int _readAheadPosition;
        private byte[]? _readAheadBuffer;

        public int ReadAheadLength { get; }

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override bool CanTimeout => _stream.CanTimeout;
        public override int ReadTimeout { get => _stream.ReadTimeout; set => _stream.ReadTimeout = value; }
        public override int WriteTimeout { get => _stream.WriteTimeout; set => _stream.WriteTimeout = value; }

        public override long Length => _stream.Length;
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public PrefixedStream(Memory<byte> prefix, Stream stream, bool leaveOpen)
        {
            _prefix = prefix;
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;

            if (!stream.CanRead)
                throw new IOException("The stream is not readable.");

            ReadAheadLength = 0;
            _readAheadPosition = 0;
        }

        public PrefixedStream(Stream stream, int readAhead, bool leaveOpen)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;

            if (readAhead < 0)
                throw new ArgumentOutOfRangeException(nameof(readAhead), "Value may not be negative.");

            if (!stream.CanRead)
                throw new IOException("The stream is not readable.");

            ReadAheadLength = readAhead;
            if (ReadAheadLength > 0)
                _readAheadBuffer = new byte[ReadAheadLength];
        }

        public PrefixedStream(Memory<byte> prefix, Stream stream) : this(prefix, stream, leaveOpen: false)
        {
        }

        public Memory<byte> GetPrefix(CancellationToken cancellationToken = default)
        {
            FillReadAheadBuffer(cancellationToken);
            return _prefix;
        }

        public async ValueTask<Memory<byte>> GetPrefixAsync(CancellationToken cancellationToken = default)
        {
            await FillReadAheadBufferAsync(cancellationToken).ConfigureAwait(false);
            return _prefix;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            if (buffer.IsEmpty)
                return 0;

            if (_prefix.IsEmpty)
                FillReadAheadBuffer(default);

            int left = buffer.Length;
            int totalRead = 0;

            while (left > 0)
            {
                int read = _position < _prefix.Length
                    ? ReadBufferedPrefix(buffer[totalRead..])
                    : _stream.Read(buffer[totalRead..]);

                if (read == 0)
                    break;

                _position += read;
                totalRead += read;
                left -= read;
            }
            return totalRead;
        }

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (buffer.IsEmpty)
                return 0;

            if (_prefix.IsEmpty)
                await FillReadAheadBufferAsync(cancellationToken).ConfigureAwait(false);

            int left = buffer.Length;
            int totalRead = 0;

            while (left > 0)
            {
                int read = _position < _prefix.Length
                    ? ReadBufferedPrefix(buffer[totalRead..].Span)
                    : await _stream.ReadAsync(buffer[totalRead..], cancellationToken).ConfigureAwait(false);

                if (read == 0)
                    break;

                _position += read;
                totalRead += read;
                left -= read;
            }
            return totalRead;
        }

        private int ReadBufferedPrefix(Span<byte> buffer)
        {
            int position = (int)_position;
            int prefixAvailable = _prefix.Length - position;
            int prefixToRead = Math.Min(prefixAvailable, buffer.Length);
            _prefix.Slice(position, prefixToRead).Span.CopyTo(buffer);
            return prefixToRead;
        }

        private void FillReadAheadBuffer(CancellationToken cancellationToken)
        {
            if (_readAheadPosition < ReadAheadLength && _prefix.IsEmpty)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int read;
                while ((read = _stream.Read(
                    _readAheadBuffer, _readAheadPosition, ReadAheadLength - _readAheadPosition)) > 0)
                {
                    _readAheadPosition += read;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                _prefix = _readAheadBuffer.AsMemory(0, _readAheadPosition);
            }
        }

        private async ValueTask FillReadAheadBufferAsync(CancellationToken cancellationToken)
        {
            if (_readAheadPosition < ReadAheadLength && _prefix.IsEmpty)
            {
                int read;
                while ((read = await _stream.ReadAsync(
                    _readAheadBuffer.AsMemory(_readAheadPosition, ReadAheadLength - _readAheadPosition),
                    cancellationToken).ConfigureAwait(false)) > 0)
                {
                    _readAheadPosition += read;
                }
                _prefix = _readAheadBuffer.AsMemory(0, _readAheadPosition);
            }
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override void Flush() => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_leaveOpen)
                    _stream?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}