using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

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

        #region LoadPixels

        private delegate void LoadPixelsDelegate(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride, Image destination);

        private static MethodInfo _loadPixelsMethod;
        private static ConcurrentDictionary<FromToPixelTypes, LoadPixelsDelegate> _loadPixelsDelegateCache;

        private static void SetupReflectionLoadPixels()
        {
            var loadPixelsTypes = typeof(LoadPixelsDelegate).GetDelegateParameters().GetGenericTypeDefinitions();
            loadPixelsTypes[loadPixelsTypes.Length - 1] = typeof(Image<>).GetGenericTypeDefinition();

            _loadPixelsMethod = typeof(Image<>).GetMethod("LoadPixels", loadPixelsTypes);
            _loadPixelsDelegateCache = new ConcurrentDictionary<FromToPixelTypes, LoadPixelsDelegate>();
        }

        private static LoadPixelsDelegate GetLoadPixelsDelegate(
            VectorTypeInfo fromPixelType, VectorTypeInfo toPixelType)
        {
            if (fromPixelType == null) throw new ArgumentNullException(nameof(fromPixelType));
            if (toPixelType == null) throw new ArgumentNullException(nameof(toPixelType));

            var loadPixelsDelegateKey = (fromPixelType, toPixelType);
            if (!_loadPixelsDelegateCache.TryGetValue(loadPixelsDelegateKey, out var loadDelegate))
            {
                var genericLoadPixels = _loadPixelsMethod.MakeGenericMethod(fromPixelType.Type, toPixelType.Type);
                loadDelegate = genericLoadPixels.CreateDelegate<LoadPixelsDelegate>();
                _loadPixelsDelegateCache.TryAdd(loadPixelsDelegateKey, loadDelegate);
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
            var loadPixelRowsTypes = typeof(LoadPixelRowsDelegate).GetDelegateParameters().GetGenericTypeDefinitions();
            loadPixelRowsTypes[loadPixelRowsTypes.Length - 1] = typeof(Image<>).GetGenericTypeDefinition();

            _loadPixelRowsMethod = typeof(Image<>).GetMethod("LoadPixels", loadPixelRowsTypes);
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
                loadDelegate = genericLoadPixels.CreateDelegate<LoadPixelRowsDelegate>();
                _loadPixelRowsDelegateCache.TryAdd(loadPixelsDelegateKey, loadDelegate);
            }
            return loadDelegate;
        }

        #endregion

        #region Create

        private delegate Image CreateDelegate(int width, int height);

        private static Type[] _createArgumentTypes;
        private static ConcurrentDictionary<VectorTypeInfo, CreateDelegate> _createDelegateCache;

        private static void SetupReflectionCreate()
        {
            _createArgumentTypes = typeof(CreateDelegate).GetDelegateParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            _createDelegateCache = new ConcurrentDictionary<VectorTypeInfo, CreateDelegate>(
                VectorTypeInfoEqualityComparer.Instance);
        }

        #endregion
    }
}
