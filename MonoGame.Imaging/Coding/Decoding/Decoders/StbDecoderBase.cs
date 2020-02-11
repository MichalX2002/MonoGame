using System;
using MonoGame.Framework;
using MonoGame.Imaging.Coding.Identification;
using StbSharp;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract class StbDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }

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
                var vectorType = StbIdentifierBase.CompToVectorType(
                    state.Components, state.BitsPerComponent.Value);

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
            var decoderState = new ImageStbDecoderState(config, this, stream);
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
            ImageDecoderState decoderState,
            DecodeProgressCallback onProgress = null)
        {
            var state = (ImageStbDecoderState)decoderState;
            if (state.ImageIndex < 0)
                throw new InvalidOperationException(
                    $"The decoder state has an invalid {nameof(state.ImageIndex)}.");

            var readState = CreateReadState(state, null, onProgress);
            if (ReadNext(decoderState.ImagingConfig, state.Stream.Context, out void* result, ref readState))
            {
                var image = ParseStbResult(state.ImagingConfig, result, readState);
                state.CurrentImage = image;
                state.ImageIndex++;
            }
            else
            {
                state.CurrentImage = null;
                throw GetFailureException(state.ImagingConfig, decoderState.Stream.Context);
            }
        }

        public virtual void FinishState(ImageDecoderState decoderState)
        {
        }

        #endregion
    }
}
