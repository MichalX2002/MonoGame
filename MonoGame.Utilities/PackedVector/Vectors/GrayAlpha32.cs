// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing an 16-bit XYZ luminance an 16-bit W component.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GrayAlpha32 : IPackedVector<uint>, IEquatable<GrayAlpha32>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Luminance, sizeof(ushort) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(ushort) * 8));

        [CLSCompliant(false)]
        public ushort L;

        [CLSCompliant(false)]
        public ushort A;

        [CLSCompliant(false)]
        public GrayAlpha32(ushort luminance, ushort alpha)
        {
            L = luminance;
            A = alpha;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<GrayAlpha32, uint>(this);
            set => Unsafe.As<GrayAlpha32, uint>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            Vector4.Multiply(vector, ushort.MaxValue, out var v);
            v += Vector4.Half;
            v.Clamp(0, ushort.MaxValue);

            L = PackedVectorHelper.Get16BitBT709Luminance((ushort)v.X, (ushort)v.Y, (ushort)v.Z);
            A = (ushort)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.X = vector.Y = vector.Z = L / (float)ushort.MaxValue;
            vector.W = A / (float)ushort.MaxValue;
        }

        public void FromScaledVector4(in Vector4 scaledVector) => FromVector4(scaledVector);

        public readonly void ToScaledVector4(out Vector4 scaledVector) => ToVector4(out scaledVector);

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(source.L);
            A = ushort.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            L = source.L;
            A = ushort.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(source.L);
            A = PackedVectorHelper.UpScale8To16Bit(source.A);
        }

        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
            A = ushort.MaxValue;
        }

        public void FromColor(Color source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
            A = ushort.MaxValue;
        }

        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);
            A = ushort.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);
            A = ushort.MaxValue;
        }

        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = PackedVectorHelper.DownScale16To8Bit(L);
            destination.A = PackedVectorHelper.DownScale16To8Bit(A);
        }

        #endregion

        #region Equals

        public static bool operator ==(GrayAlpha32 a, GrayAlpha32 b) => a.L == b.L && a.A == b.A;
        public static bool operator !=(GrayAlpha32 a, GrayAlpha32 b) => !(a == b);

        public bool Equals(GrayAlpha32 other) => this == other;
        public override bool Equals(object obj) => obj is GrayAlpha32 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(GrayAlpha32) + $"(L:{L}, A:{A})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
