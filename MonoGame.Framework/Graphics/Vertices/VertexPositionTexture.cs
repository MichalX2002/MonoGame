using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTexture : IVertexType
    {
        public static VertexDeclaration VertexDeclaration { get; } = new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        });
    
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public Vector3 Position;
        public Vector2 TextureCoordinate;

        public VertexPositionTexture(in Vector3 position, in Vector2 textureCoordinate)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, TextureCoordinate);
        }

        public override string ToString()
        {
            return "{{Position:" + Position + " TextureCoordinate:" + TextureCoordinate + "}}";
        }

        public static bool operator ==(in VertexPositionTexture left, in VertexPositionTexture right)
        {
            return (left.Position == right.Position)
                && (left.TextureCoordinate == right.TextureCoordinate);
        }

        public static bool operator !=(in VertexPositionTexture left, in VertexPositionTexture right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is VertexPositionTexture other && this == other;
        }
    }
}
