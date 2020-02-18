// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture2D
    {
        private static ConcurrentDictionary<SurfaceFormat, VectorSaveFormat> SaveFormatsBySurface { get; } =
            new ConcurrentDictionary<SurfaceFormat, VectorSaveFormat>();

        private static ConcurrentDictionary<Type, VectorSaveFormat> SaveFormatsByType { get; } =
            new ConcurrentDictionary<Type, VectorSaveFormat>();

        private static void InitializeVectorSaveFormat()
        {
            // TODO: implement SRgb

            AddVectorSaveFormat(SurfaceFormat.Alpha8, typeof(Alpha8));
            AddVectorSaveFormat(SurfaceFormat.Single, typeof(Gray32));
            // AddSaveFormat(SurfaceFormat.Rgba32SRgb, typeof(Rgba32SRgb));
            AddVectorSaveFormat(SurfaceFormat.Rgba32, typeof(Color));
            AddVectorSaveFormat(SurfaceFormat.Rg32, typeof(Rg32));
            AddVectorSaveFormat(SurfaceFormat.Rgba64, typeof(Rgba64));
            AddVectorSaveFormat(SurfaceFormat.Rgba1010102, typeof(Rgba1010102));
            AddVectorSaveFormat(SurfaceFormat.Bgr565, typeof(Bgr565));
            AddVectorSaveFormat(SurfaceFormat.Bgra5551, typeof(Bgra5551));
            AddVectorSaveFormat(SurfaceFormat.Bgra4444, typeof(Bgra4444));

            // AddSaveFormat(SurfaceFormat.Bgr32SRgb, typeof(Bgr32SRgb));
            // AddSaveFormat(SurfaceFormat.Bgra32SRgb, typeof(Bgra32SRgb));
            AddVectorSaveFormat(SurfaceFormat.Bgr32, typeof(Bgr32));
            AddVectorSaveFormat(SurfaceFormat.Bgra32, typeof(Bgra32));

            AddVectorSaveFormat(SurfaceFormat.HalfSingle, typeof(HalfSingle));
            AddVectorSaveFormat(SurfaceFormat.HalfVector2, typeof(HalfVector2));
            AddVectorSaveFormat(SurfaceFormat.HalfVector4, typeof(HalfVector4));
            AddVectorSaveFormat(SurfaceFormat.Vector2, typeof(Vector2));
            AddVectorSaveFormat(SurfaceFormat.Vector4, typeof(Vector4));
            AddVectorSaveFormat(SurfaceFormat.HdrBlendable, typeof(RgbaVector));

            AddVectorSaveFormat(SurfaceFormat.NormalizedByte2, typeof(NormalizedByte2));
            AddVectorSaveFormat(SurfaceFormat.NormalizedByte4, typeof(NormalizedByte4));
        }

        private static void AddVectorSaveFormat(SurfaceFormat format, Type pixelType)
        {
            var pixelSaveFormat = new VectorSaveFormat(format, pixelType);
            SaveFormatsBySurface.TryAdd(format, pixelSaveFormat);
            SaveFormatsByType.TryAdd(pixelType, pixelSaveFormat);
        }

        public static VectorSaveFormat GetVectorSaveFormat(SurfaceFormat textureFormat)
        {
            if (!SaveFormatsBySurface.TryGetValue(textureFormat, out var pixelFormat))
            {
                var innerException = textureFormat.IsCompressedFormat()
                    ? new NotSupportedException("Compressed texture formats are currently not supported.")
                    : null;

                throw new NotSupportedException(
                    $"The format {textureFormat} is not supported.", innerException);
            }
            return pixelFormat;
        }

        public static VectorSaveFormat GetVectorSaveFormat(Type vectorType)
        {
            if (!SaveFormatsByType.TryGetValue(vectorType, out var pixelFormat))
            {
                throw new NotSupportedException(
                    $"The vector type {vectorType} is not supported.");
            }
            return pixelFormat;
        }

        public class VectorSaveFormat
        {
            private static MethodInfo _getDataMethod;

            public delegate IMemory GetDataDelegate(Texture2D texture, Rectangle? rectangle, int level, int arraySlice);

            public SurfaceFormat Format { get; }
            public VectorTypeInfo VectorType { get; }

            public GetDataDelegate GetData { get; }

            static VectorSaveFormat()
            {
                var methodParams = typeof(GetDataDelegate).GetDelegateParameters().Skip(1);
                _getDataMethod = typeof(Texture2D).GetMethod("GetData", methodParams.Select(x => x.ParameterType).ToArray());
            }

            public VectorSaveFormat(SurfaceFormat format, Type vectorType)
            {
                if (vectorType == null)
                    throw new ArgumentNullException(nameof(vectorType));

                Format = format;
                VectorType = VectorTypeInfo.Get(vectorType);
                GetData = GetGetDataDelegate(vectorType);
            }

            private static GetDataDelegate GetGetDataDelegate(Type vectorType)
            {
                var genericMethod = _getDataMethod.MakeGenericMethod(vectorType);
                return genericMethod.CreateDelegate<GetDataDelegate>();
            }
        }
    }
}
