using System;
using System.Diagnostics.CodeAnalysis;
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

        public Font(ReadOnlyMemory<byte> fontData)
        {
            FontInfo = new TT.FontInfo();
            TT.InitFont(FontInfo, fontData);
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

        public bool TryGetGlyphRect(int glyphIndex, Vector2 scale, Point oversample, Vector2 shift, out Rectangle rectangle)
        {
            if (TT.GetGlyphBitmapBoxSubpixel(
                FontInfo, glyphIndex, scale * oversample, shift, out TT.IntRect rect))
            {
                rectangle = new Rectangle(rect.X, rect.Y, rect.W, rect.H);
                return true;
            }
            rectangle = default;
            return false;
        }

        public static RectangleF GetDrawRectangle(Rectangle rectangle, Point oversample, Vector2 shift)
        {
            int w = rectangle.Width + oversample.X - 1;
            int h = rectangle.Height + oversample.Y - 1;
            float sub_x = TT.OversampleShift(oversample.X);
            float sub_y = TT.OversampleShift(oversample.Y);
            float drawX = (rectangle.X - shift.X) / oversample.X + sub_x;
            float drawY = (rectangle.Y - shift.Y) / oversample.Y + sub_y;
            float drawW = w / (float)oversample.X + sub_x;
            float drawH = h / (float)oversample.Y + sub_y;
            RectangleF drawRectangle = new (drawX, drawY, drawW, drawH);
            return drawRectangle;
        }

        public bool TryGetGlyphBitmap(
            int glyphIndex, Vector2 scale, Point oversample, Vector2 shift,
            [MaybeNullWhen(false)] out Image<Alpha8> image,
            out RectangleF drawRectangle)
        {
            if (!TryGetGlyphRect(glyphIndex, scale, oversample, shift, out Rectangle rect))
            {
                image = default;
                drawRectangle = default;
                return false;
            }

            int w = rect.Width + oversample.X - 1;
            int h = rect.Height + oversample.Y - 1;
            image = Image<Alpha8>.CreateUninitialized(w, h);

            TT.Bitmap glyphBmp = GetTTBitmap(image, rect.Width, rect.Height);
            TT.MakeGlyphBitmap(
                FontInfo,
                glyphBmp,
                0.35f,
                scale * oversample,
                shift,
                TT.IntPoint.Zero,
                glyphIndex);

            TT.Bitmap filterBmp = GetTTBitmap(image, w, h);
            if (oversample.X > 1)
                TT.HorizontalPrefilter(filterBmp, oversample.X);
            if (oversample.Y > 1)
                TT.VerticalPrefilter(filterBmp, oversample.Y);

            drawRectangle = GetDrawRectangle(rect, oversample, shift);

            return true;
        }

        public static TT.Bitmap GetTTBitmap(IPixelMemory image, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return new TT.Bitmap(image.GetPixelByteSpan(), width, height, image.ByteStride);
        }

        public static TT.Bitmap GetTTBitmap(IPixelMemory image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return GetTTBitmap(image, image.Width, image.Height);
        }
    }
}