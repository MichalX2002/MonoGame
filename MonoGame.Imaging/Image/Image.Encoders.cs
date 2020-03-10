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
            }
            return _encoders;
        }

        #region [Try]GetEncoder

        public static bool TryGetEncoder(ImageFormat format, out IImageEncoder encoder)
        {
            if (_encoderByFormat == null)
            {
                _encoderByFormat = new ConcurrentDictionary<ImageFormat, IImageEncoder>();

                _encoderByFormat.TryAdd(GetEncoders()[0].Format, GetEncoders()[0]);
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
