
using System;

namespace MonoGame.Framework.Graphics
{
    public readonly struct ResourceCreatedEvent
    {
        /// <summary>
        /// The newly created resource object.
        /// </summary>
        public object Resource { get; }

        public ResourceCreatedEvent(object resource)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }
    }
}
