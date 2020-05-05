using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Codecs.Decoding;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Detection
{
    public abstract class StbImageInfoDetectorBase : IImageInfoDetector
    {
        public readonly struct InfoResult
        {
            public ImageRead.ReadState ReadState { get; }

            public InfoResult(ImageRead.ReadState readState)
            {
                ReadState = readState ?? throw new ArgumentNullException(nameof(readState));
            }
        }

        public CodecOptions DefaultOptions => CodecOptions.Default;

        public abstract ImageFormat Format { get; }

        protected abstract Task<InfoResult> GetInfo(IImagingConfig config, ImageRead.BinReader reader);

        private async Task<ImageInfo> Identify(IImagingConfig config, ImageRead.BinReader reader)
        {
            var info = await GetInfo(config, reader);
            var readState = info.ReadState;

            int comp = readState.Components;
            int bitsPerComp = readState.Depth;

            var vectorType = GetVectorType(comp, bitsPerComp);
            var compInfo = vectorType?.ComponentInfo ??
                new VectorComponentInfo(new VectorComponent(VectorComponentType.Raw, comp * bitsPerComp));

            return new ImageInfo(readState.Width, readState.Height, compInfo, Format);
        }

        public async Task<ImageInfo> Identify(
            IImagingConfig config, Stream stream, CancellationToken cancellationToken = default)
        {
            var reader = new ImageRead.BinReader(stream, leaveOpen: true, cancellationToken);
            return await Identify(config, reader);
        }

        public static VectorTypeInfo GetVectorType(int components, int depth)
        {
            Exception CreateDepthNotSupportedException()
            {
                return new ArgumentOutOfRangeException(
                    nameof(depth), $"Given depth of {depth} is not supported.");
            }

            switch (components)
            {
                case 1:
                    return
                        depth == 8 ? VectorTypeInfo.Get<Gray8>() :
                        depth == 16 ? VectorTypeInfo.Get<Gray16>() :
                        depth == 32 ? VectorTypeInfo.Get<Gray32>() :
                        throw CreateDepthNotSupportedException();

                case 2:
                    return
                        depth == 8 ? VectorTypeInfo.Get<GrayAlpha16>() :
                        depth == 16 ? VectorTypeInfo.Get<GrayAlpha32>() :
                        //depth == 32 ? VectorTypeInfo.Get<Vector2>() :
                        throw CreateDepthNotSupportedException();

                case 3:
                    return
                        depth == 8 ? VectorTypeInfo.Get<Rgb24>() :
                        depth == 16 ? VectorTypeInfo.Get<Rgb48>() :
                        depth == 32 ? VectorTypeInfo.Get<Vector3>() : // TODO: RgbVector
                        throw CreateDepthNotSupportedException();

                case 4:
                    return
                        depth == 8 ? VectorTypeInfo.Get<Color>() :
                        depth == 16 ? VectorTypeInfo.Get<Rgba64>() :
                        depth == 32 ? VectorTypeInfo.Get<RgbaVector>() :
                        throw CreateDepthNotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException(
                        $"Given component count of {components} is not supported.",
                        nameof(components));
            }
        }
    }
}
