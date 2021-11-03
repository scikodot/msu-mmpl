using System;
using System.Collections.Generic;

namespace Quicksort
{
    public static class Quicksort
    {
        private static readonly Random _rng = new();

        public static void Sort<T>(T[] arr, Comparison<T> cmp)
        {
            SortSegment(arr, 0, arr.Length, cmp);
        }

        public static void Sort<T>(T[] arr, IComparer<T> cmp)
        {
            SortSegment(arr, 0, arr.Length, cmp.Compare);
        }

        public static void Sort<T>(T[] arr) where T : IComparable<T>
        {
            SortSegment(arr, 0, arr.Length, (x, y) => x.CompareTo(y));
        }

        private static void SortSegment<T>(T[] arr, int start, int end, Comparison<T> cmp)
        {
            // Randomly select a pivot
            int index = _rng.Next(end);
            var pivot = arr[index];

            // Put the pivot on the first position
            (arr[0], arr[index]) = (arr[index], arr[0]);

            // Perform ordering into smaller and bigger sequences
            int head = start + 1;
            for (int i = head; i < end; i++)
            {
                if (cmp(arr[i], pivot) < 0)
                {
                    (arr[head], arr[i]) = (arr[i], arr[head]);
                    head++;
                }
            }

            // Make the pivot a head of the smaller sequence
            (arr[0], arr[head - 1]) = (arr[head - 1], arr[0]);

            // Repeat on both parts
            SortSegment(arr, 0, head - 1, cmp);
            SortSegment(arr, head, arr.Length, cmp);
        }
    }
}
