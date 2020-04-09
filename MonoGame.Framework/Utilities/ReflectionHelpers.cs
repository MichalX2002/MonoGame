// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Reflection;

namespace MonoGame.Framework.Utilities
{
    public static partial class ReflectionHelpers
    {
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

        /// <summary>
        /// Returns true if the get method of the given property exist and are public.
        /// Note that we allow a getter-only property to be serialized (and deserialized),
        /// *if* CanDeserializeIntoExistingObject is true for the property type.
        /// </summary>
        public static bool PropertyIsPublic(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var getMethod = property.GetMethod;
            if (getMethod == null || !getMethod.IsPublic)
                return false;

            return true;
        }
    }
}
