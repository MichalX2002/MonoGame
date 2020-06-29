// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing signed 16-bit XYZW integer components.
    /// <para>
    /// Ranges from [-37268, -37268, -37268, -37268] to [37267, 37267, 37267, 37267] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Short4 : IPackedPixel<Short4, ulong>
    {
        private static Vector4 Offset => new Vector4(32768);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Alpha));

        public short X;
        public short Y;
        public short Z;
        public short W;

        #region Constructors

        public Short4(short x, short y, short z, short w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Short4(Vector4 vector) : this()
        {
            FromVector(vector);
        }

        public Short4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeR.As<Short4, ulong>(this);
            set => Unsafe.As<Short4, ulong>(ref this) = value;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= ushort.MaxValue;
            scaledVector -= Offset;

            FromVector(scaledVector);
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaledVector = ToVector4();
            scaledVector += Offset;
            scaledVector /= ushort.MaxValue;

            return scaledVector;
        }

        public void FromVector(Vector4 vector)
        {
            vector.Clamp(short.MinValue, short.MaxValue);
            vector.Round();

            X = (short)vector.X;
            Y = (short)vector.Y;
            Z = (short)vector.Z;
            W = (short)vector.W;
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(X, Y, Z, W);
        }

        #endregion

        #region Equals

        public readonly bool Equals(Short4 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Short4 other && Equals(other);
        }

        public static bool operator ==(in Short4 a, in Short4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Short4 a, in Short4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(Short4) + $"({X}, {Y}, {Z}, {W})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
