using System.Linq;

namespace Serpen.Uni.Automat.Turing {

    [System.Serializable]
    public sealed class TuringTransformSingleBand : TransformBase<TuringKey, TuringVal> {

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this.OrderBy(a => a.Key.ToString())) {
                sw.Append($"({item.Key.ToString()})=>");
                sw.Append($"({item.Value.ToString()}); ");
                // sw.Append("); ");
            }
            return sw.ToString();
        }

        public void Add(uint q, char c, uint qNext, char c2, TMDirection dir) {
            Add(new TuringKey(q, c), new TuringVal(qNext, c2, dir));
        }

        public void AddByStateStore(string[] states, string q, char c, string qNext, char c2, TMDirection dir) {
            base.Add(new TuringKey(states.ArrayIndex(q), c), new TuringVal(states.ArrayIndex(qNext), c2, dir));
        }

        public void AddByStateStoreAndBand(string[] states, string[] band, char[] bandTrans, string q, string c, string qNext, string c2, TMDirection dir) {
            base.Add(
                new TuringKey(states.ArrayIndex(q), bandTrans[band.ArrayIndex(c)]),
                new TuringVal(states.ArrayIndex(qNext), bandTrans[band.ArrayIndex(c2)], dir)
            );
        }

        public void Insert(TuringTransformSingleBand transform, uint index) {
            foreach (var ti in transform) {
                var tk = new TuringKey(ti.Key.q + index, ti.Key.c);
                var tv = new TuringVal(ti.Value.qNext + index, ti.Value.c2, ti.Value.Direction);
                Add(tk, tv);

            }
        }

    }

    [System.Serializable]
    public struct TuringKey : ITransformKey {
        public TuringKey(uint q, char c) {
            this.q = q;
            this.c = c;
        }
        public uint q { get; }
        public char c { get; }

        char[] ITransformKey.c => new char[] { c };

        public override bool Equals(object obj) {
            if (obj is TuringKey tk) {
                return tk.q == this.q && tk.c == this.c;
            }
            return false;
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() => $"({q}, {c})";
    }

    [System.Serializable]
    public struct TuringVal : ITransformValue {
        public TuringVal(uint qNext, char c, TMDirection dir) {
            this.qNext = qNext;
            this.c2 = c;
            this.Direction = dir;
        }
        public uint qNext { get; }
        public char c2 { get; }
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