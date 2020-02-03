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
        public static void Save(
            this IEnumerable<IReadOnlyPixelBuffer> images,
            ImagingConfig imagingConfig,
            Stream output, 
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            var encoder = AssertValidArguments(imagingConfig, format, encoderOptions);
            if (encoderOptions == null)
                encoderOptions = encoder.DefaultOptions;

            AssertValidOutput(output);

            if (encoder == null) throw new ArgumentNullException(nameof(encoder));
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            if (images == null) throw new ArgumentNullException(nameof(images));

            return new ImageEncoderEnumerator(
                encoder, output, encoderOptions, imagingConfig, cancellationToken, onProgress);
        }

        public static FileStream OpenWrite(string filePath)
        {
            AssertValidPath(filePath);
            return new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            Stream output,
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(frames, output, format, encoderOptions,
                ImagingConfig.Default, cancellationToken, onProgress);
        }

        public static void Save<TPixel>(
            this ReadOnlyFrameCollection<TPixel> frames,
            ImagingConfig imagingConfig,
            string filePath, 
            ImageFormat format = null, 
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (format == null) 
                format = ImageFormat.GetByPath(filePath)[0];
            var encoder = AssertValidArguments(imagingConfig, format, encoderOptions);
            
            using (var fs = OpenWrite(filePath))
                Save(frames, fs, format, encoderOptions, imagingConfig, cancellationToken, onProgress);
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

        #region Save(Buffer, Stream)

        public static void Save(
            this IReadOnlyPixelBuffer pixels,
            ImagingConfig imagingConfig, 
            Stream output, 
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            AssertValidArguments(imagingConfig, format, encoderOptions);
            AssertValidOutput(output);

            Save(
                new[] { pixels }, imagingConfig, output, format,
                encoderOptions, cancellationToken, onProgress);
        }

        public static void Save(
            this IReadOnlyPixelBuffer pixels,
            Stream output,
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
        {
            Save(pixels, ImagingConfig.Default, output, format, encoderOptions, onProgress);
        }

        #endregion

        #region Save(Buffer, FilePath)

        public static void Save<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            ImagingConfig imagingConfig,
            string filePath, 
            ImageFormat format = null, 
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (format == null)
                format = ImageFormat.GetByPath(filePath)[0];
            AssertValidArguments(imagingConfig, format, encoderOptions);
            AssertValidPath(filePath);

            var collection = new ReadOnlyFrameCollection<TPixel>(pixels);
            Save(collection, imagingConfig, filePath, format, encoderOptions, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            string filePath,
            ImageFormat format = null,
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(
                pixels, ImagingConfig.Default, filePath,
                format, encoderOptions, onProgress);
        }

        public static void Save<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            string filePath,
            EncoderOptions encoderOptions = null,
            EncodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            Save(
                pixels, ImagingConfig.Default, filePath,
                format: null, encoderOptions, onProgress);
        }

        #endregion

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
    }
}