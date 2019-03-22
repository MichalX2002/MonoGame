using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    public readonly struct TextInputEvent
    {
        public GameWindow Window { get; }
        public int Character { get; }
        public Keys Key { get; }

        public TextInputEvent(GameWindow window, int character, Keys key)
        {
            Window = window;
            Character = character;
            Key = key;
        }
    }
}
