// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Vector
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
        /// Should be retrieved through <see cref="VectorTypeInfo"/>.
        /// </summary>
        // TODO: replace with C#9 shape and a required static field
        VectorComponentInfo ComponentInfo { get; }

        /// <summary>
        /// Sets the vector from a scaled <see cref="Vector3"/>
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        void FromScaledVector3(Vector3 scaledVector);

        /// <summary>
        /// Gets the vector as a scaled <see cref="Vector3"/>
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        Vector3 ToScaledVector3();

        /// <summary>
        /// Sets the vector from a scaled <see cref="Vector4"/>
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        void FromScaledVector4(Vector4 scaledVector);

        /// <summary>
        /// Gets the vector as a scaled <see cref="Vector4"/>
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        Vector4 ToScaledVector4();

        /// <summary>
        /// Sets the vector from a <see cref="Vector3"/> 
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        void FromVector3(Vector3 vector) => FromScaledVector3(vector);

        /// <summary>
        /// Gets the vector as a <see cref="Vector3"/>
        /// where the XYZ components correspond to RGB. 
        /// </summary>
        Vector3 ToVector3() => ToScaledVector3();

        /// <summary>
        /// Sets the vector from a <see cref="Vector4"/> 
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        void FromVector4(Vector4 vector) => FromScaledVector4(vector);

        /// <summary>
        /// Gets the vector as a <see cref="Vector4"/>
        /// where the XYZW components correspond to RGBA. 
        /// </summary>
        Vector4 ToVector4() => ToScaledVector4();
    }

    public interface IVector<TSelf> : IVector, IEquatable<TSelf>
        where TSelf : IVector<TSelf>
    {
    }
}
