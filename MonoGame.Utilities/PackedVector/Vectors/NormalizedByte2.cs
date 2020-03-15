// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XY components.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedByte2 : IPackedVector<ushort>, IEquatable<NormalizedByte2>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(sbyte) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(sbyte) * 8));

        [CLSCompliant(false)]
        public sbyte X;

        [CLSCompliant(false)]
        public sbyte Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(ushort packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public NormalizedByte2(Vector2 vector) => Pack(vector, out this);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public NormalizedByte2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        public readonly Vector2 ToVector2() => new Vector2(X, Y) / 127f;

        private static void Pack(in Vector2 vector, out NormalizedByte2 destination)
        {
            Vector2.Clamp(vector, -1, 1, out var v);
            v *= 127f;

            destination.X = (sbyte)v.X;
            destination.Y = (sbyte)v.Y;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeUtils.As<NormalizedByte2, ushort>(this);
            set => Unsafe.As<NormalizedByte2, ushort>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            Pack(vector.ToVector2(), out this);
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.X = X / sbyte.MaxValue;
            vector.Y = Y / sbyte.MaxValue;
            vector.Z = 0;
            vector.W = 1;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var scaled = scaledVector.ToVector2();
            scaled *= 2;
            scaled -= Vector2.One;
            Pack(scaled, out this);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
            scaledVector.X = (scaledVector.X + 1) / 2;
            scaledVector.Y = (scaledVector.Y + 1) / 2;
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

        public static bool operator ==(in NormalizedByte2 a, in NormalizedByte2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(in NormalizedByte2 a, in NormalizedByte2 b) => !(a == b);

        public override bool Equals(object obj) => obj is NormalizedByte2 other && Equals(other);

        public bool Equals(NormalizedByte2 other) => this == other;

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(NormalizedByte2) + $"({X}, {Y})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
