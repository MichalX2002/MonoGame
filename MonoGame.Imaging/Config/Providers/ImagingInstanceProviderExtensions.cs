using System;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging.Config.Providers
{
    public static class ImagingInstanceProviderExtensions
    {
        public static int GetMaxHeaderSize(this ImagingInstanceProvider<IImageFormatDetector> provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            int max = 0;
            foreach (IImageFormatDetector formatDetector in provider.Values)
                max = Math.Max(max, formatDetector.HeaderSize);
            return max;
        }
    }
}
