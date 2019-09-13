namespace Serpen.Uni.Automat {
    public class TuringMachineBase : AutomatBase<TuringMachineBase.TuringKey, TuringMachineBase.TuringVal> {

        public const char BLANK = ' ';
        public TuringMachineBase(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, TuringTransform transform, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(stateCount,inputAlphabet, startState, name) {}

        public struct TuringKey {
            public TuringKey(uint qNext, char c, Direction dir) {
                this.q = qNext;
                this.c = c;
                this.Direction = dir;
            }
            public uint q {get;}
            public char c {get;}
            public Direction Direction {get;}
        }
        public struct TuringVal {
            public TuringVal(uint q, char c) {
                this.qNext=q;
                this.c=c;
            }
            public uint qNext;
            public char c;
        }
        public sealed class TuringConfig : IConfig {
            public uint q;
            public string band;
            public uint bandPos;

        }

        public enum Direction {Left, Right, None}

        public sealed class TuringTransform : TransformBase<TuringKey, TuringVal> {} 
        TuringConfig[] GoChar(TuringConfig tcfg) => throw new System.NotImplementedException();

        public override bool AcceptWord(string w) => throw new System.NotImplementedException();

        public override string ToString() => throw new System.NotImplementedException();

    }
}