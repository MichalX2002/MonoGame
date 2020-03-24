// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

// Original code from SilverSprite Project

namespace MonoGame.Framework.Graphics
{
    public partial class SpriteFont
    {
        /// <summary>
        /// Struct that defines the spacing, Kerning, and bounds of a character.
        /// </summary>
        /// <remarks>Provides the data necessary to implement custom SpriteFont rendering.</remarks>
        public readonly struct Glyph
        {
            /// <summary>
            /// The char associated with this glyph.
            /// </summary>
            public readonly char Character;

            /// <summary>
            /// Rectangle in the font texture where this letter exists.
            /// </summary>
            public readonly Rectangle BoundsInTexture;

            /// <summary>
            /// Cropping applied to the BoundsInTexture to calculate the bounds of the actual character.
            /// </summary>
            public readonly Rectangle Cropping;

            /// <summary>
            /// The amount of space between the left side ofthe character and its first pixel in the X dimention.
            /// </summary>
            public readonly float LeftSideBearing;

            /// <summary>
            /// The amount of space between the right side of the character and its last pixel in the X dimention.
            /// </summary>
            public readonly float RightSideBearing;

            /// <summary>
            /// Width of the character before kerning is applied. 
            /// </summary>
            public readonly float Width;

            /// <summary>
            /// Width of the character before kerning is applied. 
            /// </summary>
            public readonly float WidthIncludingBearings;

            public static readonly Glyph Empty = new Glyph();

            public Glyph(
                char character, in Rectangle boundsInTexture, in Rectangle cropping,
                float leftSideBearing, float rightSideBearing, float width, float widthIncludingBearings)
            {
                Character = character;
                BoundsInTexture = boundsInTexture;
                Cropping = cropping;
                LeftSideBearing = leftSideBearing;
                RightSideBearing = rightSideBearing;
                Width = width;
                WidthIncludingBearings = widthIncludingBearings;
            }

            public override string ToString()
            {
                return
                    "CharacterIndex=" + Character +
                    ", Glyph=" + BoundsInTexture +
                    ", Cropping=" + Cropping +
                    ", Kerning=" + LeftSideBearing + "," + Width + "," + RightSideBearing;
            }
        }
    }
}
