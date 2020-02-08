using System;
using System.Reflection;

namespace MonoGame.Framework
{
    public static class ReflectionUtils
    {
        public static ParameterInfo[] GetDelegateParameters(this Type delegateType)
        {
            if (!typeof(Delegate).IsAssignableFrom(delegateType))
                throw new ArgumentException("The type is not a delegate.", nameof(delegateType));

            // Simple trick to get delegate arguments.
            var invokeMethod = delegateType.GetMethod("Invoke");

            var methodParams = invokeMethod.GetParameters();
            return methodParams;
        }

        public static ParameterInfo[] GetParameters(this Delegate @delegate)
        {
            var type = @delegate.GetType();
            return GetDelegateParameters(type);
        }
    }
}
