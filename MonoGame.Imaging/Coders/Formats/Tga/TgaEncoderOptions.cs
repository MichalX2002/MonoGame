using System;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    [Serializable]
    public class TgaEncoderOptions : EncoderOptions
    {
        public bool UseRunLengthEncoding { get; }

        public TgaEncoderOptions(bool useRunLengthEncoding)
        {
            UseRunLengthEncoding = useRunLengthEncoding;
        }
        
        public TgaEncoderOptions() : this(useRunLengthEncoding: true)
        {
        }
    }
}