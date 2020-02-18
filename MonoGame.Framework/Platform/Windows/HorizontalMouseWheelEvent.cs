
namespace MonoGame.Framework.Windows
{
    internal readonly struct HorizontalMouseWheelEvent
    {
        public int Delta { get; }

        public HorizontalMouseWheelEvent(int delta)
        {
            Delta = delta;
        }
    }
}
