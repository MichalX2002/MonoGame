using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MonoGame.Framework;
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
            SetupReflectionLoadPixels();
            SetupReflectionLoadPixelRows();
            SetupReflectionCreate();
        }

        private static TDelegate CreateDelegateForMethod<TDelegate>(MethodInfo method)
            where TDelegate : Delegate
        {
            var delegateParams = typeof(TDelegate).GetDelegateParameters().AsTypes();
            var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToArray();
            var call = Expression.Call(method, lambdaParams);
            var lambda = Expression.Lambda<TDelegate>(call, lambdaParams);
            return lambda.Compile();
        }

        #region LoadPixels

        private delegate void LoadPixelSpanDelegate(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride, Image destination);

        private static MethodInfo _loadPixelSpanMethod;
        private static ConcurrentDictionary<FromToPixelTypes, LoadPixelSpanDelegate> _loadPixelSpanDelegateCache;

        private static void SetupReflectionLoadPixels()
        {
            _loadPixelSpanMethod = typeof(Image).GetMethod(nameof(LoadPixelSpan), BindingFlags.NonPublic | BindingFlags.Static);
            _loadPixelSpanDelegateCache = new ConcurrentDictionary<FromToPixelTypes, LoadPixelSpanDelegate>();
        }

        private static LoadPixelSpanDelegate GetLoadPixelSpanDelegate(
            VectorTypeInfo fromPixelType, VectorTypeInfo toPixelType)
        {
            if (fromPixelType == null) throw new ArgumentNullException(nameof(fromPixelType));
            if (toPixelType == null) throw new ArgumentNullException(nameof(toPixelType));

            var loadPixelsDelegateKey = (fromPixelType, toPixelType);
            if (!_loadPixelSpanDelegateCache.TryGetValue(loadPixelsDelegateKey, out var loadDelegate))
            {
                var delegateParams = typeof(LoadPixelSpanDelegate).GetDelegateParameters().AsTypes();
                var lambdaParams = delegateParams.Select(x => Expression.Parameter(x)).ToArray();
                var callParams = lambdaParams.Cast<Expression>().ToArray();
                var body = new List<Expression>();

                var spanCast = ImagingReflectionHelper.SpanCastMethod.MakeGenericMethod(typeof(byte), fromPixelType.Type);
                var spanCastCall = Expression.Call(spanCast, lambdaParams[0]);
                callParams[0] = spanCastCall;

                callParams[3] = Expression.Convert(callParams[3], typeof(Image<>).MakeGenericType(toPixelType.Type));

                var genericLoadPixelSpan = _loadPixelSpanMethod.MakeGenericMethod(fromPixelType.Type, toPixelType.Type);
                body.Add(Expression.Call(genericLoadPixelSpan, callParams));

                var block = Expression.Block(body);
                var lambda = Expression.Lambda<LoadPixelSpanDelegate>(block, lambdaParams);
                loadDelegate = lambda.Compile();
                _loadPixelSpanDelegateCache.TryAdd(loadPixelsDelegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region LoadPixelRows

        private delegate void LoadPixelRowsDelegate(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination);

        private static MethodInfo _loadPixelRowsMethod;
        private static ConcurrentDictionary<FromToPixelTypes, LoadPixelRowsDelegate> _loadPixelRowsDelegateCache;

        private static void SetupReflectionLoadPixelRows()
        {
            _loadPixelRowsMethod = typeof(Image).GetMethod(nameof(LoadPixelRows), BindingFlags.NonPublic | BindingFlags.Static);
            _loadPixelRowsDelegateCache = new ConcurrentDictionary<FromToPixelTypes, LoadPixelRowsDelegate>();
        }

        private static LoadPixelRowsDelegate GetLoadPixelRowsDelegate(
            VectorTypeInfo fromPixelType, VectorTypeInfo toPixelType)
        {
            if (fromPixelType == null) throw new ArgumentNullException(nameof(fromPixelType));
            if (toPixelType == null) throw new ArgumentNullException(nameof(toPixelType));

            var loadPixelsDelegateKey = (fromPixelType, toPixelType);
            if (!_loadPixelRowsDelegateCache.TryGetValue(loadPixelsDelegateKey, out var loadDelegate))
            {
                var genericLoadPixels = _loadPixelRowsMethod.MakeGenericMethod(fromPixelType.Type, toPixelType.Type);
                loadDelegate = CreateDelegateForMethod<LoadPixelRowsDelegate>(genericLoadPixels);
                _loadPixelRowsDelegateCache.TryAdd(loadPixelsDelegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region Create

        private delegate Image CreateDelegate(int width, int height);

        private static Type[] _createMethodArguments;
        private static ConcurrentDictionary<VectorTypeInfo, CreateDelegate> _createDelegateCache;

        private static void SetupReflectionCreate()
        {
            _createMethodArguments = typeof(CreateDelegate).GetDelegateParameters().AsTypes();
            _createDelegateCache = new ConcurrentDictionary<VectorTypeInfo, CreateDelegate>(
                VectorTypeInfoEqualityComparer.Instance);
        }

        private static CreateDelegate GetCreateDelegate(VectorTypeInfo pixelType)
        {
            if (pixelType == null)
                throw new ArgumentNullException(nameof(VectorTypeInfo));

            if (!_createDelegateCache.TryGetValue(pixelType, out var createDelegate))
            {
                var imageType = typeof(Image<>).MakeGenericType(pixelType.Type);
                var genericCreateMethod = imageType.GetMethod("Create", _createMethodArguments);
                createDelegate = CreateDelegateForMethod<CreateDelegate>(genericCreateMethod);
                _createDelegateCache.TryAdd(pixelType, createDelegate);
            }
            return createDelegate;
        }

        #endregion
    }
}
