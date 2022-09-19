using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Compiler {
    public class LR0Element {
        public char Var { get; }
        public string Prod { get; }
        public int Position { get; internal set; }

        public LR0Element(char var, string prod, int pos = 0) {
            Var = var;
            Prod = prod;
            Position = pos;
        }

        public LR0Element Clone(int pos = 0) {
            return new LR0Element(Var, Prod, Position);
        }

        public bool hasDottedVar(Serpen.Uni.Automat.GrammerBase G) {
            if (Position < Prod.Length)
                return G.Variables.Contains(Prod[Position]);
            else
                return false;
        }


        public char dottedSymbol {
            get {
                if (Position < Prod.Length)
                    return Prod[Position];
                else 
                    return (char)0;
            }
        }

        public override string ToString()
            => $"{Var} => {Prod.Insert(Position, "◦")}";

        public override bool Equals(object obj) {
            if (obj is LR0Element lr0e)
                return (lr0e.Position == Position &&
                        lr0e.Prod == Prod &&
                        lr0e.Var == Var);
            return false;
        }

        public override int GetHashCode() => ToString().GetHashCode();

    }

    public class LR0Closure : IEnumerable<LR0Element> {

        private List<LR0Element> list = new List<LR0Element>();
        private HashSet<int> hashset = new HashSet<int>();

        private Serpen.Uni.Automat.ContextFree.CFGrammer Grammar;

        public LR0Closure(Serpen.Uni.Automat.ContextFree.CFGrammer grammar, char Var) {
            if (!grammar.Variables.Contains(Var) | !grammar.Rules.ContainsKey(Var))
                throw new System.Exception();

            this.Grammar = grammar;

            foreach (var r in grammar.Rules)
                if (r.Key == Var)
                    Add(r.Key, r.Value);

            int finishedtill = 0;
            do {
                int rulesCount = Count;
                for (int i = finishedtill; i < rulesCount; i++)
                    if (this[i].hasDottedVar(grammar))
                        foreach (var addr in grammar.Rules)
                            if (addr.Key == this[i].dottedSymbol)
                                Add(addr.Key, addr.Value, 0);

                finishedtill = rulesCount;
            } while (finishedtill < Count);

        }

        public LR0Closure(Serpen.Uni.Automat.ContextFree.CFGrammer grammar, LR0Closure lr0c, char gotoVar) {

            this.Grammar = grammar;

            foreach (var item in lr0c)
                if (item.dottedSymbol == gotoVar)
                    Add(item.Var, item.Prod, item.Position + 1);

            int finishedtill = 0;
            do {
                int rulesCount = Count;
                for (int i = finishedtill; i < rulesCount; i++)
                    if (this[i].hasDottedVar(grammar))
                        foreach (var addr in grammar.Rules)
                            if (addr.Key == this[i].dottedSymbol)
                                Add(addr.Key, addr.Value, 0);

                finishedtill = rulesCount;
            } while (finishedtill < Count);

        }

        public LR0Closure[] Gotos() {
            throw new System.NotImplementedException();

            var lr0clst = new List<LR0Closure>();
            foreach (var item in list) {
                if (item.hasDottedVar(Grammar)) {
                    var clone = item.Clone(item.Position + 1);
                    lr0clst.Add(new LR0Closure(Grammar, item.dottedSymbol));
                }
            }

        }

        public LR0Element this[int index] { get => list[index]; set => list[index] = value; }

        public int Count => list.Count;

        public void Add(char var, string prod, int pos = 0) {
            Add(new LR0Element(var, prod, pos));
        }
        public void Add(char var, string[] prods, int pos = 0) {
            foreach (var prod in prods)
                Add(new LR0Element(var, prod, pos));
        }

        public void Add(LR0Element item) {
            if (!hashset.Contains(item.GetHashCode())) {
                list.Add(item);
                hashset.Add(item.GetHashCode());
            }
        }

        public IEnumerator<LR0Element> GetEnumerator() {
            return list.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator() {
            return list.GetEnumerator();
        }

        public override string ToString() {
            if (Count == 0) return "";

            var sb = new System.Text.StringBuilder(Count * 10);
            for (int i = 0; i < this.Count - 1; i++)
                sb.Append(this[i].ToString() + ", ");
            sb.Append(this[Count - 1].ToString());

            return sb.ToString();
        }
    }
}