using System;
using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.Finite {

    [System.Serializable]
    public class NFAe : FABase, INFA {

        public static readonly FABase Empty = new NFAe("Empty", 1, Array.Empty<char>(), new NFAeTransform(), 0, Array.Empty<uint>());

        public NFAe(string name, uint stateCount, char[] Alphabet, NFAeTransform transform, uint startState, params uint[] acceptedStates)
            : base(name, stateCount, Alphabet, transform, startState, acceptedStates) {
        }

        public NFAe(string name, string[] states, char[] alphabet, NFAeTransform transform, uint startState, params uint[] acceptedStates)
            : base(name, states, alphabet, transform, startState, acceptedStates) {
        }

        #region "Conversions"

        public static explicit operator NFAe(NFA A) {
            var t = new NFAeTransform();
            // Übergangsfunktion im NEA ist eine Menge, im Dea ein Element
            foreach (var t2 in A.Transforms)
                t.Add(t2.Key.q, t2.Key.c, t2.Value);

            return new NFAe($"NFAe_({A.Name})", A.StatesCount, A.Alphabet, t, A.StartState, A.AcceptedStates);
        }

        public static explicit operator NFAe(DFA D) {
            var t = new NFAeTransform();
            // Übergangsfunktion im NEA ist eine Menge, im Dea ein Element
            foreach (var t2 in D.Transforms)
                t.Add(t2.Key.q, t2.Key.c, t2.Value);

            return new NFAe($"NFAe_({D.Name})", D.StatesCount, D.Alphabet, t, D.StartState, D.AcceptedStates);
        }

        #endregion

        uint[] INFA.GoChar(uint[] q, char w) => GoChar(q, w);

        /// <summary>
        /// Go a single char
        /// </summary>
        /// <param name="q">start States</param>
        /// <param name="w">Char to go</param>
        /// <returns>Set of possible states</returns>
        internal uint[] GoChar(uint[] q, char w) {
            var retQ = new List<uint>(); // store all possible q-Next

            //combine to E-Huelle of all states
            var eh = EpsilonHuelle(q);

            for (int i = 0; i < eh.Length; i++) //iterate q+e(q)
            {
                var t = new EATuple(eh[i], w); //e Transform
                if (Transforms.TryGetValue(t, out uint[] qNext))
                    retQ.AddRange(qNext);
            }
            retQ.Sort();

            //Final State + E-Huelle
            return EpsilonHuelle(retQ.Distinct().ToArray());
        }

        protected override uint[] GoChar(uint q, char w) => GoChar(new uint[] { q }, w);

        public uint[] EpsilonHuelle(uint q) => EpsilonHuelle(new uint[] { q });

        /// <summary>
        /// Epsilon-Huelle for a set of states
        /// </summary>
        /// <param name="q">States</param>
        /// <returns></returns>
        [AlgorithmSource("EAFK-2.5.3")]
        public uint[] EpsilonHuelle(uint[] q) {
            var toProcess = new Stack<uint>((int)StatesCount); //States that should be processed
            var Processed = new List<uint>((int)StatesCount); //States that have beend processed

            //init every q to be processed
            for (int i = 0; i < q.Length; i++)
                toProcess.Push(q[i]);

            //loop while still items to process
            while (toProcess.Any()) {
                var curQ = toProcess.Pop();

                var t = new EATuple(curQ, null); //e Transform

                if (Transforms.TryGetValue(t, out uint[] eQs))  //Exists e Transform?
                    foreach (var eQ in eQs) //for every e Transform
                        if (!Processed.Contains(eQ)) //only add if not already processed
                            toProcess.Push(eQ);

                Processed.Add(curQ);
            }

            Processed.Sort();
            return Processed.ToArray();
        }

        /// <summary>
        /// Checks wether a word ends in accepted state
        /// </summary>
        /// <param name="w">Word to Process</param>
        /// <returns></returns>
        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            uint[] q = EpsilonHuelle(StartState); //Init Startstate

            //For every letter
            for (int i = 0; i < w.Length; i++) {
                q = GoChar(q, w[i]); //Go with i-Letter and E-Huelle
                if (q.Length == 0) //Sackgasse
                    return false;
            }

            //Intersection between possible States and accepted states exists?
            return q.Intersect(AcceptedStates).Any();
        }

        public static NFAe GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            int stateCount = rnd.Next(1, MAX_STATES);

            char[] alphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);

            var t = new NFAeTransform();
            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, alphabet.Length);
                for (int j = 0; j < transformsRnd; j++)
                    t.AddM(i, alphabet.RndElement(), (uint)rnd.Next(0, stateCount));
            }

            return new NFAe( $"NFAe_Random_q{stateCount}_a{alphabet.Length}_t{t.Count}_a{accState.Length}", (uint)stateCount, alphabet, t, (uint)rnd.Next(0, stateCount), accState);
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.RemovedStateTranslateTables();

            var newT = new NFAeTransform();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    foreach (var v in t2.Value)
                        if (translate.Contains(v))
                            newT.AddM(translate.ArrayIndex(t2.Key.q), t2.Key.c, translate.ArrayIndex(v));

            return new NFAe($"{Name}_purged", names, Alphabet, newT, translate.ArrayIndex(StartState), aStates);
        }

        #region "Operations"
        public override IAutomat HomomorphismChar(IDictionary<char, char> Translate) {
            var neat = new NFAeTransform();
            var Alp = (char[])this.Alphabet.Clone();

            foreach (var dt in this.Transforms)
                if (dt.Key.c.HasValue) {
                    if (Translate.ContainsKey(dt.Key.c.Value))
                        neat.Add(dt.Key.q, Translate[dt.Key.c.Value], dt.Value);
                    else
                        neat.Add(dt.Key.q, dt.Key.c, dt.Value);

                } else
                    neat.Add(dt.Key.q, null, dt.Value);

            for (int i = 0; i < this.Alphabet.Length; i++)
                if (Translate.ContainsKey(this.Alphabet[i]))
                    Alp[i] = Translate[this.Alphabet[i]];

            return new NFAe($"NFAe_HomomorphismChar({Name})", this.StatesCount, Alp, neat, this.StartState, this.AcceptedStates);
        }

        public override IAutomat Intersect(IIntersect A) => throw new System.NotImplementedException();
        public override IAutomat Diff(IDiff A) => throw new System.NotImplementedException();

        #endregion

        public override bool Equals(object obj1) {
            DFA dobj;
            if (obj1 is DFA)
                dobj = obj1 as DFA;
            else if (obj1 is NFA)
                dobj = DFA.Nea2TeilmengenDea(obj1 as NFA);
            else if (obj1 is NFAe)
                dobj = DFA.Nea2TeilmengenDea(obj1 as NFAe);
            else
                return false;

            DFA thisobj = DFA.Nea2TeilmengenDea(this);
            return thisobj.Equals(dobj);
        }

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString()
            => $"{Name} NEAe(|{StatesCount}|={string.Join(";", States)}, {{{String.Join(',', Alphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {{{string.Join(',', AcceptedStates)}}})".Trim();

    } //end class
} //end ns