using System;

namespace Microsoft.Xna.Framework
{
    public readonly struct TaskbarProgressValue : IEquatable<TaskbarProgressValue>
    {
        public long Completed { get; }
        public long Total { get; }

        public TaskbarProgressValue(long completed, long total)
        {
            if (completed < 0)
                throw new ArgumentOutOfRangeException(nameof(completed));

            if (total < 0)
                throw new ArgumentOutOfRangeException(nameof(total));

            Completed = completed;
            Total = total;
        }

        public bool Equals(TaskbarProgressValue other)
        {
            return Completed == other.Completed 
                && Total == other.Total;
        }
    }
}
