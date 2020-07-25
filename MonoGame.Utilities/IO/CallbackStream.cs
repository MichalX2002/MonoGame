using System;
using System.IO;

namespace MonoGame.Framework.IO
{
    public sealed class CallbackStream : Stream
    {
        public delegate void OnReadDelegate(ReadOnlySpan<byte> data);
        public delegate void OnWriteDelegate(ReadOnlySpan<byte> data);

        private readonly bool _leaveOpen;
        private readonly OnReadDelegate? OnRead;
        private readonly OnWriteDelegate? OnWrite;

        private Stream UnderlyingStream { get; }

        public override bool CanRead => UnderlyingStream.CanRead;
        public override bool CanSeek => UnderlyingStream.CanSeek;
        public override bool CanWrite => UnderlyingStream.CanWrite;
        public override bool CanTimeout => UnderlyingStream.CanTimeout;
        public override long Length => UnderlyingStream.Length;
        public override long Position { get => UnderlyingStream.Position; set => UnderlyingStream.Position = value; }
        public override int ReadTimeout { get => UnderlyingStream.ReadTimeout; set => UnderlyingStream.ReadTimeout = value; }
        public override int WriteTimeout { get => UnderlyingStream.WriteTimeout; set => UnderlyingStream.WriteTimeout = value; }

        public CallbackStream(
            Stream innerStream, OnReadDelegate? onRead, OnWriteDelegate? onWrite, bool leaveOpen)
        {
            UnderlyingStream = innerStream;
            _leaveOpen = leaveOpen;

            OnRead = onRead;
            OnWrite = onWrite;
        }

        public override int Read(Span<byte> buffer)
        {
            int length = UnderlyingStream.Read(buffer);
            OnRead?.Invoke(buffer);
            return length;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            UnderlyingStream.Write(buffer);
            OnWrite?.Invoke(buffer);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Write(buffer.AsSpan(offset, count));
        }

        public override int ReadByte()
        {
            int b = UnderlyingStream.ReadByte();
            if (b != -1)
                OnWrite?.Invoke(stackalloc byte[] { (byte)b });
            return b;
        }

        public override void WriteByte(byte value)
        {
            UnderlyingStream.WriteByte(value);
            OnWrite?.Invoke(stackalloc byte[] { value });
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return UnderlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            UnderlyingStream.SetLength(value);
        }

        public override void Flush()
        {
            UnderlyingStream.Flush();
        }

        public override void Close()
        {
            UnderlyingStream.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_leaveOpen)
                    UnderlyingStream?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
