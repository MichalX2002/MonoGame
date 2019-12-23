// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework.Content
{
    public partial class ContentManager : IDisposable
    {
        private const byte ContentCompressedLzx = 0x80;
        private const byte ContentCompressedLz4 = 0x40;

        private IGraphicsDeviceService _graphicsDeviceService;
        private HashSet<IDisposable> _disposableAssets = new HashSet<IDisposable>();
        private bool _disposed;

        private static readonly object ContentManagerLock = new object();
        private static List<WeakReference> ContentManagers = new List<WeakReference>();

        private static readonly List<char> targetPlatformIdentifiers = new List<char>()
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

            // NOTE: There are additional idenfiers for consoles that 
            // are not defined in this repository.  Be sure to ask the
            // console port maintainers to ensure no collisions occur.

            
            // Legacy identifiers... these could be reused in the
            // future if we feel enough time has passed.

            'p', // PlayStationMobile
            'g', // Windows (OpenGL)
            'l', // Linux
        };

        public string RootDirectory { get; set; } = string.Empty;
        internal string RootDirectoryFullPath => Path.Combine(TitleContainer.Location, RootDirectory);

        public IServiceProvider ServiceProvider { get; }

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
                // Check if the list contains this content manager already. Also take
                // the opportunity to prune the list of any finalized content managers.
                bool contains = false;
                for (int i = ContentManagers.Count - 1; i >= 0; --i)
                {
                    var contentRef = ContentManagers[i];
                    if (ReferenceEquals(contentRef.Target, contentManager))
                        contains = true;
                    if (!contentRef.IsAlive)
                        ContentManagers.RemoveAt(i);
                }
                if (!contains)
                    ContentManagers.Add(new WeakReference(contentManager));
            }
        }

        private static void RemoveContentManager(ContentManager contentManager)
        {
            lock (ContentManagerLock)
            {
                // Check if the list contains this content manager and remove it. Also
                // take the opportunity to prune the list of any finalized content managers.
                for (int i = ContentManagers.Count - 1; i >= 0; --i)
                {
                    var contentRef = ContentManagers[i];
                    if (!contentRef.IsAlive || ReferenceEquals(contentRef.Target, contentManager))
                        ContentManagers.RemoveAt(i);
                }
            }
        }

        internal static void ReloadGraphicsContent()
        {
            lock (ContentManagerLock)
            {
                // Reload the graphic assets of each content manager. Also take the
                // opportunity to prune the list of any finalized content managers.
                for (int i = ContentManagers.Count - 1; i >= 0; --i)
                {
                    var contentRef = ContentManagers[i];
                    if (contentRef.IsAlive)
                    {
                        var contentManager = (ContentManager)contentRef.Target;
                        if (contentManager != null)
                            contentManager.ReloadGraphicsAssets();
                    }
                    else
                    {
                        ContentManagers.RemoveAt(i);
                    }
                }
            }
        }

        ~ContentManager()
        {
            Dispose(false);
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

        public bool AssetFileExists(string assetName)
        {
            return File.Exists(GetAssetPath(assetName));
        }

        public string GetAssetPath(string assetName)
        {
            return Path.Combine(RootDirectory, assetName).Replace('\\', '/') + ".xnb";
        }

        private static readonly string[] _cultureNames =
        {
            CultureInfo.CurrentCulture.Name,                        // eg. "en-US"
            CultureInfo.CurrentCulture.TwoLetterISOLanguageName     // eg. "en"
        };

        public virtual T LoadLocalized<T>(string assetName)
        {
            // Look first for a specialized language-country version of the asset,
            // then if that fails, loop back around to see if we can find one that
            // specifies just the language without the country part.
            foreach (string cultureName in _cultureNames)
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
            Stream stream;
            try
            {
                var assetPath = Path.Combine(RootDirectory, assetName) + ".xnb";

                // This is primarily for editor support. 
                // Setting the RootDirectory to an absolute path is useful in editor
                // situations, but TitleContainer can ONLY be passed relative paths.                
#if DESKTOPGL || WINDOWS
                if (Path.IsPathRooted(assetPath))
                    stream = File.OpenRead(assetPath);
                else
#endif
                    stream = TitleContainer.OpenStream(assetPath);

#if ANDROID
                stream = RecyclableMemoryManager.Instance.GetBufferedStream(stream, leaveOpen: false);
#endif
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
            return stream;
        }

        protected T ReadAsset<T>(string assetName, Action<IDisposable> recordDisposableObject)
        {
            T asset = ReadAssetCore(assetName, default(T), recordDisposableObject);

            if (asset is GraphicsResource graphicsResult)
                graphicsResult.Name = assetName;

            return asset;
        }

        private ContentReader GetContentReaderFromXnb(
            string originalAssetName, Stream stream, BinaryReader xnbReader, Action<IDisposable> recordDisposableObject)
        {
            // The first 4 bytes should be the "XNB" header. i use that to detect an invalid file
            byte x = xnbReader.ReadByte();
            byte n = xnbReader.ReadByte();
            byte b = xnbReader.ReadByte();
            byte platform = xnbReader.ReadByte();

            if (x != 'X' || n != 'N' || b != 'B' || !targetPlatformIdentifiers.Contains((char)platform))
            {
                throw new ContentLoadException(
                    "Asset does not appear to be a valid XNB file. Did you process your content for Windows?");
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

            // Avoid recording disposable objects twice. ReloadAsset will try to record the disposables again.
            // We don't know which asset recorded which disposable so just guard against storing multiple of the same instance.
            if (disposable != null)
                _disposableAssets.Add(disposable);
        }

        /// <summary>
        /// Allows a custom <see cref="ContentManager"/> to have it's assets reloaded.
        /// </summary>
        protected Dictionary<string, object> LoadedAssets { get; } =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        protected virtual void ReloadGraphicsAssets()
        {
            var methodInfo = ReflectionHelpers.GetMethodInfo(typeof(ContentManager), nameof(ReloadAsset));
            var paramArray = new object[2];

            foreach (var asset in LoadedAssets)
            {
                var typeOfValue = asset.Value.GetType();

                // This never executes as asset.Key is never null. This just forces the 
                // linker to include the ReloadAsset method when AOT compiled.
                if (asset.Key == null)
                    ReloadAsset(asset.Key, Convert.ChangeType(asset.Value, typeOfValue));

                paramArray[0] = asset.Key;
                paramArray[1] = Convert.ChangeType(asset.Value, typeOfValue);
                methodInfo.MakeGenericMethod(typeOfValue).Invoke(this, paramArray);
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
