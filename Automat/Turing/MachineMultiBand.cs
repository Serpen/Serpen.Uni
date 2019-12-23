using System.Linq;

namespace Serpen.Uni.Automat.Turing {

    [System.Serializable]
    public class TuringMachineMultiBand : TuringMachineBase<TuringTransformMultiTrack.TuringKey, TuringTransformMultiTrack.TuringVal> {

        public readonly uint Bands;

        public TuringMachineMultiBand(string name, uint tracks, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transforms = transform;
            Bands = tracks;
        }

        public TuringMachineMultiBand(string name, uint tracks, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransformMultiTrack transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, bandAlphabet, startState, blankSymbol, acceptedStates) {
            Transforms = transform;
            Bands = tracks;
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            foreach (var ti in Transforms) {
                foreach (char c in ti.Key.c)
                    if (!BandAlphabet.Contains(c))
                        throw new Automat.AlphabetException(c, this);

                foreach (char c in ti.Value.c2)
                    if (!BandAlphabet.Contains(c))
                        throw new Automat.AlphabetException(c, this);

                if (Alphabet.Contains(BlankSymbol))
                    throw new Automat.AlphabetException(BlankSymbol, this);

                if (ti.Key.q >= StatesCount)
                    throw new Automat.StateException(ti.Key.q, this);

                if (ti.Value.qNext >= StatesCount)
                    throw new Automat.StateException(ti.Value.qNext, this);

            }
        }

        TuringConfigMultiBand GoChar(TuringConfigMultiBand tcfg) {
            var tkey = new TuringTransformMultiTrack.TuringKey(tcfg.State, tcfg.CurSymbol[0..(int)Bands]);

            if (Transforms.TryGetValue(tkey, out TuringTransformMultiTrack.TuringVal tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.State = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            string[] wordTracks = new string[Bands];
            wordTracks[0] = w;
            for (int i = 1; i < Bands; i++)
                wordTracks[i] = new string(BlankSymbol, w.Length);
            Utils.DebugMessage($"w: {w}=>{string.Join(',', wordTracks)}", this, Uni.Utils.eDebugLogLevel.Verbose);

            var tcfg = new TuringConfigMultiBand(BlankSymbol, wordTracks, new int[Bands]) { State = StartState };

            int runs = 0;
            uint lastQ = tcfg.State;

            while (tcfg != null && !IsAcceptedState(tcfg.State)) {
                Utils.DebugMessage(tcfg.ToString(), this, Uni.Utils.eDebugLogLevel.Verbose);
                tcfg = GoChar(tcfg);
                if (tcfg != null)
                    lastQ = tcfg.State;
                if (runs > MAX_TURING_RUNS)
                    throw new TuringCycleException($"possible Turing cycle at {runs} with {w} now is: {tcfg.Bands[0].Trim(BlankSymbol)}", this);
                runs++;
            }

            return IsAcceptedState(lastQ);
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.RemovedStateTranslateTables();

            var newT = new TuringTransformMultiTrack(((TuringTransformMultiTrack)Transforms).StateTracks);

            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    if (translate.Contains(t2.Value.qNext)) {
                        var tk = new TuringTransformMultiTrack.TuringKey(translate.ArrayIndex(t2.Key.q), t2.Key.c);
                        var tv = new TuringTransformMultiTrack.TuringVal(translate.ArrayIndex(t2.Value.qNext), t2.Value.c2, t2.Value.Direction);
                        newT.Add(tk, tv);
                    }

            return new TuringMachineMultiBand($"{Name}_purged", Bands, names, Alphabet, BandAlphabet, newT, translate.ArrayIndex(StartState), BlankSymbol, aStates);
        }

        public static TuringMachineMultiBand GenerateRandom() {
            var rnd = Uni.Utils.RND;

            int stateCount = rnd.Next(1, 20);
            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, 20);
            char[] bandAlphabet = RandomGenerator.RandomAlphabet(1, 20, inputAlphabet.Append(BLANK));

            uint trackCount = (uint)rnd.Next(1, 5);
            string[] TrackTranslate = new string[rnd.Next(1, 10)];

            for (int i = 0; i < TrackTranslate.Length; i++) {
                var curChars = new string[trackCount];
                for (int j = 0; j < trackCount; j++)
                    curChars[j] = ((char)(bandAlphabet.RndElement())).ToString();
                TrackTranslate[i] = string.Join(",", curChars);
            }

            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);

            var t = new TuringTransformMultiTrack(TrackTranslate);
            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (uint j = 0; j < transformsRnd; j++) {
                    var tk = new TuringTransformMultiTrack.TuringKey(i, TrackTranslate.RndElement().ToCharArray());
                    var tv = new TuringTransformMultiTrack.TuringVal(j, TrackTranslate.RndElement().ToCharArray(), TMDirection.Right);
                    t.TryAdd(tk, tv);
                }
            }

            var ret = new TuringMachineMultiBand($"TM{trackCount}_Random", trackCount, (uint)stateCount, inputAlphabet, bandAlphabet, t, (uint)rnd.Next(0, stateCount), BLANK, accState);
            ret.Name = $"TM{trackCount}_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override VisualizationTuple[] VisualizationLines() {
            var tcol = new System.Collections.Generic.List<VisualizationTuple>(Transforms.Count);
            foreach (var t in Transforms) {
                var vt = new VisualizationTuple(t.Key.q, t.Value.qNext,
                 $"{new string(t.Key.c)}|{new string(t.Value.c2)} {t.Value.Direction}");
                tcol.Add(vt);
            }
            return tcol.ToArray();
        }

        public override string ToString()
            => $"{Name} TM(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', BandAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {BlankSymbol}, {{{string.Join(',', AcceptedStates)}}})".Trim();

        public override string GetBandOutput(string w) {
            throw new System.NotImplementedException();
        }
    }
}