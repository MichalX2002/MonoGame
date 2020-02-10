using System;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using StbSharp;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract class StbDecoderBase : IImageDecoder, IImageInfoDetector
    {
        public abstract ImageFormat Format { get; }

        private static VectorTypeInfo CompToVectorType(int comp, int compDepth)
        {
            switch (comp)
            {
                case 1:
                    return compDepth == 16 ? VectorTypeInfo.Get<Gray16>() : VectorTypeInfo.Get<Gray8>();

                case 2:
                    return VectorTypeInfo.Get<GrayAlpha16>();

                case 3:
                    return compDepth == 16 ? VectorTypeInfo.Get<Rgb48>() : VectorTypeInfo.Get<Rgb24>();

                case 4:
                    return compDepth == 16 ? VectorTypeInfo.Get<Rgba64>() : VectorTypeInfo.Get<Color>();

                default:
                    return default;
            }
        }

        #region DetectFormat Abstraction

        protected abstract bool TestFormat(ImagingConfig config, ReadContext context);

        private ImageFormat DetectFormat(ImagingConfig config, ReadContext context)
        {
            if (TestFormat(config, context))
                return Format;
            return default;
        }

        public ImageFormat DetectFormat(ImagingConfig config, ImageReadStream stream)
        {
            return DetectFormat(config, stream.Context);
        }

        #endregion

        #region Identify Abstraction

        protected abstract bool GetInfo(ImagingConfig config, ReadContext context, out ReadState readState);

        private ImageInfo Identify(ImagingConfig config, ReadContext context)
        {
            if (GetInfo(config, context, out var readState))
            {
                int comp = readState.Components;
                int bitsPerComp = readState.BitsPerComponent.Value;

                var vectorType = CompToVectorType(comp, bitsPerComp );
                var compInfo = vectorType != null
                    ? vectorType.ComponentInfo :
                    new VectorComponentInfo(new VectorComponent(VectorComponentType.Raw, comp * bitsPerComp));

                return new ImageInfo(readState.Width, readState.Height, compInfo, Format);
            }
            return default;
        }

        public ImageInfo Identify(ImagingConfig config, ImageReadStream stream)
        {
            return Identify(config, stream.Context);
        }

        #endregion

        #region Decode Abstraction

        protected abstract unsafe bool ReadFirst(
            ImagingConfig config, ReadContext context, out void* data, ref ReadState state);

        protected virtual unsafe bool ReadNext(
            ImagingConfig config, ReadContext context, out void* data, ref ReadState state)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, config);
            data = null;
            return false;
        }

        private static ReadState CreateReadState(
            ImageDecoderState decoderState,
            BufferReadyCallback onBufferReady,
            DecodeProgressCallback onProgress)
        {
            var progressCallback = onProgress == null
                ? (ReadProgressCallback)null
                : (percentage, rect) =>
                {
                    Rectangle? rectangle = null;
                    if (rect.HasValue)
                    {
                        var r = rect.Value;
                        rectangle = new Rectangle(r.X, r.Y, r.W, r.H);
                    }
                    onProgress.Invoke(decoderState, percentage, rectangle);
                };

            return new ReadState(onBufferReady, progressCallback);
        }

        protected virtual unsafe Image ParseStbResult(
            ImagingConfig config, void* result, ReadState state)
        {
            try
            {
                var vectorType = CompToVectorType(state.Components, state.BitsPerComponent.Value);

                int pixelCount = state.Width * state.Height;
                var span = new ReadOnlySpan<byte>(result, pixelCount * vectorType.ElementSize);
                var image = Image.LoadPixelData(vectorType, span, new Size(state.Width, state.Height));
                return image;
            }
            finally
            {
                CRuntime.Free(result);
            }
        }

        protected Exception GetFailureException(ImagingConfig config, ReadContext context)
        {
            // TODO: get some error message from the context
            return new Exception();
        }

        #endregion

        #region IImageDecoder

        public unsafe ImageDecoderState DecodeFirst(
            ImagingConfig config,
            ImageReadStream stream,
            DecodeProgressCallback onProgress = null)
        {
            var decoderState = new ImageStbDecoderState(this, config, stream);
            var readState = CreateReadState(decoderState, null, onProgress);
            if (ReadFirst(config, stream.Context, out void* result, ref readState))
            {
                var image = ParseStbResult(config, result, readState);
                decoderState.CurrentImage = image;
                decoderState.ImageIndex++;
                return decoderState;
            }
            else
            {
                throw GetFailureException(config, stream.Context);
            }
        }

        public unsafe void DecodeNext(
            ImagingConfig config,
            ImageDecoderState decoderState,
            DecodeProgressCallback onProgress = null)
        {
            var state = (ImageStbDecoderState)decoderState;
            if (state.ImageIndex < 0)
                throw new InvalidOperationException(
                    $"The decoder state has an invalid {nameof(state.ImageIndex)}.");

            var readState = CreateReadState(state, null, onProgress);
            if (ReadNext(config, state.Stream.Context, out void* result, ref readState))
            {
                var image = ParseStbResult(config, result, readState);
                state.CurrentImage = image;
                state.ImageIndex++;
            }
            else
            {
                state.CurrentImage = null;
                throw GetFailureException(config, decoderState.Stream.Context);
            }
        }

        public virtual void FinishState(ImageDecoderState decoderState)
        {
        }

        #endregion
    }
}
