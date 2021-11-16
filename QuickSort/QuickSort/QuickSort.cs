using System;
using System.Collections.Generic;

namespace Sort
{
    public static class QuickSort
    {
        public enum PivotSelectionMethod
        {
            OneShot,
            MedianOfThree,
            TukeyNinther
        }

        private static readonly Random _rng = new();

        public static void Sort<T>(T[] arr, Comparison<T> cmp, 
            bool useInsertionSort = true,
            bool usePivotHeuristics = true)
        {
            SortSegment(arr, 0, arr.Length, cmp, 
                useInsertionSort, 
                usePivotHeuristics);
        }

        public static void Sort<T>(T[] arr, IComparer<T> cmp, 
            bool useInsertionSort = true,
            bool usePivotHeuristics = true)
        {
            SortSegment(arr, 0, arr.Length, cmp.Compare, 
                useInsertionSort,
                usePivotHeuristics);
        }

        public static void Sort<T>(T[] arr, 
            bool useInsertionSort = true,
            bool usePivotHeuristics = true) where T : IComparable<T>
        {
            SortSegment(arr, 0, arr.Length, (x, y) => x.CompareTo(y), 
                useInsertionSort,
                usePivotHeuristics);
        }

        private static void SortSegment<T>(T[] arr, int start, int end, 
            Comparison<T> cmp, 
            bool useInsertionSort, 
            bool usePivotHeuristics)
        {
            // Apply insertion sort
            if (useInsertionSort && arr.Length <= 20)
            {
                InsertionSort.Sort(arr, cmp);
                return;
            }

            // Choose pivot selection method
            var pivotMethod = PivotSelectionMethod.OneShot;
            if (usePivotHeuristics)
            {
                if (arr.Length >= 300)
                    pivotMethod = PivotSelectionMethod.TukeyNinther;
                else if (arr.Length >= 100)
                    pivotMethod = PivotSelectionMethod.MedianOfThree;
            }

            while (true)
            {
                // Base case
                if (end - start < 2)
                    return;

                // Get pivot index using the appropriate method
                int index = GetPivotIndex(arr, start, end, cmp, pivotMethod);

                // Put the pivot on the first position
                (arr[start], arr[index]) = (arr[index], arr[start]);

                // Perform 3-way Quicksort iteration
                T pivot = arr[start];
                int lh = start + 1,
                    i = start + 1,
                    gt = end - 1;
                while (i <= gt)
                {
                    int res = cmp(arr[i], pivot);
                    if (res < 0)
                    {
                        (arr[lh], arr[i]) = (arr[i], arr[lh]);
                        lh++;
                        i++;
                    }
                    else if (res > 0)
                    {
                        (arr[gt], arr[i]) = (arr[i], arr[gt]);
                        gt--;
                    }
                    else
                        i++;
                }

                // Update boundaries of lesser and greater parts
                lh--; gt++;

                // Make the pivot a head of the lesser part
                (arr[start], arr[lh]) = (arr[lh], arr[start]);

                // Run recursion on the smaller part (by length)
                // and update sort range for the other part
                if (lh - start < end - gt)
                {
                    SortSegment(arr, start, lh, cmp, 
                        useInsertionSort, 
                        usePivotHeuristics);
                    start = gt;
                }
                else
                {
                    SortSegment(arr, gt, end, cmp, 
                        useInsertionSort, 
                        usePivotHeuristics);
                    end = lh;
                }
            }
        }

        private static int GetPivotIndex<T>(T[] arr, int start, int end, Comparison<T> cmp, 
            PivotSelectionMethod pivotMethod)
        {
            switch (pivotMethod)
            {
                default:
                case PivotSelectionMethod.OneShot:
                    return _rng.Next(start, end);

                case PivotSelectionMethod.MedianOfThree:
                    return MedianOfThreeIndex(arr, NextRange(start, end, 3), cmp);

                case PivotSelectionMethod.TukeyNinther:
                    var inds = new int[3];
                    for (int i = 0; i < inds.Length; i++)
                        inds[i] = MedianOfThreeIndex(arr, NextRange(start, end, 3), cmp);

                    return MedianOfThreeIndex(arr, inds, cmp);
            }
        }

        private static int[] NextRange(int start, int end, int num)
        {
            var res = new int[num];
            int frac = (end - start) / num;
            int pos = _rng.Next(start, start + frac);
            for (int i = 0; i < num; i++)
                res[i] = pos + i * frac;

            return res;
        }

        private static int MedianOfThreeIndex<T>(T[] arr, int[] inds, Comparison<T> cmp)
        {
            var elems = new T[3];
            for (int i = 0; i < elems.Length; i++)
                elems[i] = arr[inds[i]];

            int cmp01 = cmp(elems[0], elems[1]), 
                cmp02 = cmp(elems[0], elems[2]), 
                cmp12 = cmp(elems[1], elems[2]);

            if (cmp01 < 0)
            {
                if (cmp02 < 0)
                    return cmp12 < 0 ? inds[1] : inds[2];
                else
                    return inds[0];
            }
            else
            {
                if (cmp02 < 0)
                    return inds[0];
                else
                    return cmp12 < 0 ? inds[2] : inds[1];
            }
        }
    }
}
