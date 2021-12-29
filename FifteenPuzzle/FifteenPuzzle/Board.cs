using System;
using System.Collections.Generic;
using System.Linq;

namespace FifteenPuzzle
{
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    public class Board
    {
        private static readonly Random _rng = new();

        private readonly int[,] _tiles;
        public int[,] Tiles => _tiles;

        public int Size => _tiles.GetLength(0); 

        private (int, int) _emptyTile;
        public (int, int) EmptyTile => _emptyTile;

        private Direction _previousStep = Direction.None;
        public Direction PreviousStep => _previousStep;

        private int _movesCount = 0;
        public int MovesCount => _movesCount;

        private Board _previousBoard = null;
        public Board PreviousBoard
        {
            get => _previousBoard;
            set => _previousBoard = value;
        }

        public int this[(int, int) index]
        {
            get => _tiles[index.Item1, index.Item2];
            set => _tiles[index.Item1, index.Item2] = value;
        }

        public Board(int size, bool shuffle = false, int steps = 15, int? randomState = null)
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
            {
                Shuffle(steps, randomState);
                ResetPreviousStep();
                ResetMovesCount();
            }
        }

        private Board(Board board)
        {
            _tiles = new int[board.Size, board.Size];
            Array.Copy(board._tiles, _tiles, board._tiles.Length);
            _emptyTile = board._emptyTile;
            _previousStep = board._previousStep;
            _movesCount = board._movesCount;
            _previousBoard = board._previousBoard;
        }

        public void Shuffle(int steps = 15, int? randomState = null)
        {
            Random rng = randomState.HasValue ? new Random(randomState.Value) : _rng;
            for (int i = 0; i < steps; i++)
            {
                var directions = ValidDirections();
                var direction = directions[rng.Next(directions.Count)];
                Move(direction);
            }
        }

        private List<Direction> ValidDirections(bool allowStepBack = false)
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

            if (_previousStep != Direction.None && !allowStepBack)
                directions.Remove(_previousStep.Reverse());

            return directions;
        }

        private bool Contains((int, int) tile)
        {
            return tile.Item1 >= 0 && tile.Item1 < Size &&
                   tile.Item2 >= 0 && tile.Item2 < Size;
        }

        public void Move(Direction direction)
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
                    targetTile.Item2 -= 1;
                    break;
                case Direction.Right:
                    targetTile.Item2 += 1;
                    break;
                default:
                    throw new ArgumentException("Invalid move direction");
            }

            (this[_emptyTile], this[targetTile]) = (this[targetTile], this[_emptyTile]);

            _emptyTile = targetTile;
            _previousStep = direction;
            _movesCount++;
        }

        private void ResetPreviousStep()
        {
            _previousStep = Direction.None;
        }

        private void ResetMovesCount()
        {
            _movesCount = 0;
        }

        public Board Copy() => new(this);

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

        public List<Direction> Solve(out int nodesCount, int movesWeight = 1, int distanceWeight = 1)
        {
            var nodes = new PriorityQueue<Board, int> { KeyValuePair.Create(this, Distance()) };
            nodesCount = 0;
            var directionsPrevious = new Dictionary<Board, Direction> { { this, Direction.None } };

            while (!nodes.IsEmpty())
            {
                var priority = nodes.RemoveMinimum();

                if (priority.Distance() == 0)
                {
                    var path = new List<Direction>();
                    var current = priority.Copy();
                    var direction = directionsPrevious[current];
                    while (direction != Direction.None)
                    {
                        path.Add(direction);
                        current.Move(direction.Reverse());
                        direction = directionsPrevious[current];
                    }

                    path.Reverse();
                    return path;
                }

                var directions = priority.ValidDirections(allowStepBack: false);
                foreach (var direction in directions)
                {
                    var board = priority.Copy();
                    board.Move(direction);
                    if (!directionsPrevious.ContainsKey(board))
                    {
                        nodes.Add(KeyValuePair.Create(board, board.Cost(movesWeight, distanceWeight)));
                        nodesCount += 1;
                        directionsPrevious.Add(board, direction);
                    }
                }
            }

            return new List<Direction>();
        }

        //public List<Board> Solve()
        //{
        //    var nodes = new PriorityQueue<Board, int> { KeyValuePair.Create(this, Distance()) };
        //    int nodesCount = 0;
        //    var unique = new HashSet<Board> { this };

        //    while (!nodes.IsEmpty())
        //    {
        //        var priority = nodes.RemoveMinimum();
        //        unique.Remove(priority);

        //        if (priority.Distance() == 0)
        //        {
        //            var path = new List<Board>();
        //            var current = priority.Copy();
        //            while (current != null)
        //            {
        //                path.Add(current);
        //                current = current.PreviousBoard;
        //            }

        //            path.Reverse();
        //            return path;
        //        }

        //        var directions = priority.ValidDirections(allowStepBack: false);
        //        foreach (var direction in directions)
        //        {
        //            var board = priority.Copy();
        //            board.Move(direction);

        //            if (!unique.Contains(board))
        //            {
        //                nodes.Add(KeyValuePair.Create(board, board.Cost()));
        //                nodesCount += 1;
        //                unique.Add(board);
        //                board.PreviousBoard = priority;
        //            }
        //        }
        //    }

        //    return new List<Board>();
        //}

        private int Distance()
        {
            int distance = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (_tiles[i, j] == 0)
                        continue;

                    int jTarg = (_tiles[i, j] - 1) % Size;
                    int iTarg = (_tiles[i, j] - 1 - jTarg) / Size;

                    distance += Math.Abs(iTarg - i) + Math.Abs(jTarg - j);
                }
            }

            return distance;
        }

        private int Cost(int movesWeight, int distanceWeight)
        {
            return movesWeight * MovesCount + distanceWeight * Distance();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Board);
        }

        public bool Equals(Board board)
        {
            return _tiles.Cast<int>().SequenceEqual(board._tiles.Cast<int>());
        }

        public override int GetHashCode()
        {
            int hash = 3927;
            foreach (var tile in _tiles)
                hash = hash * 31 + tile.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            var columnWidths = new int[Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int width = _tiles[i, j].ToString().Length;
                    if (width > columnWidths[j])
                        columnWidths[j] = width;
                }
            }

            string str = "";
            for (int i = 0; i < Size; i++)
            {
                str += "[ ";
                for (int j = 0; j < Size; j++)
                {
                    var elem = _tiles[i, j].ToString();
                    str += elem + new string(' ', columnWidths[j] - elem.Length + 1);
                }
                str += "]\n";
            }

            return str;
        }
    }
}
