
namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineMultiTrack : TuringMachineBase {

        public new readonly TuringTransformMultiTrack Transform;
        public uint Tracks { get; }

        public TuringMachineMultiTrack(string name, uint tracks, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transform = transform;
            Tracks = tracks;
        }

        TuringConfigMultiTrack GoChar(TuringConfigMultiTrack tcfg) {
            TuringTransformMultiTrack.TuringVal tva;
            var tkey = new TuringTransformMultiTrack.TuringKey(tcfg.q, tcfg.CurSymbol);
            if (Transform.TryGetValue(tkey, out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.q = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) {
            string[] newW = new string[Tracks];
            newW[0] = w;
            for (int i = 1; i < Tracks; i++)
                newW[i] = new string(BlankSymbol, w.Length);
            string way = "";
            Utils.DebugMessage($"w: {w}=>{string.Join(',', newW)}", this);
            var tcfg = new TuringConfigMultiTrack(BlankSymbol, newW, 0) { q = StartState };
            int runs = 0;
            uint lastQ = tcfg.q;
            while (tcfg != null && !IsAcceptedState(tcfg.q)) {
                Utils.DebugMessage(tcfg.ToString(), this);
                way += $"({lastQ.ToString()}/{States[lastQ]}/{tcfg.Band[0]}|{tcfg.Band[1]}/{tcfg.Position}),";
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.q;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band[0].Trim(BlankSymbol)}");
                runs++;
            }
            if (IsAcceptedState(lastQ))
                return true;
            else
                return false;
        }

        // public string GetBandOutput(string w) {
        //     var tcfg = new TuringConfigRealMultiTrack(BlankSymbol, w, 0);
        //     int runs = 0;
        //     string lastBand = tcfg.Band;
        //     while (tcfg != null && !IsAcceptedState(tcfg.q)) {
        //         tcfg = GoChar(tcfg);
        //         if (tcfg != null)
        //             lastBand = tcfg.Band;
        //         if (runs > MAX_TURING_RUNS)
        //             throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
        //         runs++;
        //     }
        //     return lastBand.Trim(BlankSymbol);
        // }

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<System.Tuple<int, int, string>>();
            foreach (var t in Transform) {
                var vt = new System.Tuple<int, int, string>((int)t.Key.q, (int)t.Value.qNext,
                 $"{new string(t.Key.c)}|{new string(t.Value.c2)} {t.Value.Direction}");
                tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {BlankSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

    }
}