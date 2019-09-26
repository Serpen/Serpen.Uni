using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineMultiTrack : TuringMachineBase<TuringTransformMultiTrack.TuringKey, TuringTransformMultiTrack.TuringVal> {

        public uint Tracks { get; }

        public TuringMachineMultiTrack(string name, uint tracks, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            // Transform = transform;
            Tracks = tracks;
        }

        public TuringMachineMultiTrack(string name, uint tracks, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            // Transform = transform;
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

        public override IAutomat RemoveUnreachable() {
            var newT = new TuringTransformMultiTrack(((TuringTransformMultiTrack)(Transform)).StateTracks);

            bool[] fromStartReachable = new bool[StatesCount];
            fromStartReachable[StartState] = true;
            bool foundnew = true;
            while (foundnew) {
                foundnew = false;
                foreach (var t in (from tr in Transform where fromStartReachable[tr.Key.q] select tr)) {
                    fromStartReachable[t.Value.qNext] = true;
                    foundnew = true;
                }
            }

            uint[] translate = new uint[(from fsr in fromStartReachable where fsr select fsr).Count()];
            for (uint i=0; i < translate.Length; i++) {
                uint j=i;
                if (i>0)
                    j = System.Math.Max(i, translate[i-1]+1);
                while (!fromStartReachable[j])
                    j++;
                translate[i] = j;
            }

            string[] names = new string[translate.Length];
            for (int i = 0; i < translate.Length; i++)
                names[i] = translate[i].ToString();

            if (Utils.ArrayIndex(translate,StartState) > 100) {
                Utils.DebugMessage("removed with high start state", this);
            }
                
            foreach (var t2 in Transform)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext)) {
                        var tk = new TuringTransformMultiTrack.TuringKey(Utils.ArrayIndex(translate,t2.Key.q), t2.Key.c);
                        var tv = new TuringTransformMultiTrack.TuringVal(Utils.ArrayIndex(translate,t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
                        newT.Add(tk,tv);
                    }
            var astates = new System.Collections.Generic.List<uint>();
            foreach (var accept in AcceptedStates)
                if (translate.Contains(accept))
                    astates.Add(Utils.ArrayIndex(translate,accept));

            return new TuringMachineMultiTrack($"{Name}_removed", Tracks, names, Alphabet, BandAlphabet, newT, Utils.ArrayIndex(translate,StartState), BlankSymbol , astates.ToArray());
        }

        public static TuringMachineMultiTrack GenerateRandom() {
            throw new System.NotImplementedException();
        }

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