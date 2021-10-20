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

        protected char[] GetGeneratingAndReachableSymbols(char[] Terminals) {
            var list = new List<char>(Terminals);
            // list.Add(StartSymbol);

            bool foundNew = true;
            while (foundNew) {
                foundNew = false;

                foreach (var r in this) {
                    foreach (string body in r.Value) {
                        if (!list.Contains(r.Key)) {
                            if (body.Length == 1 && list.Contains(body[0]) || body == "") {
                                list.Add(r.Key);
                                foundNew = true;
                            } else {
                                bool allContained = true;
                                for (int i = 0; i < body.Length; i++)
                                    if (!list.Contains(body[i]) && body[i] != r.Key) {
                                        allContained = false;
                                        break;
                                    }
                                if (allContained) {
                                    list.Add(r.Key);
                                    foundNew = true;
                                }
                            }
                        } //end if
                    } //next body
                } //next r
            } //end while


            return list.ToArray();
        }

        internal RuleSet RemoveUnusedSymbols(List<char> finalVars, char[] Terminals) {
            var newRS = new RuleSet();

            char[] usedSymbols = GetGeneratingAndReachableSymbols(Terminals);

            foreach (var r in this) {
                if (usedSymbols.Contains(r.Key)) {
                    var newVals = new List<string>(r.Value.Length);
                    foreach (string body in r.Value) {
                        bool dontAdd = false;
                        if (body == r.Key.ToString())
                            dontAdd = true;
                        foreach (char c in body) {
                            if (!usedSymbols.Contains(c)) {
                                dontAdd = true;
                                if (finalVars.Contains(c)) {
                                    Serpen.Uni.Utils.DebugMessage($"Remove {c} from {this}", Uni.Utils.eDebugLogLevel.Verbose);
                                    finalVars.Remove(c);
                                }
                            }
                        }
                        if (!dontAdd) {
                            newVals.Add(body);
                        }
                    }
                    newRS.Add(r.Key, newVals.Distinct().ToArray());
                } else {
                    // Key isnt usefull
                }
            }

            return newRS;
        }

        public override string ToString() {
            var ruleStr = new List<string>(Count);
            foreach (var r in this)
                ruleStr.Add($"({r.Key}=>({string.Join(',', r.Value)}))");

            return "{" + string.Join(',', ruleStr) + "}";
        }
    }
}