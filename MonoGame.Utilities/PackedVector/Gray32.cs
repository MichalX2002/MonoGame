// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 32-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Gray32 : IPackedVector<uint>, IEquatable<Gray32>, IPixel
    {
        // skip using the PackedVectorHelper class as Gray32 is the
        // only pixel type that needs these kinds of 32-bit conversions
        private const double ByteMul = uint.MaxValue / 255d;
        private const double ShortMul = uint.MaxValue / 65536d;

        [CLSCompliant(false)]
        public uint L;

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { get => L; set => L = value; }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            vector *= uint.MaxValue;

            L = PackedVectorHelper.Get32BitBT709Luminance(
                (uint)vector.X, (uint)vector.Y, (uint)vector.Z);
        }

        /// <inheritdoc/>
        public readonly Vector4 ToVector4()
        {
            float l = (float)(L / (double)uint.MaxValue);
            return new Vector4(l, l, l, 1f);
        }

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromGray8(Gray8 source) => L = (uint)(source.PackedValue * ByteMul);

        /// <inheritdoc/>
        public void FromGray16(Gray16 source) => L = (uint)(source.PackedValue * ShortMul);

        /// <inheritdoc/>
        public void FromGrayAlpha16(GrayAlpha16 source) => FromGray8(source.L);

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source) =>
            L = (uint)(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B) * ByteMul);

        /// <inheritdoc/>
        public void FromColor(Color source) =>
            L = (uint)(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B) * ByteMul);

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) =>
            L = (uint)(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B) * ShortMul);

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) =>
            L = (uint)(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B) * ShortMul);

        /// <inheritdoc/>
        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = (byte)(L * 255);
            destination.A = byte.MaxValue;
        }

        #endregion

        #region Equals

        public static bool operator ==(in Gray32 a, in Gray32 b) => a.L == b.L;
        public static bool operator !=(in Gray32 a, in Gray32 b) => a.L != b.L;

        public bool Equals(Gray32 other) => this == other;
        public override bool Equals(object obj) => obj is Gray32 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => $"Gray32({L})";

        public override int GetHashCode() => L.GetHashCode();

        #endregion
    }
}
