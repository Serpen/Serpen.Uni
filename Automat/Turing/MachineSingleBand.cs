using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineSingleBand : TuringMachineBase<TuringTransformSingleBand.TuringKey, TuringTransformSingleBand.TuringVal> {

        public TuringMachineSingleBand(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transforms = transform;

            CheckConstraints();
        }
        public TuringMachineSingleBand(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transforms = transform;

            CheckConstraints();
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var ti in Transforms) {
                if (!BandAlphabet.Contains(ti.Key.c))
                    throw new Automat.AlphabetException(ti.Key.c);
                if (Alphabet.Contains(BlankSymbol))
                    throw new Automat.AlphabetException(BlankSymbol);
                if (!BandAlphabet.Contains(ti.Value.c2))
                    throw new Automat.AlphabetException(ti.Value.c2);
                if (ti.Key.q >= StatesCount)
                    throw new Automat.StateException(ti.Key.q);
                if (ti.Value.qNext >= StatesCount)
                    throw new Automat.StateException(ti.Value.qNext);

            }
        }

        TuringConfigSingleBand GoChar(TuringConfigSingleBand tcfg) {
            TuringTransformSingleBand.TuringVal tva;
            var tkey = new TuringTransformSingleBand.TuringKey(tcfg.State, tcfg.CurSymbol);
            if (Transforms.TryGetValue(tkey, out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.State = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) => AcceptWord(w, true);
        public bool AcceptWord(string w, bool wordConsumed) {
            CheckWordInAlphabet(w);

            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0) { State = StartState };

            int runs = 0;
            uint lastQ = tcfg.State;

            while (tcfg != null && (!IsAcceptedState(tcfg.State) || wordConsumed) && (!wordConsumed || tcfg.Band.Replace(BlankSymbol.ToString(), "") != "")) {
                Utils.DebugMessage(tcfg.ToString(), this, Utils.eDebugLogLevel.Verbose);
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.State;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}");
                runs++;
            }
            if (tcfg != null && (!wordConsumed || tcfg.Band.Replace(BlankSymbol.ToString(), "") == ""))
                return true;
            else if (IsAcceptedState(lastQ))
                return true;
            else
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

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>();
            foreach (var t in Transforms) {
                var vt = new VisualizationTuple(t.Key.q, t.Value.qNext,
                 $"{t.Key.c}|{t.Value.c2} {t.Value.Direction}");
                tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {BlankSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

        public static explicit operator TuringMachineSingleBand(Finite.DFA dfa) {
            var tt = new TuringTransformSingleBand();
            foreach (var dt in dfa.Transforms) {
                var tkey = new TuringTransformSingleBand.TuringKey(dt.Key.q, dt.Key.c.Value);
                var tval = new TuringTransformSingleBand.TuringVal(dt.Value[0], BLANK, TMDirection.Right);
                tt.Add(tkey, tval);
            }
            var bandAlphabet = new System.Collections.Generic.List<char>(dfa.Alphabet.Length + 1);
            bandAlphabet.AddRange(dfa.Alphabet);
            bandAlphabet.Add(BLANK);
            return new Turing.TuringMachineSingleBand($"TM_({dfa.Name})", dfa.States, dfa.Alphabet, bandAlphabet.ToArray(), tt, dfa.StartState, BLANK, dfa.AcceptedStates);
        }

        public static TuringMachineSingleBand GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, inputAlphabet.Append(BLANK));
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);

            var t = new TuringTransformSingleBand();
            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (uint j = 0; j < transformsRnd; j++) {
                    var tk = new TuringTransformSingleBand.TuringKey(i, Utils.GrAE(bandAlphabet));
                    var tv = new TuringTransformSingleBand.TuringVal(j, Utils.GrAE(bandAlphabet), TMDirection.Right);
                    t.TryAdd(tk, tv);
                }
            }

            var ret = new TuringMachineSingleBand("TM1_Random", (uint)stateCount, inputAlphabet, bandAlphabet, t, (uint)rnd.Next(0, stateCount), BLANK, accState);
            ret.Name = $"TM1_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();

            var newT = new TuringTransformSingleBand();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext)) {
                        var tk = new TuringTransformSingleBand.TuringKey(Utils.ArrayIndex(translate, t2.Key.q), t2.Key.c);
                        var tv = new TuringTransformSingleBand.TuringVal(Utils.ArrayIndex(translate, t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
                        newT.Add(tk, tv);
                    }

            return new TuringMachineSingleBand($"{Name}_purged", (uint)names.Length, Alphabet, BandAlphabet, newT, Utils.ArrayIndex(translate, StartState), BlankSymbol, aStates);
        }
    }
}