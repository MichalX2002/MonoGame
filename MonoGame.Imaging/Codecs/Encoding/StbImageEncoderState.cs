using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Pixels;
using StbSharp;
using static StbSharp.ImageWrite;

namespace MonoGame.Imaging.Codecs.Encoding
{
    public class StbImageEncoderState : ImageEncoderState
    {
        public WriteProgressCallback ProgressCallback { get; }
        private byte[] Buffer { get; }

        public new IReadOnlyPixelRows? CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int FrameIndex { get => base.FrameIndex; set => base.FrameIndex = value; }

        public StbImageEncoderState(
            IImageEncoder encoder,
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken) :
            base(encoder, config, stream, leaveOpen, cancellationToken)
        {
            ProgressCallback = (progress) => InvokeProgress(progress, null);
            Buffer = RecyclableMemoryManager.Default.GetBlock();
        }

        public new void InvokeProgress(double percentage, Rectangle? rectangle)
        {
            base.InvokeProgress(percentage, rectangle);
        }

        public WriteState<TPixelRowProvider> CreateWriteState<TPixelRowProvider>(TPixelRowProvider provider)
            where TPixelRowProvider : IPixelRowProvider
        {
            return new WriteState<TPixelRowProvider>(
                Stream,
                Buffer,
                CancellationToken,
                ProgressCallback,
                provider);
        }

        public override async ValueTask DisposeAsync()
        {
            RecyclableMemoryManager.Default.ReturnBlock(Buffer);
            await base.DisposeAsync();
        }
    }
}
