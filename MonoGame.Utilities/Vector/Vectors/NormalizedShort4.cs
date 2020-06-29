// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing signed 16-bit XYZW components.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedShort4 : IPackedPixel<NormalizedShort4, ulong>
    {
        internal static Vector4 Offset => new Vector4(-short.MinValue);

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

        public NormalizedShort4(short x, short y, short z, short w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        [CLSCompliant(false)]
        public NormalizedShort4(ulong packed) : this()
        {
            PackedValue = packed;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeR.As<NormalizedShort4, ulong>(this);
            set => Unsafe.As<NormalizedShort4, ulong>(ref this) = value;
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaled = new Vector4(X, Y, Z, W);
            scaled += Offset;
            scaled /= ushort.MaxValue;

            return scaled;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= ushort.MaxValue;
            scaledVector -= Offset;
            scaledVector.Clamp(short.MinValue, short.MaxValue);

            X = (short)scaledVector.X;
            Y = (short)scaledVector.Y;
            Z = (short)scaledVector.Z;
            W = (short)scaledVector.W;
        }

        public readonly Vector4 ToVector4()
        {
            var vector = new Vector4(X, Y, Z, W);
            vector += Offset;
            vector *= 2f / ushort.MaxValue;
            vector -= Vector4.One;

            return vector;
        }

        public void FromVector(Vector4 vector)
        {
            vector *= ushort.MaxValue / 2f;
            vector -= new Vector4(0.5f);
            vector.Clamp(short.MinValue, short.MaxValue);

            X = (short)vector.X;
            Y = (short)vector.Y;
            Z = (short)vector.Z;
            W = (short)vector.W;
        }

        #endregion

        #region Equals

        public readonly bool Equals(NormalizedShort4 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is NormalizedShort4 other && Equals(other);
        }

        public static bool operator ==(in NormalizedShort4 a, in NormalizedShort4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in NormalizedShort4 a, in NormalizedShort4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(NormalizedShort4) + $"({X}, {Y}, {Z}, {W})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
