using System;

namespace MonoGame.Framework.PackedVector
{
    [Flags]
    public enum VectorComponentType
    {
        Red,
        Green,
        Blue,
        Gray = Red | Green | Blue,
        Alpha,
        Raw
    }
}
