using System;
using System.IO;
using System.Threading;
using MonoGame.Framework.Vector;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Encoding
{
    public abstract partial class StbImageEncoderBase : IImageEncoder
    {
        public abstract ImageFormat Format { get; }
        public virtual EncoderOptions DefaultOptions => EncoderOptions.Default;

        CodecOptions IImageCodec.DefaultOptions => DefaultOptions;

        protected abstract void Write(
            StbImageEncoderState encoderState,
            IReadOnlyPixelRows image,
            ImageWrite.WriteState writeState);

        public ImageEncoderState CreateState(
            IImagingConfig imagingConfig,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            return new StbImageEncoderState(
                this, imagingConfig, stream, leaveOpen, cancellationToken);
        }

        public void Encode(
            ImageEncoderState encoderState,
            IReadOnlyPixelRows image)
        {
            if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
            if (image == null) throw new ArgumentNullException(nameof(image));

            var state = (StbImageEncoderState)encoderState;
            var pixelInfo = image.PixelType.ComponentInfo;

            // TODO: change components to dynamic/controlled (maybe in encoder options)
            bool imageHasAlpha = pixelInfo.HasComponentType(VectorComponentChannel.Alpha);
            bool encoderSupportsAlpha = true; // TODO: implement (encoderState.Encoder.SupportedComponents HasAttribute();)
            int colors = 3; // TODO: also implement grayscale
            int alpha = (imageHasAlpha && encoderSupportsAlpha) ? 1 : 0;
            int components = colors + alpha;

            int imageMaxDepth = pixelInfo.MaxBitDepth;
            int encoderMinDepth = 8; // TODO: implement
            int encoderMaxDepth = 8; // TODO: implement
            int variableDepth = Math.Max(encoderMinDepth, imageMaxDepth);
            int depth = Math.Min(encoderMaxDepth, variableDepth);

            var provider = new PixelRowProvider(image, components, depth);

            using (var writeState = state.CreateWriteState(provider))
            {
                Write(state, image, writeState);
            }
        }
    }
}
