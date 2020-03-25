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
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
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
        /// <param name="vector"><see cref="Vector2"/> containing the components.</param>
        public HalfVector2(Vector2 vector) => Pack(vector, out this);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public HalfVector2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        private static void Pack(in Vector2 vector, out HalfVector2 destination)
        {
            destination.X = new HalfSingle(vector.X);
            destination.Y = new HalfSingle(vector.Y);
        }

        public readonly Vector2 ToVector2() => new Vector2(X, Y);

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<HalfVector2, uint>(this);
            set => Unsafe.As<HalfVector2, uint>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            Pack(vector.ToVector2(), out this);
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = X;
            vector.Base.Y = Y;
            vector.Base.Z = 0;
            vector.Base.W = 1;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            FromVector4(scaledVector);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

        #endregion

        #region IPixel

        public void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

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

        public readonly void ToColor(ref Color destination)
        {
            ToScaledVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

        #endregion

        #region Equals

        public static bool operator ==(HalfVector2 a, HalfVector2 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(HalfVector2 a, HalfVector2 b) => a.PackedValue != b.PackedValue;

        public bool Equals(HalfVector2 other) => this == other;
        public override bool Equals(object obj) => obj is HalfVector2 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(HalfVector2) + $"({ToVector2()})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
