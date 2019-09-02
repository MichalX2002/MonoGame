
namespace MonoGame.Framework.Windows
{
    internal readonly struct HorizontalMouseWheelEvent
    {
        public int Delta { get; }

        internal HorizontalMouseWheelEvent(int delta)
        {
            Delta = delta;
        }
    }
}
