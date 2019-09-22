
namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineSingleBand : TuringMachineBase {

        //public new readonly TuringTransformSingleBand Transform;

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
            if (Transform.TryGetValue(new TuringTransformSingleBand.TuringKey(tcfg.State, tcfg.CurSymbol), out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.State = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) => AcceptWord(w, true);
        public bool AcceptWord(string w, bool wordConsumed) {
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0) {State=StartState};
            int runs = 0;
            uint lastQ = tcfg.State;
            while (tcfg != null && (!IsAcceptedState(tcfg.State) || wordConsumed) && (!wordConsumed || tcfg.Band.Replace(BlankSymbol.ToString(), "") != "")) {
                Utils.DebugMessage(tcfg.ToString(), this);
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.State;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
                runs++;
            }
            if (tcfg != null && IsAcceptedState(lastQ) && (!wordConsumed || tcfg.Band.Replace(BlankSymbol.ToString(), "") == ""))
                return true;
            else
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
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transform.ToString()}}}, {StartState}, {BlankSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

        public static explicit operator TuringMachineSingleBand(Finite.DFA dfa) {
            var tt = new TuringTransformSingleBand();
            foreach (var dt in dfa.Transform) {
                var tkey = new TuringTransformSingleBand.TuringKey(dt.Key.q, dt.Key.c.Value);
                var tval = new TuringTransformSingleBand.TuringVal(dt.Value[0], BLANK, TMDirection.Right);
                tt.Add(tkey, tval);
            }
            var bandAlphabet = new System.Collections.Generic.List<char>(dfa.Alphabet.Length+1);
            bandAlphabet.AddRange(dfa.Alphabet);
            bandAlphabet.Add(BLANK);
            return new Turing.TuringMachineSingleBand($"TM_({dfa.Name})", dfa.States, dfa.Alphabet, bandAlphabet.ToArray(), tt, dfa.StartState, BLANK, dfa.AcceptedStates);
        }
    }
}