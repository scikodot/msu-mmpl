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
            int[] arr1, arr2;
            arr1 = new int[] { 3, 7, 2, 6, 4, 8, 1, 5 };
            arr2 = (int[])arr1.Clone();
            
            Array.Sort(arr1);
            Quicksort.Sort(arr2);
            Assert.Equal(arr1, arr2);
        }
    }
}
