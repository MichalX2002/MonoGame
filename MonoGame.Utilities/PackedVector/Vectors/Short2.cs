// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed pixel type containing two signed 16-bit integers.
    /// <para>
    /// Ranges from [-32768, -32768, 0, 1] to [32767, 32767, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Short2 : IPackedVector<uint>, IEquatable<Short2>, IPixel
    {
        private static readonly Vector2 MinNeg = new Vector2(short.MinValue);
        private static readonly Vector2 MaxPos = new Vector2(short.MaxValue);
        private static readonly Vector2 Offset = new Vector2(32768);

        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(short) * 8));

        public short X;
        public short Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public Short2(short x, short y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Short2(uint packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Short2(Vector2 vector) => this = Pack(vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Short2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        public readonly Vector2 ToVector2() => new Vector2(X, Y);

        private static Short2 Pack(Vector2 vector)
        {
            vector += Vector2.Half;
            vector = Vector2.Clamp(vector, MinNeg, MaxPos);

            return new Short2(
                (short)vector.X,
                (short)vector.Y);
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<Short2, uint>(this);
            set => Unsafe.As<Short2, uint>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(vector.ToVector2());

        public readonly Vector4 ToVector4() => new Vector4(X, Y, 0, 1);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector)
        {
            var scaled = vector.ToVector2() * 65535f;
            scaled -= Offset;
            this = Pack(scaled);
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += Offset;
            scaled /= 65535f;
            return new Vector4(scaled, 0, 1);
        }

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

        public static bool operator ==(in Short2 a, in Short2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(in Short2 a, in Short2 b) => !(a == b);

        public bool Equals(Short2 other) => this == other;
        public override bool Equals(object obj) => obj is Short2 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(Short2) + $"({X}, {Y})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}