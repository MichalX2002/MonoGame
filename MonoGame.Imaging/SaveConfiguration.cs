using MonoGame.Utilities.Memory;

namespace MonoGame.Imaging
{
    public class SaveConfiguration
    {
        public delegate void ImageWritingDelegate(int writeCount);

        private static readonly object _initLock = new object();

        private static SaveConfiguration _default;
        public static SaveConfiguration Default
        {
            get
            {
                lock (_initLock)
                {
                    if (_default == null)
                        _default = new SaveConfiguration(true, 90, RecyclableMemoryManager.Instance);
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
