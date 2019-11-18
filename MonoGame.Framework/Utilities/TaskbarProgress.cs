using System;
using MonoGame.Utilities;

namespace MonoGame.Framework
{
    public readonly struct TaskbarProgressValue : IEquatable<TaskbarProgressValue>
    {
        public long Completed { get; }
        public long Total { get; }

        public TaskbarProgressValue(long completed, long total)
        {
            CommonArgumentGuard.AssertAtleastZero(completed, nameof(completed), false);
            CommonArgumentGuard.AssertAtleastZero(total, nameof(total), false);

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
