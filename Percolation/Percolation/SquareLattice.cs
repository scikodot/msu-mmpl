using System;
using System.Collections.Generic;

namespace Percolation
{
    public class SquareLattice
    {
        private readonly int _size;
        private readonly UnionFind _cells;
        private readonly bool[] _open;

        public SquareLattice(int size)
        {
            _size = size;
            _cells = new UnionFind(size * size + 2);
            _open = new bool[size * size];
        }

        public void Open(int x, int y)
        {
            if (!Contains(x, y))
                throw new ArgumentException($"The cell ({x}, {y}) is not present");

            int current = GetIndex(x, y);
            _open[current] = true;

            if (y == 0)
                ConnectToSource(current);
            if (y == _size - 1)
                ConnectToDrain(current);

            foreach (var (nx, ny) in GetNeighbours(x, y))
            {
                int neighbour = GetIndex(nx, ny);
                if (_open[neighbour])
                    _cells.Union(current, neighbour);
            }
        }

        public bool IsOpen(int x, int y) => _open[GetIndex(x, y)];

        public bool Percolates() => _cells.IsConnected(SourceIndex(), DrainIndex());

        private List<(int, int)> GetNeighbours(int x, int y)
        {
            var neighbours = new List<(int, int)>();
            if (Contains(x, y - 1))
                neighbours.Add((x, y - 1));
            if (Contains(x, y + 1))
                neighbours.Add((x, y + 1));
            if (Contains(x - 1, y))
                neighbours.Add((x - 1, y));
            if (Contains(x + 1, y))
                neighbours.Add((x + 1, y));

            return neighbours;
        }

        private int GetIndex(int x, int y) => y * _size + x;

        private int SourceIndex() => _size * _size;

        private int DrainIndex() => _size * _size + 1;

        private void ConnectToSource(int index) => _cells.Union(index, SourceIndex());

        private void ConnectToDrain(int index) => _cells.Union(index, DrainIndex());

        private bool Contains(int x, int y) => 
            x >= 0 && x < _size && y >= 0 && y < _size;
    }
}
