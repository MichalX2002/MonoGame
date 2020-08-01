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
    /// Packed vector type containing unsigned 4-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra4444 : IPixel<Bgra4444>, IPackedVector<ushort>
    {
        public const int MaxXYZW = 0x0F;

        readonly VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 4));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra4444(ushort packed)
        {
            PackedValue = packed;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="scaledVector"><see cref="Vector4"/> containing the components.</param>
        public Bgra4444(Vector4 scaledVector) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            FromScaledVector(scaledVector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgra4444(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= MaxXYZW;
            scaledVector += new Vector3(0.5f);

            PackedValue = (ushort)(
                (MaxXYZW << 12) |
                (((int)scaledVector.X) << 8) |
                (((int)scaledVector.Y) << 4) |
                ((int)scaledVector.Z));
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= MaxXYZW;
            scaledVector += new Vector4(0.5f);

            PackedValue = (ushort)(
                (((int)scaledVector.W) << 12) |
                (((int)scaledVector.X) << 8) |
                (((int)scaledVector.Y) << 4) |
                ((int)scaledVector.Z));
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(
                (PackedValue >> 8) & MaxXYZW,
                (PackedValue >> 4) & MaxXYZW,
                PackedValue & MaxXYZW) / MaxXYZW;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 8) & MaxXYZW,
                (PackedValue >> 4) & MaxXYZW,
                PackedValue & MaxXYZW,
                PackedValue >> 12) / MaxXYZW;
        }

        public readonly Vector3 ToVector3() => ToScaledVector3();
        public readonly Vector4 ToVector4() => ToScaledVector4();

        #endregion

        #region IPixel.From

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FromAlphaUInt4(byte value) => PackedValue = (ushort)(ushort.MaxValue & (value << 12));

        public void FromAlpha(Alpha8 source) => FromAlphaUInt4(ScalingHelper.ToUInt4(source.A));
        public void FromAlpha(Alpha16 source) => FromAlphaUInt4(ScalingHelper.ToUInt4(source.A));
        public void FromAlpha(Alpha32 source) => FromAlphaUInt4(ScalingHelper.ToUInt4(source.A));
        public void FromAlpha(AlphaF source) => FromAlphaUInt4(ScalingHelper.ToUInt4(source.A));

        public void FromColor(Bgra4444 source) => this = source;

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8() => ScalingHelper.ToUInt8From4((byte)(PackedValue >> 12));
        public readonly Alpha16 ToAlpha16() => ScalingHelper.ToUInt16From4((byte)(PackedValue >> 12));
        public readonly AlphaF ToAlphaF() => ScalingHelper.ToFloat32FromUInt4((byte)(PackedValue >> 12));

        public readonly Gray8 ToGray8() => PixelHelper.ToGray8(this);
        public readonly Gray16 ToGray16() => PixelHelper.ToGray16(this);
        public readonly GrayF ToGrayF() => PixelHelper.ToGrayF(this);
        public readonly GrayAlpha16 ToGrayAlpha16() => PixelHelper.ToGrayAlpha16(this);

        public readonly Rgb24 ToRgb24() => ScaledVectorHelper.ToRgb24(this);
        public readonly Rgb48 ToRgb48() => ScaledVectorHelper.ToRgb48(this);

        public readonly Color ToRgba32() => ScaledVectorHelper.ToRgba32(this);
        public readonly Rgba64 ToRgba64() => ScaledVectorHelper.ToRgba64(this);

        #endregion

        #region Equals

        [CLSCompliant(false)]
        public readonly bool Equals(ushort other) => PackedValue == other;

        public readonly bool Equals(Bgra4444 other) => this == other;

        public static bool operator ==(Bgra4444 a, Bgra4444 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgra4444 a, Bgra4444 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

        public override readonly bool Equals(object? obj) => obj is Bgra4444 other && Equals(other);

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(PackedValue);

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgra4444) + $"({ToScaledVector4()})";

        #endregion
    }
}
