using System;
using System.ComponentModel;
using System.Text;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Describes a range of consecutive characters that should be included in the font.
    /// </summary>
    [TypeConverter(typeof(CharacterRegionTypeConverter))]
    public readonly struct CharacterRegion
    {
        /// <summary>
        /// The default character region, spanning the base ASCII character set.
        /// </summary>
        public static CharacterRegion Default { get; } = new CharacterRegion((Rune)' ', (Rune)'~');

        public Rune Start { get; }
        public Rune End { get; }

        // Constructor.
        public CharacterRegion(Rune start, Rune end)
        {
            if (start > end)
                throw new ArgumentException("Start exceeds end.", nameof(start));

            Start = start;
            End = end;
        }
    }
}