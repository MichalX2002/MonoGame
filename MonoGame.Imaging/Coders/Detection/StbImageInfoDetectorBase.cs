using System;
using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Coders.Decoding;
using StbSharp;

namespace MonoGame.Imaging.Coders.Detection
{
    public abstract class StbImageInfoDetectorBase : IImageInfoDetector
    {
        public class InfoResult
        {
            public ImageRead.ReadState ReadState { get; }

            public InfoResult(ImageRead.ReadState readState)
            {
                ReadState = readState ?? throw new ArgumentNullException(nameof(readState));
            }
        }

        public CoderOptions DefaultOptions => CoderOptions.Default;

        public abstract ImageFormat Format { get; }

        protected abstract InfoResult GetInfo(IImagingConfig config, ImageRead.BinReader reader);

        private ImageInfo Identify(IImagingConfig config, ImageRead.BinReader reader)
        {
            var info = GetInfo(config, reader);
            var readState = info.ReadState;

            int comp = readState.Components;
            int bitsPerComp = readState.Depth;

            var vectorType = TryGetVectorType(comp, bitsPerComp);
            var compInfo = vectorType.Type?.ComponentInfo ?? new VectorComponentInfo(new VectorComponent(
                VectorComponentType.Undefined, VectorComponentChannel.Raw, comp * bitsPerComp));

            return new ImageInfo(readState.Width, readState.Height, compInfo, Format);
        }

        public ImageInfo Identify(
            IImagingConfig config, Stream stream, CancellationToken cancellationToken = default)
        {
            var buffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                using var reader = new ImageRead.BinReader(stream, buffer, leaveOpen: true, cancellationToken);
                return Identify(config, reader);
            }
            finally
            {
                RecyclableMemoryManager.Default.ReturnBlock(buffer);
            }
        }

        public static VectorType GetVectorType(int components, int depth)
        {
            var tuple = TryGetVectorType(components, depth);

            if(!tuple.Depth)
                throw new ArgumentOutOfRangeException(
                    nameof(depth), $"Given depth of {depth} is not supported.");

            if (!tuple.Components)
                throw new ArgumentOutOfRangeException(
                    nameof(components),
                    $"Given component count of {components} is not supported.");

            return tuple.Type!;
        }

        public static (bool Depth, bool Components, VectorType? Type)
            TryGetVectorType(int components, int depth)
        {
            // Note: 32bit depth means floating-point

            (bool Depth, bool Components, VectorType? Type) tuple = (true, true, null);

            switch (components)
            {
                case 1:
                    tuple.Type =
                        depth == 8 ? VectorType.Get<Gray8>() :
                        depth == 16 ? VectorType.Get<Gray16>() :
                        depth == 32 ? VectorType.Get<GrayF>() :
                        null;
                    break;

                case 2:
                    tuple.Type =
                        depth == 8 ? VectorType.Get<GrayAlpha16>() :
                        depth == 16 ? VectorType.Get<GrayAlpha32>() :
                        //TODO: depth == 32 ? VectorTypeInfo.Get<?>() :
                        null;
                    break;

                case 3:
                    tuple.Type =
                        depth == 8 ? VectorType.Get<Rgb24>() :
                        depth == 16 ? VectorType.Get<Rgb48>() :
                        depth == 32 ? VectorType.Get<RgbVector>() :
                        null;
                    break;

                case 4:
                    tuple.Type =
                        depth == 8 ? VectorType.Get<Color>() :
                        depth == 16 ? VectorType.Get<Rgba64>() :
                        depth == 32 ? VectorType.Get<RgbaVector>() :
                        null;
                    break;

                default:
                    tuple.Components = false;
                    break;
            }

            if (tuple.Type == null)
                tuple.Depth = false;

            return tuple;
        }
    }
}
