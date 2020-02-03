using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four 32-bit floating-point XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbaVector : IPixel, IEquatable<RgbaVector>
    {
        public int BitDepth => Unsafe.SizeOf<RgbaVector>() * 8;

        public float R;
        public float G;
        public float B;
        public float A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public RgbaVector(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public RgbaVector(Vector4 vector) : this(vector.X, vector.Y, vector.Z, vector.W)
        {
        }

        #endregion

        #region IPackedVector

        public void FromVector4(Vector4 vector) => this = Unsafe.As<Vector4, RgbaVector>(ref vector);

        public readonly Vector4 ToVector4() => UnsafeUtils.As<RgbaVector, Vector4>(this);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public readonly void ToColor(ref Color destination) => destination.FromScaledVector4(ToScaledVector4());

        public void FromGray8(Gray8 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGray16(Gray16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGrayAlpha16(GrayAlpha88 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb24(Rgb24 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromColor(Color source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        #endregion

        #region Equals

        public static bool operator ==(in RgbaVector a, in RgbaVector b) => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        public static bool operator !=(in RgbaVector a, in RgbaVector b) => !(a == b);

        public bool Equals(RgbaVector other) => this == other;
        public override bool Equals(object obj) => obj is RgbaVector other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(RgbaVector) + $"(R:{R}, G:{G}, B:{B}, A:{A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => ToVector4().GetHashCode();

        #endregion
    }
}
