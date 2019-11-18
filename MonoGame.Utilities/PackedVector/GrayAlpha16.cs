// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 8-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct GrayAlpha16 : IPackedVector<ushort>, IEquatable<GrayAlpha16>, IPixel
    {
        public byte L;
        public byte A;

        public GrayAlpha16(byte luminance, byte alpha)
        {
            L = luminance;
            A = alpha;
        }

        internal static GrayAlpha16 Pack(ref Vector4 vector)
        {
            vector *= Vector4.MaxBytes;
            vector += Vector4.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.MaxBytes);

            return new GrayAlpha16(PackedVectorHelper.Get8BitBT709Luminance(
                (byte)vector.X, (byte)vector.Y, (byte)vector.Z), (byte)vector.W);
        }

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ushort PackedValue
        {
            get => Unsafe.As<GrayAlpha16, ushort>(ref this);
            set => Unsafe.As<GrayAlpha16, ushort>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        /// <inheritdoc/>
        public readonly Vector4 ToVector4() => new Vector4(L, L, L, A) / byte.MaxValue;

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromGray8(Gray8 source)
        {
            L = source.L;
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromGray16(Gray16 source)
        {
            L = PackedVectorHelper.DownScale16To8Bit(source.L);
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            L = source.L;
            A = source.A;
        }

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void FromColor(Color source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void ToColor(ref Color destination)
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
        public override bool Equals(object obj) => obj is Gray8 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => $"GrayAlpha16({PackedValue})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
