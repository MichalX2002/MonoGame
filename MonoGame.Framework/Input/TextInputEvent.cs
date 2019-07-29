using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
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
