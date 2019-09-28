using System.Linq;

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

        public uint AcceptedState => base.AcceptedStates[0];

        public readonly uint DiscardState;

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
            CheckWordInAlphabet(w);
            
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0) {State=StartState};
            int runs = 0;
            uint lastQ = tcfg.State;
            while (tcfg != null) {
                Utils.DebugMessage(tcfg.ToString(), this, Utils.eDebugLogLevel.Verbose);
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

        public override string GetBandOutput(string w) {
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

        public static TuringMachineSingleBand1659 GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            var t = new TuringTransformSingleBand();
            int stateCount = rnd.Next(1, MAX_STATES);
            
            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, inputAlphabet.Append(BLANK), 0);
                        
            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (uint j = 0; j < transformsRnd; j++) {
                    var tk = new TuringTransformSingleBand.TuringKey(i,Utils.GrAE(bandAlphabet));
                    var tv = new TuringTransformSingleBand.TuringVal(j,Utils.GrAE(bandAlphabet),TMDirection.Right);
                    t.TryAdd(tk,tv);
                }
            }

            var ret = new TuringMachineSingleBand1659("TM1_Random", (uint)stateCount, inputAlphabet, bandAlphabet , t, (uint)rnd.Next(0,stateCount), BLANK , (uint)rnd.Next(0, stateCount), (uint)rnd.Next(0, stateCount));
            ret.Name = $"TM1659_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();

            var newT = new TuringTransformSingleBand();
            foreach (var t2 in Transform)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext)) {
                        var tk = new TuringTransformSingleBand.TuringKey(Utils.ArrayIndex(translate,t2.Key.q), t2.Key.c);
                        var tv = new TuringTransformSingleBand.TuringVal(Utils.ArrayIndex(translate,t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
                        newT.Add(tk,tv);
                    }

            return new TuringMachineSingleBand1659($"{Name}_purged", (uint)names.Length, Alphabet, BandAlphabet, newT, Utils.ArrayIndex(translate,StartState), BlankSymbol , Utils.ArrayIndex(translate,AcceptedState), Utils.ArrayIndex(translate,DiscardState));
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