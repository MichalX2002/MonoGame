// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing an 8-bit XYZ luminance an 8-bit W component.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GrayAlpha16 : IPackedVector<ushort>, IEquatable<GrayAlpha16>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Luminance, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(byte) * 8));

        public byte L;
        public byte A;

        public GrayAlpha16(byte luminance, byte alpha)
        {
            L = luminance;
            A = alpha;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeUtils.As<GrayAlpha16, ushort>(this);
            set => Unsafe.As<GrayAlpha16, ushort>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            var v = vector * byte.MaxValue;
            v += Vector4.Half;
            v.Clamp(0, byte.MaxValue);

            L = PackedVectorHelper.Get8BitBT709Luminance((byte)v.X, (byte)v.Y, (byte)v.Z);
            A = (byte)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = vector.Base.Y = vector.Base.Z = L / (float)byte.MaxValue;
            vector.Base.W = A / (float)byte.MaxValue;
        }

        public void FromScaledVector4(in Vector4 vector)
        {
            FromVector4(vector);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            L = source.L;
            A = byte.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            L = PackedVectorHelper.DownScale16To8Bit(source.L);
            A = byte.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            L = source.L;
            A = source.A;
        }

        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        public void FromColor(Color source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.DownScale16To8Bit(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
            A = byte.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.DownScale16To8Bit(PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
            A = byte.MaxValue;
        }

        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = L;
            destination.A = A;
        }

        #endregion

        public void FromArgb32(Argb32 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
            A = source.A;
        }

        public void FromBgr24(Bgr24 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        public void FromBgra32(Bgra32 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
            A = source.A;
        }

        #region Equals

        public static bool operator ==(GrayAlpha16 a, GrayAlpha16 b) => a.L == b.L && a.A == b.A;
        public static bool operator !=(GrayAlpha16 a, GrayAlpha16 b) => !(a == b);

        public bool Equals(GrayAlpha16 other) => this == other;
        public override bool Equals(object obj) => obj is GrayAlpha16 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(GrayAlpha16) + $"(L:{L}, A:{A})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
