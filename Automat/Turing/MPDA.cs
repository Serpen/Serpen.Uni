using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.Turing {

    [AlgorithmSource("EFAK_8.5.2")]
    public class MultiPDA : AutomatBase<MultiPDA.MPDATransformKey, MultiPDA.MPDATransformValue[]> {

        protected const int MAX_RUNS_OR_STACK = 10000;
        protected static readonly char[] EXTRASYMBOLS = new char[] { '§', '$', '%', '&' };
        public const char START = '$';
        public readonly char[] WorkAlphabet;
        //public readonly PDATransform Transform;
        public readonly char StartStackSymbol;
        public readonly uint StackCount;

        public MultiPDA(string name, uint stackCount, uint statesCount, char[] inputAlphabet, char[] workalphabet, MPDATransform transform, uint startState, char startstacksymbol, uint[] acceptedStates)
        : base(name, statesCount, inputAlphabet, startState, acceptedStates) {
            this.WorkAlphabet = workalphabet;
            if (!workalphabet.Contains(startstacksymbol))
                this.WorkAlphabet = this.WorkAlphabet.Append(startstacksymbol).ToArray();
            this.Transforms = transform;
            this.StartStackSymbol = startstacksymbol;
            this.StackCount = stackCount;

            CheckConstraints();
        }

        public MultiPDA(string name, uint stackCount, string[] names, char[] inputAlphabet, char[] workalphabet, MPDATransform transform, uint startState, char startStacksymbol, uint[] acceptedStates)
        : base(name, names, inputAlphabet, startState, acceptedStates) {
            this.WorkAlphabet = workalphabet;
            if (!workalphabet.Contains(startStacksymbol))
                this.WorkAlphabet = this.WorkAlphabet.Append(startStacksymbol).ToArray();
            this.Transforms = transform;
            this.StartStackSymbol = startStacksymbol;
            this.StackCount = stackCount;

            CheckConstraints();
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            //construct start config
            int runCount = 0;
            MPDAConfig[] pcfgs;
            if (StartStackSymbol != 0) {
                char[][] initialStack = new char[StackCount][];
                for (int i = 0; i < initialStack.GetLength(0); i++)
                    initialStack[i] = new char[] { StartStackSymbol };
                pcfgs = new MPDAConfig[] { new MPDAConfig(StartState, w, initialStack, null) };
            } else
                pcfgs = new MPDAConfig[] { new MPDAConfig(StartState, w, new char[][] { }, null) };

            //while any pcfg exists
            while (pcfgs.Length > 0) { //&& (pcfg.Where((a) => a.Stack.Length>0).Any())
                Utils.DebugMessage(string.Join(',', (from a in pcfgs select a.ToString())), this, Uni.Utils.eDebugLogLevel.Normal);
                foreach (var p in pcfgs) {
                    if (p.word.Length == 0)
                        if (IsAcceptedState(p.State))
                            return true;
                } //next p

                pcfgs = GoChar(pcfgs);

                runCount++;
                if (pcfgs.Length > MAX_RUNS_OR_STACK || runCount > MAX_RUNS_OR_STACK) {
                    Utils.DebugMessage($"{runCount}: Stack >= {pcfgs.Length}, abort", this, Uni.Utils.eDebugLogLevel.Always);
                    return false;
                }

            }

            return false;
        }

        //nondeterministic makes it grow exponetially, maybe it is best, to do a BFS instead of DFS, follow one path to its possible end
        public MPDAConfig[] GoChar(MPDAConfig[] pcfgs) {
            var retCfgs = new List<MPDAConfig>();

            if (pcfgs.Length > MAX_RUNS_OR_STACK) {
                Utils.DebugMessage($"Stack >= {pcfgs.Length}, abort", this, Uni.Utils.eDebugLogLevel.Verbose);
                return new MPDAConfig[0];
            }

            foreach (var pcfg in pcfgs) {
                MPDATransformKey[] qStart = new MPDATransformKey[1];

                //if word is empty, maybe only e-Transform is needed
                if (pcfg.word != "") {
                    if (pcfg.Stack.Any())
                        qStart[0] = new MPDATransformKey(pcfg.State, pcfg.word[0], pcfg.Stack[0]);
                    // qStart[0] = new PDATransformKey(pcfg.q, pcfg.word[0], pcfg.Stack[pcfg.Stack.Length-1]);
                    else
                        qStart[0] = new MPDATransformKey(pcfg.State, pcfg.word[0], null);
                    // continue; //TODO: why that //maybe empty stack that is not accepted should be thrown away

                } else {
                    if (pcfg.Stack.Any())
                        qStart[0] = new MPDATransformKey(pcfg.State, null, pcfg.Stack[0]);
                    else
                        qStart[0] = new MPDATransformKey(pcfg.State, null, null);
                    // continue; //TODO: why that
                }

                MPDATransformValue[] nextVals;

                //get all possible (e-)transforms
                if (((MPDATransform)Transforms).TryGetValue(ref qStart, out nextVals)) {
                    //iterate each cuple of start and next transform
                    for (int v = 0; v < nextVals.Length; v++) {
                        var newStack = new Stack<char>[StackCount];
                        var NextVal = nextVals[v];
                        for (int s = 0; s < StackCount; s++) {
                            newStack[s] = new Stack<char>(pcfg.Stack[s].Reverse());
                            //stack symbol cw2 replaces cw
                            if (qStart[v].cw[s] != (char)0 && nextVals[v].cw2 != null) {
                                newStack[s].Pop();
                                for (int i = NextVal.cw2.Length - 1; i >= 0; i--)
                                    try { newStack[s].Push(NextVal.cw2[s][i]);} catch {}

                                //cw was used, replace with e, so pop
                            } else if (qStart[v].cw[s] != (char)0 && nextVals[v].cw2 == null) {
                                newStack[s].Pop();

                                //any (e-cw) should be replaced by cw2
                            } else if (qStart[v].cw[s] == (char)0 && nextVals[v].cw2 != null) {
                                for (int i = NextVal.cw2.Length - 1; i >= 0; i--)
                                    try {newStack[s].Push(NextVal.cw2[s][i]);} catch {}

                                //cw's not relevant, no stack action
                            } else if (qStart[v].cw[s] == (char)0 && nextVals[v].cw2 == null) {
                                //do nothing
                            } else
                                throw new System.NotSupportedException("some condition forgotten??");

                            //transform was because of i, then remove words first letter
                            string newWord = pcfg.word;
                            if (qStart[v].ci.HasValue)
                                if (pcfg.word.Length > 0)
                                    newWord = pcfg.word.Substring(1);

                            char[][] newStackArray = new char[StackCount][];
                            for (int i = 0; i < StackCount; i++)
                                newStackArray[i] = newStack[i].ToArray(); 
                        
                        retCfgs.Add(new MPDAConfig(nextVals[v].qNext, newWord, newStackArray, pcfg));
                        }
                    } //next j

                } //end if tryGetValue
            } //next pcfg

            //buggy, but only perf way
            List<MPDAConfig> retCfgs2;
            if (retCfgs.Count > 100) {
                retCfgs2 = new List<MPDAConfig>();
                int maxStackLen = int.MaxValue;
                foreach (var rcfg in retCfgs) {
                    if (rcfg.Stack.Length <= rcfg.word.Length * 3) {
                        maxStackLen = rcfg.word.Length;
                        retCfgs2.Add(rcfg);
                    }
                }
            } else
                retCfgs2 = new List<MPDAConfig>(retCfgs);

            return retCfgs2.Distinct().ToArray();
        }



        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>();
            foreach (var t in Transforms) {
                foreach (var v in t.Value) {
                    string desc = $"{(t.Key.ci.HasValue ? t.Key.ci.Value : Utils.EPSILON)}|{t.Key.cw}->{v.cw2}";
                    var vt = new VisualizationTuple(t.Key.q, v.qNext, desc);
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public override string ToString() => $"{Name} PDA(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', WorkAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {StartStackSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();


        public static MultiPDA GenerateRandom() {
            throw new System.NotImplementedException();
            // const byte MAX_STATES = 20;
            // const byte MAX_CHAR = 7;

            // var rnd = Utils.RND;

            // var t = new PDATransform();
            // int stateCount = rnd.Next(1, MAX_STATES);

            // char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            // char[] workAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, new char[] {START}, 0);
            // uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount/3, stateCount); 

            // for (uint i = 0; i < stateCount; i++) {
            //     int transformsRnd = rnd.Next(0, inputAlphabet.Length);
            //     for (int j = 0; j < transformsRnd; j++) {
            //         t.AddM(i, Utils.GrAE(inputAlphabet), Utils.GrAE(workAlphabet), Utils.GrAE(workAlphabet).ToString(), (uint)rnd.Next(0, stateCount));
            //     }
            // }

            // var ret = new MultiPDA("QPDA_Random", (uint)stateCount, inputAlphabet, workAlphabet , t, (uint)rnd.Next(0,stateCount), START , accState);
            // ret.Name = $"QPDA_Random_{ret.GetHashCode()}";
            // return ret;
        }


        public override IAutomat PurgeStates() {
            throw new System.NotImplementedException();
            // (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();

            // var newT = new PDATransform();
            // foreach (var t2 in Transform)
            //     if (translate.Contains(t2.Key.q))
            //         foreach (var v in t2.Value)
            //             if (translate.Contains(v.qNext))
            //                 newT.AddM(Utils.ArrayIndex(translate,t2.Key.q), t2.Key.ci, t2.Key.cw, v.cw2, Utils.ArrayIndex(translate,v));

            // return new StatePDA($"{Name}_purged", names, Alphabet, WorkAlphabet, newT, Utils.ArrayIndex(translate,StartState), StartStackSymbol, aStates);

        }


        public class MPDATransform : TransformBase<MPDATransformKey, MPDATransformValue[]> {
            public void Add(uint q, char? ci, char[] cw, string[] cw2, uint qNext)
                => Add(new MPDATransformKey(q, ci, cw), new MPDATransformValue[] { new MPDATransformValue(cw2, qNext) });


            /// <summary>
            /// Adds Tuple + Appends if already exists
            /// </summary>
            public void AddM(uint q, char? ci, char[] cw, string[] cw2, uint qNext) {
                MPDATransformValue[] pvalRef;
                MPDATransformValue pval = new MPDATransformValue(cw2, qNext);
                var pkey = new MPDATransformKey(q, ci, cw);

                if (TryGetValue(pkey, out pvalRef))
                    this[pkey] = pvalRef.Append(pval).ToArray();
                else
                    base.Add(pkey, new MPDATransformValue[] { pval });
            }
            public bool TryGetValue(ref MPDATransformKey[] initcfg, out MPDATransformValue[] qnext) {
                var retVals = new List<MPDATransformValue>();
                var retKeys = new List<MPDATransformKey>();

                for (int i = 0; i < initcfg.Length; i++) {
                    MPDATransformKey workSuccessor;
                    try {
                        workSuccessor = new MPDATransformKey(initcfg[i].q, initcfg[i].ci, new char[]{initcfg[i].cw[0]});
                    } catch {
                        workSuccessor = new MPDATransformKey(initcfg[i].q, initcfg[i].ci, new char[]{(char)0});
                    }
                    if (base.TryGetValue(workSuccessor, out qnext)) {
                        Utils.DebugMessage($"full match {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                        retVals.AddRange(qnext);
                        for (int j = 0; j < qnext.Length; j++)
                            retKeys.Add(workSuccessor);
                        // continue;
                    }

                    workSuccessor = new MPDATransformKey(initcfg[i].q, initcfg[i].ci, new char[1] {(char)0});
                    if (base.TryGetValue(workSuccessor, out qnext)) {
                        Utils.DebugMessage($"ignore work char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                        retVals.AddRange(qnext);
                        for (int j = 0; j < qnext.Length; j++)
                            retKeys.Add(workSuccessor);
                        // continue;
                    }

                    try {
                        workSuccessor = new MPDATransformKey(initcfg[i].q, null, new char[]{initcfg[i].cw[0]});
                    } catch {
                        workSuccessor = new MPDATransformKey(initcfg[i].q, null, new char[]{(char)0});
                    }
                    if (base.TryGetValue(workSuccessor, out qnext)) {
                        Utils.DebugMessage($"ignore input char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                        retVals.AddRange(qnext);
                        for (int j = 0; j < qnext.Length; j++)
                            retKeys.Add(workSuccessor);
                        // continue;
                    }

                    workSuccessor = new MPDATransformKey(initcfg[i].q, null, new char[1] {(char)0});
                    if (base.TryGetValue(workSuccessor, out qnext)) {
                        Utils.DebugMessage($"ignore input,work char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                        retVals.AddRange(qnext);
                        for (int j = 0; j < qnext.Length; j++)
                            retKeys.Add(workSuccessor);
                        // continue;
                    }
                } //next i

                if (retKeys.Count > 0) {
                    initcfg = retKeys.ToArray();
                    qnext = retVals.ToArray();
                    return true;
                } else {
                    qnext = retVals.ToArray();
                    return false;
                }

            }

            public override string ToString() {
                var sw = new System.Text.StringBuilder();
                foreach (var item in this) {
                    sw.Append($"({item.Key.ToString()})=>");
                    foreach (var v in item.Value)
                        sw.Append($"({v.ToString()}); ");
                    // sw.Append("); ");
                }
                return sw.ToString();
            }
        }

        public struct MPDATransformValue : ITransformValue {

            public MPDATransformValue(string[] cw2, uint qnext) {
                this.cw2 = cw2;
                this.qNext = qnext;
            }

            public string[] cw2 { get; }
            public uint qNext { get; }

            public override string ToString() => $"({string.Join(',', cw2)}, {qNext})";
        }

        public struct MPDATransformKey : ITransformKey {
            public MPDATransformKey(uint q, char? ci, char[] cw) {
                this.q = q;
                this.ci = ci;
                this.cw = cw;
            }
            public uint q { get; }
            public char? ci { get; }

            char[] ITransformKey.c {
                get {
                    if (ci.HasValue)
                        return new char[] { ci.Value };
                    else
                        return new char[] { };
                }
            }
            public char[] cw { get; }


            public override bool Equals(object obj) {
                if (obj is MPDATransformKey pk) {
                    return pk.q == this.q && pk.ci == this.ci && (new string(pk.cw) == new string(this.cw));
                }
                return false;
            }

            public override int GetHashCode() => this.ToString().GetHashCode();

            public override string ToString()
                => $"({q}, {(ci.HasValue ? ci.Value : Utils.EPSILON)}, {string.Join(',', cw)})";
        }

    }

    public class MPDAConfig : IConfig {
        public uint State { get; }
        public string word { get; }
        public char[][] Stack { get; }
        public MPDAConfig Origin { get; }
        public string OriginPath() {
            var pcfg = this;
            var sw = new System.Text.StringBuilder();
            sw.Append($"[{pcfg.ToString()}]");
            while (pcfg.Origin != null) {
                pcfg = pcfg.Origin;
                sw.Append($"<-[{pcfg.ToString()}]");
            }
            return sw.ToString();
        }

        public MPDAConfig(uint q, string w, char[][] stack, MPDAConfig origin) {
            this.State = q;
            this.word = w;
            this.Stack = stack;
            this.Origin = origin;
        }

        public override string ToString() {
            var stackstr = new System.Text.StringBuilder();
            for (int i = 0; i < Stack.GetLength(0); i++)
            {
                stackstr.Append(new string(Stack[i]));
                stackstr.Append('|');
            }
            return $"({State},'{word}','{stackstr}')";
        }
        // public override string ToString() => $"({q},'{word}','{string.Join("", Stack)}')";
    }
}