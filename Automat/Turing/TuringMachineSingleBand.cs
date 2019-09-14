using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineSingleBand : TuringMachineBase {

        public new readonly TuringTransformSingleBand Transform;

        public TuringMachineSingleBand(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transform = transform;
        }
        public TuringMachineSingleBand(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transform = transform;
        }

        TuringConfigSingleBand GoChar(TuringConfigSingleBand tcfg) {
            TuringTransformSingleBand.TuringVal tva;
            if (Transform.TryGetValue(new TuringTransformSingleBand.TuringKey(tcfg.q, tcfg.CurSymbol), out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.q = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        internal System.Collections.Generic.Dictionary<char,string> RealBandAlphabet;
        internal System.Collections.Generic.Dictionary<string,char> RealBandAlphabetRev;
        
        public override bool AcceptWord(string w) {
            if (RealBandAlphabet != null) {
                foreach (var a in RealBandAlphabet)
                    w = w.Replace(a.Key.ToString(), a.Value);
            }
            string way = "";
            w = w.Replace("0", RealBandAlphabetRev["B,0"].ToString());
            w = w.Replace("1", RealBandAlphabetRev["B,1"].ToString());
            w = w.Replace("c", RealBandAlphabetRev["B,c"].ToString());
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0) {q=StartState};
            int runs = 0;
            uint lastQ = tcfg.q;
            while (tcfg != null && !AcceptedStates.Contains(tcfg.q)) {
                way += $"({lastQ.ToString()}/{States[lastQ]}/{tcfg.Band}),";
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.q;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
                runs++;
            }
            if (AcceptedStates.Contains(lastQ))
                return true;
            else
                return false;
        }

        public string GetBandOutput(string w) {
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0);
            int runs = 0;
            string lastBand = tcfg.Band;
            while (tcfg != null && !AcceptedStates.Contains(tcfg.q)) {
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastBand = tcfg.Band;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
                runs++;
            }
            return lastBand.Trim(BlankSymbol);
        }

        public override System.Tuple<int, int, string>[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<System.Tuple<int, int, string>>();
            foreach (var t in Transform) {
                var vt = new System.Tuple<int, int, string>((int)t.Key.q, (int)t.Value.qNext, 
                 $"{RealBandAlphabet[t.Key.c]}|{RealBandAlphabet[t.Value.c2]} {t.Value.Direction}");
                 tcol.Add(vt);
            }
            return tcol.ToArray();
        }
        
        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {BlankSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

    }
}