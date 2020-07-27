using System.Text;
using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    /// <summary>
    /// Represents data for a text input event.
    /// </summary>
    public readonly struct TextInputEventArgs
    {
        /// <summary>
        /// Gets the character as a Unicode scalar value.
        /// </summary>
        public Rune Character { get; }

        /// <summary>
        /// Gets the key that was pressed.
        /// </summary>
        public Keys? Key { get; }

        /// <summary>
        /// Constructs the <see cref="TextInputEventArgs"/>.
        /// </summary>
        public TextInputEventArgs(Rune character, Keys? key)
        {
            Character = character;
            Key = key;
        }
    }
}
