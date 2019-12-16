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

        public Dictionary<System.Tuple<string, bool>, System.Tuple<string, bool>[]>
            FindMNEqClasses(int count = 100) {
            var rndwords = GetRandomWords(count, 0, Serpen.Uni.Utils.Sqrt(count), System.Array.Empty<string>());

            System.Tuple<string, bool>[] mhRndTuples = new System.Tuple<string, bool>[rndwords.Length];
            for (int i = 0; i < rndwords.Length; i++)
                mhRndTuples[i] = new System.Tuple<string, bool>(rndwords[i], AcceptWord(rndwords[i]));

            return Serpen.Uni.Utils.EqualityClasses(mhRndTuples,
                (s1, s2) => Tests.InMyhillNerodeRelation(s1.Item1, s2.Item1, this, count));
        }


        protected readonly List<char> VarAndTerm;

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
                            throw new System.ArgumentOutOfRangeException("body", $"{body[i]}[{i}] not in Var/Term");
            }
        }


        protected char[] GetGeneratingAndReachableSymbols() {
            var list = new List<char>(Terminals);
            // list.Add(StartSymbol);

            bool foundNew = true;
            while (foundNew) {
                foundNew = false;

                foreach (var r in Rules) {
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

        protected RuleSet RemoveUnusedSymbols(RuleSet rs, ref List<char> newVars) {
            var newRS = new RuleSet();

            char[] usedSymbols = GetGeneratingAndReachableSymbols();

            foreach (var r in Rules) {
                if (usedSymbols.Contains(r.Key)) {
                    var newVals = new List<string>(r.Value.Length);
                    foreach (string body in r.Value) {
                        bool dontAdd = false;
                        if (body == r.Key.ToString())
                            dontAdd = true;
                        foreach (char c in body) {
                            if (!usedSymbols.Contains(c)) {
                                dontAdd = true;
                                if (newVars.Contains(c))
                                    newVars.Remove(c);
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


        public abstract bool AcceptWord(string w);

        public override string ToString()
            => $"{Name}_{{{string.Join(',', Variables)}}}, {{{string.Join(',', Terminals)}}}, {Rules.ToString()}, {StartSymbol}}}";

    }
}