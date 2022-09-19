using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.Finite {

    [System.Serializable]
    public class NFA : FABase, INFA {

        public NFA(string name, uint StateCount, char[] Alphabet, NFAeTransform Transform, uint StartState, params uint[] acceptedStates)
            : base(name, StateCount, Alphabet, Transform, StartState, acceptedStates) {
        }
        public NFA(string name, string[] states, char[] Alphabet, NFAeTransform Transform, uint StartState, params uint[] acceptedStates)
            : base(name, states, Alphabet, Transform, StartState, acceptedStates) {
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            // basic length check
            
            foreach (var t in Transforms) {
                if (!t.Key.c.HasValue)
                    throw new Automat.DeterministicException("e-Transform not allowed in NFA");
            }
        }

        uint[] INFA.GoChar(uint[] q, char w) => GoChar(q, w);
        internal uint[] GoChar(uint[] q, char w) {
            var retQ = new List<uint>();
            var neat = Transforms as NFAeTransform;

            for (int i = 0; i < q.Length; i++)
                if (neat.ContainsKey(q[i], w))
                    retQ.AddRange(neat[q[i], w]);

            retQ.Sort();
            return retQ.Distinct().ToArray();
        }

        protected override uint[] GoChar(uint q, char w) => GoChar(new uint[] { q }, w);

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            uint[] qs = new uint[] { StartState };

            for (int i = 0; i < w.Length; i++) {
                qs = GoChar(qs, w[i]);
                if (qs.Length == 0)
                    return false;
            }

            foreach (var q2 in qs)
                if (IsAcceptedState(q2))
                    return true;
            return false;
        }

        public override bool Equals(object obj1) {
            DFA dobj;
            if (obj1 is DFA dobj1)
                dobj = dobj1;
            else if (obj1 is NFA nobj)
                dobj = DFA.Nea2TeilmengenDea(nobj);
            else if (obj1 is NFAe neobj)
                dobj = DFA.Nea2TeilmengenDea(neobj);
            else
                return false;

            DFA thisobj = DFA.Nea2TeilmengenDea(this);
            return thisobj.Equals(dobj);
        }

        public override int GetHashCode() => ToString().GetHashCode();

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.RemovedStateTranslateTables();

            var newT = new NFAeTransform();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    foreach (var v in t2.Value)
                        if (translate.Contains(v))
                            newT.AddM(translate.ArrayIndex(t2.Key.q), t2.Key.c, translate.ArrayIndex(v));

            return new NFA($"{Name}_purged", names, Alphabet, newT, translate.ArrayIndex(StartState), aStates);

        }

        public static NFA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            var t = new NFAeTransform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] alphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, alphabet.Length);
                for (int j = 0; j < transformsRnd; j++) {
                    t.AddM(i, alphabet.RndElement(), (uint)rnd.Next(0, stateCount));
                }
            }

            return new NFA( $"NFA_Random_q{stateCount}_a{alphabet.Length}_t{t.Count}_a{accState.Length}", (uint)stateCount, alphabet, t, (uint)rnd.Next(0, stateCount), accState);

        }

        #region Operations

        public override IAutomat HomomorphismChar(IDictionary<char, char> translate) {
            var neat = new NFAeTransform();
            char[] Alp = this.Alphabet.Clone() as char[];

            foreach (var dt in this.Transforms)
                if (translate.ContainsKey(dt.Key.c.Value))
                    neat.Add(dt.Key.q, translate[dt.Key.c.Value], dt.Value);
                else
                    neat.Add(dt.Key.q, dt.Key.c.Value, dt.Value);

            for (int i = 0; i < this.Alphabet.Length; i++)
                if (translate.ContainsKey(this.Alphabet[i]))
                    Alp[i] = translate[this.Alphabet[i]];

            return new NFA($"NFA_HomomorphismChar({Name})", StatesCount, Alp, neat, StartState, AcceptedStates);
        }
        
        public override IAutomat Intersect(IIntersect A) => throw new System.NotImplementedException();
        public override IAutomat Diff(IDiff A) => throw new System.NotImplementedException();

        #endregion

        public static explicit operator NFA(DFA D) {
            var neaet = new NFAeTransform();
            // Übergangsfunktion im NEA ist eine Menge, im Dea ein Element
            foreach (var dt in D.Transforms)
                neaet.Add(dt.Key.q, dt.Key.c.Value, dt.Value);

            return new NFA($"NFA_({D.Name})", D.StatesCount, D.Alphabet, neaet, D.StartState, D.AcceptedStates);
        }

        public override string ToString() => $"{Name} NEA(|{States.Length}|={string.Join(";", States)}, {{{string.Join(',', Alphabet)}}}, {{{Transforms}}}, {StartState}, {{{string.Join(',', AcceptedStates)}}})".Trim();


    }
}