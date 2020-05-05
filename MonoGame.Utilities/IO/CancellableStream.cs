using System;
using System.IO;
using System.Threading;

namespace MonoGame.Framework.IO
{
    public class CancellableStream : Stream
    {
        protected Stream InnerStream { get; private set; }
        protected CancellationTokenRegistration CancellationRegistration { get; private set; }

        public StreamDisposeMethod DisposeMethod { get; }
        public CancellationToken CancellationToken { get; }

        public override bool CanSeek => false;
        public override bool CanRead => InnerStream.CanRead;
        public override bool CanWrite => false;

        public override bool CanTimeout => InnerStream.CanTimeout;
        public override int ReadTimeout { get => InnerStream.ReadTimeout; set => InnerStream.ReadTimeout = value; }
        public override int WriteTimeout { get => InnerStream.WriteTimeout; set => InnerStream.WriteTimeout = value; }

        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public CancellableStream(
            Stream stream, CancellationToken cancellationToken, StreamDisposeMethod disposeMethod)
        {
            if (disposeMethod != StreamDisposeMethod.Close &&
                disposeMethod != StreamDisposeMethod.LeaveOpen &&
                disposeMethod != StreamDisposeMethod.CancellableLeaveOpen)
                throw new ArgumentOutOfRangeException(nameof(disposeMethod));

            InnerStream = stream ?? throw new ArgumentNullException(nameof(stream));
            CancellationToken = cancellationToken;
            DisposeMethod = disposeMethod;

            if (disposeMethod != StreamDisposeMethod.LeaveOpen && CancellationToken.CanBeCanceled)
                CancellationRegistration = CancellationToken.Register(() => Dispose());
        }

        public override int Read(byte[] buffer, int offset, int count) => InnerStream.Read(buffer, offset, count);

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
                        InnerStream?.Dispose();
                    InnerStream = null;
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
