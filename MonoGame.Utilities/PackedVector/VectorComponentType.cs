using System;

namespace MonoGame.Framework.PackedVector
{
    [Flags]
    public enum VectorComponentType
    {
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
        Alpha = 1 << 3,
        Luminance = Red | Green | Blue,
        Raw = 1 << 4
    }
}
