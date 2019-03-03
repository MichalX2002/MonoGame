using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MonoGame.Imaging
{
    public class ErrorContext
    {
        private List<Item> _errors;

        public ReadOnlyCollection<Item> Errors { get; }
        public int Count => _errors.Count;

        public ErrorContext(List<Item> items)
        {
            _errors = items;
            Errors = new ReadOnlyCollection<Item>(_errors);
        }

        public ErrorContext() : this(new List<Item>())
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
            _errors.Add(new Item(error));
        }

        internal void AddError(ImagingError error, Exception exception)
        {
            _errors.Add(new Item(error, exception));
        }

        internal void RemoveError(ImagingError error)
        {
            for (int i = _errors.Count; i-- > 0;)
            {
                if (_errors[i].Error == error)
                    _errors.RemoveAt(i);
            }
        }

        public override string ToString()
        {
            const int maxErrorEntries = 20;

            int max = Count;
            if (max > maxErrorEntries)
                max = maxErrorEntries;

            var builder = new StringBuilder(max * 10 + 10);
            for (int i = 0; i < max; i++)
            {
                Item error = _errors[i];
                if (error.Exception != null)
                {
                    builder.Append(error.Error.ToString());
                    builder.Append(": ");
                    builder.AppendLine(error.Exception.ToString());
                }
                else
                    builder.AppendLine(error.Error.ToString());

                builder.Append(' ');
            }

            if (Count > max)
                builder.AppendLine($" and {Count - max} more...");

            return builder.ToString();
        }

        public struct Item
        {
            public ImagingError Error { get; }
            public Exception Exception { get; }

            public Item(ImagingError error, Exception exception)
            {
                Error = error;
                Exception = exception;
            }

            public Item(ImagingError error) : this(error, null)
            {
            }
        }
    }
}
