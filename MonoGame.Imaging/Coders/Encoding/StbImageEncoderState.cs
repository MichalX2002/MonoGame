using System.IO;
using System.Threading;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Utilities;
using StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coders.Encoding
{
    public class StbImageEncoderState : ImageEncoderState
    {
        private byte[]? _buffer;

        public WriteProgressCallback ProgressCallback { get; }
        public WriteState WriteState { get; private set; }

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
            ProgressCallback = (progress, rect) => InvokeProgress(progress, rect?.ToMGRect());
            
            _buffer = RecyclableMemoryManager.Default.GetBlock();

            WriteState = new WriteState(
                Stream,
                _buffer,
                ProgressCallback,
                CancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            WriteState?.Dispose();
            WriteState = null!;

            RecyclableMemoryManager.Default.ReturnBlock(_buffer);
            _buffer = null;

            base.Dispose(disposing);
        }
    }
}
