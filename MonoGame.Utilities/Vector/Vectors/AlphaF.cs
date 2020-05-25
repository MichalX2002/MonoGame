// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing an 32-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AlphaF : IPackedPixel<AlphaF, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Alpha));

        [CLSCompliant(false)]
        public float A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public AlphaF(float alpha)
        {
            A = alpha;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public AlphaF(uint value) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = value;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<AlphaF, uint>(this);
            set => Unsafe.As<AlphaF, uint>(ref this) = value;
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            A = scaledVector.W;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(1, 1, 1, A);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            A = 1f;
        }

        public void FromGray16(Gray16 source)
        {
            A = 1f;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            A = source.A / (float)byte.MaxValue;
        }

        public void FromRgb24(Rgb24 source)
        {
            A = 1f;
        }

        public void FromRgba32(Color source)
        {
            A = source.A / (float)byte.MaxValue;
        }

        public void FromRgb48(Rgb48 source)
        {
            A = 1f;
        }

        public void FromRgba64(Rgba64 source)
        {
            A = source.A / (float)ushort.MaxValue;
        }

        public readonly Color ToColor()
        {
            byte a = MathHelper.Clamp((int)(A * 255), byte.MinValue, byte.MaxValue);
            return new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, a);
        }

        #endregion

        #region Equals

        public readonly bool Equals(AlphaF other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is AlphaF other && Equals(other);
        }

        public static bool operator ==(AlphaF a, AlphaF b)
        {
            return a.A == b.A;
        }

        public static bool operator !=(AlphaF a, AlphaF b)
        {
            return a.A != b.A;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(AlphaF) + $"({A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => A.GetHashCode();

        #endregion

        public static implicit operator AlphaF(float alpha) => new AlphaF(alpha);

        public static implicit operator float(AlphaF value) => value.A;
    }
}
