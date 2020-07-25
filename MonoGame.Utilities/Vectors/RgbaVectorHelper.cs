// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    public static class RgbaVectorHelper
    {
        // TODO: optimize with SSE (new intrinsics in NET5)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToScaledUInt8(Vector3 scaledVector)
        {
            scaledVector *= byte.MaxValue;
            scaledVector += new Vector3(0.5f);
            scaledVector = VectorHelper.ZeroMax(scaledVector, byte.MaxValue);

            return scaledVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToScaledUInt8(Vector4 scaledVector)
        {
            scaledVector *= byte.MaxValue;
            scaledVector += new Vector4(0.5f);
            scaledVector = VectorHelper.ZeroMax(scaledVector, byte.MaxValue);

            return scaledVector;
        }
    }
}