using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalTexture : IVertexType
    {
        public static VertexDeclaration VertexDeclaration { get; } = new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(0x18, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        });

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Normal, TextureCoordinate);
        }

        public override string ToString()
        {
            return "{{Position:" + Position + " Normal:" + Normal + " TextureCoordinate:" + TextureCoordinate + "}}";
        }

        public static bool operator ==(in VertexPositionNormalTexture left, in VertexPositionNormalTexture right)
        {
            return (left.Position == right.Position)
                && (left.Normal == right.Normal)
                && (left.TextureCoordinate == right.TextureCoordinate);
        }

        public static bool operator !=(in VertexPositionNormalTexture left, in VertexPositionNormalTexture right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is VertexPositionNormalTexture other && this == other;
        }
    }
}
