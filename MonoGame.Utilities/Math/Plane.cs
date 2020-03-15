// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Plane : IEquatable<Plane>
    {
        [DataMember]
        public Vector3 Normal;

        [DataMember]
        public float D;

        public Plane(in Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        public Plane(in Vector4 value) : this(new Vector3(value.X, value.Y, value.Z), value.W)
        {
        }

        public Plane(in Vector3 a, in Vector3 b, in Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;

            Vector3.Cross(ab, ac, out var cross);
            Vector3.Normalize(cross, out Normal);
            D = -Vector3.Dot(Normal, a);
        }

        public Plane(float a, float b, float c, float d) : this(new Vector3(a, b, c), d)
        {
        }

        public readonly Vector4 ToVector4()
        {
            return UnsafeUtils.As<Plane, Vector4>(this);
        }
        
        public readonly float Dot(in Vector4 value)
        {
            return Vector4.Dot(ToVector4(), value);
        }

        public readonly float DotNormal(in Vector3 value)
        {
            return Vector3.Dot(Normal, value);
        }

        public readonly float DotCoordinate(in Vector3 value)
        {
            return DotNormal(value) + D;
        }

        /// <summary>
        /// Transforms a normalized plane by a matrix.
        /// </summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <param name="result">The transformed plane.</param>
        public static void Transform(in Plane plane, in Matrix matrix, out Plane result)
        {
            // See "Transforming Normals" in http://www.glprogramming.com/red/appendixf.html
            // for an explanation of how this works.

            Matrix.Invert(matrix, out var transformedMatrix);
            Matrix.Transpose(transformedMatrix, out transformedMatrix);

            Vector4.Transform(plane.ToVector4(), transformedMatrix, out var transformedVector);
            result = Unsafe.As<Vector4, Plane>(ref transformedVector);
        }

        /// <summary>
        /// Transforms a normalized plane by a matrix.
        /// </summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed plane.</returns>
        public static Plane Transform(in Plane plane, in Matrix matrix)
        {
            Transform(plane, matrix, out var result);
            return result;
        }

        /// <summary>
        /// Transforms a normalized plane by a quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <returns>The transformed plane.</returns>
        public static Plane Transform(in Plane plane, in Quaternion rotation)
        {
            return new Plane(Vector3.Transform(plane.Normal, rotation), plane.D);
        }

        #region Normalize

        public static void Normalize(in Plane value, out Plane result)
        {
            float length = value.Normal.Length();

            Vector3.Divide(value.Normal, length, out result.Normal);
            result.D = value.D / length;
        }

        public static Plane Normalize(in Plane value)
        {
            Normalize(value, out var result);
            return result;
        }

        public void Normalize()
        {
            Normalize(this, out this);
        }

        #endregion

        public static bool operator !=(in Plane plane1, in Plane plane2)
        {
            return !plane1.Equals(plane2);
        }

        public static bool operator ==(in Plane plane1, in Plane plane2)
        {
            return plane1.Normal == plane2.Normal
                && plane1.D == plane2.D;
        }

        public override bool Equals(object obj)
        {
            return (obj is Plane other) ? Equals(other) : false;
        }

        public bool Equals(Plane other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int code = 7 + Normal.GetHashCode();
                return code * 31 + D.GetHashCode();
            }
        }

        internal PlaneIntersectionType Intersects(Vector3 point)
        {
            float distance = DotCoordinate(point);
            if (distance > 0)
                return PlaneIntersectionType.Front;
            if (distance < 0)
                return PlaneIntersectionType.Back;
            return PlaneIntersectionType.Intersecting;
        }

        internal string DebuggerDisplay => string.Concat(Normal.DebuggerDisplay, "  ", D.ToString());

        public override string ToString()
        {
            return "{Normal:" + Normal + " D:" + D + "}";
        }

        /// <summary>
        /// Deconstruction method for <see cref="Plane"/>.
        /// </summary>
        public readonly void Deconstruct(out Vector3 normal, out float d)
        {
            normal = Normal;
            d = D;
        }

        /// <summary>
        /// Returns a value indicating what side (positive/negative) of a plane a point is
        /// </summary>
        /// <param name="point">The point to check with</param>
        /// <param name="plane">The plane to check against</param>
        /// <returns>Greater than zero if on the positive side, less than zero if on the negative size, 0 otherwise</returns>
        public static float ClassifyPoint(in Vector3 point, in Plane plane)
        {
            return Vector3.Dot(point, plane.Normal) + plane.D;
        }

        ///// <summary>
        ///// Returns the perpendicular distance from a point to a plane
        ///// </summary>
        ///// <param name="point">The point to check</param>
        ///// <param name="plane">The place to check</param>
        ///// <returns>The perpendicular distance from the point to the plane</returns>
        //public static float PerpendicularDistance(ref Vector3 point, ref Plane plane)
        //{
        //    // dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
        //    return (float)Math.Abs((plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z)
        //                            / Math.Sqrt(plane.Normal.X * plane.Normal.X + plane.Normal.Y * plane.Normal.Y + plane.Normal.Z * plane.Normal.Z));
        //}
    }
}