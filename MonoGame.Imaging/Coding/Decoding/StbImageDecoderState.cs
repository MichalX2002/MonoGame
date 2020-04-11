using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Coding.Identification;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class StbImageDecoderState : ImageDecoderState
    {
        private StateReadyCallback _onStateReady;
        private OutputByteDataCallback _onOutputByteData;
        private OutputShortDataCallback _onOutputShortData;
        private OutputFloatDataCallback _onOutputFloatData;

        public VectorTypeInfo SourcePixelType { get; private set; }

        public ReadContext Context => Stream.Context;
        public new ImageReadStream Stream => (ImageReadStream)base.Stream;

        public new Image CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int FrameIndex { get => base.FrameIndex; set => base.FrameIndex = value; }

        public StbImageDecoderState(
            IImageDecoder decoder,
            ImagingConfig config,
            ImageReadStream stream) :
            base(decoder, config, stream)
        {
            _onStateReady = OnStateReady;
            _onOutputByteData = OnOutputByteData;
            _onOutputShortData = OnOutputShortData;
            _onOutputFloatData = OnOutputFloatData;
        }

        public ReadState CreateReadState()
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

            return new ReadState(
                _onStateReady,
                _onOutputByteData,
                _onOutputShortData,
                _onOutputFloatData);
        }

        private void OnStateReady(in ReadState state)
        {
            SourcePixelType = GetVectorType(state);
            var dstType = PreferredPixelType ?? SourcePixelType;
            var size = new Size(state.Width, state.Height);
            CurrentImage = Image.Create(dstType, size);
        }

        private void AssertValidStateForOutput()
        {
            if (SourcePixelType == null)
                throw new InvalidOperationException("Missing source pixel type.");

            if (CurrentImage == null)
                throw new InvalidOperationException("Missing image buffer.");
        }

        private void OnOutputByteData(
            in ReadState state, int line, AddressingMajor addressMajor, ReadOnlySpan<byte> pixels)
        {
            AssertValidStateForOutput();

            if (SourcePixelType == CurrentImage.PixelType)
            {
                if (addressMajor == AddressingMajor.Row)
                    CurrentImage.SetPixelByteRow(0, line, pixels);
                else if (addressMajor == AddressingMajor.Column)
                    CurrentImage.SetPixelByteColumn(0, line, pixels);
                else
                    throw new ArgumentOutOfRangeException(nameof(addressMajor));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void OnOutputShortData(
            in ReadState state, int line, AddressingMajor addressMajor, ReadOnlySpan<ushort> pixels)
        {
            AssertValidStateForOutput();

            throw new NotImplementedException();
        }

        private void OnOutputFloatData(
            in ReadState state, int line, AddressingMajor addressMajor, ReadOnlySpan<float> pixels)
        {
            AssertValidStateForOutput();

            throw new NotImplementedException();
        }

        public static VectorTypeInfo GetVectorType(in ReadState state)
        {
            return StbIdentifierBase.GetVectorType(state.Components, state.OutDepth);
        }
    }
}
