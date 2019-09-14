using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Finite {
    
    public abstract class FABase : AutomatBase<EATuple, uint[]> {
        
        public FABase(uint stateCount, char[] alphabet, TransformBase<EATuple,uint[]> eat, uint startState, uint[] acceptedStates, string name = "") 
        : base(stateCount, alphabet, startState, name) {
            this.Transform = eat;
            this.AcceptedStates = acceptedStates;
        }

        public FABase(string[] names, char[] alphabet, TransformBase<EATuple,uint[]> eat, uint startState, uint[] acceptedStates, string name = "") 
        : base((uint)names.Length, alphabet, startState, name) {
            this.Transform = eat;
            this.AcceptedStates = acceptedStates;
            for (int i = 0; i < States.Length; i++)
                    States[i] = names[i];
        }

        protected abstract uint[] GoChar(uint q, char c); //maybe return only uint

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<System.Tuple<int, int, string>>();
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

        public FABase Complement() {
            var Dc = this.MemberwiseClone() as FABase;
            var accStates = new List<uint>();

            for (uint i = 0; i < this.States.Length; i++)
                if (!this.AcceptedStates.Contains(i))
                    accStates.Add(i);    
            
            Dc.AcceptedStates = accStates.ToArray();
            return Dc;
        }

        
        public NFAe Concat(FABase A) {
            var neaET = new NFAeTransform();

            if (!Utils.SameAlphabet(this, A)) {
                throw new System.NotImplementedException("Different Alphabets are not implemented");
            }
            
            var accStates = new List<uint>(A.AcceptedStates.Length);
            uint offset = this.StatesCount;

            foreach (var t in this.Transform)
                neaET.Add(t.Key.q, t.Key.c, t.Value);
            
            foreach (var t in A.Transform) {
                uint[] qnexts = new uint[t.Value.Length];
                for (int i = 0; i < t.Value.Length; i++) 
                    qnexts[i] = t.Value[i]+offset;
                neaET.Add((t.Key.q+offset), t.Key.c, qnexts);
            }
                

            for (int i = 0; i < this.AcceptedStates.Length; i++)
                neaET.Add(this.AcceptedStates[i], null, offset);
            
            for (int i = 0; i < A.AcceptedStates.Length; i++)
                accStates.Add((A.AcceptedStates[i]+offset));

            accStates.Sort();

            return new NFAe($"NFAe_Concat({Name}+{A.Name})", A.StatesCount+offset, this.Alphabet, neaET, this.StartState, accStates.ToArray());
        }

        
        /// <summary>
        /// Unions two EAs to a NEA, which has a StartState with e to both Start States
        /// </summary>
        /// <returns></returns>
        public NFAe UnionNEA(FABase fa) {
            uint offsetD2 = this.StatesCount+1; //first state of D2
            uint[] accStates = new uint[this.AcceptedStates.Length + fa.AcceptedStates.Length];
            char[] nAlphabet = this.Alphabet.Union(fa.Alphabet).ToArray();

            var neat = new NFAeTransform();
            //add new start state, with e to both starts
            neat.Add(0, null, 1, offsetD2);

            //add each D1 transform, +1
            foreach (var item in this.Transform)
                neat.Add(item.Key.q+1, item.Key.c, item.Value[0]+1);

            //add each D1 transform, +offset
            foreach (var item in fa.Transform)
                neat.Add((item.Key.q+offsetD2), item.Key.c, (item.Value[0]+offsetD2));


            int i; //store where D1 acc ends
            //iterate D1 acc and +1
            for (i = 0; i < this.AcceptedStates.Length; i++)
                accStates[i] = this.AcceptedStates[i]+1;

            //iterate D2 acc and add offset
            for (int j = 0; j < fa.AcceptedStates.Length; j++)
                accStates[i+j] = (fa.AcceptedStates[j]+offsetD2);

            return new NFAe($"NFAe_Union({Name}+{fa.Name})",this.StatesCount+fa.StatesCount+1, nAlphabet, neat, 0, accStates);
        }

        public FABase ProductEA(FABase A, DFA.eProductDeaMode mode) {
            
            uint len = (this.StatesCount*A.StatesCount);

            if (!Utils.SameAlphabet(this, A)) {
                char[] Alphabet = this.Alphabet;
                var accStates = new List<uint>(); 
                string[] stateNames = new string[len];

                var deat = new FATransform();

                //iterate Cross D1xD2, chars
                for (uint i = 0; i < this.StatesCount; i++)
                {
                    for (uint j = 0; j < A.StatesCount; j++)
                    {
                        //index of state in matrix
                        uint index = (i*A.StatesCount+j); 
                        stateNames[index] = $"{i},{j}";

                        foreach (char c in Alphabet)
                        {
                            uint qNext1, qNext2; //next states for D1, D2

                            //tuple for D1,D2

                            //Transform exists, out qNext
                            var exist1 = ((Finite.FATransform)this.Transform).TryGetValue(i,c, out qNext1);
                            var exist2 = ((Finite.FATransform)A.Transform).TryGetValue(j,c, out qNext2);
                            // var exist1 = this.Transform.TryGetValue(i,c, out qNext1);
                            // var exist2 = A.Transform.TryGetValue(j,c, out qNext2);

                            //same calc logic for dstIndex
                            uint dstIndex;
                            if (exist1 & exist2)
                                dstIndex = (qNext1*A.StatesCount+qNext2);
                            else if (exist1)
                                dstIndex = (qNext1*A.StatesCount+qNext1);
                            else if (exist2)
                                dstIndex = qNext2;
                            else
                                throw new System.ApplicationException();

                            //add non existing tuple
                            if (!deat.ContainsKey(index,c)) // & exist1 & exist2)
                                deat.Add(index,c,dstIndex);
                            else
                                throw new System.ApplicationException();

                            if (mode==DFA.eProductDeaMode.Intersect) {
                                //add to accStates if one dea state ist acc
                                if (this.AcceptedStates.Contains(i) & A.AcceptedStates.Contains(j))
                                    accStates.Add(index);
                            } else if (mode==DFA.eProductDeaMode.Union) {
                                //add to accStates if both dea state ist acc
                                if (this.AcceptedStates.Contains(i) | A.AcceptedStates.Contains(j))
                                    accStates.Add(index);
                            } else if (mode==DFA.eProductDeaMode.Diff) {
                                //add to accStates if both dea state ist acc
                                if (this.AcceptedStates.Contains(i) & !A.AcceptedStates.Contains(j))
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
        /// <returns></returns>  
        public NFAe Reverse() {
            var neaET = new NFAeTransform();

            //array pointer old to new state            
            uint[] newstate = new uint[this.StatesCount];
            for (uint i = 0; i < newstate.Length; i++)
                newstate[i] = this.StatesCount - i;

            //turn and add each transform
            foreach (var dt in this.Transform.Reverse()) {

                //new tuple with opposite direction
                var newVals = new uint[dt.Value.Length];
                for (int i = 0; i < dt.Value.Length; i++) {


                    var tuple = new EATuple(newstate[dt.Value[i]], dt.Key.c);
                    

                    if (!neaET.ContainsKey(tuple.q, tuple.c.Value))
                        //add turned tuple
                        neaET.Add(tuple.q, tuple.c, newstate[dt.Key.q]);
                    else //could exists multiple possibilites, so append tranform
                        neaET[tuple.q, tuple.c.Value] = neaET[tuple.q, tuple.c.Value].Append(newstate[dt.Key.q]).ToArray();
                }
                
            }

            //start state is qe, which leads to every old accepted state
            for (int i = 0; i < this.AcceptedStates.Length; i++)
                neaET.Add(0, null, newstate[this.AcceptedStates[i]]);
            
            return new NFAe($"NFAe_Reverse({Name})", this.StatesCount+1, this.Alphabet, neaET, 0, this.StatesCount);
        }

        public NFAe KleeneStern() {
            var neaET = new NFAeTransform();

            //accepted front state
            neaET.Add(0, null, 1);

            //add transforms, +1
            foreach (var item in this.Transform) {
                uint[] qnext = new uint[item.Value.Length];
                for (int i = 0; i < item.Value.Length; i++)
                    qnext[i] = item.Value[i]+1;   
                neaET.Add(item.Key.q+1, item.Key.c, qnext);
            }

            //add e-transform from org accepted to front state
            for (uint i = 0; i < this.AcceptedStates.Length; i++)
                neaET.Add(this.AcceptedStates[i]+1, null, 0);

            return new NFAe($"NFAe_KleeneStern({Name})", this.StatesCount, this.Alphabet, neaET, 0, 0);
        }

        public abstract FABase HomomorphismChar(System.Collections.Generic.Dictionary<char,char> Translate);

        public abstract FABase Join(FABase A);

        public abstract override bool Equals(object obj);
        public abstract override string ToString();
        
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}