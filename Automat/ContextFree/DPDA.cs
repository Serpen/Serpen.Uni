using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Automat.ContextFree {

    /// <summary>
    /// deterministic PDA with Stack Symbol, which must end as empty stack and accepted state
    /// </summary>
    [System.Serializable]
    public class DPDA : AutomatBase<PDATransformKey, PDATransformValue>, IPDA {

        public DPDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, DPDATransform Transform, uint StartState, char? Startsymbol, uint[] acceptedStates)
        : base(name, StatesCount, InputAlphabet, StartState, acceptedStates) {
            this.WorkAlphabet = Workalphabet;
            this.Transforms = Transform;
            this.StartSymbol = Startsymbol;

            CheckConstraints();
        }

        public DPDA(string name, string[] names, char[] InputAlphabet, char[] Workalphabet, DPDATransform Transform, uint StartState, char? Startsymbol, uint[] acceptedStates)
                : base(name, names, InputAlphabet, StartState, acceptedStates) {
            this.WorkAlphabet = Workalphabet;
            this.Transforms = Transform;
            this.StartSymbol = Startsymbol;

            CheckConstraints();
        }
        public DPDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, DPDATransform Transform, uint StartState, params uint[] acceptedStates)
        : base(name, StatesCount, InputAlphabet, StartState, acceptedStates) {
            this.WorkAlphabet = Workalphabet;
            this.Transforms = Transform;

            CheckConstraints();
        }

        public DPDA(string name, string[] names, char[] InputAlphabet, char[] Workalphabet, DPDATransform Transform, uint StartState, params uint[] acceptedStates)
        : base(name, names, InputAlphabet, StartState, acceptedStates) {
            this.WorkAlphabet = Workalphabet;
            this.Transforms = Transform;

            CheckConstraints();
        }

        public static explicit operator DPDA(Finite.DFA D) {
            var t = new DPDATransform();
            foreach (var t2 in (Finite.DFATransform)D.Transforms)
                t.Add(t2.Key.q, t2.Key.c, null, null, t2.Value[0]);
            for (int i = 0; i < D.AcceptedStates.Length; i++) {
                t.Add(D.AcceptedStates[i], null, DPDA.START, null, D.StatesCount);
            }

            return new DPDA($"DPDA_({D.Name})", D.StatesCount + 1, D.Alphabet, new char[] { DPDA.START }, t, D.StartState, DPDA.START, new uint[] { D.StatesCount });
        }

        public static explicit operator StatePDA(DPDA D) {
            var t = new PDATransform();
            foreach (var t2 in D.Transforms)
                t.Add(t2.Key.q, t2.Key.ci, t2.Key.cw, t2.Value.cw2, t2.Value.qNext);

            return new StatePDA($"QPDA_({D.Name})", D.StatesCount, D.Alphabet, D.WorkAlphabet, t, D.StartState, D.StartSymbol, D.AcceptedStates);
        }


        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var t in Transforms) {
                if (t.Key.q >= StatesCount | t.Value.qNext >= StatesCount) //transform states within states
                    throw new StateException(t.Key.q, this);
                else if (t.Key.ci.HasValue && !Alphabet.Contains(t.Key.ci.Value)) //input alphabet of transform key
                    throw new AlphabetException(t.Key.ci.Value, this);
                else if (t.Key.cw.HasValue && !WorkAlphabet.Contains(t.Key.cw.Value)) //work alphabet of transform key
                    throw new AlphabetException(t.Key.cw.Value, this);
                //BUG: contains string
                else if (t.Value.cw2 != null && !WorkAlphabet.Contains(t.Value.cw2[0])) //work alphabet of transform value
                    throw new AlphabetException(t.Value.cw2[0], this);
            }

            //accepted States in States
            for (int i = 0; i < AcceptedStates.Length; i++)
                if (AcceptedStates[i] >= StatesCount)
                    throw new StateException(AcceptedStates[i], this);
        }

        public const char START = '$';
        public char[] WorkAlphabet { get; }
        public char? StartSymbol { get; }

        IList<PDAConfig> IPDA.GoChar(PDAConfig[] pcfgs) => new PDAConfig[] { GoChar(pcfgs[0]) };
        public PDAConfig GoChar(PDAConfig pcfg) {
            PDATransformKey qStart;

            //if word is empty, maybe only e-Transform is needed
            if (!string.IsNullOrEmpty(pcfg.word))
                qStart = new PDATransformKey(pcfg.State, pcfg.word[0], pcfg.Stack[^1]);
            else
                qStart = new PDATransformKey(pcfg.State, null, pcfg.Stack[^1]);

            //get all possible (e-)transforms
            if (!((DPDATransform)Transforms).TryGetValue(ref qStart, out PDATransformValue qNext))
                return null;
            else {
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

            }
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            //construct start config
            PDAConfig pcfg;
            if (StartSymbol.HasValue)
                pcfg = new PDAConfig(StartState, w, new char[] { StartSymbol.Value }, null);
            else
                pcfg = new PDAConfig(StartState, w, System.Array.Empty<char>(), null);

            //while new pcfg exists and stack is still not empty
            while (pcfg != null && (pcfg.Stack.Length > 0)) {
                pcfg = GoChar(pcfg);
            }

            //check if a cfg has word cleared and ends in accepted states
            if (pcfg == null || pcfg.word.Length > 0)
                return false;
            else {
                if (IsAcceptedState(pcfg.State))
                    return true;
                else
                    return false;
            }
        }

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>(Transforms.Count);
            foreach (var t in Transforms) {
                string desc = $"{(t.Key.ci ?? Utils.EPSILON)}|{(t.Key.cw ?? Utils.EPSILON)}->{(!string.IsNullOrEmpty(t.Value.cw2) ? t.Value.cw2 : Utils.EPSILON.ToString())}";
                var vt = new VisualizationTuple(t.Key.q, t.Value.qNext, desc);
                tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public static DPDA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            var t = new DPDATransform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] workAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, new char[] { START });
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(1, inputAlphabet.Length);
                for (int j = 0; j < transformsRnd; j++) {
                    var tk = new PDATransformKey(i, inputAlphabet.RndElement(), workAlphabet.RndElement());
                    var tv = new PDATransformValue(workAlphabet.RndElement().ToString(), (uint)rnd.Next(0, stateCount));
                    t.TryAdd(tk, tv);
                }
            }

            var ret = new DPDA("DPDA_Random", (uint)stateCount, inputAlphabet, workAlphabet, t, (uint)rnd.Next(0, stateCount), START, accState);
            ret.Name = $"DPDA_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.RemovedStateTranslateTables();

            var newT = new DPDATransform();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext))
                        newT.Add(translate.ArrayIndex(t2.Key.q), t2.Key.ci, t2.Key.cw, t2.Value.cw2, translate.ArrayIndex(t2.Value.qNext));

            return new DPDA($"{Name}_purged", (uint)names.Length, Alphabet, WorkAlphabet, newT, translate.ArrayIndex(StartState), StartSymbol, aStates);
        }

        public virtual IAutomat Reverse() => ((StatePDA)this).Reverse();

        public override string ToString() => $"{Name} DPDA(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', WorkAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {StartSymbol} {{{string.Join(',', AcceptedStates)}}})".Trim();

    } //end class

    [System.Serializable]
    public class DPDATransform : TransformBase<PDATransformKey, PDATransformValue> {
        public void Add(uint q, char? ci, char? cw, string cw2, uint qNext)
            => base.Add(new PDATransformKey(q, ci, cw), new PDATransformValue(cw2, qNext));
        public void Add(uint q, char? ci, char? cw, char cw2, uint qNext)
            => base.Add(new PDATransformKey(q, ci, cw), new PDATransformValue(cw2.ToString(), qNext));

        public bool TryGetValue(ref PDATransformKey initcfg, out PDATransformValue qnext) {
            bool result;

            var worksuccessor = new PDATransformKey(initcfg.q, initcfg.ci, initcfg.cw);
            result = base.TryGetValue(worksuccessor, out qnext);
            if (result) {
                initcfg = worksuccessor;
                return true;
            }

            worksuccessor = new PDATransformKey(initcfg.q, initcfg.ci, null);
            result = base.TryGetValue(worksuccessor, out qnext);
            if (result) {
                initcfg = worksuccessor;
                return true;
            }

            worksuccessor = new PDATransformKey(initcfg.q, null, initcfg.cw);
            result = base.TryGetValue(worksuccessor, out qnext);
            if (result) {
                initcfg = worksuccessor;
                return true;
            }

            return false;

        }

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this.OrderBy(a => a.Key.ToString())) {
                sw.Append($"({item.Key.ToString()})=>");
                sw.Append($"({item.Value.ToString()}); ");
                sw.Append("); ");
            }
            return sw.ToString();
        }
    }

} //end ns