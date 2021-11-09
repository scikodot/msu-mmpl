using System;
using System.Collections.Generic;

namespace QuicksortTests
{
    public class Comparator<T> : IComparer<T>
    {
        private readonly Comparison<T> _cmp;
        private int _count = 0;
        public int Count => _count;

        public Comparator(Comparison<T> cmp)
        {
            _cmp = cmp;
        }
        public int Compare(T x, T y)
        {
            _count++;
            return _cmp(x, y);
        }

        public void Reset()
        {
            _count = 0;
        }
    }
}
