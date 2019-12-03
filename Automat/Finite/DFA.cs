using System;
using System.Collections.Generic;
using System.Linq;
using RegExText = System.Text.RegularExpressions.Regex;

namespace Serpen.Uni.Automat.Finite {

    [System.Serializable]
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
                if (t.Key.q > StatesCount) //to high state
                    throw new Automat.StateException(t.Key.q, this);
                if (t.Value[0] > StatesCount) //to high state
                    throw new Automat.StateException(t.Value[0], this);
                if (!Alphabet.Contains(t.Key.c.Value)) //char not in alphabet
                    throw new Automat.AlphabetException(t.Key.c.Value, this);
            }
        } // end checkTransFormComplete

        /// <summary>
        /// Minimize DFA with TableFillingAlg
        /// </summary>
        /// <returns></returns>
        public DFA Minimize() {
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
        [AlgorithmSource("1659_D2.9")]
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

        public override IAutomat Union(IUnion A) => ProductDea(this, (DFA)A, eProductDeaMode.Union);
        public IAutomat UnionNea(IUnion A) => base.Union(A);
        public override IAutomat Intersect(IIntersect A) => ProductDea(this, (DFA)A, eProductDeaMode.Intersect);
        public override IAutomat Diff(IDiff A) => ProductDea(this, (DFA)A, eProductDeaMode.Diff);

        public enum eProductDeaMode { Union, Intersect, Diff }

        [AlgorithmSource("1659_S2.8_P44")]
        public static DFA ProductDea(DFA D1, DFA D2, eProductDeaMode mode) {

            uint len = (D1.StatesCount * D2.StatesCount);

            char[] newAlphabet = D1.Alphabet.Union(D2.Alphabet).ToArray();
            // if (!D1.SameAlphabet(D2))
            //     throw new NotImplementedException("Different Alphabets are not implemented");

            var accStates = new List<uint>(D1.AcceptedStates.Length + D2.AcceptedStates.Length);

            string[] stateNames = new string[len];

            var deat = new DFATransform();

            //iterate Cross D1xD2, chars
            for (uint i = 0; i < D1.StatesCount; i++) {
                for (uint j = 0; j < D2.StatesCount; j++) {
                    //index of state in matrix
                    uint index = (i * D2.StatesCount + j);
                    stateNames[index] = $"{i},{j}";

                    foreach (char c in newAlphabet) {
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

            return new DFA($"DEA_Product{mode}({D1.Name}+{D2.Name})", stateNames, newAlphabet, deat, 0, accStates.Distinct().ToArray());
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


        public static readonly DFA Empty = new DFA("DFA_Empty", 1, new char[] { }, new DFATransform(), 0, new uint[] { });

        public override bool Equals(object obj1) {
            DFA dobj;
            if (obj1 is DFA dobj1)
                dobj = dobj1;
            else if (obj1 is NFA nobj)
                dobj = Nea2TeilmengenDea(nobj);
            else if (obj1 is NFAe nobje)
                dobj = Nea2TeilmengenDea(nobje);
            else
                return false;

            var dd = Join(dobj) as DFA;
            return dd.StatesEqual(this.StartState, (dobj.StartState + this.StatesCount));

        }

        #region "Conversion"

        public static explicit operator DFA(NFA N) => Nea2TeilmengenDea(N);
        public static explicit operator DFA(NFAe Ne) => Nea2TeilmengenDea(Ne);

        [AlgorithmSource("EAFK-2.3.5", "EAFK-2.5.5","1659-D-2.8")]
        public static Finite.DFA Nea2TeilmengenDea(Finite.INFA N) {
            var States = new List<uint[]>((int)N.StatesCount);
            var DeaStatesNames2Index = new Dictionary<string, uint>((int)N.StatesCount);
            var States2Check = new Queue<uint[]>((int)N.StatesCount);
            var t = new Finite.DFATransform();
            var accStates = new List<uint>(N.AcceptedStates.Length);

            // Add first state to DFA states, for check and naming
            uint[] qStart;
            if (N is Finite.NFAe Ne)
                qStart = Ne.EpsilonHuelle(N.StartState);
            else
                qStart = new uint[] { N.StartState };

            States.Add(qStart);
            States2Check.Enqueue(qStart);
            DeaStatesNames2Index.Add(string.Join(',', qStart), 0);

            // if start State is also acceptable
            if (qStart.Intersect(N.AcceptedStates).Any())
                accStates.Add(0);

            // ?find all states for checking
            while (States2Check.Any()) {
                var q = States2Check.Dequeue();

                foreach (var w in N.Alphabet) { // every existing transform for char
                    var qNext = N.GoChar(q, w);
                    var qStr = string.Join(",", qNext);
                    if (!DeaStatesNames2Index.ContainsKey(qStr)) {
                        States.Add(qNext);
                        DeaStatesNames2Index.Add(qStr, (uint)States.Count - 1);
                        if (qNext.Intersect(N.AcceptedStates).Any())
                            accStates.Add((uint)States.Count - 1);

                        if (!(from s in States2Check where s.SequenceEqual(qNext) select s).Any())
                            States2Check.Enqueue(qNext);
                    }
                }
            }

            // calc transform
            for (uint i = 0; i < States.Count; i++) { // iterate every state
                foreach (var w in N.Alphabet) { // iterate alphabet
                    var x = N.GoChar(States[(int)i], w);
                    var XStr = string.Join(',', x);
                    var xPos = DeaStatesNames2Index[XStr];

                    if (!t.ContainsKey(i, w))
                        t.Add(i, w, xPos);
                }
            }

            // Naming the DFA states, with the NFA state set
            string[] names = new string[States.Count];
            for (int i = 0; i < names.Length; i++)
                names[i] = string.Join(",", States[i]);

            return new Finite.DFA($"DEA_PotenzMenge({N.Name})", names, N.Alphabet, t, 0, accStates.ToArray());

        }

        /// <summary>
        /// Returns the NOT minimal RegEx for a given DFA
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        [AlgorithmSource("1659-S-2.11")]
        public static string DEA2RegExp(Finite.DFA D) {
            //represents R_ij^(k) From i to j, without State higher than k
            string[,,] R = new string[D.States.Length, D.States.Length, D.States.Length + 1];

            //first k! then i, j
            for (uint k = 0; k < R.GetLength(2); k++) {
                for (uint i = 0; i < R.GetLength(0); i++) {
                    for (uint j = 0; j < R.GetLength(0); j++) {
                        // k==0 calculates Transforms for direct contact
                        if (k == 0) {
                            var toAdd = new List<string>();
                            if (i == j)  //from i to i, means ε 
                                toAdd.Add(Utils.EPSILON.ToString());

                            //check if any char loops in this state
                            foreach (char c in D.Alphabet) {
                                if (((Finite.DFATransform)D.Transforms).TryGetValue(i, c, out uint qNext) & qNext == j)
                                    toAdd.Add(c.ToString());
                            }
                            R[i, j, 0] = string.Join('+', toAdd);
                            if (R[i, j, 0] == Utils.EPSILON.ToString()) R[i, j, 0] = "";

                        } else {
                            //R is now calculated by previous R^(k-1)
                            //Rijk = Rijk+Rikk Rkkk* Rjkk
                            var Rijk = R[i, j, k - 1];
                            var Rikk = R[i, k - 1, k - 1];
                            var Rkkk = R[k - 1, k - 1, k - 1];
                            var Rkjk = R[k - 1, j, k - 1];

                            //try some magic to shorten formula
                            if (Rikk == Rkkk | Rikk == $"({Rkkk})" | $"({Rikk}" == Rkkk)
                                Rikk = string.Empty;
                            if (Rkjk == Rkkk | Rkjk == $"({Rkkk})" | $"({Rkjk}" == Rkkk)
                                Rkjk = string.Empty;
                            if ($"{Rikk}{Rkkk}{Rkjk}" == Rijk)
                                Rijk = "";
                            if (Rijk != string.Empty) Rijk = $"({Rijk})+";
                            if (Rikk != string.Empty) Rikk = $"({Rikk})";
                            if (Rkjk != string.Empty) Rkjk = $"({Rkjk})";
                            if (Rkkk != string.Empty) Rkkk = $"({Rkkk})*";

                            R[i, j, k] = $"{Rijk}{Rikk}{Rkkk}{Rkjk}";

                            if (R[i, j, k] == "()*") R[i, j, k] = string.Empty;

                            //try some magic to shorten formula with regex
                            R[i, j, k] = RegExText.Replace(R[i, j, k], @"\((\w)\)", "$1");
                            R[i, j, k] = RegExText.Replace(R[i, j, k], @"\((\w)\*\)", "$1*");
                            //R[i,j,k] = RegExText.Replace(R[i,j,k],@"\(ε\+(\w)\)\*","$1*");

                        }
                    } //next j
                } //next i
            } //next k

            return R[D.StartState, D.AcceptedStates[0], R.GetLength(2) - 1];
        } //end function

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