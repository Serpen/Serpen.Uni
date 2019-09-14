namespace Serpen.Uni.Automat.Turing {
    public abstract class TuringMachineBase : AutomatBase<TuringKey, TuringVal> { //where TConfig : ITuringConfig {

        public const char BLANK = ' ';

        public char[] BandAlphabet;
        public char BlankSymbol;
        public new readonly TuringTransform Transform;
        public TuringMachineBase(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransform transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(stateCount, inputAlphabet, startState, name) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
            AcceptedStates = acceptedStates;
            Transform = transform;
        }
        public TuringMachineBase(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, TuringTransform transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(states, inputAlphabet, startState, name) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
            AcceptedStates = acceptedStates;
            Transform = transform;
        }

        protected const int MAX_TURING_RUNS = 10000;
        TuringConfigSingleBand GoChar(TuringConfigSingleBand tcfg) {
            TuringVal tva;
            if (Transform.TryGetValue(new TuringKey(tcfg.q, tcfg.CurSymbol), out tva)) {
                tcfg.ReplaceChar(tva.c2, tva.Direction);
                tcfg.q = tva.qNext;
                return tcfg;
            } else
                return null;
        }

        // public abstract string GetBandOutput(string w);

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

    }
    
    public sealed class TuringCycleException : Serpen.Uni.Exception {
        public TuringCycleException(string msg) : base(msg) { }
    }

    public enum TMDirection { Left, Right, Halt }

}