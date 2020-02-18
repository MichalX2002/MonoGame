// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four unsigned 8-bit XYZ components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgb48 : IEquatable<Rgb48>, IPixel
    {
        private static readonly Vector3 Max = new Vector3(ushort.MaxValue);

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

        public void FromVector4(Vector4 vector)
        {
            ref Vector3 vector3 = ref Unsafe.As<Vector4, Vector3>(ref vector);
            vector3 *= ushort.MaxValue;
            vector3 += Vector3.Half;
            vector3 = Vector3.Clamp(vector3, Vector3.Zero, Max);

            R = (ushort)vector.X;
            G = (ushort)vector.Y;
            B = (ushort)vector.Z;
        }

        public readonly Vector4 ToVector4() => new Vector4(ToVector3(), 1);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

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