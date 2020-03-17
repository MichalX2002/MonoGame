using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    /// <summary>
    /// Represents data for a text input event.
    /// </summary>
    public readonly struct TextInputEventArgs
    {
        /// <summary>
        /// Gets the Unicode (UTF-32) character.
        /// </summary>
        public int Character { get; }

        /// <summary>
        /// Gets the key that was pressed.
        /// </summary>
        public Keys Key { get; }

        /// <summary>
        /// Constructs the <see cref="TextInputEventArgs"/>.
        /// </summary>
        public TextInputEventArgs(int character, Keys key)
        {
            Character = character;
            Key = key;
        }
    }
}
