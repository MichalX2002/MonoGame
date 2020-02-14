using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace MonoGame.Framework.Content
{
    public static class ContentExtensions
    {
        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            return type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, 
                null, types: Array.Empty<Type>(), null);
        }

        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            // Sometimes, overridden properties of abstract classes can show up even
            // with BindingFlags.DeclaredOnly is passed to GetProperties. 
            // Make sure that all properties in this list are defined in this class 
            // by comparing its get method with that of it's base class. 
            // If they're the same then it's an overridden property.

            var nonStaticPropertyInfos = 
                from p in type.GetTypeInfo().DeclaredProperties
                where
                p.GetMethod != null &&
                !p.GetMethod.IsStatic &&
                p.GetMethod == p.GetMethod.GetRuntimeBaseDefinition()
                select p;

            return nonStaticPropertyInfos.ToArray();
        }

        public static FieldInfo[] GetAllFields(this Type type)
        {
            return type.GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);
        }
    }
}
