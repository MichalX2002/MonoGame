// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Concurrent;
using System.Linq;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture2D
    {
        public class PixelSaveFormat
        {
            private delegate IMemory GetDataDelegate(Texture2D texture, Rectangle rect, int level, int arraySlice);
            
            private GetDataDelegate _getDataDelegate;

            public SurfaceFormat Format { get; }
            public PixelTypeInfo PixelType { get; }

            public PixelSaveFormat(SurfaceFormat format, Type pixelType)
            {
                if (pixelType == null)
                    throw new ArgumentNullException(nameof(pixelType));

                Format = format;
                PixelType = PixelTypeInfo.Get(pixelType);

                TDelegate GetDelegate<TDelegate>(Type methodHost, string methodName)
                    where TDelegate : Delegate
                {
                    var methodParams = typeof(TDelegate).GetDelegateParameters();
                    var method = methodHost.GetMethod(
                        methodName, methodParams.Select(x => x.ParameterType).ToArray());

                    var genericMethod = method.MakeGenericMethod(PixelType.Type);
                    return (TDelegate)genericMethod.CreateDelegate(typeof(TDelegate));
                }

                _getDataDelegate = GetDelegate<GetDataDelegate>(typeof(Texture2D), "GetData");
            }

            public IMemory GetData(Texture2D texture, Rectangle rect, int level, int arraySlice)
            {
                return _getDataDelegate.Invoke(texture, rect, level, arraySlice);
            }
        }

        private static ConcurrentDictionary<SurfaceFormat, PixelSaveFormat> SaveFormatsBySurface { get; }
        private static ConcurrentDictionary<Type, PixelSaveFormat> SaveFormatsByType { get; }

        static Texture2D()
        {
            // TODO: implement SRgb

            SaveFormatsBySurface = new ConcurrentDictionary<SurfaceFormat, PixelSaveFormat>();
            SaveFormatsByType = new ConcurrentDictionary<Type, PixelSaveFormat>();

            AddPixelSaveFormat(SurfaceFormat.Alpha8, typeof(Alpha8));
            AddPixelSaveFormat(SurfaceFormat.Single, typeof(Gray32));
            // AddSaveFormat(SurfaceFormat.Rgba32SRgb, typeof(Rgba32SRgb));
            AddPixelSaveFormat(SurfaceFormat.Rgba32, typeof(Color));
            AddPixelSaveFormat(SurfaceFormat.Rg32, typeof(Rg32));
            AddPixelSaveFormat(SurfaceFormat.Rgba64, typeof(Rgba64));
            AddPixelSaveFormat(SurfaceFormat.Rgba1010102, typeof(Rgba1010102));
            AddPixelSaveFormat(SurfaceFormat.Bgr565, typeof(Bgr565));
            AddPixelSaveFormat(SurfaceFormat.Bgra5551, typeof(Bgra5551));
            AddPixelSaveFormat(SurfaceFormat.Bgra4444, typeof(Bgra4444));

            // AddSaveFormat(SurfaceFormat.Bgr32SRgb, typeof(Bgr32SRgb));
            // AddSaveFormat(SurfaceFormat.Bgra32SRgb, typeof(Bgra32SRgb));
            AddPixelSaveFormat(SurfaceFormat.Bgr32, typeof(Bgr32));
            AddPixelSaveFormat(SurfaceFormat.Bgra32, typeof(Bgra32));

            AddPixelSaveFormat(SurfaceFormat.HalfSingle, typeof(HalfSingle));
            AddPixelSaveFormat(SurfaceFormat.HalfVector2, typeof(HalfVector2));
            AddPixelSaveFormat(SurfaceFormat.HalfVector4, typeof(HalfVector4));
            AddPixelSaveFormat(SurfaceFormat.Vector2, typeof(Vector2));
            AddPixelSaveFormat(SurfaceFormat.Vector4, typeof(Vector4));
            AddPixelSaveFormat(SurfaceFormat.HdrBlendable, typeof(RgbaVector));

            AddPixelSaveFormat(SurfaceFormat.NormalizedByte2, typeof(NormalizedByte2));
            AddPixelSaveFormat(SurfaceFormat.NormalizedByte4, typeof(NormalizedByte4));
        }

        private static void AddPixelSaveFormat(SurfaceFormat format, Type pixelType)
        {
            var pixelSaveFormat = new PixelSaveFormat(format, pixelType);
            SaveFormatsBySurface.TryAdd(format, pixelSaveFormat);
            SaveFormatsByType.TryAdd(pixelType, pixelSaveFormat);
        }

        public static PixelSaveFormat GetPixelSaveFormat(SurfaceFormat textureFormat)
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

        public static PixelSaveFormat GetPixelSaveFormat(Type pixelType)
        {
            if (!SaveFormatsByType.TryGetValue(pixelType, out var pixelFormat))
            {
                throw new NotSupportedException(
                    $"The pixel type {pixelType} is not supported.");
            }
            return pixelFormat;
        }
    }
}
