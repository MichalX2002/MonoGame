using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Represents a collection of image frames that have the same size and pixel type.
    /// </summary>
    public class FrameCollection<TPixel> : FrameCollectionBase<TPixel, ImageFrame<TPixel>>,
        IList<ImageFrame<TPixel>>,
        IReadOnlyCollection<Image<TPixel>>,
        IReadOnlyCollection<IPixelRows<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        #region Constructors

        /// <summary>
        /// Constructs the collection without any initial frames, defining an inital capacity.
        /// </summary>
        public FrameCollection(int capacity) : base(null, capacity)
        {
        }

        /// <summary>
        /// Constructs the collection without any initial frames.
        /// </summary>
        public FrameCollection() : this(0)
        {
        }

        /// <summary>
        /// Constructs the collection and adds frames from an enumerable.
        /// </summary>
        /// <param name="frames">The enumerable of frames that will be added to the collection.</param>
        public FrameCollection(IEnumerable<ImageFrame<TPixel>> frames) : this()
        {
            if (frames != null)
                foreach (var frame in frames)
                    Add(frame);
        }

        /// <summary>
        /// Constructs the collection and adds images from an enumerable.
        /// </summary>
        /// <param name="images">The enumerable of images that will be added to the collection.</param>
        /// <param name="delay">The delay to use for every image in the enumerable.</param>
        public FrameCollection(IEnumerable<Image<TPixel>> images, int delay) :
            this(GetInitialCapacity(images))
        {
            if (images != null)
                foreach (var image in images)
                    Add(new ImageFrame<TPixel>(image, delay));
        }

        public FrameCollection(Image<TPixel> image) : this(1)
        {
            Add(new ImageFrame<TPixel>(image, 0));
        }

        #endregion

        #region Pure methods

        public bool Contains(Image<TPixel> image) => IndexOf(image) != -1;

        public void CopyTo(Image<TPixel>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The index may not be negative.", nameof(arrayIndex));

            if (_frames.Count + arrayIndex > array.Length)
                throw new ArgumentException(
                    $"The array (starting at {nameof(arrayIndex)}) has insufficient capacity.", nameof(array));

            for (int i = 0; i < _frames.Count; i++)
                array[i + arrayIndex] = _frames[i].Pixels;
        }

        public int IndexOf(Image<TPixel> image)
        {
            for (int i = 0; i < _frames.Count; i++)
            {
                if (image == _frames[i].Pixels)
                    return i;
            }
            return -1;
        }

        #endregion

        #region Mutating methods

        public void Add(Image<TPixel> image, int delay)
        {
            Add(new ImageFrame<TPixel>(image, delay));
        }

        public bool Remove(Image<TPixel> image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            for (int i = 0; i < _frames.Count; i++)
            {
                if (image == _frames[i].Pixels)
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region GetEnumerator

        IEnumerator<Image<TPixel>> IEnumerable<Image<TPixel>>.GetEnumerator() => new ImageEnumerator(this);
        IEnumerator<IPixelRows<TPixel>> IEnumerable<IPixelRows<TPixel>>.GetEnumerator() => new PixelRowsEnumerator(this);

        /// <summary>
        /// Enumerates images from the frames of a <see cref="FrameCollection{TPixel}"/>.
        /// </summary>
        public struct ImageEnumerator : IEnumerator<Image<TPixel>>
        {
            private FrameCollection<TPixel> _collection;
            private int _index;
            private int _version;

            public Image<TPixel> Current { get; private set; }
            object IEnumerator.Current => Current;

            internal ImageEnumerator(FrameCollection<TPixel> collection)
            {
                _collection = collection;
                _index = 0;
                _version = _collection._version;
                Current = null;
            }

            public bool MoveNext()
            {
                AssertCollectionVersion();

                if (_index < _collection.Count)
                {
                    Current = _collection._frames[_index].Pixels;
                    _index++;
                    return true;
                }

                _index = _collection.Count + 1;
                Current = null;
                return false;
            }

            public void Reset()
            {
                AssertCollectionVersion();

                _index = 0;
                Current = null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void AssertCollectionVersion()
            {
                if (_version != _collection._version)
                    throw new InvalidOperationException("The underlying collection has changed.");
            }

            public void Dispose()
            {
            }
        }

        /// <summary>
        /// Enumerates images as views from the frames of a <see cref="FrameCollection{TPixel}"/>.
        /// </summary>
        public struct PixelRowsEnumerator : IEnumerator<IPixelRows<TPixel>>
        {
            private FrameCollection<TPixel> _collection;
            private int _index;
            private int _version;

            public IPixelRows<TPixel> Current { get; private set; }
            object IEnumerator.Current => Current;

            internal PixelRowsEnumerator(FrameCollection<TPixel> collection)
            {
                _collection = collection;
                _index = 0;
                _version = _collection._version;
                Current = null;
            }

            public bool MoveNext()
            {
                AssertCollectionVersion();

                if (_index < _collection.Count)
                {
                    Current = _collection._frames[_index].Pixels;
                    _index++;
                    return true;
                }

                _index = _collection.Count + 1;
                Current = null;
                return false;
            }

            public void Reset()
            {
                AssertCollectionVersion();

                _index = 0;
                Current = null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void AssertCollectionVersion()
            {
                if (_version != _collection._version)
                    throw new InvalidOperationException("The underlying collection has changed.");
            }

            public void Dispose()
            {
            }
        }

        #endregion
    }
}
