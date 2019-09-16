using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {
    public abstract class GrammerBase {
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

        // char[] IAcceptWord.Alphabet => Terminals;

        public char StartSymbol { get; }

        public string GetRandomWord() {
            var rnd = Utils.RND;
            var wLen = rnd.Next(0, 10);
            string w = "";
            for (int k = 0; k < wLen; k++)
                w = w.Insert(k, Terminals[rnd.Next(0, Terminals.Length)].ToString());

            return w;
        }

        protected List<char> VarAndTerm;

        public virtual bool CheckConstraints() {
            if (this.Variables.Intersect(this.Terminals).Any()) {
                throw new System.ArgumentOutOfRangeException("Variables", "var intersect term");
            }
            foreach (var r in Rules) {
                if (!Variables.Contains(r.Key)) throw new System.ArgumentOutOfRangeException("head", $"{r.Key} not in vars");
                foreach (string body in r.Value) {
                    for (int i = 0; i < body.Length; i++) {
                        if (!VarAndTerm.Contains(body[i]))
                            throw new System.ArgumentOutOfRangeException("body", $"{body[i]}[{i}] not in Var/Term");
                    }
                }
            }

            return true;
        }


        protected char[] GetGeneratingAndReachableSymbols() {
            var list = new List<char>();
            foreach (char t in Terminals)
                list.Add(t);

            bool foundNew = true;
            while (foundNew) {
                foundNew = false;

                foreach (var r in Rules) {
                    foreach (string body in r.Value) {
                        if (!list.Contains(r.Key)) {
                            if (body.Length == 1 && list.Contains(body[0])) {
                                list.Add(r.Key);
                                foundNew = true;
                            } else if (body == "") {
                                list.Add(r.Key);
                                foundNew = true;
                            } else {
                                bool allContained = true;
                                for (int i = 0; i < body.Length; i++)
                                    if (!list.Contains(body[i])) {
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
                    var newVals = new List<string>();
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


        public abstract bool AcceptWord(string w);


        public override string ToString()
            => $"{{{string.Join(',', Variables)}}}, {{{string.Join(',', Terminals)}}}, {Rules.ToString()}, {StartSymbol}}}";



    }
}

namespace Serpen.Uni.Automat.Finite {
    public class ReGrammer : GrammerBase {
        public ReGrammer(string name, char[] variables, char[] terminals, RuleSet rules, char startSymbol)
        : base(name, variables, terminals, rules, startSymbol) { }

        [AlgorithmSource("1659_3.2.3")]
        public override bool AcceptWord(string w) => throw new System.NotImplementedException();


        public static explicit operator ContextFree.CFGrammer(ReGrammer grammar) {
            return new ContextFree.CFGrammer(grammar.Name, grammar.Variables, grammar.Terminals, grammar.Rules, grammar.StartSymbol);
        }

        public static explicit operator NFAe(ReGrammer grammar) {
            var t = new NFAeTransform();
            var acceptable = new List<uint>();

            foreach (var r in grammar.Rules) {
                uint rKeyIndex = (uint)System.Array.IndexOf(grammar.Variables, r.Key);
                foreach (string body in r.Value) {
                    if (body.Length == 2)
                        t.Add((uint)rKeyIndex, body[0], (uint)System.Array.IndexOf(grammar.Variables, body[1]));
                    else if (body == "") {
                        t.Add(rKeyIndex, null, (uint)System.Array.IndexOf(grammar.Variables, body[1]));
                        acceptable.Add(rKeyIndex);
                    }
                    else
                        throw new System.ArgumentException();
                }
            }

            return new NFAe($"{grammar.Name}-Grammar", (from g in grammar.Variables select g.ToString()).ToArray(), grammar.Terminals, t, (uint)System.Array.IndexOf(grammar.Variables, grammar.StartSymbol), acceptable.ToArray());
        }

        public static explicit operator ReGrammer(NFAe A) {
            var rs = new RuleSet();

            var stateChars = new char[A.States.Length];
            for (int i = 0; i < stateChars.Length; i++)
                stateChars[i] = (char)(((int)'A')+i);
            
            foreach (var eat in A.Transform) {
                char fromChar = stateChars[eat.Key.q];
                var newVals = new List<string>();
                for (int i = 0; i < eat.Value.Length; i++)
                {
                    char toChar = stateChars[eat.Value[i]];
                    newVals.Add($"{eat.Key.c}{toChar}");
                }
                if (A.IsAcceptedState(eat.Key.q))
                    newVals.Add("");

                rs.AddM(fromChar, newVals.ToArray());
            }

            return new ReGrammer($"{A.Name}-NFA", stateChars, A.Alphabet, rs, stateChars[A.StartState]);
        }

        public override bool CheckConstraints() {
            var ret = base.CheckConstraints();
            if (!ret) return false;

            foreach (var r in Rules) {
                foreach (string body in r.Value) {
                    if (body.Length > 2)
                        return false;
                    else if (body.Length == 0) { } else if (body.Length == 1) {
                        if (Variables.Contains(body[0]))
                            return false;
                    } else {
                        if (Variables.Contains(body[0]))
                            return false;
                        if (!Variables.Contains(body[1]))
                            return false;
                    }
                }
            }

            return ret;
        }
    }
}