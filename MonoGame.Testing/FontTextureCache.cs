using System;
using System.Collections.Generic;
using System.Numerics;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;

namespace MonoGame.Testing
{
    public class FontTextureCache
    {
        public class Glyph
        {
            public GlyphHeightMap HeightMap { get; }
            public Texture2D Texture { get; }
            public Rectangle TextureRect { get; }
            public RectangleF GlyphRect { get; }

            public Vector2 Scale => HeightMap.Scale;

            public Glyph(
                GlyphHeightMap heightMap, Texture2D texture, Rectangle textureRect, RectangleF glyphRect)
            {
                HeightMap = heightMap ?? throw new ArgumentNullException(nameof(heightMap));
                Texture = texture ?? throw new ArgumentNullException(nameof(texture));
                TextureRect = textureRect;
                GlyphRect = glyphRect;
            }
        }

        public class GlyphLookup
        {
            public Font Font { get; }

            public Dictionary<int, Glyph?> Glyphs { get; } =
                new Dictionary<int, Glyph?>();

            public GlyphLookup(Font font)
            {
                Font = font ?? throw new ArgumentNullException(nameof(font));
            }
        }

        public class GlyphHeightMap
        {
            public float PixelHeight { get; }
            public Vector2 Scale { get; }

            public Dictionary<Font, GlyphLookup> Lookups { get; } =
                new Dictionary<Font, GlyphLookup>();

            public GlyphHeightMap(float pixelHeight, Vector2 scale)
            {
                PixelHeight = pixelHeight;
                Scale = scale;
            }
        }

        public class TextureArray
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

            public TextureArray(Texture2D texture)
            {
                Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            }

            public bool Insert(Size size, out Point position)
            {
                position = default;

                int availableX = Width - CurrentX;
                if (size.Width > availableX)
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

        private Dictionary<float, GlyphHeightMap> _map = new Dictionary<float, GlyphHeightMap>();
        public List<TextureArray> _textureStates = new List<TextureArray>();

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

        public Glyph? GetGlyph(
            Font font, int pixelHeight, int glyphIndex)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));
            if (pixelHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(pixelHeight));

            pixelHeight = Math.Max(pixelHeight - pixelHeight % 2, 2);

            if (!_map.TryGetValue(pixelHeight, out var heightMap))
            {
                float scale = font.GetScaleByPixel(pixelHeight);
                heightMap = new GlyphHeightMap(pixelHeight, new Vector2(scale));
                _map.Add(pixelHeight, heightMap);
            }

            var lookups = heightMap.Lookups;
            if (!lookups.TryGetValue(font, out var lookup))
            {
                lookup = new GlyphLookup(font);
                lookups.Add(font, lookup);
            }

            var glyphs = lookup.Glyphs;
            if (!glyphs.TryGetValue(glyphIndex, out var state))
            {
                using var image = font.GetGlyphBitmap(glyphIndex, heightMap.Scale);
                if (image != null)
                {
                    var texRect = new Rectangle(0, 0, image.Width, image.Height);

                    TextureArray? array = null;
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

                        array = new TextureArray(texture);
                        _textureStates.Add(array);

                        if (!array.Insert(texRect.Size, out Point position))
                        {
                            throw new Exception(
                                "Failed to insert texture into new texture array.");
                        }
                        texRect.Position = position;
                    }

                    font.GetGlyphBox(glyphIndex, out var glyphRect);
                    var region = new Glyph(heightMap, array.Texture, texRect, glyphRect);
                    region.Texture.SetData(image, texRect);
                    state = region;
                }

                glyphs.Add(glyphIndex, state);
            }

            return state;
        }
    }
}
