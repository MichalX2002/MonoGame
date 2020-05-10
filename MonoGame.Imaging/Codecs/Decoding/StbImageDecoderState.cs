using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vector;
using MonoGame.Imaging.Codecs.Detection;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Codecs.Decoding
{
    public class StbImageDecoderState : ImageDecoderState
    {
        private StateReadyDelegate _onStateReady;
        private OutputPixelLineDelegate _onOutputPixelLine;
        private OutputPixelDelegate _onOutputPixel;

        private Image.ConvertPixelsDelegate? _convertPixelSpan;
        private byte[] Buffer { get; }

        public VectorTypeInfo? SourcePixelType { get; private set; }
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

            if (GetCodecOptions<DecoderOptions>().ClearImageMemory)
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
                    _convertPixelSpan!.Invoke(pixels, dstSpan);
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
            int line, int start, int spacing, ReadOnlySpan<byte> pixels)
        {
            if (spacing == 0)
                throw new ArgumentOutOfRangeException(nameof(spacing));

            AssertValidStateForOutput();

            if (HasProgressListener)
            {
                InvokeProgress(0, new Rectangle(new Point(start, line), CurrentImage!.Size));
            }

            if (spacing == 1)
            {
                OnOutputPixelLineContiguous(addressing, line, start, pixels);
                return;
            }

            int elementSize = CurrentImage!.PixelType.ElementSize;

            if (SourcePixelType == CurrentImage.PixelType)
            {
                if (addressing == AddressingMajor.Row)
                {
                    var imageRow = CurrentImage.GetPixelByteRowSpan(line).Slice(start * elementSize);
                    for (int x = 0; x < pixels.Length; x += elementSize)
                    {
                        var src = pixels.Slice(x, elementSize);
                        var dst = imageRow.Slice(x * spacing, elementSize);
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
                    try
                    {
                        // Create buffer with size aligned to element size.
                        Span<byte> buffer = stackalloc byte[2048 / elementSize * elementSize];
                        var imageRow = CurrentImage.GetPixelByteRowSpan(line).Slice(start * elementSize);
                        int srcElementSize = SourcePixelType!.ElementSize;

                        while (pixels.Length > 0)
                        {
                            var pixelSlice = pixels.Length > buffer.Length
                                ? pixels.Slice(0, buffer.Length)
                                : pixels;

                            Image.ConvertPixelBytes(
                                SourcePixelType!, CurrentImage.PixelType, pixelSlice, buffer);

                            int dstOffset = 0;
                            for (int x = 0; x < pixelSlice.Length; x += srcElementSize)
                            {
                                var src = buffer.Slice(dstOffset, elementSize);
                                var dst = imageRow.Slice(dstOffset * spacing, elementSize);
                                src.CopyTo(dst);

                                dstOffset += elementSize;
                            }

                            pixels = pixels.Slice(pixelSlice.Length);
                            imageRow = imageRow.Slice(dstOffset);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
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

        public static VectorTypeInfo GetVectorType(ReadState state)
        {
            return StbImageInfoDetectorBase.GetVectorType(state.OutComponents, state.OutDepth);
        }

        public override async ValueTask DisposeAsync()
        {
            RecyclableMemoryManager.Default.ReturnBlock(Buffer);

            await Reader.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
