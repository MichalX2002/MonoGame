using System;
using System.Collections.Generic;
using System.Numerics;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;

namespace MonoGame.Testing
{
    public class FontGlyph
    {
        public FontGlyphHeightMap HeightMap { get; }
        public Texture2D Texture { get; }
        public int GlyphIndex { get; }
        public RectangleF SourceRect { get; }
        public RectangleF DestinationRect { get; }

        public Vector2 Scale => HeightMap.Scale;

        public FontGlyph(
            FontGlyphHeightMap heightMap, Texture2D texture, int glyphIndex,
            RectangleF sourceRect, RectangleF destinationRect)
        {
            HeightMap = heightMap ?? throw new ArgumentNullException(nameof(heightMap));
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            GlyphIndex = glyphIndex;
            SourceRect = sourceRect;
            DestinationRect = destinationRect;
        }
    }

    public class FontGlyphLayout
    {

    }

    public class FontGlyphLookup
    {
        public Font Font { get; }

        public Dictionary<int, FontGlyph?> Glyphs { get; } =
            new Dictionary<int, FontGlyph?>();

        public FontGlyphLookup(Font font)
        {
            Font = font ?? throw new ArgumentNullException(nameof(font));
        }
    }

    public class FontGlyphHeightMap
    {
        public float PixelHeight { get; }
        public Vector2 Scale { get; }

        public Dictionary<Font, FontGlyphLookup> Lookups { get; } =
            new Dictionary<Font, FontGlyphLookup>();

        public FontGlyphHeightMap(float pixelHeight, Vector2 scale)
        {
            PixelHeight = pixelHeight;
            Scale = scale;
        }
    }

    public class FontAtlas
    {
        public Texture2D Texture { get; }
        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public int Padding { get; } = 2;

        public int Column;
        public int Row;

        public int CurrentX;
        public int CurrentY;
        public int TotalY;

        public FontAtlas(Texture2D texture)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        }

        public bool Insert(Size size, out Point position)
        {
            position = default;

            if (size.Width > Width - CurrentX ||
                size.Height > Height - TotalY)
            {
                TotalY += CurrentY + Padding;

                CurrentX = 0;
                CurrentY = 0;
                return false;
            }

            position.X = CurrentX;
            position.Y = TotalY;

            CurrentX += size.Width + Padding;
            if (size.Height > CurrentY)
                CurrentY = size.Height;

            return true;
        }
    }

    public class FontSystem
    {
        private Dictionary<float, FontGlyphHeightMap> _heightMaps = new();
        public List<FontAtlas> _atlases = new();

        public GraphicsDevice GraphicsDevice { get; }
        public int TextureSize { get; }
        public SurfaceFormat Format { get; }

        public FontSystem(
            GraphicsDevice graphicsDevice, int textureSize, SurfaceFormat format)
        {
            GraphicsDevice = graphicsDevice ??
                throw new ArgumentNullException(nameof(graphicsDevice));

            TextureSize = textureSize;
            Format = format;
        }

        public static float RoundPixelHeight(float pixelHeight, float min, float max)
        {
            if (pixelHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(pixelHeight));

            float minHeight = 16;
            float maxHeight = 128;

            int log2 = BitOperations.Log2((uint)MathF.Round(pixelHeight));
            int nextPowOf2 = 1 << (log2 + 1);
            float value = nextPowOf2;

            float clampedHeight = MathHelper.Clamp(value, minHeight, maxHeight);
            return MathF.Round(clampedHeight);
        }

        //public FontGlyphLayout? GetGlyphLayout(Font font, int glyphIndex)
        //{
        //}

        public FontGlyph? GetGlyph(Font font, int glyphIndex, float pixelHeight)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            Point oversample = new(2, 2);
            pixelHeight = RoundPixelHeight(pixelHeight, 16, 128);

            if (!_heightMaps.TryGetValue(pixelHeight, out FontGlyphHeightMap? heightMap))
            {
                float scale = font.GetScaleByPixel(pixelHeight);
                heightMap = new FontGlyphHeightMap(pixelHeight, new Vector2(scale));
                _heightMaps.Add(pixelHeight, heightMap);

                Console.WriteLine("New height: " + heightMap.PixelHeight);
            }

            var lookups = heightMap.Lookups;
            if (!lookups.TryGetValue(font, out FontGlyphLookup? lookup))
            {
                lookup = new FontGlyphLookup(font);
                lookups.Add(font, lookup);
            }

            var glyphs = lookup.Glyphs;
            if (!glyphs.TryGetValue(glyphIndex, out FontGlyph? glyph))
            {
                if (font.TryGetGlyphBitmap(
                    glyphIndex, heightMap.Scale, oversample, Vector2.Zero,
                    out Image<Alpha8>? image, out RectangleF dstRect))
                {
                    using (image)
                    {
                        var srcRect = new Rectangle(0, 0, image.Width, image.Height);

                        FontAtlas? array = null;
                        for (int i = 0; i < _atlases.Count; i++)
                        {
                            if (_atlases[i].Insert(srcRect.Size, out Point position))
                            {
                                srcRect.Position = position;
                                array = _atlases[i];
                                break;
                            }
                        }

                        if (array == null)
                        {
                            var texture = new Texture2D(
                                GraphicsDevice, TextureSize, TextureSize, mipmap: false, Format);

                            array = new FontAtlas(texture);
                            _atlases.Add(array);

                            if (!array.Insert(srcRect.Size, out Point position))
                            {
                                throw new Exception(
                                    "Failed to insert texture into new texture array.");
                            }
                            srcRect.Position = position;
                        }

                        glyph = new FontGlyph(heightMap, array.Texture, glyphIndex, srcRect, dstRect);
                        glyph.Texture.SetData(image, srcRect);
                    }
                }

                glyphs.Add(glyphIndex, glyph);
            }

            return glyph;
        }
    }
}
