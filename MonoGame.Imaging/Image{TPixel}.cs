using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public class Image<TPixel> : IDisposable, IReadOnlyCollection<ImageFrame<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        public bool IsDisposed { get; private set; }

        public ImageFrameCollection<TPixel> Frames { get; }
        public int Count => Frames.Count;

        #region Constructors

        public Image(ImageFrameCollection<TPixel> frames)
        {
            Frames = frames;
            Frames.AttachParent(this);
        }

        #endregion

        #region IEnumerator Getters

        public List<ImageFrame<TPixel>>.Enumerator GetEnumerator()
        {
            AssertNotDisposed();
            return Frames.GetEnumerator();
        }

        IEnumerator<ImageFrame<TPixel>> IEnumerable<ImageFrame<TPixel>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IDisposable

        [DebuggerHidden]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Image<TPixel>));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Frames.Dispose();
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Image()
        {
            Dispose(false);
        }

        #endregion
    }
}
