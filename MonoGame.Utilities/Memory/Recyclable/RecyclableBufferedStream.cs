using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static MonoGame.Framework.StreamHelpers;

namespace MonoGame.Framework.Memory
{
    // Copied from .NET Foundation, and Modified

    /// <summary>
    /// Adds a buffering layer where the underlying buffer is 
    /// a fixed size block from a <see cref="RecyclableMemoryManager"/>.
    /// </summary>
    public sealed class RecyclableBufferedStream : Stream
    {
        private RecyclableMemoryManager _manager;
        private Stream? _stream;           // Underlying stream.  Close sets _stream to null.
        private bool _leaveOpen;

        private byte[]? _bufferBlock;      // Shared read/write buffer.  Alloc on first use.
        private int _readPos;              // Read pointer within shared buffer.
        private int _readLen;              // Number of bytes read in buffer from _stream.
        private int _writePos;             // Write pointer within shared buffer.

        // The last successful Task returned from ReadAsync
        // (perf optimization for successive reads of the same size)
        // Removing a private default constructor is a breaking change for the DataDebugSerializer.
        // Because this ctor was here previously we need to keep it around.
        private Task<int>? _lastSyncCompletedReadTask;

        private SemaphoreSlim? _asyncActiveSemaphore;

        // _stream can be null when disposed. 
        // However we don't want to make UnderlyingStream nullable just for that scenario, 
        // since doing operations after dispose is invalid anyway.
        public Stream UnderlyingStream => _stream!;

        public int BufferSize => _bufferBlock?.Length ?? 0;

        public override bool CanRead => _stream != null && _stream.CanRead;

        public override bool CanWrite => _stream != null && _stream.CanWrite;

        public override bool CanSeek => _stream != null && _stream.CanSeek;

        public override long Length
        {
            get
            {
                EnsureNotClosed();

                if (_writePos > 0)
                    FlushWrite();

                return _stream!.Length;
            }
        }

        public override long Position
        {
            get
            {
                EnsureNotClosed();
                EnsureCanSeek();

                Debug.Assert(
                    !(_writePos > 0 && _readPos != _readLen),
                    "Read and Write buffers cannot both have data in them at the same time.");

                return _stream!.Position + (_readPos - _readLen + _writePos);
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedNonNegNum);

                EnsureNotClosed();
                EnsureCanSeek();

                if (_writePos > 0)
                    FlushWrite();

                _readPos = 0;
                _readLen = 0;
                _stream!.Seek(value, SeekOrigin.Begin);
            }
        }

        public RecyclableBufferedStream(RecyclableMemoryManager manager, Stream stream, bool leaveOpen)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _leaveOpen = leaveOpen;

            if (!_stream.CanRead && !_stream.CanWrite)
                throw new ArgumentException(
                    "The stream is not readable nor writable." ,nameof(stream));

            // Allocate _buffer on its first use - it will not be used if all reads
            // & writes are greater than or equal to buffer size.
        }

        internal SemaphoreSlim LazyEnsureAsyncActiveSemaphoreInitialized()
        {
            // Lazily-initialize _asyncActiveSemaphore.  
            // As we're never accessing the SemaphoreSlim's WaitHandle, 
            // we don't need to worry about Disposing it.
            return LazyInitializer.EnsureInitialized(
                ref _asyncActiveSemaphore, () => new SemaphoreSlim(1, 1));
        }

        private void EnsureNotClosed()
        {
            if (_stream == null)
                throw new ObjectDisposedException(
                    GetType().FullName, SR.ObjectDisposed_StreamClosed);
        }

        private void EnsureCanSeek()
        {
            Debug.Assert(_stream != null);

            if (!_stream.CanSeek)
                throw new NotSupportedException(SR.NotSupported_UnseekableStream);
        }

        private void EnsureCanRead()
        {
            Debug.Assert(_stream != null);

            if (!_stream.CanRead)
                throw new NotSupportedException(SR.NotSupported_UnreadableStream);
        }

        private void EnsureCanWrite()
        {
            Debug.Assert(_stream != null);

            if (!_stream.CanWrite)
                throw new NotSupportedException(SR.NotSupported_UnwritableStream);
        }

        private void EnsureBufferAllocated()
        {
            // BufferedStream is not intended for multi-threaded use,
            // so no worries about the get/set race on _buffer.
            if (_bufferBlock == null)
                _bufferBlock = _manager.GetBlock();
        }

        public override void Flush()
        {
            EnsureNotClosed();

            // Has write data in the buffer:
            if (_writePos > 0)
            {
                FlushWrite();
                Debug.Assert(_writePos == 0 && _readPos == 0 && _readLen == 0);
                return;
            }

            // Has read data in the buffer:
            if (_readPos < _readLen)
            {
                // If the underlying stream is not seekable AND we 
                // have something in the read buffer, then FlushRead would throw.
                // We can either throw away the buffer resulting in data loss (!) or ignore the Flush.
                // (We cannot throw because it would be a breaking change.) 
                // We opt into ignoring the Flush in that situation.
                if (_stream!.CanSeek)
                {
                    FlushRead();
                }

                // User streams may have opted to throw from Flush if CanWrite 
                // is false (although the abstract Stream does not do so).
                // However, if we do not forward the Flush to the underlying stream,
                // we may have problems when chaining several streams.
                // Let us make a best effort attempt:
                if (_stream.CanWrite)
                    _stream.Flush();

                // If the Stream was seekable, 
                // then we should have called FlushRead which resets _readPos & _readLen.
                Debug.Assert(_writePos == 0 && (!_stream.CanSeek || (_readPos == 0 && _readLen == 0)));
                return;
            }

            // We had no data in the buffer, but we still need to tell the underlying stream to flush.
            if (_stream!.CanWrite)
                _stream.Flush();

            _writePos = _readPos = _readLen = 0;
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            EnsureNotClosed();

            return FlushAsyncInternal(cancellationToken);
        }

        private async Task FlushAsyncInternal(CancellationToken cancellationToken)
        {
            Debug.Assert(_stream != null);

            SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
            await sem.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_writePos > 0)
                {
                    await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
                    Debug.Assert(_writePos == 0 && _readPos == 0 && _readLen == 0);
                    return;
                }

                if (_readPos < _readLen)
                {
                    // If the underlying stream is not seekable AND we
                    // have something in the read buffer, then FlushRead would throw.
                    // We can either throw away the buffer resulting in 
                    // data loss (!) or ignore the Flush. (We cannot throw because it
                    // would be a breaking change.) We opt into ignoring the Flush in that situation.
                    if (_stream.CanSeek)
                    {
                        FlushRead();  // not async; it uses Seek, but there's no SeekAsync
                    }

                    // User streams may have opted to throw from Flush if CanWrite is false 
                    // (although the abstract Stream does not do so).
                    // However, if we do not forward the Flush to the underlying stream, 
                    // we may have problems when chaining several streams.
                    // Let us make a best effort attempt:
                    if (_stream.CanWrite)
                        await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                    // If the Stream was seekable, 
                    // then we should have called FlushRead which resets _readPos & _readLen.
                    Debug.Assert(_writePos == 0 && (!_stream.CanSeek || (_readPos == 0 && _readLen == 0)));
                    return;
                }

                // We had no data in the buffer, but we still need to tell the underlying stream to flush.
                if (_stream.CanWrite)
                    await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                // There was nothing in the buffer:
                Debug.Assert(_writePos == 0 && _readPos == _readLen);

            }
            finally
            {
                sem.Release();
            }
        }

        // Reading is done in blocks, but someone could read 1 byte from the buffer then write.
        // At that point, the underlying stream's pointer is out of sync with this stream's position.
        // All write functions should call this function to ensure that the buffered data is not lost.
        private void FlushRead()
        {
            Debug.Assert(_stream != null);
            Debug.Assert(_writePos == 0, "BufferedStream: Write buffer must be empty in FlushRead!");

            if (_readPos - _readLen != 0)
                _stream.Seek(_readPos - _readLen, SeekOrigin.Current);

            _readPos = 0;
            _readLen = 0;
        }

        /// <summary>
        /// Called by Write methods to clear the Read Buffer
        /// </summary>
        private void ClearReadBufferBeforeWrite()
        {
            Debug.Assert(_stream != null);
            Debug.Assert(_readPos <= _readLen, "_readPos <= _readLen [" + _readPos + " <= " + _readLen + "]");

            // No read data in the buffer:
            if (_readPos == _readLen)
            {
                _readPos = _readLen = 0;
                return;
            }

            // Must have read data.
            Debug.Assert(_readPos < _readLen);

            // If the underlying stream cannot seek, FlushRead would end up throwing NotSupported.
            // However, since the user did not call a method that is intuitively expected to seek,
            // a better message is in order.
            // Ideally, we would throw an InvalidOperation here,
            // but for backward compat we have to stick with NotSupported.
            if (!_stream.CanSeek)
                throw new NotSupportedException(
                    SR.NotSupported_CannotWriteToBufferedStreamIfReadBufferCannotBeFlushed);

            FlushRead();
        }

        private void FlushWrite()
        {
            Debug.Assert(_stream != null);
            Debug.Assert(_readPos == 0 && _readLen == 0,
                "BufferedStream: Read buffer must be empty in FlushWrite!");

            Debug.Assert(_bufferBlock != null && BufferSize >= _writePos,
                "BufferedStream: Write buffer must be allocated and " +
                "write position must be in the bounds of the buffer in FlushWrite!");

            _stream.Write(_bufferBlock, 0, _writePos);
            _writePos = 0;
            _stream.Flush();
        }

        private async ValueTask FlushWriteAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(_stream != null);
            Debug.Assert(_readPos == 0 && _readLen == 0,
                "BufferedStream: Read buffer must be empty in FlushWrite!");
            Debug.Assert(_bufferBlock != null && BufferSize >= _writePos,
                "BufferedStream: Write buffer must be allocated and write " +
                "position must be in the bounds of the buffer in FlushWrite!");

            await _stream.WriteAsync(
                new ReadOnlyMemory<byte>(_bufferBlock, 0, _writePos), cancellationToken).ConfigureAwait(false);
            _writePos = 0;
            await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        private int ReadFromBuffer(byte[] array, int offset, int count)
        {
            int readbytes = _readLen - _readPos;
            Debug.Assert(readbytes >= 0);

            if (readbytes == 0)
                return 0;

            if (readbytes > count)
                readbytes = count;
            Buffer.BlockCopy(_bufferBlock!, _readPos, array, offset, readbytes);
            _readPos += readbytes;

            return readbytes;
        }

        private int ReadFromBuffer(Span<byte> destination)
        {
            int readbytes = Math.Min(_readLen - _readPos, destination.Length);
            Debug.Assert(readbytes >= 0);
            if (readbytes > 0)
            {
                new ReadOnlySpan<byte>(_bufferBlock, _readPos, readbytes).CopyTo(destination);
                _readPos += readbytes;
            }
            return readbytes;
        }

        private int ReadFromBuffer(byte[] array, int offset, int count, out Exception? error)
        {
            try
            {
                error = null;
                return ReadFromBuffer(array, offset, count);
            }
            catch (Exception ex)
            {
                error = ex;
                return 0;
            }
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            EnsureNotClosed();
            EnsureCanRead();
            Debug.Assert(_stream != null);

            int bytesFromBuffer = ReadFromBuffer(array, offset, count);

            // We may have read less than the number of bytes the user asked for, but that is part of the Stream Debug.

            // Reading again for more data may cause us to block if we're using a device with no clear end of file,
            // such as a serial port or pipe. If we blocked here and this code was used with redirected pipes for a
            // process's standard output, this can lead to deadlocks involving two processes.
            // BUT - this is a breaking change.
            // So: If we could not read all bytes the user asked for from the buffer, we will try once 
            // from the underlying stream thus ensuring the same blocking behaviour as 
            // if the underlying stream was not wrapped in this BufferedStream.
            if (bytesFromBuffer == count)
                return bytesFromBuffer;

            int alreadySatisfied = bytesFromBuffer;
            if (bytesFromBuffer > 0)
            {
                count -= bytesFromBuffer;
                offset += bytesFromBuffer;
            }

            // So the read buffer is empty.
            Debug.Assert(_readLen == _readPos);
            _readPos = _readLen = 0;

            // If there was anything in the write buffer, clear it.
            if (_writePos > 0)
                FlushWrite();

            // If the requested read is larger than buffer size, avoid the buffer and still use a single read:
            if (count >= BufferSize)
            {
                return _stream.Read(array, offset, count) + alreadySatisfied;
            }

            // Ok. We can fill the buffer:
            EnsureBufferAllocated();
            _readLen = _stream.Read(_bufferBlock!, 0, BufferSize);

            bytesFromBuffer = ReadFromBuffer(array, offset, count);

            // We may have read less than the number of bytes the user asked for, but that is part of the Stream Debug.
            // Reading again for more data may cause us to block if we're using a device with no clear end of stream,
            // such as a serial port or pipe. 
            // If we blocked here & this code was used with redirected pipes for a process's
            // standard output, this can lead to deadlocks involving two processes. 
            // Additionally, translating one read on the BufferedStream to more than one read on
            // the underlying Stream may defeat the whole purpose of buffering of the
            // underlying reads are significantly more expensive.

            return bytesFromBuffer + alreadySatisfied;
        }

        public override int Read(Span<byte> destination)
        {
            EnsureNotClosed();
            EnsureCanRead();
            Debug.Assert(_stream != null);

            // Try to read from the buffer.
            int bytesFromBuffer = ReadFromBuffer(destination);
            if (bytesFromBuffer == destination.Length)
            {
                // We got as many bytes as were asked for; we're done.
                return bytesFromBuffer;
            }

            // We didn't get as many bytes as were asked for from the buffer, so try filling the buffer once.

            if (bytesFromBuffer > 0)
            {
                destination = destination[bytesFromBuffer..];
            }

            // The read buffer must now be empty.
            Debug.Assert(_readLen == _readPos);
            _readPos = _readLen = 0;

            // If there was anything in the write buffer, clear it.
            if (_writePos > 0)
            {
                FlushWrite();
            }

            if (destination.Length >= BufferSize)
            {
                // If the requested read is larger than buffer size, avoid the buffer and just read
                // directly into the destination.
                return _stream.Read(destination) + bytesFromBuffer;
            }
            else
            {
                // Otherwise, fill the buffer, then read from that.
                EnsureBufferAllocated();
                _readLen = _stream.Read(_bufferBlock!, 0, BufferSize);
                return ReadFromBuffer(destination) + bytesFromBuffer;
            }
        }

        private Task<int> LastSyncCompletedReadTask(int val)
        {
            Task<int>? t = _lastSyncCompletedReadTask;
            Debug.Assert(t == null || t.IsCompletedSuccessfully);

            if (t != null && t.Result == val)
                return t;

            t = Task.FromResult(val);
            _lastSyncCompletedReadTask = t;
            return t;
        }

        public override Task<int> ReadAsync(
            byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            // Fast path check for cancellation already requested
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            EnsureNotClosed();
            EnsureCanRead();

            int bytesFromBuffer = 0;
            // Try to satisfy the request from the buffer synchronously.
            // But still need a sem-lock in case that another
            // Async IO Task accesses the buffer concurrently. 
            // If we fail to acquire the lock without waiting, make this an Async operation.
            SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
            Task semaphoreLockTask = sem.WaitAsync(cancellationToken);
            if (semaphoreLockTask.IsCompletedSuccessfully)
            {
                bool completeSynchronously = true;
                try
                {
                    bytesFromBuffer = ReadFromBuffer(buffer, offset, count, out Exception? error);

                    // If we satisfied enough data from the buffer, we can complete synchronously.
                    // Reading again for more data may cause us to block if we're using a device with 
                    // no clear end of file, such as a serial port or pipe.
                    // If we blocked here and this code was used with redirected pipes for a
                    // process's standard output, this can lead to deadlocks involving two processes.
                    // BUT - this is a breaking change.
                    // So: If we could not read all bytes the user asked for from the buffer, 
                    // we will try once from the underlying stream thus ensuring the same 
                    // blocking behaviour as if the underlying stream was not wrapped in this BufferedStream.
                    completeSynchronously = bytesFromBuffer == count || error != null;

                    if (completeSynchronously)
                    {

                        return (error == null)
                            ? LastSyncCompletedReadTask(bytesFromBuffer)
                            : Task.FromException<int>(error);
                    }
                }
                finally
                {
                    // if this is FALSE, we will be entering ReadFromUnderlyingStreamAsync and releasing there.
                    if (completeSynchronously)
                        sem.Release();
                }
            }

            // Delegate to the async implementation.
            return ReadFromUnderlyingStreamAsync(
                new Memory<byte>(buffer, offset + bytesFromBuffer, count - bytesFromBuffer),
                bytesFromBuffer, semaphoreLockTask, cancellationToken).AsTask();
        }

        public override ValueTask<int> ReadAsync(
            Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
            }

            EnsureNotClosed();
            EnsureCanRead();

            int bytesFromBuffer = 0;
            SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
            Task semaphoreLockTask = sem.WaitAsync(cancellationToken);
            if (semaphoreLockTask.IsCompletedSuccessfully)
            {
                bool completeSynchronously = true;
                try
                {
                    bytesFromBuffer = ReadFromBuffer(buffer.Span);
                    completeSynchronously = bytesFromBuffer == buffer.Length;
                    if (completeSynchronously)
                    {
                        // If we satisfied enough data from the buffer, we can complete synchronously.
                        return new ValueTask<int>(bytesFromBuffer);
                    }
                }
                finally
                {
                    if (completeSynchronously)
                    {
                        // if this is FALSE, we will be entering ReadFromUnderlyingStreamAsync and releasing there.
                        sem.Release();
                    }
                }
            }

            // Delegate to the async implementation.
            return ReadFromUnderlyingStreamAsync(
                buffer[bytesFromBuffer..], bytesFromBuffer, semaphoreLockTask, cancellationToken);
        }

        /// <summary>
        /// BufferedStream should be as thin a wrapper as possible. 
        /// We want ReadAsync to delegate to ReadAsync of the underlying _stream rather than calling the 
        /// base Stream which implements the one in terms of the other.
        /// This allows BufferedStream to affect the semantics of the stream it wraps as little as possible. 
        /// </summary>
        /// <returns>
        /// -2 if _bufferSize was set to 0 while waiting on the semaphore; otherwise num of bytes read.
        /// </returns>
        private async ValueTask<int> ReadFromUnderlyingStreamAsync(
            Memory<byte> buffer, int bytesAlreadySatisfied, Task semaphoreLockTask, CancellationToken cancellationToken)
        {
            // Same conditions validated with exceptions in ReadAsync:
            Debug.Assert(_stream != null);
            Debug.Assert(_stream.CanRead);
            Debug.Assert(BufferSize > 0);
            Debug.Assert(semaphoreLockTask != null);

            // Employ async waiting based on the same synchronization used in BeginRead of the abstract Stream.
            await semaphoreLockTask.ConfigureAwait(false);
            try
            {
                // The buffer might have been changed by another async task while we were waiting on the semaphore.
                // Check it now again.
                int bytesFromBuffer = ReadFromBuffer(buffer.Span);
                if (bytesFromBuffer == buffer.Length)
                {
                    return bytesAlreadySatisfied + bytesFromBuffer;
                }

                if (bytesFromBuffer > 0)
                {
                    buffer = buffer[bytesFromBuffer..];
                    bytesAlreadySatisfied += bytesFromBuffer;
                }

                Debug.Assert(_readLen == _readPos);
                _readPos = _readLen = 0;

                // If there was anything in the write buffer, clear it.
                if (_writePos > 0)
                {
                    // no Begin-End read version for Flush. Use Async.
                    await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
                }

                // If the requested read is larger than buffer size, avoid the buffer and still use a single read:
                if (buffer.Length >= BufferSize)
                {
                    return bytesAlreadySatisfied + await _stream.ReadAsync(
                        buffer, cancellationToken).ConfigureAwait(false);
                }

                // Ok. We can fill the buffer:
                EnsureBufferAllocated();
                _readLen = await _stream.ReadAsync(
                    new Memory<byte>(_bufferBlock, 0, BufferSize), cancellationToken).ConfigureAwait(false);

                bytesFromBuffer = ReadFromBuffer(buffer.Span);
                return bytesAlreadySatisfied + bytesFromBuffer;
            }
            finally
            {
                SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
                sem.Release();
            }
        }

        public override IAsyncResult BeginRead(
            byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return TaskToApm.Begin(ReadAsync(buffer, offset, count, CancellationToken.None), callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return TaskToApm.End<int>(asyncResult);
        }

        public override int ReadByte()
        {
            return _readPos != _readLen ?
                _bufferBlock![_readPos++] :
                ReadByteSlow();
        }

        private int ReadByteSlow()
        {
            Debug.Assert(_readPos == _readLen);

            // We want to check for whether the underlying stream has been closed and whether
            // it's readable, but we only need to do so if we don't have data in our buffer,
            // as any data we have came from reading it from an open stream, and we don't
            // care if the stream has been closed or become unreadable since. Further, if
            // the stream is closed, its read buffer is flushed, so we'll take this slow path.
            EnsureNotClosed();
            EnsureCanRead();
            Debug.Assert(_stream != null);

            if (_writePos > 0)
                FlushWrite();

            EnsureBufferAllocated();
            _readLen = _stream.Read(_bufferBlock!, 0, BufferSize);
            _readPos = 0;

            if (_readLen == 0)
                return -1;

            return _bufferBlock![_readPos++];
        }

        private void WriteToBuffer(byte[] array, ref int offset, ref int count)
        {
            int bytesToWrite = Math.Min(BufferSize - _writePos, count);

            if (bytesToWrite <= 0)
                return;

            EnsureBufferAllocated();
            Buffer.BlockCopy(array, offset, _bufferBlock!, _writePos, bytesToWrite);

            _writePos += bytesToWrite;
            count -= bytesToWrite;
            offset += bytesToWrite;
        }

        private int WriteToBuffer(ReadOnlySpan<byte> buffer)
        {
            int bytesToWrite = Math.Min(BufferSize - _writePos, buffer.Length);
            if (bytesToWrite > 0)
            {
                EnsureBufferAllocated();
                buffer.Slice(0, bytesToWrite).CopyTo(new Span<byte>(_bufferBlock, _writePos, bytesToWrite));
                _writePos += bytesToWrite;
            }
            return bytesToWrite;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            EnsureNotClosed();
            EnsureCanWrite();
            Debug.Assert(_stream != null);

            if (_writePos == 0)
                ClearReadBufferBeforeWrite();

            #region Write algorithm comment
            // We need to use the buffer, while avoiding unnecessary buffer usage / memory copies.
            // We ASSUME that memory copies are much cheaper than writes to the underlying stream, 
            // so if an extra copy is guaranteed to reduce the number of writes, we prefer it.
            // We pick a simple strategy that makes degenerate cases rare if our assumptions are right.
            //
            // For ever write, we use a simple heuristic (below) to decide whether to use the buffer.
            // The heuristic has the desirable property (*) that if the specified user data can fit into
            // the currently available buffer space without filling it up completely,
            // the heuristic will always tell us to use the buffer. 
            // It will also tell us to use the buffer in cases where the current write would fill the buffer,
            // but the remaining data is small enough such that subsequent operations can use the buffer again.
            //
            // Algorithm:
            // Determine whether or not to buffer according to the heuristic (below).
            // If we decided to use the buffer:
            //     Copy as much user data as we can into the buffer.
            //     If we consumed all data: We are finished.
            //     Otherwise, write the buffer out.
            //     Copy the rest of user data into the now cleared buffer 
            //     (no need to write out the buffer again as the heuristic will prevent it from being filled twice).
            // If we decided not to use the buffer:
            //     Write out any data possibly in the buffer.
            //     Write out user data directly.
            //
            // Heuristic:
            // If the subsequent write operation that follows the current write operation will result in a write to the
            // underlying stream in case that we use the buffer in the current write, while it would not have if we 
            // avoided using the buffer in the current write 
            // (by writing current user data to the underlying stream directly), 
            // then we prefer to avoid using the buffer since the corresponding memory copy is wasted 
            // (it will not reduce the number of writes to the underlying stream, which is what we are optimising for).
            // ASSUME that the next write will be for the same amount of bytes as the current write 
            // (most common case) and determine if it will cause a write to the underlying stream. 
            // If the next write is actually larger, our heuristic still yields the right behaviour, 
            // if the next write is actually smaller, we may making an unnecessary write to the underlying stream. 
            // However, this can only occur if the current write is larger than 
            // half the buffer size and we will recover after one iteration.
            // We have:
            //     useBuffer = (_writePos + count + count < _bufferSize + _bufferSize)
            //
            // Example with _bufferSize = 20, _writePos = 6, count = 10:
            //
            //     +---------------------------------------+---------------------------------------+
            //     |             current buffer            | next iteration's "future" buffer      |
            //     +---------------------------------------+---------------------------------------+
            //     |0| | | | | | | | | |1| | | | | | | | | |2| | | | | | | | | |3| | | | | | | | | |
            //     |0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|
            //     +-----------+-------------------+-------------------+---------------------------+
            //     | _writePos |  current count    | assumed next count|avail buff after next write|
            //     +-----------+-------------------+-------------------+---------------------------+
            //
            // A nice property (*) of this heuristic is that it will always succeed if the 
            // user data completely fits into the available buffer, i.e. if count < (_bufferSize - _writePos).

            #endregion Write algorithm comment

            Debug.Assert(_writePos < BufferSize);

            int totalUserbytes;
            bool useBuffer;
            checked
            {  // We do not expect buffer sizes big enough for an overflow, but if it happens, lets fail early:
                totalUserbytes = _writePos + count;
                useBuffer = totalUserbytes + count < (BufferSize + BufferSize);
            }

            if (useBuffer)
            {
                WriteToBuffer(array, ref offset, ref count);

                if (_writePos < BufferSize)
                {
                    Debug.Assert(count == 0);
                    return;
                }

                Debug.Assert(count >= 0);
                Debug.Assert(_writePos == BufferSize);
                Debug.Assert(_bufferBlock != null);

                _stream.Write(_bufferBlock, 0, _writePos);
                _writePos = 0;

                WriteToBuffer(array, ref offset, ref count);

                Debug.Assert(count == 0);
                Debug.Assert(_writePos < BufferSize);
            }
            else
            {
                // Write out the buffer if necessary.
                if (_writePos > 0)
                {
                    Debug.Assert(_bufferBlock != null);
                    Debug.Assert(totalUserbytes >= BufferSize);

                    _stream.Write(_bufferBlock, 0, _writePos);
                    _writePos = 0;
                }

                // Write out user data.
                _stream.Write(array, offset, count);
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            EnsureNotClosed();
            EnsureCanWrite();
            Debug.Assert(_stream != null);

            if (_writePos == 0)
            {
                ClearReadBufferBeforeWrite();
            }
            Debug.Assert(_writePos < BufferSize, $"Expected {_writePos} < {BufferSize}");

            int totalUserbytes;
            bool useBuffer;
            checked
            {
                // We do not expect buffer sizes big enough for an overflow, but if it happens, lets fail early:
                totalUserbytes = _writePos + buffer.Length;
                useBuffer = totalUserbytes + buffer.Length < (BufferSize + BufferSize);
            }

            if (useBuffer)
            {
                // Copy as much data to the buffer as will fit.  If there's still room in the buffer,
                // everything must have fit.
                int bytesWritten = WriteToBuffer(buffer);
                if (_writePos < BufferSize)
                {
                    Debug.Assert(bytesWritten == buffer.Length);
                    return;
                }
                buffer = buffer[bytesWritten..];

                Debug.Assert(_writePos == BufferSize);
                Debug.Assert(_bufferBlock != null);

                // Output the buffer to the underlying stream.
                _stream.Write(_bufferBlock, 0, _writePos);
                _writePos = 0;

                // Now write the remainder.  It must fit, as we're only on this path if that's true.
                bytesWritten = WriteToBuffer(buffer);
                Debug.Assert(bytesWritten == buffer.Length);

                Debug.Assert(_writePos < BufferSize);
            }
            else // skip the buffer
            {
                // Flush anything existing in the buffer.
                if (_writePos > 0)
                {
                    Debug.Assert(_bufferBlock != null);
                    Debug.Assert(totalUserbytes >= BufferSize);

                    _stream.Write(_bufferBlock, 0, _writePos);
                    _writePos = 0;
                }

                // Write out user data.
                _stream.Write(buffer);
            }
        }

        public override ValueTask WriteAsync(
            ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            // Fast path check for cancellation already requested
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask(Task.FromCanceled(cancellationToken));
            }

            EnsureNotClosed();
            EnsureCanWrite();

            // Try to satisfy the request from the buffer synchronously.
            SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
            Task semaphoreLockTask = sem.WaitAsync(cancellationToken);
            if (semaphoreLockTask.IsCompletedSuccessfully)
            {
                bool completeSynchronously = true;
                try
                {
                    if (_writePos == 0)
                    {
                        ClearReadBufferBeforeWrite();
                    }

                    Debug.Assert(_writePos < BufferSize);

                    // If the write completely fits into the buffer, we can complete synchronously:
                    completeSynchronously = buffer.Length < BufferSize - _writePos;
                    if (completeSynchronously)
                    {
                        int bytesWritten = WriteToBuffer(buffer.Span);
                        Debug.Assert(bytesWritten == buffer.Length);
                        return default;
                    }
                }
                finally
                {
                    // if this is FALSE, we will be entering WriteToUnderlyingStreamAsync and releasing there.
                    if (completeSynchronously)
                        sem.Release();
                }
            }

            // Delegate to the async implementation.
            return WriteToUnderlyingStreamAsync(buffer, semaphoreLockTask, cancellationToken);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            return WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        /// <summary>
        /// BufferedStream should be as thin a wrapper as possible. We want WriteAsync to delegate to
        /// WriteAsync of the underlying _stream rather than calling the base Stream which implements the one
        /// in terms of the other. This allows BufferedStream to affect the semantics of the stream it wraps as
        /// little as possible.
        /// </summary>
        private async ValueTask WriteToUnderlyingStreamAsync(
            ReadOnlyMemory<byte> buffer, Task semaphoreLockTask, CancellationToken cancellationToken)
        {
            Debug.Assert(_stream != null);
            Debug.Assert(_stream.CanWrite);
            Debug.Assert(BufferSize > 0);
            Debug.Assert(semaphoreLockTask != null);

            // See the LARGE COMMENT in Write(..) for the explanation of the write buffer algorithm.

            await semaphoreLockTask.ConfigureAwait(false);
            try
            {
                // The buffer might have been changed by another async task while we were waiting on the semaphore.
                // However, note that if we recalculate the sync completion condition
                // to TRUE, then useBuffer will also be TRUE.

                if (_writePos == 0)
                    ClearReadBufferBeforeWrite();

                int totalUserBytes;
                bool useBuffer;
                checked
                {
                    // We do not expect buffer sizes big enough for an overflow, but if it happens, lets fail early:
                    totalUserBytes = _writePos + buffer.Length;
                    useBuffer = totalUserBytes + buffer.Length < (BufferSize + BufferSize);
                }

                if (useBuffer)
                {
                    buffer = buffer[WriteToBuffer(buffer.Span)..];

                    if (_writePos < BufferSize)
                    {
                        Debug.Assert(buffer.IsEmpty);
                        return;
                    }

                    Debug.Assert(buffer.Length >= 0);
                    Debug.Assert(_writePos == BufferSize);
                    Debug.Assert(_bufferBlock != null);

                    await _stream.WriteAsync(
                        new ReadOnlyMemory<byte>(_bufferBlock, 0, _writePos), cancellationToken).ConfigureAwait(false);
                    _writePos = 0;

                    int bytesWritten = WriteToBuffer(buffer.Span);
                    Debug.Assert(bytesWritten == buffer.Length);

                    Debug.Assert(_writePos < BufferSize);

                }
                else
                {
                    // Write out the buffer if necessary.
                    if (_writePos > 0)
                    {
                        Debug.Assert(_bufferBlock != null);
                        Debug.Assert(totalUserBytes >= BufferSize);

                        await _stream.WriteAsync(
                            new ReadOnlyMemory<byte>(_bufferBlock, 0, _writePos), cancellationToken).ConfigureAwait(false);
                        _writePos = 0;
                    }

                    // Write out user data.
                    await _stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
                sem.Release();
            }
        }

        public override IAsyncResult BeginWrite(
            byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return TaskToApm.Begin(WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            TaskToApm.End(asyncResult);
        }

        public override void WriteByte(byte value)
        {
            EnsureNotClosed();

            if (_writePos == 0)
            {
                EnsureCanWrite();
                ClearReadBufferBeforeWrite();
                EnsureBufferAllocated();
            }

            // We should not be flushing here, but only writing to the underlying stream,
            // but previous version flushed, so we keep this.
            if (_writePos >= BufferSize - 1)
                FlushWrite();

            _bufferBlock![_writePos++] = value;

            Debug.Assert(_writePos < BufferSize);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            EnsureNotClosed();
            EnsureCanSeek();
            Debug.Assert(_stream != null);

            // If we have bytes in the write buffer, flush them out, seek and be done.
            if (_writePos > 0)
            {
                // We should be only writing the buffer and not flushing,
                // but the previous version did flush and we stick to it for back-compat reasons.
                FlushWrite();
                return _stream.Seek(offset, origin);
            }

            // The buffer is either empty or we have a buffered read.

            if (_readLen - _readPos > 0 && origin == SeekOrigin.Current)
            {
                // If we have bytes in the read buffer, 
                // adjust the seek offset to account for the resulting difference
                // between this stream's position and the underlying stream's position.
                offset -= _readLen - _readPos;
            }

            long oldPos = Position;
            Debug.Assert(oldPos == _stream.Position + (_readPos - _readLen));

            long newPos = _stream.Seek(offset, origin);

            // If the seek destination is still within the data currently in the buffer, 
            // we want to keep the buffer data and continue using it.
            // Otherwise we will throw away the buffer. 
            // This can only happen on read, as we flushed write data above.

            // The offset of the new/updated seek pointer within _buffer:
            _readPos = (int)(newPos - (oldPos - _readPos));

            // If the offset of the updated seek pointer in the buffer is still legal,
            // then we can keep using the buffer:
            if (0 <= _readPos && _readPos < _readLen)
            {
                // Adjust the seek pointer of the underlying stream to 
                // reflect the amount of useful bytes in the read buffer:
                _stream.Seek(_readLen - _readPos, SeekOrigin.Current);
            }
            else
            {  // The offset of the updated seek pointer is not a legal offset. Loose the buffer.
                _readPos = _readLen = 0;
            }

            Debug.Assert(newPos == Position, "newPos (=" + newPos + ") == Position (=" + Position + ")");
            return newPos;
        }

        public override void SetLength(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedNonNegNum);

            EnsureNotClosed();
            EnsureCanSeek();
            EnsureCanWrite();
            Debug.Assert(_stream != null);

            Flush();
            _stream.SetLength(value);
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            ValidateCopyToArgs(this, destination, bufferSize);
            Debug.Assert(_stream != null);

            int readBytes = _readLen - _readPos;
            Debug.Assert(readBytes >= 0, $"Expected a non-negative number of bytes in buffer, got {readBytes}");

            if (readBytes > 0)
            {
                // If there's any read data in the buffer, write it all to the destination stream.
                Debug.Assert(_writePos == 0, "Write buffer must be empty if there's data in the read buffer");
                destination.Write(_bufferBlock!, _readPos, readBytes);
                _readPos = _readLen = 0;
            }
            else if (_writePos > 0)
            {
                // If there's write data in the buffer, flush it back to the underlying stream, as does ReadAsync.
                FlushWrite();
            }

            // Our buffer is now clear. Copy data directly from the source stream to the destination stream.
            _stream.CopyTo(destination, bufferSize);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            ValidateCopyToArgs(this, destination, bufferSize);

            return cancellationToken.IsCancellationRequested ?
                Task.FromCanceled<int>(cancellationToken) :
                CopyToAsyncCore(destination, bufferSize, cancellationToken);
        }

        private async Task CopyToAsyncCore(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            Debug.Assert(_stream != null);

            // Synchronize async operations as does Read/WriteAsync.
            await LazyEnsureAsyncActiveSemaphoreInitialized().WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                int readBytes = _readLen - _readPos;
                Debug.Assert(readBytes >= 0, $"Expected a non-negative number of bytes in buffer, got {readBytes}");

                if (readBytes > 0)
                {
                    // If there's any read data in the buffer, write it all to the destination stream.
                    Debug.Assert(_writePos == 0, "Write buffer must be empty if there's data in the read buffer");
                    await destination.WriteAsync(
                        new ReadOnlyMemory<byte>(_bufferBlock, _readPos, readBytes), cancellationToken).ConfigureAwait(false);
                    _readPos = _readLen = 0;
                }
                else if (_writePos > 0)
                {
                    // If there's write data in the buffer, flush it back to the underlying stream, as does ReadAsync.
                    await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
                }

                // Our buffer is now clear. Copy data directly from the source stream to the destination stream.
                await _stream.CopyToAsync(destination, bufferSize, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _asyncActiveSemaphore!.Release();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _stream != null)
                {
                    try
                    {
                        Flush();
                    }
                    finally
                    {
                        if (!_leaveOpen)
                            _stream.Dispose();
                    }
                }
            }
            finally
            {
                _stream = null;
                _manager.ReturnBlock(_bufferBlock);

                base.Dispose(disposing);
            }
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                if (_stream != null)
                {
                    try
                    {
                        await FlushAsync().ConfigureAwait(false);
                    }
                    finally
                    {
                        if (!_leaveOpen)
                            await _stream.DisposeAsync().ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                _stream = null;
                _manager.ReturnBlock(_bufferBlock);

                await base.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}