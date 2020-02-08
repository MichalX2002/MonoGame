using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging.Utilities
{
    public static class ImagingReflectionHelper
    {
        public static MethodInfo SpanCastMethod { get; }

        static ImagingReflectionHelper()
        {
            SpanCastMethod = typeof(MemoryMarshal).GetMember(
                nameof(MemoryMarshal.Cast),
                MemberTypes.Method,
                BindingFlags.Public | BindingFlags.Static)
                .Cast<MethodInfo>()
                .First(x => x.ReturnType.GetGenericTypeDefinition() == typeof(ReadOnlySpan<>));
        }
    }
}
