using System;
using System.IO;

namespace MonoGame.Utilities.Memory
{
    /// <summary>
    /// The underlying buffer is a block from a <see cref="RecyclableMemoryManager"/> 
    /// and therefore the buffer size is fixed.
    /// </summary>
    public sealed class RecyclableBufferedStream : Stream
    {
        #region Private Fields

        private RecyclableMemoryManager _manager;
        private Stream _stream;
        private bool _leaveOpen;

        /// <summary>Shared buffer that's only allocated when needed.</summary>
        private byte[] _buffer;

        private int _readLength;
        private int _readPosition;
        private int _writePosition;

        #endregion

        private int BufferSize => _manager.BlockSize;

        #region Public Properties

        public override bool CanRead => _stream != null && _stream.CanRead;
        public override bool CanWrite => _stream != null && _stream.CanWrite;
        public override bool CanSeek => _stream != null && _stream.CanSeek;

        public override long Length
        {
            get
            {
                AssertNotClosed();
                if (_writePosition > 0)
                    FlushWrite();
                return _stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                AssertNotClosed();
                AssertCanSeek();
                return _stream.Position + (_readPosition - _readLength + _writePosition);
            }
            set
            {
                CommonArgumentGuard.AssertAtleastZero(value, nameof(value));
                AssertNotClosed();
                AssertCanSeek();

                if (_writePosition > 0)
                    FlushWrite();

                _readPosition = 0;
                _readLength = 0;
                _stream.Seek(value, SeekOrigin.Begin);
            }
        }

        #endregion

        public RecyclableBufferedStream(RecyclableMemoryManager manager, Stream stream, bool leaveOpen)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;

            if (!_stream.CanRead && !_stream.CanWrite)
                throw new ArgumentException("The stream is neither readable or writable.");
        }

        #region Assert

        private void AssertNotClosed()
        {
            if (_stream == null)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void AssertCanSeek()
        {
            if (!_stream.CanSeek)
                throw new NotSupportedException("The stream does not support seeking.");
        }

        private void AssertCanRead()
        {
            if (!_stream.CanRead)
                throw new NotSupportedException("The stream does not support reading.");
        }

        private void AssertCanWrite()
        {
            if (!_stream.CanWrite)
                throw new NotSupportedException("The stream does not support writing.");
        }

        private void AssertValidOffset(int arrayLength, int offset, int count)
        {
            if (arrayLength - offset < count)
                throw new ArgumentException(
                    "The sum of offset and count is greater than the buffer length.");
        }

        #endregion

        #region Flush

        public override void Flush()
        {
            AssertNotClosed();

            // buffer has write data
            if (_writePosition > 0)
            {
                FlushWrite();
                return;
            }

            void SafeFlush()
            {
                if (_stream.CanWrite || _stream is BufferedStream || _stream is RecyclableBufferedStream)
                    _stream.Flush();
            }

            // buffer has read data
            if (_readPosition < _readLength)
            {
                if (!_stream.CanSeek)
                    throw new InvalidOperationException(
                        "The underlying stream is not seekable but the buffer has read data which would result in data loss.");

                FlushRead();
                SafeFlush();
                return;
            }

            SafeFlush(); // the underlying stream needs to be flushed regardless
            _writePosition = _readPosition = _readLength = 0;
        }

        /// <summary>
        /// Write methods should call this to ensure that buffered data is not lost.
        /// </summary>
        private void FlushRead()
        {
            if (_readPosition - _readLength != 0)
                _stream.Seek(_readPosition - _readLength, SeekOrigin.Current);

            _readPosition = 0;
            _readLength = 0;
        }

        /// <summary>
        /// This is called by write methods to clear the read buffer. 
        /// </summary>
        private void ClearReadBufferBeforeWrite()
        {           
            // no read data in the buffer:
            if (_readPosition == _readLength)
            {
                _readPosition = _readLength = 0;
                return;
            }

            // FlushRead throws if the underlying stream cannot seek so
            // use a better message that doesn't mention seeking
            if (!_stream.CanSeek)
                throw new InvalidOperationException(
                    "Cannot write to buffered stream if the read buffer cannot be flushed.");

            FlushRead();
        }

        private void FlushWrite()
        {
            _stream.Write(_buffer, 0, _writePosition);
            _writePosition = 0;
            _stream.Flush();
        }

        #endregion

        #region Read

        private int ReadFromBuffer(byte[] array, int offset, int count)
        {
            int readBytes = _readLength - _readPosition;
            if (readBytes == 0)
                return 0;

            if (readBytes > count)
                readBytes = count;

            Buffer.BlockCopy(_buffer, _readPosition, array, offset, readBytes);
            _readPosition += readBytes;
            return readBytes;
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            CommonArgumentGuard.AssertAtleastZero(offset, nameof(offset));
            CommonArgumentGuard.AssertAtleastZero(count, nameof(count));
            AssertValidOffset(array.Length, offset, count);

            AssertNotClosed();
            AssertCanRead();

            int bytesFromBuffer = ReadFromBuffer(array, offset, count);
            if (bytesFromBuffer == count)
                return bytesFromBuffer;

            int alreadySatisfied = bytesFromBuffer;
            if (bytesFromBuffer > 0)
            {
                count -= bytesFromBuffer;
                offset += bytesFromBuffer;
            }

            _readPosition = 0;
            _readLength = 0;

            if (_writePosition > 0)
                FlushWrite();

            // if the request is larger than buffer then
            // avoid the buffer and use a single read
            if (count >= BufferSize)
                return _stream.Read(array, offset, count) + alreadySatisfied;
            
            AllocateBufferIfNeeded();
            _readLength = _stream.Read(_buffer, 0, BufferSize);

            bytesFromBuffer = ReadFromBuffer(array, offset, count);
            return bytesFromBuffer + alreadySatisfied;
        }

        public override int ReadByte()
        {
            AssertNotClosed();
            AssertCanRead();

            if (_readPosition == _readLength)
            {
                if (_writePosition > 0)
                    FlushWrite();

                AllocateBufferIfNeeded();
                _readLength = _stream.Read(_buffer, 0, BufferSize);
                _readPosition = 0;
            }

            if (_readPosition == _readLength)
                return -1;
            return _buffer[_readPosition++];
        }

        #endregion

        #region Write

        private void WriteToBuffer(byte[] array, ref int offset, ref int count)
        {
            int bytesToWrite = Math.Min(BufferSize - _writePosition, count);
            if (bytesToWrite <= 0)
                return;

            AllocateBufferIfNeeded();
            Buffer.BlockCopy(array, offset, _buffer, _writePosition, bytesToWrite);

            _writePosition += bytesToWrite;
            count -= bytesToWrite;
            offset += bytesToWrite;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            CommonArgumentGuard.AssertAtleastZero(offset, nameof(offset));
            CommonArgumentGuard.AssertAtleastZero(count, nameof(count));
            AssertValidOffset(array.Length, offset, count);

            AssertNotClosed();
            AssertCanWrite();

            if (_writePosition == 0)
                ClearReadBufferBeforeWrite();

            int totalUserBytes;
            bool useBuffer;

            // fail early if buffer size overflows
            checked
            {
                totalUserBytes = _writePosition + count;
                useBuffer = totalUserBytes + count < BufferSize;
            }

            if (useBuffer)
            {
                WriteToBuffer(array, ref offset, ref count);
                if (_writePosition < BufferSize)
                    return;

                _stream.Write(_buffer, 0, _writePosition);
                _writePosition = 0;
                WriteToBuffer(array, ref offset, ref count);
            }
            else
            {
                if (_writePosition > 0)
                {
                    _stream.Write(_buffer, 0, _writePosition);
                    _writePosition = 0;
                }
                _stream.Write(array, offset, count);
            }
        }

        public override void WriteByte(byte value)
        {
            AssertNotClosed();

            if (_writePosition == 0)
            {
                AssertCanWrite();
                ClearReadBufferBeforeWrite();
                AllocateBufferIfNeeded();
            }

            // We should not be flushing here, but only writing to the underlying stream,
            // but previous version flushed, so we keep this.
            if (_writePosition >= BufferSize - 1)
                FlushWrite();

            _buffer[_writePosition++] = value;
        }

        #endregion

        #region Seek

        public override long Seek(long offset, SeekOrigin origin)
        {
            AssertNotClosed();
            AssertCanSeek();

            if (_writePosition > 0)
            {
                FlushWrite();
                return _stream.Seek(offset, origin);
            }

            if (_readLength - _readPosition > 0 && origin == SeekOrigin.Current)
            {
                // adjust position if there are bytes in the read buffer        
                offset -= _readLength - _readPosition;
            }

            // Only keep the buffered data if the seek is withing the buffer.
            // (this can only happen on read data or as write data was flushed above)
            long oldPos = Position;
            long newPos = _stream.Seek(offset, origin);
            _readPosition = (int)(newPos - (oldPos - _readPosition));

            // keep using the buffer if the offset is valid
            if (0 <= _readPosition && _readPosition < _readLength)
            {
                // adjust to reflect the amount of useful bytes in the read buffer
                _stream.Seek(_readLength - _readPosition, SeekOrigin.Current);
            }
            else
            {
                // invalid offset so drop buffered data
                _readPosition = 0;
                _readLength = 0;
            }
            return newPos;
        }

        #endregion

        public override void SetLength(long value)
        {
            CommonArgumentGuard.AssertAtleastZero(value, nameof(value));

            AssertNotClosed();
            AssertCanSeek();
            AssertCanWrite();

            Flush();
            _stream.SetLength(value);
        }

        private void AllocateBufferIfNeeded()
        {
            if (_buffer == null)
                _buffer = _manager.GetBlock();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_stream != null && disposing)
                    Flush();
            }
            finally
            {
                if (_buffer != null)
                    _manager?.ReturnBlock(_buffer);
                _buffer = null;
                _manager = null;

                if (!_leaveOpen)
                    _stream?.Dispose();
                _stream = null;

                base.Dispose(disposing);
            }
        }
    }
}