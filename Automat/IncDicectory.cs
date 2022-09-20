namespace Serpen.Uni.Automat {

    class IncDictionaryF<TKey> : System.Collections.Generic.Dictionary<System.Tuple<TKey, TKey>, float> {
        public void AddOrInc(TKey q1, TKey q2, float Height) {
            var t = new System.Tuple<TKey, TKey>(q1, q2);
            if (base.ContainsKey(t))
                base[t] += Height;
            else
                base.Add(t, Height);
        }

        public float this[TKey q1, TKey q2] {
            get {
                base.TryGetValue(new System.Tuple<TKey, TKey>(q1, q2), out float val);
                return val;
            }
        }
    }

    class IncDictionary<TKey> : System.Collections.Generic.Dictionary<TKey, int> {
        public void AddOrInc(TKey key, int val) {
            if (base.ContainsKey(key))
                base[key] += val;
            else
                base.Add(key, val);
        }
        public void AddOrInc(TKey key) => AddOrInc(key, 1);
    }
}