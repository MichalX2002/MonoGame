using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content
{
    internal static class ContentExtensions
    {
        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            return type.GetConstructor(flags, binder: null, types: Array.Empty<Type>(), modifiers: null);
        }

        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            // Sometimes, overridden properties of abstract classes can show up even with 
            // BindingFlags.DeclaredOnly is passed to GetProperties. Make sure that
            // all properties in this list are defined in this class by comparing
            // its get method with that of it's base class. If they're the same
            // Then it's an overridden property.

            IEnumerable<PropertyInfo> infos = type.GetTypeInfo().DeclaredProperties;
            var nonStaticPropertyInfos = from p in infos where 
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

        public static bool IsClass(this Type type)
        {
            return type.IsClass;
        }

        public static bool IsClass<T>()
        {
            return IsClass(typeof(T));
        }
    }
}
