// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing an 32-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Alpha32 : IPackedPixel<Alpha32, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int32, VectorComponentChannel.Alpha));

        [CLSCompliant(false)]
        public uint A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        [CLSCompliant(false)]
        public Alpha32(uint value)
        {
            A = value;
        }

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha32(float alpha)
        {
            A = Pack(alpha);
        }

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public readonly float ToAlpha()
        {
            return A / (float)uint.MaxValue;
        }

        private static uint Pack(float alpha)
        {
            alpha *= uint.MaxValue;
            alpha += 0.5f;
            alpha = MathHelper.Clamp(alpha, 0, uint.MaxValue);

            return (uint)alpha;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { readonly get => A; set => A = value; }

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
            A = uint.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            A = uint.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            A = PackedVectorHelper.UpScale8To16Bit(source.A);
        }

        public void FromRgb24(Rgb24 source)
        {
            A = uint.MaxValue;
        }

        public void FromRgba32(Color source)
        {
            A = PackedVectorHelper.UpScale8To16Bit(source.A);
        }

        public void FromRgb48(Rgb48 source)
        {
            A = uint.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            A = source.A;
        }

        public readonly Color ToColor()
        {
            return new Color(byte.MaxValue, PackedVectorHelper.DownScale16To8Bit(A));
        }

        #endregion

        #region Equals

        public readonly bool Equals(Alpha32 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Alpha32 other && Equals(other);
        }

        public static bool operator ==(in Alpha32 a, in Alpha32 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Alpha32 a, in Alpha32 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Alpha32) + $"({A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => A.GetHashCode();

        #endregion

        [CLSCompliant(false)]
        public static implicit operator Alpha32(uint alpha) => new Alpha32(alpha);

        [CLSCompliant(false)]
        public static implicit operator uint(Alpha32 value) => value.A;
    }
}
