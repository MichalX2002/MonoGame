// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

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
        /// <summary>
        /// The identity matrix.
        /// </summary>
        public static readonly Matrix Identity = new Matrix(
            1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

        #region Public Fields

        /// <summary>
        /// A first row and first column value.
        /// </summary>
        [DataMember]
        public float M11;

        /// <summary>
        /// A first row and second column value.
        /// </summary>
        [DataMember]
        public float M12;

        /// <summary>
        /// A first row and third column value.
        /// </summary>
        [DataMember]
        public float M13;

        /// <summary>
        /// A first row and fourth column value.
        /// </summary>
        [DataMember]
        public float M14;

        /// <summary>
        /// A second row and first column value.
        /// </summary>
        [DataMember]
        public float M21;

        /// <summary>
        /// A second row and second column value.
        /// </summary>
        [DataMember]
        public float M22;

        /// <summary>
        /// A second row and third column value.
        /// </summary>
        [DataMember]
        public float M23;

        /// <summary>
        /// A second row and fourth column value.
        /// </summary>
        [DataMember]
        public float M24;

        /// <summary>
        /// A third row and first column value.
        /// </summary>
        [DataMember]
        public float M31;

        /// <summary>
        /// A third row and second column value.
        /// </summary>
        [DataMember]
        public float M32;

        /// <summary>
        /// A third row and third column value.
        /// </summary>
        [DataMember]
        public float M33;

        /// <summary>
        /// A third row and fourth column value.
        /// </summary>
        [DataMember]
        public float M34;

        /// <summary>
        /// A fourth row and first column value.
        /// </summary>
        [DataMember]
        public float M41;

        /// <summary>
        /// A fourth row and second column value.
        /// </summary>
        [DataMember]
        public float M42;

        /// <summary>
        /// A fourth row and third column value.
        /// </summary>
        [DataMember]
        public float M43;

        /// <summary>
        /// A fourth row and fourth column value.
        /// </summary>
        [DataMember]
        public float M44;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Constructs a matrix.
        /// </summary>
        /// <param name="m11">A first row and first column value.</param>
        /// <param name="m12">A first row and second column value.</param>
        /// <param name="m13">A first row and third column value.</param>
        /// <param name="m14">A first row and fourth column value.</param>
        /// <param name="m21">A second row and first column value.</param>
        /// <param name="m22">A second row and second column value.</param>
        /// <param name="m23">A second row and third column value.</param>
        /// <param name="m24">A second row and fourth column value.</param>
        /// <param name="m31">A third row and first column value.</param>
        /// <param name="m32">A third row and second column value.</param>
        /// <param name="m33">A third row and third column value.</param>
        /// <param name="m34">A third row and fourth column value.</param>
        /// <param name="m41">A fourth row and first column value.</param>
        /// <param name="m42">A fourth row and second column value.</param>
        /// <param name="m43">A fourth row and third column value.</param>
        /// <param name="m44">A fourth row and fourth column value.</param>
        public Matrix(
            float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        /// <summary>
        /// Constructs a matrix.
        /// </summary>
        /// <param name="row1">A first row of the created matrix.</param>
        /// <param name="row2">A second row of the created matrix.</param>
        /// <param name="row3">A third row of the created matrix.</param>
        /// <param name="row4">A fourth row of the created matrix.</param>
        public Matrix(in Vector4 row1, in Vector4 row2, in Vector4 row3, in Vector4 row4)
        {
            M11 = row1.X;
            M12 = row1.Y;
            M13 = row1.Z;
            M14 = row1.W;
            M21 = row2.X;
            M22 = row2.Y;
            M23 = row2.Z;
            M24 = row2.W;
            M31 = row3.X;
            M32 = row3.Y;
            M33 = row3.Z;
            M34 = row3.W;
            M41 = row4.X;
            M42 = row4.Y;
            M43 = row4.Z;
            M44 = row4.W;
        }

        #endregion

        #region Indexers

        public float this[int index]
        {
            readonly get
            {
                switch (index)
                {
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M14;
                    case 4: return M21;
                    case 5: return M22;
                    case 6: return M23;
                    case 7: return M24;
                    case 8: return M31;
                    case 9: return M32;
                    case 10: return M33;
                    case 11: return M34;
                    case 12: return M41;
                    case 13: return M42;
                    case 14: return M43;
                    case 15: return M44;
                }
                throw new ArgumentOutOfRangeException();
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
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public float this[int row, int column]
        {
            get => this[(row * 4) + column];
            set => this[(row * 4) + column] = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the backward vector formed from the third row M31, M32, M33 elements.
        /// </summary>
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

        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains sum of two matrixes.
        /// </summary>
        /// <param name="a">The first matrix to add.</param>
        /// <param name="b">The second matrix to add.</param>
        /// <param name="result">The result of the matrix addition.</param>
        public static Matrix Add(in Matrix a, in Matrix b) => a + b;

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for spherical billboarding that rotates around specified object position.
        /// </summary>
        /// <param name="objectPosition">Position of billboard object. It will rotate around that vector.</param>
        /// <param name="cameraPosition">The camera position.</param>
        /// <param name="cameraUpVector">The camera up vector.</param>
        /// <param name="cameraForwardVector">Optional camera forward vector.</param>
        /// <returns>The <see cref="Matrix"/> for spherical billboarding.</returns>
        public static Matrix CreateBillboard(
            Vector3 objectPosition, Vector3 cameraPosition,
            Vector3 cameraUpVector, Vector3? cameraForwardVector)
        {
            Vector3 vector = objectPosition - cameraPosition;
            float num = vector.LengthSquared();
            vector = num < 0.0001f
                ? cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward
                : Vector3.Multiply(vector, 1f / MathF.Sqrt(num));

            Vector3.Cross(cameraUpVector, vector, out var vector3);
            vector3.Normalize();
            
            Vector3.Cross(vector, vector3, out var vector2);

            return new Matrix(
                m11: vector3.X,
                m12: vector3.Y,
                m13: vector3.Z,
                m14: 0,
                m21: vector2.X,
                m22: vector2.Y,
                m23: vector2.Z,
                m24: 0,
                m31: vector.X,
                m32: vector.Y,
                m33: vector.Z,
                m34: 0,
                m41: objectPosition.X,
                m42: objectPosition.Y,
                m43: objectPosition.Z,
                m44: 1);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for cylindrical billboarding that rotates around specified axis.
        /// </summary>
        /// <param name="objectPosition">Object position the billboard will rotate around.</param>
        /// <param name="cameraPosition">Camera position.</param>
        /// <param name="rotateAxis">Axis of billboard for rotation.</param>
        /// <param name="cameraForwardVector">Optional camera forward vector.</param>
        /// <param name="objectForwardVector">Optional object forward vector.</param>
        /// <returns>The <see cref="Matrix"/> for cylindrical billboarding.</returns>
        public static Matrix CreateConstrainedBillboard(
            Vector3 objectPosition, Vector3 cameraPosition, Vector3 rotateAxis,
            Vector3? cameraForwardVector, Vector3? objectForwardVector)
        {
            Vector3 vector2 = objectPosition - cameraPosition;
            float num2 = vector2.LengthSquared();
            vector2 = num2 < 0.0001f
                ? cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward
                : Vector3.Multiply(vector2, 1f / (MathF.Sqrt(num2)));

            Vector3 vector1;
            Vector3 vector3;
            float num = Vector3.Dot(rotateAxis, vector2);
            if (Math.Abs(num) > 0.9982547f)
            {
                if (objectForwardVector.HasValue)
                {
                    vector1 = objectForwardVector.Value;
                    num = Vector3.Dot(rotateAxis, vector1);
                    if (Math.Abs(num) > 0.9982547f)
                    {
                        num = (rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y) + (rotateAxis.Z * Vector3.Forward.Z);
                        vector1 = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
                    }
                }
                else
                {
                    num = (rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y) + (rotateAxis.Z * Vector3.Forward.Z);
                    vector1 = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
                }
                vector3 = Vector3.Cross(rotateAxis, vector1);
                vector3.Normalize();
                vector1 = Vector3.Cross(vector3, rotateAxis);
            }
            else
            {
                vector3 = Vector3.Cross(rotateAxis, vector2);
                vector3.Normalize();
                vector1 = Vector3.Cross(vector3, rotateAxis);
            }
            vector1.Normalize();

            return new Matrix(
                m11: vector3.X,
                m12: vector3.Y,
                m13: vector3.Z,
                m14: 0,
                m21: rotateAxis.X,
                m22: rotateAxis.Y,
                m23: rotateAxis.Z,
                m24: 0,
                m31: vector1.X,
                m32: vector1.Y,
                m33: vector1.Z,
                m34: 0,
                m41: objectPosition.X,
                m42: objectPosition.Y,
                m43: objectPosition.Z,
                m44: 1);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains the rotation moment around specified axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        public static Matrix CreateFromAxisAngle(Vector3 axis, float angle)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num2 = MathF.Sin(angle);
            float num = MathF.Cos(angle);
            float num11 = x * x;
            float num10 = y * y;
            float num9 = z * z;
            float num8 = x * y;
            float num7 = x * z;
            float num6 = y * z;

            return new Matrix(
                m11: num11 + (num * (1f - num11)),
                m12: num8 - (num * num8) + (num2 * z),
                m13: num7 - (num * num7) - (num2 * y),
                m14: 0,
                m21: num8 - (num * num8) - (num2 * z),
                m22: num10 + (num * (1f - num10)),
                m23: num6 - (num * num6) + (num2 * x),
                m24: 0,
                m31: num7 - (num * num7) + (num2 * y),
                m32: num6 - (num * num6) - (num2 * x),
                m33: num9 + (num * (1f - num9)),
                m34: 0,
                m41: 0,
                m42: 0,
                m43: 0,
                m44: 1);
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion"><see cref="Quaternion"/> of rotation moment.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        public static Matrix CreateFromQuaternion(in Quaternion quaternion)
        {
            float num9 = quaternion.X * quaternion.X;
            float num8 = quaternion.Y * quaternion.Y;
            float num7 = quaternion.Z * quaternion.Z;
            float num6 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num4 = quaternion.Z * quaternion.X;
            float num3 = quaternion.Y * quaternion.W;
            float num2 = quaternion.Y * quaternion.Z;
            float num = quaternion.X * quaternion.W;

            return new Matrix(
                m11: 1f - (2f * (num8 + num7)),
                m12: 2f * (num6 + num5),
                m13: 2f * (num4 - num3),
                m14: 0f,
                m21: 2f * (num6 - num5),
                m22: 1f - (2f * (num7 + num9)),
                m23: 2f * (num2 + num),
                m24: 0f,
                m31: 2f * (num4 + num3),
                m32: 2f * (num2 - num),
                m33: 1f - (2f * (num8 + num9)),
                m34: 0f,
                m41: 0f,
                m42: 0f,
                m43: 0f,
                m44: 1f);
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from the specified yaw, pitch and roll values.
        /// </summary>
        /// <param name="yaw">The yaw rotation value in radians.</param>
        /// <param name="pitch">The pitch rotation value in radians.</param>
        /// <param name="roll">The roll rotation value in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        /// <remarks>
        /// For more information about yaw, pitch and roll visit http://en.wikipedia.org/wiki/Euler_angles.
        /// </remarks>
        public static Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Quaternion quaternion = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            return CreateFromQuaternion(quaternion);
        }

        /// <summary>
        /// Creates a new viewing <see cref="Matrix"/>.
        /// </summary>
        /// <param name="cameraPosition">Position of the camera.</param>
        /// <param name="cameraTarget">Lookup vector of the camera.</param>
        /// <param name="cameraUpVector">The direction of the upper edge of the camera.</param>
        /// <returns>The viewing <see cref="Matrix"/>.</returns>
        public static Matrix CreateLookAt(
            in Vector3 cameraPosition, in Vector3 cameraTarget, in Vector3 cameraUpVector)
        {
            var vector1 = Vector3.Normalize(cameraPosition - cameraTarget);
            var vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector1));
            var vector3 = Vector3.Cross(vector1, vector2);

            return new Matrix(
                m11: vector2.X,
                m12: vector3.X,
                m13: vector1.X,
                m14: 0f,
                m21: vector2.Y,
                m22: vector3.Y,
                m23: vector1.Y,
                m24: 0f,
                m31: vector2.Z,
                m32: vector3.Z,
                m33: vector1.Z,
                m34: 0f,
                m41: -Vector3.Dot(vector2, cameraPosition),
                m42: -Vector3.Dot(vector3, cameraPosition),
                m43: -Vector3.Dot(vector1, cameraPosition),
                m44: 1f);
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for orthographic view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for orthographic view.</returns>
        public static Matrix CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
        {
            CreateOrthographic(width, height, zNearPlane, zFarPlane, out Matrix matrix);
            return matrix;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for orthographic view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <param name="result">The new projection <see cref="Matrix"/> for orthographic view as an output parameter.</param>
        public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix result)
        {
            result.M11 = 2f / width;
            result.M12 = result.M13 = result.M14 = 0f;
            result.M22 = 2f / height;
            result.M21 = result.M23 = result.M24 = 0f;
            result.M33 = 1f / (zNearPlane - zFarPlane);
            result.M31 = result.M32 = result.M34 = 0f;
            result.M41 = result.M42 = 0f;
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = 1f;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized orthographic view.
        /// </summary>
        /// <param name="viewingVolume">The viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for customized orthographic view.</returns>
        public static Matrix CreateOrthographicOffCenter(Rectangle viewingVolume, float zNearPlane, float zFarPlane)
        {
            return CreateOrthographicOffCenter(
                viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top, zNearPlane, zFarPlane);
        }

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
            float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
            return new Matrix(
                m11: (float)(2.0 / (right - (double)left)),
                m12: 0f,
                m13: 0f,
                m14: 0f,
                m21: 0f,
                m22: (float)(2.0 / (top - (double)bottom)),
                m23: 0f,
                m24: 0f,
                m31: 0f,
                m32: 0f,
                m33: (float)(1.0 / (zNearPlane - (double)zFarPlane)),
                m34: 0f,
                m41: (float)((left + (double)right) / (left - (double)right)),
                m42: (float)((top + (double)bottom) / (bottom - (double)top)),
                m43: (float)(zNearPlane / (zNearPlane - (double)zFarPlane)),
                m44: 1f);
        }

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
            if (nearPlaneDistance <= 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(nearPlaneDistance), $"{nameof(nearPlaneDistance)} <= 0 ({nearPlaneDistance})");

            if (farPlaneDistance <= 0f)
                throw new ArgumentException(
                    nameof(farPlaneDistance), $"{nameof(farPlaneDistance)} <= 0 ({farPlaneDistance})");

            if (nearPlaneDistance >= farPlaneDistance)
                throw new ArgumentException(
                    nameof(nearPlaneDistance), $"{nameof(nearPlaneDistance)} >= {nameof(farPlaneDistance)} " +
                    $"({nearPlaneDistance} >= {farPlaneDistance})");

            return new Matrix(
                m11: 2f * nearPlaneDistance / width,
                m12: 0,
                m13: 0,
                m14: 0f,
                m22: 2f * nearPlaneDistance / height,
                m21: 0,
                m23: 0,
                m24: 0f,
                m33: farPlaneDistance / (nearPlaneDistance - farPlaneDistance),
                m31: 0,
                m32: 0f,
                m34: -1f,
                m41: 0,
                m42: 0,
                m44: 0f,
                m43: nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance));
        }

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
            if ((fieldOfView <= 0f) || (fieldOfView >= 3.141593f))
                throw new ArgumentException(
                    nameof(fieldOfView), $"{nameof(fieldOfView)} <= 0 or >= PI ({fieldOfView})");

            if (nearPlaneDistance <= 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(nearPlaneDistance), $"{nameof(nearPlaneDistance)} <= 0 ({nearPlaneDistance})");

            if (farPlaneDistance <= 0f)
                throw new ArgumentException(
                    nameof(farPlaneDistance), $"{nameof(farPlaneDistance)} <= 0 ({farPlaneDistance})");

            if (nearPlaneDistance >= farPlaneDistance)
                throw new ArgumentException(
                    nameof(nearPlaneDistance), $"{nameof(nearPlaneDistance)} >= {nameof(farPlaneDistance)} " +
                    $"({nearPlaneDistance} >= {farPlaneDistance})");

            float num = 1f / (MathF.Tan(fieldOfView * 0.5f));
            float num9 = num / aspectRatio;

            return new Matrix(
                m11: num9,
                m12: 0,
                m13: 0,
                m14: 0,
                m22: num,
                m21: 0,
                m23: 0,
                m24: 0,
                m31: 0,
                m32: 0f,
                m33: farPlaneDistance / (nearPlaneDistance - farPlaneDistance),
                m34: -1,
                m41: 0,
                m42: 0,
                m44: 0,
                m43: nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance));
        }

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
            float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
        {
            if (nearPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nearPlaneDistance), $"{nameof(nearPlaneDistance)} <= 0 ({nearPlaneDistance})");
            }
            if (farPlaneDistance <= 0f)
            {
                throw new ArgumentException(
                    nameof(farPlaneDistance), $"{nameof(farPlaneDistance)} <= 0 ({farPlaneDistance})");
            }
            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentException(
                    nameof(nearPlaneDistance), $"{nameof(nearPlaneDistance)} >= {nameof(farPlaneDistance)} " +
                    $"({nearPlaneDistance} >= {farPlaneDistance})");
            }

            return new Matrix(
                m11: 2f * nearPlaneDistance / (right - left),
                m12: 0,
                m13: 0,
                m14: 0,
                m22: 2f * nearPlaneDistance / (top - bottom),
                m21: 0,
                m23: 0,
                m24: 0,
                m31: (left + right) / (right - left),
                m32: (top + bottom) / (top - bottom),
                m33: farPlaneDistance / (nearPlaneDistance - farPlaneDistance),
                m34: -1,
                m43: nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance),
                m41: 0,
                m42: 0,
                m44: 0);
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized perspective view.
        /// </summary>
        /// <param name="viewingVolume">The viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <returns>The new <see cref="Matrix"/> for customized perspective view.</returns>
        public static Matrix CreatePerspectiveOffCenter(
            RectangleF viewingVolume, float nearPlaneDistance, float farPlaneDistance)
        {
            return CreatePerspectiveOffCenter(
                viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top,
                nearPlaneDistance, farPlaneDistance);
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around X axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around X axis.</returns>
        public static Matrix CreateRotationX(float radians)
        {
            var val1 = MathF.Cos(radians);
            var val2 = MathF.Sin(radians);

            var result = Identity;
            result.M22 = val1;
            result.M23 = val2;
            result.M32 = -val2;
            result.M33 = val1;
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Y axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Y axis.</returns>
        public static Matrix CreateRotationY(float radians)
        {
            var val1 = MathF.Cos(radians);
            var val2 = MathF.Sin(radians);

            var result = Identity;
            result.M11 = val1;
            result.M13 = -val2;
            result.M31 = val2;
            result.M33 = val1;
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Z axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Z axis.</returns>
        public static Matrix CreateRotationZ(float radians)
        {
            var val1 = MathF.Cos(radians);
            var val2 = MathF.Sin(radians);

            var result = Identity;
            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;
            return result;
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scale"><see cref="Vector3"/> representing x,y and z scale values.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(in Vector3 scale)
        {
            return new Matrix(
                m11: scale.X, m12: 0, m13: 0, m14: 0,
                m21: 0, m22: scale.Y, m23: 0, m24: 0,
                m31: 0, m32: 0, m33: scale.Z, m34: 0,
                m41: 0, m42: 0, m43: 0, m44: 1);
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
            return CreateScale(new Vector3(xScale, yScale, zScale));
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scale">Scale value for all three axises.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(float scale)
        {
            return CreateScale(new Vector3(scale));
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that flattens geometry into a specified <see cref="Plane"/> as if casting a shadow from a specified light source. 
        /// </summary>
        /// <param name="lightDirection">A vector specifying the direction from which the light that will cast the shadow is coming.</param>
        /// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
        /// <returns>A <see cref="Matrix"/> that can be used to flatten geometry onto the specified plane from the specified direction. </returns>
        public static Matrix CreateShadow(in Vector3 lightDirection, in Plane plane)
        {
            float dot = (plane.Normal.X * lightDirection.X) + (plane.Normal.Y * lightDirection.Y) + (plane.Normal.Z * lightDirection.Z);
            float x = -plane.Normal.X;
            float y = -plane.Normal.Y;
            float z = -plane.Normal.Z;
            float d = -plane.D;

            return new Matrix(
                m11: (x * lightDirection.X) + dot,
                m12: x * lightDirection.Y,
                m13: x * lightDirection.Z,
                m14: 0,
                m21: y * lightDirection.X,
                m22: (y * lightDirection.Y) + dot,
                m23: y * lightDirection.Z,
                m24: 0,
                m31: z * lightDirection.X,
                m32: z * lightDirection.Y,
                m33: (z * lightDirection.Z) + dot,
                m34: 0,
                m41: d * lightDirection.X,
                m42: d * lightDirection.Y,
                m43: d * lightDirection.Z,
                m44: dot);
        }

        /// <summary>
        /// Creates a new translation <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">X,Y and Z coordinates of translation.</param>
        /// <returns>The translation <see cref="Matrix"/>.</returns>
        public static Matrix CreateTranslation(in Vector3 position)
        {
            return new Matrix(
                m11: 1,
                m12: 0,
                m13: 0,
                m14: 0,
                m21: 0,
                m22: 1,
                m23: 0,
                m24: 0,
                m31: 0,
                m32: 0,
                m33: 1,
                m34: 0,
                m41: position.X,
                m42: position.Y,
                m43: position.Z,
                m44: 1);
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
            return CreateTranslation(new Vector3(xPosition, yPosition, zPosition));
        }

        /// <summary>
        /// Creates a new reflection <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">The plane that used for reflection calculation.</param>
        /// <returns>The reflection <see cref="Matrix"/>.</returns>
        public static Matrix CreateReflection(in Plane value)
        {
            Plane plane = Plane.Normalize(value);
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float num3 = -2f * x;
            float num2 = -2f * y;
            float num = -2f * z;

            return new Matrix(
                m11: (num3 * x) + 1f,
                m12: num2 * x,
                m13: num * x,
                m14: 0,
                m21: num3 * y,
                m22: (num2 * y) + 1,
                m23: num * y,
                m24: 0,
                m31: num3 * z,
                m32: num2 * z,
                m33: (num * z) + 1,
                m34: 0,
                m41: num3 * plane.D,
                m42: num2 * plane.D,
                m43: num * plane.D,
                m44: 1);
        }

        #region CreateWorld

        /// <summary>
        /// Creates a new world <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">The position vector.</param>
        /// <param name="forward">The forward direction vector.</param>
        /// <param name="up">The upward direction vector. Usually <see cref="Vector3.Up"/>.</param>
        /// <param name="result">The world <see cref="Matrix"/>.</param>
        public static void CreateWorld(in Vector3 position, in Vector3 forward, in Vector3 up, out Matrix result)
        {
            Vector3.Cross(forward, up, out var x);
            x.Normalize();
            
            Vector3.Cross(x, forward, out var y);
            y.Normalize();

            Vector3.Normalize(forward, out var z);

            result.M11 = x.X;
            result.M12 = x.Y;
            result.M13 = x.Z;
            result.M14 = 0;

            result.M21 = y.X;
            result.M22 = y.Y;
            result.M23 = y.Z;
            result.M24 = 0;

            result.M31 = -z.X;
            result.M32 = -z.Y;
            result.M33 = -z.Z;
            result.M34 = 0;

            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = 1f;
        }

        /// <summary>
        /// Creates a new world <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">The position vector.</param>
        /// <param name="forward">The forward direction vector.</param>
        /// <param name="up">The upward direction vector. Usually <see cref="Vector3.Up"/>.</param>
        /// <returns>The world <see cref="Matrix"/>.</returns>
        public static Matrix CreateWorld(in Vector3 position, in Vector3 forward, in Vector3 up)
        {
            CreateWorld(position, forward, up, out var result);
            return result;
        }

        #endregion

        /// <summary>
        /// Decomposes this matrix to translation, rotation and scale elements. Returns <see langword="true"/> if matrix can be decomposed; <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="scale">Scale vector as an output parameter.</param>
        /// <param name="rotation">Rotation quaternion as an output parameter.</param>
        /// <param name="translation">Translation vector as an output parameter.</param>
        /// <returns><see langword="true"/> if matrix can be decomposed; <see langword="false"/> otherwise.</returns>
        public bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            translation.X = M41;
            translation.Y = M42;
            translation.Z = M43;

            float xs = (Math.Sign(M11 * M12 * M13 * M14) < 0) ? -1 : 1;
            float ys = (Math.Sign(M21 * M22 * M23 * M24) < 0) ? -1 : 1;
            float zs = (Math.Sign(M31 * M32 * M33 * M34) < 0) ? -1 : 1;

            scale.X = xs * MathF.Sqrt(M11 * M11 + M12 * M12 + M13 * M13);
            scale.Y = ys * MathF.Sqrt(M21 * M21 + M22 * M22 + M23 * M23);
            scale.Z = zs * MathF.Sqrt(M31 * M31 + M32 * M32 + M33 * M33);

            if (scale.X == 0.0 || scale.Y == 0.0 || scale.Z == 0.0)
            {
                rotation = Quaternion.Identity;
                return false;
            }

            var m1 = new Matrix(
                M11 / scale.X, M12 / scale.X, M13 / scale.X, 0,
                M21 / scale.Y, M22 / scale.Y, M23 / scale.Y, 0,
                M31 / scale.Z, M32 / scale.Z, M33 / scale.Z, 0,
                0, 0, 0, 1);

            rotation = Quaternion.CreateFromRotationMatrix(m1);
            return true;
        }

        /// <summary>
        /// Returns a determinant of this <see cref="Matrix"/>.
        /// </summary>
        /// <returns>Determinant of this <see cref="Matrix"/></returns>
        /// <remarks>See more about determinant here - http://en.wikipedia.org/wiki/Determinant.
        /// </remarks>
        public float Determinant()
        {
            float num22 = M11;
            float num21 = M12;
            float num20 = M13;
            float num19 = M14;
            float num12 = M21;
            float num11 = M22;
            float num10 = M23;
            float num9 = M24;
            float num8 = M31;
            float num7 = M32;
            float num6 = M33;
            float num5 = M34;
            float num4 = M41;
            float num3 = M42;
            float num2 = M43;
            float num = M44;
            float num18 = (num6 * num) - (num5 * num2);
            float num17 = (num7 * num) - (num5 * num3);
            float num16 = (num7 * num2) - (num6 * num3);
            float num15 = (num8 * num) - (num5 * num4);
            float num14 = (num8 * num2) - (num6 * num4);
            float num13 = (num8 * num3) - (num7 * num4);

            return
                (num22 * ((num11 * num18) - (num10 * num17) + (num9 * num16))) -
                (num21 * ((num12 * num18) - (num10 * num15) + (num9 * num14))) +
                (num20 * ((num12 * num17) - (num11 * num15) + (num9 * num13))) -
                (num19 * ((num12 * num16) - (num11 * num14) + (num10 * num13)));
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by the elements of another matrix.
        /// </summary>
        /// <param name="left">Source <see cref="Matrix"/>.</param>
        /// <param name="right">Divisor <see cref="Matrix"/>.</param>
        /// <returns>The result of dividing the matrix.</returns>
        public static Matrix Divide(in Matrix left, in Matrix right) => left / right;

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a matrix by a scalar.</returns>
        public static Matrix Divide(in Matrix matrix, float divider) => matrix / divider;

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Matrix"/> without any tolerance.
        /// </summary>
        /// <param name="other">The <see cref="Matrix"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public bool Equals(Matrix other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/> without any tolerance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Matrix other ? this == other : false;
        }

        /// <summary>
        /// Gets the hash code of this <see cref="Matrix"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Matrix"/>.</returns>
        public override int GetHashCode()
        {
            int code = 7 + M11.GetHashCode();
            code = code * 31 + M12.GetHashCode();
            code = code * 31 + M13.GetHashCode();
            code = code * 31 + M14.GetHashCode();
            code = code * 31 + M21.GetHashCode();
            code = code * 31 + M22.GetHashCode();
            code = code * 31 + M23.GetHashCode();
            code = code * 31 + M24.GetHashCode();
            code = code * 31 + M31.GetHashCode();
            code = code * 31 + M32.GetHashCode();
            code = code * 31 + M33.GetHashCode();
            code = code * 31 + M34.GetHashCode();
            code = code * 31 + M41.GetHashCode();
            code = code * 31 + M42.GetHashCode();
            code = code * 31 + M43.GetHashCode();
            return code * 31 + M44.GetHashCode();
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <param name="result">The inverted matrix.</param>
        public static void Invert(in Matrix matrix, out Matrix result)
        {
            float n1 = matrix.M11;
            float n2 = matrix.M12;
            float n3 = matrix.M13;
            float n4 = matrix.M14;
            float n5 = matrix.M21;
            float n6 = matrix.M22;
            float n7 = matrix.M23;
            float n8 = matrix.M24;
            float n9 = matrix.M31;
            float n10 = matrix.M32;
            float n11 = matrix.M33;
            float n12 = matrix.M34;
            float n13 = matrix.M41;
            float n14 = matrix.M42;
            float n15 = matrix.M43;
            float n16 = matrix.M44;
            float n17 = (float)(n11 * (double)n16 - n12 * (double)n15);
            float n18 = (float)(n10 * (double)n16 - n12 * (double)n14);
            float n19 = (float)(n10 * (double)n15 - n11 * (double)n14);
            float n20 = (float)(n9 * (double)n16 - n12 * (double)n13);
            float n21 = (float)(n9 * (double)n15 - n11 * (double)n13);
            float n22 = (float)(n9 * (double)n14 - n10 * (double)n13);
            float n23 = (float)(n6 * (double)n17 - n7 * (double)n18 + n8 * (double)n19);
            float n24 = (float)-(n5 * (double)n17 - n7 * (double)n20 + n8 * (double)n21);
            float n25 = (float)(n5 * (double)n18 - n6 * (double)n20 + n8 * (double)n22);
            float n26 = (float)-(n5 * (double)n19 - n6 * (double)n21 + n7 * (double)n22);
            float n27 = (float)(1.0 / (n1 * (double)n23 + n2 * (double)n24 + n3 * (double)n25 + n4 * (double)n26));
            float n28 = (float)(n7 * (double)n16 - n8 * (double)n15);
            float n29 = (float)(n6 * (double)n16 - n8 * (double)n14);
            float n30 = (float)(n6 * (double)n15 - n7 * (double)n14);
            float n31 = (float)(n5 * (double)n16 - n8 * (double)n13);
            float n32 = (float)(n5 * (double)n15 - n7 * (double)n13);
            float n33 = (float)(n5 * (double)n14 - n6 * (double)n13);
            float n34 = (float)(n7 * (double)n12 - n8 * (double)n11);
            float n35 = (float)(n6 * (double)n12 - n8 * (double)n10);
            float n36 = (float)(n6 * (double)n11 - n7 * (double)n10);
            float n37 = (float)(n5 * (double)n12 - n8 * (double)n9);
            float n38 = (float)(n5 * (double)n11 - n7 * (double)n9);
            float n39 = (float)(n5 * (double)n10 - n6 * (double)n9);

            result.M11 = n23 * n27;
            result.M21 = n24 * n27;
            result.M31 = n25 * n27;
            result.M41 = n26 * n27;
            result.M12 = (float)-(n2 * (double)n17 - n3 * (double)n18 + n4 * (double)n19) * n27;
            result.M22 = (float)(n1 * (double)n17 - n3 * (double)n20 + n4 * (double)n21) * n27;
            result.M32 = (float)-(n1 * (double)n18 - n2 * (double)n20 + n4 * (double)n22) * n27;
            result.M42 = (float)(n1 * (double)n19 - n2 * (double)n21 + n3 * (double)n22) * n27;
            result.M13 = (float)(n2 * (double)n28 - n3 * (double)n29 + n4 * (double)n30) * n27;
            result.M23 = (float)-(n1 * (double)n28 - n3 * (double)n31 + n4 * (double)n32) * n27;
            result.M33 = (float)(n1 * (double)n29 - n2 * (double)n31 + n4 * (double)n33) * n27;
            result.M43 = (float)-(n1 * (double)n30 - n2 * (double)n32 + n3 * (double)n33) * n27;
            result.M14 = (float)-(n2 * (double)n34 - n3 * (double)n35 + n4 * (double)n36) * n27;
            result.M24 = (float)(n1 * (double)n34 - n3 * (double)n37 + n4 * (double)n38) * n27;
            result.M34 = (float)-(n1 * (double)n35 - n2 * (double)n37 + n4 * (double)n39) * n27;
            result.M44 = (float)(n1 * (double)n36 - n2 * (double)n38 + n3 * (double)n39) * n27;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <returns>The inverted matrix.</returns>
        public static Matrix Invert(in Matrix matrix)
        {
            Invert(matrix, out var result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains linear interpolation of the values in specified matrixes.
        /// </summary>
        /// <param name="a">The first <see cref="Matrix"/>.</param>
        /// <param name="b">The second <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>>The result of linear interpolation of the specified matrixes.</returns>
        public static Matrix Lerp(in Matrix a, in Matrix b, float amount)
        {
            return new Matrix(
                a.M11 + (b.M11 - a.M11) * amount,
                a.M12 + (b.M12 - a.M12) * amount,
                a.M13 + (b.M13 - a.M13) * amount,
                a.M14 + (b.M14 - a.M14) * amount,
                a.M21 + (b.M21 - a.M21) * amount,
                a.M22 + (b.M22 - a.M22) * amount,
                a.M23 + (b.M23 - a.M23) * amount,
                a.M24 + (b.M24 - a.M24) * amount,
                a.M31 + (b.M31 - a.M31) * amount,
                a.M32 + (b.M32 - a.M32) * amount,
                a.M33 + (b.M33 - a.M33) * amount,
                a.M34 + (b.M34 - a.M34) * amount,
                a.M41 + (b.M41 - a.M41) * amount,
                a.M42 + (b.M42 - a.M42) * amount,
                a.M43 + (b.M43 - a.M43) * amount,
                a.M44 + (b.M44 - a.M44) * amount);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of two matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/>.</param>
        /// <param name="result">Result of the matrix multiplication.</param>
        public static void Multiply(in Matrix left, in Matrix right, out Matrix result)
        {
            result.M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31) + (left.M14 * right.M41);
            result.M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32) + (left.M14 * right.M42);
            result.M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33) + (left.M14 * right.M43);
            result.M14 = (left.M11 * right.M14) + (left.M12 * right.M24) + (left.M13 * right.M34) + (left.M14 * right.M44);
            result.M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31) + (left.M24 * right.M41);
            result.M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32) + (left.M24 * right.M42);
            result.M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33) + (left.M24 * right.M43);
            result.M24 = (left.M21 * right.M14) + (left.M22 * right.M24) + (left.M23 * right.M34) + (left.M24 * right.M44);
            result.M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31) + (left.M34 * right.M41);
            result.M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32) + (left.M34 * right.M42);
            result.M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33) + (left.M34 * right.M43);
            result.M34 = (left.M31 * right.M14) + (left.M32 * right.M24) + (left.M33 * right.M34) + (left.M34 * right.M44);
            result.M41 = (left.M41 * right.M11) + (left.M42 * right.M21) + (left.M43 * right.M31) + (left.M44 * right.M41);
            result.M42 = (left.M41 * right.M12) + (left.M42 * right.M22) + (left.M43 * right.M32) + (left.M44 * right.M42);
            result.M43 = (left.M41 * right.M13) + (left.M42 * right.M23) + (left.M43 * right.M33) + (left.M44 * right.M43);
            result.M44 = (left.M41 * right.M14) + (left.M42 * right.M24) + (left.M43 * right.M34) + (left.M44 * right.M44);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of two matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/>.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        public static Matrix Multiply(in Matrix left, in Matrix right)
        {
            Multiply(left, right, out var result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of <see cref="Matrix"/> and a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static Matrix Multiply(in Matrix matrix, float scaleFactor) => matrix * scaleFactor;

        /// <summary>
        /// Creates an array copy of the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix">The source <see cref="Matrix"/>.</param>
        /// <returns>The array which matrix values will be stored.</returns>
        /// <remarks>
        /// Required for OpenGL 2.0 projection matrix stuff.
        /// </remarks>
        public static float[] ToFloatArray(in Matrix matrix)
        {
            return new float[]
            {
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
            };
        }

        /// <summary>
        /// Returns a matrix with the all values negated.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <returns>Result of the matrix negation.</returns>
        public static Matrix Negate(in Matrix matrix) => -matrix;

        /// <summary>
        /// Adds two matrixes.
        /// </summary>
        /// <param name="a">Source <see cref="Matrix"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Matrix"/> on the right of the add sign.</param>
        /// <returns>Sum of the matrixes.</returns>
        public static Matrix operator +(in Matrix a, in Matrix b)
        {
            return new Matrix(
                a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13, a.M14 + b.M14,
                a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23, a.M24 + b.M24,
                a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33, a.M34 + b.M34,
                a.M41 + b.M41, a.M42 + b.M42, a.M43 + b.M43, a.M44 + b.M44);
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by the elements of another <see cref="Matrix"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Matrix"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Matrix"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the matrixes.</returns>
        public static Matrix operator /(in Matrix left, in Matrix right)
        {
            return new Matrix(
                left.M11 / right.M11, left.M12 / right.M12, left.M13 / right.M13, left.M14 / right.M14,
                left.M21 / right.M21, left.M22 / right.M22, left.M23 / right.M23, left.M24 / right.M24,
                left.M31 / right.M31, left.M32 / right.M32, left.M33 / right.M33, left.M34 / right.M34,
                left.M41 / right.M41, left.M42 / right.M42, left.M43 / right.M43, left.M44 / right.M44);
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a matrix by a scalar.</returns>
        public static Matrix operator /(in Matrix matrix, float divider)
        {
            float num = 1f / divider;
            return new Matrix(
                matrix.M11 * num, matrix.M12 * num, matrix.M13 * num, matrix.M14 * num,
                matrix.M21 * num, matrix.M22 * num, matrix.M23 * num, matrix.M24 * num,
                matrix.M31 * num, matrix.M32 * num, matrix.M33 * num, matrix.M34 * num,
                matrix.M41 * num, matrix.M42 * num, matrix.M43 * num, matrix.M44 * num);
        }

        /// <summary>
        /// Compares whether two <see cref="Matrix"/> instances are equal without any tolerance.
        /// </summary>
        /// <param name="a">Source <see cref="Matrix"/> on the left of the equal sign.</param>
        /// <param name="b">Source <see cref="Matrix"/> on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Matrix a, in Matrix b)
        {
            return a.M11 == b.M11 && a.M12 == b.M12 && a.M13 == b.M13 && a.M14 == b.M14
                && a.M21 == b.M21 && a.M22 == b.M22 && a.M23 == b.M23 && a.M24 == b.M24
                && a.M31 == b.M31 && a.M32 == b.M32 && a.M33 == b.M33 && a.M34 == b.M34
                && a.M41 == b.M41 && a.M42 == b.M42 && a.M43 == b.M43 && a.M44 == b.M44;
        }

        /// <summary>
        /// Compares whether two <see cref="Matrix"/> instances are not equal without any tolerance.
        /// </summary>
        /// <param name="a">Source <see cref="Matrix"/> on the left of the not equal sign.</param>
        /// <param name="b">Source <see cref="Matrix"/> on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>
        public static bool operator !=(in Matrix a, in Matrix b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Multiplies two matrixes.
        /// </summary>
        /// <param name="left">Source <see cref="Matrix"/> on the left of the mul sign.</param>
        /// <param name="right">Source <see cref="Matrix"/> on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        /// <remarks>
        /// Using matrix multiplication algorithm - see http://en.wikipedia.org/wiki/Matrix_multiplication.
        /// </remarks>
        public static Matrix operator *(in Matrix left, in Matrix right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        /// Multiplies the elements of matrix by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static Matrix operator *(in Matrix matrix, float scaleFactor)
        {
            return new Matrix(
                matrix.M11 * scaleFactor, matrix.M12 * scaleFactor, matrix.M13 * scaleFactor, matrix.M14 * scaleFactor,
                matrix.M21 * scaleFactor, matrix.M22 * scaleFactor, matrix.M23 * scaleFactor, matrix.M24 * scaleFactor,
                matrix.M31 * scaleFactor, matrix.M32 * scaleFactor, matrix.M33 * scaleFactor, matrix.M34 * scaleFactor,
                matrix.M41 * scaleFactor, matrix.M42 * scaleFactor, matrix.M43 * scaleFactor, matrix.M44 * scaleFactor);
        }

        /// <summary>
        /// Subtracts the values of one <see cref="Matrix"/> from another <see cref="Matrix"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Matrix"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Matrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the matrix subtraction.</returns>
        public static Matrix operator -(in Matrix left, in Matrix right)
        {
            return new Matrix(
                left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13, left.M14 - right.M14,
                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23, left.M24 - right.M24,
                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33, left.M34 - right.M34,
                left.M41 - right.M41, left.M42 - right.M42, left.M43 - right.M43, left.M44 - right.M44);
        }

        /// <summary>
        /// Negates values in the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Matrix operator -(in Matrix matrix)
        {
            return new Matrix(
                -matrix.M11, -matrix.M12, -matrix.M13, -matrix.M14,
                -matrix.M21, -matrix.M22, -matrix.M23, -matrix.M24,
                -matrix.M31, -matrix.M32, -matrix.M33, -matrix.M34,
                -matrix.M41, -matrix.M42, -matrix.M43, -matrix.M44);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains subtraction of one matrix from another.
        /// </summary>
        /// <param name="left">The first <see cref="Matrix"/>.</param>
        /// <param name="right">The second <see cref="Matrix"/>.</param>
        /// <returns>The result of the matrix subtraction.</returns>
        public static Matrix Subtract(in Matrix left, in Matrix right) => left - right;

        internal string DebugDisplayString
        {
            get
            {
                if (this == Identity)
                    return "Identity";

                return string.Concat(
                     "( ", M11.ToString(), "  ", M12.ToString(), "  ", M13.ToString(), "  ", M14.ToString(), " )  \r\n",
                     "( ", M21.ToString(), "  ", M22.ToString(), "  ", M23.ToString(), "  ", M24.ToString(), " )  \r\n",
                     "( ", M31.ToString(), "  ", M32.ToString(), "  ", M33.ToString(), "  ", M34.ToString(), " )  \r\n",
                     "( ", M41.ToString(), "  ", M42.ToString(), "  ", M43.ToString(), "  ", M44.ToString(), " )");
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Matrix"/> in the format:
        /// {M11:[<see cref="M11"/>] M12:[<see cref="M12"/>] M13:[<see cref="M13"/>] M14:[<see cref="M14"/>]}
        /// {M21:[<see cref="M21"/>] M12:[<see cref="M22"/>] M13:[<see cref="M23"/>] M14:[<see cref="M24"/>]}
        /// {M31:[<see cref="M31"/>] M32:[<see cref="M32"/>] M33:[<see cref="M33"/>] M34:[<see cref="M34"/>]}
        /// {M41:[<see cref="M41"/>] M42:[<see cref="M42"/>] M43:[<see cref="M43"/>] M44:[<see cref="M44"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Matrix"/>.</returns>
        public readonly override string ToString()
        {
            return "{M11:" + M11 + " M12:" + M12 + " M13:" + M13 + " M14:" + M14 + "}"
                + " {M21:" + M21 + " M22:" + M22 + " M23:" + M23 + " M24:" + M24 + "}"
                + " {M31:" + M31 + " M32:" + M32 + " M33:" + M33 + " M34:" + M34 + "}"
                + " {M41:" + M41 + " M42:" + M42 + " M43:" + M43 + " M44:" + M44 + "}";
        }

        /// <summary>
        /// Swap the matrix rows and columns.
        /// </summary>
        /// <param name="matrix">The matrix for transposing operation.</param>
        /// <returns>The new <see cref="Matrix"/> which contains the transposed result.</returns>
        public static Matrix Transpose(in Matrix matrix)
        {
            return new Matrix(
                m11: matrix.M11, m12: matrix.M21, m13: matrix.M31, m14: matrix.M41,
                m21: matrix.M12, m22: matrix.M22, m23: matrix.M32, m24: matrix.M42,
                m31: matrix.M13, m32: matrix.M23, m33: matrix.M33, m34: matrix.M43,
                m41: matrix.M14, m42: matrix.M24, m43: matrix.M34, m44: matrix.M44);
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
    }
}
