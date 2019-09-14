using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public class TuringMachineBase : AutomatBase<TuringKey, TuringVal> {

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

        private const int MAX_TURING_RUNS = 10000;
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
            var tcfg = new TuringConfigSingleBand(BlankSymbol, w, 0);
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



    public abstract class TuringConfigBase<TQ, TBand, TPos> : IConfig {

        protected TuringConfigBase(char blankSymbol) {
            BlankSymbol = blankSymbol;
        }
        protected char BlankSymbol;
        public TQ q;
        public TBand Band;
        public TPos BandPos;

        public override bool Equals(object obj) {
            if (obj is TuringConfigBase<TQ, TBand, TPos> tcfg) {
                return tcfg.q.Equals(this.q) && tcfg.Band.Equals(this.Band) && tcfg.BandPos.Equals(this.BandPos);
            }
            return false;
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() => $"({q}, {Band}, {BandPos})";

    }

    public sealed class TuringConfigSingleBand : TuringConfigBase<uint, string, int> {

        public TuringConfigSingleBand(char blankSymbol, string band, int bandPos) : base(blankSymbol) {
            Band = band;
            BandPos = bandPos;
        }

        public char CurSymbol {
            get {
                if (BandPos >= Band.Length)
                    Band = Band.Insert(BandPos, BlankSymbol.ToString());
                return base.Band[base.BandPos];

            }
        }

        public void ReplaceChar(char newChar, TMDirection dir) {
            string s = Band.Remove(BandPos, 1);
            s = s.Insert(BandPos, newChar.ToString());
            if (dir == TMDirection.Left) {
                BandPos--;
                if (BandPos == -1) {
                    s = s.Insert(0, BlankSymbol.ToString());
                    BandPos = 0;
                }
            } else if (dir == TMDirection.Right) {
                BandPos++;
                if (BandPos > s.Length)
                    s = s.Insert(BandPos, BlankSymbol.ToString());
            }
            Band = s;
        }
    }
    public sealed class TuringConfigMultiTracks : TuringConfigBase<uint, string[], uint> {
        TuringConfigMultiTracks(char blankSymbol) : base(blankSymbol) { }
    }
    public sealed class TuringConfigMultiBands : TuringConfigBase<uint, string[], int[]> {
        TuringConfigMultiBands(char blankSymbol) : base(blankSymbol) { }
    }
    public sealed class TuringConfigStateStorage : TuringConfigBase<System.Tuple<uint, char>, string, int> {
        TuringConfigStateStorage(char blankSymbol) : base(blankSymbol) { }
    }

    public sealed class TuringCycleException : Serpen.Uni.Exception {
        public TuringCycleException(string msg) : base(msg) { }
    }

    public enum TMDirection { Left, Right, Halt }

}