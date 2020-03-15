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
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Luminance, sizeof(uint) * 8));

        [CLSCompliant(false)]
        public uint L;

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { readonly get => L; set => L = value; }

        public void FromVector4(in Vector4 vector) => FromScaledVector4(vector);

        public readonly void ToVector4(out Vector4 vector) => ToScaledVector4(out vector);

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            Vector4.Clamp(scaledVector, 0, 1, out var v);
            v *= uint.MaxValue;

            L = PackedVectorHelper.Get32BitBT709Luminance((uint)v.X, (uint)v.Y, (uint)v.Z);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.X = scaledVector.Y = scaledVector.Z = (float)(L / (double)uint.MaxValue);
            scaledVector.W = 1;
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source) => L = PackedVectorHelper.UpScale8To32Bit(source.L);

        public void FromGray16(Gray16 source) => L = PackedVectorHelper.UpScale16To32Bit(source.L);

        public void FromGrayAlpha16(GrayAlpha16 source) => L = PackedVectorHelper.UpScale8To32Bit(source.L);

        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.UpScale8To32Bit(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromColor(Color source)
        {
            L = PackedVectorHelper.UpScale8To32Bit(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.UpScale16To32Bit(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.UpScale16To32Bit(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
        }

        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = PackedVectorHelper.DownScale32To8Bit(L);
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
