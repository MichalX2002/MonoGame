// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Reflection;

namespace MonoGame.Utilities
{
    internal static partial class ReflectionHelpers
    {
        public static bool IsValueType(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            return targetType.IsValueType;
        }

        public static bool IsValueType<T>()
        {
            return IsValueType(typeof(T));
        }

        public static Type GetBaseType(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            return targetType.BaseType;
        }

        /// <summary>
        /// Returns the Assembly of a Type
        /// </summary>
        public static Assembly GetAssembly(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            return targetType.Assembly;
        }

        /// <summary>
        /// Returns true if the given type represents a non-object type that is not abstract.
        /// </summary>
        public static bool IsConcreteClass(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(object))
                return false;

            if (type.IsClass && !type.IsAbstract)
                return true;

            return false;
        }

        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            return type.GetTypeInfo().GetDeclaredMethod(methodName);
        }

        public static MethodInfo GetPropertyGetMethod(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            return property.GetMethod;
        }

        public static MethodInfo GetPropertySetMethod(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            return property.SetMethod;
        }

        public static T GetCustomAttribute<T>(MemberInfo member) where T : Attribute
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            return member.GetCustomAttribute(typeof(T)) as T;
        }

        /// <summary>
        /// Returns true if the get method of the given property exist and are public.
        /// Note that we allow a getter-only property to be serialized (and deserialized),
        /// *if* CanDeserializeIntoExistingObject is true for the property type.
        /// </summary>
        public static bool PropertyIsPublic(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var getMethod = GetPropertyGetMethod(property);
            if (getMethod == null || !getMethod.IsPublic)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if the given type can be assigned the given value
        /// </summary>
        public static bool IsAssignableFrom(Type type, object value)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return IsAssignableFromType(type, value.GetType());
        }

        /// <summary>
        /// Returns true if the given type can be assigned a value with the given object type
        /// </summary>
        public static bool IsAssignableFromType(Type type, Type objectType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            if (type.IsAssignableFrom(objectType))
                return true;

            return false;
        }
    }
}
