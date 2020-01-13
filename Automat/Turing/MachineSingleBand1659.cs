using System.Linq;

namespace Serpen.Uni.Automat.Turing {

    [System.Serializable]
    public class TuringMachineSingleBand1659 : TuringMachineBase<TuringKey, TuringVal>, IComplement {

        public static TuringMachineSingleBand1659 EMPTY = 
            new TuringMachineSingleBand1659("Empty", 1, KnownAutomat.binAlp, 
            KnownAutomat.binAlp.Append(BLANK).ToArray(), 
            new TuringTransformSingleBand(), 1, BLANK, 0, 0);
            
        public TuringMachineSingleBand1659(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] { acceptedState }) {
            Transforms = transform;
            DiscardState = discardState;
            CheckConstraints();
        }
        public TuringMachineSingleBand1659(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformSingleBand transform, uint startState, char blankSymbol, uint acceptedState, uint discardState)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, new uint[] { acceptedState }) {
            Transforms = transform;
            DiscardState = discardState;
            CheckConstraints();
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var t in Transforms) {
                if (t.Value.qNext >= StatesCount)
                    throw new StateException(t.Value.qNext, this);
                if (!BandAlphabet.Contains(t.Value.c2))
                    throw new AlphabetException(t.Value.c2, this);
            }
        }

        public uint AcceptedState => base.AcceptedStates[0];

        public uint DiscardState {get;}

        TuringConfigSingleBand GoChar(TuringConfigSingleBand tcfg) {
            if (Transforms.TryGetValue(new TuringKey(tcfg.State, tcfg.CurSymbol), out TuringVal tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.State = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0) { State = StartState };
            int runs = 0;
            uint lastQ = tcfg.State;
            while (tcfg != null) {
                Utils.DebugMessage(tcfg.ToString(), this, Uni.Utils.eDebugLogLevel.Verbose);
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.State;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}", this);
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
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Band.Trim(BlankSymbol)}", this);
                runs++;
            }
            return lastBand.Trim(BlankSymbol);
        }

        [AlgorithmSource("1659_4.6.1_P129")]
        public string ToBinString() {
            var sb = new System.Text.StringBuilder();

            const char S = '0'; // Separator
            const char C = '1'; // Counter

            var stateTranslate = new System.Collections.Generic.Dictionary<uint, uint>();
            for (uint i = 0; i < StatesCount; i++)
                stateTranslate.Add(i, i);

            if (stateTranslate[0] != StartState)
                (
                stateTranslate[0],
                stateTranslate[StartState]
                ) = (StartState, 0);

            if (stateTranslate[1] != AcceptedState)
                (
                stateTranslate[1],
                stateTranslate[AcceptedState]
                ) = (AcceptedState, 1);

            if (stateTranslate[2] != DiscardState)
                (
                stateTranslate[2],
                stateTranslate[DiscardState]
                ) = (DiscardState, 2);

            foreach (var t in Transforms) {
                sb.Append(
                    new string(C, (int)stateTranslate[t.Key.q] + 1) + S +
                    new string(C, (int)BandAlphabet.ArrayIndex(t.Key.c) + 1) + S +
                    new string(C, (int)stateTranslate[t.Value.qNext] + 1) + S +
                    new string(C, (int)BandAlphabet.ArrayIndex(t.Value.c2) + 1) + S +
                    new string(C, (int)t.Value.Direction - 1) +
                    S + S
                );
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        [AlgorithmSource("1659_4.6.1_P129")]
        public static TuringMachineSingleBand1659 FromBinString(string binString) {
            var transform = new TuringTransformSingleBand();

            const char S = '0'; // Separator
            const char C = '1'; // Counter

            const char SC = '0';

            int curPos = 0;

            char cmax = SC;
            uint qmax = 0;


            while (curPos < binString.Length) {
                uint q = 0;
                char c = SC;

                while (binString[curPos] == C) {
                    q++;
                    curPos++;
                }
                if (binString[curPos] != S)
                    throw new Serpen.Uni.Automat.Exception("Format", null);
                else
                    curPos++;

                while (binString[curPos] == C) {
                    c++;
                    curPos++;
                }
                if (binString[curPos] != S)
                    throw new Serpen.Uni.Automat.Exception("Format", null);
                else
                    curPos++;

                if (cmax < c) cmax = c;
                if (qmax < q) qmax = q;
                var tmkey = new TuringKey(q - 1, --c);

                uint qnext = 0;
                char c2 = SC;
                uint dir = 0;

                while (binString[curPos] == C) {
                    qnext++;
                    curPos++;
                }
                if (binString[curPos] != S)
                    throw new Serpen.Uni.Automat.Exception("Format", null);
                else
                    curPos++;

                while (binString[curPos] == C) {
                    c2++;
                    curPos++;
                }
                if (binString[curPos] != S)
                    throw new Serpen.Uni.Automat.Exception("Format", null);
                else
                    curPos++;

                while (curPos < binString.Length && binString[curPos] == C) {
                    dir++;
                    curPos++;
                }

                if (curPos < binString.Length && binString.Substring(curPos, 2) != new string(S,2))
                    throw new Serpen.Uni.Automat.Exception("Format", null);
                else
                    curPos += 2;

                if (cmax < c2) cmax = c2;
                if (qmax < qnext) qmax = qnext;
                var tmval = new TuringVal(qnext - 1, --c2, (TMDirection)(++dir));

                transform.Add(tmkey, tmval);
            }

            var alp = new char[cmax - SC];
            for (int i = 0; i < alp.Length; i++)
                alp[i] = (char)(SC + i);

            return new TuringMachineSingleBand1659("uTM", qmax, alp.SkipLast(1).ToArray(), alp, transform, 0, alp[^1], 1, 2);
        }

        public static TuringMachineSingleBand1659 GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            var t = new TuringTransformSingleBand();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, inputAlphabet.Append(BLANK));

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (uint j = 0; j < stateCount; j++) {
                    var tk = new TuringKey(i, bandAlphabet.RndElement());
                    var tv = new TuringVal(j, bandAlphabet.RndElement(), TMDirection.Right);
                    t.TryAdd(tk, tv);
                }
            }

            var ret = new TuringMachineSingleBand1659("TM1_Random", (uint)stateCount, inputAlphabet, bandAlphabet, t, (uint)rnd.Next(0, stateCount), BLANK, (uint)rnd.Next(0, stateCount), (uint)rnd.Next(0, stateCount));
            ret.Name = $"TM1659_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, _) = base.RemovedStateTranslateTables(AcceptedState, DiscardState); //BUG: dont remove ac, dis State

            var newT = new TuringTransformSingleBand();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext)) {
                        var tk = new TuringKey(translate.ArrayIndex(t2.Key.q), t2.Key.c);
                        var tv = new TuringVal(translate.ArrayIndex(t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
                        newT.Add(tk, tv);
                    }

            return new TuringMachineSingleBand1659($"{Name}_purged", (uint)names.Length, Alphabet, BandAlphabet, newT, translate.ArrayIndex(StartState), BlankSymbol, translate.ArrayIndex(AcceptedState), translate.ArrayIndex(DiscardState));
        }

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>(Transforms.Count);
            foreach (var t in Transforms) {
                var vt = new VisualizationTuple(t.Key.q, t.Value.qNext,
                 $"{t.Key.c}|{t.Value.c2} {t.Value.Direction}");
                tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {BlankSymbol}, {AcceptedState}, {DiscardState})".Trim();

        [AlgorithmSource("1659_S4.1")]
        public IAutomat Complement() {
            return new TuringMachineSingleBand1659($"co-{this.Name}", this.States, this.Alphabet, this.BandAlphabet, (TuringTransformSingleBand)this.Transforms, this.StartState, this.BlankSymbol, this.DiscardState, this.AcceptedState);
        }
    }
}