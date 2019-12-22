using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Framework.IO;
using MonoGame.Framework.Memory;
using static StbSharp.StbImage;

namespace MonoGame.Imaging
{
    public class ImageReadStream : Stream
    {
        private Stream _stream;
        private CancellationTokenRegistration _cancellationRegistration;
        private byte[] _readBuffer;

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

        public StreamDisposalMethod DisposalMethod { get; }

        public ImageReadStream(
            Stream stream, CancellationToken cancellation, StreamDisposalMethod disposalType)
        {
            if (disposalType != StreamDisposalMethod.Close &&
                disposalType != StreamDisposalMethod.LeaveOpen &&
                disposalType != StreamDisposalMethod.CancellableClose)
                throw new ArgumentOutOfRangeException(nameof(disposalType));

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            DisposalMethod = disposalType;

            if (DisposalMethod == StreamDisposalMethod.CancellableClose && cancellation.CanBeCanceled)
                _cancellationRegistration = cancellation.Register(() => _stream?.Dispose());

            _readBuffer = RecyclableMemoryManager.Default.GetBlock();
            Context = new ReadContext(
                _stream, _readBuffer, cancellation, ReadCallback, SkipCallback);
        }

        public static ImageReadStream Create(
            Stream stream, CancellationToken cancellation, StreamDisposalMethod disposalMethod) =>
            new ImageReadStream(stream, cancellation, disposalMethod);

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        #region IO Callbacks

        private static int SkipCallback(ReadContext context, int n)
        {
            CommonArgumentGuard.AssertAtleastZero(n, nameof(n), false);
            try
            {
                if (n == 0)
                    return 0;

                if (!context.Stream.CanSeek)
                {
                    int skipped = 0;
                    int left = n;
                    while (left > 0)
                    {
                        int count = Math.Min(left, context.ReadBuffer.Length);
                        int read = context.Stream.Read(context.ReadBuffer, 0, count);
                        if (read == 0)
                            break;

                        left -= read;
                        skipped += read;
                    }
                    return skipped;
                }
                else
                {
                    long current = context.Stream.Position;
                    long seeked = context.Stream.Seek(n, SeekOrigin.Current);
                    return (int)(seeked - current);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // TODO manage exception somehow
                return 0;
            }
        }

        private static unsafe int ReadCallback(ReadContext context, Span<byte> data)
        {
            try
            {
                if (data.IsEmpty)
                    return 0;

                byte[] buffer = context.ReadBuffer;
                int total = 0;
                int left = data.Length;
                int read;
                while (left > 0 && (read = context.Stream.Read(buffer, 0, Math.Min(buffer.Length, left))) > 0)
                {
                    var src = buffer.AsSpan(0, read);
                    var dst = data.Slice(total);
                    src.CopyTo(dst);

                    left -= read;
                    total += read;
                }

                return total;
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
            if (_readBuffer != null)
            {
                RecyclableMemoryManager.Default.ReturnBlock(_readBuffer);
                _readBuffer = null;
            }

            try
            {
                if (disposing)
                {
                    if (DisposalMethod != StreamDisposalMethod.LeaveOpen &&
                        DisposalMethod != StreamDisposalMethod.CancellableLeaveOpen)
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