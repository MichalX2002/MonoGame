using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MonoGame.Imaging.Coders;

namespace MonoGame.Imaging.Config
{
    public class ImageCoderProvider<TCoder> : IEnumerable<KeyValuePair<ImageFormat, TCoder>>
        where TCoder : class, IImageCoder
    {
        private ConcurrentDictionary<ImageFormat, TCoder> _coders =
            new ConcurrentDictionary<ImageFormat, TCoder>();

        public IEnumerable<ImageFormat> Keys => _coders.Keys;
        public IEnumerable<TCoder> Values => _coders.Values;

        public bool TryAdd(ImageFormat format, TCoder coder)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (coder == null) throw new ArgumentNullException(nameof(coder));

            return _coders.TryAdd(format, coder);
        }

        public bool TryGetCoder(ImageFormat format, [MaybeNullWhen(false)] out TCoder? coder)
        {
            return _coders.TryGetValue(format, out coder);
        }

        public IEnumerator<KeyValuePair<ImageFormat, TCoder>> GetEnumerator()
        {
            return _coders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
