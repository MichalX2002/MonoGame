// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing three unsigned 16-bit XYZ components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgb48 : IPixel<Rgb48>
    {
        public static Rgb48 Black => new Rgb48(ushort.MinValue, ushort.MinValue, ushort.MinValue);
        public static Rgb48 White => new Rgb48(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Blue));

        [CLSCompliant(false)]
        public ushort R;

        [CLSCompliant(false)]
        public ushort G;

        [CLSCompliant(false)]
        public ushort B;

        #region Constructors

        [CLSCompliant(false)]
        public Rgb48(ushort r, ushort g, ushort b)
        {
            R = r;
            G = g;
            B = b;
        }

        [CLSCompliant(false)]
        public Rgb48(ushort value) : this(value, value, value)
        {
        }

        #endregion

        #region IPackedVector

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector *= ushort.MaxValue;
            scaledVector += new Vector3(0.5f);
            scaledVector = VectorHelper.ZeroMax(scaledVector, ushort.MaxValue);

            R = (ushort)scaledVector.X;
            G = (ushort)scaledVector.Y;
            B = (ushort)scaledVector.Z;
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(R, G, B) / ushort.MaxValue;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            FromScaledVector(scaledVector.ToVector3());
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToScaledVector3(), 1);
        }

        #endregion

        #region IPixel.From

        public void FromGray(Gray8 source)
        {
            R = G = B = ScalingHelper.ToUInt16(source.L);
        }

        public void FromGray(Gray16 source)
        {
            R = G = B = source.L;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            R = G = B = ScalingHelper.ToUInt16(source.L);
        }

        public void FromRgb(Rgb24 source)
        {
            R = ScalingHelper.ToUInt16(source.R);
            G = ScalingHelper.ToUInt16(source.G);
            B = ScalingHelper.ToUInt16(source.B);
        }

        public void FromRgba(Color source)
        {
            R = ScalingHelper.ToUInt16(source.R);
            G = ScalingHelper.ToUInt16(source.G);
            B = ScalingHelper.ToUInt16(source.B);
        }

        public void FromRgb(Rgb48 source)
        {
            this = source;
        }

        public void FromRgba(Rgba64 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        #endregion

        #region IPixel.To

        public readonly Rgb24 ToRgb24()
        {
            return new Rgb24(
                ScalingHelper.ToUInt8(R),
                ScalingHelper.ToUInt8(G),
                ScalingHelper.ToUInt8(B));
        }

        public readonly Rgb48 ToRgb48()
        {
            return this;
        }

        public readonly Color ToRgba32()
        {
            return new Color(
                ScalingHelper.ToUInt8(R),
                ScalingHelper.ToUInt8(G),
                ScalingHelper.ToUInt8(B),
                byte.MaxValue);
        }

        public readonly Rgba64 ToRgba64()
        {
            return new Rgba64(R, G, B);
        }

        #endregion

        #region Equals

        public readonly bool Equals(Rgb48 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rgb48 other && Equals(other);
        }

        public static bool operator ==(in Rgb48 a, in Rgb48 b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public static bool operator !=(in Rgb48 a, in Rgb48 b)
        {
            return !(a == b);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rgb48) + $"(R:{R}, G:{G}, B:{B})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(R, G, B);

        #endregion
    }
}