using System;
using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Coders.Detection;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Decoding
{
    public class StbImageDecoderState : ImageDecoderState
    {
        private StateReadyDelegate _onStateReady;
        private OutputPixelLineDelegate _onOutputPixelLine;
        private OutputPixelDelegate _onOutputPixel;

        private Image.ConvertPixelDataDelegate? _convertPixels;
        private byte[]? Buffer { get; set; }

        public VectorType? SourcePixelType { get; private set; }
        public BinReader Reader { get; }

        public new Image? CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int FrameIndex { get => base.FrameIndex; set => base.FrameIndex = value; }

        public StbImageDecoderState(
            IImageDecoder decoder,
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken) :
            base(decoder, config, stream, leaveOpen, cancellationToken)
        {
            _onStateReady = OnStateReady;
            _onOutputPixelLine = OnOutputPixelLine;
            _onOutputPixel = OnOutputPixel;

            Buffer = RecyclableMemoryManager.Default.GetBlock();
            Reader = new BinReader(Stream, Buffer, leaveOpen: true, cancellationToken);
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

            if (GetCoderOptionsOrDefault<DecoderOptions>().ClearImageMemory)
                CurrentImage = Image.Create(dstType, size);
            else
                CurrentImage = Image.CreateUninitialized(dstType, size);

            _convertPixels = Image.GetConvertPixelsDelegate(SourcePixelType, dstType);

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
            if (SourcePixelType == CurrentImage!.PixelType)
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
                    _convertPixels!.Invoke(pixels, dstSpan);
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
            ReadState state, AddressingMajor addressing,
            int line, int start, int spacing, ReadOnlySpan<byte> pixelData)
        {
            if (spacing == 0)
                throw new ArgumentOutOfRangeException(nameof(spacing));

            AssertValidStateForOutput();

            if (HasProgressListener)
                InvokeProgress(0, new Rectangle(new Point(start, line), CurrentImage!.Size));

            if (spacing == 1)
            {
                OnOutputPixelLineContiguous(addressing, line, start, pixelData);
                return;
            }

            int dstElementSize = CurrentImage!.PixelType.ElementSize;

            if (SourcePixelType == CurrentImage.PixelType)
            {
                if (addressing == AddressingMajor.Row)
                {
                    var imageRow = CurrentImage.GetPixelByteRowSpan(line).Slice(start * dstElementSize);
                    for (int x = 0; x < pixelData.Length; x += dstElementSize)
                    {
                        var src = pixelData.Slice(x, dstElementSize);
                        var dst = imageRow.Slice(x * spacing, dstElementSize);
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

                if (addressing == AddressingMajor.Row)
                {
                    Span<byte> imageRow = CurrentImage.GetPixelByteRowSpan(line).Slice(start * dstElementSize);
                    Span<byte> buffer = stackalloc byte[4096];
                    int srcElementSize = SourcePixelType!.ElementSize;
                    int bufferCapacity = buffer.Length / dstElementSize;
                    int pixelCount = pixelData.Length / srcElementSize;
                    int dstOffset = 0;

                    do
                    {
                        int count = pixelCount > bufferCapacity ? bufferCapacity : pixelCount;
                        var slice = pixelData.Slice(0, count * srcElementSize);

                        _convertPixels!.Invoke(slice, buffer);

                        int bufOffset = 0;
                        for (int x = 0; x < slice.Length; x += srcElementSize)
                        {
                            var src = buffer.Slice(bufOffset, dstElementSize);
                            var dst = imageRow.Slice(dstOffset, dstElementSize);
                            src.CopyTo(dst);

                            bufOffset += dstElementSize;
                            dstOffset += dstElementSize * spacing;
                        }

                        pixelData = pixelData.Slice(slice.Length);
                        pixelCount -= count;

                    } while (!pixelData.IsEmpty);
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
        }

        private void OnOutputPixel(ReadState state, int x, int y, ReadOnlySpan<byte> pixel)
        {
            AssertValidStateForOutput();

            if (SourcePixelType == CurrentImage!.PixelType)
            {
                CurrentImage.SetPixelByteRow(x, y, pixel);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static VectorType GetVectorType(ReadState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            return StbImageInfoDetectorBase.GetVectorType(state.OutComponents, state.OutDepth);
        }

        protected override void Dispose(bool disposing)
        {
            if (Buffer != null)
            {
                RecyclableMemoryManager.Default.ReturnBlock(Buffer);
                Buffer = null;
            }

            if (disposing)
            {
                Reader.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
