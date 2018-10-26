using MonoGame.Utilities.IO;

namespace MonoGame.Imaging
{
    public class SaveConfiguration
    {
        public delegate void ImageWritingDelegate(int writeCount);

        private static RecyclableMemoryManager _defaultMemory;
        public static RecyclableMemoryManager DefaultMemoryManager
        {
            get
            {
                if (_defaultMemory == null)
                {
                    _defaultMemory = new RecyclableMemoryManager(1024 * 32, 2, 1024 * 80)
                    {
                        AggressiveBufferReturn = true,
                        GenerateCallStacks = false
                    };
                }
                return _defaultMemory;
            }
        }

        private static SaveConfiguration _default;
        public static SaveConfiguration Default
        {
            get
            {
                if (_default == null)
                    _default = new SaveConfiguration(true, 90, DefaultMemoryManager);
                return _default;
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
