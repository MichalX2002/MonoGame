
namespace MonoGame.Framework.Input
{
    public readonly struct TextDroppedEventArgs
    {
        private readonly string? _text;

        public Point Position { get; }
        public string Text => _text ?? string.Empty;

        public TextDroppedEventArgs(Point position, string? text)
        {
            Position = position;
            _text = text;
        }
    }
}
