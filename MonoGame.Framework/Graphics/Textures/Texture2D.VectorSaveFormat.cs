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
        private static ConcurrentDictionary<SurfaceFormat, HashSet<VectorSaveFormat>> SaveFormatsBySurface { get; } =
            new ConcurrentDictionary<SurfaceFormat, HashSet<VectorSaveFormat>>();

        private static ConcurrentDictionary<Type, HashSet<VectorSaveFormat>> SaveFormatsByType { get; } =
            new ConcurrentDictionary<Type, HashSet<VectorSaveFormat>>();

        private static Dictionary<SurfaceFormat, ReadOnlySet<VectorSaveFormat>> SaveFormatsBySurfaceRO { get; } =
            new Dictionary<SurfaceFormat, ReadOnlySet<VectorSaveFormat>>();

        private static Dictionary<Type, ReadOnlySet<VectorSaveFormat>> SaveFormatsByTypeRO { get; } =
            new Dictionary<Type, ReadOnlySet<VectorSaveFormat>>();

        private static void InitializeVectorSaveFormats()
        {
            // TODO: implement SRgb

            AddVectorSaveFormat(SurfaceFormat.Alpha8, typeof(Alpha8));
            AddVectorSaveFormat(SurfaceFormat.Single, typeof(Gray32));
            // AddSaveFormat(SurfaceFormat.Rgba32SRgb, typeof(Rgba32SRgb));
            AddVectorSaveFormat(SurfaceFormat.Color, typeof(Color), typeof(Byte4), typeof(NormalizedByte4));
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
            AddVectorSaveFormat(SurfaceFormat.Vector4, typeof(Vector4), typeof(RgbaVector));
            AddVectorSaveFormat(SurfaceFormat.HdrBlendable, typeof(RgbaVector), typeof(Vector2));

            AddVectorSaveFormat(SurfaceFormat.NormalizedByte2, typeof(NormalizedByte2));
            AddVectorSaveFormat(SurfaceFormat.NormalizedByte4, typeof(NormalizedByte4), typeof(Byte4));
        }

        private static void AddVectorSaveFormat(SurfaceFormat format, params Type[] vectorTypes)
        {
            var vectorInfos = vectorTypes.Select(x => VectorTypeInfo.Get(x));
            var saveFormat = new VectorSaveFormat(format, vectorInfos);

            foreach (var vectorType in vectorTypes)
            {
                HashSet<VectorSaveFormat> CreateUpdatedSet<TKey>(
                    TKey key, HashSet<VectorSaveFormat> existingSet,
                    Dictionary<TKey, ReadOnlySet<VectorSaveFormat>> output)
                {
                    var set = existingSet != null
                        ? new HashSet<VectorSaveFormat>(existingSet)
                        : new HashSet<VectorSaveFormat>();
                    set.Add(saveFormat);

                    lock (output)
                        output[key] = set.AsReadOnly();
                    
                    return set;
                }

                SaveFormatsBySurface.AddOrUpdate(
                    format,
                    addValueFactory: (f) => CreateUpdatedSet(f, null, SaveFormatsBySurfaceRO),
                    updateValueFactory: (f, src) => CreateUpdatedSet(f, src, SaveFormatsBySurfaceRO));

                SaveFormatsByType.AddOrUpdate(
                    vectorType,
                    addValueFactory: (t) => CreateUpdatedSet(t, null, SaveFormatsByTypeRO),
                    updateValueFactory: (t, src) => CreateUpdatedSet(t, src, SaveFormatsByTypeRO));
            }
        }

        public static ReadOnlySet<VectorSaveFormat> GetVectorSaveFormats(SurfaceFormat textureFormat)
        {
            if (!SaveFormatsBySurfaceRO.TryGetValue(textureFormat, out var formatSet))
                throw GetUnsupportedSurfaceFormatForImagingException(textureFormat, nameof(textureFormat));
            return formatSet;
        }

        private static Exception GetUnsupportedSurfaceFormatForImagingException(
            SurfaceFormat textureFormat, string paramName)
        {
            var innerException = textureFormat.IsCompressedFormat()
                ? new ArgumentException("Compressed texture formats are currently not supported.")
                : null;

            return new ArgumentException(
                $"The format {textureFormat} is not supported.", paramName, innerException);
        }

        public static VectorSaveFormat GetVectorSaveFormat(SurfaceFormat textureFormat)
        {
            foreach (var format in GetVectorSaveFormats(textureFormat))
                return format;
            return null;
        }

        public static ReadOnlySet<VectorSaveFormat> GetVectorSaveFormats(Type vectorType)
        {
            if (!SaveFormatsByTypeRO.TryGetValue(vectorType, out var formatSet))
                throw new NotSupportedException($"The vector type {vectorType} is not supported.");
            return formatSet;
        }

        public static VectorSaveFormat GetVectorSaveFormat(Type vectorType)
        {
            foreach (var format in GetVectorSaveFormats(vectorType))
                return format;
            return null;
        }

        public class VectorSaveFormat
        {
            private static MethodInfo _getDataMethod;

            public delegate IMemory GetDataDelegate(
                Texture2D texture, Rectangle? rectangle, int level, int arraySlice);

            public SurfaceFormat Format { get; }

            /// <summary>
            /// Gets structurally equal types that
            /// represent this <see cref="VectorSaveFormat"/>.
            /// </summary>
            public ReadOnlyMemory<VectorTypeInfo> Types { get; }

            public GetDataDelegate GetData { get; }

            static VectorSaveFormat()
            {
                var methodParams = typeof(GetDataDelegate).GetDelegateParameters().Skip(1);
                _getDataMethod = typeof(Texture2D).GetMethod(
                    "GetData", methodParams.Select(x => x.ParameterType).ToArray());
            }

            public VectorSaveFormat(SurfaceFormat format, IEnumerable<VectorTypeInfo> types)
            {
                if (types == null)
                    throw new ArgumentNullException(nameof(types));

                Types = types.ToArray();
                if (Types.IsEmpty)
                    throw new ArgumentEmptyException(nameof(types));

                Format = format;
                GetData = GetGetDataDelegate(Types.Span[0].Type);
            }

            private static GetDataDelegate GetGetDataDelegate(Type type)
            {
                var genericMethod = _getDataMethod.MakeGenericMethod(type);
                return genericMethod.CreateDelegate<GetDataDelegate>();
            }
        }
    }
}
