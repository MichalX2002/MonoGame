using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public abstract class StbIdentifierBase : IImageIdentifier
    {
        public abstract ImageFormat Format { get; }

        public virtual CodecOptions DefaultOptions => CodecOptions.Default;

        public abstract int HeaderSize { get; }

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

        #region DetectFormat Abstraction

        protected abstract bool TestFormat(ImagingConfig config, ReadOnlySpan<byte> header);

        public ImageFormat DetectFormat(ImagingConfig config, ReadOnlySpan<byte> header)
        {
            if (TestFormat(config, header))
                return Format;
            return null;
        }

        #endregion

        #region Identify Abstraction

        protected abstract bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState);

        private ImageInfo Identify(ImagingConfig config, ReadContext context)
        {
            if (GetInfo(config, context, out var readState))
            {
                int comp = readState.Components;
                int bitsPerComp = readState.Depth;

                var vectorType = GetVectorType(comp, bitsPerComp);

                var compInfo = vectorType?.ComponentInfo ??
                    new VectorComponentInfo(new VectorComponent(VectorComponentType.Raw, comp * bitsPerComp));

                return new ImageInfo(readState.Width, readState.Height, compInfo, Format);
            }
            return default;
        }

        public ImageInfo Identify(ImagingConfig config, ImageReadStream stream)
        {
            return Identify(config, stream.Context);
        }

        #endregion
    }
}
