// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
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

        #endregion

        public readonly Vector3 ToVector3()
        {
            return new Vector3(R, G, B) / ushort.MaxValue;
        }

        #region IPackedVector

        public void FromScaledVector4(Vector4 scaledVector)
        {
            var vector = scaledVector.ToVector3();
            vector *= ushort.MaxValue;
            vector += Vector3.Half;
            vector.Clamp(0, ushort.MaxValue);

            R = (ushort)vector.X;
            G = (ushort)vector.Y;
            B = (ushort)vector.Z;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToVector3(), 1);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);
        }

        public void FromGray16(Gray16 source)
        {
            R = G = B = source.L;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);
        }

        public void FromRgb24(Rgb24 source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
        }

        public void FromRgba32(Color source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
        }

        public void FromRgb48(Rgb48 source)
        {
            this = source;
        }

        public void FromRgba64(Rgba64 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public readonly Color ToColor()
        {
            return new Color(
                PackedVectorHelper.DownScale16To8Bit(R),
                PackedVectorHelper.DownScale16To8Bit(G),
                PackedVectorHelper.DownScale16To8Bit(B),
                byte.MaxValue);
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

        #region Object Overrides

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