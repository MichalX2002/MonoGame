// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;

namespace MonoGame.Framework.Vectors
{
    // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.graphics.packedvector.ipackedvector.aspx
    /// <summary>
    /// Allows vectors to be converted to and from a <see cref="Vector4"/>
    /// representation with values scaled between 0 and 1.
    /// </summary>
    public interface IVector
    {
        /// <summary>
        /// Describes the components of this vector.
        /// Should be retrieved through <see cref="VectorType"/>.
        /// </summary>
        // TODO: replace with static field (C# 9 or 10 role/shape)
        VectorComponentInfo ComponentInfo { get; }

        #region Scaled value-space

        /// <summary>
        /// Sets the vector from a scaled <see cref="Vector3"/>
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        void FromScaledVector(Vector3 scaledVector);

        /// <summary>
        /// Sets the vector from a scaled <see cref="Vector4"/>
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        void FromScaledVector(Vector4 scaledVector);

        /// <summary>
        /// Gets the vector as a scaled <see cref="Vector3"/>
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        Vector3 ToScaledVector3();

        /// <summary>
        /// Gets the vector as a scaled <see cref="Vector4"/>
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        Vector4 ToScaledVector4();

        #endregion

        #region Custom value-space

        /// <summary>
        /// Sets the vector from a <see cref="Vector3"/> 
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        void FromVector(Vector3 vector) => FromScaledVector(vector);

        /// <summary>
        /// Sets the vector from a <see cref="Vector4"/> 
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        void FromVector(Vector4 vector) => FromScaledVector(vector);

        /// <summary>
        /// Gets the vector as a <see cref="Vector3"/>
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        Vector3 ToVector3();

        /// <summary>
        /// Gets the vector as a <see cref="Vector4"/>
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        Vector4 ToVector4();

        #endregion
    }

    public interface IVector<TSelf> : IVector, IEquatable<TSelf>
        where TSelf : IVector<TSelf>
    {
    }
}
