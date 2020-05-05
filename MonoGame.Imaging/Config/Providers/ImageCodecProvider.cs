using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MonoGame.Imaging.Codecs;

namespace MonoGame.Imaging.Config
{
    public class ImageCodecProvider<TCodec> : IEnumerable<KeyValuePair<ImageFormat, TCodec>>
        where TCodec : class, IImageCodec
    {
        private ConcurrentDictionary<ImageFormat, TCodec> _codecs = new ConcurrentDictionary<ImageFormat, TCodec>();

        public IEnumerable<ImageFormat> Keys => _codecs.Keys;
        public IEnumerable<TCodec> Values => _codecs.Values;

        public bool TryAdd(ImageFormat format, TCodec codec)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (codec == null) throw new ArgumentNullException(nameof(codec));

            return _codecs.TryAdd(format, codec);
        }

        public bool TryGetCodec(ImageFormat format, out TCodec? codec)
        {
            return _codecs.TryGetValue(format, out codec);
        }

        public IEnumerator<KeyValuePair<ImageFormat, TCodec>> GetEnumerator()
        {
            return _codecs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
