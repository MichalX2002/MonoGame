// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Numerics;
using System.Xml;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Intermediate
{
    [ContentTypeSerializer]
    class Matrix4x4Serializer : ElementSerializer<Matrix4x4>
    {
        public Matrix4x4Serializer() : base("Matrix4x4", 16)
        {
        }

        protected internal override Matrix4x4 Deserialize(string[] inputs, ref int index)
        {
            return new Matrix4x4(
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]),
                XmlConvert.ToSingle(inputs[index++]));
        }

        protected internal override void Serialize(Matrix4x4 value, List<string> results)
        {
            results.Add(XmlConvert.ToString(value.M11));
            results.Add(XmlConvert.ToString(value.M12));
            results.Add(XmlConvert.ToString(value.M13));
            results.Add(XmlConvert.ToString(value.M14));
            results.Add(XmlConvert.ToString(value.M21));
            results.Add(XmlConvert.ToString(value.M22));
            results.Add(XmlConvert.ToString(value.M23));
            results.Add(XmlConvert.ToString(value.M24));
            results.Add(XmlConvert.ToString(value.M31));
            results.Add(XmlConvert.ToString(value.M32));
            results.Add(XmlConvert.ToString(value.M33));
            results.Add(XmlConvert.ToString(value.M34));
            results.Add(XmlConvert.ToString(value.M41));
            results.Add(XmlConvert.ToString(value.M42));
            results.Add(XmlConvert.ToString(value.M43));
            results.Add(XmlConvert.ToString(value.M44));
        }
    }
}