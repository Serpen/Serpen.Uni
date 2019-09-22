using System;
using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.Finite {
    public class NFAe : FABase, INFA, IAbgeschlossenheitseigenschaften<FABase, NFAe> {

        public static readonly FABase Empty = new NFAe("Empty", 0, new char[] { }, new NFAeTransform(), 0, new uint[] { });

        public NFAe(string name, uint stateCount, char[] Alphabet, NFAeTransform transform, uint startState, params uint[] acceptedStates)
            : base(stateCount, Alphabet, transform, startState, acceptedStates, name) {
        }

        public NFAe(string name, string[] states, char[] alphabet, NFAeTransform transform, uint startState, params uint[] acceptedStates)
            : base(states, alphabet, transform, startState, acceptedStates, name) {
        }

        #region "Operators"

        public static explicit operator NFAe(NFA A) {
            var t = new NFAeTransform();
            // Übergangsfunktion im NEA ist eine Menge, im Dea ein Element
            foreach (var t2 in A.Transform)
                t.Add(t2.Key.q, t2.Key.c, t2.Value);

            return new NFAe($"NFAe_({A.Name})", A.StatesCount, A.Alphabet, t, A.StartState, A.AcceptedStates);
        }

        public static explicit operator NFAe(DFA D) {
            var t = new NFAeTransform();
            // Übergangsfunktion im NEA ist eine Menge, im Dea ein Element
            foreach (var t2 in D.Transform)
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
                uint[] qNext; //Transform results
                if ((Transform as NFAeTransform).TryGetValue(t, out qNext))
                    retQ.AddRange(qNext);
            }
            retQ.Sort();

            //Final State + E-Huelle
            return EpsilonHuelle(retQ.Distinct().ToArray());
        }

        protected override uint[] GoChar(uint q, char w) {
            return GoChar(new uint[] { q }, w);
        }

        public uint[] EpsilonHuelle(uint q) {
            return EpsilonHuelle(new uint[] { q });
        }

        /// <summary>
        /// Epsilon-Huelle for a set of states
        /// </summary>
        /// <param name="q">States</param>
        /// <returns></returns>
        [System.ComponentModel.Description("EAFK-2.5.3")]
        public uint[] EpsilonHuelle(uint[] q) {
            var toProcess = new Stack<uint>(); //States that should be processed
            var Processed = new List<uint>(); //States that have beend processed

            //init every q to be processed
            for (int i = 0; i < q.Length; i++)
                toProcess.Push(q[i]);

            //loop while still items to process
            while (toProcess.Any()) {
                var curQ = toProcess.Pop();

                var t = new EATuple(curQ, null); //e Transform
                uint[] eQs;

                if ((Transform as NFAeTransform).TryGetValue(t, out eQs)) { //Exists e Transform?
                    for (int i = 0; i < eQs.Length; i++) //for every e Transform
                        if (!Processed.Contains(eQs[i])) //only add if not already processed
                            toProcess.Push(eQs[i]);
                }
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

            var rnd = Utils.RND;

            var t = new NFAeTransform();
            int stateCount = rnd.Next(1, MAX_STATES);
            char[] alphabet = new char[rnd.Next(1, MAX_CHAR)];
            for (int i = 0; i < alphabet.Length; i++)
                alphabet[i] = (char)rnd.Next('a', 'z');

            alphabet = alphabet.Distinct().ToArray();

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, alphabet.Length);
                for (int j = 0; j < transformsRnd; j++) {
                    var rndInt = rnd.Next(0, (int)System.Math.Max(alphabet.Length + 2, (int)alphabet.Length * 1.2));
                    EATuple k;
                    if (rndInt <= alphabet.Length)
                        k = new EATuple(i, alphabet[rndInt]);
                    else
                        k = new EATuple(i, null);
                    t.AddM(i, alphabet[j], (uint)rnd.Next(0, stateCount));
                }
            }

            uint[] accState = new uint[rnd.Next(1, System.Math.Max(1, stateCount / 3))];
            for (int i = 0; i < accState.Length; i++)
                accState[i] = (uint)rnd.Next(0, stateCount);

            accState = accState.Distinct().ToArray();

            var ret = new NFAe("NFAe_Random", (uint)stateCount, alphabet, t, (uint)rnd.Next(0, stateCount), accState);
            ret.Name = $"NFAe_Random_{ret.GetHashCode()}";
            return ret;

        }


        public override FABase HomomorphismChar(Dictionary<char, char> Translate) {
            var neat = new NFAeTransform();
            var Alp = (char[])this.Alphabet.Clone();

            foreach (var dt in this.Transform)
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

        public static NFAe KleeneStern(NFAe D) {
            var neaET = new NFAeTransform();

            //accepted front state
            neaET.Add(0, null, 1);

            //add transforms, +1
            foreach (var item in D.Transform) {
                uint[] qnext = new uint[item.Value.Length];
                for (int i = 0; i < item.Value.Length; i++)
                    qnext[i] = item.Value[i] + 1;
                neaET.Add(item.Key.q + 1, item.Key.c, qnext);
            }

            //add e-transform from org accepted to front state
            for (uint i = 0; i < D.AcceptedStates.Length; i++)
                neaET.Add(D.AcceptedStates[i] + 1, null, 0);

            return new NFAe($"NFAe_KleeneStern({D.Name})", D.StatesCount, D.Alphabet, neaET, 0, 0);
        }

        #region Ops

        public override FABase Join(FABase A) {
            var N2 = A as NFAe;

            if (!(N2 is null)) {
                var neat = new NFAeTransform();

                if (!Utils.SameAlphabet(this, A)) {
                    throw new NotImplementedException("Different Alphabets are not implemented");
                }

                var accStates = new List<uint>(this.AcceptedStates.Length + N2.AcceptedStates.Length);
                uint sc = this.StatesCount;

                foreach (var t in this.Transform)
                    neat.Add(t.Key.q, t.Key.c, t.Value);
                foreach (var t in N2.Transform) {
                    uint[] qnexts = new uint[t.Value.Length];
                    for (int i = 0; i < t.Value.Length; i++)
                        qnexts[i] = t.Value[i] + sc;
                    neat.Add(t.Key.q + sc, t.Key.c, qnexts);
                }

                accStates.AddRange(this.AcceptedStates);
                for (int i = 0; i < N2.AcceptedStates.Length; i++)
                    accStates.Add(N2.AcceptedStates[i] + sc);

                accStates.Sort();

                return new NFAe($"NFAe_Join({Name}+{A.Name})", N2.StatesCount + sc, this.Alphabet, neat, this.StartState, accStates.ToArray());
            } else
                throw new NotSupportedException();
        }

        #endregion

        public override bool Equals(object obj1) {
            DFA dobj;
            if (obj1 is DFA)
                dobj = obj1 as DFA;
            else if (obj1 is NFA)
                dobj = Converter.Nea2TeilmengenDea(obj1 as NFA);
            else if (obj1 is NFAe)
                dobj = Converter.Nea2TeilmengenDea(obj1 as NFAe);
            else
                return false;

            DFA thisobj = Converter.Nea2TeilmengenDea(this);
            return thisobj.Equals(dobj);
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            return $"{Name} NEAe(|{StatesCount}|={string.Join(";", States)}), {{{String.Join(',', Alphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {{{string.Join(',', AcceptedStates)}}})".Trim();
        }

        public FABase Union(FABase A) => throw new System.NotImplementedException();
        public FABase Intersect(FABase A) => throw new System.NotImplementedException();
        public FABase Diff(FABase A) => throw new System.NotImplementedException();

    } //end class
} //end ns