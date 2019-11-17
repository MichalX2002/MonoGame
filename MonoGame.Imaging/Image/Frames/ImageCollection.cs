using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MonoGame.Utilities;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    // TODO: add a LayerCollection and change some constraints (like TImage) to be more generous

    public abstract class ImageCollection<TPixel, TImage> : IList<TImage>, IReadOnlyList<TImage>
        where TPixel : unmanaged, IPixel
        where TImage : ReadOnlyImageFrame<TPixel>
    {
        protected List<TImage> _frames;
        protected int _version;

        public TImage this[int imageIndex]
        {
            get => _frames[imageIndex];
            set
            {
                AssertValidFrame(value);
                _frames[imageIndex] = value;
            }
        }

        #region Constructors

        internal ImageCollection(List<TImage> frames, int capacity)
        {
            CommonArgumentGuard.AssertAtleastZero(capacity, nameof(capacity));
            _frames = frames ?? (capacity > 0 ? new List<TImage>(capacity) : new List<TImage>());
        }

        protected static int GetInitialCapacity<T>(IEnumerable<T> enumerable)
        {
            if (enumerable is ICollection coll && coll.Count > 1)
                return coll.Count;
            else if (enumerable is ICollection<T> gColl && gColl.Count > 1)
                return gColl.Count;
            else if (enumerable is IReadOnlyCollection<T> roColl && roColl.Count > 1)
                return roColl.Count;
            return 0;
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
        public TImage First => _frames.Count > 0 ? _frames[0] : default;

        /// <summary>
        /// Gets whether the frames in this collection are a looping sequence.
        /// <para>
        /// This is a simple value used by coders to state that an image is looping.
        /// </para>
        /// </summary>
        public bool IsLooping { get; set; }

        #endregion

        #region Pure methods

        public bool Contains(TImage frame) => _frames.Contains(frame);

        public void CopyTo(TImage[] array, int arrayIndex) => _frames.CopyTo(array, arrayIndex);

        public int IndexOf(TImage frame) => _frames.IndexOf(frame);

        #endregion

        #region Mutating methods

        public void Add(TImage frame)
        {
            AssertValidFrame(frame);

            _frames.Add(frame);
            IncrementVersion();
        }

        public void Insert(int index, TImage frame)
        {
            AssertValidFrame(frame);

            _frames.Insert(index, frame);
            IncrementVersion();
        }

        public bool Remove(TImage image)
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
        protected void AssertValidFrame(TImage frame)
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

        public List<TImage>.Enumerator GetEnumerator() => _frames.GetEnumerator();
        IEnumerator<TImage> IEnumerable<TImage>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
