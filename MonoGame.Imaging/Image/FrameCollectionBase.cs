using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public class FrameCollectionBase<TPixel, TFrame>
        where TPixel : unmanaged, IPixel
        where TFrame : ReadOnlyImageFrame<TPixel>
    {
        protected List<TFrame> _frames;
        protected int _version;

        public TFrame this[int imageIndex]
        {
            get => _frames[imageIndex];
            set
            {
                AssertValidFrame(value);
                _frames[imageIndex] = value;
            }
        }

        #region Constructors

        internal FrameCollectionBase(List<TFrame> frames, bool oneAsInitialCapacity)
        {
            _frames = frames ?? (oneAsInitialCapacity ? new List<TFrame>(1) : new List<TFrame>());
        }

        protected static bool UseOneAsInitialCapacity<T>(IEnumerable<T> enumerable)
        {
            if (enumerable is ICollection coll)
                return coll.Count > 1;
            else if (enumerable is ICollection<T> gColl)
                return gColl.Count > 1;
            else if (enumerable is IReadOnlyCollection<T> readOnlyColl)
                return readOnlyColl.Count > 1;
            return true;
        }

        #endregion

        #region Properties

        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the number of frames in this collection.
        /// </summary>
        public int Count => _frames.Count;

        public int Capacity
        {
            get => _frames.Capacity;
            set => _frames.Capacity = value;
        }

        /// <summary>
        /// Gets the first frame in the collection.
        /// Returns <see langword="default" /> if the collection is empty.
        /// </summary>
        public TFrame First => _frames.Count > 0 ? _frames[0] : default;

        /// <summary>
        /// Gets whether the frames in this collection are a looping sequence.
        /// <para>
        /// This is a simple value used by coders to state that an image is looping.
        /// </para>
        /// </summary>
        public bool IsLooping { get; set; }

        #endregion

        #region Pure methods

        public bool Contains(TFrame frame) => _frames.Contains(frame);

        public void CopyTo(TFrame[] array, int arrayIndex) => _frames.CopyTo(array, arrayIndex);

        public int IndexOf(TFrame frame) => _frames.IndexOf(frame);

        #endregion

        #region Mutating methods

        public void Add(TFrame frame)
        {
            AssertValidFrame(frame);

            _frames.Add(frame);
            IncrementVersion();
        }

        public void Insert(int index, TFrame frame)
        {
            AssertValidFrame(frame);

            _frames.Insert(index, frame);
            IncrementVersion();
        }

        public bool Remove(TFrame image)
        {
            if (_frames.Remove(image))
            {
                IncrementVersion();
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            _frames.RemoveAt(index);
            IncrementVersion();
        }

        /// <summary>
        /// Clears the collection, optionally disposing images.
        /// </summary>
        /// <param name="dispose"></param>
        public void Clear(bool dispose)
        {
            InternalClear(dispose);
        }

        /// <summary>
        /// Clears the collection, not disposing images.
        /// </summary>
        public void Clear()
        {
            Clear(false);
        }

        private void InternalClear(bool dispose)
        {
            if (dispose)
            {
                foreach (var frame in _frames)
                    frame.Pixels.Dispose();
            }
            _frames.Clear();
            IncrementVersion();
        }

        #endregion

        #region Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void IncrementVersion()
        {
            unchecked
            {
                _version++;
            }
        }

        [DebuggerHidden]
        protected void AssertValidFrame(TFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            if (Count > 0 && First.GetSize() != frame.GetSize())
                throw new ArgumentException(
                    "The frame has a different size than the frames in this collection.", nameof(frame));

            if (Contains(frame))
                throw new InvalidOperationException("The same frame cannot be added twice.");
        }

        #endregion
    }
}
