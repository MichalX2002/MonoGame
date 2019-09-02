using System.Collections.Generic;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
	// Importer interface allows the conversion tool to support multiple source font formats.
	internal interface IFontImporter
	{
		void Import(FontDescription options, string fontName);

		IEnumerable<Glyph> Glyphs { get; }

		float LineSpacing { get; }

		int YOffsetMin { get; }
	}
}
