using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Imaging
{
    public class ErrorContext
    {
        public IList<ImagingError> Errors { get; private set; }
        public int Count => Errors.Count;

        public ErrorContext()
        {
            Errors = new List<ImagingError>();
        }

        public ErrorContext(IList<ImagingError> items)
        {
            Errors = items;
        }

        public void Clear()
        {
            Errors.Clear();
        }

        internal int Error(ImagingError error)
        {
            AddError(error);
            return 0;
        }

        internal void AddError(ImagingError error)
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
                builder.AppendLine(Enum.GetName(typeof(ImagingError), Errors[i]));
                builder.Append(' ');
            }

            if (Count > max)
                builder.AppendLine($" and {Count - max} more...");

            return builder.ToString();
        }
    }
}
