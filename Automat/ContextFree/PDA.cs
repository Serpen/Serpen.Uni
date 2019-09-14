using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.ContextFree {
    
    /// <summary>
    /// nondeterministic PDA with Stack Symbol, which must end as empty stack and accepted state
    /// </summary>
    public abstract class PDA : AutomatBase<PDATransformKey, PDATransformValue[]> {

        protected static readonly char[] EXTRASYMBOLS =  new char[] {'§','$','%','&'};
        public const char START = '$';
        public readonly char[] WorkAlphabet;
        public new readonly PDATransform Transform;
        public readonly char StartStackSymbol;
        
        /// <summary>
        /// Create a PDA which accepts when ending in Accepted States
        /// </summary>
        /// <param name="StatesCount"></param>
        /// <param name="InputAlphabet"></param>
        /// <param name="Workalphabet"></param>
        /// <param name="Transform"></param>
        /// <param name="StartState"></param>
        /// <param name="Startstacksymbol"></param>
        /// <param name="AcceptedStates">Accepted Endstates</param>
        public PDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char Startstacksymbol, uint[] AcceptedStates)
        : base(StatesCount, InputAlphabet, StartState, name) {
            this.WorkAlphabet = Workalphabet;
            if (!Workalphabet.Contains(Startstacksymbol))
                this.WorkAlphabet = this.WorkAlphabet.Append(Startstacksymbol).ToArray();
            this.Transform = Transform;
            this.StartStackSymbol = Startstacksymbol;
            this.AcceptedStates = AcceptedStates;

            checkConstraints();
        }


        protected void checkConstraints() {
            foreach (var t in Transform)
            {
                for (int i = 0; i < t.Value.Length; i++)
                {
                    if (t.Key.q > StatesCount | t.Value[i].qNext > StatesCount)
                        throw new System.ArgumentOutOfRangeException("State not in states");
                    else if (t.Key.ci.HasValue && !Alphabet.Contains(t.Key.ci.Value))
                        throw new System.ArgumentOutOfRangeException("char not in InputAlphabet");
                    else if (t.Key.cw.HasValue && !WorkAlphabet.Contains(t.Key.cw.Value))
                        throw new System.ArgumentOutOfRangeException("char not in WorkAlphabet");
                    else if (t.Value[i].cw2 != null && t.Value[i].cw2 != "" && !WorkAlphabet.Contains(t.Value[i].cw2[0]))
                        throw new System.ArgumentOutOfRangeException("char2 not in WorkAlphabet");
                    
                }
            }
            for (int i = 0; i < AcceptedStates.Length; i++)
                if (AcceptedStates[i] > StatesCount)
                        throw new System.ArgumentOutOfRangeException("Accepted State not in states");
            
        }

        //nondeterministic makes it grow exponetially, maybe it is best, to do a BFS instead of DFS, follow one path to its possible end
        public PDAConfig[] GoChar(PDAConfig[] pcfgs) {
            var retCfgs = new List<PDAConfig>();

            foreach (var pcfg in pcfgs)
            {
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
                if (Transform.TryGetValue(ref qStart, out qNext)) {
                    //iterate each cuple of start and next transform
                    for (int j = 0; j < qNext.Length; j++)
                    {
                        var newStack = new Stack<char>(pcfg.Stack.Reverse());
                        var qNextj = qNext[j];

                        //stack symbol cw2 replaces cw
                        if (qStart[j].cw.HasValue && qNext[j].cw2 != null) {
                            newStack.Pop();
                            for (int i = qNextj.cw2.Length-1; i >= 0 ; i--)
                                newStack.Push(qNextj.cw2[i]);

                        //cw was used, replace with e, so pop
                        } else if (qStart[j].cw.HasValue && qNext[j].cw2 == null) {
                            newStack.Pop();
                        
                        //any (e-cw) should be replaced by cw2
                        } else if (!qStart[j].cw.HasValue && qNext[j].cw2 != null) {
                            for (int i = qNextj.cw2.Length-1; i >= 0 ; i--)
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
                    if (rcfg.Stack.Length <= rcfg.word.Length*3) { 
                        maxStackLen = rcfg.word.Length;
                        retCfgs2.Add(rcfg);
                    }
                }
            } else
                retCfgs2 = new List<PDAConfig>(retCfgs);

            return retCfgs2.Distinct().ToArray();
        }

        public override bool AcceptWord(string w) {
            return AcceptWordBySomeCrappyMiscOfBoth(w);
        }

        internal bool AcceptWordBySomeCrappyMiscOfBoth(string w) {
            //construct start config
            PDAConfig[] pcfg;
            if (StartStackSymbol != 0)
                pcfg = new PDAConfig[] {new PDAConfig(StartState, w, new char[] {StartStackSymbol}, null)};
            else
                pcfg = new PDAConfig[] {new PDAConfig(StartState, w, new char[] {}, null)};

            //while any pcfg exists
            while (pcfg.Length > 0) { //&& (pcfg.Where((a) => a.Stack.Length>0).Any())
                pcfg = GoChar(pcfg);

                var notfoolstates = new List<PDAConfig>();
                //check if a cfg has word and stack cleared and ends in accepted states
                foreach (var p in pcfg) {
                    if (p.Stack.Count() == 0 && p.word.Length == 0) {
                        if (AcceptedStates.Contains(p.q)) {
                            return true;
                        } else {
                            System.Console.WriteLine($"{pcfg} seems useless");
                        } //state is useless and could't move anymore??? --> IS THAT TRUE???
                    } else
                        notfoolstates.Add(p);
                }
                pcfg = notfoolstates.ToArray();
            }

            return false;
        }

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