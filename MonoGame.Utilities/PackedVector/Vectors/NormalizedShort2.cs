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

        public NormalizedShort2(Vector2 vector) => Pack(vector, out this);

        public NormalizedShort2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        public readonly Vector2 ToVector2() => new Vector2(X, Y) / short.MaxValue;

        private static void Pack(in Vector2 vector, out NormalizedShort2 destination)
        {
            var v = Vector2.Clamp(vector, -Vector2.One, Vector2.One);
            v *= short.MaxValue;

            destination.X = (short)v.X;
            destination.Y = (short)v.Y;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<NormalizedShort2, uint>(this);
            set => Unsafe.As<NormalizedShort2, uint>(ref this) = value;
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
            vector.Base.W = short.MaxValue;
            vector /= short.MaxValue;
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
