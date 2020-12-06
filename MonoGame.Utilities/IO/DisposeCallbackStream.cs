using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGame.Framework.IO
{
    public class DisposeCallbackStream : Stream
    {
        public delegate void DisposeDelegate(DisposeCallbackStream sender, bool disposing);

        private Stream _stream;
        private bool _leaveOpen;

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;
        public override bool CanTimeout => _stream.CanTimeout;
        public override int ReadTimeout { get => _stream.ReadTimeout; set => _stream.ReadTimeout = value; }
        public override int WriteTimeout { get => _stream.WriteTimeout; set => _stream.WriteTimeout = value; }

        public override long Length => _stream.Length;
        public override long Position { get => _stream.Position; set => _stream.Position = value; }

        public event DisposeDelegate? OnDisposing;
        public event DisposeDelegate? OnDispose;

        public DisposeCallbackStream(Stream innerStream, bool leaveOpen) : base()
        {
            _stream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _leaveOpen = leaveOpen;
        }

        public DisposeCallbackStream(Stream innerStream) : this(innerStream, false)
        {
        }

        #region Read

        public override int ReadByte() => _stream.ReadByte();
        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);
        public override int EndRead(IAsyncResult asyncResult) => _stream.EndRead(asyncResult);

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
            _stream.BeginRead(buffer, offset, count, callback, state);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            _stream.ReadAsync(buffer, offset, count, cancellationToken);

        #endregion

        #region Write

        public override void WriteByte(byte value) => _stream.WriteByte(value);
        public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);
        public override void EndWrite(IAsyncResult asyncResult) => _stream.EndWrite(asyncResult);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
            _stream.BeginWrite(buffer, offset, count, callback, state);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            _stream.WriteAsync(buffer, offset, count, cancellationToken);

        #endregion

        public override Task FlushAsync(CancellationToken cancellationToken) => _stream.FlushAsync(cancellationToken);
        public override void Flush() => _stream.Flush();

        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
        public override void SetLength(long value) => _stream.SetLength(value);

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            _stream.CopyToAsync(destination, bufferSize, cancellationToken);

        public override void Close() => _stream.Close();

        protected override void Dispose(bool disposing)
        {
            void DisposeStream()
            {
                if (disposing && !_leaveOpen)
                    _stream?.Dispose();
            }

            try
            {
                OnDisposing?.Invoke(this, disposing);
                DisposeStream();
                OnDispose?.Invoke(this, disposing);
            }
            catch // the event invocation may throw
            {
                DisposeStream();
                throw;
            }

            base.Dispose(disposing);
        }
    }
}