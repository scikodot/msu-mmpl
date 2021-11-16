using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Xunit;

using Sort;

namespace QuickSortTests
{
    public class QuickSortTests
    {
        private static readonly Stopwatch _sw = new();
        private static readonly string _logsPath = Environment.CurrentDirectory + "/logs/";

        public static string LogsPath => _logsPath;

        static QuickSortTests()
        {
            // Create logs dir
            Directory.CreateDirectory(_logsPath);

            // Clear all files
            foreach (var file in Directory.GetFiles(_logsPath))
                File.WriteAllText(file, string.Empty);
        }

        [Theory]
        [MemberData(nameof(QuickSortData.IntegerData), MemberType = typeof(QuickSortData))]
        public void TestInteger(int[] arr)
        {
            var cmp = new Comparator<int>((x, y) => x > y ? 1 : (x == y ? 0 : -1));
            TestSample(arr, cmp, out string log);
            WriteLog(log, MethodBase.GetCurrentMethod().Name);
        }

        [Theory]
        [MemberData(nameof(QuickSortData.FloatData), MemberType = typeof(QuickSortData))]
        public void TestFloat(float[] arr)
        {
            var cmp = new Comparator<float>((x, y) => Math.Sign(x > y ? 1 : (x == y ? 0 : -1)));
            TestSample(arr, cmp, out string log);
            WriteLog(log, MethodBase.GetCurrentMethod().Name);
        }

        [Theory]
        [MemberData(nameof(QuickSortData.StringData), MemberType = typeof(QuickSortData))]
        public void TestStringLength(string[] arr)
        {
            var cmp = new Comparator<string>((x, y) =>
            {
                if (x == null)
                    return y == null ? 0 : -1;
                else
                    return y == null ? 1 : x.Length - y.Length;
            });
            TestSample(arr, cmp, out string log);
            WriteLog(log, MethodBase.GetCurrentMethod().Name);
        }

        [Theory]
        [MemberData(nameof(QuickSortData.StringData), MemberType = typeof(QuickSortData))]
        public void TestStringLexicographic(string[] arr)
        {
            var cmp = new Comparator<string>((x, y) => string.Compare(x, y));
            TestSample(arr, cmp, out string log);
            WriteLog(log, MethodBase.GetCurrentMethod().Name);
        }

        private void TestSample<T>(T[] arr, Comparator<T> cmp, out string log)
        {
            log = $"Type: {typeof(T)} ; Length: {arr.Length}\n";

            // Reference sort
            var arr1 = (T[])arr.Clone();
            Run(arr1, cmp, Array.Sort, ref log);

            // Methods to be tested
            var methods = new Action<T[], Comparator<T>>[]
            {
                QuickSort.Sort,
                InsertionSort.Sort
            };

            // Test
            for (int i = 0; i < methods.Length; i++)
            {
                var arr2 = (T[])arr.Clone();
                Run(arr2, cmp, methods[i], ref log);
                Assert.Equal(arr1, arr2, cmp);
            }

            log += "\n";
        }

        private void Run<T>(T[] arr, Comparator<T> cmp, Action<T[], Comparator<T>> sort, ref string log)
        {
            // Reset comparator and timer
            cmp.Reset();
            _sw.Reset();

            // Sort and measure execution time
            _sw.Start();
            sort(arr, cmp);
            _sw.Stop();

            // Log
            string methodName = $"{sort.Method.DeclaringType.Name}.{sort.Method.Name}";
            log += string.Format("{0} : Time -> {1:F3} sec ; Comparisons -> {2}\n", 
                methodName, _sw.ElapsedMilliseconds / 1000f, cmp.Count);
        }

        private void WriteLog(string log, string filename) => 
            File.AppendAllText(_logsPath + filename + ".txt", log);
    }
}
