using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate Image RowsProcessorCallback(ReadOnlyPixelRowsContext context);

    public static class PixelRowsProcessingExtensions
    {
        public static Image Process(this IReadOnlyPixelRows pixels,
            ImagingConfig config, RowsProcessorCallback processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelRowsContext(config, pixels));
        }

        public static Image Process(this IReadOnlyPixelRows pixels,
            RowsProcessorCallback processor)
        {
            return Process(pixels, ImagingConfig.Default, processor);
        }
    }
}
