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
    public struct Alpha16 : IPackedVector<ushort>, IEquatable<Alpha16>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Alpha, sizeof(ushort) * 8));

        [CLSCompliant(false)]
        public ushort A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        [CLSCompliant(false)]
        public Alpha16(ushort value) => A = value;

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha16(float alpha) => A = Pack(alpha);

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public readonly float ToAlpha() => A / (float)ushort.MaxValue;

        private static ushort Pack(float alpha)
        {
            alpha *= ushort.MaxValue;
            alpha += 0.5f;
            alpha = MathHelper.Clamp(alpha, 0, ushort.MaxValue);
            return (ushort)alpha;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { readonly get => A; set => A = value; }

        public void FromVector4(in Vector4 vector) => FromScaledVector4(vector);

        public readonly void ToVector4(out Vector4 vector) => ToScaledVector4(out vector);

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            A = Pack(scaledVector.W);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.X = scaledVector.Y = scaledVector.Z = 1;
            scaledVector.W = ToAlpha();
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source) => A = ushort.MaxValue;

        public void FromGray16(Gray16 source) => A = ushort.MaxValue;

        public void FromGrayAlpha16(GrayAlpha16 source) => A = PackedVectorHelper.UpScale8To16Bit(source.A);

        public void FromRgb24(Rgb24 source) => A = ushort.MaxValue;

        public void FromColor(Color source) => A = PackedVectorHelper.UpScale8To16Bit(source.A);

        public void FromRgb48(Rgb48 source) => A = ushort.MaxValue;

        public void FromRgba64(Rgba64 source) => A = source.A;

        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = byte.MaxValue;
            destination.A = PackedVectorHelper.DownScale16To8Bit(A);
        }

        #endregion

        #region Equals

        public static bool operator ==(Alpha16 a, Alpha16 b) => a.A == b.A;
        public static bool operator !=(Alpha16 a, Alpha16 b) => a.A != b.A;

        public bool Equals(Alpha16 other) => this == other;
        public override bool Equals(object obj) => obj is Alpha16 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Alpha16) + $"({A.ToString()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => A;

        #endregion
    }
}
