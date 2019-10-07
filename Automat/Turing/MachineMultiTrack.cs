using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineMultiTrack : TuringMachineBase<TuringTransformMultiTrack.TuringKey, TuringTransformMultiTrack.TuringVal> {

        public readonly uint Tracks;

        public TuringMachineMultiTrack(string name, uint tracks, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transforms = transform;
            Tracks = tracks;
        }

        public TuringMachineMultiTrack(string name, uint tracks, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transforms = transform;
            Tracks = tracks;
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var ti in Transforms) {
                foreach (char c in ti.Key.c)
                    if (!BandAlphabet.Contains(c))
                        throw new Automat.AlphabetException(c);
                foreach (char c in ti.Value.c2)
                    if (!BandAlphabet.Contains(c))
                        throw new Automat.AlphabetException(c);

                if (Alphabet.Contains(BlankSymbol))
                    throw new Automat.AlphabetException(BlankSymbol);

                if (ti.Key.q >= StatesCount)
                    throw new Automat.StateException(ti.Key.q);
                if (ti.Value.qNext >= StatesCount)
                    throw new Automat.StateException(ti.Value.qNext);

            }
        }

        TuringConfigMultiTrack GoChar(TuringConfigMultiTrack tcfg) {
            TuringTransformMultiTrack.TuringVal tva;
            var tkey = new TuringTransformMultiTrack.TuringKey(tcfg.q, tcfg.CurSymbol[0..2]);
            if (Transforms.TryGetValue(tkey, out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.q = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            string[] wordTracks = new string[Tracks];
            wordTracks[0] = w;
            for (int i = 1; i < Tracks; i++)
                wordTracks[i] = new string(BlankSymbol, w.Length);
            Utils.DebugMessage($"w: {w}=>{string.Join(',', wordTracks)}", this, Utils.eDebugLogLevel.Verbose);

            var tcfg = new TuringConfigMultiTrack(BlankSymbol, wordTracks, 0) { q = StartState };

            int runs = 0;
            uint lastQ = tcfg.q;

            while (tcfg != null && !IsAcceptedState(tcfg.q)) {
                Utils.DebugMessage(tcfg.ToString(), this, Utils.eDebugLogLevel.Verbose);
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

        public override string GetBandOutput(string w) {
            string[] wordTracks = new string[Tracks];
            wordTracks[0] = w;
            for (int i = 1; i < Tracks; i++)
                wordTracks[i] = new string(BlankSymbol, w.Length);
            Utils.DebugMessage($"w: {w}=>{string.Join(',', wordTracks)}", this, Utils.eDebugLogLevel.Verbose);

            var tcfg = new TuringConfigMultiTrack(BlankSymbol, wordTracks, 0) { q = StartState };

            int runs = 0;
            string lastBand = tcfg.Band[0];
            while (tcfg != null && !IsAcceptedState(tcfg.q)) {
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastBand = tcfg.Band[0];
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {lastBand.Trim(BlankSymbol)}");
                runs++;
            }
            return lastBand.Trim(BlankSymbol);
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.removedStateTranslateTables();

            var newT = new TuringTransformMultiTrack(((TuringTransformMultiTrack)(Transforms)).StateTracks);
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext)) {
                        var tk = new TuringTransformMultiTrack.TuringKey(Utils.ArrayIndex(translate, t2.Key.q), t2.Key.c);
                        var tv = new TuringTransformMultiTrack.TuringVal(Utils.ArrayIndex(translate, t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
                        newT.Add(tk, tv);
                    }

            return new TuringMachineMultiTrack($"{Name}_purged", Tracks, names, Alphabet, BandAlphabet, newT, Utils.ArrayIndex(translate, StartState), BlankSymbol, aStates);
        }

        public static TuringMachineMultiTrack GenerateRandom() {
            var rnd = Utils.RND;

            int stateCount = rnd.Next(1, 20);
            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, 20);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, 20, inputAlphabet.Append(BLANK));
            uint trackCount = (uint)rnd.Next(1, 5);
            string[] TrackTranslate = new string[rnd.Next(1, 10)];
            for (int i = 0; i < TrackTranslate.Length; i++) {
                var curChars = new string[trackCount];
                for (int j = 0; j < trackCount; j++)
                    curChars[j] = ((char)(Utils.GrAE(bandAlphabet))).ToString();
                TrackTranslate[i] = string.Join(",", curChars);
            }
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);

            var t = new TuringTransformMultiTrack(TrackTranslate);
            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (uint j = 0; j < transformsRnd; j++) {
                    var tk = new TuringTransformMultiTrack.TuringKey(i, Utils.GrAE(TrackTranslate).ToCharArray());
                    var tv = new TuringTransformMultiTrack.TuringVal(j, Utils.GrAE(TrackTranslate).ToCharArray(), TMDirection.Right);
                    t.TryAdd(tk, tv);
                }
            }

            var ret = new TuringMachineMultiTrack($"TM{trackCount}_Random", trackCount, (uint)stateCount, inputAlphabet, bandAlphabet, t, (uint)rnd.Next(0, stateCount), BLANK, accState);
            ret.Name = $"TM{trackCount}_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>();
            foreach (var t in Transforms) {
                var vt = new VisualizationTuple(t.Key.q, t.Value.qNext,
                 $"{new string(t.Key.c)}|{new string(t.Value.c2)} {t.Value.Direction}");
                tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {BlankSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

    }
}