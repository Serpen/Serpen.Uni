using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.ContextFree {

    /// <summary>
    /// nondeterministic PDA with Stack Symbol, which must end as empty stack and accepted state
    /// </summary>
    public abstract class PDA : AutomatBase<PDATransformKey, PDATransformValue[]>, IReverse, IUnion, IConcat {

        protected const int MAX_RUNS_OR_STACK = 10000;
        protected static readonly char[] EXTRASYMBOLS = new char[] { '§', '$', '%', '&' };
        public const char START = '$';
        public readonly char[] WorkAlphabet;
        //public readonly PDATransform Transform;
        public readonly char StartStackSymbol;

        /// <summary>
        /// Create a PDA which accepts when ending in Accepted States
        /// </summary>
        /// <param name="statesCount"></param>
        /// <param name="inputAlphabet"></param>
        /// <param name="workalphabet"></param>
        /// <param name="transform"></param>
        /// <param name="startState"></param>
        /// <param name="startstacksymbol"></param>
        /// <param name="acceptedStates">Accepted Endstates</param>
        public PDA(string name, uint statesCount, char[] inputAlphabet, char[] workalphabet, PDATransform transform, uint startState, char startstacksymbol, uint[] acceptedStates)
        : base(statesCount, inputAlphabet, startState, name, acceptedStates) {
            this.WorkAlphabet = workalphabet;
            if (!workalphabet.Contains(startstacksymbol))
                this.WorkAlphabet = this.WorkAlphabet.Append(startstacksymbol).ToArray();
            this.Transform = transform;
            this.StartStackSymbol = startstacksymbol;

            CheckConstraints();
        }

        public PDA(string name, string[] states, char[] inputAlphabet, char[] workalphabet, PDATransform transform, uint startState, char startStacksymbol, uint[] acceptedStates)
        : base(states, inputAlphabet, startState, name, acceptedStates) {
            this.WorkAlphabet = workalphabet;
            if (!workalphabet.Contains(startStacksymbol))
                this.WorkAlphabet = this.WorkAlphabet.Append(startStacksymbol).ToArray();
            this.Transform = transform;
            this.StartStackSymbol = startStacksymbol;

            CheckConstraints();
        }


        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var t in Transform) {
                for (int i = 0; i < t.Value.Length; i++) {
                    if (t.Key.q > StatesCount)
                        throw new StateException(t.Key.q);
                    else if (t.Value[i].qNext > StatesCount)
                        throw new StateException(t.Value[i].qNext);
                    else if (t.Key.ci.HasValue && !Alphabet.Contains(t.Key.ci.Value))
                        throw new AlphabetException(t.Key.ci.Value);
                    else if (t.Key.cw.HasValue && !WorkAlphabet.Contains(t.Key.cw.Value))
                        throw new AlphabetException(t.Key.cw.Value);
                    else if (t.Value[i].cw2 != null && t.Value[i].cw2 != "" && !WorkAlphabet.Contains(t.Value[i].cw2[0]))
                        throw new AlphabetException(t.Value[i].cw2[0]);

                }
            }
            for (int i = 0; i < AcceptedStates.Length; i++)
                if (AcceptedStates[i] > StatesCount)
                    throw new StateException(AcceptedStates[i]);

        }

        //nondeterministic makes it grow exponetially, maybe it is best, to do a BFS instead of DFS, follow one path to its possible end
        public PDAConfig[] GoChar(PDAConfig[] pcfgs) {
            var retCfgs = new List<PDAConfig>();

            if (pcfgs.Length > MAX_RUNS_OR_STACK) {
                Utils.DebugMessage($"Stack >= {pcfgs.Length}, abort", this, Utils.eDebugLogLevel.Always);
                return new PDAConfig[0];
            }

            foreach (var pcfg in pcfgs) {
                PDATransformKey[] qStart = new PDATransformKey[1];

                //if word is empty, maybe only e-Transform is needed
                if (pcfg.word != "") {
                    if (pcfg.Stack.Any())
                        qStart[0] = new PDATransformKey(pcfg.q, pcfg.word[0], pcfg.Stack[0]);
                    // qStart[0] = new PDATransformKey(pcfg.q, pcfg.word[0], pcfg.Stack[pcfg.Stack.Length-1]);
                    else
                        qStart[0] = new PDATransformKey(pcfg.q, pcfg.word[0], null);
                    // continue; //TODO: why that //maybe empty stack that is not accepted should be thrown away

                } else {
                    if (pcfg.Stack.Any())
                        qStart[0] = new PDATransformKey(pcfg.q, null, pcfg.Stack[0]);
                    else
                        qStart[0] = new PDATransformKey(pcfg.q, null, null);
                    // continue; //TODO: why that
                }

                PDATransformValue[] qNext;

                //get all possible (e-)transforms
                if (((PDATransform)Transform).TryGetValue(ref qStart, out qNext)) {
                    //iterate each cuple of start and next transform
                    for (int j = 0; j < qNext.Length; j++) {
                        var newStack = new Stack<char>(pcfg.Stack.Reverse());
                        var qNextj = qNext[j];

                        //stack symbol cw2 replaces cw
                        if (qStart[j].cw.HasValue && qNext[j].cw2 != null) {
                            newStack.Pop();
                            for (int i = qNextj.cw2.Length - 1; i >= 0; i--)
                                newStack.Push(qNextj.cw2[i]);

                            //cw was used, replace with e, so pop
                        } else if (qStart[j].cw.HasValue && qNext[j].cw2 == null) {
                            newStack.Pop();

                            //any (e-cw) should be replaced by cw2
                        } else if (!qStart[j].cw.HasValue && qNext[j].cw2 != null) {
                            for (int i = qNextj.cw2.Length - 1; i >= 0; i--)
                                newStack.Push(qNextj.cw2[i]);

                            //cw's not relevant, no stack action
                        } else if (!qStart[j].cw.HasValue && qNext[j].cw2 == null) {
                            //do nothing
                        } else
                            throw new System.NotSupportedException("some condition forgotten??");

                        //transform was because of i, then remove words first letter
                        string newWord = pcfg.word;
                        if (qStart[j].ci.HasValue)
                            if (pcfg.word.Length > 0)
                                newWord = pcfg.word.Substring(1);

                        retCfgs.Add(new PDAConfig(qNext[j].qNext, newWord, newStack.ToArray(), pcfg));

                    } //next j

                } //end if tryGetValue
            } //next pcfg

            //buggy, but only perf way
            List<PDAConfig> retCfgs2;
            if (retCfgs.Count > 100) {
                retCfgs2 = new List<PDAConfig>();
                int maxStackLen = int.MaxValue;
                foreach (var rcfg in retCfgs) {
                    if (rcfg.Stack.Length <= rcfg.word.Length * 3) {
                        maxStackLen = rcfg.word.Length;
                        retCfgs2.Add(rcfg);
                    }
                }
            } else
                retCfgs2 = new List<PDAConfig>(retCfgs);

            return retCfgs2.Distinct().ToArray();
        }

        public abstract override bool AcceptWord(string w);

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>();
            foreach (var t in Transform) {
                foreach (var v in t.Value) {
                    string desc = $"{(t.Key.ci.HasValue ? t.Key.ci.Value : Utils.EPSILON)}|{(t.Key.cw.HasValue ? t.Key.cw.Value : Utils.EPSILON)}->{(!string.IsNullOrEmpty(v.cw2) ? v.cw2 : Utils.EPSILON.ToString())}";
                    var vt = new VisualizationTuple(t.Key.q, v.qNext, desc);
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public override string ToString() => $"{Name} PDA(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', WorkAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {StartStackSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

        #region "Operations"

        public IAutomat Union(IUnion automat) {

            var pda = automat as PDA;
            if (pda == null)
                throw new System.NotSupportedException();

            uint offsetD2 = this.StatesCount + 1; //first state of D2
            uint[] accStates = new uint[this.AcceptedStates.Length + pda.AcceptedStates.Length];
            char[] inputAlphabet = this.Alphabet.Union(pda.Alphabet).ToArray();
            char[] workAlphabet = this.WorkAlphabet.Union(pda.WorkAlphabet).ToArray();

            var pdat = new PDATransform();
            //add new start state, with e to both starts
            pdat.Add(0, null, null, this.StartStackSymbol.ToString(), 1);
            pdat.AddM(0, null, null, pda.StartStackSymbol.ToString(), offsetD2);

            //add each D1 transform, +1
            foreach (var item in this.Transform)
                foreach (var val in item.Value)
                    pdat.AddM(item.Key.q + 1, item.Key.ci, item.Key.cw, val.cw2, val.qNext + 1);

            //add each D1 transform, +offset
            foreach (var item in pda.Transform)
                foreach (var val in item.Value)
                    pdat.AddM(item.Key.q + offsetD2, item.Key.ci, item.Key.cw, val.cw2, val.qNext + offsetD2);


            int i; //store where D1 acc ends
            //iterate D1 acc and +1
            for (i = 0; i < this.AcceptedStates.Length; i++)
                accStates[i] = this.AcceptedStates[i] + 1;

            //iterate D2 acc and add offset
            for (int j = 0; j < pda.AcceptedStates.Length; j++)
                accStates[i + j] = (pda.AcceptedStates[j] + offsetD2);

            if (this is StatePDA)
                return new StatePDA($"Union({Name}+{pda.Name})", this.StatesCount + pda.StatesCount + 1, inputAlphabet, workAlphabet, pdat, 0, (char)0, accStates);
            else if (this is StackPDA)
                return new StackPDA($"Union({Name}+{pda.Name})", this.StatesCount + pda.StatesCount + 1, inputAlphabet, workAlphabet, pdat, 0, (char)0);
            else
                throw new System.NotImplementedException();
        }

        public IAutomat Concat(IConcat automat) {

            var pda = automat as PDA;
            if (pda == null)
                throw new System.NotSupportedException();

            var pdat = new PDATransform();

            char[] inputAlphabet = this.Alphabet.Union(pda.Alphabet).ToArray();
            char[] workAlphabet = this.WorkAlphabet.Union(pda.WorkAlphabet).ToArray();

            // if (!Utils.SameAlphabet(this, A))
            //     throw new System.NotImplementedException("Different Alphabets are not implemented");

            var accStates = new List<uint>(pda.AcceptedStates.Length);
            uint offset = this.StatesCount;

            foreach (var t in this.Transform)
                foreach (var val in t.Value)
                    pdat.AddM(t.Key.q, t.Key.ci, t.Key.cw, val.cw2, val.qNext);

            foreach (var t in pda.Transform) {
                uint[] qnexts = new uint[t.Value.Length];
                for (int i = 0; i < t.Value.Length; i++) {
                    qnexts[i] = t.Value[i].qNext + offset;
                    pdat.AddM(t.Key.q + offset, t.Key.ci, t.Key.cw, t.Value[i].cw2, qnexts[i]);
                }
            }


            for (int i = 0; i < this.AcceptedStates.Length; i++)
                pdat.AddM(this.AcceptedStates[i], null, null, null, offset);

            for (int i = 0; i < pda.AcceptedStates.Length; i++)
                accStates.Add((pda.AcceptedStates[i] + offset));

            accStates.Sort();

            if (this is StatePDA)
                return new StatePDA($"Union({Name}+{pda.Name})", this.StatesCount + pda.StatesCount, inputAlphabet, workAlphabet, pdat, 0, (char)0, accStates.ToArray());
            else if (this is StackPDA)
                return new StackPDA($"Union({Name}+{pda.Name})", this.StatesCount + pda.StatesCount, inputAlphabet, workAlphabet, pdat, 0, (char)0);
            else
                throw new System.NotImplementedException();

        }

        public virtual IAutomat Reverse() {
            var pdat = new PDATransform();

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
                    pdat.AddM(newstate[dtv.qNext], dt.Key.ci, dt.Key.cw, dtv.cw2, newstate[dt.Key.q]);

            //start state is qe, which leads to every old accepted state
            for (int i = 0; i < this.AcceptedStates.Length; i++)
                pdat.AddM(0, null, null, null, newstate[this.AcceptedStates[i]]);

            if (this is StatePDA)
                return new StatePDA($"Reverse({Name})", names, this.Alphabet, this.WorkAlphabet, pdat, 0, START, new uint[] { this.StatesCount });
            else if (this is StackPDA)
                return new StackPDA($"Reverse({Name})", names, this.Alphabet, this.WorkAlphabet, pdat, 0, START);
            else
                throw new System.NotImplementedException();
        }

        #endregion

    } //end class

} //end ns