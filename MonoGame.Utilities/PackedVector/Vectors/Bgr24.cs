// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing three unsigned 8-bit XYZ components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr24 : IEquatable<Bgr24>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Blue, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Red, sizeof(byte) * 8));

        [CLSCompliant(false)]
        public byte R;

        [CLSCompliant(false)]
        public byte G;

        [CLSCompliant(false)]
        public byte B;

        #region Constructors

        [CLSCompliant(false)]
        public Bgr24(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Bgr24(Vector3 vector) : this()
        {
            vector.ToVector4(out var v);
            FromScaledVector4(v);
        }

        public Bgr24(float x, float y, float z) : this(new Vector3(x, y, z))
        {
        }

        #endregion

        public readonly Vector3 ToVector3() => new Vector3(R, G, B) / byte.MaxValue;

        #region IPackedVector

        public void FromVector4(in Vector4 vector) => FromScaledVector4(vector);

        public readonly void ToVector4(out Vector4 vector) => ToScaledVector4(out vector);

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            Rgb24 rgb = default;
            rgb.FromScaledVector4(scaledVector);
            FromRgb24(rgb);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector = new Vector4(R, G, B, byte.MaxValue) / byte.MaxValue;
        }

        #endregion

        #region IPixel

        public readonly void ToColor(ref Color destination)
        {
            destination.R = R;
            destination.G = G;
            destination.B = B;
            destination.A = byte.MaxValue;
        }

        public void FromGray8(Gray8 source) => B = G = R = source.L;

        public void FromGray16(Gray16 source) => B = G = R = PackedVectorHelper.DownScale16To8Bit(source.L);

        public void FromGrayAlpha16(GrayAlpha16 source) => B = G = R = source.L;

        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromColor(Color source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromRgb48(Rgb48 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
        }

        public void FromRgba64(Rgba64 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
        }

        #endregion

        #region Equals

        public override bool Equals(object obj) => obj is Bgr24 other && Equals(other);
        public bool Equals(Bgr24 other) => this == other;

        public static bool operator ==(in Bgr24 a, in Bgr24 b) =>
            a.R == b.R && a.G == b.G && a.B == b.B;

        public static bool operator !=(in Bgr24 a, in Bgr24 b) => !(a == b);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Bgr24) + $"(R:{R}, G:{G}, B:{B})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(R, G, B);

        #endregion
    }
}