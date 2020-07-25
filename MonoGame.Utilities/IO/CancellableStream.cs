using System;
using System.IO;
using System.Threading;

namespace MonoGame.Framework.IO
{
    public class CancellableStream : Stream
    {
        protected Stream UnderlyingStream { get; private set; }
        protected CancellationTokenRegistration CancellationRegistration { get; private set; }

        public StreamDisposeMethod DisposeMethod { get; }
        public CancellationToken CancellationToken { get; }

        public override bool CanSeek => false;
        public override bool CanRead => UnderlyingStream.CanRead;
        public override bool CanWrite => false;

        public override bool CanTimeout => UnderlyingStream.CanTimeout;
        public override int ReadTimeout { get => UnderlyingStream.ReadTimeout; set => UnderlyingStream.ReadTimeout = value; }
        public override int WriteTimeout { get => UnderlyingStream.WriteTimeout; set => UnderlyingStream.WriteTimeout = value; }

        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public CancellableStream(
            Stream stream, StreamDisposeMethod disposeMethod, CancellationToken cancellationToken)
        {
            if (disposeMethod != StreamDisposeMethod.Close &&
                disposeMethod != StreamDisposeMethod.LeaveOpen &&
                disposeMethod != StreamDisposeMethod.CancellableLeaveOpen)
                throw new ArgumentOutOfRangeException(nameof(disposeMethod));

            UnderlyingStream = stream ?? throw new ArgumentNullException(nameof(stream));
            DisposeMethod = disposeMethod;
            CancellationToken = cancellationToken;

            if (disposeMethod != StreamDisposeMethod.LeaveOpen && CancellationToken.CanBeCanceled)
                CancellationRegistration = CancellationToken.Register(() => Dispose());
        }

        public override int Read(byte[] buffer, int offset, int count) => UnderlyingStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (DisposeMethod == StreamDisposeMethod.Close)
                        UnderlyingStream?.Dispose();
                }
            }
            finally
            {
                CancellationRegistration.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
