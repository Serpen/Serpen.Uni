using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {

    [System.Serializable]
    public abstract class GrammerBase : IAcceptWord, IReverse {
        protected GrammerBase(string name, char[] variables, char[] terminals, RuleSet rules, char startSymbol) {
            this.Variables = variables;
            this.Terminals = terminals;
            this.Rules = rules;
            this.StartSymbol = startSymbol;
            this.Name = name;
            this.VarAndTerm = variables.Union(terminals).ToList();

            CheckConstraints();
        }

        public char[] Variables { get; }
        public char[] Terminals { get; }

        public string Name { get; }

        public RuleSet Rules { get; }

        public char StartSymbol { get; }

        char[] IAcceptWord.Alphabet => Terminals;

        public bool SameAlphabet(IAcceptWord A2) {
            if (this.Terminals.Length != A2.Alphabet.Length) return false;
            for (int i = 0; i < this.Terminals.Length; i++)
                if (this.Terminals[i] != A2.Alphabet[i])
                    return false;
            return true;
        }

        public string GetRandomWord() {
            var rnd = Uni.Utils.RND;
            var wLen = rnd.Next(0, 10);
            string w = "";
            for (int k = 0; k < wLen; k++)
                w = w.Insert(k, Terminals[rnd.Next(0, Terminals.Length)].ToString());

            return w;
        }

        public string[] GetRandomWords(int count, int minLen, int maxLen, string[] blocked) {
            var words = new System.Collections.Generic.List<string>();
            var rnd = Uni.Utils.RND;

            int i = 0;

            words.Add("");

            while (words.Count < count) {
                string w = "";
                var wLen = rnd.Next(minLen, maxLen);
                for (int k = 0; k < wLen; k++)
                    w = w.Insert(k, Terminals[rnd.Next(0, Terminals.Length)].ToString());

                if (!words.Contains(w) && !blocked.Contains(w))
                    words.Add(w);

                if (i > count * 10) {
                    Utils.DebugMessage($"Unable to get enough random words {i} tries>{words.Count}>{count}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    break;
                }
                i++;

            }

            words.Sort(delegate (string s1, string s2) {
                if (s1.Length < s2.Length) return -1;
                else if (s1.Length > s2.Length) return 1;
                else return s1.CompareTo(s2);
            });

            return words.ToArray();

        }

        public Dictionary<System.Tuple<string, bool>, System.Tuple<string, bool>[]> FindMNEqClasses(int count = 100) {
            var rndwords = GetRandomWords(count, 0, Serpen.Uni.Utils.Sqrt(count), System.Array.Empty<string>());

            System.Tuple<string, bool>[] mhRndTuples = new System.Tuple<string, bool>[rndwords.Length];
            for (int i = 0; i < rndwords.Length; i++)
                mhRndTuples[i] = new System.Tuple<string, bool>(rndwords[i], AcceptWord(rndwords[i]));

            return Serpen.Uni.Utils.EqualityClasses(mhRndTuples,
                (s1, s2) => Tests.InMyhillNerodeRelation(s1.Item1, s2.Item1, this, count));
        }


        internal readonly ICollection<char> VarAndTerm;

        protected virtual void CheckConstraints() {
            if (this.Variables.Intersect(this.Terminals).Any())
                throw new System.ArgumentOutOfRangeException("Variables", "var intersect term");

            // if (!Rules.ContainsKey(StartSymbol))
            //     throw new Serpen.Uni.Exception("Startsymbol not in Rules");

            foreach (var r in Rules) {
                if (!Variables.Contains(r.Key)) throw new System.ArgumentOutOfRangeException("head", $"{r.Key} not in vars");
                foreach (string body in r.Value)
                    for (int i = 0; i < body.Length; i++)
                        if (!VarAndTerm.Contains(body[i]))
                            throw new System.ArgumentOutOfRangeException("body", $"{body[i]} not in Var/Term {string.Join(',', VarAndTerm)}");
            }
        }




        #region Abgeschlossenheiteseigenschaften

        [AlgorithmSource("EAFK_7.3.3")]
        public IAcceptWord Reverse() {
            var rs = new RuleSet();
            foreach (var r in this.Rules) {
                var bodys = new string[r.Value.Length];
                for (int i = 0; i < r.Value.Length; i++)
                    bodys[i] = string.Join("", r.Value[i].Reverse());
                rs.Add(r.Key, bodys);
            }

            return new ContextFree.CFGrammer($"Rev_({this.Name}", this.Variables, this.Terminals, rs, this.StartSymbol);
        }

        #endregion

        [AlgorithmSource("HHU_4")]
        public char[] Nullable() {
            var nullable = new Dictionary<char, bool>(VarAndTerm.Count);
            foreach (var symbol in VarAndTerm)
                nullable[symbol] = false;

            bool changed = false;

            do {
                changed = false;
                foreach (var r in Rules) {
                    if (!nullable[r.Key])
                        foreach (var prod in r.Value) {
                            bool allNullable = true;
                            for (int i = 0; i < prod.Length; i++) {
                                if (!nullable[prod[i]]) {
                                    allNullable = false;
                                    break;
                                }
                            }
                            if (allNullable) {
                                nullable[r.Key] = true;
                                changed = true;
                            }
                        }
                }
            } while (changed);
            return (from n in nullable where n.Value select n.Key).ToArray();
        }

        [AlgorithmSource("1810_A3.14_P61")]
        public string[] FirstSet(char Var) {
            if (Terminals.Contains(Var))
                return new System.String[] {Var.ToString()};

            var first = new Dictionary<char, List<string>>();

            foreach (var v in Variables)
                first.Add(v, new List<string>());
            foreach (var t in Terminals) {
                first.Add(t, new List<string> { t.ToString() });
            }

            var Prods = Rules[Var];
            var D = new List<string>[Prods.Length];

            for (int i = 0; i < Prods.Length; i++) {
                var prod = Prods[i];

                if (prod == string.Empty) {
                    D[i] = new List<string>();
                    D[i].Add("");
                } else {
                    D[i] = new List<string>(FirstSet(prod[0]).Where(s => s != ""));
                    int j = 1;
                    while (Prods[i].Length > j && (FirstSet(Prods[i][j])).Contains("") && j < Prods[i].Length) {
                        j++;
                        D[i] = D[i].Union((FirstSet(Prods[i][j]).Where(s => s != ""))).ToList();
                    }
                    if (j == Prods[i].Length && FirstSet(prod[prod.Length-1]).Contains(""))
                        D[i].Add("");
                }
                first[Var].AddRange(D[i]);
            }
            return first[Var].ToArray();
        }

        [AlgorithmSource("HHU_4")]
        public char[] FirstSet_HHU(char Var) {
            var first = new Dictionary<char, List<char>>(VarAndTerm.Count);

            foreach (var t in Terminals)
                first[t] = new List<char>(new char[] { t });
            foreach (var v in Variables)
                first[v] = new List<char>();

            bool changed = false;
            int runs = 0;
            do {
                runs++;
                changed = false;
                foreach (var r in Rules) {
                    var prod = r.Value[0];
                    for (int i = 0; i < prod.Length; i++) {
                        first[r.Key].AddRange(first[prod[i]]);
                        changed = true;
                    }
                    for (int i = 1; i < r.Value.Length; i++) {
                        for (int j = 0; j < r.Value[i].Length; j++) {
                            if (Nullable().Contains(r.Value[i][j])) {
                                first[r.Key].AddRange(first[r.Value[i][j]]);
                                changed = true;
                            } else
                                break;
                        }

                    }

                }
            } while (changed & runs < 100);

            foreach (var k in first.Keys)
                first[k] = first[k].Distinct().ToList();

            throw new System.NotImplementedException();
            return first[Var].ToArray();
        }

        public char[] FollowSet(char NonTerminal) => throw new System.NotImplementedException();


        public abstract bool AcceptWord(string w);

        public override string ToString()
            => $"{Name}_{{{string.Join(',', Variables)}}}, {{{string.Join(',', Terminals)}}}, {Rules.ToString()}, {StartSymbol}}}";

    }
}