// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZW components.
    /// The XYZ components use 10 bits each, and the W component uses 2 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgba1010102 : IPackedPixel<Rgba1010102, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 2));

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Rgba1010102(uint packed)
        {
            PackedValue = packed;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Rgba1010102(in Vector4 vector) : this()
        {
            FromVector4(vector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Rgba1010102(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        public void FromVector4(in Vector4 vector)
        {
            var v = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            v *= 1023f;

            PackedValue = (uint)(
                (((int)MathF.Round(v.X) & 0x03FF) << 0) |
                (((int)MathF.Round(v.Y) & 0x03FF) << 10) |
                (((int)MathF.Round(v.Z) & 0x03FF) << 20) |
                (((int)MathF.Round(v.W / 1023f * 3f) & 0x03) << 30));
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = (PackedValue >> 0) & 0x03FF;
            vector.Base.Y = (PackedValue >> 10) & 0x03FF;
            vector.Base.Z = (PackedValue >> 20) & 0x03FF;
            vector.Base.W = ((PackedValue >> 30) & 0x03) * 1023f / 3f;
            vector /= 1023;
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

        public void FromScaledVector4(in Vector4 vector)
        {
            FromVector4(vector);
        }

        #endregion

        #region Equals

        public static bool operator ==(Rgba1010102 a, Rgba1010102 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Rgba1010102 a, Rgba1010102 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public bool Equals(Rgba1010102 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Rgba1010102 other && Equals(other);
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Rgba1010102) + $"({this.ToVector4()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
