// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing three unsigned 16-bit XYZ components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgb48 : IEquatable<Rgb48>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(ushort) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(ushort) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(ushort) * 8));

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

        public Rgb48(Vector3 vector) : this()
        {
            FromVector4(new Vector4(vector, 1));
        }

        public Rgb48(float x, float y, float z) : this(new Vector3(x, y, z))
        {
        }

        #endregion

        public readonly Vector3 ToVector3() => new Vector3(R, G, B) / ushort.MaxValue;

        #region IPackedVector

        public void FromVector4(in Vector4 vector)
        {
            var v = vector.ToVector3() * ushort.MaxValue;
            v += Vector3.Half;
            v.Clamp(0, ushort.MaxValue);

            R = (ushort)vector.X;
            G = (ushort)vector.Y;
            B = (ushort)vector.Z;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = R;
            vector.Base.Y = G;
            vector.Base.Z = B;
            vector.Base.W = ushort.MaxValue;
            vector /= ushort.MaxValue;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            FromVector4(scaledVector);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

        #endregion

        #region IPixel

        public readonly void ToColor(ref Color destination)
        {
            destination.R = PackedVectorHelper.DownScale16To8Bit(R);
            destination.G = PackedVectorHelper.DownScale16To8Bit(G);
            destination.B = PackedVectorHelper.DownScale16To8Bit(B);
            destination.A = byte.MaxValue;
        }

        public void FromGray8(Gray8 source) => R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);

        public void FromGray16(Gray16 source) => R = G = B = source.L;

        public void FromGrayAlpha16(GrayAlpha16 source) => R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);

        public void FromRgb24(Rgb24 source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
        }

        public void FromColor(Color source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
        }

        public void FromRgb48(Rgb48 source) => this = source;

        public void FromRgba64(Rgba64 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        #endregion

        #region Equals

        public override bool Equals(object obj) => obj is Rgb48 other && Equals(other);
        public bool Equals(Rgb48 other) => this == other;

        public static bool operator ==(in Rgb48 a, in Rgb48 b) =>
            a.R == b.R && a.G == b.G && a.B == b.B;

        public static bool operator !=(in Rgb48 a, in Rgb48 b) => !(a == b);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Rgb48) + $"(R:{R}, G:{G}, B:{B})";

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