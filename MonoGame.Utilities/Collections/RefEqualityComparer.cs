
using System;

namespace MonoGame.Utilities.Collections
{
    public static class RefEqualityComparer<T>
    {
        private static IRefEqualityComparer<T> _default;

        public static IRefEqualityComparer<T> Default
        {
            get
            {
                if(_default == null)
                {
                    Type type = typeof(T);
                    if (type is IRefEquatable<T>)
                        _default = new InterfacedComparer();
                    else if (type == typeof(IntPtr) || type == typeof(UIntPtr))
                        _default = new SpecialComparer();
                    else
                        _default = new DefaultComparer();
                }
                return _default;
            }
        }
        
        private struct InterfacedComparer : IRefEqualityComparer<T>
        {
            public bool EqualsByRef(in T obj1, in T obj2)
            {
                return ((IRefEquatable<T>)obj1).EqualsByRef(obj2);
            }

            public long GetLongHashCode(in T obj)
            {
                return ((IRefEquatable<T>)obj).GetLongHashCode();
            }
        }

        private struct DefaultComparer : IRefEqualityComparer<T>
        {
            public bool EqualsByRef(in T obj1, in T obj2)
            {
                return obj1.Equals(obj2);
            }

            public long GetLongHashCode(in T obj)
            {
                return obj.GetHashCode();
            }
        }

        private struct SpecialComparer : IRefEqualityComparer<T>
        {
            public bool EqualsByRef(in T obj1, in T obj2)
            {
                return obj1.Equals(obj2);
            }

            public long GetLongHashCode(in T obj)
            {
                switch (obj)
                {
                    case IntPtr ptr:
                        return ptr.ToInt64();

                    case UIntPtr ptr:
                        return (long)ptr.ToUInt64();

                    default:
                        throw new InvalidCastException();
                }
            }
        }
    }
}
