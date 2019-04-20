using System;
using System.IO;

namespace MonoGame.Utilities
{
    public class LengthStream : Stream
    {
        private Stream _stream;
        private long _length;

        public LengthStream(Stream stream, long length)
        {
            _stream = stream;
            _length = length;
        }

        public override bool CanRead => _stream.CanRead;
        public override bool CanWrite => false;
        public override bool CanSeek => false;
        public override long Length => _length;

        public override long Position
        {
            get => _stream.Position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }
            _length = 0;

            base.Dispose(disposing);
        }
    }
}