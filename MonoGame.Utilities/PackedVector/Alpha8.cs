// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing an 8-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Alpha8 : IPackedVector<byte>, IEquatable<Alpha8>, IPixel
    {
        public byte A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        public Alpha8(byte value) => A = value;

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha8(float alpha) => A = Pack(alpha);

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public float ToAlpha() => A / 255f;

        private static byte Pack(float alpha)
        {
            alpha = MathHelper.Clamp(alpha, 0, 1);
            alpha *= 255f;
            alpha += 0.5f;
            return (byte)alpha;
        }

        #region IPackedVector

        public byte PackedValue { readonly get => A; set => A = value; }

        public void FromVector4(Vector4 vector) => PackedValue = Pack(vector.W);

        public readonly Vector4 ToVector4() => new Vector4(0, 0, 0, A / 255f);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public void FromGray8(Gray8 source) => A = byte.MaxValue;

        public void FromGray16(Gray16 source) => A = byte.MaxValue;

        public void FromGrayAlpha16(GrayAlpha88 source) => A = source.A;

        public void FromRgb24(Rgb24 source) => A = byte.MaxValue;

        public void FromColor(Color source) => A = source.A;

        public void FromRgb48(Rgb48 source) => A = byte.MaxValue;

        public void FromRgba64(Rgba64 source) => A = PackedVectorHelper.DownScale16To8Bit(source.A);

        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = byte.MaxValue;
            destination.A = A;
        }

        #endregion

        #region Equals

        public static bool operator ==(Alpha8 a, Alpha8 b) => a.A == b.A;
        public static bool operator !=(Alpha8 a, Alpha8 b) => a.A != b.A;

        public bool Equals(Alpha8 other) => this == other;
        public override bool Equals(object obj) => obj is Alpha8 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Alpha8) + $"({A.ToString()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => A;

        #endregion
    }
}
