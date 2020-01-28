// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 16-bit XYZW components.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedShort4 : IPackedVector<ulong>, IEquatable<NormalizedShort4>, IPixel
    {
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
        public NormalizedShort4(ulong packed) : this() => PackedValue = packed;

        public NormalizedShort4(Vector4 vector) => this = Pack(ref vector);

        public NormalizedShort4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static NormalizedShort4 Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, -Vector4.One, Vector4.One);
            vector *= 32767f;
            
            return new NormalizedShort4(
                (short)vector.X,
                (short)vector.Y,
                (short)vector.Z,
                (short)vector.W);
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            get => Unsafe.As<NormalizedShort4, ulong>(ref this);
            set => Unsafe.As<NormalizedShort4, ulong>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        public readonly Vector4 ToVector4() => new Vector4(X, Y, Z, W) / 32767f;

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

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

        public static bool operator ==(NormalizedShort4 a, NormalizedShort4 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(NormalizedShort4 a, NormalizedShort4 b) => a.PackedValue == b.PackedValue;

        public bool Equals(NormalizedShort4 other) => this == other;
        public override bool Equals(object obj) => obj is NormalizedShort4 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(NormalizedShort4) + $"({X}, {Y}, {Z}, {W})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
