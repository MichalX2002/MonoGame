﻿// ---------------------------------------------------------------------
// Copyright (c) 2015-2016 Microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ---------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace MonoGame.Framework.Memory
{
    /// <summary>
    /// Manages pools of small and large byte arrays.
    /// </summary>
    /// <remarks>
    /// There are two pools managed in here. The small pool contains same-sized
    /// buffers that are handed to streams as they write more data.
    /// 
    /// For scenarios that need to call GetBuffer(), 
    /// the large pool contains buffers of various sizes,
    /// all multiples/exponentials of LargeBufferMultiple (1 MB by default). 
    /// They are split by size to avoid overly-wasteful buffer usage. 
    /// There should be far fewer 8 MB buffers than 1 MB buffers, for example.
    /// </remarks>
    public partial class RecyclableMemoryManager
    {
        public enum MemoryDiscardReason
        {
            TooLarge,
            EnoughFree
        }

        #region Delegates

        /// <summary>
        /// Delegate for handling large buffer discard reports.
        /// </summary>
        /// <param name="tag">The tag associated with the event.</param>
        /// <param name="reason">Reason the buffer was discarded.</param>
        public delegate void LargeBufferDiscardedEventHandler(string tag, MemoryDiscardReason reason);

        /// <summary>
        /// Delegate for handling reports of stream size when streams are allocated
        /// </summary>
        /// <param name="bytes">Bytes allocated.</param>
        public delegate void StreamLengthReportHandler(long bytes);

        /// <summary>
        /// Delegate for handling actions that will return a callstack 
        /// whenever <see cref="GenerateCallStacks"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="tag">The tag associated with the event.</param>
        /// <param name="callStack">The call stack of the allocation.</param>
        public delegate void ImportantActionHandler(string tag, string callStack);

        /// <summary>
        /// Delegate for handling actions that have an attached tag.
        /// </summary>
        /// <param name="tag">The tag associated with the event.</param>
        public delegate void TaggedActionHandler(string tag);

        /// <summary>
        /// Delegate for handling periodic reporting of memory use statistics.
        /// </summary>
        /// <param name="smallPoolInUseBytes">Bytes currently in use in the small pool.</param>
        /// <param name="smallPoolFreeBytes">Bytes currently free in the small pool.</param>
        /// <param name="largePoolInUseBytes">Bytes currently in use in the large pool.</param>
        /// <param name="largePoolFreeBytes">Bytes currently free in the large pool.</param>
        public delegate void UsageReportEventHandler(
            long smallPoolInUseBytes, long smallPoolFreeBytes, long largePoolInUseBytes, long largePoolFreeBytes);

        #endregion

        public const int DefaultBlockSize = 80 * 1024;
        public const int DefaultLargeBufferMultiple = 1024 * 1024;
        public const int DefaultMaximumBufferSize = 16 * 1024 * 1024;

        private static object _instanceInitMutex = new object();
        private static RecyclableMemoryManager _instance;

        public static RecyclableMemoryManager Default
        {
            get
            {
                lock (_instanceInitMutex)
                {
                    if (_instance == null)
                        _instance = new RecyclableMemoryManager();
                    return _instance;
                }
            }
        }

        #region Instance Fields

        private readonly long[] _largeBufferFreeSize;
        private readonly long[] _largeBufferInUseSize;

        /// <summary>
        /// pools[0] = 1x largeBufferMultiple buffers
        /// pools[1] = 2x largeBufferMultiple buffers
        /// pools[2] = 3x(multiple)/4x(exponential) largeBufferMultiple buffers
        /// etc., up to maximumBufferSize
        /// </summary>
        private readonly ConcurrentStack<byte[]>[] _largePools;
        private readonly ConcurrentStack<byte[]> _smallPool;

        private long _smallPoolFreeSize;
        private long _smallPoolInUseSize;

        #endregion

        #region Public Properties

        /// <summary>
        /// The size of each block. It must be set at creation and cannot be changed.
        /// </summary>
        public int BlockSize { get; }

        /// <summary>
        /// All buffers are multiples/exponentials of this number. It must be set at creation and cannot be changed.
        /// </summary>
        public int LargeBufferMultiple { get; }

        /// <summary>
        /// Use multiple large buffer allocation strategy. It must be set at creation and cannot be changed.
        /// </summary>
        public bool UseMultipleLargeBuffer => !UseExponentialLargeBuffer;

        /// <summary>
        /// Use exponential large buffer allocation strategy. It must be set at creation and cannot be changed.
        /// </summary>
        public bool UseExponentialLargeBuffer { get; }

        /// <summary>
        /// Gets the maximum buffer size of large buffers.
        /// </summary>
        /// <remarks>
        /// Any buffer that is returned to the pool that is larger than this will be ignored.
        /// </remarks>
        public int MaximumLargeBufferSize { get; }

        /// <summary>
        /// Number of bytes in small pool not currently in use
        /// </summary>
        public long SmallPoolFreeSize => _smallPoolFreeSize;

        /// <summary>
        /// Number of bytes currently in use by stream from the small pool
        /// </summary>
        public long SmallPoolInUseSize => _smallPoolInUseSize;

        /// <summary>
        /// Number of bytes in large pool not currently in use
        /// </summary>
        public long LargePoolFreeSize
        {
            get
            {
                long sum = 0;
                foreach (var item in _largeBufferFreeSize)
                    sum += item;
                return sum;
            }
        }

        /// <summary>
        /// Number of bytes currently in use by streams from the large pool
        /// </summary>
        public long LargePoolInUseSize
        {
            get
            {
                long sum = 0;
                foreach (var item in _largeBufferInUseSize)
                    sum += item;
                return sum;
            }
        }

        /// <summary>
        /// How many blocks are in the small pool
        /// </summary>
        public long SmallBlocksFree => _smallPool.Count;

        /// <summary>
        /// How many buffers are in the large pool
        /// </summary>
        public long LargeBuffersFree
        {
            get
            {
                long free = 0;
                foreach (var pool in _largePools)
                    free += pool.Count;
                return free;
            }
        }

        /// <summary>
        /// How many bytes of small free blocks to allow before we start dropping
        /// those returned to us.
        /// </summary>
        public long MaximumFreeSmallPoolBytes { get; set; }

        /// <summary>
        /// How many bytes of large free buffers to allow before we start dropping
        /// those returned to us.
        /// </summary>
        public long MaximumFreeLargePoolBytes { get; set; }

        /// <summary>
        /// Maximum stream capacity in bytes. Attempts to set a larger capacity will
        /// result in an exception.
        /// </summary>
        /// <remarks>A value of 0 indicates no limit.</remarks>
        public long MaximumStreamCapacity { get; set; }

        /// <summary>
        /// Whether to save callstacks for stream allocations. This can help in debugging.
        /// It should NEVER be turned on generally in production.
        /// </summary>
        public bool GenerateCallStacks { get; set; }

        /// <summary>
        /// Whether dirty buffers can be immediately returned to the buffer pool when GetBuffer() is called on
        /// a stream and creates a single large buffer, if this setting is enabled, the other blocks will be
        /// returned to the buffer pool immediately.
        /// </summary>
        public bool AggresiveBlockReturn { get; set; }

        /// <summary>
        /// Whether dirty buffers can be immediately returned to the buffer pool. E.g. when GetBuffer() is called on
        /// a stream and creates a single large buffer, if this setting is enabled, the other blocks will be returned
        /// to the buffer pool immediately.
        /// Note when enabling this setting that the user is responsible for ensuring that any buffer previously
        /// retrieved from a stream which is subsequently modified is not used after modification (as it may no longer
        /// be valid).
        /// </summary>
        public bool AggressiveLargeBufferReturn { get; set; }

        #endregion

        #region Public Events

        /// <summary>
        /// Triggered when a new block is created.
        /// </summary>
        public event Action BlockCreated;

        /// <summary>
        /// Triggered when a new block is created.
        /// </summary>
        public event TaggedActionHandler BlockDiscarded;

        /// <summary>
        /// Triggered when a new large buffer is created.
        /// </summary>
        public event ImportantActionHandler LargeBufferCreated;

        /// <summary>
        /// Triggered when a new stream is created.
        /// </summary>
        public event Action StreamCreated;

        /// <summary>
        /// Triggered when a stream is disposed.
        /// </summary>
        public event Action StreamDisposed;

        /// <summary>
        /// Triggered when a stream is finalized.
        /// </summary>
        public event Action StreamFinalized;

        /// <summary>
        /// Triggered when a stream is converted to an array.
        /// </summary>
        public event ImportantActionHandler StreamConvertedToArray;

        /// <summary>
        /// Triggered when a stream is finalized.
        /// </summary>
        public event StreamLengthReportHandler StreamLength;

        /// <summary>
        /// Triggered when a large buffer is discarded, along with the reason for the discard.
        /// </summary>
        public event LargeBufferDiscardedEventHandler LargeBufferDiscarded;

        /// <summary>
        /// Periodically triggered to report usage statistics.
        /// </summary>
        public event UsageReportEventHandler UsageReport;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the memory manager with the default block/buffer specifications.
        /// </summary>
        public RecyclableMemoryManager()
            : this(DefaultBlockSize, DefaultLargeBufferMultiple, DefaultMaximumBufferSize, false)
        {
        }

        /// <summary>
        /// Initializes the memory manager with the given block requiredSize.
        /// </summary>
        /// <param name="blockSize">Size of each block that is pooled. Must be > 0.</param>
        /// <param name="largeBufferMultiple">Each large buffer will be a multiple of this value.</param>
        /// <param name="maximumBufferSize">Buffers larger than this are not pooled</param>
        /// <exception cref="ArgumentOutOfRangeException">blockSize is not a positive number, or largeBufferMultiple is not a positive number, or maximumBufferSize is less than blockSize.</exception>
        /// <exception cref="ArgumentException">maximumBufferSize is not a multiple of largeBufferMultiple</exception>
        public RecyclableMemoryManager(int blockSize, int largeBufferMultiple, int maximumBufferSize)
            : this(blockSize, largeBufferMultiple, maximumBufferSize, false)
        {
        }

        /// <summary>
        /// Initializes the memory manager with the given block requiredSize.
        /// </summary>
        /// <param name="blockSize">Size of each block that is pooled. Must be > 0.</param>
        /// <param name="largeBufferMultiple">Each large buffer will be a multiple/exponential of this value.</param>
        /// <param name="maximumBufferSize">Buffers larger than this are not pooled</param>
        /// <param name="useExponentialLargeBuffer">Switch to exponential large buffer allocation strategy</param>
        /// <exception cref="ArgumentOutOfRangeException">blockSize is not a positive number, or largeBufferMultiple is not a positive number, or maximumBufferSize is less than blockSize.</exception>
        /// <exception cref="ArgumentException">maximumBufferSize is not a multiple/exponential of largeBufferMultiple</exception>
        public RecyclableMemoryManager(int blockSize, int largeBufferMultiple, int maximumBufferSize, bool useExponentialLargeBuffer)
        {
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(blockSize), blockSize, "blockSize must be a positive number");

            if (largeBufferMultiple <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(largeBufferMultiple), "largeBufferMultiple must be a positive number");

            if (maximumBufferSize < blockSize)
                throw new ArgumentOutOfRangeException(
                    nameof(maximumBufferSize), "maximumBufferSize must be at least blockSize");

            BlockSize = blockSize;
            LargeBufferMultiple = largeBufferMultiple;
            MaximumLargeBufferSize = maximumBufferSize;
            UseExponentialLargeBuffer = useExponentialLargeBuffer;
            AggresiveBlockReturn = true;

            if (!IsLargeBufferSize(maximumBufferSize))
                throw new ArgumentException(
                    string.Format("maximumBufferSize is not {0} of largeBufferMultiple",
                    UseExponentialLargeBuffer ? "an exponential" : "a multiple"), nameof(maximumBufferSize));

            _smallPool = new ConcurrentStack<byte[]>();
            int numLargePools = useExponentialLargeBuffer
                ? ((int)Math.Log(maximumBufferSize / largeBufferMultiple, 2) + 1)
                : (maximumBufferSize / largeBufferMultiple);

            // +1 to store size of bytes in use that are too large to be pooled
            _largeBufferInUseSize = new long[numLargePools + 1];
            _largeBufferFreeSize = new long[numLargePools];

            _largePools = new ConcurrentStack<byte[]>[numLargePools];
            for (var i = 0; i < _largePools.Length; ++i)
                _largePools[i] = new ConcurrentStack<byte[]>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes and returns a single block from the pool.
        /// </summary>
        /// <returns>A byte[] array</returns>
        public byte[] GetBlock()
        {
            if (!_smallPool.TryPop(out byte[] block))
            {
                // We'll add this back to the pool when the stream is disposed
                // (unless our free pool is too large)
                block = new byte[BlockSize];
                ReportBlockCreated();
            }
            else
                Interlocked.Add(ref _smallPoolFreeSize, -BlockSize);

            Interlocked.Add(ref _smallPoolInUseSize, BlockSize);
            return block;
        }

        /// <summary>
        /// Returns a buffer of arbitrary size from the large buffer pool. This buffer
        /// will be at least the requiredSize and always be a multiple/exponential of largeBufferMultiple.
        /// </summary>
        /// <param name="requiredSize">The minimum length of the buffer</param>
        /// <param name="tag">The tag of the stream returning this buffer, for logging if necessary.</param>
        /// <returns>A buffer of at least the required size.</returns>
        public byte[] GetLargeBuffer(long requiredSize, string tag)
        {
            requiredSize = RoundToLargeBufferSize(requiredSize);
            int poolIndex = GetPoolIndex(requiredSize);

            byte[] buffer;
            if (poolIndex < _largePools.Length)
            {
                if (!_largePools[poolIndex].TryPop(out buffer))
                {
                    buffer = new byte[requiredSize];
                    ReportLargeBufferCreated(tag);
                }
                else
                    Interlocked.Add(ref _largeBufferFreeSize[poolIndex], -buffer.Length);
            }
            else
            {
                // Buffer is too large to pool. They get a new buffer.

                // We still want to track the size, though, and we've reserved a slot
                // in the end of the inuse array for nonpooled bytes in use.
                poolIndex = _largeBufferInUseSize.Length - 1;

                // We still want to round up to reduce heap fragmentation.
                buffer = new byte[requiredSize];
                ReportLargeBufferCreated(tag);
            }

            Interlocked.Add(ref _largeBufferInUseSize[poolIndex], buffer.Length);
            return buffer;
        }

        /// <summary>
        /// Returns the buffer to the large pool
        /// </summary>
        /// <param name="buffer">The buffer to return.</param>
        /// <param name="tag">The tag of the stream returning this buffer, for logging if necessary.</param>
        /// <exception cref="ArgumentNullException">buffer is null</exception>
        /// <exception cref="ArgumentException">
        /// buffer.Length is not a multiple/exponential of LargeBufferMultiple (it did not originate from this pool)
        /// </exception>
        public void ReturnLargeBuffer(byte[] buffer, string tag)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Length > MaximumLargeBufferSize)
                return;

            if (!IsLargeBufferSize(buffer.Length))
                throw new ArgumentException(string.Format("The size is not {0} of {1}.",
                    UseExponentialLargeBuffer ? "an exponential" : "a multiple", LargeBufferMultiple));

            int poolIndex = GetPoolIndex(buffer.Length);
            if (poolIndex < _largePools.Length)
            {
                if ((_largePools[poolIndex].Count + 1) * buffer.Length <= MaximumFreeLargePoolBytes ||
                    MaximumFreeLargePoolBytes == 0)
                {
                    _largePools[poolIndex].Push(buffer);
                    Interlocked.Add(ref _largeBufferFreeSize[poolIndex], buffer.Length);
                }
                ReportLargeBufferDiscarded(tag, MemoryDiscardReason.EnoughFree);
            }
            else
            {
                // This is a non-poolable buffer, but we still want to track its size for inuse
                // analysis. We have space in the inuse array for this.
                poolIndex = _largeBufferInUseSize.Length - 1;
                ReportLargeBufferDiscarded(tag, MemoryDiscardReason.TooLarge);
            }

            Interlocked.Add(ref _largeBufferInUseSize[poolIndex], -buffer.Length);
            ReportUsageReport(_smallPoolInUseSize, _smallPoolFreeSize, LargePoolInUseSize, LargePoolFreeSize);
        }

        /// <summary>
        /// Returns a collection of blocks to the pool.
        /// </summary>
        /// <param name="blocks">Collection of blocks to return to the pool.</param>
        /// <param name="tag">The tag of the object returning these blocks, for logging if necessary.</param>
        /// <exception cref="ArgumentNullException">blocks is null.</exception>
        /// <exception cref="ArgumentException">blocks contains buffers that are the wrong size (or null) for this memory manager.</exception>
        public void ReturnBlocks(ICollection<byte[]> blocks, string tag)
        {
            if (blocks == null)
                throw new ArgumentNullException(nameof(blocks));

            void CheckBlock(byte[] block)
            {
                if (block == null || block.Length != BlockSize)
                    throw new ArgumentException("blocks contains buffers that are not BlockSize in length");
            }

            bool PushBlock(byte[] block)
            {
                if (MaximumFreeSmallPoolBytes == 0 || SmallPoolFreeSize < MaximumFreeSmallPoolBytes)
                {
                    Interlocked.Add(ref _smallPoolFreeSize, BlockSize);
                    _smallPool.Push(block);
                    return true;
                }
                ReportBlockDiscarded(tag);
                return false;
            }

            int bytesToReturn = blocks.Count * BlockSize;
            Interlocked.Add(ref _smallPoolInUseSize, -bytesToReturn);

            // small thing to reduce 2 IEnumerable+IEnumerator allocations in most cases
            if (blocks is List<byte[]> blockList)
            {
                foreach (byte[] block in blockList)
                    CheckBlock(block);
                foreach (byte[] block in blockList)
                    if (!PushBlock(block))
                        break;
            }
            else
            {
                foreach (byte[] block in blocks)
                    CheckBlock(block);
                foreach (byte[] block in blocks)
                    if (!PushBlock(block))
                        break;
            }

            ReportUsageReport(_smallPoolInUseSize, _smallPoolFreeSize, LargePoolInUseSize, LargePoolFreeSize);
        }

        /// <summary>
        /// Returns a block to the pool.
        /// </summary>
        /// <param name="block">The block to return to the pool.</param>
        /// <exception cref="ArgumentNullException">block is null.</exception>
        /// <exception cref="ArgumentException">block is the wrong size for this memory manager.</exception>
        public void ReturnBlock(byte[] block, string tag)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            if (block.Length != BlockSize)
                throw new ArgumentException("The block is the wrong size for this memory manager.");

            Interlocked.Add(ref _smallPoolInUseSize, -BlockSize);

            if (MaximumFreeSmallPoolBytes == 0 || SmallPoolFreeSize < MaximumFreeSmallPoolBytes)
            {
                Interlocked.Add(ref _smallPoolFreeSize, BlockSize);
                _smallPool.Push(block);
            }
            else
            {
                ReportBlockDiscarded(tag);
            }

            if (UsageReport != null)
                ReportUsageReport(_smallPoolInUseSize, _smallPoolFreeSize, LargePoolInUseSize, LargePoolFreeSize);
        }

        /// <summary>
        /// Returns a block to the pool.
        /// </summary>
        /// <param name="block">The block to return to the pool.</param>
        /// <exception cref="ArgumentNullException">block is null.</exception>
        /// <exception cref="ArgumentException">block is the wrong size for this memory manager.</exception>
        public void ReturnBlock(byte[] block) => ReturnBlock(block, null);

        #endregion

        #region Helpers

        [DebuggerHidden]
        private string GetCallStack()
        {
            if (GenerateCallStacks)
                return Environment.StackTrace;
            return null;
        }

        private int RoundToLargeBufferSize(long requiredSize)
        {
            if (UseExponentialLargeBuffer)
            {
                int pow = 1;
                while (LargeBufferMultiple * pow < requiredSize)
                    pow <<= 1;

                return LargeBufferMultiple * pow;
            }
            else
                return (int)((requiredSize + LargeBufferMultiple - 1) / LargeBufferMultiple * LargeBufferMultiple);
        }

        private bool IsLargeBufferSize(int value)
        {
            return (value != 0) && (UseExponentialLargeBuffer
                ? (value == RoundToLargeBufferSize(value))
                : (value % LargeBufferMultiple) == 0);
        }

        private int GetPoolIndex(long length)
        {
            if (UseExponentialLargeBuffer)
            {
                int index = 0;
                while ((LargeBufferMultiple << index) < length)
                    ++index;
                return index;
            }
            else
            {
                return (int)(length / LargeBufferMultiple - 1);
            }
        }

        #endregion

        #region Event Helpers

        internal void ReportBlockCreated() => BlockCreated?.Invoke();
        internal void ReportStreamCreated() => StreamCreated?.Invoke();
        internal void ReportStreamDisposed() => StreamDisposed?.Invoke();
        internal void ReportStreamFinalized() => StreamFinalized?.Invoke();

        internal void ReportBlockDiscarded(string tag) => BlockDiscarded?.Invoke(tag);

        // also grab the stack, we want to know who requires such large buffers
        internal void ReportLargeBufferCreated(string tag) => LargeBufferCreated?.Invoke(tag, GetCallStack());
        internal void ReportStreamToArray(string tag) => StreamConvertedToArray?.Invoke(tag, GetCallStack());

        internal void ReportStreamLength(long bytes) => StreamLength?.Invoke(bytes);

        internal void ReportLargeBufferDiscarded(string tag, MemoryDiscardReason reason)
        {
            LargeBufferDiscarded?.Invoke(tag, reason);
        }

        internal void ReportUsageReport(
            long smallPoolInUseBytes, long smallPoolFreeBytes, long largePoolInUseBytes, long largePoolFreeBytes)
        {
            UsageReport?.Invoke(smallPoolInUseBytes, smallPoolFreeBytes, largePoolInUseBytes, largePoolFreeBytes);
        }

        #endregion

        #region Stream Construction Helpers

        /// <summary>
        /// Constructs a new <see cref="RecyclableBufferedStream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="leaveOpen"></param>
        /// <returns>The <see cref="RecyclableBufferedStream"/>.</returns>
        public RecyclableBufferedStream GetBufferedStream(Stream stream, bool leaveOpen) =>
            new RecyclableBufferedStream(this, stream, leaveOpen);

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with no tag
        /// tag and a default initial capacity.
        /// </summary>
        /// <returns>The <see cref="RecyclableMemoryStream"/>.</returns>
        public RecyclableMemoryStream GetMemoryStream() => new RecyclableMemoryStream(this);

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with the given 
        /// tag and a default initial capacity.
        /// </summary>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <returns>The <see cref="RecyclableMemoryStream"/>.</returns>
        public RecyclableMemoryStream GetMemoryStream(string tag) => new RecyclableMemoryStream(this, tag);

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with at least the given capacity.
        /// </summary>
        /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
        /// <returns>The <see cref="RecyclableMemoryStream"/>.</returns>
        public RecyclableMemoryStream GetMemoryStream(long requiredSize) =>
            new RecyclableMemoryStream(this, null, requiredSize);

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with the given 
        /// tag and at least the given capacity.
        /// </summary>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
        /// <returns>The <see cref="RecyclableMemoryStream"/>.</returns>
        public RecyclableMemoryStream GetMemoryStream(string tag, long requiredSize) =>
            new RecyclableMemoryStream(this, tag, requiredSize);

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with the given tag 
        /// and at least the given capacity, possibly using a single continugous underlying buffer.
        /// </summary>
        /// <remarks>Retrieving a MemoryStream which provides a single contiguous buffer can be useful in situations
        /// where the initial size is known and it is desirable to avoid copying data between the smaller underlying
        /// buffers to a single large one. This is most helpful when you know that you will always call GetBuffer
        /// on the underlying stream.</remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
        /// <param name="asContiguousBuffer">Whether to attempt to use a single contiguous buffer.</param>
        /// <returns>A MemoryStream.</returns>
        public RecyclableMemoryStream GetMemoryStream(string tag, long requiredSize, bool asContiguousBuffer)
        {
            if (!asContiguousBuffer || requiredSize <= BlockSize)
                return GetMemoryStream(tag, requiredSize);
            return new RecyclableMemoryStream(this, tag, requiredSize, GetLargeBuffer(requiredSize, tag));
        }

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with the given 
        /// tag and with contents copied from the provided span.
        /// </summary>
        /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <returns>The <see cref="RecyclableMemoryStream"/>.</returns>
        public RecyclableMemoryStream GetMemoryStream(string tag, ReadOnlySpan<byte> data)
        {
            RecyclableMemoryStream stream = null;
            try
            {
                stream = new RecyclableMemoryStream(this, tag, data.Length);
                stream.Write(data);
                stream.Position = 0;
                return stream;
            }
            catch
            {
                stream?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with the given 
        /// tag and with contents copied from the provided buffer.
        /// The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <param name="offset">The offset from the start of the buffer to copy from.</param>
        /// <param name="count">The number of bytes to copy from the buffer.</param>
        /// <returns>The <see cref="RecyclableMemoryStream"/>.</returns>
        public RecyclableMemoryStream GetMemoryStream(string tag, byte[] buffer, int offset, int count) =>
            GetMemoryStream(tag, buffer.AsSpan(offset, count));

        /// <summary>
        /// Constructs a new <see cref="RecyclableMemoryStream"/> with the given 
        /// tag and with contents copied from the provided <see cref="Stream"/>.
        /// </summary>
        /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The <see cref="RecyclableMemoryStream"/>.</returns>
        public RecyclableMemoryStream GetMemoryStream(string tag, Stream stream)
        {
            RecyclableMemoryStream result = null;
            try
            {
                result = stream.CanSeek
                    ? GetMemoryStream(tag, stream.Length)
                    : GetMemoryStream(tag);

                stream.PooledCopyTo(result);
                result.Position = 0;
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        #endregion
    }
}