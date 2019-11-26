using System;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {

    public class DFA : FABase, IAlleAbgeschlossenheitseigenschaften { // IAbgeschlossenheitseigenschaften<FABase, NFAe> {

        public DFA(string name, uint stateCount, char[] alphabet, DFATransform transform, uint startState, params uint[] acceptedStates)
            : base(stateCount, alphabet, transform, startState, acceptedStates, name) {
        }

        public DFA(string name, string[] states, char[] alphabet, DFATransform transform, uint startState, params uint[] acceptedStates)
            : base(states, alphabet, transform, startState, acceptedStates, name) {
        }

        protected uint[] GoChar(EATuple eat) => GoChar(eat.q, eat.c.Value);
        protected override uint[] GoChar(uint q, char w) {
            if (((DFATransform)Transforms).TryGetValue(q, w, out uint qNext))
                return new uint[] { qNext };
            else
                throw new NotImplementedException();
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            uint q = StartState;

            //process word through GoChar
            for (int i = 0; i < w.Length; i++)
                q = GoChar(q, w[i])[0];

            return IsAcceptedState(q);
        } //end AcceptWord

        protected override void CheckConstraints() {
            base.CheckConstraints();
            //basic length check
            if (Transforms.Count != StatesCount * Alphabet.Length)
                throw new Automat.DeterministicException("Tranformation Count missmatch State*Alphabet", this);

            //check every state, char
            for (uint q = 0; q < StatesCount; q++)
                foreach (char c in Alphabet)
                    if (!((DFATransform)Transforms).ContainsKey(q, c))
                        throw new Automat.DeterministicException($"q={q}, c={c} is missing", this);

            foreach (var t in Transforms) {
                if (t.Key.q > StatesCount | t.Value[0] > StatesCount) //to high state
                    throw new Automat.StateException(t.Key.q, this);
                if (!Alphabet.Contains(t.Key.c.Value)) //char not in alphabet
                    throw new Automat.AlphabetException(t.Key.c.Value, this);
            }
        } // end checkTransFormComplete

        public DFA MinimizeTF() {
            var tfEqClasses = TableFillingAlgEqClasses(this); //half matrix TF
            var State2eqClass = new uint[this.StatesCount];

            //Fill state to eqClass matching
            for (uint i = 0; i < tfEqClasses.Length; i++)
                for (int j = 0; j < tfEqClasses[i].Length; j++)
                    State2eqClass[tfEqClasses[i][j]] = i;

            //generate Transform, and modify to eqclasses
            var deaT = new DFATransform();
            foreach (var item in Transforms) {
                //q,qnext EqClass
                uint tSrcEqClass = State2eqClass[item.Key.q];
                uint tDstEqClass = State2eqClass[item.Value[0]];

                deaT.TryAdd(tSrcEqClass, item.Key.c.Value, tDstEqClass);
            }

            //add eqClasses of accepted states 
            var acc = new List<uint>(AcceptedStates.Length);
            for (uint i = 0; i < AcceptedStates.Length; i++)
                acc.Add(State2eqClass[AcceptedStates[i]]);

            acc.Sort();

            //give each onging state its eqClass states[] as name 
            string[] names = new string[tfEqClasses.Length];
            for (uint i = 0; i < tfEqClasses.Length; i++)
                names[i] = string.Join(',', tfEqClasses[i]);

            return new DFA($"DFA_Minimized({Name})", names, this.Alphabet, deaT, State2eqClass[this.StartState], acc.Distinct().ToArray());
        }


        // Alle Wörter müssen in der gleichen Partition enden (akzeptiert, nicht akzeptiert)
        [AlgorithmSource("1659_D29")]
        public bool StatesEqual(uint x, uint y) => StatesEqual(x, y, new List<uint>((int)StatesCount));

        bool StatesEqual(uint x, uint y, List<uint> processed) {
            if (!processed.Contains(x)) {
                processed.Add(x);

                //not in same Group, devider is e
                if (IsAcceptedState(x) != IsAcceptedState(y)) return false;

                //check for every char
                foreach (char c in Alphabet) {
                    //get Next States
                    uint x_next = GoChar(x, c)[0];
                    uint y_next = GoChar(y, c)[0];

                    //if this char is okay, other may not be
                    if (IsAcceptedState(x_next) != IsAcceptedState(y_next))
                        //c is devider, ending
                        return false;
                    else {
                        //go to next states for the word
                        bool r = StatesEqual(x_next, y_next, processed);
                        if (!r) return false;
                    }
                }
            }
            return true;
        } //end StatesEqual

        #region "Operations"

        /// <returns>
        /// D1 ∩ ⌐D2
        /// return Intersect(D1, Complement(D2));
        /// </returns>
        public static DFA Diff(DFA D1, DFA D2) =>
            ProductDea(D1, D2, eProductDeaMode.Diff);

        public override IAutomat HomomorphismChar(Dictionary<char, char> Translate) {
            var deat = new DFATransform();
            var Alp = (char[])this.Alphabet.Clone();

            foreach (var dt in (DFATransform)Transforms)
                if (Translate.ContainsKey(dt.Key.c.Value))
                    deat.Add(dt.Key.q, Translate[dt.Key.c.Value], dt.Value[0]);
                else
                    deat.Add(dt.Key.q, dt.Key.c.Value, dt.Value[0]);

            for (int i = 0; i < this.Alphabet.Length; i++)
                if (Translate.ContainsKey(this.Alphabet[i]))
                    Alp[i] = Translate[this.Alphabet[i]];

            return new DFA($"DFA_HomomorphismChar({Name})", this.StatesCount, Alp, deat, this.StartState, this.AcceptedStates);
        }

        public override IAutomat Join(IJoin A) {
            if (!(A is DFA D2))
                throw new NotSupportedException();
            if (!this.SameAlphabet(A))
                throw new NotImplementedException("Different Alphabets are not implemented");

            var deat = new DFATransform();

            var accStates = new List<uint>(this.AcceptedStates.Length + D2.AcceptedStates.Length);
            uint sc = this.StatesCount;

            foreach (var t in (DFATransform)Transforms)
                deat.Add(t.Key.q, t.Key.c.Value, t.Value[0]);
            foreach (var t in (DFATransform)D2.Transforms)
                deat.Add(t.Key.q + sc, t.Key.c.Value, t.Value[0] + sc);

            accStates.AddRange(this.AcceptedStates);
            for (int i = 0; i < D2.AcceptedStates.Length; i++)
                accStates.Add(D2.AcceptedStates[i] + sc);

            accStates.Sort();

            return new DFA($"DFA_Join({Name}+{A.Name})", (D2.StatesCount + sc), this.Alphabet, deat, this.StartState, accStates.ToArray());
        }

        public override IAutomat Union(IUnion A) => UnionProduct(this, (DFA)A);
        public override IAutomat Intersect(IIntersect A) => Intersect(this, (DFA)A);
        public override IAutomat Diff(IDiff A) => Diff(this, (DFA)A);

        public static DFA UnionProduct(DFA D1, DFA D2)
            => ProductDea(D1, D2, eProductDeaMode.Union);

        public static DFA Intersect(DFA D1, DFA D2)
            => ProductDea(D1, D2, eProductDeaMode.Intersect);

        public enum eProductDeaMode { Union, Intersect, Diff }
        public static DFA ProductDea(DFA D1, DFA D2, eProductDeaMode mode) {

            uint len = (D1.StatesCount * D2.StatesCount);

            if (!D1.SameAlphabet(D2))
                throw new NotImplementedException("Different Alphabets are not implemented");

            var accStates = new List<uint>(D1.AcceptedStates.Length + D2.AcceptedStates.Length);

            string[] stateNames = new string[len];

            var deat = new DFATransform();

            //iterate Cross D1xD2, chars
            for (uint i = 0; i < D1.StatesCount; i++) {
                for (uint j = 0; j < D2.StatesCount; j++) {
                    //index of state in matrix
                    uint index = (i * D2.StatesCount + j);
                    stateNames[index] = $"{i},{j}";

                    foreach (char c in D1.Alphabet) {
                        //Transform exists, out qNext
                        bool exist1 = ((DFATransform)D1.Transforms).TryGetValue(i, c, out uint qNext1);
                        bool exist2 = ((DFATransform)D2.Transforms).TryGetValue(j, c, out uint qNext2);

                        //same calc logic for dstIndex in Matrix
                        uint dstIndex;
                        if (exist1 & exist2)
                            dstIndex = (qNext1 * D2.StatesCount + qNext2);
                        else if (exist1)
                            dstIndex = (qNext1 * D2.StatesCount + qNext1);
                        else if (exist2)
                            dstIndex = qNext2;
                        else
                            throw new ApplicationException();

                        //add non existing tuple
                        if (!deat.ContainsKey(index, c)) // & exist1 & exist2)
                            deat.Add(index, c, dstIndex);
                        else
                            throw new ApplicationException();

                        if (mode == eProductDeaMode.Intersect) {
                            //add to accStates if one dea state ist acc
                            if (D1.IsAcceptedState(i) & D2.IsAcceptedState(j))
                                accStates.Add(index);
                        } else if (mode == eProductDeaMode.Union) {
                            //add to accStates if both dea state ist acc
                            if (D1.IsAcceptedState(i) | D2.IsAcceptedState(j))
                                accStates.Add(index);
                        } else if (mode == eProductDeaMode.Diff) {
                            //add to accStates if both dea state ist acc
                            if (D1.IsAcceptedState(i) & !D2.IsAcceptedState(j))
                                accStates.Add(index);
                        }
                    }
                }
            }

            return new DFA($"DEA_Product_({mode},{D1.Name}+{D2.Name})", stateNames, D1.Alphabet, deat, 0, accStates.Distinct().ToArray());
        }

        #endregion

        public static DFA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            int stateCount = rnd.Next(1, MAX_STATES);

            char[] alphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);


            var t = new DFATransform();
            for (uint i = 0; i < stateCount; i++) {
                char[] rndAlph = alphabet.Randomize();
                for (int j = 0; j < rndAlph.Length; j++)
                    t.Add(i, alphabet[j], (uint)rnd.Next(0, stateCount));
            }

            var ret = new DFA("DFA_Random", (uint)stateCount, alphabet, t, (uint)rnd.Next(0, stateCount), accState);
            ret.Name = $"DFA_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {

            (uint[] translate, string[] names, uint[] aStates) = base.RemovedStateTranslateTables();

            var newT = new DFATransform();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    foreach (var v in t2.Value)
                        if (translate.Contains(v))
                            newT.Add(translate.ArrayIndex(t2.Key.q), t2.Key.c.Value, translate.ArrayIndex(v));

            return new DFA($"{Name}_purged", names, Alphabet, newT, translate.ArrayIndex(StartState), aStates);
        }


        public static readonly DFA Empty = new DFA("DFA_Empty", new String[] { }, new char[] { }, new DFATransform(), 0, new uint[] { });

        public override bool Equals(object obj1) {
            DFA dobj;
            if (obj1 is DFA dobj1)
                dobj = dobj1;
            else if (obj1 is NFA nobj)
                dobj = Converter.Nea2TeilmengenDea(nobj);
            else if (obj1 is NFAe nobje)
                dobj = Converter.Nea2TeilmengenDea(nobje);
            else
                return false;

            var dd = Join(dobj) as DFA;
            return dd.StatesEqual(this.StartState, (dobj.StartState + this.StatesCount));

        }

        #region "Conversion"

        public static explicit operator DFA(NFA N) => Converter.Nea2TeilmengenDea(N);
        public static explicit operator DFA(NFAe Ne) => Converter.Nea2TeilmengenDea(Ne);

        #endregion

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString()
            => $"{Name} DEA(|{StatesCount}|={string.Join(";", States)}, {{{String.Join(',', Alphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {{{string.Join(',', AcceptedStates)}}})".Trim();

        /// <summary>
        /// Table Filling Algorithm to minimize a DEA
        /// </summary>
        /// <returns>complete bool Table showing eq pairs</returns>
        [AlgorithmSource("1659_S27_Algo1")]
        public static bool[,] TableFillingAlg(Finite.DFA D) {
            bool[,] t = new bool[D.StatesCount, D.StatesCount];

            // first, mark all states which differ between accepted and not
            for (uint x = 0; x < t.GetLength(0); x++)
                for (uint y = 0; y < t.GetLength(0); y++)
                    if (D.IsAcceptedState(x) != D.IsAcceptedState(y))
                        t[x, y] = true;

            bool found; // loop while found something new
            do {
                found = false;

                // iterate full table, for each char
                for (uint x = 0; x < t.GetLength(0); x++) {
                    for (uint y = x + 1; y < t.GetLength(1); y++) {
                        if (!t[x, y]) { // don't work already processed (PERF)
                            foreach (char c in D.Alphabet) {
                                uint xNext = D.GoChar(x, c)[0];
                                uint yNext = D.GoChar(y, c)[0];

                                // calculate based on previous iterations
                                // if next states for both x,y has been set to different
                                // set current pair to be different, and enable loop
                                if (t[xNext, yNext])
                                    t[x, y] = t[y, x] = found = true;
                            } // next c
                        } // end if
                    } // next y
                } // next x

            } while (found);

            return t;
        } //end function TableFillingAlg

        /// <summary>
        /// Table Filling Algorithm EQ Classes
        /// </summary>
        /// <returns>Returns all TF EQ Classes</returns>
        public static uint[][] TableFillingAlgEqClasses(Finite.DFA D) {
            var tf = TableFillingAlg(D);
            var eqClasses = new List<uint[]>((int)D.StatesCount);
            int?[] State2eqClass = new int?[D.StatesCount];

            // iterate table and process false(/double) values
            for (uint x = 0; x < tf.GetLength(0); x++) {
                for (uint y = x + 1; y < tf.GetLength(1); y++) {
                    if (!tf[x, y]) {
                        if (State2eqClass[x].HasValue) {
                            eqClasses[State2eqClass[x].Value] = eqClasses[State2eqClass[x].Value].Append(y).ToArray();
                            State2eqClass[y] = State2eqClass[x];
                        } else {
                            eqClasses.Add(new uint[] { x, y });
                            State2eqClass[x] = eqClasses.Count - 1;
                            State2eqClass[y] = eqClasses.Count - 1;
                        }
                    } // end if
                } // next y
            } // next x

            // add all none processed states
            for (uint i = 0; i < D.StatesCount; i++)
                if (!State2eqClass[i].HasValue)
                    eqClasses.Add(new uint[] { i });

            eqClasses.Sort((first, second) => first[0].CompareTo(second[0]));

            return eqClasses.ToArray();
        } //end function TableFillingAlgEqClasses

    } // end class
} //end ns