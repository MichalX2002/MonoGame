// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

// Original code from SilverSprite Project

using System.Text;

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
            /// The Unicode scalar associated with this glyph.
            /// </summary>
            public Rune Character { get; }

            /// <summary>
            /// Rectangle in the font texture where this letter exists.
            /// </summary>
            public Rectangle BoundsInTexture { get; }

            /// <summary>
            /// Cropping applied to the BoundsInTexture to calculate the bounds of the actual character.
            /// </summary>
            public Rectangle Cropping { get; }

            /// <summary>
            /// The amount of space between the left side ofthe character and its first pixel in the X dimention.
            /// </summary>
            public float LeftSideBearing { get; }

            /// <summary>
            /// The amount of space between the right side of the character and its last pixel in the X dimention.
            /// </summary>
            public float RightSideBearing { get; }

            /// <summary>
            /// Width of the character before kerning is applied. 
            /// </summary>
            public float Width { get; }

            /// <summary>
            /// Width of the character before kerning is applied. 
            /// </summary>
            public float WidthIncludingBearings { get; }

            public Glyph(
                Rune character, 
                in Rectangle boundsInTexture,
                in Rectangle cropping,
                float leftSideBearing, 
                float rightSideBearing, 
                float width)
            {
                Character = character;
                BoundsInTexture = boundsInTexture;
                Cropping = cropping;
                LeftSideBearing = leftSideBearing;
                RightSideBearing = rightSideBearing;
                Width = width;

                WidthIncludingBearings = Width + LeftSideBearing + RightSideBearing;
            }

            public override readonly string ToString()
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
