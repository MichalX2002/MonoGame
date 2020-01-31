using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Imaging.Coding.Encoding;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public static partial class SaveExtensions
    {
        public static void Save<TPixel>(
            this IEnumerable<IReadOnlyPixelBuffer<TPixel>> images,
            ImagingConfig config,
            Stream output, 
            ImageFormat format,
            EncoderOptions encoderOptions,
            CancellationToken cancellation,
            EncodeProgressCallback onProgress = null)
        {
            var encoder = AssertValidArguments(config, format, encoderOptions);
            if (encoderOptions == null)
                encoderOptions = encoder.DefaultOptions;

            AssertValidOutput(output);

            if (encoder == null) throw new ArgumentNullException(nameof(encoder));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (images == null) throw new ArgumentNullException(nameof(images));

            return new ImageEncoderEnumerator(
                encoder, output, encoderOptions, config, cancellation, onProgress);
        }

        public static FileStream OpenWrite(string filePath)
        {
            AssertValidPath(filePath);
            return new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        #region Argument Validation

        [DebuggerHidden]
        private static IImageEncoder AssertValidArguments(
            ImagingConfig config, ImageFormat format, EncoderOptions encoderOptions)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (format == null) throw new ArgumentNullException(nameof(format));

            var encoder = Image.GetEncoder(format);

            if (encoderOptions != null)
                EncoderOptions.AssertTypeEqual(
                    encoder.DefaultOptions, encoderOptions, nameof(encoderOptions));

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
            EncoderOptions encoderOptions,
            CancellationToken cancellation,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(frames, output, format, encoderOptions,
                ImagingConfig.Default, cancellation, onProgress);
        }

        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            string filePath, 
            ImagingConfig imagingConfig,
            CancellationToken cancellation,
            ImageFormat format = null, 
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (format == null) format = ImageFormat.GetByPath(filePath);
            var encoder = AssertValidArguments(imagingConfig, format, encoderOptions);
            AssertValidSource(encoder, imagingConfig, frames);

            using (var fs = OpenWrite(filePath))
                Save(frames, fs, format, encoderOptions, imagingConfig, cancellation, onProgress);
        }

        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            string filePath, 
            ImageFormat format = null,
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(
                frames, filePath, ImagingConfig.Default, CancellationToken.None,
                format, encoderOptions, onProgress);
        }

        #endregion

        // TODO: remove this and add ones for Image<T> instead
        #region Save(IReadOnlyPixelRows) Overloads

        public static void Save<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            ImagingConfig imagingConfig, 
            Stream output, 
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            AssertValidSource(pixels);
            AssertValidArguments(imagingConfig, format, encoderOptions);
            AssertValidOutput(output);
            
            var collection = new LayerCollection<TPixel, IReadOnlyPixelBuffer<TPixel>>(pixels);
            Save(collection, imagingConfig, output, format, encoderOptions, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            Stream output, 
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(pixels, ImagingConfig.Default, output, format, encoderOptions, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            ImagingConfig imagingConfig,
            string filePath, 
            ImageFormat format = null, 
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            AssertValidSource(pixels);
            if (format == null) format = ImageFormat.GetByPath(filePath);
            AssertValidArguments(imagingConfig, format, encoderOptions);
            AssertValidPath(filePath);

            var collection = new ReadOnlyFrameCollection<TPixel>(pixels);
            Save(collection, imagingConfig, filePath, format, encoderOptions, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            string filePath, ImageFormat format = null, EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(pixels, ImagingConfig.Default, filePath, format, encoderOptions, onProgress);
        }

        #endregion
    }
}