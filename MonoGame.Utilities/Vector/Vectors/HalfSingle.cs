// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing a 16-bit floating-point X component.
    /// <para>Ranges from [-1, 0, 0, 1] to [1, 0, 0, 1] in vector form.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HalfSingle : IPackedPixel<HalfSingle, ushort>, IPixel
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Red));

        [CLSCompliant(false)]
        public ushort X;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public HalfSingle(ushort x)
        {
            X = x;
        }

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        public HalfSingle(float single) : this(HalfTypeHelper.Pack(single))
        {
        }

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public readonly float ToSingle()
        {
            return HalfTypeHelper.Unpack(PackedValue);
        }

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public readonly float ToScaledSingle()
        {
            return (ToSingle() + 1) / 2f;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { readonly get => X; set => X = value; }

        public void FromScaledVector(Vector4 vector)
        {
            float scaled = vector.X;
            scaled *= 2;
            scaled -= 1;

            PackedValue = HalfTypeHelper.Pack(scaled);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToScaledSingle(), 0, 0, 1);
        }

        public void FromVector(Vector4 vector)
        {
            X = HalfTypeHelper.Pack(vector.X);
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(ToSingle(), 0, 0, 1);
        }

        #endregion

        #region IPixel

        public readonly Color ToColor()
        {
            return new Color(ToScaledSingle(), 0, 0, 1);
        }

        #endregion

        public static implicit operator float(HalfSingle value)
        {
            return value.ToSingle();
        }

        #region Equals

        public readonly bool Equals(HalfSingle other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is HalfSingle other && Equals(other);
        }

        public static bool operator ==(HalfSingle a, HalfSingle b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(HalfSingle a, HalfSingle b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => ToSingle().ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
