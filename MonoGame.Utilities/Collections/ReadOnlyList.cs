using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Framework.Collections
{
    public class ReadOnlyList<T> : ReadOnlyCollection<T>
    {
        protected new List<T> Items => (List<T>)base.Items;

        public ReadOnlyList(List<T> list) : base(list)
        {
        }

        public new List<T>.Enumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
