using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorTexture : IVertexType
    {
        public static VertexDeclaration VertexDeclaration { get; } = new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        });

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoordinate;

        public VertexPositionColorTexture(in Vector3 position, in Color color, in Vector2 textureCoordinate)
        {
            Position = position;
            Color = color;
            TextureCoordinate = textureCoordinate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Color, TextureCoordinate);
        }

        public override string ToString()
        {
            return "{Position:" + Position + " Color:" + Color + " TextureCoordinate:" + TextureCoordinate + "}";
        }

        public static bool operator ==(in VertexPositionColorTexture left, in VertexPositionColorTexture right)
        {
            return (left.Position == right.Position)
                && (left.Color == right.Color)
                && (left.TextureCoordinate == right.TextureCoordinate);
        }

        public static bool operator !=(in VertexPositionColorTexture left, in VertexPositionColorTexture right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is VertexPositionColorTexture other && other == this;
        }
    }
}
