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
                Samplers[s].Type = (SamplerType)reader.ReadByte();
                Samplers[s].TextureSlot = reader.ReadByte();
                Samplers[s].SamplerSlot = reader.ReadByte();

                if (reader.ReadBoolean())
                {
                    Samplers[s].State = new SamplerState
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

                Samplers[s].Name = reader.ReadString();
                Samplers[s].Parameter = reader.ReadByte();
            }

            byte cbufferCount = reader.ReadByte();
            CBuffers = new int[cbufferCount];
            for (int c = 0; c < cbufferCount; c++)
                CBuffers[c] = reader.ReadByte();

            byte attributeCount = reader.ReadByte();
            Attributes = new VertexAttribute[attributeCount];
            for (int a = 0; a < attributeCount; a++)
            {
                Attributes[a].Name = reader.ReadString();
                Attributes[a].Usage = (VertexElementUsage)reader.ReadByte();
                Attributes[a].Index = reader.ReadByte();
                Attributes[a].Location = reader.ReadInt16();
            }

            PlatformConstruct(Stage, shaderBytecode);
        }

        protected override void GraphicsDeviceResetting()
        {
            PlatformGraphicsDeviceResetting();
        }
    }
}

