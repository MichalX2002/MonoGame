﻿using System;

namespace MonoGame.Framework.Utilities
{
    /// <summary>
    /// Represents the progress ratio for a taskbar progress indicator.
    /// </summary>
    public readonly struct TaskbarProgressValue : IEquatable<TaskbarProgressValue>
    {
        /// <summary>
        /// Gets the value that indicates the proportion of the operation that has been completed.
        /// </summary>
        public long Completed { get; }

        /// <summary>
        /// Gets the value that specifies when the operation is complete.
        /// </summary>
        public long Total { get; }

        /// <summary>
        /// Constructs the <see cref="TaskbarProgressValue"/>.
        /// </summary>
        /// <param name="completed">Indicates the proportion of the operation that has been completed.</param>
        /// <param name="total">Specifies when the operation is complete.</param>
        public TaskbarProgressValue(long completed, long total)
        {
            ArgumentGuard.AssertAtLeastZero(completed, nameof(completed), false);
            ArgumentGuard.AssertAtLeastZero(total, nameof(total), false);

            Completed = completed;
            Total = total;
        }

        /// <summary>
        /// Determines whether the specified value is equal to the current value.
        /// </summary>
        public bool Equals(TaskbarProgressValue other)
        {
            return this == other;
        }

        /// <summary>
        /// Determines whether the specified value is equal to the current value.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is TaskbarProgressValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Completed, Total);
        }

        public static bool operator ==(TaskbarProgressValue a, TaskbarProgressValue b)
        {
            return a.Completed == b.Completed
                && a.Total == b.Total;
        }

        public static bool operator !=(TaskbarProgressValue a, TaskbarProgressValue b)
        {
            return !(a == b);
        }
    }
}
