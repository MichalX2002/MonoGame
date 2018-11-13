using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Media
{
    public class VisualizationData
    {
        internal const int Size = 256;

        internal float[] _frequencies;
        internal float[] _samples;

        public ReadOnlyCollection<float> Frequencies { get; }
        public ReadOnlyCollection<float> Samples { get; }

        public VisualizationData()
        {
            _frequencies = new float[Size];
            _samples = new float[Size];

            Frequencies = new ReadOnlyCollection<float>(_frequencies);
            Samples = new ReadOnlyCollection<float>(_samples);
        }
    }
}