using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Media
{
    public class VisualizationData
    {
        public const int MAX_SAMPLES = 48000;

        internal const int DefaultFrequencies = 256;
        private const int DefaultSamples = 1024;

        internal float[] _frequencies;
        internal float[] _samples;

        public ReadOnlyCollection<float> Frequencies { get; }
        public ReadOnlyCollection<float> Samples { get; }

        public VisualizationData() : this(DefaultSamples)
        {
        }

        public VisualizationData(int samplingSize)
        {
            if (samplingSize < 1 || samplingSize > MAX_SAMPLES)
                throw new ArgumentOutOfRangeException(nameof(samplingSize));

            _frequencies = new float[DefaultFrequencies];
            _samples = new float[samplingSize];

            Frequencies = new ReadOnlyCollection<float>(_frequencies);
            Samples = new ReadOnlyCollection<float>(_samples);
        }
    }
}