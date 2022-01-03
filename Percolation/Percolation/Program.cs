using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace Percolation
{
    class Program
    {
        private static readonly string _plotFile = Environment.CurrentDirectory + "/output/plot.png";
        private static readonly List<int> _sizes = new() { 10, 50, 100, 500, 1000 };
        private static readonly int _pointsCount = 1000;

        static void Main()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_plotFile));

            var plot = new PlotModel
            {
                Title = "Percolation",
                TitleFontSize = 24,
                Subtitle = $"Lattice type: Square",
                SubtitleFontSize = 20
            };
            plot.Axes.Add(new LinearAxis
            {
                Title = "Open cells fraction",
                TitleFontSize = 16,
                FontSize = 12,
                Position = AxisPosition.Bottom
            });
            plot.Axes.Add(new LinearAxis
            {
                Title = "Percolation probability",
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

            foreach (var size in _sizes)
            {
                // Collect statistics
                var steps = new List<int>();
                for (int i = 0; i < _pointsCount; i++)
                {
                    var lattice = new SquareLattice(size);
                    lattice.OpenUntilPercolates(out int stepsPerformed);
                    steps.Add(stepsPerformed);
                }
                steps.Sort();

                Console.WriteLine($"Size: {size}; Steps mean: {(double)steps.Sum() / steps.Count}");

                // Get (X, Y) points
                var points = new List<DataPoint>();
                double step = 1.0 / _pointsCount;
                int counter = 0;
                for (double x = 0; x <= 1; x += step)
                {
                    int limit = (int)(x * size * size);
                    while (counter < _pointsCount && steps[counter] <= limit)
                        counter++;

                    double y = (double)counter / _pointsCount;
                    points.Add(new DataPoint(x, y));
                }

                // Add series to plot
                var series = new LineSeries
                {
                    Title = $"Size = {size}"
                };
                series.Points.AddRange(points);
                plot.Series.Add(series);
            }

            // Export to .png
            using var stream = File.Create(_plotFile);
            var exporter = new PngExporter(800, 600);
            exporter.Export(plot, stream);
        }
    }
}
