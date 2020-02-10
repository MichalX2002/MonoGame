// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing 16-bit floating-point XY components.
    /// <para>Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HalfVector2 : IPackedVector<uint>, IEquatable<HalfVector2>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, Unsafe.SizeOf<HalfSingle>() * 8),
            new VectorComponent(VectorComponentType.Green, Unsafe.SizeOf<HalfSingle>() * 8));

        public HalfSingle X;
        public HalfSingle Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public HalfVector2(HalfSingle x, HalfSingle y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public HalfVector2(uint packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public HalfVector2(float x, float y) => this = Pack(x, y);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector2"/> containing the components.</param>
        public HalfVector2(Vector2 vector) : this(vector.X, vector.Y)
        {
        }

        #endregion

        private static HalfVector2 Pack(float x, float y) => new HalfVector2(
            new HalfSingle(x),
            new HalfSingle(y));

        public readonly Vector2 ToVector2() => new Vector2(X, Y);

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<HalfVector2, uint>(this);
            set => Unsafe.As<HalfVector2, uint>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(vector.X, vector.Y);

        public readonly Vector4 ToVector4()
        {
            var vector = ToVector2();
            return new Vector4(vector.X, vector.Y, 0, 1);
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

        #region IPixel

        public void FromScaledVector4(Vector4 vector)
        {
            var scaled = new Vector2(vector.X, vector.Y);
            scaled *= 2;
            scaled -= Vector2.One;
            this = Pack(scaled.X, scaled.Y);
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += Vector2.One;
            scaled /= 2F;
            return new Vector4(scaled, 0, 1);
        }

        #endregion

        #region Equals

        public static bool operator ==(HalfVector2 a, HalfVector2 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(HalfVector2 a, HalfVector2 b) => a.PackedValue != b.PackedValue;

        public bool Equals(HalfVector2 other) => this == other;
        public override bool Equals(object obj) => obj is HalfVector2 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(HalfVector2) + $"({ToVector2().ToString()})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
