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
        private string _statHeader;
        private string _statNodesFile;
        private string _statStepsFile;
        private string _plotNodesFile;
        private string _plotStepsFile;

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
            _statHeader = statsHeader;
            _statNodesFile = string.Join('/', _directory, "statNodes.csv");
            _statStepsFile = string.Join('/', _directory, "statSteps.csv");
            _plotNodesFile = string.Join('/', _directory, "plotNodes.png");
            _plotStepsFile = string.Join('/', _directory, "plotSteps.png");
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

        public void WriteSteps(IEnumerable<int> steps) => Write(steps, _statStepsFile);

        public void WriteNodes(IEnumerable<int> nodes) => Write(nodes, _statNodesFile);

        public void Write(IEnumerable<int> stats, string file)
        {
            string output = string.Join(',', stats);

            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            if (!File.Exists(file))
                File.WriteAllText(file, _statHeader + "\n");

            File.AppendAllText(file, output + "\n");
        }

        public void PlotSteps(int size)
        {
            var plot = new PlotModel
            {
                Title = "A* performance (steps)",
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
                Title = "Solution steps",
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

            FillPlot(plot, _statStepsFile);
            ExportPlot(plot, _plotStepsFile);
        }

        public void PlotNodes(int size)
        {
            var plot = new PlotModel
            {
                Title = "A* performance (nodes)",
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

            FillPlot(plot, _statNodesFile);
            ExportPlot(plot, _plotNodesFile);
        }

        private void FillPlot(PlotModel plot, string file)
        {
            // Load saved data
            var data = File.ReadLines(file)
                           .Skip(1)
                           .Select(x => x.Split(',').Select(y => int.Parse(y)))
                           .OrderBy(x => x.First());

            var enumerators = data.Select(x =>
            {
                var e = x.GetEnumerator();
                e.MoveNext();
                return e;
            }).ToList();

            // Get (X, Y) series for each plot
            var X = enumerators.Select(e => e.Current).ToList();
            var header = _statHeader.Split(',').Skip(1).GetEnumerator();
            while (enumerators.All(e => e.MoveNext()))
            {
                header.MoveNext();
                var series = new LineSeries
                {
                    Title = string.Format("a = {0}, b = {1}", header.Current.Split(';'))
                };
                var Y = enumerators.Select(e => e.Current);
                series.Points.AddRange(X.Zip(Y, (x, y) => new DataPoint(x, y)));
                plot.Series.Add(series);
            }

            // Cleanup
            foreach (var e in enumerators)
                e.Dispose();
            header.Dispose();
        }

        private void ExportPlot(PlotModel plot, string file)
        {
            // Export to .png
            using var stream = File.Create(file);
            var exporter = new PngExporter(800, 600);
            exporter.Export(plot, stream);
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
            "ShuffleSteps," + string.Join(',', _weights.Select(x => $"{x.Item1};{x.Item2}"));

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
            int max = 10, scale = 5;
            foreach (var shuffleSteps in Enumerable.Range(1, max).Select(x => x * scale))
            {
                var stats = new List<(int, int)> { (shuffleSteps, shuffleSteps) };
                var board = new Board(size, shuffle: true, steps: shuffleSteps);
                foreach (var (movesWeight, distanceWeight) in _weights)
                {
                    var solution = new List<Board> { board };
                    var path = board.Solve(out int nodesCount, movesWeight, distanceWeight);
                    var boardCopy = board.Copy();
                    for (int i = 0; i < path.Count; i++)
                    {
                        boardCopy.Move(path[i]);
                        solution.Add(boardCopy.Copy());
                    }

                    Assert.True(boardCopy.IsSolved());

                    stats.Add((solution.Count - 1, nodesCount));

                    if (shuffleSteps == max * scale)
                        _logFixture.WriteSolution(solution, nodesCount, movesWeight, distanceWeight);                    
                }

                _logFixture.WriteSteps(stats.Select(x => x.Item1));
                _logFixture.WriteNodes(stats.Select(x => x.Item2));
            }

            _logFixture.PlotSteps(size);
            _logFixture.PlotNodes(size);
        }
    }
}
