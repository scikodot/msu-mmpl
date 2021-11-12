using System;
using System.IO;
using System.Diagnostics;
using Xunit;

using Sort;

namespace QuicksortTests
{
    public class QuicksortTests
    {
        private static readonly Stopwatch _sw = new();
        private static readonly string _logPath = Environment.CurrentDirectory + "/logs/";

        public static string LogPath => _logPath;

        static QuicksortTests()
        {
            // Create logs dir
            Directory.CreateDirectory(LogPath);

            // Create or clear log file
            _logPath += "log.txt";
            File.WriteAllText(LogPath, string.Empty);
        }

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
        public void TestStringLength(string[] arr)
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

        [Theory]
        [MemberData(nameof(QuicksortData.StringData), MemberType = typeof(QuicksortData))]
        public void TestStringLexicographic(string[] arr)
        {
            var cmp = new Comparator<string>((x, y) => string.Compare(x, y));
            TestSample(arr, cmp);
        }

        public void TestSample<T>(T[] arr, Comparator<T> cmp)
        {
            T[] arr1, arr2;
            arr1 = (T[])arr.Clone();
            arr2 = (T[])arr.Clone();

            string log = "";
            Run(arr1, cmp, Array.Sort, ref log);
            Run(arr2, cmp, Quicksort.Sort, ref log);
            Assert.Equal(arr1, arr2, cmp);

            // Write log
            log += "\n";
            File.AppendAllText(LogPath, log);
        }

        private void Run<T>(T[] arr, Comparator<T> cmp, Action<T[], Comparator<T>> sort, ref string log)
        {
            _sw.Start();
            sort(arr, cmp);
            _sw.Stop();

            string methodName = $"{sort.Method.DeclaringType.Name}.{sort.Method.Name}";
            log += string.Format("{0} : Time -> {1:F3} sec ; Comparisons -> {2}\n", 
                methodName, _sw.ElapsedMilliseconds / 1000f, cmp.Count);

            cmp.Reset();
            _sw.Reset();
        }
    }
}
