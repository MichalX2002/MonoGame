using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    using FromToPixelTypes = ValueTuple<VectorTypeInfo, VectorTypeInfo>;

    public partial class Image
    {
        private static void SetupReflection()
        {
            SetupReflection_LoadPixels();
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

        #region LoadPixels

        private delegate void LoadPixelSpanDelegate(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride, Image destination);

        private static MethodInfo _loadPixelSpanMethod;
        private static ConcurrentDictionary<FromToPixelTypes, LoadPixelSpanDelegate> _loadPixelSpanDelegateCache;

        private static void SetupReflection_LoadPixels()
        {
            _loadPixelSpanMethod = typeof(Image).GetMethod(nameof(LoadPixelSpan), BindingFlags.NonPublic | BindingFlags.Static);
            _loadPixelSpanDelegateCache = new ConcurrentDictionary<FromToPixelTypes, LoadPixelSpanDelegate>();
        }

        private static LoadPixelSpanDelegate GetLoadPixelSpanDelegate(
            VectorTypeInfo fromPixelType, VectorTypeInfo toPixelType)
        {
            if (fromPixelType == null) throw new ArgumentNullException(nameof(fromPixelType));
            if (toPixelType == null) throw new ArgumentNullException(nameof(toPixelType));

            var delegateKey = (fromPixelType, toPixelType);
            if (!_loadPixelSpanDelegateCache.TryGetValue(delegateKey, out var loadDelegate))
            {
                var delegateParams = typeof(LoadPixelSpanDelegate).GetDelegateParameters().AsTypes();
                var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToList();
                var callParams = lambdaParams.Cast<Expression>().ToList();
                var body = new List<Expression>();

                var spanCast = ImagingReflectionHelper.SpanCastMethod.MakeGenericMethod(typeof(byte), fromPixelType.Type);
                var spanCastCall = Expression.Call(spanCast, lambdaParams[0]);
                callParams[0] = spanCastCall;

                callParams[3] = Expression.Convert(callParams[3], typeof(Image<>).MakeGenericType(toPixelType.Type));

                var genericMethod = _loadPixelSpanMethod.MakeGenericMethod(fromPixelType.Type, toPixelType.Type);
                body.Add(Expression.Call(genericMethod, callParams));

                var block = Expression.Block(body);
                var lambda = Expression.Lambda<LoadPixelSpanDelegate>(block, lambdaParams);
                loadDelegate = lambda.Compile();
                _loadPixelSpanDelegateCache.TryAdd(delegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region LoadPixelRows

        private delegate void LoadPixelRowsDelegate(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination);

        private static MethodInfo _loadPixelRowsMethod;
        private static ConcurrentDictionary<FromToPixelTypes, LoadPixelRowsDelegate> _loadPixelRowsDelegateCache;

        private static void SetupReflection_LoadPixelRows()
        {
            _loadPixelRowsMethod = typeof(Image).GetMethod(nameof(LoadPixelRows), BindingFlags.NonPublic | BindingFlags.Static);
            _loadPixelRowsDelegateCache = new ConcurrentDictionary<FromToPixelTypes, LoadPixelRowsDelegate>();
        }

        private static LoadPixelRowsDelegate GetLoadPixelRowsDelegate(
            VectorTypeInfo fromPixelType, VectorTypeInfo toPixelType)
        {
            if (fromPixelType == null) throw new ArgumentNullException(nameof(fromPixelType));
            if (toPixelType == null) throw new ArgumentNullException(nameof(toPixelType));

            var delegateKey = (fromPixelType, toPixelType);
            if (!_loadPixelRowsDelegateCache.TryGetValue(delegateKey, out var loadDelegate))
            {
                var genericMethod = _loadPixelRowsMethod.MakeGenericMethod(fromPixelType.Type, toPixelType.Type);
                loadDelegate = CreateDelegateForMethod<LoadPixelRowsDelegate>(genericMethod);
                _loadPixelRowsDelegateCache.TryAdd(delegateKey, loadDelegate);
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
