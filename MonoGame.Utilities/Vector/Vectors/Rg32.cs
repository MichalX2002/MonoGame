// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing signed 16-bit XY components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rg32 : IPackedPixel<Rg32, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Green));

        [CLSCompliant(false)]
        public ushort R;

        [CLSCompliant(false)]
        public ushort G;

        #region Constructors

        [CLSCompliant(false)]
        public Rg32(ushort x, ushort y)
        {
            R = x;
            G = y;
        }

        [CLSCompliant(false)]
        public Rg32(uint packed) : this()
        {
            PackedValue = packed;
        }

        #endregion

        /// <summary>
        /// Gets the packed vector in <see cref="Vector2"/> format.
        /// </summary>
        public readonly Vector2 ToVector2()
        {
            return new Vector2(R, G) / ushort.MaxValue;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<Rg32, uint>(this);
            set => Unsafe.As<Rg32, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            var v = scaledVector.ToVector2() * ushort.MaxValue;
            v += new Vector2(0.5f);
            v.Clamp(ushort.MinValue, ushort.MaxValue);

            R = (ushort)v.X;
            G = (ushort)v.Y;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToVector2(), 0, 1);
        }

        #endregion

        #region IPixel

        public void FromGray(Gray8 source)
        {
            R = G = ScalingHelper.ToUInt16(source.L);
        }

        public void FromGray(Gray16 source)
        {
            R = G = source.L;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            R = G = source.L;
        }

        public void FromRgb(Rgb24 source)
        {
            R = ScalingHelper.ToUInt16(source.R);
            G = ScalingHelper.ToUInt16(source.G);
        }

        public void FromRgba(Color source)
        {
            R = ScalingHelper.ToUInt16(source.R);
            G = ScalingHelper.ToUInt16(source.G);
        }

        public void FromRgb(Rgb48 source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromRgba(Rgba64 source)
        {
            R = source.R;
            G = source.G;
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

        public readonly bool Equals(Rg32 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rg32 other && Equals(other);
        }

        public static bool operator ==(in Rg32 a, in Rg32 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Rg32 a, in Rg32 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rg32) + $"(R:{R}, G:{G})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
