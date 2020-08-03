// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;

namespace MonoGame.Framework.Graphics
{
    internal partial class Shader : GraphicsResource
    {
        /// <summary>
        /// Returns the platform specific shader profile identifier.
        /// </summary>
        public static int Profile => PlatformProfile();

        /// <summary>
        /// A hash value which can be used to compare shaders.
        /// </summary>
        internal int HashKey { get; private set; }

        public SamplerInfo[] Samplers { get; private set; }
        public int[] CBuffers { get; private set; }
        public ShaderStage Stage { get; private set; }
        public VertexAttribute[] Attributes { get; private set; }

        internal Shader(GraphicsDevice graphicsDevice, BinaryReader reader) : base(graphicsDevice)
        {
            bool isVertexShader = reader.ReadBoolean();
            Stage = isVertexShader ? ShaderStage.Vertex : ShaderStage.Pixel;

            int shaderLength = reader.ReadInt32();
            byte[] shaderBytecode = reader.ReadBytes(shaderLength);

            byte samplerCount = reader.ReadByte();
            Samplers = new SamplerInfo[samplerCount];
            for (int s = 0; s < samplerCount; s++)
            {
                var type  = (SamplerType)reader.ReadByte();
                int textureSlot = reader.ReadByte();
                int samplerSlot = reader.ReadByte();

                SamplerState? state = null;
                if (reader.ReadBoolean())
                {
                    state = new SamplerState
                    {
                        AddressU = (TextureAddressMode)reader.ReadByte(),
                        AddressV = (TextureAddressMode)reader.ReadByte(),
                        AddressW = (TextureAddressMode)reader.ReadByte(),
                        BorderColor = new Color(
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte()),
                        Filter = (TextureFilter)reader.ReadByte(),
                        MaxAnisotropy = reader.ReadInt32(),
                        MaxMipLevel = reader.ReadInt32(),
                        MipMapLevelOfDetailBias = reader.ReadSingle()
                    };
                }

                string name = reader.ReadString();
                int parameter = reader.ReadByte();

                Samplers[s] = new SamplerInfo(type, textureSlot, samplerSlot, name, state, parameter);
            }

            byte cbufferCount = reader.ReadByte();
            CBuffers = new int[cbufferCount];
            for (int c = 0; c < cbufferCount; c++)
                CBuffers[c] = reader.ReadByte();

            byte attributeCount = reader.ReadByte();
            Attributes = new VertexAttribute[attributeCount];
            for (int a = 0; a < attributeCount; a++)
            {
                string name = reader.ReadString();
                var usage = (VertexElementUsage)reader.ReadByte();
                int index = reader.ReadByte();
                int location = reader.ReadInt16();

                Attributes[a] = new VertexAttribute(usage, index, name, location);
            }

            PlatformConstruct(Stage, shaderBytecode);
        }

        protected override void GraphicsDeviceResetting()
        {
            PlatformGraphicsDeviceResetting();
        }
    }
}

