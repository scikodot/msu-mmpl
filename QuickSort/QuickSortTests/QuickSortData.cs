﻿using System;
using System.Collections.Generic;

namespace QuickSortTests
{
    public class QuickSortData
    {
        private static readonly Random _rng = new();
        private static readonly string _alphanum =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static readonly IEnumerable<object[]> _integerData =
            new List<object[]>
            {
                new object[] { Array.Empty<int>() },
                new object[] { new int[] { 3, 7, 2, 6, 4, 8, 1, 5 } },
                new object[] { new int[] { 1, 1, 1, 1, 1 } },
                new object[] { new int[] { 2, 1, 3, 1, 2 } },
                new object[] { new int[] { -1, 0, -6, 8, 4, 15, -3, 7, 7, 7, -6 } },
                new object[] { RandomArray(100, NextInt) },
                new object[] { RandomArray(1000, NextInt) },
                new object[] { RandomArray(10000, NextInt) },
                new object[] { RandomArray(100000, NextInt) }
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
                new object[] { RandomArray(100, NextFloat) }
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
                new object[] { RandomArray(100, NextString) }
            };
        public static IEnumerable<object[]> StringData => _stringData;

        private static T[] RandomArray<T>(int length, Func<T> next)
        {
            var res = new T[length];
            for (int i = 0; i < length; i++)
                res[i] = next();

            return res;
        }

        private static int NextInt() => _rng.Next(int.MinValue, int.MaxValue);
        private static float NextFloat() => float.MaxValue * (2 * (float)_rng.NextDouble() - 1);
        private static string NextString()
        {
            int length = _rng.Next(20);
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = _alphanum[_rng.Next(_alphanum.Length)];

            return new string(chars);
        }
    }
}
