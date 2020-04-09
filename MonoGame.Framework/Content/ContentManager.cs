// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Memory;

namespace MonoGame.Framework.Content
{
    public partial class ContentManager : IDisposable
    {
        private delegate void ReloadAssetDelegate(ContentManager manager, string key, object value);

        private const byte ContentCompressedLzx = 0x80;
        private const byte ContentCompressedLz4 = 0x40;

        private IGraphicsDeviceService _graphicsDeviceService;
        private HashSet<IDisposable> _disposableAssets = new HashSet<IDisposable>();
        private bool _disposed;

        private static ReadOnlyMemory<string> CultureNames { get; } = new[]
        {
            CultureInfo.CurrentCulture.Name,                     // eg. "en-US"
            CultureInfo.CurrentCulture.TwoLetterISOLanguageName  // eg. "en"
        };

        private static object ContentManagerLock { get; } = new object();

        private static List<WeakReference<ContentManager>> ContentManagers { get; } =
            new List<WeakReference<ContentManager>>();

        private static ConcurrentDictionary<string, ReloadAssetDelegate> ReloadAssetDelegateCache { get; } =
            new ConcurrentDictionary<string, ReloadAssetDelegate>();

        private static List<char> _targetPlatformIdentifiers = new List<char>()
        {
            'w', // Windows (XNA & DirectX)
            'x', // Xbox360 (XNA)
            'm', // WindowsPhone7.0 (XNA)
            'i', // iOS
            'a', // Android
            'd', // DesktopGL
            'X', // MacOSX
            'W', // WindowsStoreApp
            'n', // NativeClient
            'M', // WindowsPhone8
            'r', // RaspberryPi
            'P', // PlayStation4
            'v', // PSVita
            'O', // XboxOne
            'S', // Nintendo Switch
            'G', // Google Stadia

            // NOTE: There are additional idenfiers for consoles that 
            // are not defined in this repository.  Be sure to ask the
            // console port maintainers to ensure no collisions occur.

            
            // Legacy identifiers... these could be reused in the
            // future if we feel enough time has passed.

            'p', // PlayStationMobile
            'g', // Windows (OpenGL)
            'l', // Linux
        };

        /// <summary>
        /// Allows a custom <see cref="ContentManager"/> to have it's assets reloaded.
        /// </summary>
        protected Dictionary<string, object> LoadedAssets { get; } =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public string RootDirectory { get; set; } = string.Empty;
        public IServiceProvider ServiceProvider { get; }

        internal string RootDirectoryFullPath => Path.Combine(TitleContainer.Location, RootDirectory);

        static ContentManager()
        {
            // Allow any per-platform static initialization to occur.
            PlatformStaticInit();
        }

        static partial void PlatformStaticInit();

        private static void AddContentManager(ContentManager contentManager)
        {
            lock (ContentManagerLock)
            {
                // Check if the list contains this content manager already. 
                // Also take the opportunity to prune the list of any finalized content managers.
                bool contains = false;
                for (int i = ContentManagers.Count; i-- > 0;)
                {
                    if (!ContentManagers[i].TryGetTarget(out var target))
                        ContentManagers.RemoveAt(i);

                    if (ReferenceEquals(target, contentManager))
                        contains = true;
                }
                if (!contains)
                    ContentManagers.Add(new WeakReference<ContentManager>(contentManager));
            }
        }

        private static void RemoveContentManager(ContentManager contentManager)
        {
            lock (ContentManagerLock)
            {
                // Check if the list contains this content manager and remove it. 
                // Also take the opportunity to prune the list of any finalized content managers.
                for (int i = ContentManagers.Count; i-- > 0;)
                {
                    if (!ContentManagers[i].TryGetTarget(out var target) ||
                        ReferenceEquals(target, contentManager))
                        ContentManagers.RemoveAt(i);
                }
            }
        }

        internal static void ReloadGraphicsContent()
        {
            lock (ContentManagerLock)
            {
                // Reload the graphic assets of each content manager. 
                // Also take the opportunity to prune the list of any finalized content managers.
                for (int i = ContentManagers.Count; i-- > 0;)
                {
                    if (ContentManagers[i].TryGetTarget(out var target))
                        target.ReloadGraphicsAssets();
                    else
                        ContentManagers.RemoveAt(i);
                }
            }
        }

        public ContentManager(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            AddContentManager(this);
        }

        public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            RootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));
            AddContentManager(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            // Once disposed, content manager wont be used again
            RemoveContentManager(this);
        }

        // If disposing is true, it was called explicitly and we should dispose managed objects.
        // If disposing is false, it was called by the finalizer and managed objects should not be disposed.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Unload();

                _disposed = true;
            }
        }

        ~ContentManager()
        {
            Dispose(false);
        }

        public string GetAssetPath(string assetName)
        {
            return Path.Combine(RootDirectory, assetName).Replace('\\', '/') + ".xnb";
        }

        public bool AssetFileExists(string assetName)
        {
            return File.Exists(GetAssetPath(assetName));
        }

        public virtual T LoadLocalized<T>(string assetName)
        {
            // Look first for a specialized language-country version of the asset,
            // then if that fails, loop back around to see if we can find one that
            // specifies just the language without the country part.
            foreach (string cultureName in CultureNames.Span)
            {
                try
                {
                    string localizedAssetName = assetName + '.' + cultureName;
                    return Load<T>(localizedAssetName);
                }
                catch (ContentLoadException exception)
                {
                    if (!(exception.InnerException is FileNotFoundException))
                        throw exception;
                }
            }

            // If we didn't find any localized asset, fall back to the default name.
            return Load<T>(assetName);
        }

        public virtual T Load<T>(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException(nameof(assetName));

            if (_disposed)
                throw new ObjectDisposedException(nameof(ContentManager));

            // On some platforms, name and slash direction matter.
            // We store the asset by a /-seperating key rather than how the
            // path to the file was passed to us to avoid
            // loading "content/asset1.xnb" and "content\\ASSET1.xnb" as if they were two 
            // different files. This matches stock XNA behavior.
            // The dictionary will ignore case differences
            var key = assetName.Replace('\\', '/');

            // Check for a previously loaded asset first
            if (LoadedAssets.TryGetValue(key, out object asset))
            {
                if (asset is T typedAsset)
                    return typedAsset;
            }

            // Load the asset.
            T result = ReadAsset<T>(assetName, null);

            LoadedAssets[key] = result;
            return result;
        }

        protected virtual Stream OpenStream(string assetName)
        {
            try
            {
                var assetPath = Path.Combine(RootDirectory, assetName) + ".xnb";
                Stream stream;

#if DESKTOPGL || WINDOWS
                // This is primarily for editor support. 
                // Setting the RootDirectory to an absolute path is useful in editor
                // situations, but TitleContainer can ONLY be passed relative paths.  
                if (Path.IsPathRooted(assetPath))
                {
                    stream = File.OpenRead(assetPath);
                }
                else
#endif
                {
                    stream = TitleContainer.OpenStream(assetPath);
                }
#if ANDROID
                stream = Utilities.RecyclableMemoryManager.Default.GetBufferedStream(stream, leaveOpen: false);
#endif
                return stream;
            }
            catch (FileNotFoundException fileNotFound)
            {
                throw new ContentLoadException("The content file was not found.", fileNotFound);
            }
#if !WINDOWS_UAP
            catch (DirectoryNotFoundException directoryNotFound)
            {
                throw new ContentLoadException("The directory was not found.", directoryNotFound);
            }
#endif
            catch (Exception exception)
            {
                throw new ContentLoadException("Failed to open stream.", exception);
            }
        }

        protected T ReadAsset<T>(string assetName, Action<IDisposable> recordDisposableObject)
        {
            T asset = ReadAssetCore(assetName, default(T), recordDisposableObject);

            if (asset is GraphicsResource graphicsResult)
                graphicsResult.Name = assetName;

            return asset;
        }

        private ContentReader GetContentReaderFromXnb(
            string originalAssetName, Stream stream, BinaryReader xnbReader, 
            Action<IDisposable> recordDisposableObject)
        {
            // The first 4 bytes should be the "XNB" header. i use that to detect an invalid file
            byte x = xnbReader.ReadByte();
            byte n = xnbReader.ReadByte();
            byte b = xnbReader.ReadByte();
            byte platform = xnbReader.ReadByte();

            if (x != 'X' || n != 'N' || b != 'B' || !_targetPlatformIdentifiers.Contains((char)platform))
            {
                throw new ContentLoadException(
                    "Asset does not appear to be a valid XNB file. " +
                    "Was the content processed for the wrong platform?");
            }

            byte version = xnbReader.ReadByte();
            byte flags = xnbReader.ReadByte();

            bool compressedLzx = (flags & ContentCompressedLzx) != 0;
            bool compressedLz4 = (flags & ContentCompressedLz4) != 0;
            if (version != 5 && version != 4)
                throw new ContentLoadException("Invalid XNB version.");

            // The next int32 is the length of the XNB file
            int xnbLength = xnbReader.ReadInt32();

            Stream decompressedStream = null;
            if (compressedLzx || compressedLz4)
            {
                // Decompress the xnb
                int decompressedSize = xnbReader.ReadInt32();

                if (compressedLzx)
                {
                    int compressedSize = xnbLength - 14;
                    decompressedStream = new LzxDecoderStream(stream, decompressedSize, compressedSize);
                }
                else if (compressedLz4)
                {
                    decompressedStream = new Lz4DecoderStream(stream);
                }
            }
            else
            {
                decompressedStream = stream;
            }

            var reader = new ContentReader(
                this, decompressedStream, originalAssetName, version, recordDisposableObject);

            return reader;
        }

        /// <summary>
        /// Adds a disposable object to a set that will be disposed on <see cref="Unload"/>.
        /// Assets loaded through typical means are automatically recorded for disposal.
        /// </summary>
        public void RecordDisposable(IDisposable disposable)
        {
            Debug.Assert(disposable != null, "The disposable is null!");

            // Avoid recording disposable objects twice. 
            // ReloadAsset will try to record the disposables again.
            // We don't know which asset recorded which disposable so 
            // just guard against storing multiple of the same instance.
            if (disposable != null)
                _disposableAssets.Add(disposable);
        }

        public virtual void ReloadGraphicsAssets()
        {
            foreach (var asset in LoadedAssets)
            {
                if (!ReloadAssetDelegateCache.TryGetValue(asset.Key, out var reloadAssetDelegate))
                {
                    var methodInfo = ((Action<string, object>)ReloadAsset).Method.GetGenericMethodDefinition();
                    var genericMethod = methodInfo.MakeGenericMethod(asset.Value.GetType());

                    var managerParam = Expression.Parameter(typeof(ContentManager));
                    var assetNameParam = Expression.Parameter(typeof(string));
                    var assetParam = Expression.Parameter(typeof(object));

                    var convertedAsset = Expression.Convert(assetParam, asset.Value.GetType());
                    var methodCall = Expression.Call(
                        managerParam, genericMethod, assetNameParam, convertedAsset);

                    var parameters = new[] { managerParam, assetNameParam, assetParam };
                    var lambda = Expression.Lambda<ReloadAssetDelegate>(methodCall, parameters);
                    reloadAssetDelegate = lambda.Compile();

                    ReloadAssetDelegateCache.TryAdd(asset.Key, reloadAssetDelegate);
                }

                reloadAssetDelegate.Invoke(this, asset.Key, asset.Value);
            }
        }

        protected virtual void ReloadAsset<T>(string originalAssetName, T currentAsset)
        {
            ReadAssetCore(originalAssetName, currentAsset, null);
        }

        protected virtual T ReadAssetCore<T>(
            string assetName, T currentAsset, Action<IDisposable> recordDisposableObject)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException(nameof(assetName));

            if (_disposed)
                throw new ObjectDisposedException(nameof(ContentManager));

            if (_graphicsDeviceService == null)
            {
                _graphicsDeviceService = ServiceProvider.GetService<IGraphicsDeviceService>();
                if (_graphicsDeviceService == null)
                    throw new InvalidOperationException(FrameworkResources.NoGraphicsDeviceService);
            }

            using (var stream = OpenStream(assetName))
            using (var xnbReader = new BinaryReader(stream))
            using (var reader = GetContentReaderFromXnb(assetName, stream, xnbReader, recordDisposableObject))
            {
                T asset = reader.ReadAsset(currentAsset);
                if (asset == null)
                    throw new ContentLoadException("Failed to load asset: " + assetName);
                return asset;
            }
        }

        /// <summary>
        /// Disposes and clears references to loaded assets.
        /// </summary>
        public virtual void Unload()
        {
            // Look for disposable assets.
            foreach (var disposable in _disposableAssets)
                disposable.Dispose();
            _disposableAssets.Clear();

            LoadedAssets.Clear();
        }

        internal byte[] GetScratchBuffer(int size)
        {
            return RecyclableMemoryManager.Default.GetLargeBuffer(size, nameof(ContentManager));
        }

        internal void ReturnScratchBuffer(byte[] buffer)
        {
            RecyclableMemoryManager.Default.ReturnLargeBuffer(buffer, nameof(ContentManager));
        }
    }
}
