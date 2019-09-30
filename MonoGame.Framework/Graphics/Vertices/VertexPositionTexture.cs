using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VertexPositionTexture : IVertexType
    {
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public static readonly VertexDeclaration VertexDeclaration;
        public VertexPositionTexture(Vector3 position, Vector2 textureCoordinate)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override int GetHashCode()
        {
            int code = Position.GetHashCode();
            return code * 23 + TextureCoordinate.GetHashCode();
        }

        public override string ToString()
        {
            return "{{Position:" + Position + " TextureCoordinate:" + TextureCoordinate + "}}";
        }

        public static bool operator ==(VertexPositionTexture left, VertexPositionTexture right)
        {
            return ((left.Position == right.Position) && (left.TextureCoordinate == right.TextureCoordinate));
        }

        public static bool operator !=(VertexPositionTexture left, VertexPositionTexture right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return (this == ((VertexPositionTexture)obj));
        }

        static VertexPositionTexture()
        {
            VertexElement[] elements = new VertexElement[] { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0) };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

    }
}
