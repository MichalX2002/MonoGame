using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public abstract class StbIdentifierBase : IImageIdentifier
    {
        public abstract ImageFormat Format { get; }

        public static VectorTypeInfo CompToVectorType(int comp, int compDepth)
        {
            switch (comp)
            {
                case 1:
                    return compDepth == 16 ? VectorTypeInfo.Get<Gray16>() : VectorTypeInfo.Get<Gray8>();

                case 2:
                    if (compDepth == 16)
                        throw new NotSupportedException("16-bit gray and alpha color is not supported.");
                    return VectorTypeInfo.Get<GrayAlpha16>();

                case 3:
                    return compDepth == 16 ? VectorTypeInfo.Get<Rgb48>() : VectorTypeInfo.Get<Rgb24>();

                case 4:
                    return compDepth == 16 ? VectorTypeInfo.Get<Rgba64>() : VectorTypeInfo.Get<Color>();

                default:
                    return default;
            }
        }

        #region DetectFormat Abstraction

        protected abstract bool TestFormat(ImagingConfig config, ReadContext context);

        private ImageFormat DetectFormat(ImagingConfig config, ReadContext context)
        {
            if (TestFormat(config, context))
                return Format;
            return default;
        }

        public ImageFormat DetectFormat(ImagingConfig config, ImageReadStream stream)
        {
            return DetectFormat(config, stream.Context);
        }

        #endregion

        #region Identify Abstraction

        protected abstract bool GetInfo(ImagingConfig config, ReadContext context, out ReadState readState);

        private ImageInfo Identify(ImagingConfig config, ReadContext context)
        {
            if (GetInfo(config, context, out var readState))
            {
                int comp = readState.OutComponents;
                int bitsPerComp = readState.OutDepth;

                var vectorType = CompToVectorType(comp, bitsPerComp);
                var compInfo = vectorType != null
                    ? vectorType.ComponentInfo :
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
