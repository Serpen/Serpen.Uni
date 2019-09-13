using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {

        public class RuleSet : Dictionary<char, string[]> {
            RuleConstaint _ruleConstaint;

            public RuleSet() : this(RuleConstaint.None) {}
            public RuleSet(RuleConstaint ruleConstaint) {
                this._ruleConstaint = ruleConstaint;
            }

            public void AddM(char c, string[] s) {
                string[] vals;
                if (TryGetValue(c, out vals))
                    this[c] = vals.Union(s).Distinct().ToArray();
                else
                    Add(c, s);
            }

            public char[] FindProdHeads(string w)
                => (from r in this where r.Value.Contains(w) select r.Key).Distinct().ToArray();


            public override string ToString() {
                var sw = new System.Text.StringBuilder();
                sw.Append("{");
                foreach (var r in this)
                    sw.Append($"({r.Key}=>({string.Join(',', r.Value)}))");
                sw.Append("}");

                return sw.ToString();
            }
        }
}