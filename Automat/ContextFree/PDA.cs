using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.ContextFree {

    public interface IPDA : IAutomat, IReverse {
        char[] WorkAlphabet { get; }
        char? StartSymbol { get; }

        IList<PDAConfig> GoChar(PDAConfig[] pcfgs);
    }

    /// <summary>
    /// nondeterministic PDA with Stack Symbol, which must end as empty stack and accepted state
    /// </summary>
    [System.Serializable]
    public abstract class PDA : AutomatBase<PDATransformKey, PDATransformValue[]>, IPDA, IReverse, IUnion {

        protected const int MAX_RUNS_OR_STACK = 10000;
        protected static readonly char[] EXTRASYMBOLS = new char[] { '§', '$', '%', '&' };
        public const char START = '$';
        public char[] WorkAlphabet { get; }
        public char? StartSymbol { get; }

        /// <summary>
        /// Create a PDA which accepts when ending in Accepted States
        /// </summary>
        /// <param name="statesCount"></param>
        /// <param name="inputAlphabet"></param>
        /// <param name="workalphabet"></param>
        /// <param name="transform"></param>
        /// <param name="startState"></param>
        /// <param name="startStackSymbol"></param>
        /// <param name="acceptedStates">Accepted Endstates</param>
        protected PDA(string name, uint statesCount, char[] inputAlphabet, char[] workalphabet, PDATransform transform, uint startState, char? startStackSymbol, uint[] acceptedStates)
        : base(name, statesCount, inputAlphabet, startState, acceptedStates) {
            this.WorkAlphabet = workalphabet;
            if (startStackSymbol.HasValue && !workalphabet.Contains(startStackSymbol.Value))
                this.WorkAlphabet = this.WorkAlphabet.Append(startStackSymbol.Value).ToArray();
            this.Transforms = transform;
            this.StartSymbol = startStackSymbol;

            CheckConstraints();
        }

        protected PDA(string name, string[] states, char[] inputAlphabet, char[] workalphabet, PDATransform transform, uint startState, char? startStackSymbol, uint[] acceptedStates)
        : base(name, states, inputAlphabet, startState, acceptedStates) {
            this.WorkAlphabet = workalphabet;
            if (startStackSymbol.HasValue && !workalphabet.Contains(startStackSymbol.Value))
                this.WorkAlphabet = this.WorkAlphabet.Append(startStackSymbol.Value).ToArray();
            this.Transforms = transform;
            this.StartSymbol = startStackSymbol;

            CheckConstraints();
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var t in Transforms) {
                for (int i = 0; i < t.Value.Length; i++) {
                    if (t.Key.q >= StatesCount)
                        throw new StateException(t.Key.q, this);
                    else if (t.Value[i].qNext >= StatesCount)
                        throw new StateException(t.Value[i].qNext, this);
                    else if (t.Key.ci.HasValue && !Alphabet.Contains(t.Key.ci.Value))
                        throw new AlphabetException(t.Key.ci.Value, this);
                    else if (t.Key.cw.HasValue && !WorkAlphabet.Contains(t.Key.cw.Value))
                        throw new AlphabetException(t.Key.cw.Value, this);
                    else if (t.Value[i].cw2 != null && t.Value[i].cw2 != "" && !WorkAlphabet.Contains(t.Value[i].cw2[0]))
                        throw new AlphabetException(t.Value[i].cw2[0], this);
                }
            }
            for (int i = 0; i < AcceptedStates.Length; i++)
                if (AcceptedStates[i] >= StatesCount)
                    throw new StateException(AcceptedStates[i], this);
        }

        // nondeterministic makes it grow exponetially, maybe it is best, to do a BFS instead of DFS, follow one path to its possible end
        public IList<PDAConfig> GoChar(PDAConfig[] pcfgs) {
            var retCfgs = new List<PDAConfig>(pcfgs.Length*10);

            if (pcfgs.Length > MAX_RUNS_OR_STACK) {
                Utils.DebugMessage($"Stack >= {pcfgs.Length}", this, Uni.Utils.eDebugLogLevel.Normal);
                //return System.Array.Empty<PDAConfig>();
            }

            foreach (var pcfg in pcfgs) {
                PDATransformKey[] qStart = new PDATransformKey[1];

                // if word is empty, maybe only e-Transform is needed
                if (!string.IsNullOrEmpty(pcfg.word)) {
                    if (pcfg.Stack.Any())
                        qStart[0] = new PDATransformKey(pcfg.State, pcfg.word[0], pcfg.Stack[0]);
                    else
                        qStart[0] = new PDATransformKey(pcfg.State, pcfg.word[0], null);

                } else {
                    if (pcfg.Stack.Any())
                        qStart[0] = new PDATransformKey(pcfg.State, null, pcfg.Stack[0]);
                    else
                        qStart[0] = new PDATransformKey(pcfg.State, null, null);
                }

                // get all possible (e-)transforms
                if (((PDATransform)Transforms).TryGetValue(ref qStart, out PDATransformValue[] qNext)) {
                    // iterate each cuple of start and next transform
                    for (int j = 0; j < qNext.Length; j++) {
                        var newStack = new Stack<char>(pcfg.Stack.Reverse());
                        var qNextj = qNext[j];
                        var qStartj = qStart[j];

                        if (qStartj.cw.HasValue && qNextj.cw2 != null) {
                            // stack symbol cw2 replaces cw
                            newStack.Pop();
                            for (int i = qNextj.cw2.Length - 1; i >= 0; i--)
                                newStack.Push(qNextj.cw2[i]);

                        } else if (qStartj.cw.HasValue && qNextj.cw2 == null) {
                            // cw was used, replace with e, so pop
                            newStack.Pop();

                        } else if (!qStartj.cw.HasValue && qNextj.cw2 != null) {
                            // any (e-cw) should be replaced by cw2
                            for (int i = qNextj.cw2.Length - 1; i >= 0; i--)
                                newStack.Push(qNextj.cw2[i]);

                        } else if (!qStartj.cw.HasValue && qNextj.cw2 == null) {
                            // cw's not relevant, no stack action -> NOOP
                        } else
                            throw new System.NotSupportedException("some condition forgotten??");

                        // transform was because of ci, then remove words first letter
                        string newWord = pcfg.word;
                        if (qStartj.ci.HasValue && pcfg.word.Length > 0)
                            newWord = pcfg.word.Substring(1);

                        // Hack
                        if (newStack.Count <= (newWord.Length+1)*3)
                            retCfgs.Add(new PDAConfig(qNextj.qNext, newWord, newStack.ToArray(), pcfg));

                    } //next j

                } //end if tryGetValue
            } //next pcfg

            retCfgs.Sort((a,b) => a.word.Length.CompareTo(b.word.Length));

            return retCfgs.Distinct().ToList();
        }

        public abstract override bool AcceptWord(string w);

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>(Transforms.Count);
            foreach (var t in Transforms) {
                foreach (var v in t.Value) {
                    string desc = $"{(t.Key.ci ?? Utils.EPSILON)}|{(t.Key.cw ?? Utils.EPSILON)}->{(!string.IsNullOrEmpty(v.cw2) ? v.cw2 : Utils.EPSILON.ToString())}";
                    var vt = new VisualizationTuple(t.Key.q, v.qNext, desc);
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public override string ToString() => $"{Name} PDA(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', WorkAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {StartSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

        #region "Operations"

        public IAcceptWord Join(IJoin automat) => JoinConcatUnion(automat, JoinConcatUnionKind.Join);
        public IAcceptWord Concat(IConcat automat) => JoinConcatUnion(automat, JoinConcatUnionKind.Concat);
        public IAcceptWord Union(IUnion automat) => JoinConcatUnion(automat, JoinConcatUnionKind.Union);

        IAcceptWord JoinConcatUnion(IAcceptWord automat, JoinConcatUnionKind kind) {
            if (!(automat is PDA pda2))
                throw new System.NotSupportedException();
            if (kind != JoinConcatUnionKind.Join && !(automat is StatePDA))
                throw new System.NotSupportedException();
            if (this is StackPDA || automat is StackPDA)
                throw new System.NotImplementedException();
            // TODO: investigate more, State PDA is perfect Stack PDA seems complicated

            // TODO: what if A1 or A2 has a StackStartsymbol set, should it be transformed
            // to a transform or applied

            uint offsetA1 = 0; // first state of A2
            uint offsetA2 = this.StatesCount; // first state of A2
            if (kind == JoinConcatUnionKind.Union) {
                offsetA1 = 1;
                offsetA2++;
            }

            char[] inputAlphabet = this.Alphabet.Union(pda2.Alphabet).ToArray();
            char[] workAlphabet = this.WorkAlphabet.Union(pda2.WorkAlphabet).ToArray();

            var pdat = new PDATransform();

            uint startState;
            // Union: add new start state, with e to both starts
            if (kind == JoinConcatUnionKind.Union) {
                startState = 0;
                pdat.Add(0, null, null, null, this.StartState + offsetA1);
                pdat.AddM(0, null, null, null, pda2.StartState + offsetA2);
            } else
                startState = this.StartState;

            // add each D1 transform + offset of A1
            foreach (var item in this.Transforms)
                foreach (var val in item.Value)
                    pdat.AddM(item.Key.q + offsetA1, item.Key.ci, item.Key.cw, val.cw2, val.qNext + offsetA1);

            // add each D1 transform, + offset of A2
            foreach (var item in pda2.Transforms)
                foreach (var val in item.Value)
                    pdat.AddM(item.Key.q + offsetA2, item.Key.ci, item.Key.cw, val.cw2, val.qNext + offsetA2);


            uint[] accStates;
            if (kind == JoinConcatUnionKind.Concat)
                // Concat: has only accepted states from A2
                accStates = new uint[pda2.AcceptedStates.Length];
            else
                // Join, Union has A1, A2 accpted states
                accStates = new uint[this.AcceptedStates.Length + pda2.AcceptedStates.Length];

            int i = 0; // store where D1 acc ends
            // iterate A1 acc
            for (; i < this.AcceptedStates.Length; i++)
                if (kind == JoinConcatUnionKind.Concat)
                    // Concat: lead accepted states from A1 to A2 start
                    pdat.AddM(this.AcceptedStates[i] + offsetA1, null, null, null, pda2.StartState + offsetA2);
                else
                    // Join, Union: states from A1 are normal accepted
                    accStates[i] = this.AcceptedStates[i] + offsetA1;

            if (kind == JoinConcatUnionKind.Concat)
                i = 0;

            // iterate A2 acs and + offsetA2
            for (int j = 0; j < pda2.AcceptedStates.Length; j++)
                accStates[i + j] = (pda2.AcceptedStates[j] + offsetA2);

            if (this is StatePDA)
                return new StatePDA($"{kind.ToString()}({Name}+{pda2.Name})", this.StatesCount + pda2.StatesCount + offsetA1, inputAlphabet, workAlphabet, pdat, startState, pda2.StartSymbol, accStates);
            else if (this is StackPDA)
                return new StackPDA($"{kind.ToString()}({Name}+{pda2.Name})", this.StatesCount + pda2.StatesCount + offsetA1, inputAlphabet, workAlphabet, pdat, startState, pda2.StartSymbol);
            else
                throw new System.NotImplementedException();
        }

        public IAcceptWord Reverse() {
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
            foreach (var dt in this.Transforms.Reverse())
                foreach (var dtv in dt.Value)
                    pdat.AddM(newstate[dtv.qNext], dt.Key.ci, dt.Key.cw, dtv.cw2, newstate[dt.Key.q]);

            //start state is qe, which leads to every old accepted state
            for (int i = 0; i < this.AcceptedStates.Length; i++)
                pdat.AddM(0, null, null, null, newstate[this.AcceptedStates[i]]);

            if (this is StatePDA)
                return new StatePDA($"Reverse({Name})", names, this.Alphabet, this.WorkAlphabet, pdat, 0, this.StartSymbol, new uint[] { this.StatesCount });
            else if (this is StackPDA)
                return new StackPDA($"Reverse({Name})", names, this.Alphabet, this.WorkAlphabet, pdat, 0, this.StartSymbol);
            else
                throw new System.NotImplementedException();
        }

        #endregion

    } //end class

} //end ns