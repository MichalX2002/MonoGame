using System;
using System.Linq;
using System.Reflection;

namespace MonoGame.Framework
{
    public static class ReflectionExtensions
    {
        public static ParameterInfo[] GetDelegateParameters(this Type delegateType)
        {
            if (delegateType == null)
                throw new ArgumentNullException(nameof(delegateType));

            if (!typeof(Delegate).IsAssignableFrom(delegateType))
                throw new ArgumentException("The type is not a delegate.", nameof(delegateType));

            // Simple trick to get delegate arguments.
            var invokeMethod = delegateType.GetMethod("Invoke")!;

            var methodParams = invokeMethod.GetParameters();
            return methodParams;
        }

        public static ParameterInfo[] GetParameters(this Delegate @delegate)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));

            var type = @delegate.GetType();
            return GetDelegateParameters(type);
        }

        public static Type[] AsTypes(this ParameterInfo[] parameters)
        {
            return parameters.Select(x => x.ParameterType).ToArray();
        }

        public static Type[] GetGenericTypeDefinitions(this ParameterInfo[] parameters)
        {
            return parameters
                .Select(x =>
                {
                    var type = x.ParameterType;
                    if (type.IsGenericType)
                        return type.GetGenericTypeDefinition();
                    return type;
                })
                .ToArray();
        }

        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method)
            where TDelegate : Delegate
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return (TDelegate)method.CreateDelegate(typeof(TDelegate));
        }

        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method, object target)
            where TDelegate : Delegate
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return (TDelegate)method.CreateDelegate(typeof(TDelegate), target);
        }
    }
}
