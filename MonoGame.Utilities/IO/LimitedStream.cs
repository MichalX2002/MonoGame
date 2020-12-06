using System;
using System.Diagnostics;
using System.IO;

namespace MonoGame.Framework.IO
{
    public class LimitedStream : Stream
    {
        private Stream _stream;
        private long _length;
        private bool _leaveOpen;

        public override bool CanRead => _stream.CanRead;
        public override bool CanWrite => false;
        public override bool CanSeek => _stream.CanRead;
        public override bool CanTimeout => _stream.CanTimeout;
        public override long Length => _length;

        public override long Position
        {
            get => _stream.Position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public LimitedStream(Stream stream, long length, bool leaveOpen)
        {
            _stream = stream;
            _length = length;
            _leaveOpen = leaveOpen;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = (int)Math.Max(0, Math.Min(count, Length - Position));
            if (count == 0)
                return 0;
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new NotSupportedException("The inner stream is not seekable.");

            switch (origin)
            {
                case SeekOrigin.Begin:
                    AssertRange(offset);
                    break;

                case SeekOrigin.Current:
                    AssertRange(Position + offset);
                    break;

                case SeekOrigin.End:
                    offset += Length;
                    AssertRange(offset);
                    origin = SeekOrigin.Begin; // don't read at the end of the inner stream
                    break;
            }
            return _stream.Seek(offset, origin);
        }

        [DebuggerHidden]
        private void AssertRange(long newPosition)
        {
            if (newPosition > Length)
                throw new ArgumentOutOfRangeException("The new position exceeds the strict length.", "offset");
        }

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_leaveOpen)
                    _stream?.Dispose();
                _stream = null!;
            }
            base.Dispose(disposing);
        }
    }
}