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
        private OutputByteRowCallback _onOutputByteRow;

        public ImageStbDecoderState(
            ImagingConfig imagingConfig,
            IImageDecoder decoder,
            ImageReadStream stream) :
            base(imagingConfig, decoder, stream)
        {
            _onStateReady = OnStateReady;
            _onOutputByteRow = OnOutputByteRow;
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
                _onOutputByteRow,
                null,
                null);
        }

        private void OnStateReady(in ReadState state)
        {
            SourcePixelType = GetVectorType(state);
            var dstType = PreferredPixelType ?? SourcePixelType;
            var size = new Size(state.Width, state.Height);
            CurrentImage = Image.Create(dstType, size);
        }

        private void OnOutputByteRow(in ReadState state, int row, ReadOnlySpan<byte> pixelRow)
        {
            if (SourcePixelType == null)
                throw new InvalidOperationException("Missing source pixel type.");
            if (CurrentImage == null)
                throw new InvalidOperationException("Missing image buffer.");

            if (SourcePixelType == CurrentImage.PixelType)
            {
                CurrentImage.SetPixelByteRow(0, row, pixelRow);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static VectorTypeInfo GetVectorType(in ReadState state)
        {
            return StbIdentifierBase.GetVectorType(state.Components, state.OutDepth);
        }
    }
}
