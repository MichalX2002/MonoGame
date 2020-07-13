using System;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging.Config
{
    public class ImageFormatDetectorProvider : ImageCoderProvider<IImageFormatDetector>
    {
        public int GetMaxHeaderSize()
        {
            int max = 0;
            foreach (var formatDetector in Values)
                max = Math.Max(max, formatDetector.HeaderSize);
            return max;
        }
    }
}
