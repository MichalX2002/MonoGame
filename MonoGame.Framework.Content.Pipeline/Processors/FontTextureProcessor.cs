﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Graphics;
using System.Text;
using System.Numerics;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Font Texture - MonoGame")]
    public class FontTextureProcessor : ContentProcessor<Texture2DContent, SpriteFontContent>
    {
        private static readonly Color _transparentPixel = Color.Magenta;

        [DefaultValue(' ')]
        public virtual Rune FirstCharacter { get; set; }

        [DefaultValue(true)]
        public virtual bool PremultiplyAlpha { get; set; }

        public virtual TextureProcessorOutputFormat TextureFormat { get; set; }

        public FontTextureProcessor()
        {
            FirstCharacter = (Rune)' ';
            PremultiplyAlpha = true;
        }

        protected virtual Rune GetCharacterForIndex(int index)
        {
            return (Rune)(FirstCharacter.Value + index);
        }

        private List<Glyph> ExtractGlyphs(PixelBitmapContent<Color> bitmap)
        {
            var glyphs = new List<Glyph>();
            var regions = new List<Rectangle>();

            int width = bitmap.Width;
            var pixels = bitmap.GetPixelSpan();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pixels[x + y * width] != _transparentPixel)
                    {
                        // if we don't have a region that has this pixel already
                        var re = regions.Find(r =>
                        {
                            return r.Contains(x, y);
                        });

                        if (re == Rectangle.Empty)
                        {
                            // we have found the top, left of a image. 
                            // we now need to scan for the 'bounds'
                            int top = y;
                            int bottom = y;
                            int left = x;
                            int right = x;

                            while (pixels[right + bottom * width] != _transparentPixel)
                                right++;

                            while (pixels[left + bottom * width] != _transparentPixel)
                                bottom++;

                            // we got a glyph :)
                            regions.Add(new Rectangle(left, top, right - left, bottom - top));
                            x = right;
                        }
                        else
                        {
                            x += re.Width;
                        }
                    }
                }
            }

            for (int i = 0; i < regions.Count; i++)
            {
                var rect = regions[i];
                var newBitmap = new PixelBitmapContent<Color>(rect.Width, rect.Height);
                BitmapContent.Copy(bitmap, rect, newBitmap, new Rectangle(0, 0, rect.Width, rect.Height));

                var glyph = new Glyph(GetCharacterForIndex(i), newBitmap);
                glyph.CharacterWidths.B = glyph.Bitmap.Width;
                glyphs.Add(glyph);
            }
            return glyphs;
        }

        public override SpriteFontContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            // extract the glyphs from the texture and map them to a list of characters.
            // we need to call GtCharacterForIndex for each glyph in the Texture to 
            // get the char for that glyph, by default we start at ' ' then '!' and then ASCII
            // after that.
            BitmapContent face = input.Faces[0][0];
            face.TryGetFormat(out SurfaceFormat faceFormat);
            if (faceFormat != SurfaceFormat.Rgba32)
            {
                var colorFace = new PixelBitmapContent<Color>(face.Width, face.Height);
                BitmapContent.Copy(face, colorFace);
                face = colorFace;
            }

            var output = new SpriteFontContent();
            var glyphs = ExtractGlyphs((PixelBitmapContent<Color>)face);
            // Optimize.
            foreach (var glyph in glyphs)
            {
                GlyphCropper.Crop(glyph);
                output.VerticalLineSpacing = Math.Max(output.VerticalLineSpacing, glyph.Subrect.Height);
            }

            // Get the platform specific texture profile.
            var texProfile = TextureProfile.ForPlatform(context.TargetPlatform);
            texProfile.Requirements(context, TextureFormat, out bool requiresPot, out bool requiresSquare);

            face = GlyphPacker.ArrangeGlyphs(glyphs, requiresPot, requiresSquare);

            foreach (var glyph in glyphs)
            {
                output.CharacterMap.Add(glyph.Character);

                output.Regions.Add(
                    new Rectangle(glyph.Subrect.X, glyph.Subrect.Y, glyph.Subrect.Width, glyph.Subrect.Height));

                output.Croppings.Add(
                    new Rectangle((int)glyph.XOffset, (int)glyph.YOffset, glyph.Width, glyph.Height));

                var abc = glyph.CharacterWidths;
                output.Kerning.Add(new Vector3(abc.A, abc.B, abc.C));
            }

            output.Texture.Faces[0].Add(face);

            var bmp = output.Texture.Faces[0][0];
            if (PremultiplyAlpha)
            {
                var data = bmp.GetPixelData();
                var idx = 0;
                for (; idx < data.Length;)
                {
                    var r = data[idx + 0];
                    var g = data[idx + 1];
                    var b = data[idx + 2];
                    var a = data[idx + 3];
                    var col = Color.FromNonPremultiplied(r, g, b, a);

                    data[idx + 0] = col.R;
                    data[idx + 1] = col.G;
                    data[idx + 2] = col.B;
                    data[idx + 3] = col.A;

                    idx += 4;
                }

                bmp.SetPixelData(data);
            }

            // Perform the final texture conversion.
            texProfile.ConvertTexture(context, output.Texture, TextureFormat, true);

            return output;
        }
    }
}