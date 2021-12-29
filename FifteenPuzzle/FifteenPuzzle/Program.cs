using System;

namespace FifteenPuzzle
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.Write("Enter board size: ");
                var line = Console.ReadLine();
                if (!int.TryParse(line, out int size))
                {
                    if (line.Trim() == "q")
                    {
                        Console.WriteLine("Exiting...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid board size");
                        continue;
                    }
                }

                Console.Write("Enter number of shuffling steps: ");
                line = Console.ReadLine();
                if (!int.TryParse(line, out int steps))
                {
                    if (line.Trim() == "q")
                    {
                        Console.WriteLine("Exiting...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid number of steps");
                        continue;
                    }
                }

                Console.Write("Enter weights: ");
                var weights = Console.ReadLine().Split(' ');
                if (!int.TryParse(weights[0], out int movesWeight) ||
                    !int.TryParse(weights[1], out int distanceWeight))
                {
                    if (line.Trim() == "q")
                    {
                        Console.WriteLine("Exiting...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid weights");
                        continue;
                    }
                }

                var board = new Board(size, shuffle: true, steps: steps, randomState: 3927);
                Console.WriteLine("Board:");
                Console.WriteLine(board);

                var path = board.Solve(out int nodesCount, movesWeight, distanceWeight);
                if (path.Count > 0)
                {
                    Console.WriteLine($"Solution (total nodes {nodesCount}):");
                    for (int i = 0; i < path.Count; i++)
                    {
                        Console.WriteLine($"Step #{i}");
                        board.Move(path[i]);
                        Console.WriteLine(board);
                    }
                }
                else
                    Console.WriteLine("Could not solve the given board");
            }
        }
    }
}
