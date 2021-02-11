using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Attributes.Format;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public static partial class SaveExtensions
    {
        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            IImagingConfig imagingConfig,
            Stream output,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (images == null)
                throw new ArgumentNullException(nameof(images));
            if (imagingConfig == null)
                throw new ArgumentNullException(nameof(imagingConfig));
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!output.CanWrite)
                throw new ArgumentException("The stream is not writable.", nameof(output));

            using IImageEncoder encoder = imagingConfig.CreateEncoder(output, format, encoderOptions);

            if (onProgress != null && encoder is IProgressReportingCoder<IImageEncoder> progressReporter)
                progressReporter.Progress += onProgress;

            foreach (IReadOnlyPixelRows image in images)
            {
                if (!encoder.CanEncodeImage(image))
                    break;

                encoder.Encode(image, cancellationToken);
            }
        }

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            Stream output,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            Save(
                images, ImagingConfig.Default, output, format,
                encoderOptions, onProgress, cancellationToken);
        }

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            IImagingConfig imagingConfig,
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (format == null)
                format = ImageFormat.GetByPath(filePath)[0];

            using var output = OpenWriteStream(filePath);
            Save(
                images, imagingConfig, output, format,
                encoderOptions, onProgress, cancellationToken);
        }

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            Save(
                images, ImagingConfig.Default, filePath, format,
                encoderOptions, onProgress, cancellationToken);
        }

        #region Save(Stream)

        public static void Save(
            this IReadOnlyPixelRows image,
            IImagingConfig imagingConfig,
            Stream output,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            Save(
                new[] { image }, imagingConfig, output, format,
                encoderOptions, onProgress, cancellationToken);
        }

        public static void Save(
            this IReadOnlyPixelRows image,
            Stream output,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            Save(
                image, ImagingConfig.Default, output, format,
                encoderOptions, onProgress, cancellationToken);
        }

        #endregion

        #region Save(FilePath)

        public static void Save(
            this IReadOnlyPixelRows image,
            IImagingConfig imagingConfig,
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (format == null)
                format = ImageFormat.GetByPath(filePath)[0];

            Save(
                new[] { image }, imagingConfig, filePath, format,
                encoderOptions, onProgress, cancellationToken);
        }

        public static void Save(
            this IReadOnlyPixelRows image,
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            ImagingProgressCallback<IImageEncoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            Save(
                image, ImagingConfig.Default, filePath, format,
                encoderOptions, onProgress, cancellationToken);
        }

        #endregion

        public static FileStream OpenWriteStream(string filePath)
        {
            const FileOptions options = FileOptions.None;
            const int bufferSize = 1024 * 4;

            return new FileStream(
                filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, options);
        }
    }
}