// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content
{
    class MatrixReader : ContentTypeReader<Matrix>
    {
        protected internal override Matrix Read(ContentReader input, Matrix existingInstance)
        {
            float m11 = input.ReadSingle();
            float m12 = input.ReadSingle();
            float m13 = input.ReadSingle();
            float m14 = input.ReadSingle();
            float m21 = input.ReadSingle();
            float m22 = input.ReadSingle();
            float m23 = input.ReadSingle();
            float m24 = input.ReadSingle();
            float m31 = input.ReadSingle();
            float m32 = input.ReadSingle();
            float m33 = input.ReadSingle();
            float m34 = input.ReadSingle();
            float m41 = input.ReadSingle();
            float m42 = input.ReadSingle();
            float m43 = input.ReadSingle();
            float m44 = input.ReadSingle();
            return new Matrix(
                m11, m12, m13, m14,
                m21, m22, m23, m24, 
                m31, m32, m33, m34, 
                m41, m42, m43, m44);
        }
    }
}
