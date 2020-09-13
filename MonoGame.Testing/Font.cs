using System;
using System.Numerics;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;
using MonoGame.Imaging.Pixels;
using TT = StbSharp.TrueType;

namespace MonoGame.Testing
{
    public class Font
    {
        public TT.FontInfo FontInfo { get; }

        public Font(ReadOnlyMemory<byte> fontData, int fontIndex = 0)
        {
            FontInfo = new TT.FontInfo();
            TT.InitFont(FontInfo, fontData, fontIndex);
        }

        public static RectangleF GetGlyphBoxSubpixel(
            RectangleF rawGlyphBox, Vector2 scale, Vector2 shift)
        {
            return new RectangleF(
                rawGlyphBox.X * scale.X + shift.X,
                (-rawGlyphBox.Y - rawGlyphBox.Height) * scale.Y + shift.Y,
                rawGlyphBox.Width * scale.X + shift.X,
                rawGlyphBox.Height * scale.Y + shift.Y);
        }

        public bool GetGlyphRightSideBearing(int glyphIndex, out float rightSideBearing)
        {
            return TT.GetGlyphRightSideBearing(FontInfo, glyphIndex, out rightSideBearing);
        }

        public int? GetGlyphKernAdvance(int glyphIndex1, int glyphIndex2)
        {
            return TT.GetGlyphKernAdvance(FontInfo, glyphIndex1, glyphIndex2);
        }

        public void GetGlyphHMetrics(int glyphIndex, out int advanceWidth, out int leftSideBearing)
        {
            TT.GetGlyphHMetrics(FontInfo, glyphIndex, out advanceWidth, out leftSideBearing);
        }

        public void GetFontVMetrics(out int ascent, out int descent, out int lineGap)
        {
            TT.GetFontVMetrics(FontInfo, out ascent, out descent, out lineGap);
        }

        public int GetGlyphIndex(int codepoint)
        {
            return TT.FindGlyphIndex(FontInfo, codepoint);
        }

        public float GetScaleByPixel(float pixelHeight)
        {
            return TT.ScaleForPixelHeight(FontInfo, pixelHeight);
        }

        public float GetScaleByEm(float pixels)
        {
            return TT.ScaleForMappingEmToPixels(FontInfo, pixels);
        }

        public bool GetGlyphBox(int glyphIndex, out RectangleF glyphBox)
        {
            if (TT.GetGlyphBox(FontInfo, glyphIndex, out var rectBox))
            {
                glyphBox = new RectangleF(
                    rectBox.X, rectBox.Y,
                    rectBox.W, rectBox.H);
                return true;
            }
            glyphBox = default;
            return false;
        }

        public bool GetGlyphBoxSubpixel(
            int glyphIndex, Vector2 scale, Vector2 shift, out RectangleF glyphBox)
        {
            if (GetGlyphBox(glyphIndex, out var rawGlyphBox))
            {
                glyphBox = GetGlyphBoxSubpixel(rawGlyphBox, scale, shift);
                return true;
            }
            glyphBox = default;
            return false;
        }

        public Image<Alpha8>? GetGlyphBitmap(
            int glyphIndex, Vector2 scale, Vector2 shift = default)
        {
            if (TT.GetGlyphBitmapBoxSubpixel(
                FontInfo, glyphIndex, scale, shift, out var glyphBox))
            {
                var image = Image<Alpha8>.CreateUninitialized(glyphBox.W, glyphBox.H);

                float pixelFlatness = 0.35f;
                var gbm = GetTTBitmap(image, glyphBox.W, glyphBox.H);

                int num_verts = TT.GetGlyphShape(FontInfo, glyphIndex, out TT.Vertex[]? vertices);

                TT.Rasterize(
                    gbm, pixelFlatness, vertices.AsSpan(0, num_verts), scale, shift,
                    glyphBox.Position, TT.IntPoint.Zero, true);

                return image;
            }

            return null;
        }

        public static TT.Bitmap GetTTBitmap(IPixelMemory image, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return new TT.Bitmap()
            {
                w = width,
                h = height,
                stride = image.ByteStride,
                pixels = image.GetPixelByteSpan()
            };
        }

        public static TT.Bitmap GetTTBitmap(IPixelMemory image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return GetTTBitmap(image, image.Width, image.Height);
        }
    }
}