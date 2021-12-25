﻿using System;

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

                var board = new Board(size, shuffle: true, steps: steps, randomState: 3927);
                Console.WriteLine("Board:");
                Console.WriteLine(board);

                var path = board.Solve();
                if (path.Count > 0)
                {
                    Console.WriteLine("Solution:");
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
