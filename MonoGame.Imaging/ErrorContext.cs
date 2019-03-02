using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MonoGame.Imaging
{
    public class ErrorContext
    {
        private IList<ImagingError> _errors;

        public ReadOnlyCollection<ImagingError> Errors { get; }
        public int Count => _errors.Count;

        public ErrorContext(IList<ImagingError> items)
        {
            _errors = items;
            Errors = new ReadOnlyCollection<ImagingError>(_errors);
        }

        public ErrorContext() : this(new List<ImagingError>())
        {
        }

        public void Clear()
        {
            _errors.Clear();
        }

        internal int Error(ImagingError error)
        {
            AddError(error);
            return 0;
        }

        internal void AddError(ImagingError error)
        {
            _errors.Add(error);
        }

        internal void RemoveError(ImagingError error)
        {
            _errors.Remove(error);
        }

        public override string ToString()
        {
            int max = Count;
            if (max > 10)
                max = 10;

            var builder = new StringBuilder(max * 10 + 5);
            for (int i = 0; i < max; i++)
            {
                builder.AppendLine(Errors[i].ToString());
                builder.Append(' ');
            }

            if (Count > max)
                builder.AppendLine($" and {Count - max} more...");

            return builder.ToString();
        }
    }
}
