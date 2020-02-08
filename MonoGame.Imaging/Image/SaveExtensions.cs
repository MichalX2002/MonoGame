using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Imaging.Coding.Encoding;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public static partial class SaveExtensions
    {
        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
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

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            Stream output,
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            Save(
                images, ImagingConfig.Default, output, format, 
                encoderOptions, cancellationToken, onProgress);
        }

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            ImagingConfig imagingConfig,
            string filePath, 
            ImageFormat format = null, 
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            if (format == null) 
                format = ImageFormat.GetByPath(filePath)[0];
            
            using (var outputStream = OpenWrite(filePath))
                Save(
                    images, imagingConfig, outputStream, format, 
                    encoderOptions, cancellationToken, onProgress);
        }

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            string filePath, 
            ImageFormat format = null,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            Save(
                images, ImagingConfig.Default, filePath, format, 
                encoderOptions, cancellationToken, onProgress);
        }

        #region Save(Stream)

        public static void Save(
            this IReadOnlyPixelRows image,
            ImagingConfig imagingConfig, 
            Stream output, 
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            AssertValidArguments(imagingConfig, format, encoderOptions);
            AssertValidOutput(output);

            Save(
                new[] { image }, imagingConfig, output, format,
                encoderOptions, cancellationToken, onProgress);
        }

        public static void Save(
            this IReadOnlyPixelRows image,
            Stream output,
            ImageFormat format,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            Save(
                image, ImagingConfig.Default, output, format,
                encoderOptions, cancellationToken, onProgress);
        }

        #endregion

        #region Save(FilePath)

        public static void Save(
            this IReadOnlyPixelRows image,
            ImagingConfig imagingConfig,
            string filePath, 
            ImageFormat format = null, 
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            if (format == null)
                format = ImageFormat.GetByPath(filePath)[0];
            AssertValidArguments(imagingConfig, format, encoderOptions);
            AssertValidPath(filePath);

            Save(
                new[] { image }, imagingConfig, filePath, format,
                encoderOptions, cancellationToken, onProgress);
        }

        public static void Save(
            this IReadOnlyPixelRows image,
            string filePath,
            ImageFormat format = null,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            Save(
                image, ImagingConfig.Default, filePath, format, 
                encoderOptions, cancellationToken, onProgress);
        }

        #endregion

        public static FileStream OpenWrite(string filePath)
        {
            AssertValidPath(filePath);
            return new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        #region Argument Validation

        [DebuggerHidden]
        private static IImageEncoder AssertValidArguments(
            ImagingConfig imagingConfig, ImageFormat format, EncoderOptions encoderOptions = null)
        {
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
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

            if (!output.CanWrite)
                throw new ArgumentException("The stream is not writable." ,nameof(output));
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