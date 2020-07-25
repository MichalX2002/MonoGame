using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing four 32-bit floating-point XYZ components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbVector : IPixel<RgbVector>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Blue));

        public Vector3 Base;

        public float R { readonly get => Base.X; set => Base.X = value; }
        public float G { readonly get => Base.Y; set => Base.Y = value; }
        public float B { readonly get => Base.Z; set => Base.Z = value; }

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a vector.
        /// </summary>
        public RgbVector(Vector3 vector)
        {
            Base = vector;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public RgbVector(float r, float g, float b) : this(new Vector3(r, g, b))
        {
        }

        #endregion

        #region IPackedVector

        public void FromScaledVector(Vector3 scaledVector)
        {
            Base = scaledVector;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            Base = scaledVector.ToVector3();
        }

        public readonly Vector3 ToScaledVector3()
        {
            return Base;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return Base.ToVector4();
        }

        #endregion

        #region Equals

        public readonly bool Equals(RgbVector other)
        {
            return Base.Equals(other.Base);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is RgbVector other && Equals(other);
        }

        public static bool operator ==(in RgbVector a, in RgbVector b)
        {
            return a.Base == b.Base;
        }

        public static bool operator !=(in RgbVector a, in RgbVector b)
        {
            return a.Base != b.Base;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(RgbVector) + $"({Base})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => Base.GetHashCode();

        #endregion

        public static implicit operator RgbVector(in Vector3 vector)
        {
            return UnsafeR.As<Vector3, RgbVector>(vector);
        }

        public static implicit operator Vector3(in RgbVector vector)
        {
            return UnsafeR.As<RgbVector, Vector3>(vector);
        }
    }
}
