using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract partial class StbImageDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }
        public virtual DecoderOptions DefaultOptions => DecoderOptions.Default;

        CodecOptions IImageCodec.DefaultOptions => DefaultOptions;

        protected abstract bool Read(StbImageDecoderState decoderState, ReadState readState);

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

        public ImageDecoderState CreateState(ImagingConfig config, ImageReadStream stream)
        {
            return new StbImageDecoderState(this, config, stream);
        }

        public bool Decode(ImageDecoderState decoderState)
        {
            var state = (StbImageDecoderState)decoderState;
            if (state.FrameIndex < 0)
                throw new InvalidOperationException("The decoder state is invalid.");

            var readState = state.CreateReadState();
            if (!Read(state, readState))
            {
                // TODO: get some error message from the context
                if (state.Context.ErrorCode != ErrorCode.Ok)
                    throw new ImagingException(state.Context.ErrorCode.ToString());

                return false;
            }

            state.FrameIndex++;
            return true;
        }
    }
}
