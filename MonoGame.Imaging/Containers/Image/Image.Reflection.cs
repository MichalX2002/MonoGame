using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    // TODO: put Reflection* methods into seperate class

    // TODO: create container class so delegates can be accessed by field instead of dict-lookup

    using SrcDstTypePair = ValueTuple<VectorType, VectorType>;

    public partial class Image
    {
        private static void SetupReflection()
        {
            SetupReflection_ConvertPixels();
            SetupReflection_LoadPixelSpan();
            SetupReflection_LoadPixelRows();
            SetupReflection_WrapMemory();
            SetupReflection_Create();
        }

        public static TDelegate CreateDelegateFromMethod<TDelegate>(
            MethodInfo method, bool useFirstArgumentAsInstance = false)
               where TDelegate : Delegate
        {
            var delegateParams = typeof(TDelegate).GetDelegateParameters().AsTypes();
            var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToList();
            var call = useFirstArgumentAsInstance
                ? Expression.Call(lambdaParams[0], method, lambdaParams.Skip(1))
                : Expression.Call(method, lambdaParams);
            var lambda = Expression.Lambda<TDelegate>(call, lambdaParams);
            return lambda.Compile();
        }

        #region ConvertPixelSpan

        public delegate void ConvertPixelsDelegate(ReadOnlySpan<byte> source, Span<byte> destination);

        private static MethodInfo? _convertPixelsMethod;
        private static ConcurrentDictionary<SrcDstTypePair, ConvertPixelsDelegate> _convertPixelSpanDelegateCache =
            new ConcurrentDictionary<SrcDstTypePair, ConvertPixelsDelegate>();

        private static void ReflectionConvertPixels<TPixelFrom, TPixelTo>(
           ReadOnlySpan<byte> source, Span<byte> destination)
           where TPixelFrom : unmanaged, IPixel
           where TPixelTo : unmanaged, IPixel
        {
            ConvertPixelBytes<TPixelFrom, TPixelTo>(source, destination);
        }

        private static void SetupReflection_ConvertPixels()
        {
            _convertPixelsMethod = typeof(Image).GetMethod(
                nameof(ReflectionConvertPixels), BindingFlags.NonPublic | BindingFlags.Static);

            if (_convertPixelsMethod == null)
                throw new Exception("Can not find required method for reflection.");
        }

        public static ConvertPixelsDelegate GetConvertPixelsDelegate(
            VectorType sourceType, VectorType destinationType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!_convertPixelSpanDelegateCache.TryGetValue(delegateKey, out var convertDelegate))
            {
                var genericMethod = _convertPixelsMethod!.MakeGenericMethod(sourceType.Type, destinationType.Type);
                convertDelegate = CreateDelegateFromMethod<ConvertPixelsDelegate>(genericMethod);
                _convertPixelSpanDelegateCache.TryAdd(delegateKey, convertDelegate);
            }
            return convertDelegate;
        }

        #endregion

        #region LoadPixels

        private delegate void LoadPixelSpanDelegate(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride, Image destination);

        private static MethodInfo? _loadPixelSpanMethod;
        private static ConcurrentDictionary<SrcDstTypePair, LoadPixelSpanDelegate> _loadPixelSpanDelegateCache =
            new ConcurrentDictionary<SrcDstTypePair, LoadPixelSpanDelegate>();

        private static unsafe void ReflectionLoadPixelBytes<TPixelFrom, TPixelTo>(
           ReadOnlySpan<byte> pixelData,
           Rectangle sourceRectangle,
           int? byteStride,
           Image<TPixelTo> destination)
           where TPixelFrom : unmanaged, IPixel
           where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            var pixels = MemoryMarshal.Cast<byte, TPixelFrom>(pixelData);
            LoadPixels(pixels, sourceRectangle, byteStride, destination);
        }

        private static void SetupReflection_LoadPixelSpan()
        {
            _loadPixelSpanMethod = typeof(Image).GetMethod(
                nameof(ReflectionLoadPixelBytes), BindingFlags.NonPublic | BindingFlags.Static);

            if (_loadPixelSpanMethod == null)
                throw new Exception("Can not find required method for reflection.");
        }

        private static LoadPixelSpanDelegate GetLoadPixelSpanDelegate(
            VectorType sourceType, VectorType destinationType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!_loadPixelSpanDelegateCache!.TryGetValue(delegateKey, out var loadDelegate))
            {
                var delegateParams = typeof(LoadPixelSpanDelegate).GetDelegateParameters().AsTypes();
                var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToList();
                var callParams = lambdaParams.Cast<Expression>().ToList();
                callParams[3] = Expression.Convert(callParams[3], typeof(Image<>).MakeGenericType(destinationType.Type));

                var genericMethod = _loadPixelSpanMethod!.MakeGenericMethod(sourceType.Type, destinationType.Type);
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

        private static MethodInfo? _loadPixelsMethod;
        private static ConcurrentDictionary<SrcDstTypePair, LoadPixelRowsDelegate> _loadPixelsDelegateCache =
            new ConcurrentDictionary<SrcDstTypePair, LoadPixelRowsDelegate>();

        private static void ReflectionLoadPixelRows<TPixelFrom, TPixelTo>(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            LoadPixels<TPixelFrom, TPixelTo>(pixels, sourceRectangle, (Image<TPixelTo>)destination);
        }

        private static void SetupReflection_LoadPixelRows()
        {
            _loadPixelsMethod = typeof(Image).GetMethod(
                nameof(ReflectionLoadPixelRows), BindingFlags.NonPublic | BindingFlags.Static);

            if (_loadPixelsMethod == null)
                throw new Exception("Can not find required method for reflection.");
        }

        private static LoadPixelRowsDelegate GetLoadPixelRowsDelegate(
            VectorType sourceType, VectorType destinationType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!_loadPixelsDelegateCache!.TryGetValue(delegateKey, out var loadDelegate))
            {
                var genericMethod = _loadPixelsMethod!.MakeGenericMethod(sourceType.Type, destinationType.Type);
                loadDelegate = CreateDelegateFromMethod<LoadPixelRowsDelegate>(genericMethod);
                _loadPixelsDelegateCache.TryAdd(delegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region WrapMemory

        private delegate Image WrapMemoryDelegate(
            IMemory memory, Size size, bool leaveOpen = false, int? byteStride = null);

        private static MethodInfo? _wrapMemoryMethod;
        private static ConcurrentDictionary<VectorType, WrapMemoryDelegate>? _wrapMemoryDelegateCache =
            new ConcurrentDictionary<VectorType, WrapMemoryDelegate>();

        private static Image ReflectionWrapMemory<TPixel>(
            IMemory memory, Size size, bool leaveOpen = false, int? byteStride = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return WrapMemory<TPixel>(memory, size, leaveOpen, byteStride);
        }

        private static void SetupReflection_WrapMemory()
        {
            _wrapMemoryMethod = typeof(Image).GetMethod(
                nameof(ReflectionWrapMemory), BindingFlags.NonPublic | BindingFlags.Static);

            if (_wrapMemoryMethod == null)
                throw new Exception("Can not find required method for reflection.");
        }

        private static WrapMemoryDelegate GetWrapMemoryDelegate(VectorType pixelType)
        {
            if (pixelType == null)
                throw new ArgumentNullException(nameof(pixelType));

            if (!_wrapMemoryDelegateCache!.TryGetValue(pixelType, out var wrapMemoryDelegate))
            {
                var genericMethod = _wrapMemoryMethod!.MakeGenericMethod(pixelType.Type);
                wrapMemoryDelegate = CreateDelegateFromMethod<WrapMemoryDelegate>(genericMethod);
                _wrapMemoryDelegateCache.TryAdd(pixelType, wrapMemoryDelegate);
            }
            return wrapMemoryDelegate;
        }

        #endregion

        #region Create

        private delegate Image CreateDelegate(Size size, bool zeroFill);

        private static MethodInfo? _createMethod;
        private static ConcurrentDictionary<VectorType, CreateDelegate> _createDelegateCache =
            new ConcurrentDictionary<VectorType, CreateDelegate>();

        internal static Image ReflectionCreate<TPixel>(Size size, bool zeroFill)
           where TPixel : unmanaged, IPixel<TPixel>
        {
            return Image<TPixel>.CreateCore(size, zeroFill);
        }

        private static void SetupReflection_Create()
        {
            _createMethod = typeof(Image).GetMethod(
                nameof(ReflectionCreate), BindingFlags.NonPublic | BindingFlags.Static);

            if (_createMethod == null)
                throw new Exception("Can not find required method for reflection.");
        }

        private static CreateDelegate GetCreateDelegate(VectorType pixelType)
        {
            if (pixelType == null)
                throw new ArgumentNullException(nameof(pixelType));

            if (!_createDelegateCache!.TryGetValue(pixelType, out var createDelegate))
            {
                var genericMethod = _createMethod!.MakeGenericMethod(pixelType.Type);
                createDelegate = CreateDelegateFromMethod<CreateDelegate>(genericMethod);
                _createDelegateCache.TryAdd(pixelType, createDelegate);
            }
            return createDelegate;
        }

        #endregion
    }
}
