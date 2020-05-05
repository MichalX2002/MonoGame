using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using static StbSharp.ImageWrite;

namespace MonoGame.Imaging.Codecs.Encoding
{
    public class StbImageEncoderState : ImageEncoderState
    {
        public WriteProgressCallback ProgressCallback { get; }

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
        }

        public new void InvokeProgress(double percentage, Rectangle? rectangle)
        {
            base.InvokeProgress(percentage, rectangle);
        }
    }
}
