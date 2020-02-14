using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Coding.Identification;
using StbSharp;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract partial class StbDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }

        #region Decode Abstraction

        protected abstract bool ReadFirst(
            ImagingConfig config, ReadContext context, out IMemoryResult data, ref ReadState state);

        protected virtual bool ReadNext(
            ImagingConfig config, ReadContext context, out IMemoryResult data, ref ReadState state)
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

            return new ReadState(null, null, onBufferReady, progressCallback);
        }

        protected virtual unsafe Image ParseMemoryResult(
            ImagingConfig config, IMemoryResult result, in ReadState state, VectorTypeInfo pixelType = null)
        {
            try
            {
                var fromType = StbIdentifierBase.CompToVectorType(state.OutComponents, state.OutDepth);
                var toType = pixelType ?? fromType;

                if(fromType == toType)
                {
                    var memory = new ResultWrapper(result);
                    var image = Image.WrapMemory(fromType, memory, leaveOpen: false, state.Width, state.Height);
                    return image;
                }
                else
                {
                    using (result)
                    {
                        var span = new ReadOnlySpan<byte>((void*)result.Pointer, result.Length);
                        var image = Image.LoadPixelData(fromType, toType, span, new Size(state.Width, state.Height));
                        return image;
                    }
                }
            }
            catch
            {
                result?.Dispose();
                throw;
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
            VectorTypeInfo pixelType = null,
            DecodeProgressCallback onProgress = null)
        {
            var decoderState = new ImageStbDecoderState(config, this, stream);
            var readState = CreateReadState(decoderState, null, onProgress);
            if (ReadFirst(config, stream.Context, out var result, ref readState))
            {
                var image = ParseMemoryResult(config, result, readState, pixelType);
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
            VectorTypeInfo pixelType = null,
            DecodeProgressCallback onProgress = null)
        {
            var state = (ImageStbDecoderState)decoderState;
            if (state.ImageIndex < 0)
                throw new InvalidOperationException(
                    $"The decoder state has an invalid {nameof(state.ImageIndex)}.");

            var readState = CreateReadState(state, null, onProgress);
            if (ReadNext(decoderState.ImagingConfig, state.Stream.Context, out var result, ref readState))
            {
                var image = ParseMemoryResult(state.ImagingConfig, result, readState, pixelType);
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
