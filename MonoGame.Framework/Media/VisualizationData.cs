using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Media
{
    public class VisualizationData
    {
        internal const int Size = 256;

        internal float[] _frequencies;
        internal float[] _samples;

        private ReadOnlyCollection<float> _readFrequencies;
        private ReadOnlyCollection<float> _readSamples;

        public ReadOnlyCollection<float> Frequencies
        {
            get
            {
                if (_readFrequencies == null)
                    _readFrequencies = new ReadOnlyCollection<float>(_frequencies);
                return _readFrequencies;
            }
        }

        public ReadOnlyCollection<float> Samples
        {
            get
            {
                if (_readSamples == null)
                    _readSamples = new ReadOnlyCollection<float>(_samples);
                return _readSamples;
            }
        }
        
        public VisualizationData()
        {
            _frequencies = new float[Size];
            _samples = new float[Size];
        }
    }
}