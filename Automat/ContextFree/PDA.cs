using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.ContextFree {

    /// <summary>
    /// nondeterministic PDA with Stack Symbol, which must end as empty stack and accepted state
    /// </summary>
    public abstract class PDA : AutomatBase<PDATransformKey, PDATransformValue[]> {

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
                    if (t.Key.q > StatesCount | t.Value[i].qNext > StatesCount)
                        throw new StateException(t.Key.q);
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
                Utils.DebugMessage($"Stack >= {pcfgs.Length}, abort", this);
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

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<System.Tuple<int, int, string>>();
            foreach (var t in Transform) {
                foreach (var v in t.Value) {
                    string desc = $"{(t.Key.ci.HasValue ? t.Key.ci.Value : Utils.EPSILON)}|{(t.Key.cw.HasValue ? t.Key.cw.Value : Utils.EPSILON)}->{(!string.IsNullOrEmpty(v.cw2) ? v.cw2 : Utils.EPSILON.ToString())}";
                    var vt = new System.Tuple<int, int, string>((int)t.Key.q, (int)v.qNext, desc);
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public override string ToString() => $"{Name} PDA(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', WorkAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {StartStackSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();
    } //end class

} //end ns