using MonoGame.Utilities.IO;

namespace MonoGame.Imaging
{
    public class SaveConfiguration
    {
        public delegate void ImageWritingDelegate(int writeCount);

        private static readonly object _initLock = new object();

        private static RecyclableMemoryManager _defaultMemory;
        public static RecyclableMemoryManager DefaultMemoryManager
        {
            get
            {
                lock (_initLock)
                {
                    if (_defaultMemory == null)
                    {
                        _defaultMemory = new RecyclableMemoryManager(1024 * 64, 1024 * 64, 1024 * 256, false)
                        {
                            AggressiveBufferReturn = true,
                            GenerateCallStacks = false
                        };
                    }
                    return _defaultMemory;
                }
            }
        }

        private static SaveConfiguration _default;
        public static SaveConfiguration Default
        {
            get
            {
                lock (_initLock)
                {
                    if (_default == null)
                        _default = new SaveConfiguration(true, 90, DefaultMemoryManager);
                    return _default;
                }
            }
        }

        public bool UseTgaRLE { get; }
        public int JpgQuality { get; }
        public RecyclableMemoryManager MemoryManager { get; }

        public SaveConfiguration(bool useTgaRLE, int jpgQuality, RecyclableMemoryManager manager)
        {
            UseTgaRLE = useTgaRLE;
            JpgQuality = jpgQuality < 0 ? 90 : (jpgQuality > 100 ? 100 : jpgQuality);
            MemoryManager = manager;
        }
    }
}
