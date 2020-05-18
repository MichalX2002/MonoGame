using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Imaging.Attributes.Format;
using MonoGame.Imaging.Codecs;
using MonoGame.Imaging.Codecs.Encoding;
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
            EncodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            AssertValidOutput(output);

            var encoder = AssertValidArguments(imagingConfig, format);
            if (encoder == null) throw new ArgumentNullException(nameof(encoder));
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            if (images == null) throw new ArgumentNullException(nameof(images));

            var state = encoder.CreateState(
                imagingConfig, output, leaveOpen: true, cancellationToken);

            using (state)
            {
                state.EncoderOptions = encoderOptions;
                state.Progress += onProgress;

                bool hasAnimationSupport =
                    format.HasAttribute<IAnimatedFormatAttribute>() &&
                    encoder.HasAttribute<IAnimatedFormatAttribute>();

                foreach (var image in images)
                {
                    encoder.Encode(state, image);

                    if (!hasAnimationSupport)
                        break;
                }
            }
        }

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            Stream output,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            EncodeProgressCallback? onProgress = null,
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
            EncodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (format == null)
                format = ImageFormat.GetByPath(filePath)[0];

            using (var outputStream = OpenWriteStream(filePath))
                Save(
                    images, imagingConfig, outputStream, format,
                    encoderOptions, onProgress, cancellationToken);
        }

        public static void Save(
            this IEnumerable<IReadOnlyPixelRows> images,
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            EncodeProgressCallback? onProgress = null,
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
            EncodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            AssertValidArguments(imagingConfig, format);
            AssertValidOutput(output);

            Save(
                new[] { image }, imagingConfig, output, format,
                encoderOptions, onProgress, cancellationToken);
        }

        public static void Save(
            this IReadOnlyPixelRows image,
            Stream output,
            ImageFormat format,
            EncoderOptions? encoderOptions = null,
            EncodeProgressCallback? onProgress = null,
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
            EncodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (format == null)
                format = ImageFormat.GetByPath(filePath)[0];

            AssertValidArguments(imagingConfig, format);
            AssertValidPath(filePath);

            Save(
                new[] { image }, imagingConfig, filePath, format,
                encoderOptions, onProgress, cancellationToken);
        }

        public static void Save(
            this IReadOnlyPixelRows image,
            string filePath,
            ImageFormat? format = null,
            EncoderOptions? encoderOptions = null,
            EncodeProgressCallback? onProgress = null,
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

            AssertValidPath(filePath);

            return new FileStream(
                filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, options);
        }

        #region Argument Validation

        private static IImageEncoder AssertValidArguments(
            IImagingConfig imagingConfig, ImageFormat format)
        {
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            if (format == null) throw new ArgumentNullException(nameof(format));

            return imagingConfig.GetEncoder(format);
        }

        private static void AssertValidOutput(Stream output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!output.CanWrite)
                throw new ArgumentException("The stream is not writable.", nameof(output));
        }

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