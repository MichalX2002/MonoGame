// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Numerics;

namespace MonoGame.Framework
{
    /// <summary>
    /// Defines a viewing frustum for intersection operations.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class BoundingFrustum : IEquatable<BoundingFrustum>
    {
        /// <summary>
        /// The number of planes in the frustum.
        /// </summary>
        public const int PlaneCount = 6;

        /// <summary>
        /// The number of corner points in the frustum.
        /// </summary>
        public const int CornerCount = 8;

        private Matrix4x4 _matrix;
        private readonly Vector3[] _corners = new Vector3[CornerCount];
        private readonly Plane[] _planes = new Plane[PlaneCount];

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Matrix4x4"/> transformation of the frustum.
        /// </summary>
        public Matrix4x4 Matrix
        {
            get => _matrix;
            set
            {
                _matrix = value;
                CreatePlanes();  // FIXME: The odds are the planes will be used a lot more often than the matrix
                CreateCorners(); // is updated, so this should help performance. I hope ;)
            }
        }

        /// <summary>
        /// Gets a span of this frustum's corners.
        /// </summary>
        public ReadOnlySpan<Vector3> Corners => _corners.AsSpan();

        /// <summary>
        /// Gets a span of this frustum's planes.
        /// </summary>
        public ReadOnlySpan<Plane> Planes => _planes.AsSpan();

        /// <summary>
        /// Gets the near plane of the frustum.
        /// </summary>
        public Plane Near => _planes[0];

        /// <summary>
        /// Gets the far plane of the frustum.
        /// </summary>
        public Plane Far => _planes[1];

        /// <summary>
        /// Gets the left plane of the frustum.
        /// </summary>
        public Plane Left => _planes[2];

        /// <summary>
        /// Gets the right plane of the frustum.
        /// </summary>
        public Plane Right => _planes[3];

        /// <summary>
        /// Gets the top plane of the frustum.
        /// </summary>
        public Plane Top => _planes[4];

        /// <summary>
        /// Gets the bottom plane of the frustum.
        /// </summary>
        public Plane Bottom => _planes[5];

        #endregion

        #region Internal Properties

        internal string DebuggerDisplay => string.Concat(
            "Near = ", _planes[0].ToString(), ", ",
            "Far = ", _planes[1].ToString(), ", ",
            "Left = ", _planes[2].ToString(), ", ",
            "Right = ", _planes[3].ToString(), ", ",
            "Top = ", _planes[4].ToString(), ", ",
            "Bottom = ", _planes[5].ToString());

        #endregion

        /// <summary>
        /// Constructs the frustum by extracting the view planes from a matrix.
        /// </summary>
        /// <param name="value">Combined matrix which usually is (View * Projection).</param>
        public BoundingFrustum(in Matrix4x4 value)
        {
            _matrix = value;
            CreatePlanes();
            CreateCorners();
        }

        private void CreateCorners()
        {
            IntersectionPoint(_planes[0], _planes[2], _planes[4], out _corners[0]);
            IntersectionPoint(_planes[0], _planes[3], _planes[4], out _corners[1]);
            IntersectionPoint(_planes[0], _planes[3], _planes[5], out _corners[2]);
            IntersectionPoint(_planes[0], _planes[2], _planes[5], out _corners[3]);
            IntersectionPoint(_planes[1], _planes[2], _planes[4], out _corners[4]);
            IntersectionPoint(_planes[1], _planes[3], _planes[4], out _corners[5]);
            IntersectionPoint(_planes[1], _planes[3], _planes[5], out _corners[6]);
            IntersectionPoint(_planes[1], _planes[2], _planes[5], out _corners[7]);
        }

        private void CreatePlanes()
        {
            _planes[0] = new Plane(-_matrix.M13, -_matrix.M23, -_matrix.M33, -_matrix.M43);
            _planes[1] = new Plane(_matrix.M13 - _matrix.M14, _matrix.M23 - _matrix.M24, _matrix.M33 - _matrix.M34, _matrix.M43 - _matrix.M44);
            _planes[2] = new Plane(-_matrix.M14 - _matrix.M11, -_matrix.M24 - _matrix.M21, -_matrix.M34 - _matrix.M31, -_matrix.M44 - _matrix.M41);
            _planes[3] = new Plane(_matrix.M11 - _matrix.M14, _matrix.M21 - _matrix.M24, _matrix.M31 - _matrix.M34, _matrix.M41 - _matrix.M44);
            _planes[4] = new Plane(_matrix.M12 - _matrix.M14, _matrix.M22 - _matrix.M24, _matrix.M32 - _matrix.M34, _matrix.M42 - _matrix.M44);
            _planes[5] = new Plane(-_matrix.M14 - _matrix.M12, -_matrix.M24 - _matrix.M22, -_matrix.M34 - _matrix.M32, -_matrix.M44 - _matrix.M42);

            _planes[0] = Plane.Normalize(_planes[0]);
            _planes[1] = Plane.Normalize(_planes[1]);
            _planes[2] = Plane.Normalize(_planes[2]);
            _planes[3] = Plane.Normalize(_planes[3]);
            _planes[4] = Plane.Normalize(_planes[4]);
            _planes[5] = Plane.Normalize(_planes[5]);
        }

        #region Public Methods

        #region Contains

        /// <summary>
        /// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">A <see cref="BoundingBox"/> for testing.</param>
        /// <returns>Containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingBox"/>.</returns>
        public ContainmentType Contains(in BoundingBox box)
        {
            foreach (var plane in _planes)
            {
                switch (box.Intersects(plane))
                {
                    case PlaneIntersectionType.Front:
                        return ContainmentType.Disjoint;

                    case PlaneIntersectionType.Intersecting:
                        return ContainmentType.Intersects;
                }
            }
            return ContainmentType.Contains;
        }

        /// <summary>
        /// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingFrustum"/>.
        /// </summary>
        /// <param name="frustum">A <see cref="BoundingFrustum"/> for testing.</param>
        /// <returns>Containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingFrustum"/>.</returns>
        public ContainmentType Contains(BoundingFrustum frustum)
        {
            if (this == frustum)                 // We check to see if the two frustums are equal
                return ContainmentType.Contains; // If they are, there's no need to go any further.

            foreach (var plane in _planes)
            {
                switch (frustum.Intersects(plane))
                {
                    case PlaneIntersectionType.Front:
                        return ContainmentType.Disjoint;

                    case PlaneIntersectionType.Intersecting:
                        return ContainmentType.Intersects;
                }
            }
            return ContainmentType.Contains;
        }

        /// <summary>
        /// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">A <see cref="BoundingSphere"/> for testing.</param>
        /// <returns>Containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingSphere"/>.</returns>
        public ContainmentType Contains(in BoundingSphere sphere)
        {
            foreach (var plane in _planes)
            {
                switch (sphere.Intersects(plane))
                {
                    case PlaneIntersectionType.Front:
                        return ContainmentType.Disjoint;

                    case PlaneIntersectionType.Intersecting:
                        return ContainmentType.Intersects;
                }
            }
            return ContainmentType.Contains;
        }

        /// <summary>
        /// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="point">A <see cref="Vector3"/> for testing.</param>
        /// <returns>Containment between this <see cref="BoundingFrustum"/> and specified <see cref="Vector3"/>.</returns>
        public ContainmentType Contains(Vector3 point)
        {
            foreach (var plane in _planes)
            {
                if (PlaneHelper.ClassifyPoint(point, plane) > 0)
                    return ContainmentType.Disjoint;
            }
            return ContainmentType.Contains;
        }

        #endregion

        /// <summary>
        /// Gets whether or not a specified <see cref="BoundingBox"/> intersects with this <see cref="BoundingFrustum"/>.
        /// </summary>
        /// <param name="box">A <see cref="BoundingBox"/> for intersection test.</param>
        /// <returns>Whether the <see cref="BoundingBox"/> intersects with this <see cref="BoundingFrustum"/>.</returns>
        public bool Intersects(in BoundingBox box)
        {
            return Contains(box) != ContainmentType.Disjoint;
        }

        /// <summary>
        /// Gets whether or not a specified <see cref="BoundingFrustum"/> intersects with this <see cref="BoundingFrustum"/>.
        /// </summary>
        /// <param name="frustum">An other <see cref="BoundingFrustum"/> for intersection test.</param>
        /// <returns>Whether the <see cref="BoundingFrustum"/> intersects with this <see cref="BoundingFrustum"/>.</returns>
        public bool Intersects(BoundingFrustum frustum)
        {
            return Contains(frustum) != ContainmentType.Disjoint;
        }

        /// <summary>
        /// Gets whether or not a specified <see cref="BoundingSphere"/> intersects with this <see cref="BoundingFrustum"/>.
        /// </summary>
        /// <param name="sphere">A <see cref="BoundingSphere"/> for intersection test.</param>
        /// <returns>Whether the <see cref="BoundingSphere"/> intersects with this <see cref="BoundingFrustum"/>.</returns>
        public bool Intersects(in BoundingSphere sphere)
        {
            return Contains(sphere) != ContainmentType.Disjoint;
        }

        /// <summary>
        /// Gets type of intersection between specified <see cref="Plane"/> and this <see cref="BoundingFrustum"/>.
        /// </summary>
        /// <param name="plane">A <see cref="Plane"/> for intersection test.</param>
        /// <returns>A plane intersection type.</returns>
        public PlaneIntersectionType Intersects(Plane plane)
        {
            var result = plane.Intersects(_corners[0]);
            for (int i = 1; i < _corners.Length; i++)
            {
                if (plane.Intersects(_corners[i]) != result)
                    result = PlaneIntersectionType.Intersecting;
            }
            return result;
        }

        /// <summary>
        /// Gets the distance of intersection of <see cref="Ray"/> and this <see cref="BoundingFrustum"/>.
        /// </summary>
        /// <param name="ray">A <see cref="Ray"/> for intersection test.</param>
        /// <param name="distance">Distance at which ray intersects with this <see cref="BoundingFrustum"/>.</param>
        /// <returns>Whether the <see cref="Ray"/> intersects the <see cref="BoundingFrustum"/>.</returns>
        public bool Intersects(in Ray ray, out float distance)
        {
            distance = 0f;

            switch (Contains(ray.Position))
            {
                case ContainmentType.Disjoint:
                    return false;

                case ContainmentType.Contains:
                    return true;

                case ContainmentType.Intersects:
                    throw new NotImplementedException();
            }
            return false;
        }

        #endregion

        private static void IntersectionPoint(Plane a, Plane b, Plane c, out Vector3 destination)
        {
            var bcCross = Vector3.Cross(b.Normal, c.Normal);
            var v1 = bcCross * a.D;
            var v2 = Vector3.Cross(c.Normal, a.Normal) * b.D;
            var v3 = Vector3.Cross(a.Normal, b.Normal) * c.D;

            float f = -Vector3.Dot(a.Normal, bcCross);
            destination = (v1 + v2 + v3) / f;
        }

        #region Equals

        public bool Equals(BoundingFrustum other) => this == other;

        public override bool Equals(object obj) => obj is BoundingFrustum other && Equals(other);

        public static bool operator ==(BoundingFrustum a, BoundingFrustum b)
        {
            if (Equals(a, null))
                return Equals(b, null);
            if (Equals(b, null))
                return Equals(a, null);

            return a._matrix == b._matrix;
        }

        public static bool operator !=(BoundingFrustum a, BoundingFrustum b)
        {
            return !(a == b);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Returns the hash code of the <see cref="BoundingFrustum"/>.
        /// </summary>
        public override int GetHashCode() => _matrix.GetHashCode();

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="BoundingFrustum"/>.
        /// </summary>
        public override string ToString()
        {
            return string.Concat(
                "{Near: " + _planes[0].ToString() +
                " Far:" + _planes[1].ToString() +
                " Left:" + _planes[2].ToString() +
                " Right:" + _planes[3].ToString() +
                " Top:" + _planes[4].ToString() +
                " Bottom:" + _planes[5].ToString() +
                "}");
        }

        #endregion
    }
}

