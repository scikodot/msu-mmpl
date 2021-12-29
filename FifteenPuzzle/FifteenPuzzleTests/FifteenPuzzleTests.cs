using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Xunit;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.ImageSharp;

using FifteenPuzzle;

namespace FifteenPuzzleTests
{
    public class LogFixture
    {
        private readonly string _path = Environment.CurrentDirectory + "/output";
        private string _directory;
        private string _statsHeader;
        private string _statsFile;
        private string _plotFile;

        public LogFixture()
        {
            // Create logs dir
            Directory.CreateDirectory(_path);

            // Delete all files
            foreach (var directory in Directory.GetDirectories(_path))
                Directory.Delete(directory, recursive: true);
        }

        public void Setup(string directory, string statsHeader)
        {
            _directory = string.Join('/', _path, directory);
            _statsHeader = statsHeader;
            _statsFile = string.Join('/', _directory, "stats.csv");
            _plotFile = string.Join('/', _directory, "plot.png");
        }

        public void WriteSolution(List<Board> solution, int nodesCount, int movesWeight, int distanceWeight)
        {
            string output = "Initial board:\n" + solution[0] + "\n";
            if (solution.Count > 1)
            {
                output += $"Solution (total nodes {nodesCount}):\n";
                for (int i = 1; i < solution.Count; i++)
                    output += $"Step #{i}:\n{solution[i]}\n";
            }
            else
                output += "No solution has been found.";

            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            string file = string.Join('/', _directory, $"a={movesWeight},b={distanceWeight}.txt");
            File.WriteAllText(file, output);
        }

        public void WriteStats<T>(IEnumerable<T> stats)
        {
            string output = string.Join(',', stats);

            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            if (!File.Exists(_statsFile))
                File.WriteAllText(_statsFile, _statsHeader + "\n");

            File.AppendAllText(_statsFile, output + "\n");
        }

        public void PlotStats(int size)
        {
            // Define plot model
            var plot = new PlotModel
            {
                Title = "A* performance",
                TitleFontSize = 24,
                Subtitle = $"Board size: {size}x{size}",
                SubtitleFontSize = 20
            };
            plot.Axes.Add(new LinearAxis
            {
                Title = "Shuffle steps",
                TitleFontSize = 16,
                FontSize = 12,
                Position = AxisPosition.Bottom
            });
            plot.Axes.Add(new LogarithmicAxis
            {
                Title = "Total nodes",
                TitleFontSize = 16,
                FontSize = 12,
                Position = AxisPosition.Left
            });
            plot.Legends.Add(new Legend
            {
                LegendFontSize = 16,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Vertical
            });

            // Plot colors
            //var colors = new List<OxyColor>
            //{
            //    OxyColors.ForestGreen,
            //    OxyColors.Red,
            //    OxyColors.MediumBlue,
            //    OxyColors.Orange,
            //    OxyColors.Purple
            //};

            // Load saved data
            var data = File.ReadLines(_statsFile)
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
            var header = _statsHeader.Split(',').Skip(1).GetEnumerator();
            //var color = colors.GetEnumerator();
            while (enumerators.All(x => x.MoveNext()))
            {
                header.MoveNext();
                string legendTitle = string.Format("a = {0}, b = {1}", header.Current.Split(';'));
                //color.MoveNext();
                var series = new LineSeries
                {
                    Title = legendTitle
                    //Color = color.Current
                };
                var Y = enumerators.Select(e => e.Current);
                series.Points.AddRange(X.Zip(Y, (x, y) => new DataPoint(x, y)));
                plot.Series.Add(series);
            }

            // Export to .png
            using (var stream = File.Create(_plotFile))
            {
                var exporter = new PngExporter(800, 600);
                exporter.Export(plot, stream);
            }

            // Cleanup
            foreach (var e in enumerators)
                e.Dispose();
            header.Dispose();
            //color.Dispose();
        }
    }

    public class FifteenPuzzleTests : IClassFixture<LogFixture>
    {
        private readonly LogFixture _logFixture;

        // When [movesWeight > distanceWeight], e.g. (1, 0), we have a BFS traversal and a VERY poor performance;
        // hence, consider only such pairs of weights that suffice the condition [movesWeight <= distanceWeight]
        private static readonly List<(int, int)> _weights = new()
        {
            (0, 1),  // DFS; [distanceWeight]'s increase won't affect anything
            (1, 1),  // Vanilla A*; both weights' (simultaneous) increase won't affect anything
            (1, 2),
            (1, 3),
            (1, 4),
            (1, 5),
            (2, 3),
            (2, 4),
            (2, 5),
            (3, 4),
            (3, 5),
            (4, 5)
        };
        private static readonly string _header =
            "Steps," + string.Join(',', _weights.Select(x => $"{x.Item1};{x.Item2}"));

        public FifteenPuzzleTests(LogFixture logFixture)
        {
            _logFixture = logFixture;
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void Test(int size)
        {
            string directory = $"{size}x{size}";
            _logFixture.Setup(directory, _header);
            foreach (var steps in Enumerable.Range(1, 5).Select(x => x * 10))
            {
                var stats = new List<int> { steps };
                var board = new Board(size, shuffle: true, steps: steps, randomState: 3927);
                foreach (var (movesWeight, distanceWeight) in _weights)
                {
                    var solution = new List<Board> { board };
                    var path = board.Solve(out int nodesCount, movesWeight, distanceWeight);
                    var boardCopy = board.Copy();
                    for (int i = 0; i < path.Count; i++)
                    {
                        boardCopy.Move(path[i]);
                        solution.Add(boardCopy);
                    }

                    Assert.True(boardCopy.IsSolved());

                    stats.Add(nodesCount);
                    _logFixture.WriteSolution(solution, nodesCount, movesWeight, distanceWeight);                    
                }

                _logFixture.WriteStats(stats);
            }

            _logFixture.PlotStats(size);
        }
    }
}
