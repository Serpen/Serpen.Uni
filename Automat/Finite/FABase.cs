using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {

    public abstract class FABase : AutomatBase<EATuple, uint[]>, IAlleAbgeschlossenheitseigenschaften {

        public FABase(uint stateCount, char[] alphabet, TransformBase<EATuple, uint[]> eat, uint startState, uint[] acceptedStates, string name = "")
        : base(stateCount, alphabet, startState, name, acceptedStates) {
            this.Transforms = eat;
            CheckConstraints();
        }

        public FABase(string[] names, char[] alphabet, TransformBase<EATuple, uint[]> eat, uint startState, uint[] acceptedStates, string name = "")
        : base(names, alphabet, startState, name, acceptedStates) {
            this.Transforms = eat;
            CheckConstraints();
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
        }

        protected abstract uint[] GoChar(uint q, char c); //maybe return only uint

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new List<VisualizationTuple>(Transforms.Count);
            foreach (var t in Transforms) {
                string desc = t.Key.c.ToString();
                if (desc == "")
                    desc = Uni.Utils.EPSILON.ToString();
                foreach (uint v in t.Value) {
                    var vt = new VisualizationTuple(t.Key.q, v, desc);
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public IAutomat Complement() {
            var Dc = this.MemberwiseClone() as FABase;
            var accStates = new List<uint>((int)StatesCount-AcceptedStates.Length);

            for (uint i = 0; i < this.States.Length; i++)
                if (!this.IsAcceptedState(i))
                    accStates.Add(i);

            Dc.AcceptedStates = accStates.ToArray();
            return Dc;
        }


        public IAutomat Concat(IConcat automat) {
            if (!(automat is FABase A))
                throw new System.NotSupportedException();

            var neaET = new NFAeTransform();

            var accStates = new List<uint>(A.AcceptedStates.Length);
            uint offset = this.StatesCount;

            foreach (var t in this.Transforms)
                neaET.Add(t.Key.q, t.Key.c, t.Value);

            foreach (var t in A.Transforms) {
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

			var neat = new NFAeTransform
			{
				{ 0, null, 1, offsetD2 }
			};

			//add each D1 transform, +1
			foreach (var item in this.Transforms)
                foreach (uint val in item.Value)
                    neat.AddM(item.Key.q + 1, item.Key.c, val + 1);

            //add each D1 transform, +offset
            foreach (var item in fa.Transforms)
                foreach (uint val in item.Value)
                    neat.AddM(item.Key.q + offsetD2, item.Key.c, val + offsetD2);


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
            foreach (var dt in this.Transforms.Reverse())
                foreach (var dtv in dt.Value)
                    neaET.AddM(newstate[dtv], dt.Key.c, newstate[dt.Key.q]);

            //start state is qe, which leads to every old accepted state
            for (int i = 0; i < this.AcceptedStates.Length; i++)
                neaET.AddM(0, null, newstate[this.AcceptedStates[i]]);

            return new NFAe($"NFAe_Reverse({Name})", names, this.Alphabet, neaET, 0, this.StatesCount);
        }

        public IAutomat KleeneStern() {
			var neaET = new NFAeTransform
			{
				//accepted front state
				{ 0, null, 1 }
			};

			//add transforms, +1
			foreach (var item in this.Transforms) {
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

        public override int GetHashCode() => ToString().GetHashCode();
    }
}