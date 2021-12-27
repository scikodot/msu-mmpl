using System;
using System.Collections;
using System.Collections.Generic;

namespace FifteenPuzzle
{
    class PriorityQueue<TK, TV> : IEnumerable<KeyValuePair<TK, TV>> where TV : IComparable
    {
        //private readonly List<KeyValuePair<TK, TV>> _data;

        //public PriorityQueue()
        //{
        //    _data = new List<KeyValuePair<TK, TV>> { default };
        //}

        //public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        //{
        //    return _data.GetEnumerator();
        //}

        //public bool IsEmpty()
        //{
        //    return _data.Count == 1;
        //}

        //public void Add(KeyValuePair<TK, TV> element)
        //{
        //    _data.Add(element);

        //    int k = _data.Count - 1;
        //    while (k > 1 && _data[k].Value.CompareTo(_data[k / 2].Value) < 0)
        //    {
        //        (_data[k], _data[k / 2]) = (_data[k / 2], _data[k]);
        //        k /= 2;
        //    }
        //}

        //public TK RemoveMinimum()
        //{
        //    var root = _data[1];
        //    _data[1] = _data[^1];
        //    _data.RemoveAt(_data.Count - 1);

        //    int k = 1;
        //    while (2 * k < _data.Count)
        //    {
        //        int j = 2 * k;
        //        if (j + 1 < _data.Count && _data[j].Value.CompareTo(_data[j + 1].Value) > 0)
        //            j++;
        //        if (_data[k].Value.CompareTo(_data[j].Value) <= 0)
        //            break;

        //        (_data[k], _data[j]) = (_data[j], _data[k]);
        //        k = j;
        //    }

        //    return root.Key;
        //}

        private KeyValuePair<TK, TV>[] _data = new KeyValuePair<TK, TV>[4];
        private int _size;

        public PriorityQueue()
        {

        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return _data.GetEnumerator() as IEnumerator<KeyValuePair<TK, TV>>;
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        public void Add(KeyValuePair<TK, TV> element)
        {
            if (_size == _data.Length - 1)
                Array.Resize(ref _data, _data.Length * 2);

            _data[++_size] = element;

            int k = _size;
            while (k > 1 && _data[k].Value.CompareTo(_data[k / 2].Value) < 0)
            {
                (_data[k], _data[k / 2]) = (_data[k / 2], _data[k]);
                k /= 2;
            }
        }

        public TK RemoveMinimum()
        {
            var root = _data[1];
            _data[1] = _data[_size];
            _data[_size--] = default;

            int k = 1;
            while (2 * k <= _size)
            {
                int j = 2 * k;
                if (j + 1 <= _size && _data[j].Value.CompareTo(_data[j + 1].Value) > 0)
                    j++;
                if (_data[k].Value.CompareTo(_data[j].Value) <= 0)
                    break;

                (_data[k], _data[j]) = (_data[j], _data[k]);
                k = j;
            }

            if (_size < _data.Length / 4)
                Array.Resize(ref _data, _data.Length / 2);

            return root.Key;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
