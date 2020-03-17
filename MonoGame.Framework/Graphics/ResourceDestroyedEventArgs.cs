using System;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Describes in short a destroyed resource.
    /// </summary>
    public readonly struct ResourceDestroyedEventArgs
    {
        /// <summary>
        /// The name of the destroyed resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The resource manager tag of the destroyed resource.
        /// </summary>
        public object Tag { get; }

        /// <summary>
        /// Constructs the resource destruction event.
        /// </summary>
        public ResourceDestroyedEventArgs(string name, object tag)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tag = tag;
        }
    }
}
