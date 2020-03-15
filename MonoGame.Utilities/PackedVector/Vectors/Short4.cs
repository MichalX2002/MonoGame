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
    public struct Short4 : IPackedVector<ulong>, IEquatable<Short4>, IPixel
    {
        private static readonly Vector4 Offset = new Vector4(-short.MinValue);

        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(short) * 8));

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

        public void FromVector4(in Vector4 vector)
        {
            Vector4.Add(vector, Vector4.Half, out var v);
            v.Clamp(short.MinValue, short.MaxValue);

            X = (short)v.X;
            Y = (short)v.Y;
            Z = (short)v.Z;
            W = (short)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.X = X;
            vector.Y = Y;
            vector.Z = Z;
            vector.W = W;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            Vector4.Multiply(scaledVector, ushort.MaxValue, out var v);
            v -= Offset;
            FromVector4(v);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
            scaledVector += Offset;
            scaledVector /= ushort.MaxValue;
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGray16(Gray16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb24(Rgb24 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb48(Rgb48 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgba64(Rgba64 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public readonly void ToColor(ref Color destination)
        {
            ToScaledVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

        #endregion

        #region Equals

        public static bool operator ==(in Short4 a, in Short4 b) =>
            a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        public static bool operator !=(in Short4 a, in Short4 b) => !(a == b);

        public bool Equals(Short4 other) => this == other;
        public override bool Equals(object obj) => obj is Short4 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(Short4) + $"({X}, {Y}, {Z}, {W})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
