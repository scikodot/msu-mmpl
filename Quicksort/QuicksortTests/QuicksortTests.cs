using System;
using Xunit;
using Sort;

namespace QuicksortTests
{
    public class QuicksortTests
    {
        [Theory]
        [InlineData(new int[] { })]
        [InlineData(new int[] { 3, 7, 2, 6, 4, 8, 1, 5 })]
        [InlineData(new int[] { 1, 1, 1, 1, 1 })]
        [InlineData(new int[] { 2, 1, 3, 1, 2 })]
        [InlineData(new int[] { -1, 0, -6, 8, 4, 15, -3, 7, 7, 7, -6 })]
        public void TestInteger(int[] arr)
        {
            var cmp = new Comparator<int>((x, y) => x - y);
            TestSample(arr, cmp);
        }

        [Theory]
        [InlineData(new float[] { })]
        [InlineData(new float[] { 0.0345f, 9.8f, 1.23f, 4.567f })]
        [InlineData(new float[] { 3.92781f, 3.92781f, 3.92781f })]
        [InlineData(new float[] { 
            3.92781243729f, 
            3.92781243729f, 
            3.92781243729f 
        })]
        [InlineData(new float[] { 
            1.234567891234567892f, 
            1.234567891234567891f, 
            1.234567891234567896f, 
            1.234567891234567894f, 
            1.234567891234567897f 
        })]
        [InlineData(new float[] { 
            0, 0.05735f, -34.58263f, 123.16f, -16f, 
            47f, -20.791784f, 58.1623f 
        })]
        public void TestFloat(float[] arr)
        {
            var cmp = new Comparator<float>((x, y) => Math.Sign(x - y));
            TestSample(arr, cmp);
        }

        [Theory]
        [InlineData(new object[] { new string[] { } })]
        [InlineData(new object[] { new string[] 
        {
            "The", "quick", "brown", "fox",
            "jumps", "over", "the", "lazy", "dog"
        }})]
        [InlineData(new object[] { new string[] 
        {
            "d", "b", "a", "c"
        }})]
        [InlineData(new object[] { new string[]
        {
            "", "aa", "a", "aaaa", "aaa"
        }})]
        [InlineData(new object[] { new string[]
        {
            "One", "string", "here", null, "missing", "!"
        }})]
        [InlineData(new object[] { new string[]
        {
            "Now", "there", null, "two", null, "missing", "!"
        }})]
        public void TestString(string[] arr)
        {
            var cmp = new Comparator<string>((x, y) =>
            {
                if (x == null)
                    return y == null ? 0 : -1;
                else
                    return y == null ? 1 : x.Length - y.Length;
            });
            TestSample(arr, cmp);
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
