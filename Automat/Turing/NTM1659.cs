using System.Linq;

namespace Serpen.Uni.Automat.Turing {

    [System.Serializable]
    public class NTM1659 : TuringMachineBase<TuringKey, TuringVal[]> {

        public NTM1659(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, NTM1659Transform transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] { acceptedState }) {
            Transforms = transform;
            DiscardState = discardState;
        }
        public NTM1659(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, NTM1659Transform transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] { acceptedState }) {
            Transforms = transform;
            DiscardState = discardState;
        }

        public uint AcceptedState => base.AcceptedStates[0];

        public uint DiscardState { get; }

        TuringConfigSingleBand[] GoChar(TuringConfigSingleBand[] tcfgs) {
            var retTcfgs = new System.Collections.Generic.List<TuringConfigSingleBand>(Transforms.Count * 10);
            foreach (var tcfg in tcfgs) {
                if (Transforms.TryGetValue(new TuringKey(tcfg.State, tcfg.CurSymbol), out TuringVal[] tvas)) {
                    foreach (var tva in tvas) {
                        var ntcfg = new TuringConfigSingleBand(this.BlankSymbol, tcfg.Band, tcfg.Position);
                        ntcfg.ReplaceChar(tva.c2, tva.Direction);
                        ntcfg.State = tva.qNext;
                        retTcfgs.Add(ntcfg);
                    }
                }
            }
            return retTcfgs.ToArray();
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            var tcfgs = new TuringConfigSingleBand[] { new TuringConfigSingleBand(BlankSymbol, w, 0) { State = StartState } };
            int runs = 0;
            uint lastQ = tcfgs[0].State;
            while (tcfgs.Length > 0) {
                foreach (var tcfg in tcfgs) {
                    Utils.DebugMessage(tcfg.ToString(), this, Uni.Utils.eDebugLogLevel.Verbose);
                    if (tcfg != null)
                        lastQ = tcfg.State;
                    if (runs > MAX_TURING_RUNS)
                        throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}", this);
                    runs++;
                    if (IsAcceptedState(lastQ))
                        return true;
                    else if (DiscardState == lastQ) // TM1659 should end when in discard state
                        return false;
                }
                tcfgs = GoChar(tcfgs);
            }
            return false;
        }

        public override string GetBandOutput(string w) {
            var tcfgs = new TuringConfigSingleBand[] { new TuringConfigSingleBand(BlankSymbol, w, 0) { State = StartState } };
            int runs = 0;
            uint lastQ = tcfgs[0].State;
            string lastBand = tcfgs[0].Band;

            while (tcfgs.Length > 0 && !IsAcceptedState(lastQ)) {
                foreach (var tcfg in tcfgs) {
                    Utils.DebugMessage(tcfg.ToString(), this, Uni.Utils.eDebugLogLevel.Verbose);
                    if (tcfg != null)
                        lastBand = tcfg.Band;
                    if (runs > MAX_TURING_RUNS)
                        throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}", this);
                    runs++;
                }
                tcfgs = GoChar(tcfgs);
            }
            return lastBand.Trim(BlankSymbol);
        }

        public static NTM1659 GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            var t = new NTM1659Transform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, inputAlphabet.Append(BLANK));

            int transformsRnd = rnd.Next(0, stateCount * inputAlphabet.Length);
            for (uint k = 0; k < transformsRnd; k++) {
                var tk = new TuringKey((uint)rnd.Next(0, stateCount), bandAlphabet.RndElement());
                var tv = new TuringVal((uint)rnd.Next(0, stateCount), bandAlphabet.RndElement(), TMDirection.Right);
                t.TryAdd(tk, new TuringVal[] { tv });
            }

            var ret = new NTM1659("NTM1_Random", (uint)stateCount, inputAlphabet, bandAlphabet, t, (uint)rnd.Next(0, stateCount), BLANK, (uint)rnd.Next(0, stateCount), (uint)rnd.Next(0, stateCount));
            ret.Name = $"TM1659_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, _) = base.RemovedStateTranslateTables();

            var newT = new NTM1659Transform();
            foreach (var ti in Transforms)
                if (translate.Contains(ti.Key.q))
                    foreach (var tv in ti.Value)
                        if (translate.Contains(tv.qNext))
                            newT.AddM(translate.ArrayIndex(ti.Key.q), ti.Key.c, translate.ArrayIndex(tv.qNext), tv.c2, tv.Direction);

            return new NTM1659($"{Name}_purged", (uint)names.Length, Alphabet, BandAlphabet, newT, translate.ArrayIndex(StartState), BlankSymbol, translate.ArrayIndex(AcceptedState), translate.ArrayIndex(DiscardState));
        }

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>(Transforms.Count);
            foreach (var t in Transforms) {
                foreach (var tv in t.Value) {
                    var vt = new VisualizationTuple(t.Key.q, tv.qNext,
                    $"{t.Key.c}|{tv.c2} {tv.Direction}");
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {BlankSymbol}, {AcceptedState}, {DiscardState})".Trim();

    }

    [System.Serializable]
    public sealed class NTM1659Transform : TransformBase<TuringKey, TuringVal[]> {

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var ti in this.OrderBy(a => a.Key.ToString())) {
                foreach (var tv in ti.Value) {
                    sw.Append($"({ti.Key.ToString()})=>");
                    sw.Append($"({tv.ToString()}); ");
                }
                // sw.Append("); ");
            }
            return sw.ToString();
        }

        public void Add(uint q, char c, uint qNext, char c2, TMDirection dir) =>
            Add(new TuringKey(q, c), new TuringVal[] { new TuringVal(qNext, c2, dir) });

        internal void AddM(uint q, char c, uint qNext, char c2, TMDirection dir) {
            var eat = new TuringKey(q, c);
            if (TryGetValue(eat, out TuringVal[] qBefore))
                this[eat] = qBefore.Append(new TuringVal(qNext, c2, dir)).ToArray();
            else
                Add(new TuringKey(q, c), new TuringVal[] { new TuringVal(qNext, c2, dir) });
        }

    }
}