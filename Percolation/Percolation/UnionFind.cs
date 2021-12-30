using System;

namespace Percolation
{
    public class UnionFind
    {
        private readonly int[] _parent;
        private readonly int[] _size;

        private int _count;
        public int Count => _count;

        public UnionFind(int count)
        {
            _count = count;
            _parent = new int[count];
            _size = new int[count];
            for (int i = 0; i < count; i++)
            {
                _parent[i] = i;
                _size[i] = 1;
            }
        }

        public void Union(int element1, int element2)
        {
            int root1 = Find(element1),
                root2 = Find(element2);

            if (root1 == root2)
                return;

            if (_size[root1] < _size[root2])
            {
                _parent[root1] = root2;
                _size[root2] += _size[root1];
            }
            else
            {
                _parent[root2] = root1;
                _size[root1] += _size[root2];
            }

            _count--;
        }

        public int Find(int element)
        {
            if (!Contains(element))
                throw new ArgumentException($"Element {element} is not present");

            int root = element;
            while (root != _parent[root])
                root = _parent[root];

            while (element != root)
            {
                int parent = _parent[element];
                _parent[element] = root;
                element = parent;
            }

            return root;
        }

        public bool IsConnected(int element1, int element2) => 
            Find(element1) == Find(element2);

        public bool Contains(int element) => 
            element >= 0 && element < _parent.Length;
    }
}
