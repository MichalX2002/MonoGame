// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MonoGame.Framework.Vectors;
using SharpFont;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Uses FreeType to rasterize TrueType fonts into a series of glyph bitmaps.
    /// </summary>
    internal class SharpFontImporter : IFontImporter
    {
        private Library _lib = null;

        /// <summary>
        /// Properties hold the imported font data.
        /// </summary>
        public IEnumerable<Glyph> Glyphs { get; private set; }

        public float LineSpacing { get; private set; }
        public int YOffsetMin { get; private set; }

        public void Import(FontDescription options, string fontName)
        {
            _lib = new Library();
            
            // Create a bunch of GDI+ objects.
            var face = CreateFontFace(options, fontName);
            try
            {
                // Which characters do we want to include?
                var characters = options.Characters;

                var glyphList = new List<Glyph>();
                // Rasterize each character in turn.
                foreach (var character in characters)
                {
                    var glyph = ImportGlyph(character, face);
                    glyphList.Add(glyph);
                }
                Glyphs = glyphList;

                // Store the font height.
                LineSpacing = face.Size.Metrics.Height >> 6;

                // The height used to calculate the Y offset for each character.
                YOffsetMin = -face.Size.Metrics.Ascender >> 6;
            }
            finally
            {
                if (face != null)
                    face.Dispose();

                _lib?.Dispose();
                _lib = null;
            }
        }

        // Attempts to instantiate the requested GDI+ font object.
        private Face CreateFontFace(FontDescription options, string fontName)
        {
            try
            {
                const uint dpi = 96;
                var face = _lib.NewFace(fontName, 0);
                var fixedSize = ((int)options.Size) << 6;
                face.SetCharSize(0, fixedSize, dpi, dpi);

                if (face.FamilyName == "Microsoft Sans Serif" && options.FontName != "Microsoft Sans Serif")
                    throw new PipelineException(
                        string.Format("Font {0} is not installed on this computer.", options.FontName));

                return face;

                // A font substitution must have occurred.
                //throw new Exception(string.Format("Can't find font '{0}'.", options.FontName));
            }
            catch
            {
                throw;
            }
        }

        // Rasterizes a single character glyph.
        private Glyph ImportGlyph(Rune character, Face face)
        {
            uint glyphIndex = face.GetCharIndex((uint)character.Value);
            face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
            face.Glyph.RenderGlyph(RenderMode.Normal);

            // Render the character.
            BitmapContent glyphBitmap = null;
            if (face.Glyph.Bitmap.Width > 0 && face.Glyph.Bitmap.Rows > 0)
            {
                glyphBitmap = new PixelBitmapContent<Alpha8>(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows);
                byte[] gpixelAlphas = new byte[face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows];

                //if the character bitmap has 1bpp we have to expand the buffer data to get the 8bpp pixel data
                //each byte in bitmap.bufferdata contains the value of to 8 pixels in the row
                //if bitmap is of width 10, each row has 2 bytes with 10 valid bits, and the last 6 bits of 2nd byte must be discarded
                if (face.Glyph.Bitmap.PixelMode == PixelMode.Mono)
                {
                    //variables needed for the expansion, amount of written data, length of the data to write
                    int written = 0, length = face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows;
                    for (int i = 0; written < length; i++)
                    {
                        //width in pixels of each row
                        int width = face.Glyph.Bitmap.Width;
                        while (width > 0)
                        {
                            //valid data in the current byte
                            int stride = Math.Min(8, width);
                            //copy the valid bytes to pixeldata
                            //System.Array.Copy(ExpandByte(face.Glyph.Bitmap.BufferData[i]), 0, gpixelAlphas, written, stride);
                            ExpandByteAndCopy(face.Glyph.Bitmap.BufferData[i], stride, gpixelAlphas, written);
                            written += stride;
                            width -= stride;
                            if (width > 0)
                                i++;
                        }
                    }
                }
                else
                {
                    Marshal.Copy(face.Glyph.Bitmap.Buffer, gpixelAlphas, 0, gpixelAlphas.Length);
                }
                glyphBitmap.SetPixelData(gpixelAlphas);
            }

            if (glyphBitmap == null)
            {
                var gHA = face.Glyph.Metrics.HorizontalAdvance >> 6;
                var gVA = face.Size.Metrics.Height >> 6;

                gHA = gHA > 0 ? gHA : gVA;
                gVA = gVA > 0 ? gVA : gHA;

                glyphBitmap = new PixelBitmapContent<Alpha8>(gHA, gVA);
            }

            // not sure about this at all
            var abc = new ABCFloat
            {
                A = face.Glyph.Metrics.HorizontalBearingX >> 6,
                B = face.Glyph.Metrics.Width >> 6
            };
            abc.C = (face.Glyph.Metrics.HorizontalAdvance >> 6) - (abc.A + abc.B);

            // Construct the output Glyph object.
            return new Glyph(character, glyphBitmap)
            {
                XOffset = -(face.Glyph.Advance.X >> 6),
                XAdvance = face.Glyph.Metrics.HorizontalAdvance >> 6,
                YOffset = -(face.Glyph.Metrics.HorizontalBearingY >> 6),
                CharacterWidths = abc
            };
        }


        /// <summary>
        /// Reads each individual bit of a byte from left to right and expands it to a full byte, 
        /// ones get byte.maxvalue, and zeros get byte.minvalue.
        /// </summary>
        /// <param name="origin">Byte to expand and copy</param>
        /// <param name="length">Number of Bits of the Byte to copy, from 1 to 8</param>
        /// <param name="destination">Byte array where to copy the results</param>
        /// <param name="startIndex">Position where to begin copying the results in destination</param>
        private static void ExpandByteAndCopy(byte origin, int length, byte[] destination, int startIndex)
        {
            byte tmp;
            for (int i = 7; i > 7 - length; i--)
            {
                tmp = (byte)(1 << i);
                if (origin / tmp == 1)
                {
                    destination[startIndex + 7 - i] = byte.MaxValue;
                    origin -= tmp;
                }
                else
                    destination[startIndex + 7 - i] = byte.MinValue;
            }
        }
    }
}