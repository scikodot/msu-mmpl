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

            // Float
            var cmpFloat = new Comparator<float>((x, y) => Math.Sign(x - y));
            TestSample(new float[] { 0.0345f, 9.8f, 1.23f, 4.567f }, cmpFloat);
            TestSample(new float[] { 3.92781f, 3.92781f, 3.92781f }, cmpFloat);
            TestSample(new float[] { 3.92781243729f, 3.92781243729f, 3.92781243729f }, cmpFloat);
            TestSample(new float[] { 
                1.234567891234567892f, 
                1.234567891234567891f, 
                1.234567891234567896f,
                1.234567891234567894f,
                1.234567891234567897f }, cmpFloat);
            TestSample(new float[] {
                0, 0.05735f, -34.58263f, 123.16f, -16f,
                47f, -20.791784f, 58.1623f }, cmpFloat);
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
