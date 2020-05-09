// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZW components.
    /// The XYZ components use 5 bits each, and the W component uses 1 bit.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra5551 : IPackedPixel<Bgra5551, ushort>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 5),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 5),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 5),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 1));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra5551(ushort packed)
        {
            PackedValue = packed;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Bgra5551(in Vector4 vector) : this()
        {
            FromVector4(vector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgra5551(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromVector4(in Vector4 vector)
        {
            var v = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            v *= 31f;

            PackedValue = (ushort)(
                (((int)MathF.Round(v.X) & 0x1F) << 10) |
                (((int)MathF.Round(v.Y) & 0x1F) << 5) |
                (((int)MathF.Round(v.Z) & 0x1F) << 0) |
                (((int)MathF.Round(v.W / 31f) & 0x1) << 15));
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = (PackedValue >> 10) & 0x1F;
            vector.Base.Y = (PackedValue >> 5) & 0x1F;
            vector.Base.Z = (PackedValue >> 0) & 0x1F;
            vector.Base.W = ((PackedValue >> 15) & 0x01) * 31f;
            vector /= 31f;
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

        #region Equals

        public static bool operator ==(Bgra5551 a, Bgra5551 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Bgra5551 a, Bgra5551 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public bool Equals(Bgra5551 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Bgra5551 other && Equals(other);
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Bgra5551) + $"({this.ToVector4()}";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}