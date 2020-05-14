// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XY components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rg16 : IPackedPixel<Rg16, ushort>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Green));

        [CLSCompliant(false)]
        public byte R;

        [CLSCompliant(false)]
        public byte G;

        #region Constructors

        [CLSCompliant(false)]
        public Rg16(byte x, byte y)
        {
            R = x;
            G = y;
        }

        [CLSCompliant(false)]
        public Rg16(ushort packed) : this()
        {
            PackedValue = packed;
        }

        #endregion

        /// <summary>
        /// Gets the packed vector in <see cref="Vector2"/> format.
        /// </summary>
        public readonly Vector2 ToVector2()
        {
            return new Vector2(R, G) / byte.MaxValue;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeR.As<Rg16, ushort>(this);
            set => Unsafe.As<Rg16, ushort>(ref this) = value;
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            var vector = scaledVector.ToVector2();
            vector *= byte.MaxValue;
            vector += Vector2.Half;
            vector.Clamp(0, byte.MaxValue);

            R = (byte)vector.X;
            G = (byte)vector.Y;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToVector2(), 0, 1);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            R = G = source.L;
        }

        public void FromGray16(Gray16 source)
        {
            R = G = PackedVectorHelper.DownScale16To8Bit(source.L);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            R = G = source.L;
        }

        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromColor(Color source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromRgb48(Rgb48 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
        }

        public void FromRgba64(Rgba64 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
        }

        public readonly void ToColor(out Color destination)
        {
            destination.R = PackedVectorHelper.DownScale16To8Bit(R);
            destination.G = PackedVectorHelper.DownScale16To8Bit(G);
            destination.B = 0;
            destination.A = byte.MaxValue;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Rg16 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rg16 other && Equals(other);
        }

        public static bool operator ==(in Rg16 a, in Rg16 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Rg16 a, in Rg16 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rg16) + $"(R:{R}, G:{G})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
