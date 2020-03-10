using System;
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

        protected abstract bool ReadFirst(ImageStbDecoderState decoderState, ref ReadState readState);

        protected virtual bool ReadNext(ImageStbDecoderState decoderState, ref ReadState readState)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, decoderState.ImagingConfig);
            return false;
        }

        private static ReadState CreateReadState(
            ImageStbDecoderState decoderState,
            VectorTypeInfo pixelType = null)
        {
            //var progressCallback = onProgress == null
            //    ? (ReadProgressCallback)null
            //    : (percentage, rect) =>
            //    {
            //        Rectangle? rectangle = null;
            //        if (rect.HasValue)
            //        {
            //            var r = rect.Value;
            //            rectangle = new Rectangle(r.X, r.Y, r.W, r.H);
            //        }
            //        onProgress.Invoke(decoderState, percentage, rectangle);
            //    };

            void OnStateReady(in ReadState state)
            {
                decoderState.SourcePixelType = GetVectorType(state);
                var dstType = pixelType ?? decoderState.SourcePixelType;
                var size = new Size(state.Width, state.Height);
                decoderState.CurrentImage = Image.Create(dstType, size);
            }

            void OnOutputBytes(in ReadState state, int row, ReadOnlySpan<byte> pixels)
            {
                if (decoderState.SourcePixelType == null)
                    throw new InvalidOperationException("Missing source pixel type.");
                if (decoderState.CurrentImage == null)
                    throw new InvalidOperationException("Missing image buffer.");

                if (decoderState.SourcePixelType != decoderState.CurrentImage.PixelType)
                    throw new NotImplementedException();

                decoderState.CurrentImage.SetPixelByteRow(0, row, pixels);
            }

            return new ReadState(OnStateReady, OnOutputBytes, null, null);
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
            VectorTypeInfo pixelType = null,
            DecodeProgressCallback onProgress = null)
        {
            var state = new ImageStbDecoderState(config, this, stream);
            var readState = CreateReadState(state, pixelType);
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
            VectorTypeInfo pixelType = null,
            DecodeProgressCallback onProgress = null)
        {
            var state = (ImageStbDecoderState)decoderState;
            if (state.ImageIndex < 0)
                throw new InvalidOperationException("The decoder state is invalid.");

            var readState = CreateReadState(state, pixelType);
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

        public static VectorTypeInfo GetVectorType(in ReadState state)
        {
            return StbIdentifierBase.GetVectorType(state.Components, state.OutDepth);
        }
    }
}
