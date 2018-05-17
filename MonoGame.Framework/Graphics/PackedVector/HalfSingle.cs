// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    public struct HalfSingle : IPackedVector<UInt16>, IEquatable<HalfSingle>, IPackedVector
    {
        public HalfSingle(float single)
        {
            PackedValue = HalfTypeHelper.Convert(single);
        }

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public float ToSingle()
        {
            return HalfTypeHelper.Convert(this.PackedValue);
        }

        void IPackedVector.PackFromVector4(Vector4 vector)
        {
            this.PackedValue = HalfTypeHelper.Convert(vector.X);
        }

        Vector4 IPackedVector.ToVector4()
        {
            return new Vector4(this.ToSingle(), 0f, 0f, 1f);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == this.GetType())
            {
                return this == (HalfSingle)obj;
            }

            return false;
        }

        public bool Equals(HalfSingle other)
        {
            return this.PackedValue == other.PackedValue;
        }

        public override string ToString()
        {
            return this.ToSingle().ToString();
        }

        public override int GetHashCode()
        {
            return this.PackedValue.GetHashCode();
        }

        public static bool operator ==(HalfSingle lhs, HalfSingle rhs)
        {
            return lhs.PackedValue == rhs.PackedValue;
        }

        public static bool operator !=(HalfSingle lhs, HalfSingle rhs)
        {
            return lhs.PackedValue != rhs.PackedValue;
        }
    }
}
