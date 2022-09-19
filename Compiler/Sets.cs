
using System.Collections.Generic;
using Serpen.Uni.Automat.ContextFree;
using System.Linq;
using System;

namespace Serpen.Uni.Compiler {
    class FirstSet {
        private readonly CFGrammer grammar;

        private IDictionary<char, List<string>> first = new Dictionary<char, List<string>>(); 

        public FirstSet(CFGrammer Grammar) {
            grammar = Grammar;

            foreach (var terminal in grammar.Terminals)
                first.Add(terminal, new List<string>(new string[] { terminal.ToString() }));
        }

        [AlgorithmSource("1810_A3.14_P61")]
        public string[] SetFor(char Var) {
            Serpen.Uni.Utils.DebugMessage($"FirstSet for {Var}", Uni.Utils.eDebugLogLevel.Verbose);

            if (first.ContainsKey(Var))
                return first[Var].ToArray();
            else if (!grammar.VarAndTerm.Contains(Var))
                throw new Serpen.Uni.Automat.AlphabetException(Var);
            else if (!grammar.Rules.ContainsKey(Var))
                return new String[] { };

            var Prods = grammar.Rules[Var];
            var D = new List<string>[Prods.Length];

            for (int i = 0; i < Prods.Length; i++) {
                var prod = Prods[i];

                if (prod == string.Empty) { // A -> e
                    D[i] = new List<string>() { "" };
                } else {
                    if (prod[0] != Var)
                        D[i] = SetFor(prod[0]).Where(s => s != "").ToList();
                    int j = 1;

                    // ε ϵ FIRST(Prod[i])
                    while (Prods[i].Length > j && ContainsE(Prods[i][j])) {
                        j++;
                        D[i] = D[i].Union((SetFor(Prods[i][j])).Where(s => s != "")).Distinct().ToList();
                    }

                    if (j == Prods[i].Length && ContainsE(prod[prod.Length - 1]))
                        D[i].Add("");
                }
                if (!first.ContainsKey(Var))
                    first[Var] = new();

                if (D[i] != null)
                    first[Var].AddRange(D[i].Distinct());
            }
            if (!first.ContainsKey(Var))
                first[Var] = new();

            first[Var] = first[Var].Distinct().OrderBy(s => s).ToList();
            return first[Var].ToArray();
        }

        bool ContainsE(char prod) {
            if (grammar.Terminals.Contains(prod))
                return false;
            else
                return SetFor(prod).Contains("");
        }

        [AlgorithmSource("HHU_4")]
        public char[] FirstSet_HHU(char Var) {
            var first = new Dictionary<char, List<char>>(grammar.VarAndTerm.Count);

            foreach (var t in grammar.Terminals)
                first[t] = new List<char>(new char[] { t });
            foreach (var v in grammar.Variables)
                first[v] = new List<char>();

            bool changed = false;
            int runs = 0;
            do {
                runs++;
                changed = false;
                foreach (var r in grammar.Rules) {
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

        public char[] FollowSet(char NonTerminal) {
            var follow = new Dictionary<char, List<char>>();
            follow.Add(grammar.StartSymbol, new List<char>('$'));

            foreach (var prod in grammar.Rules) {

            }

            throw new System.NotImplementedException();

        }

        [AlgorithmSource("HHU_4")]
        public char[] Nullable() {
            var nullable = new Dictionary<char, bool>(grammar.VarAndTerm.Count);
            foreach (var symbol in grammar.VarAndTerm)
                nullable[symbol] = false;

            bool changed = false;

            do {
                changed = false;
                foreach (var r in grammar.Rules) {
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

        [AlgorithmSource("https://www.hsg-kl.de/faecher/inf/compiler/parser/first/index.php")]
        IDictionary<char, string[]> python_FIRST_Mengen() {
            var dic = new Dictionary<char, string[]>();

            foreach (char t in grammar.Terminals)
                dic.Add(t, new string[] { t.ToString() });

            foreach (char V in grammar.Variables)
                dic.Add(V, new string[] { });

            int Durchlaeufe = 0;
            bool changed = true;
            while (changed) {
                changed = false;
                foreach (var p in grammar.Rules) {
                    var X = p.Key;
                    var f = dic[X];
                    foreach (var p2 in p.Value) {
                        if (p2 == "") {
                            if (!f.Contains("")) f = f.Append("").ToArray();
                            dic[X] = f;
                            changed = true;
                        } else {
                            foreach (var y in p2) {
                                var fy = dic[y];
                                f = f.Union(fy.Except(ecol)).ToArray();
                                dic[X] = f;
                                changed = true;
                                if (y == p2[p2.Length - 1] & (fy.Contains(""))) {
                                    f = f.Union(ecol).ToArray();
                                    dic[X] = f;
                                    changed = true;
                                }
                                if (!fy.Contains("")) break;
                            }
                        }
                    }
                }
                //  if (fa.Count == dic.Count && !fa.Except(dic).Any()) break;
                //  if (fa.Intersect(dic).Count() == fa.Union(dic).Count()) break;
                if (Durchlaeufe > MAX_DURCHLAEUFE) break;
                Durchlaeufe++;
            }
            return dic;
        }

        public IList<string> python_FIRST(string X) {
            if (X == "") return new List<string>() {""};

            var F = python_FIRST_Mengen();
            var f = new List<string>();

            foreach (char y in X) {
                var fy = F[y];
                f = fy.Except(ecol).Union(f).ToList();
                if (y == X[0] && fy.Contains(""))
                    f = f.Union(ecol).ToList();
                if (!fy.Contains("")) break;
            }
            return f;

        }

        IDictionary<char, string[]> python_Follow_Mengen() {
            var dic = new Dictionary<char, string[]>();
            foreach (char V in grammar.Variables)
                if (V == grammar.StartSymbol)
                    dic.Add(V, new string[] { "$" });
                else
                    dic.Add(V, new String[] { });

            int Durchlaeufe = 0;
            while (true) {
                foreach (var p in grammar.Rules) {
                    foreach (var p1 in p.Value) {
                        if (p1.Length > 0) {
                            foreach (char y in p1) {
                                if (grammar.Variables.Contains(y)) {
                                    var beta = p1.Substring(p1.IndexOf(y) + 1);
                                    var fy = dic[y];
                                    fy = fy.Union(python_FIRST(beta).Except(ecol)).ToArray();
                                    dic[y] = fy;
                                    if (beta == "" | python_FIRST(beta).Contains("")) {
                                        var fA = dic[p.Key];
                                        fy = fy.Union(fA).ToArray();
                                        dic[y] = fy;
                                    }
                                }
                            }
                        }
                    }
                }
                if (Durchlaeufe > MAX_DURCHLAEUFE) break;
                Durchlaeufe++;
            }
            return dic;
        }

        public string[] python_Follow(char X) => python_Follow_Mengen()[X];

        const int MAX_DURCHLAEUFE = 100;

        readonly string[] ecol = new string[] { "" };
    }
}