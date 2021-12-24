using System;
using System.Collections.Generic;

namespace FifteenPuzzle
{
    enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    class Board
    {
        private static readonly Random _rng = new();

        private readonly int[,] _tiles;
        public int[,] Tiles => _tiles;

        public int Size => _tiles.GetLength(0); 

        private (int, int) _emptyTile;
        public (int, int) EmptyTile => _emptyTile;

        private int _movesCount = 0;
        public int MovesCount => _movesCount;

        public int this[(int, int) index]
        {
            get => _tiles[index.Item1, index.Item2];
            set => _tiles[index.Item1, index.Item2] = value;
        }

        public Board(int size, bool shuffle = true, int steps = 15)
        {
            if (size < 2)
                throw new ArgumentException("Invalid size");

            _tiles = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _tiles[i, j] = i * size + j + 1;
                }
            }

            _emptyTile = (size - 1, size - 1);
            this[_emptyTile] = 0;

            if (shuffle)
                Shuffle(steps);
        }

        public void Shuffle(int steps = 15)
        {
            for (int i = 0; i < steps; i++)
            {
                var directions = ValidDirections();
                var direction = directions[_rng.Next(directions.Count)];
                Move(direction);
            }
        }

        private List<Direction> ValidDirections()
        {
            var directions = new List<Direction>();
            if (Contains((_emptyTile.Item1 - 1, _emptyTile.Item2)))
                directions.Add(Direction.Up);
            if (Contains((_emptyTile.Item1 + 1, _emptyTile.Item2)))
                directions.Add(Direction.Down);
            if (Contains((_emptyTile.Item1, _emptyTile.Item2 - 1)))
                directions.Add(Direction.Left);
            if (Contains((_emptyTile.Item1, _emptyTile.Item2 + 1)))
                directions.Add(Direction.Right);

            return directions;
        }

        private bool Contains((int, int) tile)
        {
            return tile.Item1 >= 0 && tile.Item1 < Size &&
                   tile.Item2 >= 0 && tile.Item2 < Size;
        }

        private void Move(Direction direction)
        {
            var targetTile = _emptyTile;
            switch (direction)
            {
                case Direction.Up:
                    targetTile.Item1 -= 1;
                    break;
                case Direction.Down:
                    targetTile.Item1 += 1;
                    break;
                case Direction.Left:
                    targetTile.Item1 -= 1;
                    break;
                case Direction.Right:
                    targetTile.Item1 += 1;
                    break;
                default:
                    throw new ArgumentException("Invalid move direction");
            }

            (this[_emptyTile], this[targetTile]) = (this[targetTile], this[_emptyTile]);

            _emptyTile = targetTile;
            _movesCount++;
        }

        public Board Copy()
        {
            return MemberwiseClone() as Board;
        }

        public bool IsSolved()
        {
            var lastTile = (Size - 1, Size - 1);
            if (_emptyTile != lastTile)
                return false;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var tile = (i, j);
                    if (this[tile] != i * Size + j + 1 && tile != lastTile)
                        return false;
                }
            }

            return true;
        }

        public List<Board> Solve()
        {
            var nodes = new PriorityQueue<int, Board> { KeyValuePair.Create(Distance(), this) };
            var previous = new Dictionary<Board, Board>();

            while (!nodes.IsEmpty())
            {
                var priority = nodes.RemoveMinimum();

                if (priority.Distance() == 0)
                {
                    var path = new List<Board> { priority };
                    while (priority != null)
                    {
                        path.Add(previous[priority]);
                        priority = previous[priority];
                    }

                    return path;
                }

                var directions = priority.ValidDirections();
                foreach (var direction in directions)
                {
                    var board = priority.Copy();
                    board.Move(direction);
                    nodes.Add(KeyValuePair.Create(board.MovesCount + board.Distance(), board));
                    previous.Add(board, priority);
                }
            }

            return new List<Board>();
        }

        private int Distance()
        {
            int distance = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int jTarg = (_tiles[i, j] - 1) % Size;
                    int iTarg = (_tiles[i, j] - 1 - jTarg) / Size;

                    distance += Math.Abs(iTarg - i) + Math.Abs(jTarg - j);
                }
            }

            return distance;
        }
    }
}
