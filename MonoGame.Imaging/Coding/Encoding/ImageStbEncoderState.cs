using System;
using System.IO;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class ImageStbEncoderState : ImageEncoderState
    {
        public new IReadOnlyPixelRows CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int ImageIndex { get => base.ImageIndex; set => base.ImageIndex = value; }

        private byte[] Buffer { get; set; }
        public Memory<byte> ScratchBuffer => Buffer;

        public ImageStbEncoderState(
            ImagingConfig config,
            IImageEncoder encoder,
            Stream stream,
            bool leaveOpen) :
            base(config, encoder, stream, leaveOpen)
        {
            Buffer = RecyclableMemoryManager.Default.GetBlock();
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
