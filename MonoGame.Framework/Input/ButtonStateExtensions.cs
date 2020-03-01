
namespace MonoGame.Framework.Input
{
    public static class ButtonStateExtensions
    {
        public static bool ToBoolean(this ButtonState state)
        {
            return state == ButtonState.Pressed;
        }
    }
}
