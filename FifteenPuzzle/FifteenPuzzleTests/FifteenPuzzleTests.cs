using System;

using Xunit;

using FifteenPuzzle;

namespace FifteenPuzzleTests
{
    public class FifteenPuzzleTests
    {
        [Theory]
        [InlineData(4, 50)]
        [InlineData(5, 50)]
        [InlineData(6, 50)]
        [InlineData(7, 50)]
        public void Test(int size, int steps)
        {
            var board = new Board(size, shuffle: true, steps: steps, randomState: 3927);
            var path = board.Solve();
            if (path.Count > 0)
            {
                foreach (var direction in path)
                    board.Move(direction);

                Assert.True(board.IsSolved());
            }
        }
    }
}
