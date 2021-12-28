using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Xunit;

using FifteenPuzzle;

namespace FifteenPuzzleTests
{
    public class FifteenPuzzleTests
    {
        private static readonly string _outputDirectory = Environment.CurrentDirectory + "/output/";

        private static readonly IEnumerable<int> _movesWeights = Enumerable.Range(1, 2);
        private static readonly IEnumerable<int> _distanceWeights = Enumerable.Range(1, 2);

        static FifteenPuzzleTests()
        {
            // Create logs dir
            Directory.CreateDirectory(_outputDirectory);

            // Delete all files
            foreach (var directory in Directory.GetDirectories(_outputDirectory))
                Directory.Delete(directory, recursive: true);
        }

        [Theory]
        [InlineData(4, 50)]
        [InlineData(5, 50)]
        [InlineData(6, 50)]
        [InlineData(7, 50)]
        public void Test(int size, int steps)
        {
            var directory = _outputDirectory + $"{size}x{size}/";

            var board = new Board(size, shuffle: true, steps: steps, randomState: 3927);
            var paramsGrid = _movesWeights.SelectMany(x => _distanceWeights, (x, y) => (x, y));
            foreach (var (movesWeight, distanceWeight) in paramsGrid)
            {
                var filename = directory + $"a = {movesWeight}, b = {distanceWeight}.txt";
                string output = "Initial board:\n" + board.ToString() + "\n";

                var path = board.Solve(movesWeight, distanceWeight);
                if (path.Count > 0)
                {
                    var boardCopy = board.Copy();
                    output += "Solution:\n";
                    for (int i = 0; i < path.Count; i++)
                    {
                        output += $"Step #{i + 1}:\n";
                        boardCopy.Move(path[i]);
                        output += boardCopy.ToString() + "\n";
                    }

                    Assert.True(boardCopy.IsSolved());
                }
                else
                    output += "No solution has been found.";

                // Write to a file
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(filename, output);
            }
        }
    }
}
