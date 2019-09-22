using System;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {

    public class DFA : FABase, IAbgeschlossenheitseigenschaften<FABase, NFAe> {
        public Dictionary<Tuple<uint, string>, uint> cachedExtendTransform = new Dictionary<Tuple<uint, string>, uint>();

        public DFA(string name, uint stateCount, char[] alphabet, FATransform transform, uint startState, params uint[] acceptedStates)
            : base(stateCount, alphabet, transform, startState, acceptedStates, name) {
            CheckConstraints();
        }


        public DFA(string name, string[] states, char[] alphabet, FATransform transform, uint startState, params uint[] acceptedStates)
            : base(states, alphabet, transform, startState, acceptedStates, name) {
            CheckConstraints();
        }

        protected uint[] GoChar(EATuple eat) => GoChar(eat.q, eat.c.Value);
        protected override uint[] GoChar(uint q, char w) {

            uint qNext;
            if (((FATransform)Transform).TryGetValue(q, w, out qNext))
                return new uint[] { qNext };
            else
                throw new NotImplementedException();
        }

        public override bool AcceptWord(string w) {
            uint q = StartState;

            //check if word is valid
            for (int i = 0; i < w.Length; i++)
                if (!Alphabet.Contains(w[i]))
                    throw new AlphabetException(w[i]);

            var extTuple = new Tuple<uint, string>(StartState, w);

            if (!cachedExtendTransform.ContainsKey(extTuple)) {
                //process word through GoChar
                for (int i = 0; i < w.Length; i++)
                    q = GoChar(q, w[i])[0];
                cachedExtendTransform.Add(extTuple, q);
            } else
                q = cachedExtendTransform[extTuple];

            return IsAcceptedState(q);
        } //end AcceptWord

        protected override void CheckConstraints() {
            base.CheckConstraints();
            //basic length check
            if (Transform.Count != StatesCount * Alphabet.Length)
                throw new Automat.DeterministicException($"Tranformation Count missmatch State*Alphabet");

            //check every state, char
            for (uint q = 0; q < StatesCount; q++)
                foreach (char c in Alphabet)
                    if (!((FATransform)Transform).ContainsKey(q, c))
                        throw new Automat.DeterministicException($"q={q}, c={c} is missing");

            foreach (var t in (FATransform)Transform) {
                if (t.Key.q > StatesCount | t.Value[0] > StatesCount) //to high state
                    throw new Automat.StateException(t.Key.q);
                if (!Alphabet.Contains(t.Key.c.Value)) //char not in alphabet
                    throw new Automat.AlphabetException(t.Key.c.Value);
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
            var deaT = new FATransform();
            foreach (var item in (FATransform)Transform) {
                //q,qnext EqClass
                uint tSrcEqClass = State2eqClass[item.Key.q];
                uint tDstEqClass = State2eqClass[item.Value[0]];

                deaT.TryAdd(tSrcEqClass, item.Key.c.Value, tDstEqClass);
            }

            //add eqClasses of accepted states 
            var acc = new List<uint>();
            for (uint i = 0; i < AcceptedStates.Length; i++)
                acc.Add(State2eqClass[AcceptedStates[i]]);

            acc.Sort();

            //return obj

            //give each onging state its eqClass states[] as name 
            string[] names = new string[tfEqClasses.Length];
            for (uint i = 0; i < tfEqClasses.Length; i++)
                names[i] = string.Join(',', tfEqClasses[i]);

            return new DFA($"DFA_Minimized({Name})", names, this.Alphabet, deaT, 0, acc.Distinct().ToArray());
        }

        // Alle Wörter müssen in der gleichen Partition enden (akzeptiert, nicht akzeptiert)
        public bool StatesEqual(uint x, uint y) => StatesEqual(x, y, new List<uint>());

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

        /// <returns>
        /// D1 ∩ ⌐D2
        /// return Intersect(D1, Complement(D2));
        /// </returns>
        public static DFA Diff(DFA D1, DFA D2) =>
            ProductDea(D1, D2, eProductDeaMode.Diff);

        public override FABase HomomorphismChar(Dictionary<char, char> Translate) {
            var deat = new FATransform();
            var Alp = (char[])this.Alphabet.Clone();

            foreach (var dt in (FATransform)Transform)
                if (Translate.ContainsKey(dt.Key.c.Value))
                    deat.Add(dt.Key.q, Translate[dt.Key.c.Value], dt.Value[0]);
                else
                    deat.Add(dt.Key.q, dt.Key.c.Value, dt.Value[0]);

            for (int i = 0; i < this.Alphabet.Length; i++)
                if (Translate.ContainsKey(this.Alphabet[i]))
                    Alp[i] = Translate[this.Alphabet[i]];

            return new DFA($"DFA_HomomorphismChar({Name})", this.StatesCount, Alp, deat, this.StartState, this.AcceptedStates);
        }

        public override FABase Join(FABase A) {
            var D2 = A as DFA;

            if (D2 != null) {
                var deat = new FATransform();

                if (!Utils.SameAlphabet(this, A)) {
                    throw new NotImplementedException("Different Alphabets are not implemented");
                }

                var accStates = new List<uint>(this.AcceptedStates.Length + D2.AcceptedStates.Length);
                uint sc = this.StatesCount;

                foreach (var t in (FATransform)Transform)
                    deat.Add(t.Key.q, t.Key.c.Value, t.Value[0]);
                foreach (var t in (FATransform)D2.Transform)
                    deat.Add(t.Key.q + sc, t.Key.c.Value, t.Value[0] + sc);

                accStates.AddRange(this.AcceptedStates);
                for (int i = 0; i < D2.AcceptedStates.Length; i++)
                    accStates.Add(D2.AcceptedStates[i] + sc);

                accStates.Sort();

                return new DFA($"DFA_Join({Name}+{A.Name})", (D2.StatesCount + sc), this.Alphabet, deat, this.StartState, accStates.ToArray());
            } else
                throw new NotSupportedException();
        }

        FABase IAbgeschlossenheitseigenschaften<FABase, NFAe>.Union(FABase A) => UnionProduct(this, (DFA)A);
        FABase IAbgeschlossenheitseigenschaften<FABase, NFAe>.Intersect(FABase A) => Intersect(this, (DFA)A);
        FABase IAbgeschlossenheitseigenschaften<FABase, NFAe>.Diff(FABase A) => Diff(this, (DFA)A);

        public static DFA UnionProduct(DFA D1, DFA D2)
            => ProductDea(D1, D2, eProductDeaMode.Union);

        public static DFA Intersect(DFA D1, DFA D2)
            => ProductDea(D1, D2, eProductDeaMode.Intersect);

        public enum eProductDeaMode { Union, Intersect, Diff }
        public static DFA ProductDea(DFA D1, DFA D2, eProductDeaMode mode) {

            uint len = (D1.StatesCount * D2.StatesCount);

            if (Utils.SameAlphabet(D1, D2)) {
                char[] Alphabet = D1.Alphabet;
                var accStates = new List<uint>();
                string[] stateNames = new string[len];

                var deat = new FATransform();

                //iterate Cross D1xD2, chars
                for (uint i = 0; i < D1.StatesCount; i++) {
                    for (uint j = 0; j < D2.StatesCount; j++) {
                        //index of state in matrix
                        uint index = (i * D2.StatesCount + j);
                        stateNames[index] = $"{i},{j}";

                        foreach (char c in Alphabet) {
                            uint qNext1, qNext2; //next states for D1, D2

                            //tuple for D1,D2

                            //Transform exists, out qNext
                            var exist1 = ((FATransform)D1.Transform).TryGetValue(i, c, out qNext1);
                            var exist2 = ((FATransform)D2.Transform).TryGetValue(j, c, out qNext2);

                            //same calc logic for dstIndex
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

                return new DFA($"DEA_Product_({mode},{D1.Name}+{D2.Name})", stateNames, Alphabet, deat, 0, accStates.Distinct().ToArray());
            } else
                throw new NotImplementedException("Different Alphabets are not implemented");
        }

        public static readonly DFA Empty = new DFA("DFA_Empty", new String[] { }, new char[] { }, new FATransform(), 0, new uint[] { });

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

        

        public static explicit operator DFA(NFA N) => Converter.Nea2TeilmengenDea(N);
        public static explicit operator DFA(NFAe Ne) => Converter.Nea2TeilmengenDea(Ne);
        
        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString()
            => $"{Name} DEA(|{StatesCount}|={string.Join(";", States)}), {{{String.Join(',', Alphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {{{string.Join(',', AcceptedStates)}}})".Trim();

        /// <summary>
        /// Table Filling Algorithm for minimize a DEA
        /// </summary>
        /// <returns>complete bool Table showing eq pairs</returns>
        internal static bool[,] TableFillingAlg(Finite.DFA D) {
            bool[,] t = new bool[D.States.Length, D.States.Length];

            //first, mark all states which differ between accepted and not
            for (uint x = 0; x < t.GetLength(0); x++)
                for (uint y = 0; y < t.GetLength(0); y++)
                    if (D.IsAcceptedState(x) != D.IsAcceptedState(y))
                        t[x, y] = true;

            bool found; //loop while found something new
            do {
                found = false;

                //iterate full table, for each char
                for (uint x = 0; x < t.GetLength(0); x++) {
                    for (uint y = 0; y < t.GetLength(1); y++) { //y=0/1 works, y=x+1 works incorrectly
                        foreach (char c in D.Alphabet) {
                            uint xNext = D.GoChar(x, c)[0];
                            uint yNext = D.GoChar(y, c)[0];

                            //calculate based on previous iterations
                            //if next states for both x,y has been set to different and not already processed
                            //set current pair to be different, and enable loop
                            if (t[xNext, yNext] & !t[x, y]) {
                                t[x, y] = true;
                                found = true;
                            }
                        }
                    }
                }
            } while (found);
            return t;
        } //end function TableFillingAlg

        /// <summary>
        /// Table Filling Algorithm EQ Classes
        /// </summary>
        /// <returns>Returns all TF EQ Classes</returns>
        internal static uint[][] TableFillingAlgEqClasses(Finite.DFA D) {
            var tf = TableFillingAlg(D);
            bool[] qAlready = new bool[tf.GetLength(0)]; //state already added
            var eqClasses = new System.Collections.Generic.List<uint[]>();
            var State2eqClass = new uint?[D.States.Length];

            //iterate table and process false(/double) values
            for (uint x = 0; x < tf.GetLength(0); x++)
                for (uint y = x + 1; y < tf.GetLength(1); y++)
                    if (!tf[x, y]) {
                        //add class and mark as processed

                        int? xEqClass = (int?)State2eqClass[x];
                        int? yEqClass = (int?)State2eqClass[y];

                        //TODO: bug buggy
                        if (qAlready[x] & xEqClass.HasValue & !yEqClass.HasValue) {
                            eqClasses[xEqClass.Value] = eqClasses[xEqClass.Value].Append(y).ToArray();
                            qAlready[y] = true;
                            State2eqClass[y] = (uint)xEqClass;
                        } else if (qAlready[y] & yEqClass.HasValue & !yEqClass.HasValue) {
                            eqClasses[yEqClass.Value] = eqClasses[yEqClass.Value].Append(x).ToArray();
                            qAlready[x] = true;
                            State2eqClass[x] = (uint)yEqClass;
                        } else if (!qAlready[x] & !qAlready[y]) {
                            eqClasses.Add(new uint[] { x, y });
                            qAlready[x] = qAlready[y] = true;
                            State2eqClass[x] = (uint)eqClasses.Count - 1;
                            State2eqClass[y] = (uint)eqClasses.Count - 1;
                        }

                    }

            //add all none processed states
            for (uint i = 0; i < qAlready.Length; i++)
                if (!qAlready[i])
                    eqClasses.Add(new uint[] { i });

            eqClasses.Sort((first, second) => first[0].CompareTo(second[0]));

            return eqClasses.ToArray();
        } //end function

    } // end class
} //end ns