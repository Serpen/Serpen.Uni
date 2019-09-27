using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineSingleBand : TuringMachineBase<TuringTransformSingleBand.TuringKey, TuringTransformSingleBand.TuringVal> {

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

        public static TuringMachineSingleBand GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            var t = new TuringTransformSingleBand();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, inputAlphabet.Append(BLANK), 0);
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount/3, stateCount); 
            
            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (uint j = 0; j < transformsRnd; j++) {
                    var tk = new TuringTransformSingleBand.TuringKey(i,Utils.GrAE(bandAlphabet));
                    var tv = new TuringTransformSingleBand.TuringVal(j,Utils.GrAE(bandAlphabet),TMDirection.Right);
                    t.TryAdd(tk,tv);
                }
            }

            var ret = new TuringMachineSingleBand("TM1_Random", (uint)stateCount, inputAlphabet, bandAlphabet , t, (uint)rnd.Next(0,stateCount), BLANK , accState);
            ret.Name = $"TM1_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat RemoveUnreachable() {
            var newT = new TuringTransformSingleBand();

            bool[] fromStartReachable = base.reachableStates();

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
                        var tk = new TuringTransformSingleBand.TuringKey(Utils.ArrayIndex(translate,t2.Key.q), t2.Key.c);
                        var tv = new TuringTransformSingleBand.TuringVal(Utils.ArrayIndex(translate,t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
                        newT.Add(tk,tv);
                    }
            var astates = new System.Collections.Generic.List<uint>();
            foreach (var accept in AcceptedStates)
                if (translate.Contains(accept))
                    astates.Add(Utils.ArrayIndex(translate,accept));

            return new TuringMachineSingleBand($"{Name}_removed", (uint)names.Length, Alphabet, BandAlphabet, newT, Utils.ArrayIndex(translate,StartState), BlankSymbol , astates.ToArray());
        }
    }
}