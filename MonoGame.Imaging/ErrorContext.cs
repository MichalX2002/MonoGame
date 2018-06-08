using System.Collections.Generic;
using System.Text;

namespace MonoGame.Imaging
{
    public class ErrorContext
    {
        public IList<string> Errors { get; private set; }
        public int Count => Errors.Count;

        public ErrorContext()
        {
            Errors = new List<string>();
        }

        public ErrorContext(IList<string> items)
        {
            Errors = items;
        }

        public void Clear()
        {
            Errors.Clear();
        }

        internal int Error(string error)
        {
            AddError(error);
            return 0;
        }

        internal void AddError(string error)
        {
            Errors.Add(error);
        }

        public override string ToString()
        {
            int max = Count;
            if (max > 10)
                max = 10;

            var builder = new StringBuilder(max * 10 + 5);
            for (int i = 0; i < max; i++)
            {
                builder.AppendLine(Errors[i]);
                builder.Append(' ');
            }

            if (Count > 10)
                builder.AppendLine($" and {Count - max} more...");

            return builder.ToString();
        }
    }
}
