using System;
using System.Collections.Generic;

namespace Sort
{
    public static class InsertionSort
    {
        public static void Sort<T>(T[] arr, Comparison<T> cmp)
        {
            SortCommon(arr, cmp);
        }

        public static void Sort<T>(T[] arr, IComparer<T> cmp)
        {
            SortCommon(arr, cmp.Compare);
        }

        public static void Sort<T>(T[] arr) where T : IComparable<T>
        {
            SortCommon(arr, (x, y) => x.CompareTo(y));
        }

        public static void SortCommon<T>(T[] arr, Comparison<T> cmp)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var curr = arr[i];
                int j = i - 1;
                while (j >= 0 && cmp(arr[j], curr) > 0)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = curr;
            }
        }
    }
}
