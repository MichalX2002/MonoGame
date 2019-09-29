using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    /// <summary>
    /// This struct is used for <see cref="GameWindow.TextInput"/>.
    /// </summary>
    public readonly struct TextInputEvent
    {
        public int Character { get; }
        public Keys Key { get; }

        public TextInputEvent(int character, Keys key)
        {
            Character = character;
            Key = key;
        }
    }
}
