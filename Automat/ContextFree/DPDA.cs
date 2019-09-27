using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.ContextFree {
    
    /// <summary>
    /// deterministic PDA with Stack Symbol, which must end as empty stack and accepted state
    /// </summary>
    public class DPDA : AutomatBase<PDATransformKey, PDATransformValue> {
        public DPDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, DPDATransform Transform, uint StartState, char Startsymbol, uint[] acceptedStates)
        : base(StatesCount, InputAlphabet, StartState, name, acceptedStates) {
            this.WorkAlphabet = Workalphabet;
            this.Transform = Transform;
            this.StartSymbol = Startsymbol;

            CheckConstraints();
        }

        public static explicit operator DPDA(Finite.DFA D) {
            var t = new DPDATransform();
            foreach (var t2 in (Finite.FATransform)D.Transform)
                t.Add(t2.Key.q, t2.Key.c, null, null, t2.Value[0]);
            for (int i = 0; i < D.AcceptedStates.Length; i++) {
                t.Add(D.AcceptedStates[i], null, DPDA.START, null, D.StatesCount);
            }

            return new DPDA($"DPDA_({D.Name})", D.StatesCount + 1, D.Alphabet, new char[] {DPDA.START}, t, D.StartState, DPDA.START, new uint[] { D.StatesCount });
        }


        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var t in Transform)
            {
                if (t.Key.q > StatesCount | t.Value.qNext > StatesCount) //transform states within states
                    throw new StateException(t.Key.q);
                else if (t.Key.ci.HasValue && !Alphabet.Contains(t.Key.ci.Value)) //input alphabet of transform key
                    throw new AlphabetException(t.Key.ci.Value);
                else if (t.Key.cw.HasValue && !WorkAlphabet.Contains(t.Key.cw.Value)) //work alphabet of transform key
                    throw new AlphabetException(t.Key.cw.Value);
                //BUG: contains string
                else if (t.Value.cw2 != null && !WorkAlphabet.Contains(t.Value.cw2[0])) //work alphabet of transform value
                    throw new AlphabetException(t.Value.cw2[0]);
            }

            //accepted States in States
            for (int i = 0; i < AcceptedStates.Length; i++)
                if (AcceptedStates[i] > StatesCount)
                    throw new StateException(AcceptedStates[i]);   
        }

        public const char START = '$';
        public readonly char[] WorkAlphabet;
        public readonly char StartSymbol;

        public PDAConfig GoChar(PDAConfig pcfg) {
            PDATransformKey qStart;

            //if word is empty, maybe only e-Transform is needed
            if (pcfg.word != "") 
                qStart = new PDATransformKey(pcfg.q, pcfg.word[0], pcfg.Stack[pcfg.Stack.Length-1]);
            else
                qStart = new PDATransformKey(pcfg.q, null, pcfg.Stack[pcfg.Stack.Length-1]);


            PDATransformValue qNext;

            //get all possible (e-)transforms
            if (((DPDATransform)Transform).TryGetValue(ref qStart, out qNext)) {
                var newStack = new Stack<char>(pcfg.Stack); //new stack vor pcfg

                //stack symbol cw2 replaces cw
                if (qStart.cw.HasValue && qNext.cw2 != null) {
                    newStack.Pop();
                    for (int i = 0; i < qNext.cw2.Length; i++)
                        newStack.Push(qNext.cw2[i]);

                //cw was used, replace with e, so pop
                } else if (qStart.cw.HasValue && qNext.cw2 == null) {
                    newStack.Pop();

                //any (e-cw) should be replaced by cw2
                } else if (!qStart.cw.HasValue && qNext.cw2 != null) {
                    for (int i = 0; i < qNext.cw2.Length; i++)
                        newStack.Push(qNext.cw2[i]);

                //cw's not relevant, no stack action
                } else if (!qStart.cw.HasValue && qNext.cw2 == null) {
                    //do nothing
                } else
                    throw new System.NotSupportedException("some condition forgotten??");

                //transform was because of i, then remove words first letter
                if (qStart.ci.HasValue)
                    return new PDAConfig(qNext.qNext, pcfg.word.Substring(1), newStack.Reverse().ToArray(), pcfg);
                else
                    return new PDAConfig(qNext.qNext, pcfg.word, newStack.Reverse().ToArray(), pcfg);

            } else
                return null;     
        }

        public override bool AcceptWord(string w) {
            //construct start config
            var pcfg = new PDAConfig(StartState, w, new char[] {StartSymbol}, null);

            //while new pcfg exists and stack is still not empty
            while (pcfg != null && (pcfg.Stack.Length>0)) {
                pcfg = GoChar(pcfg);
            }
            
            //check if a cfg has word cleared and ends in accepted states
            if (pcfg == null || pcfg.word.Length > 0)
                return false;
            else
                if (IsAcceptedState(pcfg.q))
                    return true;
                else
                    return false;
        }

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<System.Tuple<int, int, string>>();
            foreach (var t in Transform) {
                string desc = $"{(t.Key.ci.HasValue ? t.Key.ci.Value : Utils.EPSILON)}|{(t.Key.cw.HasValue ? t.Key.cw.Value : Utils.EPSILON)}->{(!string.IsNullOrEmpty(t.Value.cw2) ? t.Value.cw2 : Utils.EPSILON.ToString())}";
                var vt = new System.Tuple<int, int, string>((int)t.Key.q, (int)t.Value.qNext, desc);
                 tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public static DPDA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            var t = new DPDATransform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] workAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, new char[] {START}, 0);
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount/3, stateCount); 

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (int j = 0; j < transformsRnd; j++) {
                    try {
                        var tk = new PDATransformKey(i, Utils.GrAE(inputAlphabet), Utils.GrAE(workAlphabet));
                        var tv = new PDATransformValue(Utils.GrAE(workAlphabet).ToString(), (uint)rnd.Next(0, stateCount));
                        t.TryAdd(tk,tv);
                    } catch {}
                }
            }

            var ret = new DPDA("DPDA_Random", (uint)stateCount, inputAlphabet, workAlphabet , t, (uint)rnd.Next(0,stateCount), START , accState);
            ret.Name = $"DPDA_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();

            var newT = new DPDATransform();
            foreach (var t2 in Transform)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext))
                        newT.Add(Utils.ArrayIndex(translate,t2.Key.q), t2.Key.ci, t2.Key.cw, t2.Value.cw2, Utils.ArrayIndex(translate,t2.Value));

            return new DPDA($"{Name}_purged", (uint)names.Length, Alphabet, WorkAlphabet, newT, Utils.ArrayIndex(translate,StartState), StartSymbol, aStates);
        }

        public override string ToString() => $"{Name} DPDA(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', WorkAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {StartSymbol} {{{string.Join(',', AcceptedStates)}}})".Trim();
        
    } //end class
} //end ns