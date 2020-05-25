// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing a 32-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GrayF : IPackedPixel<GrayF, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Luminance));

        [CLSCompliant(false)]
        public float L;

        [CLSCompliant(false)]
        public GrayF(float luminance)
        {
            L = luminance;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<GrayF, uint>(this);
            set => Unsafe.As<GrayF, uint>(ref this) = value;
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector.Clamp(Vector4.Zero, Vector4.One);

            L = PackedVectorHelper.GetBT709Luminance(scaledVector.ToVector3());
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(L, L, L, 1);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            L = source.L / (float)byte.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            L = source.L / (float)ushort.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            L = source.L / (float)byte.MaxValue;
        }

        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B) / (float)byte.MaxValue;
        }

        public void FromRgba32(Color source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B) / (float)byte.MaxValue;
        }

        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B) / (float)ushort.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B) / (float)ushort.MaxValue;
        }

        public readonly Color ToColor()
        {
            byte rgb = MathHelper.Clamp((int)(L * 255f), byte.MinValue, byte.MaxValue);
            return new Color(rgb, byte.MaxValue);
        }

        #endregion

        #region Equals

        public readonly bool Equals(GrayF other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GrayF other && Equals(other);
        }

        public static bool operator ==(GrayF a, GrayF b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(GrayF a, GrayF b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        public override readonly string ToString() => nameof(GrayF) + $"({L})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion

        public static implicit operator GrayF(float luminance) => new GrayF(luminance);

        public static implicit operator float(GrayF value) => value.L;
    }
}
