namespace MonoGame.Imaging
{
    public struct Configuration
    {
        public static Configuration Default => new Configuration(true, 90);

        public bool UseTgaRLE { get; }
        public int JpgQuality { get; }

        public Configuration(bool useTgaRLE, int jpgQuality) : this()
        {
            UseTgaRLE = useTgaRLE;
            JpgQuality = jpgQuality < 0 ? 90 : (jpgQuality > 100 ? 100 : jpgQuality);
        }
    }
}
