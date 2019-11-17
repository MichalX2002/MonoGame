using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Imaging.Encoding;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Utilities;
using MonoGame.Utilities;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public static partial class SaveExtensions
    {
        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            Stream output, 
            ImageFormat format,
            EncoderConfig encoderConfig,
            ImagingConfig imagingConfig,
            CancellationToken cancellation,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            var encoder = AssertValidArguments(imagingConfig, format, encoderConfig);
            if (encoderConfig == null)
                encoderConfig = encoder.DefaultConfig;

            AssertValidOutput(output);
            AssertValidSource(encoder, imagingConfig, frames);
            
            encoder.Encode(frames, output, encoderConfig, imagingConfig, cancellation, onProgress);
        }

        public static FileStream OpenWrite(string filePath)
        {
            AssertValidPath(filePath);
            return new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);
        }

        #region Argument Validation

        [DebuggerHidden]
        private static void AssertValidSource<TPixel, TFrame>(
            IImageEncoder encoder, ImagingConfig config, ImageCollection<TPixel, TFrame> frames)
            where TPixel : unmanaged, IPixel
            where TFrame : ReadOnlyImageFrame<TPixel>
        {
            if (encoder == null) throw new ArgumentNullException(nameof(encoder));
            if (config == null) throw new ArgumentNullException(nameof(config));
            CommonArgumentGuard.AssertNonEmpty(frames?.Count, nameof(frames));

            if (frames.Count > 1)
                ImagingArgumentGuard.AssertAnimationSupport(encoder, config);
        }

        [DebuggerHidden]
        private static IImageEncoder AssertValidArguments(
            ImagingConfig imagingConfig, ImageFormat format, EncoderConfig encoderConfig)
        {
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            if (format == null) throw new ArgumentNullException(nameof(format));

            var encoder = Image.GetEncoder(format);

            if (encoderConfig != null)
                EncoderConfig.AssertTypeEqual(encoder.DefaultConfig, encoderConfig, nameof(encoderConfig));

            return encoder;
        }

        [DebuggerHidden]
        private static void AssertValidOutput(Stream output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
        }

        [DebuggerHidden]
        public static void AssertValidPath(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentEmptyException(nameof(filePath));

            Path.GetFullPath(filePath);
        }

        #endregion

        #region Save(FrameCollection) Overloads

        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            Stream output,
            ImageFormat format,
            EncoderConfig encoderConfig,
            CancellationToken cancellation,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(frames, output, format, encoderConfig,
                ImagingConfig.Default, cancellation, onProgress);
        }

        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            string filePath, 
            ImagingConfig imagingConfig,
            CancellationToken cancellation,
            ImageFormat format = null, 
            EncoderConfig encoderConfig = null,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (format == null) format = ImageFormat.GetByPath(filePath);
            var encoder = AssertValidArguments(imagingConfig, format, encoderConfig);
            AssertValidSource(encoder, imagingConfig, frames);

            using (var fs = OpenWrite(filePath))
                Save(frames, fs, format, encoderConfig, imagingConfig, cancellation, onProgress);
        }

        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            string filePath, 
            ImageFormat format = null,
            EncoderConfig encoderConfig = null,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(
                frames, filePath, ImagingConfig.Default, CancellationToken.None,
                format, encoderConfig, onProgress);
        }

        #endregion

        /*
        
        // TODO: remove this and add ones for Image<T> instead
        #region Save(IReadOnlyPixelRows) Overloads

        public static void Save<TPixel>(
            this IReadOnlyPixelRows<TPixel> image,
            ImagingConfig imagingConfig, Stream output, 
            ImageFormat format, EncoderConfig encoderConfig = null,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            AssertValidSource(image);
            AssertValidArguments(imagingConfig, format, encoderConfig);
            AssertValidOutput(output);

            var collection = new ReadOnlyFrameCollection<TPixel>(image);
            Save(collection, imagingConfig, output, format, encoderConfig, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelRows<TPixel> image,
            Stream output, ImageFormat format, EncoderConfig encoderConfig = null,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(image, ImagingConfig.Default, output, format, encoderConfig, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelRows<TPixel> image,
            ImagingConfig imagingConfig, string filePath, 
            ImageFormat format = null, EncoderConfig encoderConfig = null,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            AssertValidSource(image);
            if (format == null) format = ImageFormat.GetByPath(filePath);
            AssertValidArguments(imagingConfig, format, encoderConfig);
            AssertValidPath(filePath);

            var collection = new ReadOnlyFrameCollection<TPixel>(image);
            Save(collection, imagingConfig, filePath, format, encoderConfig, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelRows<TPixel> image,
            string filePath, ImageFormat format = null, EncoderConfig encoderConfig = null,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(image, ImagingConfig.Default, filePath, format, encoderConfig, onProgress);
        }

        #endregion

        */
    }
}