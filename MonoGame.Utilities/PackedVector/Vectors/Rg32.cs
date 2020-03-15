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
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rg32 : IPackedVector<uint>, IEquatable<Rg32>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(ushort) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(ushort) * 8));

        [CLSCompliant(false)]
        public ushort R;

        [CLSCompliant(false)]
        public ushort G;

        #region Constructors

        [CLSCompliant(false)]
        public Rg32(ushort x, ushort y)
        {
            R = x;
            G = y;
        }

        [CLSCompliant(false)]
        public Rg32(uint packed) : this() => PackedValue = packed;

        public Rg32(Vector2 vector) : this()
        {
            FromVector4(new Vector4(vector, 0, 1));
        }

        public Rg32(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        /// <summary>
        /// Gets the packed vector in <see cref="Vector2"/> format.
        /// </summary>
        public readonly Vector2 ToVector2() => new Vector2(R, G) / ushort.MaxValue;

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<Rg32, uint>(this);
            set => Unsafe.As<Rg32, uint>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            Vector2.Multiply(vector.ToVector2(), ushort.MaxValue, out var v);
            Vector2.Add(v, Vector2.Half, out v);
            v.Clamp(0, ushort.MaxValue);

            R = (ushort)v.X;
            G = (ushort)v.Y;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.X = R / (float)ushort.MaxValue;
            vector.Y = G / (float)ushort.MaxValue;
            vector.Z = 0;
            vector.W = 1;
        }

        public void FromScaledVector4(in Vector4 scaledVector) => FromVector4(scaledVector);

        public readonly void ToScaledVector4(out Vector4 scaledVector) => ToVector4(out scaledVector);

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source) => R = G = PackedVectorHelper.UpScale8To16Bit(source.L);

        public void FromGray16(Gray16 source) => R = G = source.L;

        public void FromGrayAlpha16(GrayAlpha16 source) => R = G = source.L;

        public void FromRgb24(Rgb24 source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
        }

        public void FromColor(Color source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
        }

        public void FromRgb48(Rgb48 source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromRgba64(Rgba64 source)
        {
            R = source.R;
            G = source.G;
        }

        public readonly void ToColor(ref Color destination)
        {
            destination.R = PackedVectorHelper.DownScale16To8Bit(R);
            destination.G = PackedVectorHelper.DownScale16To8Bit(G);
        }

        #endregion

        #region Equals

        public static bool operator ==(in Rg32 a, in Rg32 b) => a.R == b.R && a.G == b.G;
        public static bool operator !=(in Rg32 a, in Rg32 b) => !(a == b);

        /// <summary>
        /// Compares another Rg32 packed vector with the packed vector.
        /// </summary>
        /// <param name="other">The Rg32 packed vector to compare.</param>
        /// <returns>True if the packed vectors are equal.</returns>
        public bool Equals(Rg32 other) => this == other;


        /// <summary>
        /// Compares an object with the packed vector.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is equal to the packed vector.</returns>
        public override bool Equals(object obj) => obj is Rg32 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Rg32) + $"(R:{R}, G:{G})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
