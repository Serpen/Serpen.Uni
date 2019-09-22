
namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineSingleBand1659 : TuringMachineBase<TuringTransformSingleBand.TuringKey, TuringTransformSingleBand.TuringVal> {

        public TuringMachineSingleBand1659(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] {acceptedState}) {
            Transform = transform;
            DiscardState = discardState;
        }
        public TuringMachineSingleBand1659(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] {acceptedState}) {
            Transform = transform;
            DiscardState = discardState;
        }

        public uint AcceptedState {
            get {
                return base.AcceptedStates[0];
            }
        }

        public uint DiscardState {get;}

        TuringConfigSingleBand GoChar(TuringConfigSingleBand tcfg) {
            TuringTransformSingleBand.TuringVal tva;
            if (Transform.TryGetValue(new TuringTransformSingleBand.TuringKey(tcfg.State, tcfg.CurSymbol), out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.State = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) {
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0) {State=StartState};
            int runs = 0;
            uint lastQ = tcfg.State;
            while (tcfg != null) {
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.State;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
                runs++;
                if (IsAcceptedState(lastQ))
                    return true;
                else if (DiscardState == lastQ)
                    return false;
            }
            return false;
        }

        public string GetBandOutput(string w) {
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0);
            int runs = 0;
            string lastBand = tcfg.Band;
            while (tcfg != null && !IsAcceptedState(tcfg.State)) {
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
                 $"{t.Key.c}|{t.Value.c2} {t.Value.Direction}");
                 tcol.Add(vt);
            }
            return tcol.ToArray();
        }
        
        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {BlankSymbol}, {AcceptedState}, {DiscardState})".Trim();

    }
}