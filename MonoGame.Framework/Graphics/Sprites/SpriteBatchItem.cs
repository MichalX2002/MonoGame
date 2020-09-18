using System;

namespace MonoGame.Framework.Graphics
{
    public class SpriteBatchItem : IComparable<SpriteBatchItem>
    {
        public float SortKey;
        public Texture2D? Texture;
        public SpriteQuad Quad;

        public int CompareTo(SpriteBatchItem? other)
        {
            if (other == null)
                return 1;

            return SortKey.CompareTo(other.SortKey);
        }
    }
}
