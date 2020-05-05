using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGame.Imaging.Codecs.Decoding
{
    public class ImageDecoderEnumerator : IAsyncEnumerator<Image>, IImagingConfigurable
    {
        public ImageDecoderState State { get; }
        public IImagingConfig Config => State.Config;

        public Image Current => State.CurrentImage!;

        #region Constructors

        public ImageDecoderEnumerator(
            IImagingConfig config,
            IImageDecoder decoder,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            if (decoder == null) throw new ArgumentNullException(nameof(decoder));

            State = decoder.CreateState(config, stream, leaveOpen, cancellationToken);
        }

        #endregion

        public async ValueTask<bool> MoveNextAsync()
        {
            if (await State.Decoder.Decode(State))
            {
                if (State.CurrentImage == null)
                    throw new Exception(
                        "The decoder reported a successful decode, but there is no resulting image.");

                return true;
            }

            return false;
        }

        public ValueTask DisposeAsync()
        {
            return State.DisposeAsync();
        }
    }
}
