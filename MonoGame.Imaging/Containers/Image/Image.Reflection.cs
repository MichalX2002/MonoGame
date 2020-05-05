using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Utilities;

namespace MonoGame.Imaging
{
    using SrcDstTypePair = ValueTuple<VectorTypeInfo, VectorTypeInfo>;

    public partial class Image
    {
        private static void SetupReflection()
        {
            SetupReflection_ConvertPixelSpan();
            SetupReflection_LoadPixelSpan();
            SetupReflection_LoadPixelRows();
            SetupReflection_WrapMemory();
            SetupReflection_Create();
        }

        private static TDelegate CreateDelegateForMethod<TDelegate>(MethodInfo method)
            where TDelegate : Delegate
        {
            var delegateParams = typeof(TDelegate).GetDelegateParameters().AsTypes();
            var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToList();
            var call = Expression.Call(method, lambdaParams);
            var lambda = Expression.Lambda<TDelegate>(call, lambdaParams);
            return lambda.Compile();
        }

        #region ConvertPixelSpan

        public delegate void ConvertPixelsDelegate(ReadOnlySpan<byte> source, Span<byte> destination);

        private static MethodInfo _convertPixelSpanMethod;
        private static ConcurrentDictionary<SrcDstTypePair, ConvertPixelsDelegate> _convertPixelSpanDelegateCache;

        private static void SetupReflection_ConvertPixelSpan()
        {
            _convertPixelSpanMethod = typeof(Image).GetMethod(nameof(ConvertPixelsCore), BindingFlags.NonPublic | BindingFlags.Static);
            _convertPixelSpanDelegateCache = new ConcurrentDictionary<SrcDstTypePair, ConvertPixelsDelegate>();
        }

        public static ConvertPixelsDelegate GetConvertPixelsDelegate(
            VectorTypeInfo sourceType, VectorTypeInfo destinationType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!_convertPixelSpanDelegateCache.TryGetValue(delegateKey, out var convertDelegate))
            {
                var genericMethod = _convertPixelSpanMethod.MakeGenericMethod(sourceType.Type, destinationType.Type);
                convertDelegate = CreateDelegateForMethod<ConvertPixelsDelegate>(genericMethod);
                _convertPixelSpanDelegateCache.TryAdd(delegateKey, convertDelegate);
            }
            return convertDelegate;
        }

        #endregion

        #region LoadPixels

        private delegate void LoadPixelSpanDelegate(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride, Image destination);

        private static MethodInfo _loadPixelSpanMethod;
        private static ConcurrentDictionary<SrcDstTypePair, LoadPixelSpanDelegate> _loadPixelSpanDelegateCache;

        private static void SetupReflection_LoadPixelSpan()
        {
            _loadPixelSpanMethod = typeof(Image).GetMethod(nameof(LoadPixels), BindingFlags.NonPublic | BindingFlags.Static);
            _loadPixelSpanDelegateCache = new ConcurrentDictionary<SrcDstTypePair, LoadPixelSpanDelegate>();
        }

        private static LoadPixelSpanDelegate GetLoadPixelSpanDelegate(
            VectorTypeInfo sourceType, VectorTypeInfo destinationType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!_loadPixelSpanDelegateCache.TryGetValue(delegateKey, out var loadDelegate))
            {
                var delegateParams = typeof(LoadPixelSpanDelegate).GetDelegateParameters().AsTypes();
                var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToList();
                var callParams = lambdaParams.Cast<Expression>().ToList();
                
                var spanCast = ImagingReflectionHelper.SpanCastMethod.MakeGenericMethod(typeof(byte), sourceType.Type);
                var spanCastCall = Expression.Call(spanCast, lambdaParams[0]);
                callParams[0] = spanCastCall;
                callParams[3] = Expression.Convert(callParams[3], typeof(Image<>).MakeGenericType(destinationType.Type));

                var genericMethod = _loadPixelSpanMethod.MakeGenericMethod(sourceType.Type, destinationType.Type);
                var call = Expression.Call(genericMethod, callParams);

                var lambda = Expression.Lambda<LoadPixelSpanDelegate>(call, lambdaParams);
                loadDelegate = lambda.Compile();
                _loadPixelSpanDelegateCache.TryAdd(delegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region LoadPixelRows

        private delegate void LoadPixelRowsDelegate(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination);

        private static MethodInfo _loadPixelsMethod;
        private static ConcurrentDictionary<SrcDstTypePair, LoadPixelRowsDelegate> _loadPixelsDelegateCache;

        private static void SetupReflection_LoadPixelRows()
        {
            _loadPixelsMethod = typeof(Image).GetMethod(nameof(LoadPixelsCore), BindingFlags.NonPublic | BindingFlags.Static);
            _loadPixelsDelegateCache = new ConcurrentDictionary<SrcDstTypePair, LoadPixelRowsDelegate>();
        }

        private static LoadPixelRowsDelegate GetLoadPixelRowsDelegate(
            VectorTypeInfo sourceType, VectorTypeInfo destinationType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!_loadPixelsDelegateCache.TryGetValue(delegateKey, out var loadDelegate))
            {
                var genericMethod = _loadPixelsMethod.MakeGenericMethod(sourceType.Type, destinationType.Type);
                loadDelegate = CreateDelegateForMethod<LoadPixelRowsDelegate>(genericMethod);
                _loadPixelsDelegateCache.TryAdd(delegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region WrapMemory

        private delegate Image WrapMemoryDelegate(
            IMemory memory, Size size, bool leaveOpen = false, int? byteStride = null);

        private static MethodInfo _wrapMemoryMethod;
        private static ConcurrentDictionary<VectorTypeInfo, WrapMemoryDelegate> _wrapMemoryDelegateCache;

        private static void SetupReflection_WrapMemory()
        {
            var arguments = typeof(WrapMemoryDelegate).GetDelegateParameters().AsTypes();

            _wrapMemoryMethod = typeof(Image).GetMethod(nameof(WrapMemory), arguments);
            _wrapMemoryDelegateCache = new ConcurrentDictionary<VectorTypeInfo, WrapMemoryDelegate>();
        }

        private static WrapMemoryDelegate GetWrapMemoryDelegate(VectorTypeInfo pixelType)
        {
            if (pixelType == null) throw new ArgumentNullException(nameof(pixelType));

            if (!_wrapMemoryDelegateCache.TryGetValue(pixelType, out var wrapMemoryDelegate))
            {
                var genericMethod = _wrapMemoryMethod.MakeGenericMethod(pixelType.Type);
                wrapMemoryDelegate = CreateDelegateForMethod<WrapMemoryDelegate>(genericMethod);
                _wrapMemoryDelegateCache.TryAdd(pixelType, wrapMemoryDelegate);
            }
            return wrapMemoryDelegate;
        }

        #endregion

        #region Create

        private delegate Image CreateDelegate(Size size, bool zeroFill);

        private static Type[] _createMethodArguments;
        private static ConcurrentDictionary<VectorTypeInfo, CreateDelegate> _createDelegateCache;

        private static void SetupReflection_Create()
        {
            _createMethodArguments = typeof(CreateDelegate).GetDelegateParameters().AsTypes();
            _createDelegateCache = new ConcurrentDictionary<VectorTypeInfo, CreateDelegate>();
        }

        private static CreateDelegate GetCreateDelegate(VectorTypeInfo pixelType)
        {
            if (pixelType == null)
                throw new ArgumentNullException(nameof(VectorTypeInfo));

            if (!_createDelegateCache.TryGetValue(pixelType, out var createDelegate))
            {
                const BindingFlags binding = BindingFlags.Static | BindingFlags.NonPublic;
                var imageType = typeof(Image<>).MakeGenericType(pixelType.Type);
                var genericCreateMethod = imageType.GetMethod(
                    "CreateCore", binding, null, _createMethodArguments, null);
                createDelegate = CreateDelegateForMethod<CreateDelegate>(genericCreateMethod);
                _createDelegateCache.TryAdd(pixelType, createDelegate);
            }
            return createDelegate;
        }

        #endregion
    }
}
