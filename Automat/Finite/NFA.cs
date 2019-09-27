using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.Finite {

    public class NFA : FABase, INFA, IAbgeschlossenheitseigenschaften<FABase, NFAe> {

        public static readonly NFA Empty = new NFA("Empty", 0, new char[] {}, new NFAeTransform(), 0, new uint[] {});

        public NFA(string name, uint StateCount, char[] Alphabet, NFAeTransform Transform, uint StartState, params uint[] acceptedStates)
            : base(StateCount, Alphabet, Transform, StartState, acceptedStates, name) {
        }
        public NFA(string name, string[] states, char[] Alphabet, NFAeTransform Transform, uint StartState, params uint[] acceptedStates)
            : base(states, Alphabet, Transform, StartState, acceptedStates, name) {
        }

        uint[] INFA.GoChar(uint[] q, char w) => GoChar(q, w);
        internal uint[] GoChar(uint[] q, char w) {
            var retQ = new List<uint>();

            for (int i = 0; i < q.Length; i++)
            {
                var t = new EATuple(q[i],w);
                if (((NFAeTransform)Transform).ContainsKey(t.q,t.c.Value))
                    retQ.AddRange((Transform as NFAeTransform)[t.q,t.c.Value]);
            }
            retQ.Sort();
            return retQ.Distinct().ToArray();
        }

        protected override uint[] GoChar(uint q, char w) => GoChar(new uint[] { q }, w);

        public override bool AcceptWord(string w) {
            uint[] q = new uint[] {StartState};

            CheckWordInAlphabet(w);
            
            for (int i = 0; i < w.Length; i++)
            {
                q = GoChar(q, w[i]);
                if (q.Length == 0)
                    return false;
            }
            
            foreach (var q2 in q)
                if (IsAcceptedState(q2))
                    return true;
            return false;
        }

        public override bool Equals(object obj1) {
            DFA dobj;
            if (obj1 is DFA dobj1)
                dobj=dobj1;
            else if (obj1 is NFA nobj)
                dobj = Converter.Nea2TeilmengenDea(nobj);
            else if (obj1 is NFAe neobj)
                dobj = Converter.Nea2TeilmengenDea(neobj);
            else
                return false;

            DFA thisobj = Converter.Nea2TeilmengenDea(this);
            return thisobj.Equals(dobj);
        }

        public override int GetHashCode() => ToString().GetHashCode();

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();
            
            var newT = new NFAeTransform();
            foreach (var t2 in Transform)
                if (translate.Contains(t2.Key.q))
                    foreach (var v in t2.Value)
                        if (translate.Contains(v))
                            newT.AddM(Utils.ArrayIndex(translate,t2.Key.q), t2.Key.c.Value, Utils.ArrayIndex(translate,v));
            
            return new NFA($"{Name}_purged", names, Alphabet, newT, Utils.ArrayIndex(translate,StartState), aStates);
        
        }

        public static NFA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            var t = new NFAeTransform();
            int stateCount = rnd.Next(1, MAX_STATES);
            
            char[] alphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount/3, stateCount); 

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0,alphabet.Length);
                for (int j = 0; j < transformsRnd; j++) {
                    t.AddM(i, Utils.GrAE(alphabet), (uint)rnd.Next(0, stateCount));
                }
            }

            var ret = new NFA("NFA_Random", (uint)stateCount, alphabet, t, (uint)rnd.Next(0,stateCount), accState);
            ret.Name = $"NFA_Random_{ret.GetHashCode()}";
            return ret;

        }

        #region Operations

        public override FABase HomomorphismChar(Dictionary<char,char> translate) {
            var neat = new NFAeTransform();
            var Alp = (char[])this.Alphabet.Clone();

            foreach (var dt in this.Transform)
                 if (translate.ContainsKey(dt.Key.c.Value))
                        neat.Add(dt.Key.q, translate[dt.Key.c.Value], dt.Value);
                    else
                        neat.Add(dt.Key.q, dt.Key.c.Value, dt.Value);

            for (int i = 0; i < this.Alphabet.Length; i++)
                if (translate.ContainsKey(this.Alphabet[i]))
                    Alp[i] = translate[this.Alphabet[i]];

            return new NFA($"NFA_HomomorphismChar({Name})", StatesCount, Alp, neat, StartState, AcceptedStates);
        }
        public override FABase Join(FABase A) {
            var N2 = A as NFA;

            if (!(N2 is null))
                throw new System.NotSupportedException();
            else {
                var neat = new NFAeTransform();

                if (!Utils.SameAlphabet(this, A)) {
                    throw new Uni.Exception("Different Alphabets are not implemented");
                }
                
                var accStates = new List<uint>(this.AcceptedStates.Length+N2.AcceptedStates.Length);
                uint sc = this.StatesCount;

                foreach (var t in this.Transform)
                    neat.Add(t.Key.q, t.Key.c.Value, t.Value);
                foreach (var t in N2.Transform) {
                    uint[] qnexts = new uint[t.Value.Length];
                    for (int i = 0; i < t.Value.Length; i++)
                        qnexts[i] = t.Value[i]+sc;
                    neat.Add(t.Key.q+sc, t.Key.c.Value, qnexts);
                }

                accStates.AddRange(this.AcceptedStates);
                for (int i = 0; i < N2.AcceptedStates.Length; i++)
                    accStates.Add(N2.AcceptedStates[i]+sc);

                accStates.Sort();

                return new NFA($"Join({Name}+{N2.Name})", (N2.StatesCount+sc), this.Alphabet, neat, this.StartState, accStates.ToArray());
            }
        }
        public FABase Union(FABase A) => throw new System.NotImplementedException();
        public FABase Intersect(FABase A) => throw new System.NotImplementedException();
        public FABase Diff(FABase A) => throw new System.NotImplementedException();

        #endregion


        public static explicit operator NFA(DFA D) {
            var t = new NFAeTransform();
            // Übergangsfunktion im NEA ist eine Menge, im Dea ein Element
            foreach (var t2 in D.Transform)
                t.Add(t2.Key.q, t2.Key.c.Value, t2.Value);

            return new NFA($"NFA_({D.Name})", D.StatesCount, D.Alphabet, t, D.StartState, D.AcceptedStates);
        }

        public override string ToString() => $"{Name} NEA(|{States.Length}|={string.Join(";", States)}, {{{string.Join(',', Alphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {{{string.Join(',', AcceptedStates)}}})".Trim();
    
        
    }
}