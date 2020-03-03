﻿using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Coding.Identification;
using StbSharp;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract partial class StbDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }

        #region Decode Abstraction

        protected abstract IMemoryHolder ReadFirst(ImageStbDecoderState decoderState, ref ReadState readState);

        protected virtual IMemoryHolder ReadNext(ImageStbDecoderState decoderState, ref ReadState readState)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, decoderState.ImagingConfig);
            return null;
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
            ImagingConfig config, IMemoryHolder result, in ReadState state, VectorTypeInfo pixelType = null)
        {
            try
            {
                var fromType = StbIdentifierBase.CompToVectorType(state.OutComponents, state.OutDepth);
                var toType = pixelType ?? fromType;

                if (fromType == toType)
                {
                    var memory = new ResultWrapper(result);
                    var image = Image.WrapMemory(fromType, memory, new Size(state.Width, state.Height), leaveOpen: false);
                    return image;
                }
                else
                {
                    using (result)
                    {
                        var size = new Size(state.Width, state.Height);
                        var image = Image.LoadPixelData(fromType, toType, result.Span, size);
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
            return new ImagingException(context.ErrorCode.ToString());
        }

        #endregion

        #region IImageDecoder

        public unsafe ImageDecoderState DecodeFirst(
            ImagingConfig config,
            ImageReadStream stream,
            VectorTypeInfo pixelType = null,
            DecodeProgressCallback onProgress = null)
        {
            var state = new ImageStbDecoderState(config, this, stream);
            var readState = CreateReadState(state, null, onProgress);
            var result = ReadFirst(state, ref readState);
            if (state.ReadContext.ErrorCode == ErrorCode.Ok && result != null)
            {
                state.CurrentImage = ParseMemoryResult(config, result, readState, pixelType);
                state.ImageIndex++;
                return state;
            }
            else
            {
                state.CurrentImage = null;
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
                throw new InvalidOperationException("The decoder state is invalid.");

            var readState = CreateReadState(state, null, onProgress);
            var result = ReadNext(state, ref readState);
            if (state.ReadContext.ErrorCode == ErrorCode.Ok && result != null)
            {
                state.CurrentImage = ParseMemoryResult(state.ImagingConfig, result, readState, pixelType);
                state.ImageIndex++;
            }
            else
            {
                state.CurrentImage = null;
                throw GetFailureException(state.ImagingConfig, decoderState.Stream.Context);
            }
        }

        #endregion
    }
}