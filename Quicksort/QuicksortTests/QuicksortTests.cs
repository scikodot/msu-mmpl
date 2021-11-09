using System;
using Xunit;
using Sort;

namespace QuicksortTests
{
    public class QuicksortTests
    {
        [Fact]
        public void Test()
        {
            // Integer
            var cmpInt = new Comparator<int>((x, y) => x - y);
            TestSample(new int[] { 3, 7, 2, 6, 4, 8, 1, 5 }, cmpInt);
            TestSample(new int[] { 1, 1, 1, 1, 1 }, cmpInt);
            TestSample(new int[] { 2, 1, 3, 1, 2 }, cmpInt);
            TestSample(new int[] { -1, 0, -6, 8, 4, 15, -3, 7, 7, 7, -6 }, cmpInt);
        }

        public void TestSample<T>(T[] arr, Comparator<T> cmp)
        {
            T[] arr1, arr2;
            arr1 = (T[])arr.Clone();
            arr2 = (T[])arr.Clone();

            Array.Sort(arr1, cmp);
            Quicksort.Sort(arr2, cmp);
            Assert.Equal(arr1, arr2);
        }
    }
}
