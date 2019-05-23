// ==++==
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// ==--==
/*============================================================
** Class:  ArraySortHelper
** <OWNER>Microsoft</OWNER>
** Purpose: class to sort arrays
===========================================================*/

using System.Diagnostics;

namespace System.Collections.Generic
{
    public static unsafe class UnmanagedSort
    {
        public static void Sort<TKey, TValue>(
            TKey* keys, TValue* values, int index, int length, IComparer<TKey> comparer)
            where TKey : unmanaged
            where TValue : unmanaged
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            ArraySortHelper<TKey, TValue>.IntrospectiveSort(keys, values, index, index + length - 1, comparer);
        }

        public static void Sort<TKey, TValue>(
            TKey* keys, TValue* values, int index, int length)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : unmanaged
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            GenericArraySortHelper<TKey, TValue>.IntrospectiveSort(keys, values, index, index + length - 1);
        }
    }

    internal static class IntrospectiveSortUtilities
    {
        // This is the threshold where Introspective sort switches to Insertion sort.
        // Imperically, 16 seems to speed up most cases without slowing down others, at least for integers.
        // Large value types may benefit from a smaller number.
        public const int IntrosortSizeThreshold = 16;

        public const int QuickSortDepthThreshold = 32;

        public static int FloorLog2(int n)
        {
            int result = 0;
            while (n >= 1)
            {
                result++;
                n /= 2;
            }
            return result;
        }

        [DebuggerHidden]
        public static void ThrowOrIgnoreBadComparer()
        {
            throw new ArgumentException("Bad comparer implementation.", "comparer");
        }
    }

    internal unsafe class ArraySortHelper<TKey, TValue> 
        where TKey : unmanaged
        where TValue : unmanaged
    {
        public void Sort(TKey* keys, TValue* values, int index, int length, IComparer<TKey> comparer)
        {
            //Contract.Assert(keys != null, "Check the arguments in the caller!");  // Precondition on interface method
            //Contract.Assert(index >= 0 && length >= 0 && (keys.Length - index >= length), "Check the arguments in the caller!");

            // Add a try block here to detect IComparers (or their
            // underlying IComparables, etc) that are bogus.
            try
            {
                if (comparer == null || comparer == Comparer<TKey>.Default)
                    comparer = Comparer<TKey>.Default;

                IntrospectiveSort(keys, values, index, length, comparer);
            }
            catch (IndexOutOfRangeException)
            {
                IntrospectiveSortUtilities.ThrowOrIgnoreBadComparer();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Comparer failed.", e);
            }
        }

        private static void SwapIfGreaterWithItems(
            TKey* keys, TValue* values, IComparer<TKey> comparer, int a, int b)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values == null || values.Length >= keys.Length);
            //Contract.Requires(comparer != null);
            //Contract.Requires(0 <= a && a < keys.Length);
            //Contract.Requires(0 <= b && b < keys.Length);

            if (a != b)
            {
                if (comparer.Compare(keys[a], keys[b]) > 0)
                {
                    TKey key = keys[a];
                    keys[a] = keys[b];
                    keys[b] = key;

                    if (values != null)
                    {
                        TValue value = values[a];
                        values[a] = values[b];
                        values[b] = value;
                    }
                }
            }
        }

        private static void Swap(TKey* keys, TValue* values, int i, int j)
        {
            if (i != j)
            {
                TKey k = keys[i];
                keys[i] = keys[j];
                keys[j] = k;
                if (values != null)
                {
                    TValue v = values[i];
                    values[i] = values[j];
                    values[j] = v;
                }
            }
        }

        internal static void DepthLimitedQuickSort(
            TKey* keys, TValue*  values, int left, int right, IComparer<TKey> comparer, int depthLimit)
        {
            do
            {
                if (depthLimit == 0)
                {
                    Heapsort(keys, values, left, right, comparer);
                    return;
                }

                int i = left;
                int j = right;

                // pre-sort the low, middle (pivot), and high values in place.
                // this improves performance in the face of already sorted data, or 
                // data that is made up of multiple sorted runs appended together.
                int middle = i + ((j - i) >> 1);
                SwapIfGreaterWithItems(keys, values, comparer, i, middle);  // swap the low with the mid point
                SwapIfGreaterWithItems(keys, values, comparer, i, j);   // swap the low with the high
                SwapIfGreaterWithItems(keys, values, comparer, middle, j); // swap the middle with the high

                TKey x = keys[middle];
                do
                {
                    while (comparer.Compare(keys[i], x) < 0) i++;
                    while (comparer.Compare(x, keys[j]) < 0) j--;
                    //Contract.Assert(i >= left && j <= right, "(i>=left && j<=right)  Sort failed - Is your IComparer bogus?");
                    if (i > j) break;
                    if (i < j)
                    {
                        TKey key = keys[i];
                        keys[i] = keys[j];
                        keys[j] = key;
                        if (values != null)
                        {
                            TValue value = values[i];
                            values[i] = values[j];
                            values[j] = value;
                        }
                    }
                    i++;
                    j--;
                } while (i <= j);

                // The next iteration of the while loop is to "recursively" sort the larger half of the array and the
                // following calls recrusively sort the smaller half.  So we subtrack one from depthLimit here so
                // both sorts see the new value.
                depthLimit--;

                if (j - left <= right - i)
                {
                    if (left < j) DepthLimitedQuickSort(keys, values, left, j, comparer, depthLimit);
                    left = i;
                }
                else
                {
                    if (i < right) DepthLimitedQuickSort(keys, values, i, right, comparer, depthLimit);
                    right = j;
                }
            } while (left < right);
        }

        internal static void IntrospectiveSort(
            TKey* keys, TValue* values, int left, int length, IComparer<TKey> comparer)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(comparer != null);
            //Contract.Requires(left >= 0);
            //Contract.Requires(length >= 0);
            //Contract.Requires(length <= keys.Length);
            //Contract.Requires(length + left <= keys.Length);
            //Contract.Requires(length + left <= values.Length);

            if (length < 2)
                return;

            IntroSort(keys, values, left, length + left - 1, 2 * IntrospectiveSortUtilities.FloorLog2(length), comparer);
        }

        private static void IntroSort(
            TKey* keys, TValue* values, int lo, int hi, int depthLimit, IComparer<TKey> comparer)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(comparer != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi < keys.Length);

            while (hi > lo)
            {
                int partitionSize = hi - lo + 1;
                if (partitionSize <= IntrospectiveSortUtilities.IntrosortSizeThreshold)
                {
                    if (partitionSize == 1)
                    {
                        return;
                    }
                    if (partitionSize == 2)
                    {
                        SwapIfGreaterWithItems(keys, values, comparer, lo, hi);
                        return;
                    }
                    if (partitionSize == 3)
                    {
                        SwapIfGreaterWithItems(keys, values, comparer, lo, hi - 1);
                        SwapIfGreaterWithItems(keys, values, comparer, lo, hi);
                        SwapIfGreaterWithItems(keys, values, comparer, hi - 1, hi);
                        return;
                    }

                    InsertionSort(keys, values, lo, hi, comparer);
                    return;
                }

                if (depthLimit == 0)
                {
                    Heapsort(keys, values, lo, hi, comparer);
                    return;
                }
                depthLimit--;

                int p = PickPivotAndPartition(keys, values, lo, hi, comparer);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(keys, values, p + 1, hi, depthLimit, comparer);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition(
            TKey* keys, TValue* values, int lo, int hi, IComparer<TKey> comparer)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(comparer != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi > lo);
            //Contract.Requires(hi < keys.Length);
            //Contract.Ensures(Contract.Result<int>() >= lo && Contract.Result<int>() <= hi);

            // Compute median-of-three.  But also partition them, since we've done the comparison.
            int middle = lo + ((hi - lo) / 2);

            // Sort lo, mid and hi appropriately, then pick mid as the pivot.
            SwapIfGreaterWithItems(keys, values, comparer, lo, middle);  // swap the low with the mid point
            SwapIfGreaterWithItems(keys, values, comparer, lo, hi);   // swap the low with the high
            SwapIfGreaterWithItems(keys, values, comparer, middle, hi); // swap the middle with the high

            TKey pivot = keys[middle];
            Swap(keys, values, middle, hi - 1);
            int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

            while (left < right)
            {
                while (comparer.Compare(keys[++left], pivot) < 0) ;
                while (comparer.Compare(pivot, keys[--right]) < 0) ;

                if (left >= right)
                    break;

                Swap(keys, values, left, right);
            }

            // Put pivot in the right location.
            Swap(keys, values, left, (hi - 1));
            return left;
        }

        private static void Heapsort(TKey* keys, TValue* values, int lo, int hi, IComparer<TKey> comparer)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(comparer != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi > lo);
            //Contract.Requires(hi < keys.Length);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; i -= 1)
                DownHeap(keys, values, i, n, lo, comparer);

            for (int i = n; i > 1; i -= 1)
            {
                Swap(keys, values, lo, lo + i - 1);
                DownHeap(keys, values, 1, i - 1, lo, comparer);
            }
        }

        private static void DownHeap(
            TKey* keys, TValue* values, int i, int n, int lo, IComparer<TKey> comparer)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(comparer != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(lo < keys.Length);

            TKey d = keys[lo + i - 1];
            TValue dValue = values[lo + i - 1];
            int child;
            while (i <= n / 2)
            {
                child = 2 * i;
                if (child < n && comparer.Compare(keys[lo + child - 1], keys[lo + child]) < 0)
                    child++;

                if (!(comparer.Compare(d, keys[lo + child - 1]) < 0))
                    break;

                keys[lo + i - 1] = keys[lo + child - 1];
                if (values != null)
                    values[lo + i - 1] = values[lo + child - 1];
                i = child;
            }
            keys[lo + i - 1] = d;
            if (values != null)
                values[lo + i - 1] = dValue;
        }

        private static void InsertionSort
            (TKey* keys, TValue* values, int lo, int hi, IComparer<TKey> comparer)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(comparer != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi >= lo);
            //Contract.Requires(hi <= keys.Length);

            int i, j;
            TKey t;
            TValue tValue;
            for (i = lo; i < hi; i++)
            {
                j = i;
                t = keys[i + 1];
                tValue = (values != null) ? values[i + 1] : default;
                while (j >= lo && comparer.Compare(t, keys[j]) < 0)
                {
                    keys[j + 1] = keys[j];
                    if (values != null)
                        values[j + 1] = values[j];
                    j--;
                }
                keys[j + 1] = t;
                if (values != null)
                    values[j + 1] = tValue;
            }
        }
    }

    internal unsafe class GenericArraySortHelper<TKey, TValue>
        where TKey : unmanaged, IComparable<TKey>
        where TValue : unmanaged
    {
        public void Sort(TKey* keys, TValue* values, int index, int length, IComparer<TKey> comparer)
        {
            //Contract.Assert(keys != null, "Check the arguments in the caller!");
            //Contract.Assert(index >= 0 && length >= 0 && (keys.Length - index >= length), "Check the arguments in the caller!");

            // Add a try block here to detect IComparers (or their
            // underlying IComparables, etc) that are bogus.
            try
            {
                if (comparer == null || comparer == Comparer<TKey>.Default)
                {
                    IntrospectiveSort(keys, values, index, length);
                }
                else
                {
                    ArraySortHelper<TKey, TValue>.IntrospectiveSort(keys, values, index, length, comparer);
                }
            }
            catch (IndexOutOfRangeException)
            {
                IntrospectiveSortUtilities.ThrowOrIgnoreBadComparer();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Comparer failed.", e);
            }
        }

        private static void SwapIfGreaterWithItems(TKey* keys, TValue* values, int a, int b)
        {
            if (a != b)
            {
                if (keys[a].CompareTo(keys[b]) > 0)
                {
                    TKey key = keys[a];
                    keys[a] = keys[b];
                    keys[b] = key;

                    if (values != null)
                    {
                        TValue value = values[a];
                        values[a] = values[b];
                        values[b] = value;
                    }
                }
            }
        }

        private static void Swap(TKey* keys, TValue* values, int i, int j)
        {
            if (i != j)
            {
                TKey k = keys[i];
                keys[i] = keys[j];
                keys[j] = k;

                if (values != null)
                {
                    TValue v = values[i];
                    values[i] = values[j];
                    values[j] = v;
                }
            }
        }

        private static void DepthLimitedQuickSort(
            TKey* keys, TValue* values, int left, int right, int depthLimit)
        {
            // The code in this function looks very similar to QuickSort in ArraySortHelper<T> class.
            // The difference is that T is constrainted to IComparable<T> here.
            // So the IL code will be different. This function is faster than the one in ArraySortHelper<T>.

            do
            {
                if (depthLimit == 0)
                {
                    Heapsort(keys, values, left, right);
                    return;
                }

                int i = left;
                int j = right;

                // pre-sort the low, middle (pivot), and high values in place.
                // this improves performance in the face of already sorted data, or 
                // data that is made up of multiple sorted runs appended together.
                int middle = i + ((j - i) >> 1);
                SwapIfGreaterWithItems(keys, values, i, middle); // swap the low with the mid point
                SwapIfGreaterWithItems(keys, values, i, j);      // swap the low with the high
                SwapIfGreaterWithItems(keys, values, middle, j); // swap the middle with the high

                TKey x = keys[middle];
                do
                {
                        while (x.CompareTo(keys[i]) > 0) i++;
                        while (x.CompareTo(keys[j]) < 0) j--;
                    
                    //Contract.Assert(i >= left && j <= right, "(i>=left && j<=right)  Sort failed - Is your IComparer bogus?");
                    if (i > j)
                        break;
                    if (i < j)
                    {
                        TKey key = keys[i];
                        keys[i] = keys[j];
                        keys[j] = key;

                        if (values != null)
                        {
                            TValue value = values[i];
                            values[i] = values[j];
                            values[j] = value;
                        }
                    }
                    i++;
                    j--;
                } while (i <= j);

                // The next iteration of the while loop is to "recursively" sort the larger half of the array and the
                // following calls recrusively sort the smaller half.  So we subtrack one from depthLimit here so
                // both sorts see the new value.
                depthLimit--;

                if (j - left <= right - i)
                {
                    if (left < j) DepthLimitedQuickSort(keys, values, left, j, depthLimit);
                    left = i;
                }
                else
                {
                    if (i < right) DepthLimitedQuickSort(keys, values, i, right, depthLimit);
                    right = j;
                }
            } while (left < right);
        }

        internal static void IntrospectiveSort(TKey* keys, TValue* values, int left, int length)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(left >= 0);
            //Contract.Requires(length >= 0);
            //Contract.Requires(length <= keys.Length);
            //Contract.Requires(length + left <= keys.Length);
            //Contract.Requires(length + left <= values.Length);

            if (length < 2)
                return;

            IntroSort(keys, values, left, length + left - 1, 2 * IntrospectiveSortUtilities.FloorLog2(length));
        }

        private static void IntroSort(TKey* keys, TValue* values, int lo, int hi, int depthLimit)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi < keys.Length);

            while (hi > lo)
            {
                int partitionSize = hi - lo + 1;
                if (partitionSize <= IntrospectiveSortUtilities.IntrosortSizeThreshold)
                {
                    if (partitionSize == 1)
                        return;

                    if (partitionSize == 2)
                    {
                        SwapIfGreaterWithItems(keys, values, lo, hi);
                        return;
                    }

                    if (partitionSize == 3)
                    {
                        SwapIfGreaterWithItems(keys, values, lo, hi - 1);
                        SwapIfGreaterWithItems(keys, values, lo, hi);
                        SwapIfGreaterWithItems(keys, values, hi - 1, hi);
                        return;
                    }

                    InsertionSort(keys, values, lo, hi);
                    return;
                }

                if (depthLimit == 0)
                {
                    Heapsort(keys, values, lo, hi);
                    return;
                }
                depthLimit--;

                int p = PickPivotAndPartition(keys, values, lo, hi);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(keys, values, p + 1, hi, depthLimit);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition(TKey* keys, TValue* values, int lo, int hi)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi > lo);
            //Contract.Requires(hi < keys.Length);
            //Contract.Ensures(Contract.Result<int>() >= lo && Contract.Result<int>() <= hi);

            // Compute median-of-three.  But also partition them, since we've done the comparison.
            int middle = lo + ((hi - lo) / 2);

            // Sort lo, mid and hi appropriately, then pick mid as the pivot.
            SwapIfGreaterWithItems(keys, values, lo, middle);  // swap the low with the mid point
            SwapIfGreaterWithItems(keys, values, lo, hi);   // swap the low with the high
            SwapIfGreaterWithItems(keys, values, middle, hi); // swap the middle with the high

            TKey pivot = keys[middle];
            Swap(keys, values, middle, hi - 1);
            int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

            while (left < right)
            {
                while (pivot.CompareTo(keys[++left]) > 0) ;
                while (pivot.CompareTo(keys[--right]) < 0) ;

                if (left >= right)
                    break;

                Swap(keys, values, left, right);
            }

            // Put pivot in the right location.
            Swap(keys, values, left, (hi - 1));
            return left;
        }

        private static void Heapsort(TKey* keys, TValue* values, int lo, int hi)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi > lo);
            //Contract.Requires(hi < keys.Length);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; i -= 1)
                DownHeap(keys, values, i, n, lo);

            for (int i = n; i > 1; i -= 1)
            {
                Swap(keys, values, lo, lo + i - 1);
                DownHeap(keys, values, 1, i - 1, lo);
            }
        }

        private static void DownHeap(TKey* keys, TValue* values, int i, int n, int lo)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(lo < keys.Length);

            TKey d = keys[lo + i - 1];
            TValue dValue = (values != null) ? values[lo + i - 1] : default;
            int child;
            while (i <= n / 2)
            {
                child = 2 * i;
                if (child < n && keys[lo + child - 1].CompareTo(keys[lo + child]) < 0)
                    child++;

                if (keys[lo + child - 1].CompareTo(d) < 0)
                    break;

                keys[lo + i - 1] = keys[lo + child - 1];
                if (values != null)
                    values[lo + i - 1] = values[lo + child - 1];
                i = child;
            }
            keys[lo + i - 1] = d;
            if (values != null)
                values[lo + i - 1] = dValue;
        }

        private static void InsertionSort(TKey* keys, TValue* values, int lo, int hi)
        {
            //Contract.Requires(keys != null);
            //Contract.Requires(values != null);
            //Contract.Requires(lo >= 0);
            //Contract.Requires(hi >= lo);
            //Contract.Requires(hi <= keys.Length);

            int i, j;
            TKey t;
            TValue tValue;
            for (i = lo; i < hi; i++)
            {
                j = i;
                t = keys[i + 1];
                tValue = values[i + 1];

                while (j >= lo && t.CompareTo(keys[j]) < 0)
                {
                    keys[j + 1] = keys[j];
                    if (values != null)
                        values[j + 1] = values[j];
                    j--;
                }
                keys[j + 1] = t;
                if (values != null)
                    values[j + 1] = tValue;
            }
        }
    }
}