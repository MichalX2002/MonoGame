using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace MonoGame.Framework.Graphics
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorTexture : IVertexType, IEquatable<VertexPositionColorTexture>
    {
        public static VertexDeclaration VertexDeclaration { get; } = new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        });

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        [DataMember]
        public Vector3 Position;

        [DataMember]
        public Color Color;

        [DataMember]
        public Vector2 TexCoord;

        public VertexPositionColorTexture(Vector3 position, Color color, Vector2 texCoord)
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }

        public readonly bool Equals(VertexPositionColorTexture other)
        {
            return other == this;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is VertexPositionColorTexture other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Position, Color, TexCoord);
        }

        public override readonly string ToString()
        {
            return "{Position:" + Position + " Color:" + Color + " TexCoord:" + TexCoord + "}";
        }

        public static bool operator ==(in VertexPositionColorTexture a, in VertexPositionColorTexture b)
        {
            return (a.Position == b.Position)
                && (a.Color == b.Color)
                && (a.TexCoord == b.TexCoord);
        }

        public static bool operator !=(in VertexPositionColorTexture a, in VertexPositionColorTexture b)
        {
            return !(a == b);
        }
    }
}
