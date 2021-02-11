using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MonoGame.Imaging.Coders;

namespace MonoGame.Imaging.Config.Providers
{
    public delegate T CoderFactory<T>(Stream stream, CoderOptions coderOptions)
        where T : class, IImageCoder;

    public class ImageCoderProvider<TCoder> : IReadOnlyDictionary<ImageFormat, CoderFactory<TCoder>>
        where TCoder : class, IImageCoder
    {
        private ConcurrentDictionary<ImageFormat, CoderFactory<TCoder>> _coders =
            new ConcurrentDictionary<ImageFormat, CoderFactory<TCoder>>();

        public int Count => _coders.Count;

        public IEnumerable<ImageFormat> Keys => _coders.Keys;
        public IEnumerable<CoderFactory<TCoder>> Values => _coders.Values;

        public CoderFactory<TCoder> this[ImageFormat key] => _coders[key];

        public bool TryAddFactory(ImageFormat format, CoderFactory<TCoder> factory)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return _coders.TryAdd(format, factory);
        }

        public bool TryGetFactory(ImageFormat format, [MaybeNullWhen(false)] out CoderFactory<TCoder> factory)
        {
            return _coders.TryGetValue(format, out factory);
        }

        public CoderFactory<TCoder> GetFactory(ImageFormat format)
        {
            if (!TryGetFactory(format, out var factory))
                throw new ImagingException("Missing coder factory for requested format.", format);
            return factory;
        }

        public bool ContainsKey(ImageFormat key)
        {
            return _coders.ContainsKey(key);
        }

        public bool TryGetValue(ImageFormat key, [MaybeNullWhen(false)] out CoderFactory<TCoder> value)
        {
            return _coders.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<ImageFormat, CoderFactory<TCoder>>> GetEnumerator()
        {
            return _coders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
