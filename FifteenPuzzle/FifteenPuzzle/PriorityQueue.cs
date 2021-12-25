using System;
using System.Collections;
using System.Collections.Generic;

namespace FifteenPuzzle
{
    class PriorityQueue<TK, TV> : IEnumerable<KeyValuePair<TK, TV>> where TK : IComparable
    {
        private readonly List<KeyValuePair<TK, TV>> _data;

        public PriorityQueue()
        {
            _data = new List<KeyValuePair<TK, TV>> { default };
        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public bool IsEmpty()
        {
            return _data.Count == 1;
        }

        public void Add(KeyValuePair<TK, TV> element)
        {
            _data.Add(element);

            int k = _data.Count - 1;
            while (k > 1 && _data[k].Key.CompareTo(_data[k / 2].Key) < 0)
            {
                (_data[k], _data[k / 2]) = (_data[k / 2], _data[k]);
                k /= 2;
            }
        }

        public TV RemoveMinimum()
        {
            var root = _data[1];

            _data[1] = _data[^1];
            _data.RemoveAt(_data.Count - 1);

            int k = 1;
            while (2 * k < _data.Count)
            {
                int j = 2 * k;
                if (j + 1 < _data.Count && _data[j].Key.CompareTo(_data[j + 1].Key) > 0)
                    j++;
                if (_data[k].Key.CompareTo(_data[j].Key) <= 0)
                    break;

                (_data[k], _data[j]) = (_data[j], _data[k]);
                k = j;
            }

            return root.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
