using System;
using System.IO;

namespace MonoGame.Utilities.IO
{
    public class RecyclableWriteBufferedStream : Stream
    {
        private bool _leaveOpen;
        private Stream _stream;
        private RecyclableMemoryManager _manager;

        private bool _disposed;
        private byte[] _buffer;
        private int _writePos;

        public override bool CanRead => false;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;

        public override long Length
        {
            get
            {
                TryFlushBuffer();
                return _stream.Length;
            }
        }

        public override long Position
        {
            get => _stream.Position + _writePos;
            set
            {
                TryFlushBuffer();
                Seek(value, SeekOrigin.Begin);
            }
        }

        internal RecyclableWriteBufferedStream(
            Stream stream, bool leaveOpen, RecyclableMemoryManager manager)
        {
            _stream = stream;
            _leaveOpen = leaveOpen;
            _manager = manager;

            _buffer = _manager.GetBlock();
        }

        private void TryOptimalFlush()
        {
            if (_writePos == _buffer.Length)
                FlushBuffer();
        }

        private void WriteToBuffer(byte[] buffer, ref int offset, ref int count)
        {
            int bytesToWrite = Math.Min(_buffer.Length - _writePos, count);
            if (bytesToWrite <= 0)
                return;

            Buffer.BlockCopy(buffer, offset, _buffer, _writePos, bytesToWrite);

            _writePos += bytesToWrite;
            count -= bytesToWrite;
            offset += bytesToWrite;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if(count > _buffer.Length)
            {
                TryFlushBuffer();
                _stream.Write(buffer, offset, count);
            }
            else
            {
                while (count > 0)
                {
                    WriteToBuffer(buffer, ref offset, ref count);
                    TryOptimalFlush();
                }
            }
        }

        public override void WriteByte(byte value)
        {
            _buffer[_writePos++] = value;
            TryOptimalFlush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Throw();
        }

        public override int ReadByte()
        {
            return Throw();
        }

        private int Throw()
        {
            throw new NotSupportedException(
                nameof(RecyclableWriteBufferedStream) + " is only meant for buffered writing. Use " +
                nameof(RecyclableReadBufferedStream) + " for buffered reads.");
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new NotSupportedException(
                    "The underlying stream is currently not seekable.");

            TryFlushBuffer();
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            TryFlushBuffer();
            _stream.SetLength(value);
        }

        private void FlushBuffer()
        {
            _stream.Write(_buffer, 0, _writePos);
            _writePos = 0;
        }

        private void TryFlushBuffer()
        {
            if (_writePos > 0)
                FlushBuffer();
        }

        public override void Flush()
        {
            TryFlushBuffer();
            _stream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Flush();
                if (!_leaveOpen)
                    _stream.Dispose();
                _stream = null;

                _manager.ReturnBlock(_buffer);
                _buffer = null;

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
