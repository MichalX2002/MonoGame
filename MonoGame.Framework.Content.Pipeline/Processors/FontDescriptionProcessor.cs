// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using MonoGame.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework;
using Glyph = MonoGame.Framework.Content.Pipeline.Graphics.Glyph;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
	[ContentProcessor(DisplayName = "Sprite Font Description - MonoGame")]
	public class FontDescriptionProcessor : ContentProcessor<FontDescription, SpriteFontContent>
	{
		private static readonly string[] _fontExtensions = new [] { "", ".ttf", ".ttc", ".otf" };
		private static readonly HashSet<string> _trueTypeFileExtensions =
			new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { ".ttf", ".ttc", ".otf" };

		[DefaultValue(true)]
		public virtual bool PremultiplyAlpha { get; set; }

		[DefaultValue(typeof(TextureProcessorOutputFormat), "Compressed")]
		public virtual TextureProcessorOutputFormat TextureFormat { get; set; }

		public FontDescriptionProcessor()
		{
			PremultiplyAlpha = true;
			TextureFormat = TextureProcessorOutputFormat.Compressed;
		}

		public override SpriteFontContent Process(
			FontDescription input, ContentProcessorContext context)
		{
			string fontFile = FindFont(input.FontName, input.Style.ToString());
			if (string.IsNullOrWhiteSpace(fontFile))
			{
				var directories = new List<string> { Path.GetDirectoryName(input.Identity.SourceFilename) };

				// Add special per platform directories
				if (CurrentPlatform.OS == OS.Windows)
				{
					directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts"));
				}
				else if (CurrentPlatform.OS == OS.MacOSX)
				{
					directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Fonts"));
					directories.Add("/Library/Fonts");
				}

				foreach (var dir in directories)
				{
					foreach (var ext in _fontExtensions)
					{
						fontFile = Path.Combine(dir, input.FontName + ext);
						if (File.Exists(fontFile))
							break;
					}
					if (File.Exists(fontFile))
						break;
				}
			}

			if (!File.Exists(fontFile))
				throw new FileNotFoundException("Could not find \"" + input.FontName + "\" font file.");

			context.Logger.LogMessage("Building Font {0}", fontFile);

			// Get the platform specific texture profile.
			var texProfile = TextureProfile.ForPlatform(context.TargetPlatform);

			if (!File.Exists(fontFile))
				throw new Exception(string.Format("Could not load {0}", fontFile));

			var glyphs = ImportFont(input, out float lineSpacing, out int yOffsetMin, context, fontFile);

			// Optimize.
			foreach (Glyph glyph in glyphs)
				GlyphCropper.Crop(glyph);

			texProfile.Requirements(context, TextureFormat, out bool requiresPot, out bool requiresSquare);

			var output = new SpriteFontContent(input);

			var face = GlyphPacker.ArrangeGlyphs(glyphs, requiresPot, requiresSquare);
			output.Texture.Faces[0].Add(face);

			// Adjust line and character spacing.
			lineSpacing += input.Spacing;
			output.VerticalLineSpacing = (int)lineSpacing;

			foreach (var glyph in glyphs)
			{
				output.CharacterMap.Add(glyph.Character);

				var texRegion = new Rectangle(glyph.Subrect.X, glyph.Subrect.Y, glyph.Subrect.Width, glyph.Subrect.Height);
				output.Regions.Add(texRegion);

				output.Croppings.Add(
					new Rectangle(0, (int)(glyph.YOffset - yOffsetMin), (int)glyph.XAdvance, output.VerticalLineSpacing));

				// Set the optional character kerning.
				if (input.UseKerning)
					output.Kerning.Add(new Vector3(glyph.CharacterWidths.A, glyph.CharacterWidths.B, glyph.CharacterWidths.C));
				else
					output.Kerning.Add(new Vector3(0, texRegion.Width, 0));
			}

			var facePixels = face.GetPixelSpan();
			if (PremultiplyAlpha)
			{
				for (int i = 0; i < facePixels.Length; i++)
				{
					ref Color pixel = ref facePixels[i];

					// A is the value of white alpha we want
					pixel.R = pixel.A;
					pixel.G = pixel.A;
					pixel.B = pixel.A;
				}
			}
			else
			{
				for (int i = 0; i < facePixels.Length; i++)
				{
					ref Color pixel = ref facePixels[i];
					pixel.R = 255;
					pixel.G = 255;
					pixel.B = 255;
				}
			}

			// Perform the final texture conversion.
			texProfile.ConvertTexture(context, output.Texture, TextureFormat, true);

			return output;
		}

		private static List<Glyph> ImportFont(
			FontDescription options, out float lineSpacing, out int yOffsetMin,
			ContentProcessorContext context, string fontName)
		{
			string fileExtension = Path.GetExtension(fontName);
			if (!_trueTypeFileExtensions.Contains(fileExtension)) 
				throw new PipelineException("Unknown file extension " + fileExtension);

			IFontImporter importer = new SharpFontImporter();

			// Import the source font data.
			importer.Import(options, fontName);

			lineSpacing = importer.LineSpacing;
			yOffsetMin = importer.YOffsetMin;

			// Get all glyphs
			var glyphs = new List<Glyph>(importer.Glyphs);

			// Validate.
			if (glyphs.Count == 0)
				throw new Exception("Font does not contain any glyphs.");

			// Sort the glyphs
			glyphs.Sort((left, right) => left.Character.CompareTo(right.Character));

			// Check that the default character is part of the glyphs
			if (options.DefaultCharacter != null)
			{
				bool defaultCharacterFound = false;
				foreach (var glyph in glyphs)
				{
					if (glyph.Character == options.DefaultCharacter)
					{
						defaultCharacterFound = true;
						break;
					}
				}

				if (!defaultCharacterFound)
					throw new InvalidOperationException("The specified DefaultCharacter is not part of this font.");
			}

			return glyphs;
		}

        private string FindFont(string name, string style)
        {
            if (CurrentPlatform.OS == OS.Windows)
            {
                var fontDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
                foreach (var key in new RegistryKey[] { Registry.LocalMachine, Registry.CurrentUser })
                {
                    var subkey = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", false);
                    foreach (var font in subkey.GetValueNames().OrderBy(x => x))
                    {
                        if (font.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                        {
                            var fontPath = subkey.GetValue(font).ToString();

                            // The registry value might have trailing NUL characters
                            // See https://github.com/MonoGame/MonoGame/issues/4061
                            var nulIndex = fontPath.IndexOf('\0');
                            if (nulIndex != -1)
                                fontPath = fontPath.Substring(0, nulIndex);

                            return Path.IsPathRooted(fontPath) ? fontPath : Path.Combine(fontDirectory, fontPath);
                        }
					}
				}
			}
			else if (CurrentPlatform.OS == OS.Linux)
			{
				ExternalTool.Run(
					"/bin/bash", $"-c \"fc-match -f '%{{file}}:%{{family}}\\n' '{name}:style={style}'\"",
					out string s, out string e);
				s = s.Trim();

				var split = s.Split(':');
				if (split.Length < 2)
					return string.Empty;

				// check font family, fontconfig might return a fallback
				if (split[1].Contains(","))
				{
					// this file defines multiple family names
					var families = split[1].Split(',');
					foreach (var f in families)
					{
						if (f.ToLowerInvariant() == name.ToLowerInvariant())
							return split[0];
					}
					// didn't find it
					return string.Empty;
				}
				else
				{
					if (split[1].ToLowerInvariant() != name.ToLowerInvariant())
						return string.Empty;
				}

				return split[0];
			}

			return string.Empty;
		}
	}
}
