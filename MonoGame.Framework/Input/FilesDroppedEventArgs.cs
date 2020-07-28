using System;

namespace MonoGame.Framework.Input
{
    public readonly struct FilesDroppedEventArgs
    {
        private readonly string[] _filePaths;

        public Point Position { get; }
        public string[] FilePaths => _filePaths ?? Array.Empty<string>();

        public FilesDroppedEventArgs(Point position, string[]? filePaths)
        {
            Position = position;
            _filePaths = filePaths;
        }
    }
}
