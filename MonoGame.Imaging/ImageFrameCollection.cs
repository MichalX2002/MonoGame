using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Represents a collection of frames that make up an image.
    /// <para>
    /// Frames that are disposed while in the collection get removed.
    /// </para>
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    public class ImageFrameCollection<TPixel> : IDisposable, IList<ImageFrame<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        // cache the predicate
        private static readonly Predicate<ImageFrame<TPixel>> _nullFramePredicate = (f) => f == null;

        private List<ImageFrame<TPixel>> _frames;
        private Image<TPixel> _parent;
        private bool _disposing;

        public bool IsReadOnly => false;
        public bool IsDisposed { get; private set; }

        #region Indexer + Properties

        public ImageFrame<TPixel> this[int frameIndex]
        {
            get
            {
                AssertNotDisposed();
                return _frames[frameIndex];
            }
            set
            {
                AssertNotDisposed();
                var old = _frames[frameIndex];
                if (old != value)
                {
                    _frames[frameIndex] = value;
                    value.OnDisposed += Frame_OnDisposed;
                    old.OnDisposed -= Frame_OnDisposed;
                }
            }
        }

        /// <summary>
        /// The first frame in the collection.
        /// </summary>
        public ImageFrame<TPixel> RootFrame
        {
            get
            {
                AssertNotDisposed();
                return _frames[0];
            }
        }

        public Image<TPixel> Parent
        {
            get
            {
                AssertNotDisposed();
                return _parent;
            }
        }

        public int Count
        {
            get
            {
                AssertNotDisposed();
                return _frames.Count;
            }
        }

        public int Capacity
        {
            get
            {
                AssertNotDisposed();
                return _frames.Capacity;
            }
            set
            {
                AssertNotDisposed();
                _frames.Capacity = value;
            }
        }

        #endregion

        #region Public Constructors

        public ImageFrameCollection(ImageFrame<TPixel> rootFrame)
        {
            if (rootFrame == null)
                throw new ArgumentNullException(nameof(rootFrame));

            _frames = new List<ImageFrame<TPixel>> { rootFrame };
        }

        public ImageFrameCollection(IEnumerable<ImageFrame<TPixel>> frames)
        {
            var list = new List<ImageFrame<TPixel>>(frames);
            list.RemoveAll(_nullFramePredicate);

            if (list.Count == 0)
                throw new ArgumentException("The enumerable is empty.");

            _frames = list;
        }

        #endregion

        public void AttachParent(Image<TPixel> image)
        {
            AssertNotDisposed();
            if (_parent != null)
                throw new InvalidOperationException(
                    "This collection is already attached to an image.");
            _parent = image ?? throw new ArgumentNullException(nameof(image));
        }

        /// <summary>
        /// Clears the collection leaving behind the root frame.
        /// </summary>
        public void Clear()
        {
            AssertNotDisposed();
            var rootFrame = RootFrame;
            _frames.Clear();
            _frames.Add(rootFrame);
        }

        public void Add(ImageFrame<TPixel> frame)
        {
            AssertNotDisposed();
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            _frames.Add(frame);
            frame.OnDisposed += Frame_OnDisposed;
        }

        public bool Contains(ImageFrame<TPixel> frame)
        {
            AssertNotDisposed();
            return _frames.Contains(frame);
        }

        public void CopyTo(ImageFrame<TPixel>[] array, int arrayIndex)
        {
            AssertNotDisposed();
            _frames.CopyTo(array, arrayIndex);
        }

        public int IndexOf(ImageFrame<TPixel> frame)
        {
            AssertNotDisposed();
            return _frames.IndexOf(frame);
        }

        public void Insert(int index, ImageFrame<TPixel> frame)
        {
            AssertNotDisposed();
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            _frames.Insert(index, frame);
            frame.OnDisposed += Frame_OnDisposed;
        }

        public bool Remove(ImageFrame<TPixel> frame)
        {
            AssertNotDisposed();
            if (_frames.Count == 0 && RootFrame == frame)
                throw new InvalidOperationException(
                    "Cannot remove the frame as it's the only one left.");

            if (_frames.Remove(frame))
            {
                frame.OnDisposed -= Frame_OnDisposed;
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            AssertNotDisposed();
            var frame = _frames[index];
            _frames.RemoveAt(index);
            frame.OnDisposed -= Frame_OnDisposed;
        }

        private void Frame_OnDisposed(ImageFrame<TPixel> sender)
        {
            if(!_disposing)
                Remove(sender);
        }

        public List<ImageFrame<TPixel>>.Enumerator GetEnumerator()
        {
            AssertNotDisposed();
            return _frames.GetEnumerator();
        }

        IEnumerator<ImageFrame<TPixel>> IEnumerable<ImageFrame<TPixel>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region IDisposable Implementation

        [DebuggerHidden]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ImageFrameCollection<TPixel>));
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                _disposing = true;

                foreach (var frame in _frames)
                    frame.Dispose();
                _frames.Clear();

                _parent = null;
                _frames = null;
                IsDisposed = true;
            }
        }

        #endregion
    }
}
