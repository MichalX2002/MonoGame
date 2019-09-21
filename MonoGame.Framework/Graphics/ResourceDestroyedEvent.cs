using System;

namespace MonoGame.Framework.Graphics
{
    public readonly struct ResourceDestroyedEvent
    {
        /// <summary>
        /// The name of the destroyed resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The resource manager tag of the destroyed resource.
        /// </summary>
        public object Tag { get; }

        public ResourceDestroyedEvent(string name, object tag)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tag = tag;
        }
    }
}
