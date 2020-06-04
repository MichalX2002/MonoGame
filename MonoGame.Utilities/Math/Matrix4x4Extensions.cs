using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework
{
    public static class Matrix4x4Extensions
    {
        public static void CopyTo(in this Matrix4x4 value, Span<float> destination)
        {
            MemoryMarshal.Cast<Matrix4x4, float>(UnsafeR.AsReadOnlySpan(value)).CopyTo(destination);
        }
    }
}
