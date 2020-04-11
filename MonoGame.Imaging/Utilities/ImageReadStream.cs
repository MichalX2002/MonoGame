using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Framework.IO;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging
{
    public class ImageReadStream : Stream
    {
        private Stream _stream;
        private CancellationTokenRegistration _cancellationRegistration;

        public override bool CanSeek => false;
        public override bool CanRead => _stream.CanRead;
        public override bool CanWrite => false;

        public override bool CanTimeout => _stream.CanTimeout;
        public override int ReadTimeout { get => _stream.ReadTimeout; set => _stream.ReadTimeout = value; }
        public override int WriteTimeout { get => _stream.WriteTimeout; set => _stream.WriteTimeout = value; }

        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the imaging context that belongs the stream.
        /// </summary>
        public ReadContext Context { get; }

        public StreamDisposeMethod DisposalMethod { get; }

        public CancellationToken CancellationToken => Context.CancellationToken;

        public ImageReadStream(
            Stream stream, CancellationToken cancellationToken, StreamDisposeMethod disposeMethod)
        {
            if (disposeMethod != StreamDisposeMethod.Close &&
                disposeMethod != StreamDisposeMethod.LeaveOpen &&
                disposeMethod != StreamDisposeMethod.CancellableClose &&
                disposeMethod != StreamDisposeMethod.CancellableLeaveOpen)
                throw new ArgumentOutOfRangeException(nameof(disposeMethod));

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            DisposalMethod = disposeMethod;

            if (DisposalMethod == StreamDisposeMethod.CancellableClose && cancellationToken.CanBeCanceled)
                _cancellationRegistration = cancellationToken.Register(() => _stream?.Dispose());

            Context = new ReadContext(_stream, cancellationToken, ReadCallback, SkipCallback);
        }

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        #region IO Callbacks

        private static int SkipCallback(ReadContext context, int count)
        {
            ArgumentGuard.AssertAtleastZero(count, nameof(count), false);
            try
            {
                if (count == 0)
                    return 0;

                if (context.Stream.CanSeek)
                {
                    long previous = context.Stream.Position;
                    long current = context.Stream.Seek(count, SeekOrigin.Current);
                    return (int)(current - previous);
                }
                else
                {
                    Span<byte> buffer = stackalloc byte[1024];
                    int skipped = 0;
                    int left = count;
                    while (left > 0)
                    {
                        int toRead = Math.Min(left, buffer.Length);
                        int read = context.Stream.Read(buffer.Slice(0, toRead));
                        if (read == 0)
                            break;

                        left -= read;
                        skipped += read;
                    }
                    return skipped;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // TODO manage exception somehow
                return 0;
            }
        }

        private static int ReadCallback(ReadContext context, Span<byte> buffer)
        {
            try
            {
                return context.Stream.Read(buffer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // TODO manage exception somehow
                return 0;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (DisposalMethod != StreamDisposeMethod.LeaveOpen &&
                        DisposalMethod != StreamDisposeMethod.CancellableLeaveOpen)
                        _stream?.Dispose();
                }
            }
            finally
            {
                _cancellationRegistration.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}