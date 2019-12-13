using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {
    public class ReGrammer : GrammerBase {
        public ReGrammer(string name, char[] variables, char[] terminals, RuleSet rules, char startSymbol)
        : base(name, variables, terminals, rules, startSymbol) { }

        [AlgorithmSource("?1659_3.2.3")]
        public override bool AcceptWord(string w) => throw new System.NotImplementedException();

        public static explicit operator ContextFree.CFGrammer(ReGrammer grammar) {
            return new ContextFree.CFGrammer(grammar.Name, grammar.Variables, grammar.Terminals, grammar.Rules, grammar.StartSymbol);
        }

        [AlgorithmSource("1659_D3.6_P83")]
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
                        if (Variables.Contains(body[0]))
                            throw new GrammerException($"first char in {body} isn't var");
                        if (!Variables.Contains(body[1]))
                            throw new GrammerException($"second char in {body} isn't terminal");
                    }
                }
            }
        }
    }
}