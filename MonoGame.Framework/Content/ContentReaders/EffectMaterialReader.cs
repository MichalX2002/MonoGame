// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework.Content
{
    internal class EffectMaterialReader : ContentTypeReader<EffectMaterial>
    {
        protected internal override EffectMaterial Read(ContentReader input, EffectMaterial existingInstance)
        {
            var effect = input.ReadExternalReference<Effect>();
            var effectMaterial = new EffectMaterial(effect);

            var dict = input.ReadObject<Dictionary<string, object>>();

            foreach (KeyValuePair<string, object> item in dict)
            {
                var parameter = effectMaterial.Parameters[item.Key];
                if (parameter != null)
                {
                    Type itemType = item.Value.GetType();

                    if (typeof(Texture).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Texture)item.Value);
                    }
                    else if (typeof(int).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((int)item.Value);
                    }
                    else if (typeof(bool).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((bool)item.Value);
                    }
                    else if (typeof(float).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((float)item.Value);
                    }
                    else if (typeof(float[]).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((float[])item.Value);
                    }
                    else if (typeof(Vector2).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Vector2)item.Value);
                    }
                    else if (typeof(Vector2[]).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Vector2[])item.Value);
                    }
                    else if (typeof(Vector3).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Vector3)item.Value);
                    }
                    else if (typeof(Vector3[]).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Vector3[])item.Value);
                    }
                    else if (typeof(Vector4).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Vector4)item.Value);
                    }
                    else if (typeof(Vector4[]).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Vector4[])item.Value);
                    }
                    else if (typeof(Matrix4x4).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Matrix4x4)item.Value);
                    }
                    else if (typeof(Matrix4x4[]).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Matrix4x4[])item.Value);
                    }
                    else if (typeof(Quaternion).IsAssignableFrom(itemType))
                    {
                        parameter.SetValue((Quaternion)item.Value);
                    }
                    else
                    {
                        throw new NotSupportedException("Parameter type is not supported");
                    }
                }
                else
                {
                    Debug.WriteLine("No parameter " + item.Key);
                }
            }

            return effectMaterial;
        }
    }
}
