using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
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
