using System;
using System.IO;

namespace MonoGame.Utilities.IO
{
    public class RecyclableReadBufferedStream : Stream
    {
        private bool _leaveOpen;
        private Stream _stream;
        private RecyclableMemoryManager _manager;

        private bool _disposed;
        private byte[] _buffer;
        private int _readPos;
        private int _readLen;

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _stream.Length;

        public override long Position
        {
            get => _stream.Position + _readPos - _readLen;
            set
            {
                _readPos = 0;
                _readLen = 0;
                Seek(value, SeekOrigin.Begin);
            }
        }

        internal RecyclableReadBufferedStream(
            Stream stream, bool leaveOpen, RecyclableMemoryManager manager)
        {
            _stream = stream;
            _leaveOpen = leaveOpen;
            _manager = manager;

            _buffer = _manager.GetBlock();
        }

        private int ReadFromBuffer(byte[] array, int offset, int count)
        {
            int readBytes = _readLen - _readPos;
            if (readBytes == 0)
                return 0;

            if (readBytes > count)
                readBytes = count;

            Buffer.BlockCopy(_buffer, _readPos, array, offset, readBytes);
            _readPos += readBytes;

            return readBytes;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesFromBuffer = ReadFromBuffer(buffer, offset, count);
            if (bytesFromBuffer == count)
                return bytesFromBuffer;

            int alreadySatisfied = bytesFromBuffer;
            if (bytesFromBuffer > 0)
            {   
                count -= bytesFromBuffer;
                offset += bytesFromBuffer;
            }

            _readPos = 0;
            _readLen = 0;
            
            if (count >= _buffer.Length)
                return _stream.Read(buffer, offset, count) + alreadySatisfied;
            
            _readLen = _stream.Read(_buffer, 0, _buffer.Length);
            bytesFromBuffer = ReadFromBuffer(buffer, offset, count);
            
            return bytesFromBuffer + alreadySatisfied;
        }

        public override int ReadByte()
        {
            if (_readPos == _readLen)
            {
                _readLen = _stream.Read(_buffer, 0, _buffer.Length);
                _readPos = 0;
            }

            if (_readPos == _readLen)
                return -1;
            
            return _buffer[_readPos++];
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Throw();
        }

        public override void WriteByte(byte value)
        {
            Throw();
        }

        private void Throw()
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

            _readPos = 0;
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _readPos = 0;
            _stream.SetLength(value);
        }

        public override void Flush()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
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
