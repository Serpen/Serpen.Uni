using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {
    public class ReGrammer : GrammerBase {
        public ReGrammer(string name, char[] variables, char[] terminals, RuleSet rules, char startSymbol)
        : base(name, variables, terminals, rules, startSymbol) { }

        [AlgorithmSource("?1659_3.2.3")]
        public override bool AcceptWord(string w) {
            var nfa = (NFAe)this;
            return nfa.AcceptWord(w);
        }

        public static explicit operator ContextFree.CFGrammer(ReGrammer grammar) {
            return new ContextFree.CFGrammer(grammar.Name, grammar.Variables, grammar.Terminals, grammar.Rules, grammar.StartSymbol);
        }

        [AlgorithmSource("1659_P83")]
        public static explicit operator NFAe(ReGrammer grammar) {
            var neat = new NFAeTransform();
            var acceptable = new List<uint>();

            foreach (var r in grammar.Rules) {
                uint rKeyIndex = grammar.Variables.ArrayIndex(r.Key); // use as q
                foreach (string body in r.Value) {
                    if (body.Length == 2)
                        neat.AddM(rKeyIndex, body[0], grammar.Variables.ArrayIndex(body[1])); // terminal to Var as State
                    else
                        acceptable.Add(rKeyIndex); // RS terminates so state is accepted
                }
            }

            return new NFAe($"NEA({grammar.Name})",
                (from g in grammar.Variables select g.ToString()).ToArray(), // variables are states
                grammar.Terminals, neat,
                grammar.Variables.ArrayIndex(grammar.StartSymbol),
                acceptable.Distinct().ToArray());
        }

        public static ReGrammer GenerateRandom(bool removeUnused = true) {
            var rnd = Uni.Utils.RND;
            const byte MAX_CHAR = 10;
            const byte MAX_VAR = 5;
            const byte MAX_RUL = 5;
            const byte MAX_WLEN = 3;
            const byte MAX_BODY = 5;

            var rs = new RuleSet();
            var Vars = new char[rnd.Next(1, MAX_VAR)];
            for (int i = 0; i < Vars.Length; i++)
                Vars[i] = (char)(rnd.Next((int)'A', (int)'Z'));

            var Terms = new char[rnd.Next(1, MAX_CHAR)];
            for (int i = 0; i < Terms.Length; i++)
                Terms[i] = (char)(rnd.Next((int)'a', (int)'z'));

            Vars = Vars.Distinct().OrderBy(s => s).ToArray();
            Terms = Terms.Distinct().OrderBy(s => s).ToArray();

            int rulesCount = rnd.Next(1, MAX_RUL);

            for (int i = 0; i < System.Math.Min(rulesCount, Vars.Length); i++) {
                char rKey = Vars[rnd.Next(0, Vars.Length)];
                var Vals = new string[rnd.Next(1, MAX_BODY)];
                for (int j = 0; j < Vals.Length; j++) {
                    string w = "";
                    var wLen = rnd.Next(0, MAX_WLEN);
                    if (wLen >= 1)
                        w = Terms.RndElement().ToString();
                    if (wLen == 2) {
                        w = w.Insert(1, Vars.RndElement().ToString());
                    }
                    Vals[j] = w;
                }

                if (!rs.ContainsKey(rKey))
                    rs.Add(rKey, Vals.Distinct().OrderBy(s => s).ToArray());
                else
                    rs[rKey] = Enumerable.Concat(rs[rKey], Vals).Distinct().OrderBy(s => s).ToArray();

            }

            var headVars = (from r in rs select r.Key).Distinct().ToArray();

            var varList = new List<char>(Vars);
            if (removeUnused)
                rs = rs.RemoveUnusedSymbols(varList, Terms);

            return new ReGrammer("RG_Random", varList.ToArray(), Terms, rs, headVars.RndElement());
        }

        [AlgorithmSource("1659_3.2.3_P84")]
        public static explicit operator ReGrammer(DFA A) {
            var rs = new RuleSet();

            var stateVars = new char[A.StatesCount];

            for (int i = 0; i < stateVars.Length; i++)
                if (i != A.StartState)
                    stateVars[i] = Utils.NextFreeCapitalLetter(stateVars.Union(A.Alphabet), null);
                else
                    stateVars[i] = Utils.NextFreeCapitalLetter(stateVars.Union(A.Alphabet), 'S');

            foreach (var eat in A.Transforms) {
                var newVals = new List<string>();
                // add all transforms cq as body
                foreach (var eatval in eat.Value)
                    newVals.Add($"{eat.Key.c}{stateVars[eatval]}");
                rs.AddM(stateVars[eat.Key.q], newVals.ToArray());
            }

            for (uint q = 0; q < A.StatesCount; q++) {
                if (A.IsAcceptedState(q))
                    rs.AddM(stateVars[q], new string[] { "" });
            }

            return new ReGrammer($"RG({A.Name})", stateVars, A.Alphabet, rs, stateVars[A.StartState]);
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();

            foreach (var r in Rules) {
                foreach (string body in r.Value) {
                    if (body.Length > 2)
                        throw new GrammerException($"body {body} longer than 2");
                    else if (body.Length == 0) {
                        // ok
                    } else if (body.Length == 1) {
                        if (Variables.Contains(body[0]))
                            throw new GrammerException($"only body {body} is Var");
                    } else {
                        if (!Terminals.Contains(body[0]))
                            throw new GrammerException($"first char in {body} is no Terminal {string.Join(',', Terminals)}");
                        if (!Variables.Contains(body[1]))
                            throw new GrammerException($"second char in {body} is no Variables {string.Join(',', Variables)}");
                    }
                }
            }
        }
    }
}