using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public static readonly VertexDeclaration VertexDeclaration;
        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override int GetHashCode()
        {
            unchecked
            {
                int code = Position.GetHashCode();
                code = code * 23 + Normal.GetHashCode();
                return code * 23 + TextureCoordinate.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "{{Position:" + Position + " Normal:" + Normal + " TextureCoordinate:" + TextureCoordinate + "}}";
        }

        public static bool operator ==(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
        {
            return (((left.Position == right.Position) && (left.Normal == right.Normal)) && (left.TextureCoordinate == right.TextureCoordinate));
        }

        public static bool operator !=(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != base.GetType())
            {
                return false;
            }
            return (this == ((VertexPositionNormalTexture)obj));
        }

        static VertexPositionNormalTexture()
        {
            VertexElement[] elements = new VertexElement[] { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(0x18, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0) };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }
}
