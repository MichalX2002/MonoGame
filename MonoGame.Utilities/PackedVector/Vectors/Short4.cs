// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
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
        private static readonly Vector4 MinValue = new Vector4(short.MinValue);
        private static readonly Vector4 MaxValue = new Vector4(short.MaxValue);
        private static readonly Vector4 Offset = new Vector4(32768);

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
            FromVector4(vector);
        }

        public Short4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeUtils.As<Short4, ulong>(this);
            set => Unsafe.As<Short4, ulong>(ref this) = value;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var v = scaledVector * ushort.MaxValue;
            v -= Offset;
            FromVector4(v);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
            scaledVector += Offset;
            scaledVector /= ushort.MaxValue;
        }

        public void FromVector4(in Vector4 vector)
        {
            var v = vector + Vector4.Half;
            v.Clamp(MinValue, MaxValue);

            X = (short)v.X;
            Y = (short)v.Y;
            Z = (short)v.Z;
            W = (short)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = X;
            vector.Base.Y = Y;
            vector.Base.Z = Z;
            vector.Base.W = W;
        }

        #endregion

        #region Equals

        public static bool operator ==(in Short4 a, in Short4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Short4 a, in Short4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public bool Equals(Short4 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Short4 other && Equals(other);
        }

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(Short4) + $"({X}, {Y}, {Z}, {W})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
