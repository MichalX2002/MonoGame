using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;

namespace MonoGame.Testing
{
    public class FontGlyph
    {
        public FontGlyphHeightMap HeightMap { get; }
        public Texture2D Texture { get; }
        public int GlyphIndex { get; }
        public Rectangle TextureRect { get; }

        public Vector2 Scale => HeightMap.Scale;

        public FontGlyph(
            FontGlyphHeightMap heightMap, Texture2D texture, int glyphIndex, Rectangle textureRect)
        {
            HeightMap = heightMap ?? throw new ArgumentNullException(nameof(heightMap));
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            GlyphIndex = glyphIndex;
            TextureRect = textureRect;
        }
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

    public class FontTextureArray
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

        public FontTextureArray(Texture2D texture)
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

    public class FontTextureCache
    {
        private Dictionary<float, FontGlyphHeightMap> _map = new Dictionary<float, FontGlyphHeightMap>();
        public List<FontTextureArray> _textureStates = new List<FontTextureArray>();

        public GraphicsDevice GraphicsDevice { get; }
        public int TextureSize { get; }
        public SurfaceFormat Format { get; }

        public FontTextureCache(
            GraphicsDevice graphicsDevice, int textureSize, SurfaceFormat format)
        {
            GraphicsDevice = graphicsDevice ??
                throw new ArgumentNullException(nameof(graphicsDevice));

            TextureSize = textureSize;
            Format = format;
        }

        public float RoundPixelHeight(float pixelHeight)
        {
            if (pixelHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(pixelHeight));

            float minHeight = 16;
            float maxHeight = 256;

            int log2 = BitOperations.Log2((uint)MathF.Round(pixelHeight));
            int nextPowOf2 = 1 << (log2 + 1);
            float value = nextPowOf2;

            float clampedHeight = MathHelper.Clamp(value, minHeight, maxHeight);
            return clampedHeight;
        }

        public FontGlyph? GetGlyph(
            Font font, int glyphIndex, float pixelHeight)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            pixelHeight = RoundPixelHeight(pixelHeight);

            if (!_map.TryGetValue(pixelHeight, out var heightMap))
            {
                float scale = font.GetScaleByPixel(pixelHeight);
                heightMap = new FontGlyphHeightMap(pixelHeight, new Vector2(scale));
                _map.Add(pixelHeight, heightMap);

                Console.WriteLine("New height: " + heightMap.PixelHeight);
            }

            var lookups = heightMap.Lookups;
            if (!lookups.TryGetValue(font, out var lookup))
            {
                lookup = new FontGlyphLookup(font);
                lookups.Add(font, lookup);
            }

            var glyphs = lookup.Glyphs;
            if (!glyphs.TryGetValue(glyphIndex, out var state))
            {
                using var image = font.GetGlyphBitmap(glyphIndex, heightMap.Scale);
                if (image != null)
                {
                    var texRect = new Rectangle(0, 0, image.Width, image.Height);

                    FontTextureArray? array = null;
                    for (int i = 0; i < _textureStates.Count; i++)
                    {
                        if (_textureStates[i].Insert(texRect.Size, out Point position))
                        {
                            texRect.Position = position;
                            array = _textureStates[i];
                            break;
                        }
                    }

                    if (array == null)
                    {
                        var texture = new Texture2D(
                            GraphicsDevice, TextureSize, TextureSize, mipmap: false, Format);

                        array = new FontTextureArray(texture);
                        _textureStates.Add(array);

                        if (!array.Insert(texRect.Size, out Point position))
                        {
                            throw new Exception(
                                "Failed to insert texture into new texture array.");
                        }
                        texRect.Position = position;
                    }

                    var region = new FontGlyph(heightMap, array.Texture, glyphIndex, texRect);
                    region.Texture.SetData(image, texRect);
                    state = region;
                }

                glyphs.Add(glyphIndex, state);
            }

            return state;
        }
    }
}
