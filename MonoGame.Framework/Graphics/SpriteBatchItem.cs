using System;
using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    public class SpriteBatchItem : IComparable<SpriteBatchItem>
    {
        public float SortKey;
        public Texture2D? Texture;

        public VertexPositionColorTexture VertexTL;
        public VertexPositionColorTexture VertexTR;
        public VertexPositionColorTexture VertexBL;
        public VertexPositionColorTexture VertexBR;

        public void Set(
            float x, float y, float dx, float dy, float w, float h,
            float sin, float cos, Color color, Vector4 texCoord, float depth)
        {
            // Should we be just assigning the Depth Value to Z?
            // We do according to:
            // http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx

            VertexTL.Position = new Vector3(
                x + dx * cos - dy * sin,
                y + dx * sin + dy * cos,
                depth);
            VertexTL.Color = color;
            VertexTL.TexCoord = new Vector2(texCoord.X, texCoord.Y);

            VertexTR.Position = new Vector3(
                x + (dx + w) * cos - dy * sin,
                y + (dx + w) * sin + dy * cos,
                depth);
            VertexTR.Color = color;
            VertexTR.TexCoord = new Vector2(texCoord.Z, texCoord.Y);

            VertexBL.Position = new Vector3(
                x + dx * cos - (dy + h) * sin,
                y + dx * sin + (dy + h) * cos,
                depth);
            VertexBL.Color = color;
            VertexBL.TexCoord = new Vector2(texCoord.X, texCoord.W);

            VertexBR.Position = new Vector3(
                x + (dx + w) * cos - (dy + h) * sin,
                y + (dx + w) * sin + (dy + h) * cos,
                depth);
            VertexBR.Color = color;
            VertexBR.TexCoord = new Vector2(texCoord.Z, texCoord.W);
        }

        public void Set(float x, float y, float w, float h, Color color, Vector4 texCoord, float depth)
        {
            VertexTL.Position = new Vector3(x, y, depth);
            VertexTL.Color = color;
            VertexTL.TexCoord = new Vector2(texCoord.X, texCoord.Y);

            VertexTR.Position = new Vector3(x + w, y, depth);
            VertexTR.Color = color;
            VertexTR.TexCoord = new Vector2(texCoord.Z, texCoord.Y);

            VertexBL.Position = new Vector3(x, y + h, depth);
            VertexBL.Color = color;
            VertexBL.TexCoord = new Vector2(texCoord.X, texCoord.W);

            VertexBR.Position = new Vector3(x + w, y + h, depth);
            VertexBR.Color = color;
            VertexBR.TexCoord = new Vector2(texCoord.Z, texCoord.W);
        }

        public int CompareTo(SpriteBatchItem? other)
        {
            if (other == null)
                return 1;

            return SortKey.CompareTo(other.SortKey);
        }
    }
}
