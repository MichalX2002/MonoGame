using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    /// <summary>
    /// Represents data for a text input event.
    /// </summary>
    public readonly struct TextInputEvent
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
        /// Constructs the <see cref="TextInputEvent"/>.
        /// </summary>
        public TextInputEvent(int character, Keys key)
        {
            Character = character;
            Key = key;
        }
    }
}
