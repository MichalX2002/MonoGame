using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Coding.Identification;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class StbImageDecoderState : ImageDecoderState
    {
        private StateReadyDelegate _onStateReady;
        private OutputPixelLineDelegate _onOutputPixelLine;
        private OutputPixelDelegate _onOutputPixel;

        private Image.ConvertPixelsDelegate _convertPixelSpan;

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
            _onOutputPixelLine = OnOutputPixelLine;
            _onOutputPixel = OnOutputPixel;
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

            return new ReadState()
            {
                StateReadyCallback = _onStateReady,
                OutputPixelLineCallback = _onOutputPixelLine,
                OutputPixelCallback = _onOutputPixel
            };
        }

        private void OnStateReady(ReadState state)
        {
            SourcePixelType = GetVectorType(state);
            var dstType = PreferredPixelType ?? SourcePixelType;
            var size = new Size(state.Width, state.Height);

            if (DecoderOptions.ClearImageMemory)
                CurrentImage = Image.Create(dstType, size);
            else
                CurrentImage = Image.CreateUninitialized(dstType, size);

            _convertPixelSpan = Image.GetConvertPixelsDelegate(SourcePixelType, dstType);

            InvokeProgress(0, null);
        }

        private void AssertValidStateForOutput()
        {
            if (SourcePixelType == null)
                throw new InvalidOperationException("Missing source pixel type.");

            if (CurrentImage == null)
                throw new InvalidOperationException("Missing image buffer.");
        }

        private void OnOutputPixelLineContiguous(
            AddressingMajor addressing, int line, int start, ReadOnlySpan<byte> pixels)
        {
            AssertValidStateForOutput();

            if (SourcePixelType == CurrentImage.PixelType)
            {
                if (addressing == AddressingMajor.Row)
                    CurrentImage.SetPixelByteRow(start, line, pixels);
                else if (addressing == AddressingMajor.Column)
                    CurrentImage.SetPixelByteColumn(start, line, pixels);
                else
                    throw new ArgumentOutOfRangeException(nameof(addressing));
            }
            else
            {
                if (addressing == AddressingMajor.Row)
                {
                    var dstSpan = CurrentImage.GetPixelByteRowSpan(line);
                    _convertPixelSpan.Invoke(pixels, dstSpan);
                }
                else if (addressing == AddressingMajor.Column)
                {
                    throw new NotImplementedException();
                    CurrentImage.SetPixelByteColumn(start, line, pixels);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(addressing));
                }
            }
        }

        private void OnOutputPixelLine(
            ReadState state, AddressingMajor addressing, int line, int start, int spacing, ReadOnlySpan<byte> pixels)
        {
            if (spacing == 1)
            {
                OnOutputPixelLineContiguous(addressing, line, start, pixels);
                return;
            }

            if (spacing == 0)
                throw new ArgumentOutOfRangeException(nameof(spacing));

            AssertValidStateForOutput();

            if (SourcePixelType == CurrentImage.PixelType)
            {
                int elementSize = SourcePixelType.ElementSize;

                if (addressing == AddressingMajor.Row)
                {
                    var byteRow = CurrentImage.GetPixelByteRowSpan(line).Slice(start * elementSize);
                    for (int x = 0; x < pixels.Length; x += elementSize)
                    {
                        var src = pixels.Slice(x, elementSize);
                        var dst = byteRow.Slice(x * spacing, elementSize);
                        src.CopyTo(dst);
                    }
                }
                else if (addressing == AddressingMajor.Column)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(addressing));
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void OnOutputPixel(ReadState state, int x, int y, ReadOnlySpan<byte> pixel)
        {
            AssertValidStateForOutput();

            if (SourcePixelType == CurrentImage.PixelType)
            {
                CurrentImage.SetPixelByteRow(x, y, pixel);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static VectorTypeInfo GetVectorType(ReadState state)
        {
            return StbIdentifierBase.GetVectorType(state.OutComponents, state.OutDepth);
        }
    }
}
