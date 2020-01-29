// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 32-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray32 : IPackedVector<uint>, IEquatable<Gray32>, IPixel
    {
        // skip using the PackedVectorHelper class as Gray32 is the
        // only pixel type that needs these kinds of 32-bit conversions
        private const double ByteMul = uint.MaxValue / (double)byte.MaxValue;
        private const double ShortMul = uint.MaxValue / (double)short.MaxValue;

        [CLSCompliant(false)]
        public uint L;

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { get => L; set => L = value; }

        public void FromVector4(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            vector *= uint.MaxValue;

            L = PackedVectorHelper.Get32BitBT709Luminance(
                (uint)vector.X, (uint)vector.Y, (uint)vector.Z);
        }

        public readonly Vector4 ToVector4()
        {
            float l = (float)(L / (double)uint.MaxValue);
            return new Vector4(l, l, l, 1f);
        }

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public void FromGray8(Gray8 source) => L = (uint)(source.PackedValue * ByteMul);

        public void FromGray16(Gray16 source) => L = (uint)(source.PackedValue * ShortMul);

        public void FromGrayAlpha16(GrayAlpha88 source) => L = (uint)(source.PackedValue * ByteMul);

        public void FromRgb24(Rgb24 source) =>
            L = (uint)(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B) * ByteMul);

        public void FromColor(Color source) =>
            L = (uint)(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B) * ByteMul);

        public void FromRgb48(Rgb48 source) =>
            L = (uint)(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B) * ShortMul);

        public void FromRgba64(Rgba64 source) =>
            L = (uint)(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B) * ShortMul);

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

        public override string ToString() => nameof(Gray32) + $"({L.ToString()})";

        public override int GetHashCode() => L.GetHashCode();

        #endregion
    }
}
