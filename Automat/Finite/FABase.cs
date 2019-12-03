using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {

    [System.Serializable]
    public abstract class FABase : AutomatBase<EATuple, uint[]>, IAlleAbgeschlossenheitseigenschaften {

        protected FABase(uint stateCount, char[] alphabet, TransformBase<EATuple, uint[]> eat, uint startState, uint[] acceptedStates, string name = "")
        : base(stateCount, alphabet, startState, name, acceptedStates) {
            this.Transforms = eat;
            CheckConstraints();
        }

		protected FABase(string[] names, char[] alphabet, TransformBase<EATuple, uint[]> eat, uint startState, uint[] acceptedStates, string name = "")
        : base(names, alphabet, startState, name, acceptedStates) {
            this.Transforms = eat;
            CheckConstraints();
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var t in Transforms) {
                for (int i = 0; i < t.Value.Length; i++) {
                    if (t.Key.q >= StatesCount)
                        throw new StateException(t.Key.q, this);
                    else if (t.Value[i] >= StatesCount)
                        throw new StateException(t.Value[i], this);
                    else if (t.Key.c.HasValue && !Alphabet.Contains(t.Key.c.Value))
                        throw new AlphabetException(t.Key.c.Value, this);
                }
            }
        }

        protected abstract uint[] GoChar(uint q, char c); //maybe return only uint

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new List<VisualizationTuple>(Transforms.Count);
            foreach (var t in Transforms) {
                string desc = t.Key.c.ToString();
                if (desc == "")
                    desc = Utils.EPSILON.ToString();
                foreach (uint v in t.Value) {
                    var vt = new VisualizationTuple(t.Key.q, v, desc);
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        [AlgorithmSource("1659_T2.5_P47")]
        public IAutomat Complement() {
            // var Dc = this.MemberwiseClone() as FABase;
            var accStates = new List<uint>((int)StatesCount - AcceptedStates.Length);

            for (uint i = 0; i < this.States.Length; i++)
                if (!this.IsAcceptedState(i))
                    accStates.Add(i);

            // Dc.AcceptedStates = accStates.ToArray();
            // return Dc;
            if (this is DFA dfa)
                return new DFA(this.Name + "_rev", this.States, this.Alphabet, (DFATransform)dfa.Transforms, this.StartState, accStates.ToArray());
            else if (this is NFA nfa)
                return new NFA(this.Name + "_rev", this.States, this.Alphabet, (NFAeTransform)nfa.Transforms, this.StartState, accStates.ToArray());
            else if (this is NFAe nfae)
                return new NFA(this.Name + "_rev", this.States, this.Alphabet, (NFAeTransform)nfae.Transforms, this.StartState, accStates.ToArray());
            else
                throw new System.NotSupportedException();
        }

        [AlgorithmSource("1659_S2.9")]
        IAutomat JoinConcatUnion(IAutomat automat, JoinConcatUnionKind unionConcatJoinKind) {
            if (!(automat is FABase fa2))
                throw new System.NotSupportedException();

            uint offsetA1 = 0; // first state of A2
            uint offsetA2 = this.StatesCount; // first state of A2
            if (unionConcatJoinKind == JoinConcatUnionKind.Union) {
                offsetA1 = 1;
                offsetA2++;
            }

            char[] inputAlphabet = this.Alphabet.Union(fa2.Alphabet).ToArray();

            var neaeT = new NFAeTransform();

            uint startState;
            // Union: add new start state, with e to both starts
            if (unionConcatJoinKind == JoinConcatUnionKind.Union) {
                startState = 0;
                neaeT.Add(0, null, this.StartState + offsetA1);
                neaeT.AddM(0, null, fa2.StartState + offsetA2);
            } else
                startState = this.StartState;

            // add each A1 transform + offset of A1
            foreach (var item in this.Transforms)
                foreach (var val in item.Value)
                    neaeT.AddM(item.Key.q + offsetA1, item.Key.c, val + offsetA1);

            // add each A1 transform, + offset of A2
            foreach (var item in fa2.Transforms)
                foreach (var val in item.Value)
                    neaeT.AddM(item.Key.q + offsetA2, item.Key.c, val + offsetA2);


            uint[] accStates;
            if (unionConcatJoinKind == JoinConcatUnionKind.Concat)
                // Concat: has only accepted states from A2
                accStates = new uint[fa2.AcceptedStates.Length];
            else
                // Join, Union has A1, A2 accpted states
                accStates = new uint[this.AcceptedStates.Length + fa2.AcceptedStates.Length];

            int i = 0; // store where D1 acc ends
            // iterate A1 acc
            for (; i < this.AcceptedStates.Length; i++)
                if (unionConcatJoinKind == JoinConcatUnionKind.Concat)
                    // Concat: lead accepted states from A1 to A2 start
                    neaeT.AddM(this.AcceptedStates[i] + offsetA1, null, fa2.StartState + offsetA2);
                else
                    // Join, Union: states from A1 are normal accepted
                    accStates[i] = this.AcceptedStates[i] + offsetA1;
                
            if (unionConcatJoinKind == JoinConcatUnionKind.Concat)
                i = 0;

            // iterate A2 acs and + offsetA2
            for (int j = 0; j < fa2.AcceptedStates.Length; j++)
                accStates[i + j] = (fa2.AcceptedStates[j] + offsetA2);

            
            return new NFAe($"NEAe{unionConcatJoinKind.ToString()}({Name}+{fa2.Name})", this.StatesCount + fa2.StatesCount + offsetA1, inputAlphabet, neaeT, startState, accStates);
        }

        public virtual IAutomat Join(IJoin automat) => JoinConcatUnion(automat, JoinConcatUnionKind.Join);
        public virtual IAutomat Concat(IConcat automat) => JoinConcatUnion(automat, JoinConcatUnionKind.Concat);
        public virtual IAutomat Union(IUnion automat) => JoinConcatUnion(automat, JoinConcatUnionKind.Union);
        public abstract IAutomat Intersect(IIntersect a);
        public abstract IAutomat Diff(IDiff a);

        /// <summary>
        /// Reverse all transforms, add an qε which goes to all former q ϵ F,
        /// former q0 becomes the only ϵ F
        /// </summary>
        [AlgorithmSource("EAFK_4.2.2","1659_T2.5_P47")]
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
            foreach (var dt in this.Transforms.Reverse())
                foreach (var dtv in dt.Value)
                    neaET.AddM(newstate[dtv], dt.Key.c, newstate[dt.Key.q]);

            //start state is qe, which leads to every old accepted state
            for (int i = 0; i < this.AcceptedStates.Length; i++)
                neaET.AddM(0, null, newstate[this.AcceptedStates[i]]);

            return new NFAe($"NFAe_Reverse({Name})", names, this.Alphabet, neaET, 0, this.StatesCount);
        }

        [AlgorithmSource("1659_S2.9_P45")]
        public IAutomat KleeneStern() {
            var neaET = new NFAeTransform
            {
                { 0, null, this.StartState+1 } // accepted front state
			};

            // add transforms, +1
            foreach (var item in this.Transforms) {
                uint[] qnext = new uint[item.Value.Length];
                for (int i = 0; i < item.Value.Length; i++)
                    qnext[i] = item.Value[i] + 1;
                neaET.Add(item.Key.q + 1, item.Key.c, qnext);
            }

            var acceptedStates = new uint[this.AcceptedStates.Length+1];
            acceptedStates[0] = 0;
            // add e-transform from org accepted to front state
            for (uint i = 0; i < this.AcceptedStates.Length; i++) {
                acceptedStates[i+1] = this.AcceptedStates[i]+1; // a little redundant, because all accepted states must go back to first
                neaET.Add(this.AcceptedStates[i] + 1, null, 0);
            }

            return new NFAe($"NFAe_KleeneStern({Name})", this.StatesCount+1, this.Alphabet, neaET, 0, acceptedStates);
        }

        public abstract IAutomat HomomorphismChar(System.Collections.Generic.Dictionary<char, char> Translate);

        public abstract override bool Equals(object obj);
        public abstract override string ToString();

        public override int GetHashCode() => ToString().GetHashCode();
    }
}