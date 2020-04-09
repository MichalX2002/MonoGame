using System;
using System.IO;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Pixels;
using static StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class StbImageEncoderState : ImageEncoderState
    {
        private byte[] Buffer { get; set; }

        public WriteProgressCallback ProgressCallback { get; }

        public Memory<byte> ScratchBuffer => Buffer;
        public new IReadOnlyPixelRows CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int ImageIndex { get => base.ImageIndex; set => base.ImageIndex = value; }

        public StbImageEncoderState(
            ImagingConfig config,
            IImageEncoder encoder,
            Stream stream,
            bool leaveOpen) :
            base(config, encoder, stream, leaveOpen)
        {
            Buffer = RecyclableMemoryManager.Default.GetBlock();
            ProgressCallback = (progress) => InvokeProgress(progress, null);
        }

        public new void InvokeProgress(double percentage, Rectangle? rectangle)
        {
            base.InvokeProgress(percentage, rectangle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Buffer != null)
            {
                RecyclableMemoryManager.Default.ReturnBlock(Buffer);
                Buffer = null;
            }
        }
    }
}
