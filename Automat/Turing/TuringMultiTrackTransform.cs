namespace Serpen.Uni.Automat.Turing {
    
    public sealed class TuringTransformMultiTrack : TransformBase<TuringTransformMultiTrack.TuringKey, TuringTransformMultiTrack.TuringVal> {

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this) {
                sw.Append($"({item.Key.ToString()})=>");
                sw.Append($"({item.Value.ToString()}); ");
                // sw.Append("); ");
            }
            return sw.ToString();
        }

        public void AddByStateStoreAndBand(string[] states, string q, char[] c, string qNext, char[] c2, TMDirection dir) {
            base.Add(
                new TuringKey(Utils.ArrayIndex(states, q), c),
                new TuringVal(Utils.ArrayIndex(states, qNext), c2, dir)
            );
        }
        public void AddByStateStoreAndBand(string[] states, string[] bandSymbols, string q, string c, string qNext, string c2, TMDirection dir) {
            string[] cSplit = c.Split(',');
            char[] c_ = new char[cSplit.Length];
            for (int i = 0; i < cSplit.Length; i++)
                c_[i] = cSplit[i][0];

            string[] c2Split = c2.Split(',');
            char[] c2_ = new char[c2Split.Length];
            for (int i = 0; i < c2Split.Length; i++)
                c2_[i] = c2Split[i][0];
            base.Add(
                new TuringKey(Utils.ArrayIndex(states, q), c_),
                new TuringVal(Utils.ArrayIndex(states, qNext), c2_, dir)
            );
        }

        public struct TuringKey {
            public TuringKey(uint q, char[] c) {
                this.q = q;
                this.c = c;
            }
            public uint q;
            public char[] c;

            public override bool Equals(object obj) {
                if (obj is TuringKey tk) {
                    return tk.q == this.q && tk.c == this.c;
                }
                return false;
            }

            public override int GetHashCode() => this.ToString().GetHashCode();

            public override string ToString() => $"({q}, {c})";
        }
        
        public struct TuringVal {
            public TuringVal(uint qNext, char[] c, TMDirection dir) {
                this.qNext = qNext;
                this.c2 = c;
                this.Direction = dir;
            }
            public uint qNext { get; }
            public char[] c2 { get; }
            public TMDirection Direction { get; }

            public override bool Equals(object obj) {
                if (obj is TuringVal tv) {
                    return tv.qNext == this.qNext && tv.c2 == this.c2 && tv.Direction == this.Direction;
                }
                return false;
            }

            public override int GetHashCode() => this.ToString().GetHashCode();

            public override string ToString() => $"({qNext}, {c2}, {(Direction)})";
        }
    }

}