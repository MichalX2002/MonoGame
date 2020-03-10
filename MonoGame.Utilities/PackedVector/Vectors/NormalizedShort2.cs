// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 16-bit XY components.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedShort2 : IPackedVector<uint>, IEquatable<NormalizedShort2>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(short) * 8));

        public short X;
        public short Y;

        #region Constructors

        public NormalizedShort2(short x, short y)
        {
            X = x;
            Y = y;
        }

        [CLSCompliant(false)]
        public NormalizedShort2(uint packed) : this() => PackedValue = packed;

        public NormalizedShort2(Vector2 vector) => this = Pack(vector);

        public NormalizedShort2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        private static NormalizedShort2 Pack(Vector2 vector)
        {
            vector = Vector2.Clamp(vector, -Vector2.One, Vector2.One);
            vector *= 32767f;

            return new NormalizedShort2(
                (short)vector.X,
                (short)vector.Y);
        }

        public readonly Vector2 ToVector2() => new Vector2(X, Y) / 32767f;

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<NormalizedShort2, uint>(this);
            set => Unsafe.As<NormalizedShort2, uint>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(vector.ToVector2());

        public readonly Vector4 ToVector4() => new Vector4(ToVector2(), 0, 1);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public readonly void ToColor(ref Color destination) => destination.FromScaledVector4(ToScaledVector4());

        public void FromGray8(Gray8 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGray16(Gray16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGrayAlpha16(GrayAlpha16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb24(Rgb24 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromColor(Color source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        #endregion

        #region Equals

        public static bool operator ==(in NormalizedShort2 a, in NormalizedShort2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(in NormalizedShort2 a, in NormalizedShort2 b) => !(a == b);

        public override bool Equals(object obj) => obj is NormalizedShort2 other && Equals(other);
        public bool Equals(NormalizedShort2 other) => this == other;

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(NormalizedShort2) + $"({X}, {Y})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
