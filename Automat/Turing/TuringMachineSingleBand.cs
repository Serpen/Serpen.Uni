using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineSingleBand : TuringMachineBase {

        public TuringMachineSingleBand(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransform transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, bandAlphabet, transform, startState, blankSymbol, acceptedStates) {
        }
        public TuringMachineSingleBand(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransform transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, transform, startState, blankSymbol, acceptedStates) {
        }

        TuringConfigSingleBand GoChar(TuringConfigSingleBand tcfg) {
            TuringVal tva;
            if (Transform.TryGetValue(new TuringKey(tcfg.q, tcfg.CurSymbol), out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.q = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) {
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0) {q=StartState};
            int runs = 0;
            uint lastQ = tcfg.q;
            while (tcfg != null && !AcceptedStates.Contains(tcfg.q)) {
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
    }
}