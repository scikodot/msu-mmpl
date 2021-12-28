using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

using FifteenPuzzle;

namespace FifteenPuzzleTests
{
    public class FifteenPuzzleTests
    {
        private static readonly string _outputDirectory = Environment.CurrentDirectory + "/output/";

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

        static FifteenPuzzleTests()
        {
            // Create logs dir
            Directory.CreateDirectory(_outputDirectory);

            // Delete all files
            foreach (var directory in Directory.GetDirectories(_outputDirectory))
                Directory.Delete(directory, recursive: true);
        }

        [Theory]
        [InlineData(4, 40)]
        [InlineData(5, 50)]
        [InlineData(6, 60)]
        [InlineData(7, 70)]
        public void Test(int size, int steps)
        {
            var directory = _outputDirectory + $"{size}x{size}/";

            var board = new Board(size, shuffle: true, steps: steps, randomState: 3927);
            foreach (var (movesWeight, distanceWeight) in _weights)
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
