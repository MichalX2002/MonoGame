using System;

namespace MonoGame.Framework
{
    /// <summary>
    /// Represents data for a text editing event.
    /// </summary>
    public readonly ref struct TextEditingEventArgs
    {
        public ReadOnlySpan<char> Text { get; }

        public int Cursor { get; }

        public int SelectionLength { get; }

        /// <summary>
        /// Constructs the <see cref="TextEditingEventArgs"/>.
        /// </summary>
        public TextEditingEventArgs(ReadOnlySpan<char> text, int cursor, int selectionLength)
        {
            Text = text;
            Cursor = cursor;
            SelectionLength = selectionLength;
        }
    }
}
