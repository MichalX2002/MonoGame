using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing four 32-bit floating-point XY components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgVector : IPixel<RgVector>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Green));

        public Vector2 Base;

        public float R { readonly get => Base.X; set => Base.X = value; }
        public float G { readonly get => Base.Y; set => Base.Y = value; }

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a vector.
        /// </summary>
        public RgVector(Vector2 vector)
        {
            Base = vector;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public RgVector(float r, float g) : this(new Vector2(r, g))
        {
        }

        #endregion

        #region IPackedVector

        public void FromScaledVector(Vector3 scaledVector)
        {
            Base = scaledVector.ToVector2();
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            Base = scaledVector.ToVector2();
        }

        public readonly Vector3 ToScaledVector3()
        {
            return Base.ToVector3();
        }

        public readonly Vector4 ToScaledVector4()
        {
            return Base.ToVector4();
        }

        #endregion

        #region Equals

        public readonly bool Equals(RgVector other)
        {
            return Base.Equals(other.Base);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is RgVector other && Equals(other);
        }

        public static bool operator ==(in RgVector a, in RgVector b)
        {
            return a.Base == b.Base;
        }

        public static bool operator !=(in RgVector a, in RgVector b)
        {
            return a.Base != b.Base;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(RgVector) + $"({Base})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => Base.GetHashCode();

        #endregion

        public static implicit operator RgVector(in Vector2 vector)
        {
            return UnsafeR.As<Vector2, RgVector>(vector);
        }

        public static implicit operator Vector2(in RgVector vector)
        {
            return UnsafeR.As<RgVector, Vector2>(vector);
        }
    }
}
