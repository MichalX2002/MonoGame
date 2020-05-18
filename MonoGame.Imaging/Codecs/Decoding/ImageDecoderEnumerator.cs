using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MonoGame.Imaging.Codecs.Decoding
{
    public class ImageDecoderEnumerator : IEnumerator<Image>, IImagingConfigurable
    {
        public ImageDecoderState State { get; }
        public IImagingConfig Config => State.Config;

        public Image Current => State.CurrentImage!;
        object? IEnumerator.Current => Current;

        #region Constructors

        public ImageDecoderEnumerator(
            IImagingConfig config,
            IImageDecoder decoder,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            if (decoder == null) 
                throw new ArgumentNullException(nameof(decoder));

            State = decoder.CreateState(config, stream, leaveOpen, cancellationToken);
        }

        #endregion

        public bool MoveNext()
        {
            State.Decoder.Decode(State);

            if (State.CurrentImage == null)
                throw new Exception("The decoder did not output an image.");

            return true;
        }

        public void Reset()
        {
            // consider implementation for seekable streams
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            State.Dispose();
        }
    }
}
