// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
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
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Green));

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

        public void FromScaledVector(Vector4 scaledVector)
        {
            var vector = scaledVector.ToVector2();
            vector *= byte.MaxValue;
            vector += new Vector2(0.5f);
            vector.Clamp(byte.MinValue, byte.MaxValue);

            R = (byte)vector.X;
            G = (byte)vector.Y;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToVector2(), 0, 1);
        }

        #endregion

        #region IPixel

        public void FromGray(Gray8 source)
        {
            R = G = source.L;
        }

        public void FromGray(Gray16 source)
        {
            R = G = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            R = G = source.L;
        }

        public void FromRgb(Rgb24 source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromRgba(Color source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromRgb(Rgb48 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
            G = ScalingHelper.ToUInt8(source.G);
        }

        public void FromRgba(Rgba64 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
            G = ScalingHelper.ToUInt8(source.G);
        }

        public readonly Color ToColor()
        {
            return new Color(
                ScalingHelper.ToUInt8(R),
                ScalingHelper.ToUInt8(G),
                (byte)0,
                byte.MaxValue);
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

        #region Object overrides

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
