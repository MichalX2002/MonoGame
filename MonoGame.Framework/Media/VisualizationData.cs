using System.Collections.ObjectModel;

namespace MonoGame.Framework.Media
{
    public class VisualizationData
    {
        private const int Size = 256;

        internal float[] _frequencies;
        internal float[] _samples;

        public ReadOnlyCollection<float> Frequencies { get; }
        public ReadOnlyCollection<float> Samples { get; }

        private VisualizationData()
        {
            _frequencies = new float[Size];
            _samples = new float[Size];

            Frequencies = new ReadOnlyCollection<float>(_frequencies);
            Samples = new ReadOnlyCollection<float>(_samples);
        }
    }
}