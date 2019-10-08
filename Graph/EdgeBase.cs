using System.Collections.Generic;

namespace Serpen.Uni.Graph {
    public class EdgeBase<TKey, TValue>
        : IEnumerable<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue> {
        private Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

        public int Count => dic.Count;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dic.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => dic.GetEnumerator();

        public void Add(TKey key, TValue value) => dic.Add(key, value);

        public bool TryAdd(TKey key, TValue value) => dic.TryAdd(key, value);

        public bool ContainsKey(TKey key) => dic.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => dic.TryGetValue(key, out value);

        public IEnumerable<TKey> Keys => dic.Keys;
        public IEnumerable<TValue> Values => dic.Values;

        public TValue this[TKey t] {
            get => dic[t];
            set => dic[t] = value;
        }
    }

}