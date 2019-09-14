namespace Serpen.Uni.Automat.Turing {
    public sealed class TuringTransform : TransformBase<TuringKey, TuringVal> {

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this) {
                sw.Append($"({item.Key.ToString()})=>");
                sw.Append($"({item.Value.ToString()}); ");
                // sw.Append("); ");
            }
            return sw.ToString();
        }
    } 
    
    public struct TuringKey {
        public TuringKey(uint q, char c) {
            this.q=q;
            this.c=c;
        }
        public uint q;
        public char c;

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
        public TuringVal(uint qNext, char c, TMDirection dir) {
            this.qNext = qNext;
            this.c2 = c;
            this.Direction = dir;
        }
        public uint qNext {get;}
        public char c2 {get;}
        public TMDirection Direction {get;}

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