using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {

    [System.Serializable]
    public class RuleSet
        : IEnumerable<KeyValuePair<char, string[]>>, IReadOnlyDictionary<char, string[]> {
        private readonly Dictionary<char, string[]> dic = new Dictionary<char, string[]>();

        public int Count => dic.Count;

        public IEnumerator<KeyValuePair<char, string[]>> GetEnumerator() => dic.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => dic.GetEnumerator();

        public void Add(char key, string[] value) => dic.Add(key, value);

        public void AddM(char c, string[] s) {
            if (TryGetValue(c, out string[] vals))
                this[c] = vals.Union(s).Distinct().ToArray();
            else
                Add(c, s);
        }

        public bool TryAdd(char key, string[] value) => dic.TryAdd(key, value);

        public bool ContainsKey(char key) => dic.ContainsKey(key);

        public bool TryGetValue(char key, out string[] value) => dic.TryGetValue(key, out value);

        public IEnumerable<char> Keys => dic.Keys;
        public IEnumerable<string[]> Values => dic.Values;

        public string[] this[char t] {
            get => dic[t];
            set => dic[t] = value;
        }

        readonly RuleConstaint _ruleConstaint;

        public RuleSet() : this(RuleConstaint.None) { }
        public RuleSet(RuleConstaint ruleConstaint) => this._ruleConstaint = ruleConstaint;

        public char[] FindProdHeads(string w)
            => (from r in this where r.Value.Contains(w) select r.Key).Distinct().ToArray();


        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            sw.Append("{");
            foreach (var r in this)
                sw.Append($"({r.Key}=>({string.Join(',', r.Value)})), ");
            sw.Append("}");

            return sw.ToString();
        }
    }
}