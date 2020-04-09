// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Framework.Collections;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture2D
    {
        private static Dictionary<SurfaceFormat, HashSet<VectorFormat>> VectorFormatsBySurface { get; } =
            new Dictionary<SurfaceFormat, HashSet<VectorFormat>>();

        private static ConcurrentDictionary<SurfaceFormat, ReadOnlySet<VectorFormat>> VectorFormatsBySurfaceRO { get; } =
            new ConcurrentDictionary<SurfaceFormat, ReadOnlySet<VectorFormat>>();

        private static Dictionary<Type, HashSet<VectorFormat>> VectorFormatsByType { get; } =
            new Dictionary<Type, HashSet<VectorFormat>>();

        private static ConcurrentDictionary<Type, ReadOnlySet<VectorFormat>> VectorFormatsByTypeRO { get; } =
            new ConcurrentDictionary<Type, ReadOnlySet<VectorFormat>>();

        private static void InitializeVectorFormats()
        {
            // TODO: implement SRgb

            AddVectorFormat(SurfaceFormat.Alpha8, typeof(Alpha8));
            AddVectorFormat(SurfaceFormat.Single, typeof(Gray32));
            //AddVectorFormat(SurfaceFormat.Rgba32SRgb, typeof(Rgba32SRgb));
            AddVectorFormat(SurfaceFormat.Color, typeof(Color), typeof(Byte4), typeof(NormalizedByte4));
            AddVectorFormat(SurfaceFormat.Rg32, typeof(Rg32));
            AddVectorFormat(SurfaceFormat.Rgba64, typeof(Rgba64));
            AddVectorFormat(SurfaceFormat.Rgba1010102, typeof(Rgba1010102));
            AddVectorFormat(SurfaceFormat.Bgr565, typeof(Bgr565));
            AddVectorFormat(SurfaceFormat.Bgra5551, typeof(Bgra5551));
            AddVectorFormat(SurfaceFormat.Bgra4444, typeof(Bgra4444));

            //AddVectorFormat(SurfaceFormat.Bgr32SRgb, typeof(Bgr32SRgb));
            //AddVectorFormat(SurfaceFormat.Bgra32SRgb, typeof(Bgra32SRgb));
            AddVectorFormat(SurfaceFormat.Bgr32, typeof(Bgr32));
            AddVectorFormat(SurfaceFormat.Bgra32, typeof(Bgra32));

            AddVectorFormat(SurfaceFormat.HalfSingle, typeof(HalfSingle));
            AddVectorFormat(SurfaceFormat.HalfVector2, typeof(HalfVector2));
            AddVectorFormat(SurfaceFormat.HalfVector4, typeof(HalfVector4));
            AddVectorFormat(SurfaceFormat.Vector2, typeof(Vector2));
            AddVectorFormat(SurfaceFormat.Vector4, typeof(Vector4), typeof(RgbaVector));
            AddVectorFormat(SurfaceFormat.HdrBlendable, typeof(RgbaVector), typeof(Vector4));

            AddVectorFormat(SurfaceFormat.NormalizedByte2, typeof(NormalizedByte2));
            AddVectorFormat(SurfaceFormat.NormalizedByte4, typeof(NormalizedByte4), typeof(Byte4), typeof(Color));
        }

        private static void AddVectorFormat(SurfaceFormat surfaceFormat, params Type[] vectorTypes)
        {
            var vectorInfos = vectorTypes.Select(x => VectorTypeInfo.Get(x));
            var vectorFormat = new VectorFormat(surfaceFormat, vectorInfos);

            lock (VectorFormatsBySurface)
            {
                if (!VectorFormatsBySurface.TryGetValue(surfaceFormat, out var set))
                {
                    set = new HashSet<VectorFormat>();
                    VectorFormatsBySurface.Add(surfaceFormat, set);
                    VectorFormatsBySurfaceRO.TryAdd(surfaceFormat, set.AsReadOnly());
                }
                set.Add(vectorFormat);
            }

            lock (VectorFormatsByType)
            {
                foreach (var vectorType in vectorTypes)
                {
                    if (!VectorFormatsByType.TryGetValue(vectorType, out var set))
                    {
                        set = new HashSet<VectorFormat>();
                        VectorFormatsByType.Add(vectorType, set);
                        VectorFormatsByTypeRO.TryAdd(vectorType, set.AsReadOnly());
                    }
                    set.Add(vectorFormat);
                }
            }
        }

        private static Exception UnsupportedSurfaceFormatForImagingException(
            SurfaceFormat surfaceFormat, string paramName)
        {
            var innerException = surfaceFormat.IsCompressedFormat()
                ? new ArgumentException("Compressed texture formats are currently not supported.")
                : null;

            return new ArgumentException(
                $"The format {surfaceFormat} is not supported.", paramName, innerException);
        }

        public static ReadOnlySet<VectorFormat> GetVectorFormats(SurfaceFormat surfaceFormat)
        {
            if (!VectorFormatsBySurfaceRO.TryGetValue(surfaceFormat, out var formatSet))
                throw UnsupportedSurfaceFormatForImagingException(surfaceFormat, nameof(surfaceFormat));
            return formatSet;
        }

        public static ReadOnlySet<VectorFormat> GetVectorFormats(Type vectorType)
        {
            if (!VectorFormatsByTypeRO.TryGetValue(vectorType, out var formatSet))
                throw new NotSupportedException($"The vector type {vectorType} is not supported.");
            return formatSet;
        }

        public static VectorFormat GetVectorFormat(SurfaceFormat surfaceFormat)
        {
            return GetVectorFormats(surfaceFormat).FirstOrDefault();
        }

        public static VectorFormat GetVectorFormat(Type vectorType)
        {
            return GetVectorFormats(vectorType).FirstOrDefault();
        }

        public class VectorFormat
        {
            private static MethodInfo _getDataMethod;

            public delegate IMemory GetDataDelegate(
                Texture2D texture, Rectangle? rectangle, int level, int arraySlice);

            public SurfaceFormat SurfaceFormat { get; }

            /// <summary>
            /// Gets structurally equal vector types that
            /// represent this <see cref="VectorFormat"/>.
            /// </summary>
            public ReadOnlyMemory<VectorTypeInfo> VectorTypes { get; }

            public GetDataDelegate GetData { get; }

            static VectorFormat()
            {
                var methodParams = typeof(GetDataDelegate).GetDelegateParameters().Skip(1);
                _getDataMethod = typeof(Texture2D).GetMethod(
                    "GetData", methodParams.Select(x => x.ParameterType).ToArray());
            }

            public VectorFormat(SurfaceFormat surfaceFormat, IEnumerable<VectorTypeInfo> vectorTypes)
            {
                if (vectorTypes == null)
                    throw new ArgumentNullException(nameof(vectorTypes));

                VectorTypes = vectorTypes.ToArray();
                if (VectorTypes.IsEmpty)
                    throw new ArgumentEmptyException(nameof(vectorTypes));

                SurfaceFormat = surfaceFormat;
                GetData = GetGetDataDelegate(VectorTypes.Span[0].Type);
            }

            private static GetDataDelegate GetGetDataDelegate(Type type)
            {
                var genericMethod = _getDataMethod.MakeGenericMethod(type);
                return genericMethod.CreateDelegate<GetDataDelegate>();
            }
        }
    }
}
