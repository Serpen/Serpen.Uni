using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {

    public abstract class FABase : AutomatBase<EATuple, uint[]>, IAlleAbgeschlossenheitseigenschaften {

        public FABase(uint stateCount, char[] alphabet, TransformBase<EATuple, uint[]> eat, uint startState, uint[] acceptedStates, string name = "")
        : base(stateCount, alphabet, startState, name, acceptedStates) {
            this.Transform = eat;
            CheckConstraints();
        }

        public FABase(string[] names, char[] alphabet, TransformBase<EATuple, uint[]> eat, uint startState, uint[] acceptedStates, string name = "")
        : base(names, alphabet, startState, name, acceptedStates) {
            this.Transform = eat;
            CheckConstraints();
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
        }

        protected abstract uint[] GoChar(uint q, char c); //maybe return only uint

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new List<System.Tuple<int, int, string>>();
            foreach (var t in Transform) {
                string desc = t.Key.c.ToString();
                if (desc == "")
                    desc = Utils.EPSILON.ToString();
                foreach (uint v in t.Value) {
                    var vt = new System.Tuple<int, int, string>((int)t.Key.q, (int)v, desc);
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public IAutomat Complement() {
            var Dc = this.MemberwiseClone() as FABase;
            var accStates = new List<uint>();

            for (uint i = 0; i < this.States.Length; i++)
                if (!this.IsAcceptedState(i))
                    accStates.Add(i);

            Dc.AcceptedStates = accStates.ToArray();
            return Dc;
        }


        public IAutomat Concat(IConcat automat) {
            var A = automat as FABase;
            if (A == null)
                throw new System.NotSupportedException();

            var neaET = new NFAeTransform();

            var accStates = new List<uint>(A.AcceptedStates.Length);
            uint offset = this.StatesCount;

            foreach (var t in this.Transform)
                neaET.Add(t.Key.q, t.Key.c, t.Value);

            foreach (var t in A.Transform) {
                uint[] qnexts = new uint[t.Value.Length];
                for (int i = 0; i < t.Value.Length; i++)
                    qnexts[i] = t.Value[i] + offset;
                neaET.Add(t.Key.q + offset, t.Key.c, qnexts);
            }


            for (int i = 0; i < this.AcceptedStates.Length; i++)
                neaET.Add(this.AcceptedStates[i], null, offset);

            for (int i = 0; i < A.AcceptedStates.Length; i++)
                accStates.Add((A.AcceptedStates[i] + offset));

            accStates.Sort();

            char[] bothAlphabets = this.Alphabet.Union(A.Alphabet).ToArray();

            return new NFAe($"Concat({Name}+{A.Name})", A.StatesCount + offset, bothAlphabets, neaET, this.StartState, accStates.ToArray());
        }


        /// <summary>
        /// Unions two EAs to a NEA, which has a StartState with e to both Start States
        /// </summary>
        /// <returns></returns>
        public NFAe UnionNEA(FABase fa) {
            uint offsetD2 = this.StatesCount + 1; //first state of D2
            uint[] accStates = new uint[this.AcceptedStates.Length + fa.AcceptedStates.Length];
            char[] nAlphabet = this.Alphabet.Union(fa.Alphabet).ToArray();

            var neat = new NFAeTransform();
            //add new start state, with e to both starts
            neat.Add(0, null, 1, offsetD2);

            //add each D1 transform, +1
            foreach (var item in this.Transform)
                neat.Add(item.Key.q + 1, item.Key.c, item.Value[0] + 1);

            //add each D1 transform, +offset
            foreach (var item in fa.Transform)
                neat.Add(item.Key.q + offsetD2, item.Key.c, item.Value[0] + offsetD2);


            int i; //store where D1 acc ends
            //iterate D1 acc and +1
            for (i = 0; i < this.AcceptedStates.Length; i++)
                accStates[i] = this.AcceptedStates[i] + 1;

            //iterate D2 acc and add offset
            for (int j = 0; j < fa.AcceptedStates.Length; j++)
                accStates[i + j] = (fa.AcceptedStates[j] + offsetD2);

            return new NFAe($"NFAe_Union({Name}+{fa.Name})", this.StatesCount + fa.StatesCount + 1, nAlphabet, neat, 0, accStates);
        }

        public abstract IAutomat Union(IUnion a);
        public abstract IAutomat Intersect(IIntersect a);
        public abstract IAutomat Diff(IDiff a);

        public FABase ProductEA(FABase A, DFA.eProductDeaMode mode) {
            uint len = (this.StatesCount * A.StatesCount);

            if (!Utils.SameAlphabet(this, A)) {
                char[] Alphabet = this.Alphabet;
                var accStates = new List<uint>();
                string[] stateNames = new string[len];

                var deat = new FATransform();

                //iterate Cross D1xD2, chars
                for (uint i = 0; i < this.StatesCount; i++) {
                    for (uint j = 0; j < A.StatesCount; j++) {
                        //index of state in matrix
                        uint index = (i * A.StatesCount + j);
                        stateNames[index] = $"{i},{j}";

                        foreach (char c in Alphabet) {
                            uint qNext1, qNext2; //next states for D1, D2

                            //tuple for D1,D2

                            //Transform exists, out qNext
                            var exist1 = ((Finite.FATransform)this.Transform).TryGetValue(i, c, out qNext1);
                            var exist2 = ((Finite.FATransform)A.Transform).TryGetValue(j, c, out qNext2);

                            //same calc logic for dstIndex
                            uint dstIndex;
                            if (exist1 & exist2)
                                dstIndex = (qNext1 * A.StatesCount + qNext2);
                            else if (exist1)
                                dstIndex = (qNext1 * A.StatesCount + qNext1);
                            else if (exist2)
                                dstIndex = qNext2;
                            else
                                throw new System.ApplicationException();

                            //add non existing tuple
                            if (!deat.ContainsKey(index, c)) // & exist1 & exist2)
                                deat.Add(index, c, dstIndex);
                            else
                                throw new System.ApplicationException();

                            if (mode == DFA.eProductDeaMode.Intersect) {
                                //add to accStates if one dea state ist acc
                                if (this.IsAcceptedState(i) & A.IsAcceptedState(j))
                                    accStates.Add(index);
                            } else if (mode == DFA.eProductDeaMode.Union) {
                                //add to accStates if both dea state ist acc
                                if (this.IsAcceptedState(i) | A.IsAcceptedState(j))
                                    accStates.Add(index);
                            } else if (mode == DFA.eProductDeaMode.Diff) {
                                //add to accStates if both dea state ist acc
                                if (this.IsAcceptedState(i) & !A.IsAcceptedState(j))
                                    accStates.Add(index);
                            }
                        }
                    }
                }

                return new DFA($"DFA_Product({mode}, {this.Name}+{A.Name})", stateNames, Alphabet, deat, 0, accStates.ToArray());
            } else
                throw new System.NotImplementedException("Different Alphabets are not implemented");
        }


        /// <summary>
        /// Reverse all transforms, add an qε which goes to all former q ϵ F,
        /// former q0 becomes the only ϵ F
        /// </summary>
        [AlgorithmSource("EAFK_4.2.2")]
        public IAutomat Reverse() {
            var neaET = new NFAeTransform();

            string[] names = new string[this.StatesCount + 1];

            //array pointer old to new state            
            uint[] newstate = new uint[this.StatesCount];
            for (uint i = 0; i < newstate.Length; i++) {
                newstate[i] = this.StatesCount - i;
                names[i + 1] = (newstate[i] - 1).ToString();
            }
            names[0] = "new";

            //turn and add each transform
            foreach (var dt in this.Transform.Reverse())
                foreach (var dtv in dt.Value)
                    neaET.AddM(newstate[dtv], dt.Key.c, newstate[dt.Key.q]);

            //start state is qe, which leads to every old accepted state
            for (int i = 0; i < this.AcceptedStates.Length; i++)
                neaET.AddM(0, null, newstate[this.AcceptedStates[i]]);

            return new NFAe($"NFAe_Reverse({Name})", names, this.Alphabet, neaET, 0, this.StatesCount);
        }

        public IAutomat KleeneStern() {
            var neaET = new NFAeTransform();

            //accepted front state
            neaET.Add(0, null, 1);

            //add transforms, +1
            foreach (var item in this.Transform) {
                uint[] qnext = new uint[item.Value.Length];
                for (int i = 0; i < item.Value.Length; i++)
                    qnext[i] = item.Value[i] + 1;
                neaET.Add(item.Key.q + 1, item.Key.c, qnext);
            }

            //add e-transform from org accepted to front state
            for (uint i = 0; i < this.AcceptedStates.Length; i++)
                neaET.Add(this.AcceptedStates[i] + 1, null, 0);

            return new NFAe($"NFAe_KleeneStern({Name})", this.StatesCount, this.Alphabet, neaET, 0, 0);
        }

        public abstract IAutomat HomomorphismChar(System.Collections.Generic.Dictionary<char, char> Translate);

        public abstract IAutomat Join(IJoin A);

        public abstract override bool Equals(object obj);
        public abstract override string ToString();

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}