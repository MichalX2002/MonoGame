
using System;

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

    internal readonly struct SamplerInfo
    {
        public SamplerType Type { get; }
        public int TextureSlot { get; }
        public int SamplerSlot { get; }
        public string Name { get; }
        public SamplerState? State { get; }

        // TODO: This should be moved to EffectPass.
        public int Parameter { get; }

        public SamplerInfo(
            SamplerType type, int textureSlot, int samplerSlot, string name, SamplerState? state, int parameter)
        {
            Type = type;
            TextureSlot = textureSlot;
            SamplerSlot = samplerSlot;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            State = state;
            Parameter = parameter;
        }
    }

    internal struct VertexAttribute
    {
        public VertexElementUsage Usage { get; }
        public int Index { get; }
        public string Name { get; }
        public int Location { get; set; }

        public VertexAttribute(VertexElementUsage usage, int index, string name, int location)
        {
            Usage = usage;
            Index = index;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Location = location;
        }
    }
}
