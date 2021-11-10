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
            TestSample(Array.Empty<int>(), cmpInt);
            TestSample(new int[] { 3, 7, 2, 6, 4, 8, 1, 5 }, cmpInt);
            TestSample(new int[] { 1, 1, 1, 1, 1 }, cmpInt);
            TestSample(new int[] { 2, 1, 3, 1, 2 }, cmpInt);
            TestSample(new int[] { -1, 0, -6, 8, 4, 15, -3, 7, 7, 7, -6 }, cmpInt);

            // Float
            var cmpFloat = new Comparator<float>((x, y) => Math.Sign(x - y));
            TestSample(Array.Empty<float>(), cmpFloat);
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

            // String
            var cmpString = new Comparator<string>((x, y) =>
            {
                if (x == null)
                    return y == null ? 0 : -1;
                else
                    return y == null ? 1 : x.Length - y.Length;
            });
            TestSample(Array.Empty<string>(), cmpString);
            TestSample(new string[] { 
                "The", "quick", "brown", "fox", 
                "jumps", "over", "the", "lazy", "dog" }, cmpString);
            TestSample(new string[] { "d", "b", "a", "c" }, cmpString);
            TestSample(new string[] { "", "aa", "a", "aaaa", "aaa" }, cmpString);
            TestSample(new string[] { "One", "string", "here", null, "missing", "!" }, cmpString);
            TestSample(new string[] { "Now", "there", null, "two", null, "missing", "!" }, cmpString);
        }

        public void TestSample<T>(T[] arr, Comparator<T> cmp)
        {
            T[] arr1, arr2;
            arr1 = (T[])arr.Clone();
            arr2 = (T[])arr.Clone();

            Array.Sort(arr1, cmp);
            Quicksort.Sort(arr2, cmp);
            Assert.Equal(arr1, arr2, cmp);
        }
    }
}
