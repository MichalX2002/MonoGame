using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
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

        public Vector4 Base;

        public float R { readonly get => Base.X; set => Base.X = value; }
        public float G { readonly get => Base.Y; set => Base.Y = value; }
        public float B { readonly get => Base.Z; set => Base.Z = value; }
        public float A { readonly get => Base.W; set => Base.W = value; }

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a vector.
        /// </summary>
        public RgbaVector(Vector4 vector)
        {
            Base = vector;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public RgbaVector(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        #region IPackedVector

        public void FromScaledVector(Vector3 scaledVector)
        {
            Base = scaledVector.ToVector4();
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            Base = scaledVector;
        }

        public readonly Vector3 ToScaledVector3()
        {
            return Base.ToVector3();
        }

        public readonly Vector4 ToScaledVector4()
        {
            return Base;
        }

        #endregion

        #region Equals

        public readonly bool Equals(RgbaVector other)
        {
            return Base.Equals(other.Base);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is RgbaVector other && Equals(other);
        }

        public static bool operator ==(in RgbaVector a, in RgbaVector b)
        {
            return a.Base == b.Base;
        }

        public static bool operator !=(in RgbaVector a, in RgbaVector b)
        {
            return a.Base != b.Base;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(RgbaVector) + $"({Base})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => Base.GetHashCode();

        #endregion

        public static implicit operator RgbaVector(in Vector4 vector)
        {
            return UnsafeR.As<Vector4, RgbaVector>(vector);
        }

        public static implicit operator Vector4(in RgbaVector vector)
        {
            return UnsafeR.As<RgbaVector, Vector4>(vector);
        }
    }
}
