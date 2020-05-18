using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGame.Imaging.Codecs
{
    public abstract class ImageCodecState : IImagingConfigurable, IDisposable
    {
        public IImageCodec Codec { get; }
        public IImagingConfig Config { get; }
        public Stream Stream { get; private set; }
        public bool LeaveOpen { get; }
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets the zero-based index of the most recently processed image.
        /// </summary>
        public int FrameIndex { get; protected set; }

        public CodecOptions? CodecOptions { get; set; }

        public ImageCodecState(
            IImageCodec codec,
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken)
        {
            Codec = codec ?? throw new ArgumentNullException(nameof(codec));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Stream = stream ?? throw new ArgumentNullException(nameof(config));
            LeaveOpen = leaveOpen;
            CancellationToken = cancellationToken;

            CodecOptions = Codec.DefaultOptions;
        }

        public TOptions GetCodecOptions<TOptions>()
            where TOptions : CodecOptions
        {
            if (CodecOptions != null)
                if (Codec.DefaultOptions.IsAssignableFrom(CodecOptions))
                    return (TOptions)CodecOptions;

            return (TOptions)Codec.DefaultOptions;
        }

        public virtual void Dispose()
        {
            if (!LeaveOpen)
                Stream.Dispose();
        }
    }
}
