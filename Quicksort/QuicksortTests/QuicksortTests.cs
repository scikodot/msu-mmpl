using System;
using Xunit;
using Sort;

namespace QuicksortTests
{
    public class QuicksortTests
    {
        [Theory]
        [MemberData(nameof(QuicksortData.IntegerData), MemberType = typeof(QuicksortData))]
        public void TestInteger(int[] arr)
        {
            var cmp = new Comparator<int>((x, y) => x - y);
            TestSample(arr, cmp);
        }

        [Theory]
        [MemberData(nameof(QuicksortData.FloatData), MemberType = typeof(QuicksortData))]
        public void TestFloat(float[] arr)
        {
            var cmp = new Comparator<float>((x, y) => Math.Sign(x - y));
            TestSample(arr, cmp);
        }

        [Theory]
        [MemberData(nameof(QuicksortData.StringData), MemberType = typeof(QuicksortData))]
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
