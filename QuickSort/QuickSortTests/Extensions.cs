using System;
using System.Collections.Generic;

namespace QuickSortTests
{
    public static class Extensions
    {
        private static readonly Random _rng = new();
        private static readonly string _alphanum =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static T[] Sample<T>(this IList<T> list, int count)
        {
            var res = new T[count];

            // Sample random elements
            int counter = 0;
            int leeway = list.Count <= count ? count - list.Count : count;
            while (counter < leeway)
                res[counter++] = list[_rng.Next(list.Count)];

            // Already filled
            if (leeway == count)
                return res;

            // Sample all elements in random order,
            // so that all unique elements are presented
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);

                var value = list[k];
                list[k] = list[n];
                list[n] = value;

                res[counter++] = value;
            }

            return res;
        }

        public static T[] RandomArray<T>(this Random _, int length, Func<T> next)
        {
            var res = new T[length];
            for (int i = 0; i < length; i++)
                res[i] = next();

            return res;
        }

        public static int NextInt(this Random rng) => 
            rng.Next(int.MinValue, int.MaxValue);

        public static float NextFloat(this Random rng) => 
            float.MaxValue * (2 * (float)rng.NextDouble() - 1);

        public static string NextString(this Random rng)
        {
            int length = rng.Next(20);
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = _alphanum[rng.Next(_alphanum.Length)];

            return new string(chars);
        }
    }
}
