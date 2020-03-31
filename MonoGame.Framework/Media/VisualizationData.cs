using System;

namespace MonoGame.Framework.Media
{
    public class VisualizationData
    {
        private const int Size = 256;

        internal float[] _frequencies;
        internal float[] _samples;

        public ReadOnlyMemory<float> Frequencies { get; }
        public ReadOnlyMemory<float> Samples { get; }

        private VisualizationData()
        {
            _frequencies = new float[Size];
            _samples = new float[Size];

            Frequencies = _frequencies;
            Samples = _samples;
        }
    }
}