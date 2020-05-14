using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing four 32-bit floating-point XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbaVector : IPixel<RgbaVector>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Alpha));

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

        #endregion

        #region IPackedVector

        public void FromScaledVector4(Vector4 scaledVector)
        {
            Unsafe.As<RgbaVector, Vector4>(ref this) = scaledVector;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return UnsafeR.As<RgbaVector, Vector4>(this);
        }

        #endregion

        #region Equals

        public readonly bool Equals(RgbaVector other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is RgbaVector other && Equals(other);
        }

        public static bool operator ==(in RgbaVector a, in RgbaVector b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        }

        public static bool operator !=(in RgbaVector a, in RgbaVector b)
        {
            return !(a == b);
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(RgbaVector) + $"({ToScaledVector4()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(R, G, B, A);

        #endregion
    }
}
