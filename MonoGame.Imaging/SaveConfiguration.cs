namespace MonoGame.Imaging
{
    public struct SaveConfiguration
    {
        public delegate void ImageWritingDelegate(int writeCount);

        public static SaveConfiguration Default => new SaveConfiguration(true, 90);

        public bool UseTgaRLE { get; }
        public int JpgQuality { get; }

        public SaveConfiguration(bool useTgaRLE, int jpgQuality)
        {
            UseTgaRLE = useTgaRLE;
            JpgQuality = jpgQuality < 0 ? 90 : (jpgQuality > 100 ? 100 : jpgQuality);
        }
    }
}
