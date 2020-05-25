using System;

namespace MonoGame.Framework.Graphics
{
    public class SpriteBatchItem : IComparable<SpriteBatchItem>
    {
        public float SortKey;
        public Texture2D Texture;

        public VertexPositionColorTexture VertexTL;
        public VertexPositionColorTexture VertexTR;
        public VertexPositionColorTexture VertexBL;
        public VertexPositionColorTexture VertexBR;

        public void Set(
            float x, float y, float dx, float dy, float w, float h,
            float sin, float cos, Color color, in Vector4 texCoord, float depth)
        {
            // Should we be just assigning the Depth Value to Z?
            // We do according to:
            // http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx
            
            VertexTL.Position.X = x + dx * cos - dy * sin;
            VertexTL.Position.Y = y + dx * sin + dy * cos;
            VertexTL.Position.Z = depth;
            VertexTL.Color = color;
            VertexTL.TexCoord.X = texCoord.X;
            VertexTL.TexCoord.Y = texCoord.Y;

            VertexTR.Position.X = x + (dx + w) * cos - dy * sin;
            VertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
            VertexTR.Position.Z = depth;
            VertexTR.Color = color;
            VertexTR.TexCoord.X = texCoord.Z;
            VertexTR.TexCoord.Y = texCoord.Y;

            VertexBL.Position.X = x + dx * cos - (dy + h) * sin;
            VertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
            VertexBL.Position.Z = depth;
            VertexBL.Color = color;
            VertexBL.TexCoord.X = texCoord.X;
            VertexBL.TexCoord.Y = texCoord.W;

            VertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
            VertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
            VertexBR.Position.Z = depth;
            VertexBR.Color = color;
            VertexBR.TexCoord.X = texCoord.Z;
            VertexBR.TexCoord.Y = texCoord.W;
        }

        public void Set(float x, float y, float w, float h, Color color, in Vector4 texCoord, float depth)
        {
            VertexTL.Position.X = x;
            VertexTL.Position.Y = y;
            VertexTL.Position.Z = depth;
            VertexTL.Color = color;
            VertexTL.TexCoord.X = texCoord.X;
            VertexTL.TexCoord.Y = texCoord.Y;

            VertexTR.Position.X = x + w;
            VertexTR.Position.Y = y;
            VertexTR.Position.Z = depth;
            VertexTR.Color = color;
            VertexTR.TexCoord.X = texCoord.Z;
            VertexTR.TexCoord.Y = texCoord.Y;

            VertexBL.Position.X = x;
            VertexBL.Position.Y = y + h;
            VertexBL.Position.Z = depth;
            VertexBL.Color = color;
            VertexBL.TexCoord.X = texCoord.X;
            VertexBL.TexCoord.Y = texCoord.W;

            VertexBR.Position.X = x + w;
            VertexBR.Position.Y = y + h;
            VertexBR.Position.Z = depth;
            VertexBR.Color = color;
            VertexBR.TexCoord.X = texCoord.Z;
            VertexBR.TexCoord.Y = texCoord.W;
        }

        public int CompareTo(SpriteBatchItem other)
        {
            return SortKey.CompareTo(other.SortKey);
        }
    }
}
