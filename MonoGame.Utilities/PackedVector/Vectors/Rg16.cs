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
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rg16 : IPackedVector<ushort>, IEquatable<Rg16>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(byte) * 8));

        [CLSCompliant(false)]
        public byte R;

        [CLSCompliant(false)]
        public byte G;

        #region Constructors

        [CLSCompliant(false)]
        public Rg16(byte x, byte y)
        {
            R = x;
            G = y;
        }

        [CLSCompliant(false)]
        public Rg16(ushort packed) : this() => PackedValue = packed;

        public Rg16(Vector2 vector) : this()
        {
            FromVector4(new Vector4(vector, 0, 1));
        }

        public Rg16(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        /// <summary>
        /// Gets the packed vector in <see cref="Vector2"/> format.
        /// </summary>
        public readonly Vector2 ToVector2() => new Vector2(R, G) / byte.MaxValue;

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeUtils.As<Rg16, ushort>(this);
            set => Unsafe.As<Rg16, ushort>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            Vector2.Multiply(vector.ToVector2(), byte.MaxValue, out var v);
            Vector2.Add(v, Vector2.Half, out v);
            v.Clamp(0, byte.MaxValue);

            R = (byte)v.X;
            G = (byte)v.Y;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.X = R / (float)byte.MaxValue;
            vector.Y = G / (float)byte.MaxValue;
            vector.Z = 0;
            vector.W = 1;
        }

        public void FromScaledVector4(in Vector4 scaledVector) => FromVector4(scaledVector);

        public readonly void ToScaledVector4(out Vector4 scaledVector) => ToVector4(out scaledVector);

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source) => R = G = source.L;

        public void FromGray16(Gray16 source) => R = G = PackedVectorHelper.DownScale16To8Bit(source.L);

        public void FromGrayAlpha16(GrayAlpha16 source) => R = G = source.L;

        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromColor(Color source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromRgb48(Rgb48 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
        }

        public void FromRgba64(Rgba64 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
        }

        public readonly void ToColor(ref Color destination)
        {
            destination.R = PackedVectorHelper.DownScale16To8Bit(R);
            destination.G = PackedVectorHelper.DownScale16To8Bit(G);
        }

        #endregion

        #region Equals

        public static bool operator ==(in Rg16 a, in Rg16 b) => a.R == b.R && a.G == b.G;
        public static bool operator !=(in Rg16 a, in Rg16 b) => !(a == b);

        /// <summary>
        /// Compares another <see cref="Rg16"/> packed vector with the packed vector.
        /// </summary>
        /// <param name="other">The <see cref="Rg16"/> packed vector to compare.</param>
        /// <returns>True if the packed vectors are equal.</returns>
        public bool Equals(Rg16 other) => this == other;


        /// <summary>
        /// Compares an object with the packed vector.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is equal to the packed vector.</returns>
        public override bool Equals(object obj) => obj is Rg16 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Rg16) + $"(R:{R}, G:{G})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
