using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.ImageSharp;

using Sort;

namespace QuickSortTests
{
    public class LogFixture : IDisposable
    {
        private static readonly string _logPath = Environment.CurrentDirectory + "/logs/";
        private static readonly string _logFile = _logPath + "log.csv";
        private static readonly string _plotFile = _logPath + "plot.png";
        private static readonly string _logsHeader = "UNIQUES,CMP_REF,CMP_NONE,CMP_INS,CMP_PIV,CMP_ALL";

        public LogFixture()
        {
            // Create logs dir
            Directory.CreateDirectory(_logPath);

            // Delete all files
            foreach (var file in Directory.GetFiles(_logPath))
                File.Delete(file);
        }

        public void Write<T>(IEnumerable<T> data)
        {
            var path = _logFile;

            // Create file
            if (!File.Exists(path))
                File.WriteAllText(path, _logsHeader);

            var log = "\n" + string.Join(',', data);
            File.AppendAllText(path, log);
        }

        public void Dispose()
        {
            var model = new PlotModel { Title = "QuickSort performance" };

            // Load saved data
            var data = File.ReadLines(_logFile)
                           .Skip(1)
                           .Select(x => x.Split(',').Select(s => int.Parse(s)))
                           .OrderBy(x => x.First());

            var enumerators = data.Select(x =>
            {
                var e = x.GetEnumerator();
                e.MoveNext();
                return e;
            }).ToList();

            // Get (X, Y) series for each plot
            var X = enumerators.Select(x => x.Current).ToList();
            while (enumerators.All(x => x.MoveNext()))
            {
                var series = new LineSeries();
                var Y = enumerators.Select(e => e.Current);
                series.Points.AddRange(X.Zip(Y, (x, y) => new DataPoint(x, y)));
                model.Series.Add(series);
            }

            // Export to .png
            using (var stream = File.Create(_plotFile))
            {
                var exporter = new PngExporter(800, 600);
                exporter.Export(model, stream);
            }

            // Cleanup
            foreach (var e in enumerators)
                e.Dispose();

            GC.SuppressFinalize(this);
        }
    }

    public class QuickSortTests : IClassFixture<LogFixture>
    {
        private static readonly Stopwatch _sw = new();
        private readonly LogFixture _logFixture;

        public QuickSortTests(LogFixture logFixture)
        {
            _logFixture = logFixture;
        }

        [Theory]
        [MemberData(nameof(QuickSortData.IntegerData), MemberType = typeof(QuickSortData))]
        public void TestInteger(int[] arr)
        {
            var cmp = new Comparator<int>((x, y) => x > y ? 1 : (x == y ? 0 : -1));
            TestSample(arr, cmp);
        }

        [Theory]
        [MemberData(nameof(QuickSortData.FloatData), MemberType = typeof(QuickSortData))]
        public void TestFloat(float[] arr)
        {
            var cmp = new Comparator<float>((x, y) => Math.Sign(x > y ? 1 : (x == y ? 0 : -1)));
            TestSample(arr, cmp);
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
            TestSample(arr, cmp);
        }

        [Theory]
        [MemberData(nameof(QuickSortData.StringData), MemberType = typeof(QuickSortData))]
        public void TestStringLexicographic(string[] arr)
        {
            var cmp = new Comparator<string>((x, y) => string.Compare(x, y));
            TestSample(arr, cmp);
        }

        [Theory]
        [MemberData(nameof(QuickSortData.HintsData), MemberType = typeof(QuickSortData))]
        public void TestHints(int uniques, int[] arr)
        {
            var cmp = new Comparator<int>((x, y) => x > y ? 1 : (x == y ? 0 : -1));
            var data = new List<int> { uniques };
            foreach (var hint in Enum.GetValues(typeof(QuickSort.Hints))
                                     .Cast<QuickSort.Hints>())
            {
                var res = TestSample(arr, cmp, hint)
                         .Select(x => x.Item2);

                // Array.Sort result
                if (data.Count == 1)
                    data.Add(res.First());

                data.Add(res.Last());
            }

            _logFixture.Write(data);
        }

        private List<(float, int)> TestSample<T>(T[] arr, Comparator<T> cmp, 
                                                 QuickSort.Hints hints = QuickSort.Hints.All)
        {
            var res = new List<(float, int)>();

            T[] arr1 = (T[])arr.Clone(), 
                arr2 = (T[])arr.Clone();

            res.Add(Run(arr1, cmp, Array.Sort));
            res.Add(Run(arr2, cmp, (arr, cmp) => QuickSort.Sort(arr, cmp, hints)));

            Assert.Equal(arr1, arr2, cmp);

            return res;
        }

        private (float, int) Run<T>(T[] arr, Comparator<T> cmp, Action<T[], Comparator<T>> sort)
        {
            // Reset comparator and timer
            cmp.Reset();
            _sw.Reset();

            // Sort and measure execution time
            _sw.Start();
            sort(arr, cmp);
            _sw.Stop();

            return (_sw.ElapsedMilliseconds / 1000f, cmp.Count);
        }
    }
}
