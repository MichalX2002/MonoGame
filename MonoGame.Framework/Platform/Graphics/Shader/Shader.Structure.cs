
namespace MonoGame.Framework.Graphics
{
    // TODO: We should convert the types below 
    // into the start of a Shader reflection API.

    internal enum SamplerType
    {
        Sampler2D = 0,
        SamplerCube = 1,
        SamplerVolume = 2,
        Sampler1D = 3,
    }

    internal struct SamplerInfo
    {
        public SamplerType Type;
        public int TextureSlot;
        public int SamplerSlot;
        public string Name;
        public SamplerState State;

        // TODO: This should be moved to EffectPass.
        public int Parameter;
    }

    internal struct VertexAttribute
    {
        public VertexElementUsage Usage;
        public int Index;
        public string Name;
        public int Location;
    }
}
