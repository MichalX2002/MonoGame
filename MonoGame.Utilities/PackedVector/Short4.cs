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
    /// Ranges from [-37267, -37267, -37267, -37267] to [37267, 37267, 37267, 37267] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Short4 : IPackedVector<ulong>, IEquatable<Short4>, IPixel
    {
        internal static Vector4 MinNeg = new Vector4(short.MinValue);
        internal static Vector4 MaxPos = new Vector4(short.MaxValue);
        internal static Vector4 MaxPosHalf = new Vector4(32767.5f);

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

        public Short4(Vector4 vector) => this = Pack(ref vector);

        public Short4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static Short4 Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, MinNeg, MaxPos);
            vector.Round();

            return new Short4(
                (short)vector.X,
                (short)vector.Y,
                (short)vector.Z,
                (short)vector.W);
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeUtils.As<Short4, ulong>(this);
            set => Unsafe.As<Short4, ulong>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        public readonly Vector4 ToVector4() => new Vector4(X, Y, Z, W);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector)
        {
            var scaled = vector * 65534f;
            scaled -= MaxPos;
            this = Pack(ref scaled);
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaled = ToVector4();
            scaled += MaxPos;
            scaled /= 65534f;
            return scaled;
        }

        public readonly void ToColor(ref Color destination) => destination.FromScaledVector4(ToScaledVector4());

        public void FromGray8(Gray8 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGray16(Gray16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGrayAlpha16(GrayAlpha88 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb24(Rgb24 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromColor(Color source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

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
