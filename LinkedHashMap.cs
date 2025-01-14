using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
    /// <summary>
    /// 排序的字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LinkedHashMap<TKey, TValue> : IDictionary<TKey, TValue> {
        private readonly Dictionary<TKey, int> _dict;
        private readonly List<KeyValuePair<TKey, TValue>> _list;
        #region constructor
        public LinkedHashMap() {
            _dict = new Dictionary<TKey, int>();
            _list = new List<KeyValuePair<TKey, TValue>>();
        }


        public LinkedHashMap(IEqualityComparer<TKey> comparer) {
            _dict = new Dictionary<TKey, int>(comparer);
            _list = new List<KeyValuePair<TKey, TValue>>();
        }


        public LinkedHashMap(int capacity) {
            _dict = new Dictionary<TKey, int>(capacity);
            _list = new List<KeyValuePair<TKey, TValue>>(capacity);
        }


        public LinkedHashMap(int capacity, IEqualityComparer<TKey> comparer) {
            _dict = new Dictionary<TKey, int>(capacity, comparer);
            _list = new List<KeyValuePair<TKey, TValue>>(capacity);
        }


        public LinkedHashMap(IEnumerable<KeyValuePair<TKey, TValue>> source) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is ICollection countable) {
                _dict = new Dictionary<TKey, int>(countable.Count);
                _list = new List<KeyValuePair<TKey, TValue>>(countable.Count);
            } else {
                _dict = new Dictionary<TKey, int>();
                _list = new List<KeyValuePair<TKey, TValue>>();
            }
            foreach (var pair in source) {
                this[pair.Key] = pair.Value;
            }
        }


        public LinkedHashMap(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is ICollection countable) {
                _dict = new Dictionary<TKey, int>(countable.Count, comparer);
                _list = new List<KeyValuePair<TKey, TValue>>(countable.Count);
            } else {
                _dict = new Dictionary<TKey, int>(comparer);
                _list = new List<KeyValuePair<TKey, TValue>>();
            }
            foreach (var pair in source) {
                this[pair.Key] = pair.Value;
            }
        }


        #endregion


        #region IDictionary implementation


        public bool ContainsKey(TKey key) {
            return _dict.ContainsKey(key);
        }


        public void Add(TKey key, TValue value) {
            DoAdd(key, value);
        }


        private void DoAdd(TKey key, TValue value) {
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            _list.Add(pair);
            _dict.Add(key, _list.Count - 1);
        }


        public bool Remove(TKey key) {
            if (!_dict.TryGetValue(key, out int index)) {
                return false;
            }
            DoRemove(index, key);
            return true;
        }


        private void DoRemove(int index, TKey key) {
            _list.RemoveAt(index);
            _dict.Remove(key);
            for (var i = index; i < _list.Count; i++) {
                var pair = _list[i];
                _dict[pair.Key] = i;
            }
        }


        public bool TryGetValue(TKey key, out TValue value) {
            if (_dict.TryGetValue(key, out int index)) {
                value = _list[index].Value;
                return true;
            }
            value = default;
            return false;
        }


        private int IndexOf(TKey key, TValue value) {
            if (_dict.TryGetValue(key, out int index)) {
                if (EqualityComparer<TValue>.Default.Equals(value, _list[index].Value)) {
                    return index;
                }
            }
            return -1;
        }


        public TValue this[TKey key] {
            get => _list[_dict[key]].Value;
            set {
                if (!_dict.TryGetValue(key, out int index)) {
                    DoAdd(key, value);
                    return;
                }
                DoSet(index, key, value);
            }
        }

        public TValue this[int index] {
            get => _list[index].Value;

        }

        private void DoSet(int index, TKey key, TValue value) {
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            _list[index] = pair;
        }


        public ICollection<TKey> Keys {
            get {
                return _list.Select(p => p.Key).ToArray();
            }
        }


        public ICollection<TValue> Values {
            get {
                return _list.Select(p => p.Value).ToArray();
            }
        }


        #endregion


        #region ICollection implementation


        public void Clear() {
            _dict.Clear();
            _list.Clear();
        }


        public int Count => _dict.Count;


        public bool IsReadOnly => false;

        #endregion


        #region IEnumerable implementation


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _list.GetEnumerator();
        }


        #endregion


        #region IEnumerable implementation


        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }


        #endregion


        #region explicit ICollection implementation


        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            return (IndexOf(item.Key, item.Value) >= 0);
        }


        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            var index = IndexOf(item.Key, item.Value);
            if (index < 0) {
                return false;
            }
            DoRemove(index, item.Key);
            return true;
        }


        #endregion
    }
}
