namespace Serpen.Uni.Automat {

    class IncDictionary<T> : System.Collections.Generic.Dictionary<System.Tuple<T, T>, float> {
        public void AddOrInc(T q1, T q2, float Height) {
            var t = new System.Tuple<T, T>(q1, q2);
            if (base.ContainsKey(t))
                base[t] += Height;
            else
                base.Add(t, Height);
        }

        public float this[T q1, T q2] {
            get {
                base.TryGetValue(new System.Tuple<T, T>(q1, q2), out float val);
                return val;
            }
        }
    }
}