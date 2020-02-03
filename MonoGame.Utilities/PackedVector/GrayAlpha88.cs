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
    public struct GrayAlpha88 : IPackedVector<ushort>, IEquatable<GrayAlpha88>, IPixel
    {
        public int BitDepth => Unsafe.SizeOf<GrayAlpha88>() * 8;

        public byte L;
        public byte A;

        public Gray8 Gray
        {
            readonly get => new Gray8(L);
            set => L = value.L;
        }

        public GrayAlpha88(byte luminance, byte alpha)
        {
            L = luminance;
            A = alpha;
        }

        private static GrayAlpha88 Pack(ref Vector4 vector)
        {
            vector *= Vector4.ByteMaxValue;
            vector += Vector4.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.ByteMaxValue);

            return new GrayAlpha88(PackedVectorHelper.Get8BitBT709Luminance(
                (byte)vector.X, (byte)vector.Y, (byte)vector.Z), (byte)vector.W);
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeUtils.As<GrayAlpha88, ushort>(this);
            set => Unsafe.As<GrayAlpha88, ushort>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        public readonly Vector4 ToVector4() => new Vector4(L, L, L, A) / byte.MaxValue;

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

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

        public void FromGrayAlpha16(GrayAlpha88 source)
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
            L = PackedVectorHelper.Get8BitBT709Luminance(
                    PackedVectorHelper.DownScale16To8Bit(source.R),
                    PackedVectorHelper.DownScale16To8Bit(source.G),
                    PackedVectorHelper.DownScale16To8Bit(source.B));
            A = byte.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(
                    PackedVectorHelper.DownScale16To8Bit(source.R),
                    PackedVectorHelper.DownScale16To8Bit(source.G),
                    PackedVectorHelper.DownScale16To8Bit(source.B));
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

        public static bool operator ==(GrayAlpha88 a, GrayAlpha88 b) => a.L == b.L && a.A == b.A;
        public static bool operator !=(GrayAlpha88 a, GrayAlpha88 b) => !(a == b);

        public bool Equals(GrayAlpha88 other) => this == other;
        public override bool Equals(object obj) => obj is Gray8 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(GrayAlpha88) + $"(L:{L.ToString()}, A:{A.ToString()})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
