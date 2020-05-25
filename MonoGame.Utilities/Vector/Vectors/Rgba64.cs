// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing four unsigned 16-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgba64 : IPackedPixel<Rgba64, ulong>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Alpha));

        [CLSCompliant(false)]
        public ushort R;

        [CLSCompliant(false)]
        public ushort G;

        [CLSCompliant(false)]
        public ushort B;

        [CLSCompliant(false)]
        public ushort A;

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Rgb48"/>
        /// </summary>
        public Rgb48 Rgb
        {
            readonly get => UnsafeR.As<Rgba64, Rgb48>(this);
            set => Unsafe.As<Rgba64, Rgb48>(ref this) = value;
        }

        #region Constructors

        [CLSCompliant(false)]
        public Rgba64(ushort r, ushort g, ushort b, ushort a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeR.As<Rgba64, ulong>(this);
            set => Unsafe.As<Rgba64, ulong>(ref this) = value;
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector.Clamp(Vector4.Zero, Vector4.One);
            scaledVector *= ushort.MaxValue;
            scaledVector += Vector4.Half;

            R = (ushort)scaledVector.X;
            G = (ushort)scaledVector.Y;
            B = (ushort)scaledVector.Z;
            A = (ushort)scaledVector.W;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(R, G, B, A) / ushort.MaxValue;
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);
            A = byte.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            R = G = B = source.L;
            A = byte.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);
            A = source.A;
        }

        public void FromRgb24(Rgb24 source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
            A = byte.MaxValue;
        }

        public void FromRgba32(Color source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
            A = PackedVectorHelper.UpScale8To16Bit(source.A);
        }

        public void FromRgb48(Rgb48 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = byte.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            this = source;
        }

        public readonly Color ToColor()
        {
            return new Color(
                PackedVectorHelper.DownScale16To8Bit(R),
                PackedVectorHelper.DownScale16To8Bit(G),
                PackedVectorHelper.DownScale16To8Bit(B),
                PackedVectorHelper.DownScale16To8Bit(A));
        }

        #endregion

        #region Equals

        public readonly bool Equals(Rgba64 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rgba64 other && Equals(other);
        }

        public static bool operator ==(in Rgba64 a, in Rgba64 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Rgba64 a, in Rgba64 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rgba64) + $"(R:{R}, G:{G}, B:{B}, A:{A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}