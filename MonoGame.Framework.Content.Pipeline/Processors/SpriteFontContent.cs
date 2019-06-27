// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
	public class SpriteFontContent
    {
        public string FontName = string.Empty;
        private FontDescriptionStyle Style = FontDescriptionStyle.Regular;
        public float FontSize;
        public Texture2DContent Texture = new Texture2DContent();
        public List<Rectangle> Regions = new List<Rectangle>();
        public List<Rectangle> Croppings = new List<Rectangle>();
        public List<char> CharacterMap = new List<char>();
        public int VerticalLineSpacing;
        public float HorizontalSpacing;
        public List<Vector3> Kerning = new List<Vector3>();
        public char? DefaultCharacter;

        public SpriteFontContent() { }

        public SpriteFontContent(FontDescription desc)
        {
            FontName = desc.FontName;
            Style = desc.Style;
            FontSize = desc.Size;
            CharacterMap = new List<char>(desc.Characters.Count);
            VerticalLineSpacing = (int)desc.Spacing; // Will be replaced in the pipeline.
            HorizontalSpacing = desc.Spacing;

            DefaultCharacter = desc.DefaultCharacter;
        } 

    }
}
