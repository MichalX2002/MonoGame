// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using FastMatrix = System.Numerics.Matrix4x4;

namespace MonoGame.Framework
{
    /// <summary>
    /// Represents the right-handed 4x4 floating-point matrix,
    /// which can store translation, scale and rotation information.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Matrix : IEquatable<Matrix>
    {
        public const int ColumnCount = 4;
        public const int RowCount = 4;
        public const int ElementCount = ColumnCount * RowCount;

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        public static Matrix Identity => FastMatrix.Identity;

        [IgnoreDataMember]
        public FastMatrix Base;

        #region Properties

        /// <summary>
        /// A first row and first column value.
        /// </summary>
        [DataMember]
        public float M11 { readonly get => Base.M11; set => Base.M11 = value; }

        /// <summary>
        /// A first row and second column value.
        /// </summary>
        [DataMember]
        public float M12 { readonly get => Base.M12; set => Base.M12 = value; }

        /// <summary>
        /// A first row and third column value.
        /// </summary>
        [DataMember]
        public float M13 { readonly get => Base.M13; set => Base.M13 = value; }

        /// <summary>
        /// A first row and fourth column value.
        /// </summary>
        [DataMember]
        public float M14 { readonly get => Base.M14; set => Base.M14 = value; }

        /// <summary>
        /// A second row and first column value.
        /// </summary>
        [DataMember]
        public float M21 { readonly get => Base.M21; set => Base.M21 = value; }

        /// <summary>
        /// A second row and second column value.
        /// </summary>
        [DataMember]
        public float M22 { readonly get => Base.M22; set => Base.M22 = value; }

        /// <summary>
        /// A second row and third column value.
        /// </summary>
        [DataMember]
        public float M23 { readonly get => Base.M23; set => Base.M23 = value; }

        /// <summary>
        /// A second row and fourth column value.
        /// </summary>
        [DataMember]
        public float M24 { readonly get => Base.M24; set => Base.M24 = value; }

        /// <summary>
        /// A third row and first column value.
        /// </summary>
        [DataMember]
        public float M31 { readonly get => Base.M31; set => Base.M31 = value; }

        /// <summary>
        /// A third row and second column value.
        /// </summary>
        [DataMember]
        public float M32 { readonly get => Base.M32; set => Base.M32 = value; }

        /// <summary>
        /// A third row and third column value.
        /// </summary>
        [DataMember]
        public float M33 { readonly get => Base.M33; set => Base.M33 = value; }

        /// <summary>
        /// A third row and fourth column value.
        /// </summary>
        [DataMember]
        public float M34 { readonly get => Base.M34; set => Base.M34 = value; }

        /// <summary>
        /// A fourth row and first column value.
        /// </summary>
        [DataMember]
        public float M41 { readonly get => Base.M41; set => Base.M41 = value; }

        /// <summary>
        /// A fourth row and second column value.
        /// </summary>
        [DataMember]
        public float M42 { readonly get => Base.M42; set => Base.M42 = value; }

        /// <summary>
        /// A fourth row and third column value.
        /// </summary>
        [DataMember]
        public float M43 { readonly get => Base.M43; set => Base.M43 = value; }

        /// <summary>
        /// A fourth row and fourth column value.
        /// </summary>
        [DataMember]
        public float M44 { readonly get => Base.M44; set => Base.M44 = value; }

        /// <summary>
        /// Gets whether the current matrix is the identity matrix.
        /// </summary>
        [IgnoreDataMember]
        public readonly bool IsIdentity => Base.IsIdentity;

        /// <summary>
        /// Gets or sets the backward vector formed from the third row M31, M32, M33 elements.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Backward
        {
            readonly get => new Vector3(M31, M32, M33);
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the down vector formed from the second row -M21, -M22, -M23 elements.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Down
        {
            readonly get => new Vector3(-M21, -M22, -M23);
            set
            {
                M21 = -value.X;
                M22 = -value.Y;
                M23 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the forward vector formed from the third row -M31, -M32, -M33 elements.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Forward
        {
            readonly get => new Vector3(-M31, -M32, -M33);
            set
            {
                M31 = -value.X;
                M32 = -value.Y;
                M33 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the left vector formed from the first row -M11, -M12, -M13 elements.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Left
        {
            readonly get => new Vector3(-M11, -M12, -M13);
            set
            {
                M11 = -value.X;
                M12 = -value.Y;
                M13 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the right vector formed from the first row M11, M12, M13 elements.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Right
        {
            readonly get => new Vector3(M11, M12, M13);
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the upper vector formed from the second row M21, M22, M23 elements.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Up
        {
            readonly get => new Vector3(M21, M22, M23);
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the position (M41, M42, M43) stored in this matrix.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Translation
        {
            readonly get => new Vector3(M41, M42, M43);
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }

        #endregion

        #region Indexers

        public float this[int index]
        {
            readonly get
            {
                return index switch
                {
                    0 => M11,
                    1 => M12,
                    2 => M13,
                    3 => M14,
                    4 => M21,
                    5 => M22,
                    6 => M23,
                    7 => M24,
                    8 => M31,
                    9 => M32,
                    10 => M33,
                    11 => M34,
                    12 => M41,
                    13 => M42,
                    14 => M43,
                    15 => M44,
                    _ => throw new IndexOutOfRangeException(),
                };
            }
            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M14 = value; break;
                    case 4: M21 = value; break;
                    case 5: M22 = value; break;
                    case 6: M23 = value; break;
                    case 7: M24 = value; break;
                    case 8: M31 = value; break;
                    case 9: M32 = value; break;
                    case 10: M33 = value; break;
                    case 11: M34 = value; break;
                    case 12: M41 = value; break;
                    case 13: M42 = value; break;
                    case 14: M43 = value; break;
                    case 15: M44 = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public float this[int row, int column]
        {
            readonly get => this[(row * 4) + column];
            set => this[(row * 4) + column] = value;
        }

        #endregion

        #region DebuggerDisplay

        internal string DebuggerDisplay
        {
            get
            {
                if (IsIdentity)
                    return "Identity";

                return string.Concat(
                     "( ", M11.ToString(), "  ", M12.ToString(), "  ", M13.ToString(), "  ", M14.ToString(), " )  \n",
                     "( ", M21.ToString(), "  ", M22.ToString(), "  ", M23.ToString(), "  ", M24.ToString(), " )  \n",
                     "( ", M31.ToString(), "  ", M32.ToString(), "  ", M33.ToString(), "  ", M34.ToString(), " )  \n",
                     "( ", M41.ToString(), "  ", M42.ToString(), "  ", M43.ToString(), "  ", M44.ToString(), " )");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a 4x4 matrix from individual values.
        /// </summary>
        /// <param name="m11">The first row and first column value.</param>
        /// <param name="m12">The first row and second column value.</param>
        /// <param name="m13">The first row and third column value.</param>
        /// <param name="m14">The first row and fourth column value.</param>
        /// <param name="m21">The second row and first column value.</param>
        /// <param name="m22">The second row and second column value.</param>
        /// <param name="m23">The second row and third column value.</param>
        /// <param name="m24">The second row and fourth column value.</param>
        /// <param name="m31">The third row and first column value.</param>
        /// <param name="m32">The third row and second column value.</param>
        /// <param name="m33">The third row and third column value.</param>
        /// <param name="m34">The third row and fourth column value.</param>
        /// <param name="m41">The fourth row and first column value.</param>
        /// <param name="m42">The fourth row and second column value.</param>
        /// <param name="m43">The fourth row and third column value.</param>
        /// <param name="m44">The fourth row and fourth column value.</param>
        public Matrix(
            float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            Base = new FastMatrix(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44);
        }

        /// <summary>
        /// Constructs a 4x4 matrix from rows.
        /// </summary>
        /// <param name="row1">The first row of the matrix.</param>
        /// <param name="row2">The second row of the matrix.</param>
        /// <param name="row3">The third row of the matrix.</param>
        /// <param name="row4">The fourth row of the matrix.</param>
        public Matrix(in Vector4 row1, in Vector4 row2, in Vector4 row3, in Vector4 row4) : this(
            row1.X, row1.Y, row1.Z, row1.W,
            row2.X, row2.Y, row2.Z, row2.W,
            row3.X, row3.Y, row3.Z, row3.W,
            row4.X, row4.Y, row4.Z, row4.W)
        {
        }

        #endregion

        #region Add (operator +)

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains sum of two matrixes.
        /// </summary>
        /// <param name="a">The first matrix to add.</param>
        /// <param name="b">The second matrix to add.</param>
        /// <returns>The result of the matrix addition.</returns>
        public static Matrix Add(in Matrix a, in Matrix b)
        {
            return FastMatrix.Add(a.Base, b.Base);
        }

        /// <summary>
        /// Adds two matrixes.
        /// </summary>
        /// <param name="a">Source <see cref="Matrix"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Matrix"/> on the right of the add sign.</param>
        /// <returns>Sum of the matrixes.</returns>
        public static Matrix operator +(in Matrix a, in Matrix b)
        {
            return a.Base + b.Base;
        }

        #endregion

        #region CreateBillboard

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for spherical billboarding that
        /// rotates around specified object position.
        /// </summary>
        /// <param name="objectPosition">Position of billboard object. It will rotate around that vector.</param>
        /// <param name="cameraPosition">The camera position.</param>
        /// <param name="cameraUpVector">The camera up vector.</param>
        /// <param name="cameraForwardVector">The camera forward vector.</param>
        /// <returns>The <see cref="Matrix"/> for spherical billboarding.</returns>
        public static Matrix CreateBillboard(
            Vector3 objectPosition,
            Vector3 cameraPosition,
            Vector3 cameraUpVector,
            Vector3 cameraForwardVector)
        {
            return FastMatrix.CreateBillboard(
                objectPosition, cameraPosition, cameraUpVector, cameraForwardVector);
        }

        #endregion

        #region CreateConstrainedBillboard

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for cylindrical billboarding that 
        /// rotates around specified axis.
        /// </summary>
        /// <param name="objectPosition">Object position the billboard will rotate around.</param>
        /// <param name="cameraPosition">Camera position.</param>
        /// <param name="rotateAxis">Axis of billboard for rotation.</param>
        /// <param name="cameraForwardVector">Camera forward vector.</param>
        /// <param name="objectForwardVector">Object forward vector.</param>
        /// <returns>The <see cref="Matrix"/> for cylindrical billboarding.</returns>
        public static Matrix CreateConstrainedBillboard(
            Vector3 objectPosition,
            Vector3 cameraPosition,
            Vector3 rotateAxis,
            Vector3 cameraForwardVector,
            Vector3 objectForwardVector)
        {
            return FastMatrix.CreateConstrainedBillboard(
                objectPosition, cameraPosition, rotateAxis, cameraForwardVector, objectForwardVector);
        }

        #endregion

        #region CreateFromAxisAngle

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains the 
        /// rotation moment around specified axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        public static Matrix CreateFromAxisAngle(Vector3 axis, float angle)
        {
            return FastMatrix.CreateFromAxisAngle(axis, angle);
        }

        #endregion

        #region CreateFromQuaternion

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion"><see cref="Quaternion"/> of rotation moment.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        public static Matrix CreateFromQuaternion(in Quaternion quaternion)
        {
            return FastMatrix.CreateFromQuaternion(quaternion.Base);
        }

        #endregion

        #region CreateFromYawPitchRoll

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from the specified yaw, pitch and roll values.
        /// </summary>
        /// <param name="yaw">The yaw rotation value in radians.</param>
        /// <param name="pitch">The pitch rotation value in radians.</param>
        /// <param name="roll">The roll rotation value in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        public static Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return FastMatrix.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        #endregion

        #region CreateLookAt

        /// <summary>
        /// Creates a new viewing <see cref="Matrix"/>.
        /// </summary>
        /// <param name="cameraPosition">Position of the camera.</param>
        /// <param name="cameraTarget">Lookup vector of the camera.</param>
        /// <param name="cameraUpVector">The direction of the upper edge of the camera.</param>
        /// <returns>The viewing <see cref="Matrix"/>.</returns>
        public static Matrix CreateLookAt(
            Vector3 cameraPosition,
            Vector3 cameraTarget,
            Vector3 cameraUpVector)
        {
            return FastMatrix.CreateLookAt(cameraPosition, cameraTarget, cameraUpVector);
        }

        #endregion

        #region CreateOrthographic

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for orthographic view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for orthographic view.</returns>
        public static Matrix CreateOrthographic(
            float width, float height, float zNearPlane, float zFarPlane)
        {
            return FastMatrix.CreateOrthographic(width, height, zNearPlane, zFarPlane);
        }

        #endregion

        #region CreateOrthographicOffCenter

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized orthographic view.
        /// </summary>
        /// <param name="left">Lower x-value at the near plane.</param>
        /// <param name="right">Upper x-value at the near plane.</param>
        /// <param name="bottom">Lower y-coordinate at the near plane.</param>
        /// <param name="top">Upper y-value at the near plane.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for customized orthographic view.</returns>
        public static Matrix CreateOrthographicOffCenter(
            float left, float right, float bottom, float top,
            float zNearPlane, float zFarPlane)
        {
            return FastMatrix.CreateOrthographicOffCenter(
                left, right, bottom, top, zNearPlane, zFarPlane);
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized orthographic view.
        /// </summary>
        /// <param name="viewingVolume">The viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for customized orthographic view.</returns>
        public static Matrix CreateOrthographicOffCenter(
            in RectangleF viewingVolume, float zNearPlane, float zFarPlane)
        {
            return CreateOrthographicOffCenter(
                viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top,
                zNearPlane, zFarPlane);
        }

        #endregion

        #region CreatePerspective

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for perspective view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for perspective view.</returns>
        public static Matrix CreatePerspective(
            float width, float height, float nearPlaneDistance, float farPlaneDistance)
        {
            return FastMatrix.CreatePerspective(
                width, height, nearPlaneDistance, farPlaneDistance);
        }

        #endregion

        #region CreatePerspectiveFieldOfView

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for perspective view with field of view.
        /// </summary>
        /// <param name="fieldOfView">Field of view in the y direction in radians.</param>
        /// <param name="aspectRatio">Width divided by height of the viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance of the near plane.</param>
        /// <param name="farPlaneDistance">Distance of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for perspective view with FOV.</returns>
        public static Matrix CreatePerspectiveFieldOfView(
            float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            return FastMatrix.CreatePerspectiveFieldOfView(
                fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        #endregion

        #region CreatePerspectiveOffCenter

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized perspective view.
        /// </summary>
        /// <param name="left">Lower x-value at the near plane.</param>
        /// <param name="right">Upper x-value at the near plane.</param>
        /// <param name="bottom">Lower y-coordinate at the near plane.</param>
        /// <param name="top">Upper y-value at the near plane.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <returns>The new <see cref="Matrix"/> for customized perspective view.</returns>
        public static Matrix CreatePerspectiveOffCenter(
            float left, float right, float bottom, float top,
            float nearPlaneDistance, float farPlaneDistance)
        {
            return FastMatrix.CreatePerspectiveOffCenter(
                left, right, bottom, top, nearPlaneDistance, farPlaneDistance);
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized perspective view.
        /// </summary>
        /// <param name="viewingVolume">The viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <returns>The new <see cref="Matrix"/> for customized perspective view.</returns>
        public static Matrix CreatePerspectiveOffCenter(
            in RectangleF viewingVolume,
            float nearPlaneDistance, float farPlaneDistance)
        {
            return CreatePerspectiveOffCenter(
                viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top,
                nearPlaneDistance, farPlaneDistance);
        }

        #endregion

        #region CreateRotationX

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around X axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around X axis.</returns>
        public static Matrix CreateRotationX(float radians)
        {
            return FastMatrix.CreateRotationX(radians);
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around X axis from a center point.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>The rotation <see cref="Matrix"/> around X axis.</returns>
        public static Matrix CreateRotationX(float radians, Vector3 centerPoint)
        {
            return FastMatrix.CreateRotationX(radians, centerPoint);
        }

        #endregion

        #region CreateRotationY

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Y axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Y axis.</returns>
        public static Matrix CreateRotationY(float radians)
        {
            return FastMatrix.CreateRotationY(radians);
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Y axis from a center point.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Y axis.</returns>
        public static Matrix CreateRotationY(float radians, Vector3 centerPoint)
        {
            return FastMatrix.CreateRotationY(radians, centerPoint);
        }

        #endregion

        #region CreateRotationZ

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Z axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Z axis.</returns>
        public static Matrix CreateRotationZ(float radians)
        {
            return FastMatrix.CreateRotationZ(radians);
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Z axis from a center point.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Z axis.</returns>
        public static Matrix CreateRotationZ(float radians, Vector3 centerPoint)
        {
            return FastMatrix.CreateRotationZ(radians, centerPoint);
        }

        #endregion

        #region CreateScale

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scales"><see cref="Vector3"/> representing x,y and z scale values.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(Vector3 scales)
        {
            return FastMatrix.CreateScale(scales);
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/> from a center point.
        /// </summary>
        /// <param name="scales"><see cref="Vector3"/> representing x,y and z scale values.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(Vector3 scales, Vector3 centerPoint)
        {
            return FastMatrix.CreateScale(scales, centerPoint);
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="xScale">Scale value for X axis.</param>
        /// <param name="yScale">Scale value for Y axis.</param>
        /// <param name="zScale">Scale value for Z axis.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(float xScale, float yScale, float zScale)
        {
            return FastMatrix.CreateScale(xScale, yScale, zScale);
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/> from a center point.
        /// </summary>
        /// <param name="xScale">Scale value for X axis.</param>
        /// <param name="yScale">Scale value for Y axis.</param>
        /// <param name="zScale">Scale value for Z axis.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(
            float xScale, float yScale, float zScale, Vector3 centerPoint)
        {
            return FastMatrix.CreateScale(xScale, yScale, zScale, centerPoint);
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scale">Scale value for all three axises.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(float scale)
        {
            return FastMatrix.CreateScale(scale);
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/> from a center point.
        /// </summary>
        /// <param name="scale">Scale value for all three axises.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(float scale, Vector3 centerPoint)
        {
            return FastMatrix.CreateScale(scale, centerPoint);
        }

        #endregion

        #region CreateShadow

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that flattens geometry into a specified 
        /// <see cref="Plane"/> as if casting a shadow from a specified light source. 
        /// </summary>
        /// <param name="lightDirection">
        /// A vector specifying the direction from which the light that will cast the shadow is coming.
        /// </param>
        /// <param name="plane">
        /// The plane onto which the new matrix should flatten geometry so as to cast a shadow.
        /// </param>
        /// <returns>
        /// A <see cref="Matrix"/> that can be used to flatten geometry
        /// onto the specified plane from the specified direction.
        /// </returns>
        public static Matrix CreateShadow(Vector3 lightDirection, in Plane plane)
        {
            return FastMatrix.CreateShadow(lightDirection, plane);
        }

        #endregion

        #region CreateTranslation

        /// <summary>
        /// Creates a new translation <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">X,Y and Z coordinates of translation.</param>
        /// <returns>The translation <see cref="Matrix"/>.</returns>
        public static Matrix CreateTranslation(Vector3 position)
        {
            return FastMatrix.CreateTranslation(position);
        }

        /// <summary>
        /// Creates a new translation <see cref="Matrix"/>.
        /// </summary>
        /// <param name="xPosition">X coordinate of translation.</param>
        /// <param name="yPosition">Y coordinate of translation.</param>
        /// <param name="zPosition">Z coordinate of translation.</param>
        /// <returns>The translation <see cref="Matrix"/>.</returns>
        public static Matrix CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            return FastMatrix.CreateTranslation(xPosition, yPosition, zPosition);
        }

        #endregion

        #region CreateReflection

        /// <summary>
        /// Creates a new reflection <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">The plane that used for reflection calculation.</param>
        /// <returns>The reflection <see cref="Matrix"/>.</returns>
        public static Matrix CreateReflection(in Plane value)
        {
            return FastMatrix.CreateReflection(value);
        }

        #endregion

        #region CreateWorld

        /// <summary>
        /// Creates a new world <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">The position vector.</param>
        /// <param name="forward">The forward direction vector.</param>
        /// <param name="up">The upward direction vector. Usually <see cref="Vector3.Up"/>.</param>
        /// <returns>The world <see cref="Matrix"/>.</returns>
        public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
        {
            return FastMatrix.CreateWorld(position, forward, up);
        }

        #endregion

        #region Decompose

        /// <summary>
        /// Decomposes this matrix to translation, rotation and scale elements.
        /// </summary>
        /// <param name="scale">Scale vector as an output parameter.</param>
        /// <param name="rotation">Rotation quaternion as an output parameter.</param>
        /// <param name="translation">Translation vector as an output parameter.</param>
        /// <returns><see langword="true"/> if matrix was decomposed; otherwise <see langword="false"/>.</returns>
        public static bool Decompose(
            in Matrix matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            return FastMatrix.Decompose(matrix, out scale.Base, out rotation.Base, out translation.Base);
        }

        #endregion

        #region GetDeterminant

        /// <summary>
        /// Calculates the determinant of the current 4x4 matrix.
        /// </summary>
        /// <returns>The determinant of this <see cref="Matrix"/></returns>
        public readonly float GetDeterminant()
        {
            return Base.GetDeterminant();
        }

        #endregion

        #region Equals (operator ==, !=)

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Matrix"/> without any tolerance.
        /// </summary>
        /// <param name="other">The <see cref="Matrix"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public readonly bool Equals(Matrix other)
        {
            return Base.Equals(other.Base);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/> without any tolerance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public readonly override bool Equals(object obj)
        {
            return obj is Matrix other ? this == other : false;
        }

        /// <summary>
        /// Compares whether two <see cref="Matrix"/> instances are equal without any tolerance.
        /// </summary>
        /// <param name="a">Source <see cref="Matrix"/> on the left of the equal sign.</param>
        /// <param name="b">Source <see cref="Matrix"/> on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Matrix a, in Matrix b)
        {
            return a.Base == b.Base;
        }

        /// <summary>
        /// Compares whether two <see cref="Matrix"/> instances are not equal without any tolerance.
        /// </summary>
        /// <param name="a">Source <see cref="Matrix"/> on the left of the not equal sign.</param>
        /// <param name="b">Source <see cref="Matrix"/> on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>
        public static bool operator !=(in Matrix a, in Matrix b)
        {
            return a.Base != b.Base;
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Gets the hash code of this <see cref="Matrix"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Matrix"/>.</returns>
        public readonly override int GetHashCode() => Base.GetHashCode();

        #endregion

        #region Invert

        /// <summary>
        /// Attempts to create a new <see cref="Matrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <param name="result">The inverted matrix.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public static bool Invert(in Matrix matrix, out Matrix result)
        {
            return FastMatrix.Invert(matrix, out result.Base);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <returns>The inverted matrix.</returns>
        /// <exception cref="ArgumentException">Failed to invert matrix.</exception>
        public static Matrix Invert(in Matrix matrix)
        {
            if (!Invert(matrix, out var result))
                throw new ArgumentException("Could not invert matrix.", nameof(matrix));
            return result;
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains linear interpolation of the values in specified matrixes.
        /// </summary>
        /// <param name="a">The first <see cref="Matrix"/>.</param>
        /// <param name="b">The second <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>>The result of linear interpolation of the specified matrixes.</returns>
        public static Matrix Lerp(in Matrix a, in Matrix b, float amount)
        {
            return FastMatrix.Lerp(a, b, amount);
        }

        #endregion

        #region Multiply (operator *)

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of two matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/>.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        public static Matrix Multiply(in Matrix left, in Matrix right)
        {
            return FastMatrix.Multiply(left, right);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of <see cref="Matrix"/> and a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static Matrix Multiply(in Matrix matrix, float scaleFactor)
        {
            return FastMatrix.Multiply(matrix, scaleFactor);
        }

        /// <summary>
        /// Multiplies two matrixes.
        /// </summary>
        /// <param name="left">Source <see cref="Matrix"/> on the left of the mul sign.</param>
        /// <param name="right">Source <see cref="Matrix"/> on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        public static Matrix operator *(in Matrix left, in Matrix right)
        {
            return left.Base * right.Base;
        }

        /// <summary>
        /// Multiplies the elements of matrix by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static Matrix operator *(in Matrix matrix, float scaleFactor)
        {
            return matrix.Base * scaleFactor;
        }

        #endregion

        #region TryCopyTo

        /// <summary>
        /// Attempts to copy the values of this <see cref="Matrix"/> into a span.
        /// </summary>
        /// <returns>Whether the operation succeeded.</returns>
        public readonly bool TryCopyTo(Span<float> destination)
        {
            var matrixSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(this), 1);
            var floatSpan = MemoryMarshal.Cast<Matrix, float>(matrixSpan);
            return floatSpan.TryCopyTo(destination);
        }

        #endregion

        #region CopyTo

        /// <summary>
        /// Copies the values of this <see cref="Matrix"/> into a span.
        /// </summary>
        public readonly void CopyTo(Span<float> destination)
        {
            var matrixSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(this), 1);
            var floatSpan = MemoryMarshal.Cast<Matrix, float>(matrixSpan);
            floatSpan.CopyTo(destination);
        }

        #endregion

        #region Negate

        /// <summary>
        /// Returns a matrix with the all values negated.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <returns>Result of the matrix negation.</returns>
        public static Matrix Negate(in Matrix matrix)
        {
            return FastMatrix.Negate(matrix);
        }

        /// <summary>
        /// Negates values in the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Matrix operator -(in Matrix matrix)
        {
            return -matrix.Base;
        }

        #endregion

        #region Subtract (operator -)

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains subtraction of one matrix from another.
        /// </summary>
        /// <param name="left">The first <see cref="Matrix"/>.</param>
        /// <param name="right">The second <see cref="Matrix"/>.</param>
        /// <returns>The result of the matrix subtraction.</returns>
        public static Matrix Subtract(in Matrix left, in Matrix right)
        {
            return FastMatrix.Subtract(left, right);
        }

        /// <summary>
        /// Subtracts the values of one <see cref="Matrix"/> from another <see cref="Matrix"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Matrix"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Matrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the matrix subtraction.</returns>
        public static Matrix operator -(in Matrix left, in Matrix right)
        {
            return left.Base - right.Base;
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Matrix"/>.
        /// </summary>
        /// <returns>String representation of this <see cref="Matrix"/>.</returns>
        public readonly override string ToString()
        {
            return Base.ToString();
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Swap the matrix rows and columns.
        /// </summary>
        /// <param name="matrix">The matrix for transposing operation.</param>
        /// <returns>The new <see cref="Matrix"/> which contains the transposed result.</returns>
        public static Matrix Transpose(in Matrix matrix)
        {
            return FastMatrix.Transpose(matrix);
        }

        #endregion

        ///// <summary>
        ///// Helper method for using the Laplace expansion theorem using two rows expansions to calculate major and 
        ///// minor determinants of a 4x4 matrix. This method is used for inverting a matrix.
        ///// </summary>
        //private static void FindDeterminants(
        //    in Matrix matrix, out float major,
        //    out float minor1, out float minor2, out float minor3, out float minor4, out float minor5, out float minor6,
        //    out float minor7, out float minor8, out float minor9, out float minor10, out float minor11, out float minor12)
        //{
        //    double det1 = matrix.M11 * (double)matrix.M22 - matrix.M12 * (double)matrix.M21;
        //    double det2 = matrix.M11 * (double)matrix.M23 - matrix.M13 * (double)matrix.M21;
        //    double det3 = matrix.M11 * (double)matrix.M24 - matrix.M14 * (double)matrix.M21;
        //    double det4 = matrix.M12 * (double)matrix.M23 - matrix.M13 * (double)matrix.M22;
        //    double det5 = matrix.M12 * (double)matrix.M24 - matrix.M14 * (double)matrix.M22;
        //    double det6 = matrix.M13 * (double)matrix.M24 - matrix.M14 * (double)matrix.M23;
        //    double det7 = matrix.M31 * (double)matrix.M42 - matrix.M32 * (double)matrix.M41;
        //    double det8 = matrix.M31 * (double)matrix.M43 - matrix.M33 * (double)matrix.M41;
        //    double det9 = matrix.M31 * (double)matrix.M44 - matrix.M34 * (double)matrix.M41;
        //    double det10 = matrix.M32 * (double)matrix.M43 - matrix.M33 * (double)matrix.M42;
        //    double det11 = matrix.M32 * (double)matrix.M44 - matrix.M34 * (double)matrix.M42;
        //    double det12 = matrix.M33 * (double)matrix.M44 - matrix.M34 * (double)matrix.M43;
        //
        //    major = (float)(det1 * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7);
        //    minor1 = (float)det1;
        //    minor2 = (float)det2;
        //    minor3 = (float)det3;
        //    minor4 = (float)det4;
        //    minor5 = (float)det5;
        //    minor6 = (float)det6;
        //    minor7 = (float)det7;
        //    minor8 = (float)det8;
        //    minor9 = (float)det9;
        //    minor10 = (float)det10;
        //    minor11 = (float)det11;
        //    minor12 = (float)det12;
        //}

        public static implicit operator FastMatrix(in Matrix matrix)
        {
            return matrix.Base;
        }

        public static implicit operator Matrix(in FastMatrix matrix)
        {
            return new Matrix { Base = matrix };
        }
    }
}
