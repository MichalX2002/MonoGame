using System.Collections;
using System.Collections.Generic;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding
{
    public abstract class ImageCoderEnumerator<TImage> : IEnumerable<TImage>, IEnumerator<TImage>
        where TImage : IReadOnlyPixelBuffer
    {
        /// <summary>
        /// Gets or sets the zero-based index of the most recently processed image.
        /// </summary>
        public int ImageIndex { get; set; }

        public TImage Current { get; protected set; }
        object IEnumerator.Current => Current;

        public abstract bool MoveNext();

        public abstract void Reset();

        public abstract void Dispose();

        IEnumerator<TImage> IEnumerable<TImage>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
