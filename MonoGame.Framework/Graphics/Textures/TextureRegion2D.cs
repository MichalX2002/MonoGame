using System;

namespace MonoGame.Framework.Graphics
{
    public class TextureRegion2D
    {
        public string? Name { get; }
        public Texture2D Texture { get; protected set; }
        public object? Tag { get; set; }
        public RectangleF Bounds { get; protected set; }

        public float X => Bounds.X;
        public float Y => Bounds.Y;
        public float Width => Bounds.Width;
        public float Height => Bounds.Height;
        public SizeF Size => Bounds.Size;

        public float TexelWidth => 1f / Bounds.Width;
        public float TexelHeight => 1f / Bounds.Height;
        public SizeF TexelSize => new SizeF(TexelWidth, TexelHeight);

        public TextureRegion2D(Texture2D texture, string? name, RectangleF bounds)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Name = name;
            Bounds = bounds;
        }

        public TextureRegion2D(Texture2D texture, string? name, float x, float y, float width, float height)
            : this(texture, name, new RectangleF(x, y, width, height))
        {
        }

        public TextureRegion2D(Texture2D texture, RectangleF bounds)
            : this(texture, null, bounds)
        {
        }

        public TextureRegion2D(Texture2D texture, float x, float y, float width, float height)
            : this(texture, null, x, y, width, height)
        {
        }

        public TextureRegion2D(Texture2D texture)
            : this(texture, texture?.Name, 0, 0, texture?.Width ?? 0, texture?.Height ?? 0)
        {
        }

        public override string ToString()
        {
            if (Name != null)
                return $"{Name} {Bounds}";
            return Bounds.ToString();
        }
    }
}