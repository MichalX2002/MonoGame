// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.graphics.packedvector.ipackedvector.aspx
    /// <summary>
    /// Allows vectors to be converted to and from a <see cref="Vector4"/> representation
    /// with values scaled and clamped between <value>0</value> and <value>1</value>.
    /// </summary>
    public interface IPackedVector
    {
        void FromVector4(Vector4 vector);

        Vector4 ToVector4();
    }

    // http://msdn.microsoft.com/en-us/library/bb197661.aspx
    public interface IPackedVector<TPacked> : IPackedVector
    {
        TPacked PackedValue { get; set; }
    }
}
