using System.IO;

namespace MonoGame.Utilities
{
    public class DisposeCallbackStream : Stream
    {
        public delegate void DisposeDelegate(DisposeCallbackStream sender, bool disposing);

        private Stream _innerStream;
        private bool _leaveOpen;

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;
        public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }

        public DisposeDelegate OnDisposing;
        public DisposeDelegate OnDispose;

        public DisposeCallbackStream(Stream innerStream, bool leaveOpen) : base()
        {
            _innerStream = innerStream;
            _leaveOpen = leaveOpen;
        }

        public DisposeCallbackStream(Stream innerStream) : this(innerStream, false)
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);
        public override void Flush() => _innerStream.Flush();

        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
        public override void SetLength(long value) => _innerStream.SetLength(value);

        protected override void Dispose(bool disposing)
        {
            OnDisposing?.Invoke(this, disposing);
            if (disposing)
            {
                if (!_leaveOpen)
                {
                    _innerStream?.Dispose();
                    _innerStream = null;
                }
            }
            OnDispose?.Invoke(this, disposing);
            base.Dispose(disposing);
        }
    }
}
