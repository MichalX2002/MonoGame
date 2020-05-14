// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XYZW components.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedByte4 : IPackedPixel<NormalizedByte4, uint>
    {
        internal static Vector4 Offset => new Vector4(-sbyte.MinValue);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Alpha));

        [CLSCompliant(false)]
        public sbyte X;

        [CLSCompliant(false)]
        public sbyte Y;

        [CLSCompliant(false)]
        public sbyte Z;

        [CLSCompliant(false)]
        public sbyte W;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte4(sbyte x, sbyte y, sbyte z, sbyte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte4(uint packed) : this()
        {
            PackedValue = packed;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<NormalizedByte4, uint>(this);
            set => Unsafe.As<NormalizedByte4, uint>(ref this) = value;
        }
        public readonly Vector4 ToScaledVector4()
        {
            var scaled = new Vector4(X, Y, Z, W);
            scaled += Offset;
            scaled /= byte.MaxValue;

            return scaled;
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector.Clamp(Vector4.Zero, Vector4.One);
            scaledVector *= byte.MaxValue;
            scaledVector -= Offset;

            X = (sbyte)scaledVector.X;
            Y = (sbyte)scaledVector.Y;
            Z = (sbyte)scaledVector.Z;
            W = (sbyte)scaledVector.W;
        }

        public readonly Vector4 ToVector4()
        {
            var vector = new Vector4(X, Y, Z, W);
            vector += Offset;
            vector *= 2f / byte.MaxValue;
            vector -= Vector4.One;

            return vector;
        }

        public void FromVector4(Vector4 vector)
        {
            vector.Clamp(Vector4.NegativeOne, Vector4.One);
            vector *= byte.MaxValue / 2f;
            vector -= Vector4.Half;

            X = (sbyte)vector.X;
            Y = (sbyte)vector.Y;
            Z = (sbyte)vector.Z;
            W = (sbyte)vector.W;
        }

        #endregion

        #region Equals

        public readonly bool Equals(NormalizedByte4 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is NormalizedByte4 other && Equals(other);
        }

        public static bool operator ==(in NormalizedByte4 a, in NormalizedByte4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in NormalizedByte4 a, in NormalizedByte4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        public override readonly string ToString() => nameof(NormalizedByte4) + $"({X}, {Y}, {Z}, {W})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
