﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace MonoGame.Framework.Design
{
    internal static class VectorConversion
    {
        public static bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(float))
                return true;
            if (destinationType == typeof(Vector2))
                return true;
            if (destinationType == typeof(Vector3))
                return true;
            if (destinationType == typeof(Vector4))
                return true;
            return false;
        }

        public static object ConvertToFromVector4(
            ITypeDescriptorContext context, CultureInfo culture, Vector4 value, Type destinationType)
        {
            if (destinationType == typeof(float))
                return value.X;
            if (destinationType == typeof(Vector2))
                return new Vector2(value.X, value.Y);
            if (destinationType == typeof(Vector3))
                return new Vector3(value.X, value.Y, value.Z);
            if (destinationType == typeof(Vector4))
                return new Vector4(value.X, value.Y, value.Z, value.W);
            return null;
        }         
    }
}
