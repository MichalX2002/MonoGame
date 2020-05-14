using System;

namespace MonoGame.Framework.Vector
{
    [Flags]
    public enum VectorComponentChannel
    {
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
        Alpha = 1 << 3,
        Luminance = Red | Green | Blue,
        Raw = 1 << 4,
        Padding = 1 << 5,
    }
}
