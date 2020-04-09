using System;
using MonoGame.Framework.PackedVector;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract partial class StbImageDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }

        #region Decode Abstraction

        protected abstract bool ReadFirst(StbImageDecoderState decoderState, ref ReadState readState);

        protected virtual bool ReadNext(StbImageDecoderState decoderState, ref ReadState readState)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, decoderState.ImagingConfig);
            return false;
        }

        //protected virtual unsafe Image ParseMemoryResult(
        //    ImagingConfig config, IMemoryHolder result, in ReadState state, VectorTypeInfo pixelType = null)
        //{
        //    try
        //    {
        //        var fromType = StbIdentifierBase.CompToVectorType(state.Components, state.Depth);
        //        var toType = pixelType ?? fromType;
        //
        //        if (fromType == toType)
        //        {
        //            var memory = new ResultWrapper(result);
        //            var image = Image.WrapMemory(
        //                fromType, memory, new Size(state.Width, state.Height), leaveOpen: false);
        //            return image;
        //        }
        //        else
        //        {
        //            using (result)
        //            {
        //                var size = new Size(state.Width, state.Height);
        //                var image = Image.LoadPixelData(fromType, toType, result.Span, size);
        //                return image;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        result?.Dispose();
        //        throw;
        //    }
        //}

        protected Exception GetFailureException(ImagingConfig config, ReadContext context)
        {
            // TODO: get some error message from the context
            return new ImagingException(context.ErrorCode.ToString());
        }

        #endregion

        #region IImageDecoder

        public ImageDecoderState DecodeFirst(
            ImagingConfig config,
            ImageReadStream stream,
            VectorTypeInfo pixelType = null)
        {
            var state = new StbImageDecoderState(config, this, stream);
            state.PreferredPixelType = pixelType;

            var readState = state.CreateReadState();
            bool result = ReadFirst(state, ref readState);
            if (result && state.Context.ErrorCode == ErrorCode.Ok)
            {
                state.ImageIndex++;
                return state;
            }
            else
            {
                state.CurrentImage?.Dispose();
                state.CurrentImage = null;
                throw GetFailureException(config, stream.Context);
            }
        }

        public void DecodeNext(
            ImageDecoderState decoderState,
            VectorTypeInfo pixelType = null)
        {
            var state = (StbImageDecoderState)decoderState;
            if (state.ImageIndex < 0)
                throw new InvalidOperationException("The decoder state is invalid.");
            state.PreferredPixelType = pixelType;

            var readState = state.CreateReadState();
            bool result = ReadNext(state, ref readState);
            if (result && state.Context.ErrorCode == ErrorCode.Ok)
            {
                state.ImageIndex++;
            }
            else
            {
                state.CurrentImage?.Dispose();
                state.CurrentImage = null;
                throw GetFailureException(state.ImagingConfig, state.Context);
            }
        }

        #endregion
    }
}
