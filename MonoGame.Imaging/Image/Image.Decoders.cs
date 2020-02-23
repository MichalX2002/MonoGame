using System.Collections.Generic;
using MonoGame.Imaging.Coding.Decoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        // TODO: fix up this mess of codec lists

        private static List<IImageDecoder> _decoders;
        private static Dictionary<ImageFormat, IImageDecoder> _decoderByFormat;

        private static List<IImageDecoder> GetDecoders()
        {
            if (_decoders == null)
            {
                _decoders = new List<IImageDecoder>();
                _decoders.Add(new BmpDecoder());
                _decoders.Add(new PngDecoder());
            }
            return _decoders;
        }

        #region [Try]GetDecoder

        public static bool TryGetDecoder(ImageFormat format, out IImageDecoder decoder)
        {
            if (_decoderByFormat == null)
            {
                _decoderByFormat = new Dictionary<ImageFormat, IImageDecoder>
                {
                    { GetDecoders()[0].Format, GetDecoders()[0] },
                    { GetDecoders()[1].Format, GetDecoders()[1] }
                };
            }
            return _decoderByFormat.TryGetValue(format, out decoder);
        }

        public static IImageDecoder GetDecoder(ImageFormat format)
        {
            if (TryGetDecoder(format, out var decoder))
                return decoder;
            throw new MissingDecoderException(format);
        }

        #endregion
    }
}
