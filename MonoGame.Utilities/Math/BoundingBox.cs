// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private string DebuggerDisplay => string.Concat(
            "Min(", Min.ToString(), " \n",
            "Max(", Max.ToString());

        public BoundingBox(in Vector3 min, in Vector3 max)
        {
            Min = min;
            Max = max;
        }

        #region Public Methods

        public ContainmentType Contains(in BoundingBox box)
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

        public ContainmentType Contains(BoundingFrustum frustum)
        {
            //TODO: bad done here need a fix. 
            // Because question is not frustum contain box but reverse and this is not the same
            int i;
            var corners = frustum.GetCorners();

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

        public ContainmentType Contains(in BoundingSphere sphere)
        {
            if (sphere.Center.X - Min.X >= sphere.Radius &&
                sphere.Center.Y - Min.Y >= sphere.Radius &&
                sphere.Center.Z - Min.Z >= sphere.Radius &&
                Max.X - sphere.Center.X >= sphere.Radius &&
                Max.Y - sphere.Center.Y >= sphere.Radius &&
                Max.Z - sphere.Center.Z >= sphere.Radius)
                return ContainmentType.Contains;

            double dmin = 0;

            double e = sphere.Center.X - Min.X;
            if (e < 0)
            {
                if (e < -sphere.Radius)
                    return ContainmentType.Disjoint;
                dmin += e * e;
            }
            else
            {
                e = sphere.Center.X - Max.X;
                if (e > 0)
                {
                    if (e > sphere.Radius)
                        return ContainmentType.Disjoint;
                    dmin += e * e;
                }
            }

            e = sphere.Center.Y - Min.Y;
            if (e < 0)
            {
                if (e < -sphere.Radius)
                    return ContainmentType.Disjoint;
                dmin += e * e;
            }
            else
            {
                e = sphere.Center.Y - Max.Y;
                if (e > 0)
                {
                    if (e > sphere.Radius)
                        return ContainmentType.Disjoint;
                    dmin += e * e;
                }
            }

            e = sphere.Center.Z - Min.Z;
            if (e < 0)
            {
                if (e < -sphere.Radius)
                    return ContainmentType.Disjoint;
                dmin += e * e;
            }
            else
            {
                e = sphere.Center.Z - Max.Z;
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

        public ContainmentType Contains(in Vector3 point)
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
        /// <exception cref="ArgumentEmptyException"></exception>
        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            bool empty = true;
            var minVec = Vector3.MaxValue;
            var maxVec = Vector3.MinValue;
            foreach (var ptVector in points)
            {
                minVec.X = (minVec.X < ptVector.X) ? minVec.X : ptVector.X;
                minVec.Y = (minVec.Y < ptVector.Y) ? minVec.Y : ptVector.Y;
                minVec.Z = (minVec.Z < ptVector.Z) ? minVec.Z : ptVector.Z;

                maxVec.X = (maxVec.X > ptVector.X) ? maxVec.X : ptVector.X;
                maxVec.Y = (maxVec.Y > ptVector.Y) ? maxVec.Y : ptVector.Y;
                maxVec.Z = (maxVec.Z > ptVector.Z) ? maxVec.Z : ptVector.Z;

                empty = false;
            }
            if (empty)
                throw new ArgumentEmptyException();

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
            if (points.IsEmpty)
                throw new ArgumentNullException();

            var minVec = Vector3.MaxValue;
            var maxVec = Vector3.MinValue;
            foreach (var ptVector in points)
            {
                minVec.X = (minVec.X < ptVector.X) ? minVec.X : ptVector.X;
                minVec.Y = (minVec.Y < ptVector.Y) ? minVec.Y : ptVector.Y;
                minVec.Z = (minVec.Z < ptVector.Z) ? minVec.Z : ptVector.Z;

                maxVec.X = (maxVec.X > ptVector.X) ? maxVec.X : ptVector.X;
                maxVec.Y = (maxVec.Y > ptVector.Y) ? maxVec.Y : ptVector.Y;
                maxVec.Z = (maxVec.Z > ptVector.Z) ? maxVec.Z : ptVector.Z;
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

        public bool Equals(BoundingBox other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is BoundingBox other ? this == other : false;
        }

        /// <summary>
        /// Creates an array copy of corners from this <see cref="BoundingBox"/>.
        /// </summary>
        public Vector3[] CloneCorners()
        {
            return new Vector3[]
            {
                new Vector3(Min.X, Max.Y, Max.Z),
                new Vector3(Max.X, Max.Y, Max.Z),
                new Vector3(Max.X, Min.Y, Max.Z),
                new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Min.Z),
                new Vector3(Max.X, Max.Y, Min.Z),
                new Vector3(Max.X, Min.Y, Min.Z),
                new Vector3(Min.X, Min.Y, Min.Z)
            };
        }

        public void GetCorners(Span<Vector3> corners)
        {
            if (corners.IsEmpty)
                throw new ArgumentException(nameof(corners));

            if (corners.Length < 8)
                throw new ArgumentException(nameof(corners), "Not enough capacity.");

            corners[0].X = Min.X;
            corners[0].Y = Max.Y;
            corners[0].Z = Max.Z;
            corners[1].X = Max.X;
            corners[1].Y = Max.Y;
            corners[1].Z = Max.Z;
            corners[2].X = Max.X;
            corners[2].Y = Min.Y;
            corners[2].Z = Max.Z;
            corners[3].X = Min.X;
            corners[3].Y = Min.Y;
            corners[3].Z = Max.Z;
            corners[4].X = Min.X;
            corners[4].Y = Max.Y;
            corners[4].Z = Min.Z;
            corners[5].X = Max.X;
            corners[5].Y = Max.Y;
            corners[5].Z = Min.Z;
            corners[6].X = Max.X;
            corners[6].Y = Min.Y;
            corners[6].Z = Min.Z;
            corners[7].X = Min.X;
            corners[7].Y = Min.Y;
            corners[7].Z = Min.Z;
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() + Max.GetHashCode();
        }

        public bool Intersects(in BoundingBox box)
        {
            if ((Max.X >= box.Min.X) && (Min.X <= box.Max.X))
            {
                if ((Max.Y < box.Min.Y) || (Min.Y > box.Max.Y))
                    return false;
                return (Max.Z >= box.Min.Z) && (Min.Z <= box.Max.Z);
            }
            return false;
        }

        public bool Intersects(BoundingFrustum frustum)
        {
            return frustum.Intersects(this);
        }

        /// <summary>
        /// Gets whether or not a specified sphere intersects with this box.
        /// </summary>
        /// <param name="box">The box for testing.</param>
        /// <returns><see langword="true"/> if <see cref="BoundingBox"/> intersects with this sphere; <see langword="false"/> otherwise.</returns>
        public bool Intersects(in BoundingSphere sphere)
        {
            if (sphere.Center.X - Min.X > sphere.Radius &&
                sphere.Center.Y - Min.Y > sphere.Radius &&
                sphere.Center.Z - Min.Z > sphere.Radius &&
                Max.X - sphere.Center.X > sphere.Radius &&
                Max.Y - sphere.Center.Y > sphere.Radius &&
                Max.Z - sphere.Center.Z > sphere.Radius)
                return true;

            double dmin = 0;

            if (sphere.Center.X - Min.X <= sphere.Radius)
                dmin += (sphere.Center.X - Min.X) * (sphere.Center.X - Min.X);
            else if (Max.X - sphere.Center.X <= sphere.Radius)
                dmin += (sphere.Center.X - Max.X) * (sphere.Center.X - Max.X);

            if (sphere.Center.Y - Min.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Min.Y) * (sphere.Center.Y - Min.Y);
            else if (Max.Y - sphere.Center.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Max.Y) * (sphere.Center.Y - Max.Y);

            if (sphere.Center.Z - Min.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Min.Z) * (sphere.Center.Z - Min.Z);
            else if (Max.Z - sphere.Center.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Max.Z) * (sphere.Center.Z - Max.Z);

            if (dmin <= sphere.Radius * sphere.Radius)
                return true;

            return false;
        }

        public PlaneIntersectionType Intersects(in Plane plane)
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

        public static bool operator ==(in BoundingBox a, in BoundingBox b)
        {
            return (a.Min == b.Min) && (a.Max == b.Max);
        }

        public static bool operator !=(in BoundingBox a, in BoundingBox b)
        {
            return !(a == b);
        }

        internal string DebugDisplayString
        {
            get => string.Concat(
                    "Min( ", Min.DebuggerDisplay, " )  \r\n",
                    "Max( ", Max.DebuggerDisplay, " )"
                    );
        }

        public override string ToString()
        {
            return "{{Min:" + Min.ToString() + " Max:" + Max.ToString() + "}}";
        }

        /// <summary>
        /// Deconstruction method for <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Deconstruct(out Vector3 min, out Vector3 max)
        {
            min = Min;
            max = Max;
        }

        #endregion Public Methods
    }
}