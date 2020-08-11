using System;

namespace MonoGame.Framework.Graphics
{
    public static partial class IndexElementTypeExtensions
    {
        /// <summary>
        /// Gets the size of the element type in bytes.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int TypeSize(this IndexElementType type)
        {
            switch (type)
            {
                case IndexElementType.Int16:
                    return 2;

                case IndexElementType.Int32:
                    return 4;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
