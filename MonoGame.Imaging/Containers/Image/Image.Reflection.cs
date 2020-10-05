using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            SetupReflection_LoadPixelData();
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

        #region ConvertPixelData

        public delegate void ConvertPixelDataDelegate(ReadOnlySpan<byte> source, Span<byte> destination);

        private static MethodInfo? _convertPixelDataMethod;
        private static ConcurrentDictionary<SrcDstTypePair, ConvertPixelDataDelegate> ConvertPixelDataDelegateCache { get; } =
            new ConcurrentDictionary<SrcDstTypePair, ConvertPixelDataDelegate>();

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReflectionConvertPixelData<TPixelFrom, TPixelTo>(
           ReadOnlySpan<byte> source, Span<byte> destination)
           where TPixelFrom : unmanaged, IPixel
           where TPixelTo : unmanaged, IPixel
        {
            ConvertPixelData<TPixelFrom, TPixelTo>(source, destination);
        }

        private static void SetupReflection_ConvertPixels()
        {
            _convertPixelDataMethod = typeof(Image).GetMethod(
                nameof(ReflectionConvertPixelData), BindingFlags.NonPublic | BindingFlags.Static);

            if (_convertPixelDataMethod == null)
                throw new Exception("Can not find required method for reflection.");
        }

        public static ConvertPixelDataDelegate GetConvertPixelsDelegate(
            VectorType sourceType, VectorType destinationType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!ConvertPixelDataDelegateCache.TryGetValue(delegateKey, out var convertDelegate))
            {
                var genericMethod = _convertPixelDataMethod!.MakeGenericMethod(sourceType.Type, destinationType.Type);
                convertDelegate = CreateDelegateFromMethod<ConvertPixelDataDelegate>(genericMethod);
                ConvertPixelDataDelegateCache.TryAdd(delegateKey, convertDelegate);
            }
            return convertDelegate;
        }

        #endregion

        #region LoadPixels

        private delegate void LoadPixelDataDelegate(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, Image destination, int? byteStride);

        private static MethodInfo? _loadPixelDataMethod;
        private static ConcurrentDictionary<SrcDstTypePair, LoadPixelDataDelegate> LoadPixelDataDelegateCache { get; } =
            new ConcurrentDictionary<SrcDstTypePair, LoadPixelDataDelegate>();

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReflectionLoadPixelData<TPixelFrom, TPixelTo>(
            ReadOnlySpan<byte> pixelData,
            Rectangle sourceRectangle,
            Image<TPixelTo> destination,
            int? byteStride)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            LoadPixelData<TPixelFrom, TPixelTo>(pixelData, sourceRectangle, destination, byteStride);
        }

        private static void SetupReflection_LoadPixelData()
        {
            _loadPixelDataMethod = typeof(Image).GetMethod(
                nameof(ReflectionLoadPixelData), BindingFlags.NonPublic | BindingFlags.Static);

            if (_loadPixelDataMethod == null)
                throw new Exception("Can not find required method for reflection.");
        }

        private static LoadPixelDataDelegate GetLoadPixelDataDelegate(
            VectorType sourceType, VectorType destinationType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!LoadPixelDataDelegateCache!.TryGetValue(delegateKey, out var loadDelegate))
            {
                var delegateParams = typeof(LoadPixelDataDelegate).GetDelegateParameters().AsTypes();
                var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToList();
                var callParams = lambdaParams.Cast<Expression>().ToList();
                callParams[2] = Expression.Convert(callParams[2], typeof(Image<>).MakeGenericType(destinationType.Type));

                var genericMethod = _loadPixelDataMethod!.MakeGenericMethod(sourceType.Type, destinationType.Type);
                var call = Expression.Call(genericMethod, callParams);

                var lambda = Expression.Lambda<LoadPixelDataDelegate>(call, lambdaParams);
                loadDelegate = lambda.Compile();
                LoadPixelDataDelegateCache.TryAdd(delegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region LoadPixelRows

        private delegate void LoadPixelRowsDelegate(
            IReadOnlyPixelRows pixels, IPixelBuffer destination, Rectangle? sourceRectangle);

        private static MethodInfo? _loadPixelsMethod;
        private static ConcurrentDictionary<SrcDstTypePair, LoadPixelRowsDelegate> LoadPixelsDelegateCache { get; } =
            new ConcurrentDictionary<SrcDstTypePair, LoadPixelRowsDelegate>();

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReflectionLoadPixelRows<TPixelFrom, TPixelTo>(
            IReadOnlyPixelRows pixels, IPixelBuffer destination, Rectangle? sourceRectangle)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            LoadPixels<TPixelFrom, TPixelTo>(pixels, (IPixelBuffer<TPixelTo>)destination, sourceRectangle);
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
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            var delegateKey = (sourceType, destinationType);
            if (!LoadPixelsDelegateCache!.TryGetValue(delegateKey, out var loadDelegate))
            {
                var genericMethod = _loadPixelsMethod!.MakeGenericMethod(sourceType.Type, destinationType.Type);
                loadDelegate = CreateDelegateFromMethod<LoadPixelRowsDelegate>(genericMethod);
                LoadPixelsDelegateCache.TryAdd(delegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region WrapMemory

        private delegate Image WrapMemoryDelegate(
            IMemory memory, Size size, bool leaveOpen = false, int? byteStride = null);

        private static MethodInfo? _wrapMemoryMethod;
        private static ConcurrentDictionary<VectorType, WrapMemoryDelegate>? WrapMemoryDelegateCache { get; } =
            new ConcurrentDictionary<VectorType, WrapMemoryDelegate>();

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            if (!WrapMemoryDelegateCache!.TryGetValue(pixelType, out var wrapMemoryDelegate))
            {
                var genericMethod = _wrapMemoryMethod!.MakeGenericMethod(pixelType.Type);
                wrapMemoryDelegate = CreateDelegateFromMethod<WrapMemoryDelegate>(genericMethod);
                WrapMemoryDelegateCache.TryAdd(pixelType, wrapMemoryDelegate);
            }
            return wrapMemoryDelegate;
        }

        #endregion

        #region Create

        private delegate Image CreateDelegate(Size size, bool zeroFill);

        private static MethodInfo? _createMethod;
        private static ConcurrentDictionary<VectorType, CreateDelegate> _createDelegateCache =
            new ConcurrentDictionary<VectorType, CreateDelegate>();

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Image ReflectionCreate<TPixel>(Size size, bool zeroFill)
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
