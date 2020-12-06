// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public const int CornerCount = 8;

        [DataMember]
        public Vector3 Min;

        [DataMember]
        public Vector3 Max;

        internal string DebuggerDisplay => string.Concat(
            "Min = ", Min.ToString(), ", ",
            "Max = ", Max.ToString());

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        #region Public Methods

        public readonly ContainmentType Contains(in BoundingBox box)
        {
            //test if all corner is in the same side of a face by just checking min and max
            if (box.Max.X < Min.X || box.Min.X > Max.X ||
                box.Max.Y < Min.Y || box.Min.Y > Max.Y ||
                box.Max.Z < Min.Z || box.Min.Z > Max.Z)
                return ContainmentType.Disjoint;

            if (box.Min.X >= Min.X && box.Max.X <= Max.X &&
                box.Min.Y >= Min.Y && box.Max.Y <= Max.Y &&
                box.Min.Z >= Min.Z && box.Max.Z <= Max.Z)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }

        public readonly ContainmentType Contains(BoundingFrustum frustum)
        {
            if (frustum == null)
                throw new ArgumentNullException(nameof(frustum));

            //TODO: bad done here need a fix. 
            // Because question is not frustum contain box but reverse and this is not the same
            int i;
            var corners = frustum.Corners;

            // First we check if frustum is in box
            for (i = 0; i < corners.Length; i++)
            {
                if (Contains(corners[i]) == ContainmentType.Disjoint)
                    break;
            }

            if (i == corners.Length) // This means we checked all the corners and they were all contain or instersect
                return ContainmentType.Contains;

            if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
                return ContainmentType.Intersects;

            // If we get here, it means the first (and only) point we checked was actually contained in the frustum.
            // So we assume that all other points will also be contained. If one of the points is disjoint, we can
            // exit immediately saying that the result is Intersects
            i++;
            for (; i < corners.Length; i++)
            {
                if (Contains(corners[i]) != ContainmentType.Contains)
                    return ContainmentType.Intersects;
            }

            // If we get here, then we know all the points were actually contained, therefore result is Contains
            return ContainmentType.Contains;
        }

        public readonly ContainmentType Contains(in BoundingSphere sphere)
        {
            var centerMin = sphere.Center - Min;
            var maxCenter = Max - sphere.Center;

            if (centerMin.X >= sphere.Radius &&
                centerMin.Y >= sphere.Radius &&
                centerMin.Z >= sphere.Radius &&
                maxCenter.X >= sphere.Radius &&
                maxCenter.Y >= sphere.Radius &&
                maxCenter.Z >= sphere.Radius)
                return ContainmentType.Contains;

            var centerMax = sphere.Center - Max;
            double dmin = 0;
            double e = centerMin.X;

            if (e < 0)
            {
                if (e < -sphere.Radius)
                    return ContainmentType.Disjoint;
                dmin += e * e;
            }
            else
            {
                e = centerMax.X;
                if (e > 0)
                {
                    if (e > sphere.Radius)
                        return ContainmentType.Disjoint;
                    dmin += e * e;
                }
            }

            e = centerMin.Y;
            if (e < 0)
            {
                if (e < -sphere.Radius)
                    return ContainmentType.Disjoint;
                dmin += e * e;
            }
            else
            {
                e = centerMax.Y;
                if (e > 0)
                {
                    if (e > sphere.Radius)
                        return ContainmentType.Disjoint;
                    dmin += e * e;
                }
            }

            e = centerMin.Z;
            if (e < 0)
            {
                if (e < -sphere.Radius)
                    return ContainmentType.Disjoint;
                dmin += e * e;
            }
            else
            {
                e = centerMax.Z;
                if (e > 0)
                {
                    if (e > sphere.Radius)
                        return ContainmentType.Disjoint;
                    dmin += e * e;
                }
            }

            if (dmin <= sphere.Radius * sphere.Radius)
                return ContainmentType.Intersects;
            return ContainmentType.Disjoint;
        }

        public readonly ContainmentType Contains(Vector3 point)
        {
            //first we get if point is out of box
            if (point.X < Min.X || point.X > Max.X ||
                point.Y < Min.Y || point.Y > Max.Y ||
                point.Z < Min.Z || point.Z > Max.Z)
                return ContainmentType.Disjoint;

            return ContainmentType.Contains;
        }

        /// <summary>
        /// Create a bounding box from the given enumerable of points.
        /// </summary>
        /// <param name="points">The enumerable of vectors defining the point cloud to bound.</param>
        /// <returns>A bounding box that encapsulates the given point cloud.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static BoundingBox CreateFromPoints<TEnumerator>(TEnumerator points)
            where TEnumerator : IEnumerator<Vector3>
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            var minVec = new Vector3(float.MaxValue);
            var maxVec = new Vector3(float.MinValue);
            while (points.MoveNext())
            {
                var point = points.Current;
                minVec = Vector3.Min(minVec, point);
                maxVec = Vector3.Max(maxVec, point);
            }
            return new BoundingBox(minVec, maxVec);
        }

        /// <summary>
        /// Create a bounding box from the given span of points.
        /// </summary>
        /// <param name="points">The span of vectors defining the point cloud to bound.</param>
        /// <returns>A bounding box that encapsulates the given point cloud.</returns>
        /// <exception cref="ArgumentEmptyException"></exception>
        public static BoundingBox CreateFromPoints(ReadOnlySpan<Vector3> points)
        {
            var minVec = new Vector3(float.MaxValue);
            var maxVec = new Vector3(float.MinValue);
            foreach (var ptVector in points)
            {
                minVec = Vector3.Min(minVec, ptVector);
                maxVec = Vector3.Max(maxVec, ptVector);
            }
            return new BoundingBox(minVec, maxVec);
        }

        public static BoundingBox CreateFromSphere(in BoundingSphere sphere)
        {
            var corner = new Vector3(sphere.Radius);
            return new BoundingBox(sphere.Center - corner, sphere.Center + corner);
        }

        public static BoundingBox CreateMerged(in BoundingBox original, in BoundingBox additional)
        {
            var min = Vector3.Min(original.Min, additional.Min);
            var max = Vector3.Max(original.Min, additional.Min);
            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Creates an array of corners from this <see cref="BoundingBox"/>.
        /// </summary>
        [Obsolete("This method allocates a new array for every call.")]
        public readonly Vector3[] GetCorners()
        {
            var array = new Vector3[8];
            GetCorners(array);
            return array;
        }

        public readonly void GetCorners(Span<Vector3> corners)
        {
            if (corners.IsEmpty)
                throw new ArgumentEmptyException(nameof(corners));

            if (corners.Length < 8)
                throw new ArgumentException("Destination is too small.", nameof(corners));

            corners[0] = new Vector3(Min.X, Max.Y, Max.Z);
            corners[1] = new Vector3(Max.X, Max.Y, Max.Z);
            corners[2] = new Vector3(Max.X, Min.Y, Max.Z);
            corners[3] = new Vector3(Min.X, Min.Y, Max.Z);
            corners[4] = new Vector3(Min.X, Max.Y, Min.Z);
            corners[5] = new Vector3(Max.X, Max.Y, Min.Z);
            corners[6] = new Vector3(Max.X, Min.Y, Min.Z);
            corners[7] = new Vector3(Min.X, Min.Y, Min.Z);
        }

        public readonly bool Intersects(in BoundingBox box)
        {
            if ((Max.X >= box.Min.X) && (Min.X <= box.Max.X))
            {
                if ((Max.Y < box.Min.Y) || (Min.Y > box.Max.Y))
                    return false;

                return (Max.Z >= box.Min.Z) && (Min.Z <= box.Max.Z);
            }
            return false;
        }

        public readonly bool Intersects(BoundingFrustum frustum)
        {
            if (frustum == null)
                throw new ArgumentNullException(nameof(frustum));

            return frustum.Intersects(this);
        }

        /// <summary>
        /// Gets whether or not a specified sphere intersects with this box.
        /// </summary>
        /// <param name="box">The box for testing.</param>
        /// <returns>
        /// <see langword="true"/> if <see cref="BoundingBox"/> intersects with this sphere; 
        /// <see langword="false"/> otherwise.
        /// </returns>
        public readonly bool Intersects(in BoundingSphere sphere)
        {
            var centerMin = sphere.Center - Min;
            var maxCenter = Max - sphere.Center;

            if (centerMin.X > sphere.Radius &&
                centerMin.Y > sphere.Radius &&
                centerMin.Z > sphere.Radius &&
                maxCenter.X > sphere.Radius &&
                maxCenter.Y > sphere.Radius &&
                maxCenter.Z > sphere.Radius)
                return true;

            var centerMax = sphere.Center - Max;
            double dmin = 0;

            if (centerMin.X <= sphere.Radius)
                dmin += centerMin.X * centerMin.X;
            else if (maxCenter.X <= sphere.Radius)
                dmin += centerMax.X * centerMax.X;

            if (centerMin.Y <= sphere.Radius)
                dmin += centerMin.Y * centerMin.Y;
            else if (maxCenter.Y <= sphere.Radius)
                dmin += centerMax.Y * centerMax.Y;

            if (centerMin.Z <= sphere.Radius)
                dmin += centerMin.Z * centerMin.Z;
            else if (maxCenter.Z <= sphere.Radius)
                dmin += centerMax.Z * centerMax.Z;

            if (dmin <= sphere.Radius * sphere.Radius)
                return true;

            return false;
        }

        public readonly PlaneIntersectionType Intersects(Plane plane)
        {
            // See http://zach.in.tu-clausthal.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html

            Vector3 positiveVertex;
            Vector3 negativeVertex;

            if (plane.Normal.X >= 0)
            {
                positiveVertex.X = Max.X;
                negativeVertex.X = Min.X;
            }
            else
            {
                positiveVertex.X = Min.X;
                negativeVertex.X = Max.X;
            }

            if (plane.Normal.Y >= 0)
            {
                positiveVertex.Y = Max.Y;
                negativeVertex.Y = Min.Y;
            }
            else
            {
                positiveVertex.Y = Min.Y;
                negativeVertex.Y = Max.Y;
            }

            if (plane.Normal.Z >= 0)
            {
                positiveVertex.Z = Max.Z;
                negativeVertex.Z = Min.Z;
            }
            else
            {
                positiveVertex.Z = Min.Z;
                negativeVertex.Z = Max.Z;
            }

            float distance = Vector3.Dot(plane.Normal, negativeVertex) + plane.D;
            if (distance > 0)
                return PlaneIntersectionType.Front;

            distance = Vector3.Dot(plane.Normal, positiveVertex) + plane.D;
            if (distance < 0)
                return PlaneIntersectionType.Back;

            return PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Deconstruction method for <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public readonly void Deconstruct(out Vector3 min, out Vector3 max)
        {
            min = Min;
            max = Max;
        }

        #endregion

        #region Equals

        public readonly bool Equals(BoundingBox other)
        {
            return this == other;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is BoundingBox other && this == other;
        }

        public static bool operator ==(in BoundingBox a, in BoundingBox b)
        {
            return (a.Min == b.Min) && (a.Max == b.Max);
        }

        public static bool operator !=(in BoundingBox a, in BoundingBox b)
        {
            return !(a == b);
        }

        #endregion

        #region Object overrides

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public override readonly string ToString()
        {
            return "{Min:" + Min.ToString() + " Max:" + Max.ToString() + "}";
        }

        #endregion
    }
}