// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing four unsigned 8-bit XYZ components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgb24 : IEquatable<Rgb24>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(byte) * 8));

        [CLSCompliant(false)]
        public byte R;

        [CLSCompliant(false)]
        public byte G;

        [CLSCompliant(false)]
        public byte B;

        #region Constructors

        [CLSCompliant(false)]
        public Rgb24(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Rgb24(Vector3 vector) : this()
        {
            FromVector4(new Vector4(vector, 1));
        }

        public Rgb24(float x, float y, float z) : this(new Vector3(x, y, z))
        {
        }

        #endregion

        public readonly Vector3 ToVector3()
        {
            return new Vector3(R, G, B) / byte.MaxValue;
        }

        #region IPackedVector

        public void FromVector4(in Vector4 vector)
        {
            var v = vector.ToVector3()* byte.MaxValue;
            v += Vector3.Half;
            v.Clamp(0, byte.MaxValue);

            R = (byte)v.X;
            G = (byte)v.Y;
            B = (byte)v.Z;
        }

        public readonly void ToVector4(out Vector4 scaledVector)
        {
            scaledVector.Base.X = R;
            scaledVector.Base.Y = G;
            scaledVector.Base.Z = B;
            scaledVector.Base.W = byte.MaxValue;
            scaledVector /= byte.MaxValue;
        }

        public void FromScaledVector4(in Vector4 scaledVector) => FromVector4(scaledVector);

        public readonly void ToScaledVector4(out Vector4 scaledVector) => ToVector4(out scaledVector);

        #endregion

        #region IPixel

        public readonly void ToColor(ref Color destination)
        {
            destination.R = R;
            destination.G = G;
            destination.B = B;
            destination.A = byte.MaxValue;
        }

        public void FromGray8(Gray8 source) => R = G = B = source.L;

        public void FromGray16(Gray16 source) => R = G = B = PackedVectorHelper.DownScale16To8Bit(source.L);

        public void FromGrayAlpha16(GrayAlpha16 source) => R = G = B = source.L;

        public void FromRgb24(Rgb24 source) => this = source;

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

        public override bool Equals(object obj) => obj is Rgb24 other && Equals(other);
        public bool Equals(Rgb24 other) => this == other;

        public static bool operator ==(in Rgb24 a, in Rgb24 b) =>
            a.R == b.R && a.G == b.G && a.B == b.B;

        public static bool operator !=(in Rgb24 a, in Rgb24 b) => !(a == b);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => $"(R:{R}, G:{G}, B:{B})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int code = 17;
                code = code * 23 + R;
                code = code * 23 + G;
                return code * 23 + B;
            }
        }

        #endregion
    }
}