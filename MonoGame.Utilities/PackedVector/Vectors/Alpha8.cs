// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing an 8-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Alpha8 : IPackedPixel<Alpha8, byte>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Alpha));

        public byte A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        public Alpha8(byte value)
        {
            A = value;
        }

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha8(float alpha)
        {
            A = Pack(alpha);
        }

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public readonly float ToAlpha()
        {
            return A / (float)byte.MaxValue;
        }

        private static byte Pack(float alpha)
        {
            alpha *= byte.MaxValue;
            alpha += 0.5f;
            alpha = MathHelper.Clamp(alpha, 0, 255);
            
            return (byte)alpha;
        }

        #region IPackedVector

        public byte PackedValue { readonly get => A; set => A = value; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            A = Pack(scaledVector.W);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(1, 1, 1, ToAlpha());
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            A = byte.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            A = byte.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            A = source.A;
        }

        public void FromRgb24(Rgb24 source)
        {
            A = byte.MaxValue;
        }

        public void FromColor(Color source)
        {
            A = source.A;
        }

        public void FromRgb48(Rgb48 source)
        {
            A = byte.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            A = PackedVectorHelper.DownScale16To8Bit(source.A);
        }

        public readonly void ToColor(out Color destination)
        {
            destination.R = destination.G = destination.B = byte.MaxValue;
            destination.A = A;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Alpha8 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Alpha8 other && Equals(other);
        }

        public static bool operator ==(Alpha8 a, Alpha8 b)
        {
            return a.A == b.A;
        }

        public static bool operator !=(Alpha8 a, Alpha8 b)
        {
            return a.A != b.A;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Alpha8) + $"({A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => A;

        #endregion
    }
}
