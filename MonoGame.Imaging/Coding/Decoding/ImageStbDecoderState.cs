using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Coding.Identification;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageStbDecoderState : ImageDecoderState
    {
        public new ImageReadStream Stream => (ImageReadStream)base.Stream;
        public ReadContext Context => Stream.Context;

        public VectorTypeInfo PreferredPixelType { get; set; }
        public VectorTypeInfo SourcePixelType { get; set; }

        public new Image CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int ImageIndex { get => base.ImageIndex; set => base.ImageIndex = value; }

        private StateReadyCallback _onStateReady;
        private OutputByteDataCallback _onOutputByteData;

        public ImageStbDecoderState(
            ImagingConfig imagingConfig,
            IImageDecoder decoder,
            ImageReadStream stream) :
            base(imagingConfig, decoder, stream)
        {
            _onStateReady = OnStateReady;
            _onOutputByteData = OnOutputByteData;
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
                null);
        }

        private void OnStateReady(in ReadState state)
        {
            SourcePixelType = GetVectorType(state);
            var dstType = PreferredPixelType ?? SourcePixelType;
            var size = new Size(state.Width, state.Height);
            CurrentImage = Image.Create(dstType, size);
        }

        private void OnOutputByteData(
            in ReadState state, int line, AddressingMajor addressMajor, ReadOnlySpan<byte> pixels)
        {
            if (SourcePixelType == null)
                throw new InvalidOperationException("Missing source pixel type.");
            if (CurrentImage == null)
                throw new InvalidOperationException("Missing image buffer.");

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

        private void OnOutputFloatData(
            in ReadState state, int line, AddressingMajor addressMajor, ReadOnlySpan<float> pixels)
        {
        }

        public static VectorTypeInfo GetVectorType(in ReadState state)
        {
            return StbIdentifierBase.GetVectorType(state.Components, state.OutDepth);
        }
    }
}
