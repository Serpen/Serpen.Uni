using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class NTM1659 : TuringMachineBase<TuringTransformSingleBand.TuringKey, TuringTransformSingleBand.TuringVal[]> {

        public NTM1659(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, NTM1659Transform transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] { acceptedState }) {
            Transform = transform;
            DiscardState = discardState;
        }
        public NTM1659(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, NTM1659Transform transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] { acceptedState }) {
            Transform = transform;
            DiscardState = discardState;
        }

        public uint AcceptedState => base.AcceptedStates[0];

        public uint DiscardState { get; }

        TuringConfigSingleBand[] GoChar(TuringConfigSingleBand[] tcfgs) {
            TuringTransformSingleBand.TuringVal[] tvas;
            var retTcfgs = new System.Collections.Generic.List<TuringConfigSingleBand>();
            foreach (var tcfg in tcfgs) {
                if (Transform.TryGetValue(new TuringTransformSingleBand.TuringKey(tcfg.State, tcfg.CurSymbol), out tvas)) {
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

            var tcfgs = new TuringConfigSingleBand[] {new TuringConfigSingleBand(BlankSymbol, w, 0) { State = StartState }};
            int runs = 0;
            uint lastQ = tcfgs[0].State;
            while (tcfgs.Length > 0) {
                foreach (var tcfg in tcfgs) {
                    Utils.DebugMessage(tcfg.ToString(), this, Utils.eDebugLogLevel.Verbose);
                    if (tcfg != null)
                        lastQ = tcfg.State;
                    if (runs > MAX_TURING_RUNS)
                        throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
                    runs++;
                    if (IsAcceptedState(lastQ))
                        return true;
                    // else if (DiscardState == lastQ) // TM1659 should end when in discard state
                    //     foundAccepted = false;
                }
                tcfgs = GoChar(tcfgs);
            }
            return false;
        }

        public override string GetBandOutput(string w) {
            var tcfgs = new TuringConfigSingleBand[] {new TuringConfigSingleBand(BlankSymbol, w, 0) { State = StartState }};
            int runs = 0;
            uint lastQ = tcfgs[0].State;
            string lastBand = tcfgs[0].Band;

            while (tcfgs.Length > 0 && !IsAcceptedState(lastQ)) {
                foreach (var tcfg in tcfgs) {
                    Utils.DebugMessage(tcfg.ToString(), this, Utils.eDebugLogLevel.Verbose);
                    if (tcfg != null)
                        lastBand = tcfg.Band;
                    if (runs > MAX_TURING_RUNS)
                        throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
                    runs++;
                }
                tcfgs = GoChar(tcfgs);
            }
            return lastBand.Trim(BlankSymbol);
        }

        public static NTM1659 GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            var t = new NTM1659Transform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, inputAlphabet.Append(BLANK));


            int transformsRnd = rnd.Next(0, stateCount*inputAlphabet.Length);
            for (uint k = 0; k < transformsRnd; k++) {
                    var tk = new TuringTransformSingleBand.TuringKey((uint)rnd.Next(0, stateCount), Utils.GrAE(bandAlphabet));
                    var tv = new TuringTransformSingleBand.TuringVal((uint)rnd.Next(0, stateCount), Utils.GrAE(bandAlphabet), TMDirection.Right);
                    t.TryAdd(tk, new TuringTransformSingleBand.TuringVal[] {tv});
            }

            var ret = new NTM1659("NTM1_Random", (uint)stateCount, inputAlphabet, bandAlphabet, t, (uint)rnd.Next(0, stateCount), BLANK, (uint)rnd.Next(0, stateCount), (uint)rnd.Next(0, stateCount));
            ret.Name = $"TM1659_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();

            var newT = new NTM1659Transform();
            foreach (var ti in Transform)
                if (translate.Contains(ti.Key.q))
                    foreach (var tv in ti.Value) {
                        if (translate.Contains(tv.qNext)) {
                            newT.AddM(Utils.ArrayIndex(translate, ti.Key.q), ti.Key.c, Utils.ArrayIndex(translate, tv.qNext), tv.c2, tv.Direction);

                        }
                    }

            return new NTM1659($"{Name}_purged", (uint)names.Length, Alphabet, BandAlphabet, newT, Utils.ArrayIndex(translate, StartState), BlankSymbol, Utils.ArrayIndex(translate, AcceptedState), Utils.ArrayIndex(translate, DiscardState));
        }

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<System.Tuple<int, int, string>>();
            foreach (var t in Transform) {
                foreach (var tv in t.Value) {
                    var vt = new System.Tuple<int, int, string>((int)t.Key.q, (int)tv.qNext,
                    $"{t.Key.c}|{tv.c2} {tv.Direction}");
                    tcol.Add(vt);
                }
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {BlankSymbol}, {AcceptedState}, {DiscardState})".Trim();

    }

    public sealed class NTM1659Transform : TransformBase<TuringTransformSingleBand.TuringKey, TuringTransformSingleBand.TuringVal[]> {

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var ti in this) {
                foreach (var tv in ti.Value) {
                    sw.Append($"({ti.Key.ToString()})=>");
                    sw.Append($"({tv.ToString()}); ");
                }
                // sw.Append("); ");
            }
            return sw.ToString();
        }

        public void Add(uint q, char c, uint qNext, char c2, TMDirection dir) {
            Add(new TuringTransformSingleBand.TuringKey(q, c), new TuringTransformSingleBand.TuringVal[] { new TuringTransformSingleBand.TuringVal(qNext, c2, dir) });
        }

        internal void AddM(uint q, char c, uint qNext, char c2, TMDirection dir) {
            TuringTransformSingleBand.TuringVal[] qBefore;
            var eat = new TuringTransformSingleBand.TuringKey(q, c);
            if (TryGetValue(eat, out qBefore))
                this[eat] = qBefore.Append(new TuringTransformSingleBand.TuringVal(qNext, c2, dir)).ToArray();
            else
                Add(new TuringTransformSingleBand.TuringKey(q, c), new TuringTransformSingleBand.TuringVal[] { new TuringTransformSingleBand.TuringVal(qNext, c2, dir) });
        }

    }
}