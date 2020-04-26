using System.Collections.Concurrent;
using System.Collections.Generic;
using MonoGame.Imaging.Coding.Encoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        // TODO: fix up this mess of codec lists

        private static List<IImageEncoder> _encoders;
        private static ConcurrentDictionary<ImageFormat, IImageEncoder> _encoderByFormat;

        private static List<IImageEncoder> GetEncoders()
        {
            if (_encoders == null)
            {
                _encoders = new List<IImageEncoder>();
                _encoders.Add(new PngImageEncoder());
                _encoders.Add(new BmpImageEncoder());
                _encoders.Add(new TgaImageEncoder());
                _encoders.Add(new JpegImageEncoder());
            }
            return _encoders;
        }

        #region [Try]GetEncoder

        public static bool TryGetEncoder(ImageFormat format, out IImageEncoder encoder)
        {
            if (_encoderByFormat == null)
            {
                _encoderByFormat = new ConcurrentDictionary<ImageFormat, IImageEncoder>();

                foreach (var enc in GetEncoders())
                    _encoderByFormat.TryAdd(enc.Format, enc);
            }
            return _encoderByFormat.TryGetValue(format, out encoder);
        }

        public static IImageEncoder GetEncoder(ImageFormat format)
        {
            if (TryGetEncoder(format, out var encoder))
                return encoder;
            throw new MissingEncoderException(format);
        }

        #endregion
    }
}
