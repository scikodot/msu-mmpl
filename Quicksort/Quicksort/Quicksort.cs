using System;
using System.Collections.Generic;

namespace Quicksort
{
    enum PivotMethod
    {
        OneShot,
        MedianOfThree,
        TukeyNinther
    }

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
            // Choose pivot selection method
            PivotMethod pivotMethod;
            if (arr.Length >= 150)
                pivotMethod = PivotMethod.TukeyNinther;
            else if (arr.Length >= 50)
                pivotMethod = PivotMethod.MedianOfThree;
            else
                pivotMethod = PivotMethod.OneShot;

            while (true)
            {
                // Base case
                if (end - start < 2)
                    return;

                // Get pivot index using the appropriate method
                int index = GetPivotIndex(arr, start, end, cmp, pivotMethod);

                // Put the pivot on the first position
                (arr[0], arr[index]) = (arr[index], arr[0]);

                // Perform ordering into smaller and bigger parts
                T pivot = arr[0];
                int head = start + 1;
                for (int i = head; i < end; i++)
                {
                    if (cmp(arr[i], pivot) < 0)
                    {
                        (arr[head], arr[i]) = (arr[i], arr[head]);
                        head++;
                    }
                }

                // Make the pivot a head of the smaller part
                (arr[0], arr[head - 1]) = (arr[head - 1], arr[0]);

                // Run recursion on the smaller part
                // and update sort range for the other part
                if (head - 1 - start < end - head)
                {
                    SortSegment(arr, start, head - 1, cmp);
                    start = head;
                }
                else
                {
                    SortSegment(arr, head, end, cmp);
                    end = head - 1;
                }
            }
        }

        private static int GetPivotIndex<T>(T[] arr, int start, int end, Comparison<T> cmp, PivotMethod pivotMethod)
        {
            int index = start;
            switch (pivotMethod)
            {
                case PivotMethod.OneShot:
                    index = _rng.Next(start, end);
                    break;
                case PivotMethod.MedianOfThree:
                    var indices = new int[3];
                    var elems = new T[3];
                    for (int i = 0; i < indices.Length; i++)
                    {
                        indices[i] = _rng.Next(start, end);
                        elems[i] = arr[indices[i]];
                    }

                    index = indices[MedianOfThree(elems, cmp)];
                    break;
                case PivotMethod.TukeyNinther:
                    var indicesMedians = new int[3];
                    var elemsMedians = new T[3];
                    for (int i = 0; i < indicesMedians.Length; i++)
                    {
                        var indicesTriplet = new int[3];
                        var elemsTriplet = new T[3];
                        for (int j = 0; j < indicesTriplet.Length; j++)
                        {
                            indicesTriplet[j] = _rng.Next(start, end);
                            elemsTriplet[j] = arr[indicesTriplet[j]];
                        }

                        indicesMedians[i] = indicesTriplet[MedianOfThree(elemsTriplet, cmp)];
                        elemsMedians[i] = arr[indicesMedians[i]];
                    }

                    index = indicesMedians[MedianOfThree(elemsMedians, cmp)];
                    break;
            }

            return index;
        }

        private static int MedianOfThree<T>(T[] arr, Comparison<T> cmp)
        {
            int cmp01 = cmp(arr[0], arr[1]), 
                cmp02 = cmp(arr[0], arr[2]), 
                cmp12 = cmp(arr[1], arr[2]);

            if (cmp01 < 0)
            {
                if (cmp02 < 0)
                    return cmp12 < 0 ? 1 : 2;
                else
                    return 0;
            }
            else
            {
                if (cmp02 < 0)
                    return 0;
                else
                    return cmp12 < 0 ? 2 : 1;
            }
        }
    }
}
