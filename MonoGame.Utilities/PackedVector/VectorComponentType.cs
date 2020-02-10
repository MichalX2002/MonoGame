using System;

namespace MonoGame.Framework.PackedVector
{
    [Flags]
    public enum VectorComponentType
    {
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
        Gray = Red | Green | Blue,
        Alpha = 1 << 3,
        Raw = 1 << 4
    }
}
