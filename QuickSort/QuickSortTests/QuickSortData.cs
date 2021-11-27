using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickSortTests
{
    public class QuickSortData
    {
        private static readonly Random _rng = new();

        private static readonly IEnumerable<object[]> _integerData =
            new List<object[]>
            {
                new object[] { Array.Empty<int>() },
                new object[] { new int[] { 3, 7, 2, 6, 4, 8, 1, 5 } },
                new object[] { new int[] { 1, 1, 1, 1, 1 } },
                new object[] { new int[] { 2, 1, 3, 1, 2 } },
                new object[] { new int[] { -1, 0, -6, 8, 4, 15, -3, 7, 7, 7, -6 } },
                new object[] { _rng.RandomArray(100, _rng.NextInt) }
            };
        public static IEnumerable<object[]> IntegerData => _integerData;

        private static readonly IEnumerable<object[]> _floatData =
            new List<object[]>
            {
                new object[] { Array.Empty<float>() },
                new object[] { new float[] { 0.0345f, 9.8f, 1.23f, 4.567f } },
                new object[] { new float[] { 3.92781f, 3.92781f, 3.92781f } },
                new object[] { new float[] {
                    3.92781243729f,
                    3.92781243729f,
                    3.92781243729f
                } },
                new object[] { new float[] {
                    1.234567891234567892f,
                    1.234567891234567891f,
                    1.234567891234567896f,
                    1.234567891234567894f,
                    1.234567891234567897f
                } },
                new object[] { new float[] {
                    0, 0.05735f, -34.58263f, 123.16f, -16f,
                    47f, -20.791784f, 58.1623f
                } },
                new object[] { _rng.RandomArray(100, _rng.NextFloat) }
            };
        public static IEnumerable<object[]> FloatData => _floatData;

        private static readonly IEnumerable<object[]> _stringData =
            new List<object[]>
            {
                new object[] { Array.Empty<string>() },
                new object[] { new string[]
                {
                    "The", "quick", "brown", "fox",
                    "jumps", "over", "the", "lazy", "dog"
                } },
                new object[] { new string[]
                {
                    "d", "b", "a", "c"
                } },
                new object[] { new string[]
                {
                    "", "aa", "a", "aaaa", "aaa"
                } },
                new object[] { new string[]
                {
                    "One", "string", "here", null, "missing", "!"
                } },
                new object[] { new string[]
                {
                    "Now", "there", null, "two", null, "missing", "!"
                } },
                new object[] { _rng.RandomArray(100, _rng.NextString) }
            };
        public static IEnumerable<object[]> StringData => _stringData;

        public static IEnumerable<object[]> HintsData()
        {
            int count = 10000;
            while (count >= 1)
            {
                var range = Enumerable.Range(0, count).ToArray().Sample(10000);
                yield return new object[] { count, range };
                count -= (int)Math.Ceiling(Math.Pow(10, Math.Ceiling(Math.Log10(count) - 1)));
            }
        }
    }
}
