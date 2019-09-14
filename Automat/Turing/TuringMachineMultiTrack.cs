using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineMultiTrack : TuringMachineBase {

        public new readonly TuringTransformMultiTrack Transform;

        public TuringMachineMultiTrack(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transform = transform;
            RealBandAlphabet = new System.Collections.Generic.Dictionary<char, string>();
            foreach (char ba in bandAlphabet)
                RealBandAlphabet.Add(ba, ba.ToString());
        }
        public TuringMachineMultiTrack(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transform = transform;
            RealBandAlphabet = new System.Collections.Generic.Dictionary<char, string>();
            foreach (char ba in bandAlphabet)
                RealBandAlphabet.Add(ba, ba.ToString());
        }

        TuringConfigMultiTracks GoChar(TuringConfigMultiTracks tcfg) {
            TuringTransformMultiTrack.TuringVal tva;
            if (Transform.TryGetValue(new TuringTransformMultiTrack.TuringKey(tcfg.q, tcfg.CurSymbol), out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.q = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) {
            string[] ws = new string[2];
            ws[0] = new string('B', w.Length);
            ws[1] = w;
            var tcfg = new TuringConfigMultiTracks(BlankSymbol, ws, 0) {q=StartState};
            int runs = 0;
            uint lastQ = tcfg.q;
            while (tcfg != null && !AcceptedStates.Contains(tcfg.q)) {
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.q;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band[0].Trim(BlankSymbol)}");
                runs++;
            }
            if (AcceptedStates.Contains(lastQ))
                return true;
            else
                return false;
        }

        public string GetBandOutput(string w) {
            var tcfg = new TuringConfigMultiTracks(BlankSymbol, new string[] {w}, 0);
            int runs = 0;
            string[] lastBand = tcfg.Band;
            while (tcfg != null && !AcceptedStates.Contains(tcfg.q)) {
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastBand = tcfg.Band;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band[0].Trim(BlankSymbol)}");
                runs++;
            }
            return lastBand[0].Trim(BlankSymbol);
        }

        internal System.Collections.Generic.Dictionary<char, string> RealBandAlphabet;


        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<System.Tuple<int, int, string>>();
            foreach (var t in Transform) {
                var vt = new System.Tuple<int, int, string>((int)t.Key.q, (int)t.Value.qNext, 
                 $"{t.Key.c[0]}|{t.Value.c2[0]} {t.Value.Direction}");
                 tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {BlankSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

    }
}