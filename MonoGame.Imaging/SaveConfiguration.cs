namespace MonoGame.Imaging
{
    public struct SaveConfiguration
    {
        public delegate void ImageWritingDelegate(int writeCount);

        public static SaveConfiguration Default => new SaveConfiguration(true, 90, null);

        public bool UseTgaRLE { get; }
        public int JpgQuality { get; }
        public ImageWritingDelegate OnWrite { get; }

        public SaveConfiguration(
            bool useTgaRLE, int jpgQuality, ImageWritingDelegate onWrite) : this()
        {
            UseTgaRLE = useTgaRLE;
            JpgQuality = jpgQuality < 0 ? 90 : (jpgQuality > 100 ? 100 : jpgQuality);
            OnWrite = onWrite;
        }

        public SaveConfiguration(ImageWritingDelegate onWrite) :
            this(Default.UseTgaRLE, Default.JpgQuality, onWrite)
        {
        }
    }
}
