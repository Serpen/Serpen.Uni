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
            throw new System.NotImplementedException();
            
            // var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0);
            // int runs = 0;
            // string lastBand = tcfg.Band;
            // while (tcfg != null && !IsAcceptedState(tcfg.State)) {
            //     tcfg = GoChar(tcfg);
            //     if (tcfg != null)
            //         lastBand = tcfg.Band;
            //     if (runs > MAX_TURING_RUNS)
            //         throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
            //     runs++;
            // }
            // return lastBand.Trim(BlankSymbol);
        }

        public static NTM1659 GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            var t = new NTM1659Transform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, inputAlphabet.Append(BLANK), 0);

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (uint j = 0; j < transformsRnd; j++) {
                    var tk = new TuringTransformSingleBand.TuringKey(i, Utils.GrAE(bandAlphabet));
                    var tv = new TuringTransformSingleBand.TuringVal(j, Utils.GrAE(bandAlphabet), TMDirection.Right);
                    t.TryAdd(tk, new TuringTransformSingleBand.TuringVal[] {tv});
                }
            }

            var ret = new NTM1659("NTM1_Random", (uint)stateCount, inputAlphabet, bandAlphabet, t, (uint)rnd.Next(0, stateCount), BLANK, (uint)rnd.Next(0, stateCount), (uint)rnd.Next(0, stateCount));
            ret.Name = $"TM1659_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            throw new System.NotImplementedException();
            // (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();

            // var newT = new TuringTransformSingleBand();
            // foreach (var t2 in Transform)
            //     if (translate.Contains(t2.Key.q))
            //         if (translate.Contains(t2.Value.qNext)) {
            //             var tk = new TuringTransformSingleBand.TuringKey(Utils.ArrayIndex(translate, t2.Key.q), t2.Key.c);
            //             var tv = new TuringTransformSingleBand.TuringVal(Utils.ArrayIndex(translate, t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
            //             newT.Add(tk, tv);
            //         }

            // return new TuringMachineSingleBand1659($"{Name}_purged", (uint)names.Length, Alphabet, BandAlphabet, newT, Utils.ArrayIndex(translate, StartState), BlankSymbol, Utils.ArrayIndex(translate, AcceptedState), Utils.ArrayIndex(translate, DiscardState));
        }

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            // throw new System.NotImplementedException();
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